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

namespace com.commend.tools.PZE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.Misc;
    using DevExpress.Web;
    using DevExpress.Xpo;

    public partial class History : System.Web.UI.UserControl
    {
        public int PageSize { get; set; }

        public User SelectedUser { get; set; }

        public int CurrentUserOid { get; set; }

        public bool EmployeeLockView { get; set; }

        public bool LeaderLockView { get; set; }

        public Session DataSession { get; set; }

        public CalendarSelection SelectedDates { get; set; }

        public XPCollection<Projects> Projects { get; set; }

        public XPQuery<Records> Records { get; set; }

        override protected void OnInit(EventArgs e)
        {
            this.Init += new System.EventHandler(this.Page_Init);
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.DataSession = XpoHelper.GetNewSession();
            this.ProjectsXpoData.Session = this.DataSession;
            this.ActivitiesXpoData.Session = this.DataSession;
            this.HryCmbPsPCodeXpoData.Session = this.DataSession;

            this.ProjectsXpoData.Criteria = "Status.StatusName = ?";
            this.ProjectsXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            this.ProjectsXpoData.DefaultSorting = "ProjectNumber ASC";
            this.ActivitiesXpoData.DefaultSorting = "ActivityNumber ASC";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DataSession == null)
            {
                return;
            }

            this.SetupGrid();

            if (this.LeaderLockView)
            {
                this.HistoryGrid.Columns["EmployeeLocked"].Visible = true;
                this.HistoryGrid.Columns["LeaderLocked"].Visible = true;
            }
            else if (this.EmployeeLockView)
            {
                this.HistoryGrid.Columns["EmployeeLocked"].Visible = true;
                this.HistoryGrid.Columns["LeaderLocked"].Visible = false;
            }
            else
            {
                this.HistoryGrid.Columns["EmployeeLocked"].Visible = false;
                this.HistoryGrid.Columns["LeaderLocked"].Visible = false;
            }

            if (!this.LeaderLockView && !this.EmployeeLockView)
            {
                this.HistoryGrid.Columns[0].Visible = true;
            }
        }

        private void SetupGrid()
        {
            this.HistoryGrid.SettingsPager.PageSize = this.PageSize;

            IEnumerable<Records> recordsToShow = this.Records.Where(r => r.User == this.SelectedUser);
            CalendarSelection dates = this.SelectedDates;

            if (this.LeaderLockView)
            {
                recordsToShow = recordsToShow.Where(r => r.EmployeeLocked != null);
            }

            if (dates == null || dates.Count == 0)
            {
                recordsToShow = recordsToShow.Where(r => r.Date == DateTime.Today);
            }
            else if (dates.Count == 1)
            {
                recordsToShow = recordsToShow.Where(r => r.Date == dates.First());
            }
            else if (dates.Count > 1)
            {
                var firstDate = this.SelectedDates.First();
                var lastDate = this.SelectedDates.Last().AddDays(1.0);
                recordsToShow = recordsToShow.Where(r => firstDate.Ticks <= r.Date.Ticks && r.Date.Ticks < lastDate.Date.Ticks);
            }

            this.HistoryGrid.DataSource = recordsToShow;
            this.HistoryGrid.DataBind();
        }

        protected string GetProjectNameByOid(object selProject)
        {
            if (selProject != null)
            {
                int oid = (int)selProject;
                var project = this.Projects.FirstOrDefault<Projects>(x => x.Oid == oid);
                return project.ProjectName;
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetPsPCodeNameByOid(object selProject, object selPspCode)
        {
            if (selProject != null && selPspCode != null)
            {
                int projectOid = (int)selProject;
                int pspOid = (int)selPspCode;
                var pspCode = this.Projects.First(proj => proj.Oid == projectOid).PspCodes.FirstOrDefault(psp => psp.Oid == pspOid);
                return pspCode.PspCodeName;
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetActivityNumberByOid(object activity)
        {
            if (activity.ToString().Equals("0"))
            {
                return string.Empty;
            }
            else
            {
                return activity.ToString();
            }
        }

        protected string GetActivityNameByOid(object selProject, object selActivity)
        {
            if (selProject != null && selActivity != null)
            {
                int projectOid = (int)selProject;
                int activityOid = (int)selActivity;
                var activity = this.Projects.First(proj => proj.Oid == projectOid).Activities.First(act => act.Oid == activityOid);

                return activity.ActivityName;
            }
            else
            {
                return string.Empty;
            }
        }

        private bool Timecorrect(DaySummary day, DateTime givenDate, int minutes, int oldMinutes)
        {
            // Return zero if totalMinutes are incorrect or illegal.
            if (minutes == 0)
            {
                throw new Exception("Buchung muss eine Zeitdauer beinhalten!");
            }

            if (minutes < 0)
            {
                throw new Exception("Die Startzeit darf nicht größer als die Endzeit sein! Bzw. Datumsübergreifende Buchungen sind nicht zulässig!");
            }

            int maxTime = 600;
            int currentlyBookedTime = day.BookedTime;

            if ((minutes + currentlyBookedTime - oldMinutes) > maxTime)
            {
                throw new Exception("Sie können an einem Tag nicht mehr als 10h buchen!");
            }

            return true;
        }

        protected void HistoryGridRowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            Records record = this.Records.First(x => x.Oid == Convert.ToInt32(e.Keys["Oid"]));

            GridViewDataColumn column = HistoryGrid.Columns["Date"] as GridViewDataColumn;
            DateTime givenDate = ((ASPxDateEdit)HistoryGrid.FindEditRowCellTemplateControl(column, "DayEdit")).Date;

            if (givenDate.Ticks > DateTime.Now.Ticks)
            {
                throw new Exception("Sie können keine Buchung in der Zukunft tätigen!");
            }

            ASPxTimeEdit hryTimeEdit = (ASPxTimeEdit)(HistoryGrid.FindEditRowCellTemplateControl(null, "HryTimeEdit"));
            int totalMin = hryTimeEdit.DateTime.Hour * 60 + hryTimeEdit.DateTime.Minute;
            int oldMinutes = record.Date == givenDate ? record.Duration : 0;

            var external = this.DataSession.Query<User>().Single(u => u.Oid == this.SelectedUser.Oid).ExternalStaff;
            if (!external)
            {
                var day = this.DataSession.Query<DaySummary>()
                    .Where(x => (x.UserID.Oid == this.SelectedUser.Oid) && (x.LoggingDay == givenDate))
                    .SingleOrDefault();

                if (day == null)
                {
                    throw new Exception("Die Datenbank ist fehlerhaft. Bitte kontaktieren Sie den Systemadministrator!");
                }

                if (day.DayStatus == (int)DayStatus1.MALocked || day.DayStatus == (int)DayStatus1.VLocked)
                {
                    throw new Exception("Dieser Tag wurde bereits freigegeben. Das gewählte Datum ist also ungültig");
                }

                this.Timecorrect(day, givenDate, totalMin, oldMinutes);
            }

            ASPxComboBox hryCmbProject = (ASPxComboBox)(HistoryGrid.FindEditRowCellTemplateControl(null, "HryCmbProject"));
            ASPxComboBox hryCmbPspCode = (ASPxComboBox)(HistoryGrid.FindEditRowCellTemplateControl(null, "HryCmbPspCode"));
            ASPxComboBox hryCmbActivity = (ASPxComboBox)(HistoryGrid.FindEditRowCellTemplateControl(null, "HryCmbActivity"));

            record.Duration = totalMin;
            record.Date = givenDate;

            int projectOid = Convert.ToInt32(hryCmbProject.SelectedItem.Value);
            record.Project = this.Projects.First(p => p.Oid == projectOid);
            if (hryCmbPspCode.SelectedItem != null)
            {
                record.PspCode = record.Project.PspCodes.First(p => p.Oid == Convert.ToInt32(hryCmbPspCode.SelectedItem.Value));
            }
            else
            {
                e.Cancel = true;
                throw new Exception("Ihre Buchung muss einen PSP-Code beinhalten!");
            }

            if (hryCmbActivity.SelectedItem != null)
            {
                record.Activity = record.Project.Activities.First(p => p.Oid == Convert.ToInt32(hryCmbActivity.SelectedItem.Value));
            }
            else
            {
                e.Cancel = true;
                throw new Exception("Ihre Buchung muss eine Tätigkeit beinhalten!");
            }

            string memo = e.NewValues["Memo"] != null ? e.NewValues["Memo"].ToString() : null;
            if (!string.IsNullOrWhiteSpace(memo) || record.Project.Oid == 18)   //Abwesenheit - no memo needed
            {
                record.Memo = memo;
            }
            else
            {
                e.Cancel = true;
                throw new Exception("Ihre Buchung muss eine Notiz beinhalten!");
            }

            record.Save();
            e.Cancel = true;
            HistoryGrid.CancelEdit();
            this.UpdateDaySummaryDataSource();
        }

        protected void HistoryGridRowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                Records record = HistoryGrid.GetRow(HistoryGrid.FocusedRowIndex) as Records;
                record.Delete();
                e.Cancel = true;
                this.UpdateDaySummaryDataSource();
            }
            catch (NullReferenceException)
            {
                throw new Exception("FocusedRow Exception!"); //TODO: Genauer ansehen
            }
        }

        protected void HryCmbPspCode_Callback(object source, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillHryCmbPspCode(value);
        }

        private void FillHryCmbPspCode(int oid)
        {
            GridViewDataColumn column = HistoryGrid.Columns["Project"] as GridViewDataColumn;
            ASPxComboBox hryCmbPspCode = (ASPxComboBox)(HistoryGrid.FindEditRowCellTemplateControl(column, "HryCmbPspCode"));

            this.HryCmbPsPCodeXpoData.Criteria = "Projects.Oid = ? AND Status.StatusName = ?";
            this.HryCmbPsPCodeXpoData.CriteriaParameters.Add("Projects.Oid", oid.ToString());
            this.HryCmbPsPCodeXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            hryCmbPspCode.DataBind();
        }

        protected void HryCmbActivity_Callback(object sender, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillHryCmbActivity(value);
        }

        private void FillHryCmbActivity(int oid)
        {
            GridViewDataColumn column = HistoryGrid.Columns["Project"] as GridViewDataColumn;
            ASPxComboBox hryCmbActivity = (ASPxComboBox)(HistoryGrid.FindEditRowCellTemplateControl(column, "HryCmbActivity"));

            this.ActivitiesXpoData.Criteria = "Projects[Oid = ?]";
            this.ActivitiesXpoData.CriteriaParameters.Add("Oid", oid.ToString());

            hryCmbActivity.DataBind();
        }

        protected void HistoryGrid_DataBinding(object sender, EventArgs e)
        {
            //this.SetupDataSource();
            HistoryGrid.FocusedRowIndex = -1;

            if (HistoryGrid.SortCount == 0)
            {
                GridViewDataColumn column = HistoryGrid.Columns["Date"] as GridViewDataColumn;
                column.SortDescending();
            }
        }

        protected void HistoryGrid_CommandButtonInit(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (this.LeaderLockView || this.EmployeeLockView)
            {
                return;
            }

            var index = e.VisibleIndex;
            var employeeLocked = HistoryGrid.GetRowValues(index, "EmployeeLocked");
            if (employeeLocked != null &&
               (DateTime)employeeLocked != default(DateTime))
            {
                e.Visible = false;
            }
        }

        protected void HistoryGrid_CustomButtonInit(object sender, ASPxGridViewCustomButtonEventArgs e)
        {
            if (this.LeaderLockView || this.EmployeeLockView)
            {
                return;
            }

            var index = e.VisibleIndex;
            var employeeLocked = HistoryGrid.GetRowValues(index, "EmployeeLocked");
            if (employeeLocked != null &&
               (DateTime)employeeLocked != default(DateTime))
            {
                e.Visible = DevExpress.Utils.DefaultBoolean.False;
            }
        }

        protected DateTime GeDateTimeFromMinutes(object totalMinutes)
        {
            int min = 0;
            int.TryParse(totalMinutes.ToString(), out min);
            DateTime dt = new DateTime().Add(TimeSpan.FromMinutes(min));
            return dt;
        }

        protected string GetHoursAndMinutesFromMinutes(object totalMinutes)
        {
            int min = 0;
            int.TryParse(totalMinutes.ToString(), out min);

            TimeSpan span = System.TimeSpan.FromMinutes(min);
            String hours = ((int)span.TotalHours).ToString();
            String minutes = span.Minutes.ToString();
            if (minutes.Length == 1)
            {
                return hours + ":0" + minutes;
            }
            else
            {
                return hours + ":" + minutes;
            }
        }

        int totalsum;
        protected void CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                totalsum = 0;
            }

            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                totalsum += Convert.ToInt32(e.FieldValue);
            }

            e.TotalValue = "Summe = " + GetHoursAndMinutesFromMinutes(totalsum) + " h";
        }

        private void UpdateDaySummaryDataSource()
        {
            XpoHelper.NotifyDirtyTables("DaySummary");
        }
    }
}