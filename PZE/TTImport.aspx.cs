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
using System.Linq;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc;
using com.commend.tools.PZE.Misc.TTConnection;
using com.commend.tools.PZE.Misc.TTConnection.TTFilters;
using com.commend.tools.PZE.View;
using DevExpress.Web;
using DevExpress.Xpo;


namespace com.commend.tools.PZE
{

    public partial class TtImport : ExtendedPage
    {
        List<TtImportData> _data;
        private TtHelper _ttHelper;
        private TtFilter _filter;

        /// <summary>
        /// initialise data
        /// </summary>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.CurrentUser != null)
            {
                this.SetupDataSource();
            }
        }

        /// <summary>
        /// handles session
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            RecordGrid.SettingsPager.PageSize = int.MaxValue;

            if (CurrentUser == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                //nur bei neu laden der Seite Daten nicht wider herstellen
                if (IsPostBack)
                {
                    _ttHelper = (TtHelper)Session["TTHelterInstance"];
                    _data = (List<TtImportData>)Session["TTImportData"];
                    _filter = (TtFilter)Session["TTFilter"];
                }

                var defaultProjektID = Session["DefaultProjectID"];

                if (IsCallback || (!IsCallback && !IsPostBack))
                {
                    RecordGrid.DataSource = _data;
                    RecordGrid.DataBind();

                    CmbDefaultActivity.Value = Session["DefaultAktivityID"];
                    CmbDefaultProject.Value = defaultProjektID;
                    CmbDefaultPSPCode.Value = Session["DefaultPspCodeID"];
                }

                if (!IsPostBack)
                {
                    if (defaultProjektID != null)
                    {
                        FillPsPCodeCombo((int)defaultProjektID);
                    }
                    else
                    {
                        FillPsPCodeCombo(0);
                    }
                }

                if (_ttHelper == null || _data == null || _filter == null)
                {
                    _ttHelper = new TtHelper();
                    _filter = new FilterToday();
                    _data = _ttHelper.GetFormatedTtEntriesOfUser(_filter, CurrentUser);

                    Session["TTImportData"] = _data;
                    Session["TTHelterInstance"] = _ttHelper;
                    Session["TTFilter"] = _filter;
                    RecordGrid.DataSource = _data;
                    RecordGrid.DataBind();
                }
                FillDateLabes();
            }
        }


        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.UserXpoData.Session = this.DataSession;
            this.ProjectsXpoData.Session = this.DataSession;
            this.PsPCodeXpoData.Session = this.DataSession;
            this.ActivitiesXpoData.Session = this.DataSession;

            this.ProjectsXpoData.Criteria = "Status.StatusName = ?";
            this.ProjectsXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            this.UserXpoData.DefaultSorting = "PersonalId ASC";
            this.ProjectsXpoData.DefaultSorting = "ProjectNumber ASC";
            this.PsPCodeXpoData.DefaultSorting = "PspCodeNumber ASC";
            this.ActivitiesXpoData.DefaultSorting = "ActivityNumber ASC";
        }

        /// <summary>
        /// reads data from Grid and saves it to the database
        /// </summary>
        protected void Import(object sender, EventArgs e)
        {
            var projects = new XPCollection<Projects>(this.DataSession);
            var pspcodes = new XPCollection<PspCodes>(this.DataSession);
            var activities = new XPCollection<Activities>(this.DataSession);

            var dateColumn = RecordGrid.Columns["Date"] as GridViewDataColumn;
            var effortColumn = RecordGrid.Columns["EffortTime"] as GridViewDataColumn;
            var activityColumn = RecordGrid.Columns["Activity"] as GridViewDataColumn;

            for (int i = 0; i < RecordGrid.VisibleRowCount; i++)
            {
                long id = Convert.ToInt64(RecordGrid.GetRowValues(i, "Id"));

                if (RecordGrid.Selection.IsRowSelectedByKey(id))
                {
                    var record = new Records(this.DataSession) { Timestamp = DateTime.Now, User = this.CurrentUser };
                    // Date
                    var deDate = (ASPxDateEdit)RecordGrid.FindRowCellTemplateControl(i, dateColumn, "deDate");
                    record.Date = deDate.Date.Add(-deDate.Date.TimeOfDay);

                    // check if Date is already locked
                    var day = this.DataSession.Query<DaySummary>().Where(x => (x.UserID == this.CurrentUser) && (x.LoggingDay == record.Date)).SingleOrDefault();

                    if (day != null && (day.DayStatus == (int)DayStatus1.MALocked || day.DayStatus == (int)DayStatus1.VLocked))
                    {
                        ClientScript.RegisterClientScriptBlock(
                            this.GetType(),
                            "validation",
                            "alert('Dieser Tag wurde bereits freigegeben und kann daher nicht mehr bearbeitet werden!');",
                            true);

                        return;
                    }


                    // Effort
                    var seEffort = (ASPxTimeEdit)RecordGrid.FindRowCellTemplateControl(i, effortColumn, "seEffort");
                    if (seEffort.Value != null)
                    {
                        record.Duration = (int)(((DateTime)seEffort.Value).TimeOfDay).TotalMinutes;
                    }
                    else
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Zeitdauer beinhalten!');", true);
                        return;
                    }
                    //Projekt
                    if (CmbDefaultProject.Value != null)
                    {
                        record.Project = projects.SingleOrDefault(t => t.Oid == (int)CmbDefaultProject.Value);
                    }
                    else
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss einem Projekt zugeordnet sein!');", true);
                        return;
                    }
                    // PspCode
                    if (CmbDefaultPSPCode.Value != null)
                    {
                        record.PspCode = pspcodes.SingleOrDefault(t => t.Oid == (int)CmbDefaultPSPCode.Value);
                    }
                    else
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss einem PSP-Code zugeordnet sein!');", true);
                        return;
                    }
                    // Note
                    var noteTextbox = (ASPxMemo)RecordGrid.FindPreviewRowTemplateControl(i, "tbxNote");
                    if (string.IsNullOrWhiteSpace(noteTextbox.Text))
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Notiz beinhalten!');", true);
                        return;
                    }
                    else
                    {
                        record.Memo = noteTextbox.Text;
                    }

                    // Activity
                    var cmbActivity = (ASPxComboBox)RecordGrid.FindRowCellTemplateControl(i, activityColumn, "CmbActivity");
                    if (cmbActivity.Value != null)
                    {
                        record.Activity = activities.SingleOrDefault(t => t.Oid == (int)cmbActivity.Value);
                    }
                    else if (CmbDefaultActivity.Value != null)
                    {
                        record.Activity = activities.SingleOrDefault(t => t.Oid == (int)CmbDefaultActivity.Value);
                    }
                    else
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Tätigkeit beinhalten!');", true);
                        return;
                    }

                    record.Save();
                    DataSession.Save(record);
                }
            }

            projects.Dispose();
            activities.Dispose();
            XpoHelper.NotifyDirtyTables("DaySummary");
            Response.Redirect("~/UserRecords.aspx");
        }

        /// <summary>
        /// Funktion for choose button
        /// </summary>
        protected void FetchTtDataButtonClick(object sender, EventArgs e)
        {
            if (FetchTTDataTodayButton == sender)
            {
                _filter = new FilterToday();
            }
            else if (FetchTTDataYesterdayButton == sender)
            {
                _filter = new FilterYesterday();
            }
            else if (FetchTTDataLastWeekButton == sender)
            {
                _filter = new FilterLastWeek();
            }
            else if (FetchTTDataThisWeekButton == sender)
            {
                _filter = new FilterThisWeek();
            }
            else if (FetchTTDataLastMonthButton == sender)
            {
                _filter = new FilterLastMonth();
            }
            else if (FetchTTDataThisMonthButton == sender)
            {
                _filter = new FilterThisMonth();
            }

            if (_filter != null)
            {
                _data = _ttHelper.GetFormatedTtEntriesOfUser(_filter, CurrentUser);
            }

            RecordGrid.DataSource = _data;
            RecordGrid.DataBind();

            FillDateLabes();

            Session["TTImportData"] = _data;
            Session["TTFilter"] = _filter;
        }

        /// <summary>
        /// fills the date lable with start and end date of current chosen filter
        /// </summary>
        private void FillDateLabes()
        {
            DateTime stDate = _filter.StartDate;
            DateTime endDate = _filter.EndDate;

            lbDateFrom.Text = stDate.ToString(@"d.M.yyyy");
            endDate = endDate.AddSeconds(-1);
            lbDateTo.Text = endDate.ToString(@"d.MM.yyyy");
        }

        protected void OnCmbDefaultActivity_Callback(object sender, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FilterActivity(value);
            CmbDefaultActivity.DataBind();
        }

        private void FilterActivity(int oid)
        {
            this.ActivitiesXpoData.Criteria = "Projects[Oid = ?]";
            this.ActivitiesXpoData.CriteriaParameters.Add("Oid", oid.ToString());
        }

        protected void OnCmbActivity_Callback(object sender, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FilterActivity(value);

            var comboBox = sender as ASPxComboBox;
            comboBox.DataBind();
        }

        /// <summary>
        /// generates a DateTime object of given houres
        /// </summary>
        /// <param name="totalHoures"> double houres </param>
        /// <returns> DateTime oject of given houres </returns>
        protected DateTime GetDateTimeFromHours(object totalHours)
        {
            double hour;
            double.TryParse(totalHours.ToString(), out hour);
            DateTime dt = new DateTime().Add(TimeSpan.FromHours(hour));
            return dt;
        }

        protected void OnCmbDefaultProject_ValueChanged(object sender, EventArgs e)
        {
            Session["DefaultProjectID"] = ((ASPxComboBox)sender).Value;
        }

        protected void OnCmbDefaultPspCode_ValueChanged(object sender, EventArgs e)
        {
            Session["DefaultPspCodeID"] = ((ASPxComboBox)sender).Value;
        }

        protected void OnCmbDefaultActivity_ValueChanged(object sender, EventArgs e)
        {
            Session["DefaultAktivityID"] = ((ASPxComboBox)sender).Value;
        }

        protected void OnCmbDefaultPSPCode_Callback(object sender, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillPsPCodeCombo(value);
        }

        protected void FillPsPCodeCombo(int oid)
        {
            PsPCodeXpoData.Criteria = "Projects.Oid = ? AND Status.StatusName=?";
            PsPCodeXpoData.CriteriaParameters.Add("Projects.Oid", oid.ToString());
            PsPCodeXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            CmbDefaultPSPCode.DataBind();
        }
    }
}