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
using System.Configuration;
using System.IO;
using System.Linq;
using com.commend.tools.PZE.Data;
using DevExpress.Xpo;


namespace com.commend.tools.PZE.Misc
{
    /// <summary>
    /// Summary description for BMDImporter
    /// </summary>
    public class BMDImporter
    {
        public BMDImporter()
        {
        }

        public void Import()
        {
            this.CreateDayEntryForExternalEmployees();
            List<BMDLog> newEntries = this.ReadBMDFile();
            this.UpdateDatabase(newEntries);
            XpoHelper.NotifyDirtyTables("DaySummary");
        }

        /// <summary>
        /// Initializes the file watcher.
        /// </summary>
        private void InitializeFileWatcher()
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["BMDFilePath"];
            watcher.Filter = ConfigurationManager.AppSettings["BMDLockFileName"];
            watcher.Created += this.OnLockFileCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void CreateDayEntryForExternalEmployees()
        {
            DateTime date = new DateTime(2015, 1, 1);
            var uof = XpoHelper.GetNewUnitOfWork();
            var oldDaySummaries = new XPCollection<DaySummary>(uof);
            var externalEmployees = new XPCollection<User>(uof).Where(u => u.ExternalStaff == true);
            while (date.Ticks <= DateTime.Today.Ticks)
            {
                externalEmployees.ToList()
                    .ForEach(
                    user =>
                    {
                        var existingBmdTime = new XPQuery<BMDTime>(uof).FirstOrDefault(b =>
                            b.User == user && b.LoggingDay == date);
                        if (existingBmdTime == null)
                        {
                            BMDTime bmd = new BMDTime(uof);
                            bmd.LoggingDay = date;
                            bmd.User = user;
                            bmd.Timestamp = DateTime.Now;
                            bmd.AttendenceMinutes = 600;
                            bmd.Save();
                        }
                        else if (date != DateTime.Today.Date)
                        {
                            existingBmdTime.AttendenceMinutes = oldDaySummaries.FirstOrDefault(d => d.UserID == user && d.LoggingDay == date).BookedTime;
                            existingBmdTime.Save();
                        }
                    });
                date += TimeSpan.FromDays(1);
            }
            uof.CommitChanges();
        }

        /// <summary>
        /// Internal data object used for updating the database.
        /// </summary>
        internal class BMDLog
        {
            public DateTime LoggingDay { get; set; }
            public int PersonalID { get; set; }
            public int WorkingMinutes { get; set; }
        }

        /// <summary>
        /// The lockfile is created before updating the BmdTimes file.
        /// It indicates that the BmdTimes file is locked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnLockFileCreated(object sender, FileSystemEventArgs e)
        {
            var watcher = (FileSystemWatcher)sender;
            watcher.Created -= this.OnLockFileCreated;
            watcher.Deleted += this.OnLockFileDeleted;
        }

        /// <summary>
        /// The lockfile is deleted after updating the BmdTimes file.
        /// It indicates that the BmdTimes file is unlocked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnLockFileDeleted(object sender, FileSystemEventArgs e)
        {
            var watcher = (FileSystemWatcher)sender;
            watcher.Created += this.OnLockFileCreated;
            watcher.Deleted -= this.OnLockFileDeleted;

            List<BMDLog> newEntries = this.ReadBMDFile();
            this.UpdateDatabase(newEntries);
        }

        /// <summary>
        /// Updates the database.
        /// </summary>
        /// <param name="bmdLogs">The BMD logs.</param>
        private void UpdateDatabase(List<BMDLog> bmdLogs)
        {
            var unitOfWork = XpoHelper.GetNewUnitOfWork();

            var allUsers = new XPCollection<User>(unitOfWork);
            var existingBmdEntries = new XPCollection<BMDTime>(unitOfWork);
            foreach (var entry in bmdLogs)
            {
                var user = allUsers.FirstOrDefault(x => x.PersonalId == entry.PersonalID);
                if (user == null)
                {
                    continue;
                }

                var actualBmdEntry = existingBmdEntries
                    .FirstOrDefault(x => (x.User.PersonalId == entry.PersonalID) && (x.LoggingDay == entry.LoggingDay));

                if (actualBmdEntry == null)
                {
                    actualBmdEntry = new BMDTime(unitOfWork);
                    actualBmdEntry.LoggingDay = entry.LoggingDay;
                    if (entry.LoggingDay == DateTime.Now.Date)
                    {
                        actualBmdEntry.AttendenceMinutes = 600;
                    }
                    else
                    {
                        actualBmdEntry.AttendenceMinutes = entry.WorkingMinutes;
                    }

                    actualBmdEntry.User = user;
                    actualBmdEntry.Timestamp = DateTime.Now;
                    actualBmdEntry.Save();
                    existingBmdEntries.Add(actualBmdEntry);
                }
                else if (actualBmdEntry != null &&
                    actualBmdEntry.LoggingDay == entry.LoggingDay &&
                    actualBmdEntry.AttendenceMinutes != entry.WorkingMinutes)
                {
                    if (actualBmdEntry.LoggingDay != DateTime.Today.Date)
                    {
                        actualBmdEntry.AttendenceMinutes = entry.WorkingMinutes;
                        actualBmdEntry.Timestamp = DateTime.Now;
                        actualBmdEntry.Save();
                    }
                }
                if (actualBmdEntry != null && actualBmdEntry.LoggingDay == DateTime.Today.Date)
                {
                    actualBmdEntry.AttendenceMinutes = 600;
                    actualBmdEntry.Timestamp = DateTime.Now;
                    actualBmdEntry.Save();
                }
            }

            unitOfWork.CommitChanges();
        }

        /// <summary>
        /// Reads the BmdTimes csv-File and stores data into a List.
        /// </summary>
        /// <returns>Returns a list with all read entries.</returns>
        private List<BMDLog> ReadBMDFile()
        {
            List<BMDLog> entries = new List<BMDLog>();

            int lineCount = 0;
            DateTime loggingDay = default(DateTime);
            int personalID = 0;
            int workingSeconds = 0;

            var bmdFilePathFolder = ConfigurationManager.AppSettings["BMDFilePath"];
            var bmdFileName = ConfigurationManager.AppSettings["BMDFileName"];

            if (!string.IsNullOrEmpty(bmdFilePathFolder) &&
                !string.IsNullOrEmpty(bmdFileName))
            {

                string bmdFilePath = Path.Combine(
                    bmdFilePathFolder,
                    bmdFileName);

                using (var reader = new StreamReader(bmdFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        lineCount++;
                        string line = reader.ReadLine();
                        string[] properties = line.Split(';');

                        // Parse date and store it into local variable
                        if (properties.Length == 5 &&
                            Int32.TryParse(properties[0], out personalID) &&    // Parse personalId and workingSeconds
                            DateTime.TryParse(properties[2], out loggingDay) &&
                            Int32.TryParse(properties[3], out workingSeconds))
                        {
                            BMDLog entry = new BMDLog();
                            entry.LoggingDay = loggingDay;
                            entry.PersonalID = personalID;
                            entry.WorkingMinutes = workingSeconds / 60;
                            entries.Add(entry);
                        }
                        else
                        {
                            throw new InvalidDataException(string.Format("Line {0}: syntax invalid", lineCount));
                            //TODO: ERROR HANDLING
                        }
                    }
                }
            }
            return entries;
        }
    }
}