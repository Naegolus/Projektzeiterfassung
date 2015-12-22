//Copyright © 2015 by Commend International GmbH.All rights reserved.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Affero General Public License, version 3,
//as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU Affero General Public License for more details.

//You should have received a copy of the GNU Affero General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc.TTConnection.TTFilters;
using com.commend.tools.PZE.TTSoapCgi;

namespace com.commend.tools.PZE.Misc.TTConnection
{
    /// <summary>
    /// Summary description for TTHelper
    /// </summary>
    public class TtHelper
    {
        long _cookie = -1;
        ttsoapcgi _svr;
        List<TtImportData> _ttImportDataEntries = null;

        /// <summary>
        /// Search in TestTrack for fixed and analysed defects of filterd defects
        /// </summary>
        /// <param name="filter"> chosen TestTrack filter </param>
        /// <returns> list of TestTrack events</returns>
        public IEnumerable<TtImportData> GetFormatedTtEntries(TtFilter filter)
        {
            string username = System.Configuration.ConfigurationManager.AppSettings["TTUser"];
            string password = System.Configuration.ConfigurationManager.AppSettings["TTUserPwd"];

            //Liste löschen oder neu erstellen
            if (_ttImportDataEntries != null)
            {
                _ttImportDataEntries.Clear();
            }
            else
            {
                _ttImportDataEntries = new List<TtImportData>();
            }

            try
            {
                _svr = new ttsoapcgi { Url = System.Configuration.ConfigurationManager.AppSettings["TTConnectionString"] };

                // Set the URL

                var project = new CProject
                                       {
                                           database =
                                               new CDatabase { name = System.Configuration.ConfigurationManager.AppSettings["TTProject"] },
                                           options = new CProjectDataOption[3]
                                       };

                project.options[0] = new CProjectDataOption { name = "TestTrack Pro" };
                project.options[1] = new CProjectDataOption { name = "TestTrack RM" };
                project.options[2] = new CProjectDataOption { name = "TestTrack TCM" };

                // Login.
                _cookie = _svr.ProjectLogon(project, username, password);

                // get filterd Defects
                _ttImportDataEntries.AddRange(GetFilterdDefects(filter));
                _ttImportDataEntries.AddRange(GetFilterdRequirements(filter));

                return _ttImportDataEntries; //TODO: Review mit flo - besser machen!!

            }
            catch (Exception)
            {
            }

            finally
            {
                if (_svr != null && _cookie != -1)
                    // When you're finished, log off.
                    _svr.DatabaseLogoffAsync(_cookie);
            }
            return new List<TtImportData>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<TtImportData> GetFormatedTtEntriesOfUser(TtFilter filter, User user)
        {
            var returnList = new List<TtImportData>();

            var formatedName = user.SurName.Trim() + ", " + user.ForName.Trim();

            IEnumerable<TtImportData> tmpList = GetFormatedTtEntries(filter);

            foreach (TtImportData entry in tmpList)
            {
                if (entry.Name.Contains(formatedName))
                {
                    returnList.Add(entry);
                }
            }
            return returnList;
        }


        enum RecordListColumns
        {
            RecordID,
            Projekt
        };


        /// <summary>
        ///
        /// </summary>
        /// <param name="ttNumber"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        private String makeNoteFromTtInfo(long ttNumber, String note)
        {
            return "TT" + ttNumber.ToString(CultureInfo.InvariantCulture) + ": " + note;
        }


        /// <summary>
        /// uses the open TT-connectiont to fetch the filterd defects
        /// </summary>
        /// <param name="filter">string value with filter name</param>
        /// <returns>list of filterd defects</returns>
        List<TtImportData> GetFilterdDefects(TtFilter filter)
        {
            // crate Column Table for Record ID
            var columns = new CTableColumn[2];

            columns[(int)RecordListColumns.RecordID] = new CTableColumn { name = "Record ID" };
            columns[(int)RecordListColumns.Projekt] = new CTableColumn { name = "Projekt" };

            // fetch List of filterd Defects passed the Filter
            CRecordListSoap list = _svr.getRecordListForTable(_cookie, "Defect", filter.getFilterName(), columns);

            // store Data
            var defs = new List<TtImportData>();

            // if record list is empty
            if (list != null)
            {
                foreach (CRecordRowSoap row in list.records)
                {
                    var recordID = (long)Convert.ToDecimal(row.row[(int)RecordListColumns.RecordID].value);
                    var def = _svr.getDefectByRecordID(_cookie, recordID, false);

                    // Create a NumberFormatInfo object and set several of its
                    // properties that apply to numbers.
                    var provider = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };

                    DateTime stDate = filter.StartDate;
                    DateTime endDate = filter.EndDate;

                    foreach (CEvent ttEvent in def.eventlist)
                    {
                        // Eintrag nur Anlegen wenn es ein Fix-, Analyse-, EnterWork oder Resolve-Event ist der im ausgewählten Zeitraum gebucht wurde
                        if ((ttEvent.name.Equals("Fix") || ttEvent.name.Equals("Analyse") || ttEvent.name.Equals("Enter Work") || ttEvent.name.Equals("Resolve")) &&
                            (ttEvent.date >= stDate && ttEvent.date < endDate))
                        {
                            var entry = new TtImportData
                                            {
                                                Nr =
                                                    (long)
                                                    Convert.ToDecimal(
                                                        row.row[(int)RecordListColumns.RecordID].value.ToString(
                                                            CultureInfo.InvariantCulture)),
                                                Project = row.row[(int)RecordListColumns.Projekt].value,
                                                Date = ttEvent.date,
                                                Name = ttEvent.user,
                                                Effort = ttEvent.hours,
                                                Note = makeNoteFromTtInfo(def.defectnumber, def.summary) + '\n' + ttEvent.notes
                                            };

                            defs.Add(entry);
                        }
                    }
                }
            }
            return defs;
        }

        /// <summary>
        /// uses the open TT-connectiont to fetch the filterd requirements
        /// </summary>
        /// <param name="filter">string value with filter name</param>
        /// <returns>list of filterd defects</returns>
        List<TtImportData> GetFilterdRequirements(TtFilter filter)
        {
            // crate Column Table for Record ID
            var columns = new CTableColumn[2];

            columns[(int)RecordListColumns.RecordID] = new CTableColumn { name = "Record ID" };
            columns[(int)RecordListColumns.Projekt] = new CTableColumn { name = "Product" };

            // fetch List of filterd Defects passed the Filter
            CRecordListSoap list = _svr.getRecordListForTable(_cookie, "Requirement", filter.getFilterName(TtFilterType.requirement), columns);

            // store Data
            var defs = new List<TtImportData>();

            // if record list is empty
            if (list != null)
            {
                foreach (CRecordRowSoap row in list.records)
                {
                    var recordID = (long)Convert.ToDecimal(row.row[(int)RecordListColumns.RecordID].value);
                    var req = _svr.getRequirementByRecordID(_cookie, recordID, false);

                    // Create a NumberFormatInfo object and set several of its
                    // properties that apply to numbers.
                    var provider = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };

                    DateTime stDate = filter.StartDate;
                    DateTime endDate = filter.EndDate;

                    foreach (CEvent ttEvent in req.eventList)
                    {
                        if (ttEvent.name.Equals("Enter Work") &&
                            (ttEvent.date >= stDate && ttEvent.date < endDate))
                        {
                            var entry = new TtImportData
                            {
                                Nr =
                                    (long)
                                    Convert.ToDecimal(
                                        row.row[(int)RecordListColumns.RecordID].value.ToString(
                                            CultureInfo.InvariantCulture)),
                                Project = row.row[(int)RecordListColumns.Projekt].value,
                                Date = ttEvent.date,
                                Name = ttEvent.user,
                                Effort = ttEvent.hours,
                                Note = req.tag + ": " + req.summary + '\n' + ttEvent.notes
                            };

                            defs.Add(entry);
                        }
                    }
                }
            }
            return defs;
        }
    }
}