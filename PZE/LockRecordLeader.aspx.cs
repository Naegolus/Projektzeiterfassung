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
    using System.Drawing;
    using System.Linq;
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.Misc;
    using com.commend.tools.PZE.View;
    using DevExpress.Web;
    using DevExpress.Xpo;

    public partial class LockRecordLeader : ExtendedPage
    {
        private User SelectedUser_Cached;

        private XPCollection<DaySummary> days;

        private XPCollection<Projects> projects;

        private XPQuery<Records> records;

        public User SelectedUser
        {
            get
            {
                var oid = Session["SelectedUserOid"];
                if ((oid != null) && (SelectedUser_Cached == null))
                {
                    SelectedUser_Cached = this.DataSession.Query<User>().Single(u => u.Oid == (int)oid);
                }
                return SelectedUser_Cached;
            }

            private set
            {
                SelectedUser_Cached = value;
                Session["SelectedUserOid"] = value.Oid;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.CurrentUser == null)
            {
                if (Page.IsCallback)
                {
                    ASPxWebControl.RedirectOnCallback("~/Account/Login.aspx");
                    return;
                }
                else
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
            }

            this.FillXpoObjects();
            this.SetupRecordsGridData();

            if (this.SelectedUser != null)
            {
                this.CmbUser.Value = this.SelectedUser.Oid;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void FillXpoObjects()
        {
            this.days = new XPCollection<DaySummary>(this.DataSession);
            this.projects = new XPCollection<Projects>(this.DataSession);
            this.records = new XPQuery<Records>(this.DataSession);
        }

        protected void OnCmbUserInit(object sender, EventArgs e)
        {
            if (this.CurrentUser == null)
            {
                return;
            }

            var cmbUser = (ASPxComboBox)sender;
            cmbUser.DataSource = new XPQuery<User>(this.DataSession).Where(
                u => u != this.CurrentUser &&
                    (u.Status.StatusName == "aktiv" &&
                        u.Division == this.CurrentUser.Division) ||
                    (u.Division.Leader == u));

            cmbUser.DataBind();
        }

        private void SetupRecordsGridData()
        {
            this.InternalHistoryGrid.PageSize = 25;
            this.InternalHistoryGrid.SelectedDates = this.LockRecordsCalendar.SelectedDates;

            if (this.SelectedUser != null)
            {
                this.InternalHistoryGrid.SelectedUser = this.SelectedUser;
            }

            this.InternalHistoryGrid.Projects = this.projects;
            this.InternalHistoryGrid.Records = this.records;

            this.InternalHistoryGrid.DataBind();
        }

        protected void CmbUser_IndexChanged(object sender, EventArgs e)
        {
            var userOid = (int)CmbUser.SelectedItem.Value;
            this.SelectedUser = this.DataSession.Query<User>().Single(u => u.Oid == userOid);

            this.SetupRecordsGridData();
            this.FillSummaryField();
        }

        protected void OnLockRecordsCalendar_DayCellPrepared(object sender, CalendarDayCellPreparedEventArgs e)
        {
            var day = this.days.FirstOrDefault(x => (x.UserID == this.SelectedUser) && (x.LoggingDay == e.Date));
            if (day == null)
            {
                e.Cell.ToolTip = "Der Mitarbeiter war an diesem Tag nicht anwesend.";
                return;
            }

            e.Cell.ToolTip = string.Format("Anwesenheit: {0}h : {1}' \n Verbucht: {2}h : {3}'",
                day.AttendenceHours,
                day.AttendenceMinutes,
                day.BookedHours,
                day.BookedMinutes);

            switch (day.DayStatus)
            {
                case (int)DayStatus1.NotBooked:
                case (int)DayStatus1.Underbooked:
                case (int)DayStatus1.OverBooked:
                case (int)DayStatus1.OK:
                    e.Cell.BackColor = Color.Salmon;
                    break;

                case (int)DayStatus1.MALocked:
                    e.Cell.BackColor = Color.LightGreen;
                    break;

                case (int)DayStatus1.VLocked:
                    e.Cell.BackColor = Color.DarkSeaGreen;
                    break;

                case (int)DayStatus1.NoAttendence:
                default:
                    e.Cell.BackColor = Color.White;
                    break;
            }
        }

        protected void NotifyCalendarSelChangedGrid_Callback(object sender, EventArgs e)
        {
            this.SetupRecordsGridData();
        }

        protected void NotifyCalendarSelChangedSum_Callback(object sender, EventArgs e)
        {
            this.FillSummaryField();
        }

        private void FillSummaryField()
        {
            int daysCount = 0;
            int attendenceTime = 0;
            int bookedTime = 0;
            int diffTime = 0;
            bool enableLockBtn = true;
            bool enableFreeBtn = true;
            var days = this.DataSession.Query<DaySummary>();
            foreach (var date in this.LockRecordsCalendar.SelectedDates)
            {
                try
                {
                    var day = days.SingleOrDefault(x => (x.UserID == this.SelectedUser) && (x.LoggingDay == date));
                    if (day == null)
                    {
                        continue;
                    }

                    daysCount++;
                    attendenceTime += day.AttendenceTime;
                    bookedTime += day.BookedTime;
                    enableLockBtn &= this.EnableLockButton(day);
                    enableFreeBtn &= this.EnableFreeButton(day);
                }
                catch (Exception)
                {
                    this.LblSelComments.Text += string.Format(
                        "Bitte kontaktieren Sie den Administrator, die Einträge für den {0} sind fehlerhaft!",
                        date.ToShortDateString());
                }
            }

            diffTime = attendenceTime - bookedTime;

            this.LblSelDays.Text = string.Format("{0}", daysCount);
            this.LblSelAttendenceHours.Text = string.Format("{0}h {1}'", attendenceTime / 60, attendenceTime % 60);
            this.LblSelBookedHours.Text = string.Format("{0}h {1}'", bookedTime / 60, bookedTime % 60);

            if (diffTime >= 0)
            {
                this.LblSelDiffHours.ForeColor = Color.Green;
                this.LblSelDiffHours.Text = string.Format("{0}h {1}'", diffTime / 60, diffTime % 60);
            }
            else
            {
                this.LblSelDiffHours.ForeColor = Color.Red;
                this.LblSelDiffHours.Text = string.Format("- {0}h {1}'", -diffTime / 60, -diffTime % 60);
            }

            if (enableLockBtn)
            {
                this.BtnLock.Enabled = true;
                this.LblSelComments.ForeColor = Color.Black;
            }
            else
            {
                this.BtnLock.Enabled = false;
                this.LblSelComments.ForeColor = Color.Red;
            }

            this.BtnFree.Enabled = enableFreeBtn ? true : false;

        }

        protected void BtnLock_Click(object sender, EventArgs e)
        {
            UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork();
            var dates = this.LockRecordsCalendar.SelectedDates;
            foreach (var date in dates)
            {
                var records = new XPQuery<Records>(unitOfWork).Where(x => (x.User.Oid == SelectedUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.LeaderLocked = DateTime.Now;
                    record.Save();
                }
            }

            unitOfWork.CommitChanges();
            BtnFree.Enabled = true;
            BtnLock.Enabled = false;

            this.UpdateDaySummaryDataSource();

            this.SetupRecordsGridData();
            this.FillSummaryField();
        }

        protected void BtnFree_Click(object sender, EventArgs e)
        {
            UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork();
            var dates = this.LockRecordsCalendar.SelectedDates;
            foreach (var date in dates)
            {
                var records = new XPQuery<Records>(unitOfWork).Where(x => (x.User.Oid == SelectedUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.LeaderLocked = default(DateTime);
                    record.Save();
                }
            }

            unitOfWork.CommitChanges();
            BtnLock.Enabled = true;
            BtnFree.Enabled = false;

            this.UpdateDaySummaryDataSource();
            this.SetupRecordsGridData();
            this.FillSummaryField();
        }

        private bool EnableLockButton(DaySummary day)
        {
            if ((day.DayStatus == (int)DayStatus1.MALocked)
                || (day.DayStatus == (int)DayStatus1.NoAttendence))
            {
                return true;
            }

            return false;
        }

        private bool EnableFreeButton(DaySummary day)
        {
            if ((day.DayStatus == (int)DayStatus1.VLocked)
                || (day.DayStatus == (int)DayStatus1.NoAttendence))
            {
                return true;
            }
            return false;
        }

        private void UpdateDaySummaryDataSource()
        {
            XpoHelper.NotifyDirtyTables("DaySummary");
            this.days = new XPCollection<DaySummary>(this.DataSession);
        }
    }
}