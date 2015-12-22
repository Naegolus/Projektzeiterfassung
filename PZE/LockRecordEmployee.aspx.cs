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
using System.Drawing;
using System.Linq;
namespace com.commend.tools.PZE
{
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.Misc;
    using com.commend.tools.PZE.View;
    using DevExpress.Web;
    using DevExpress.Xpo;

    public partial class LockRecordEmployee : ExtendedPage
    {
        private XPCollection<DaySummary> days;

        private XPCollection<Projects> projects;

        private XPQuery<Records> records;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.CurrentUser != null)
            {
                this.FillXpoObjects();
                this.SetupRecordsGridData();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.CurrentUser == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        private void FillXpoObjects()
        {
            this.days = new XPCollection<DaySummary>(this.DataSession);
            this.projects = new XPCollection<Projects>(this.DataSession);
            this.records = new XPQuery<Records>(this.DataSession);
        }

        private void SetupRecordsGridData()
        {
            this.InternalHistoryGrid.SelectedUser = this.CurrentUser;
            this.InternalHistoryGrid.PageSize = 25;
            this.InternalHistoryGrid.SelectedDates = this.LockRecordsCalendar.SelectedDates;
            this.InternalHistoryGrid.Projects = this.projects;
            this.InternalHistoryGrid.Records = this.records;
        }

        protected void OnLockRecordsCalendar_DayCellPrepared(object sender, CalendarDayCellPreparedEventArgs e)
        {
            var day = this.days.FirstOrDefault(x => (x.UserID == this.CurrentUser) && (x.LoggingDay == e.Date));
            if (day == null)
            {
                e.Cell.ToolTip = "Sie waren an diesem Tag nicht anwesend.";
                return;
            }

            e.Cell.ToolTip = string.Format("Anwesenheit: {0}h : {1}' \n Verbucht: {2}h : {3}'",
                day.AttendenceHours,
                day.AttendenceMinutes,
                day.BookedHours,
                day.BookedMinutes);

            switch (day.DayStatus)
            {
                case (int)DayStatus1.NoAttendence:
                case (int)DayStatus1.VLocked:
                    e.Cell.BackColor = Color.White;
                    break;

                case (int)DayStatus1.NotBooked:
                case (int)DayStatus1.Underbooked:
                    e.Cell.BackColor = Color.LightGray;
                    break;

                case (int)DayStatus1.OverBooked:
                    e.Cell.BackColor = Color.Salmon;
                    break;

                case (int)DayStatus1.OK:
                    e.Cell.BackColor = Color.LightGreen;
                    break;

                case (int)DayStatus1.MALocked:
                    e.Cell.BackColor = Color.DarkSeaGreen;
                    break;

                default:
                    e.Cell.BackColor = Color.Gold;
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

            foreach (var date in this.LockRecordsCalendar.SelectedDates)
            {
                try
                {
                    var day = this.days.SingleOrDefault(x => (x.UserID == this.CurrentUser) && (x.LoggingDay == date));
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
                var records = new XPQuery<Records>(unitOfWork).Where(x => (x.User.Oid == this.CurrentUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.EmployeeLocked = DateTime.Now;
                    record.Save();
                }
            }

            unitOfWork.CommitChanges();
            this.UpdateDaySummaryDataSource();

            BtnFree.Enabled = true;
            BtnLock.Enabled = false;
        }

        protected void BtnFree_Click(object sender, EventArgs e)
        {
            UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork();
            var dates = this.LockRecordsCalendar.SelectedDates;
            foreach (var date in dates)
            {
                var records = new XPQuery<Records>(unitOfWork).Where(x => (x.User.Oid == this.CurrentUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.EmployeeLocked = default(DateTime);
                    record.Save();
                }
            }

            unitOfWork.CommitChanges();
            this.UpdateDaySummaryDataSource();
            BtnLock.Enabled = true;
            BtnFree.Enabled = false;
        }

        private bool EnableLockButton(DaySummary day)
        {
            var date = DateTime.Today;
            var sundayOfLastWeek = date.AddDays(-(int)date.DayOfWeek - 7);
            var mondayOfThisWeek = sundayOfLastWeek.AddDays(+8);
            if (((date.Ticks <= mondayOfThisWeek.Ticks) && (day.LoggingDay >= sundayOfLastWeek)) || (day.LoggingDay >= mondayOfThisWeek))
            {
                this.LblSelComments.Text += string.Format(
                        "{0}: Dieser Tag kann noch nicht freigegeben werden, da die BMD-Zeit zuerst vom Personalwesen bestätigt werden muss.\n",
                        day.LoggingDay.ToShortDateString());
                return false;
            }

            switch (day.DayStatus)
            {
                case (int)DayStatus1.NoAttendence:
                case (int)DayStatus1.Underbooked:
                case (int)DayStatus1.OK:
                    return true;

                case (int)DayStatus1.MALocked:
                case (int)DayStatus1.VLocked:
                    return false;

                case (int)DayStatus1.NotBooked:
                    this.LblSelComments.Text += string.Format(
                        "{0}: Sie müssen eine Buchung erstellen, um sie freigeben zu können.\n",
                        day.LoggingDay.ToShortDateString());
                    return false;

                case (int)DayStatus1.OverBooked:
                    this.LblSelComments.Text += string.Format(
                        "{0}: Dieser Tag kann nicht freigegeben werden, da sie mehr Zeit gebucht haben, als Sie anwesend waren.\n",
                        day.LoggingDay.ToShortDateString());
                    return false;

                default:
                    return false;
            }
        }

        private bool EnableFreeButton(DaySummary day)
        {
            if ((day.DayStatus == (int)DayStatus1.MALocked)
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