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
    using System.Collections;
    using System.Drawing;
    using System.Linq;
    using System.Web.UI;
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.Misc;
    using com.commend.tools.PZE.View;
    using DevExpress.Web;
    using DevExpress.Xpo;
    using DevExpress.XtraPrinting;

    public partial class UserRecords : ExtendedPage
    {
        private const int FavoritesPageSize = 5;

        private User selectedUser;

        private XPCollection<User> userCollection;

        private XPCollection<DaySummary> days;

        private XPCollection<Projects> projects;

        private XPQuery<Records> records;

        override protected void OnInit(EventArgs e)
        {
            this.Init += new System.EventHandler(this.Page_Init);
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
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

            this.SetupComboboxes();
            this.SetupRecordsGridData();
            this.selectedUser = this.CurrentUser;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;

            if (this.CurrentUser.Permission.PermissionName == "admin")
            {
                this.newRecordUserRow.Attributes["class"] = StyleNames.AdminStyle;
                this.newRecordTeamMembersRow.Attributes["class"] = StyleNames.AdminStyle;
                this.newRecordProjectRow.Attributes["class"] = StyleNames.AdminStyle;
                this.newReocordPspCodeRow.Attributes["class"] = StyleNames.AdminStyle;
                this.newRecordActivityRow.Attributes["class"] = StyleNames.AdminStyle;
                this.newReocordMemoRow.Attributes["class"] = StyleNames.AdminStyle;

                UserLabel.Visible = true;
                CmbUser.Visible = true;

                if (CmbUser.Value == null)
                {
                    CmbUser.Value = this.CurrentUser.Oid;
                }
                else
                {
                    this.selectedUser = this.userCollection.Single(u => u.Oid == (int)CmbUser.Value);
                }
            }
            else
            {
                this.newRecordUserRow.Style.Add("display", "none");
                this.newRecordTeamMembersRow.Style.Add("display", "none");
                this.newRecordProjectRow.Attributes["class"] = StyleNames.UserStyle;
                this.newReocordPspCodeRow.Attributes["class"] = StyleNames.UserStyle;
                this.newRecordActivityRow.Attributes["class"] = StyleNames.UserStyle;
                this.newReocordMemoRow.Attributes["class"] = StyleNames.UserStyle;
            }

            if (!Page.IsCallback)
            {
                var favs = GetFavoriteRecords();
                if (favs != null)
                {
                    FavoritesGrid.DataSource = favs;
                    FavoritesGrid.DataBind();
                }
            }
        }

        private void FillXpoObjects()
        {
            this.FavoritesXpoData.Session = this.DataSession;
            this.ActivitiesXpoData.Session = this.DataSession;
            this.PspCodesXpoData.Session = this.DataSession;
            this.ProjectsXpoData.Session = this.DataSession;
            this.ProjectsXpoData.Criteria = "Status.StatusName = ?";
            this.ProjectsXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            this.ActivitiesXpoData.DefaultSorting = "ActivityNumber ASC";

            this.userCollection = new XPCollection<User>(this.DataSession);
            var userSorting = new SortingCollection();
            userSorting.Add(new SortProperty("PersonalId", DevExpress.Xpo.DB.SortingDirection.Ascending));
            this.userCollection.Sorting = userSorting;

            this.projects = new XPCollection<Projects>(this.DataSession);
            var projectsSorting = new SortingCollection();
            projectsSorting.Add(new SortProperty("ProjectNumber", DevExpress.Xpo.DB.SortingDirection.Ascending));
            this.projects.Sorting = projectsSorting;

            this.days = new XPCollection<DaySummary>(this.DataSession);
            this.records = new XPQuery<Records>(this.DataSession);
        }

        private void SetupComboboxes()
        {
            this.CmbUser.DataSource = this.userCollection;
            this.CmbUser.DataBind();

            this.SetupTeamMembersLookup();

            this.CmbProject.DataSource = this.projects.Where(p => p.Status.StatusName == "aktiv");
            this.CmbProject.DataBind();

            if (this.CurrentUser.ExternalStaff == true)
            {
                this.ExternalEmployee.Checked = true;
            }
        }

        private void SetupTeamMembersLookup()
        {
            if (this.CurrentUser == this.CurrentUser.Division.Leader)
            {
                this.TeamMembersLookup.DataSource = this.userCollection.Where(
                    u => u.Division == this.CurrentUser.Division &&
                         u.Status.StatusName == "aktiv" &&
                         u.Oid != this.CurrentUser.Oid);

                this.TeamMembersLookup.DataBind();

                LblTeamMembers.Visible = true;
                TeamMembersLookup.Visible = true;
            }
        }

        private void SetupRecordsGridData()
        {
            User selectedUser = this.CurrentUser;
            if (this.CurrentUser.Permission.PermissionName == "admin" && CmbUser.Value != null)
            {
                int value = (int)CmbUser.Value;
                selectedUser = this.userCollection.SingleOrDefault(t => t.Oid == value);
            }

            this.selectedUser = selectedUser;

            this.InternalHistoryGrid.Projects = this.projects;
            this.InternalHistoryGrid.Records = this.records;

            this.InternalHistoryGrid.SelectedUser = selectedUser;
            this.InternalHistoryGrid.SelectedDates = this.recordCalender.SelectedDates;
            this.InternalHistoryGrid.PageSize = 100;
        }

        private bool ProjectAndPspCodeValid()
        {
            int projectId = (int)CmbProject.Value;
            var project = this.projects.FirstOrDefault(t => t.Oid == projectId);

            // The Project state has to be 'aktiv'
            if (project.Status.StatusName == "aktiv")
            {
                Status pspCodeStatus;
                if (CmbPsPCode.Value != null)
                {
                    int pspcodeId = (int)CmbPsPCode.Value;
                    pspCodeStatus = project.PspCodes.FirstOrDefault(t => t.Oid == pspcodeId).Status;
                    if (pspCodeStatus.StatusName == "aktiv")
                    {
                        return true;
                    }
                    else // Especially if it is 'inaktiv'
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Der gewählte PSPCode wurde vom Administrator auf inaktiv gesetzt und ist daher nicht buchbar!');", true);
                        return false;
                    }
                }
                else
                {
                    // It is not allowed that no PSP-Code is selected
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Der gewählte PSP-Code wurde vom Administrator auf inaktiv gesetzt und ist daher nicht buchbar!');", true);
                    return false;
                }
            }
            else // Especially if it is 'inaktiv'
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Das gewählte Projekt wurde vom Administrator auf inaktiv gesetzt und ist daher nicht buchbar!');", true);
                return false;
            }
        }

        private bool ActivityAndMemoValid()
        {
            if (string.IsNullOrWhiteSpace(CmbActivity.Text))
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Tätigkeit beinhalten!');", true);
                return false;
            }

            int projectId = (int)CmbProject.Value;
            if (string.IsNullOrWhiteSpace(ASPxMemo1.Text) && projectId != 18)    //Abwesenheit - no memo needed
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Notiz beinhalten!');", true);
                return false;
            }

            return true;
        }

        private int GetRecordsTime()
        {
            return TimeEdit.DateTime.Hour * 60 + TimeEdit.DateTime.Minute;
        }

        private bool Timecorrect(int minutes, DateTime date, DaySummary day)
        {
            // Return zero if totalMinutes are incorrect or illegal.
            if (minutes == 0)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Buchung muss eine Zeitdauer beinhalten!');", true);
                return false;
            }

            if (minutes < 0)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Die Startzeit darf nicht größer als die Endzeit sein! Bzw. Datumsübergreifende Buchungen sind nicht zulässig!');", true);
                return false;
            }

            int maxTime = 600;
            int currentlyBookedTime = 0;
            if (date.Date.Day != DateTime.Today.Day)
            {
                currentlyBookedTime = day.BookedTime;
                maxTime = day.AttendenceTime < maxTime ? day.AttendenceTime : maxTime;
            }

            if ((minutes + currentlyBookedTime) > maxTime)
            {
                ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    "validation",
                    "alert('Sie können nicht mehr Zeit buchen, als Sie anwesend waren');",
                    true);

                return false;
            }

            return true;
        }

        protected void storeRecordToDB(object sender, EventArgs e)
        {
            UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork();
            Records record = new Records(unitOfWork);

            if (!(ProjectAndPspCodeValid() && ActivityAndMemoValid()))
            {
                return;
            }

            DateTime givenDate = TENewRecordDate.DateTime.Date;
            if (givenDate.Ticks > DateTime.Now.Ticks)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Sie können keine Buchung in der Zukunft tätigen!');", true);
                return;
            }

            int minutes = this.GetRecordsTime();

            if (!this.selectedUser.ExternalStaff)
            {
                var day = this.days.Where(x => (x.UserID == this.selectedUser) && (x.LoggingDay == givenDate)).FirstOrDefault();
                if (day != null && (day.DayStatus == (int)DayStatus1.MALocked || day.DayStatus == (int)DayStatus1.VLocked))
                {
                    ClientScript.RegisterClientScriptBlock(
                        this.GetType(),
                        "validation",
                        "alert('Dieser Tag wurde bereits freigegeben und kann daher nicht mehr bearbeitet werden!');",
                        true);

                    return;
                }

                if (!Timecorrect(minutes, givenDate, day))
                {
                    return;
                }
            }

            record.Duration = minutes;
            record.Date = givenDate;

            int value = (int)CmbProject.Value;
            var selectedProject = new XPQuery<Projects>(unitOfWork).First(t => t.Oid == value);
            record.Project = selectedProject;

            value = (int)CmbPsPCode.Value;
            record.PspCode = new XPQuery<PspCodes>(unitOfWork).First(t => t.Oid == value);

            // Muss abgefangen werden - wie in project und psp code
            value = (int)CmbActivity.Value;
            var selectedActivity = new XPQuery<Activities>(unitOfWork).First(t => t.Oid == value);
            record.Activity = selectedActivity;

            record.Memo = ASPxMemo1.Text.Trim();

            var users = new XPQuery<User>(unitOfWork);

            if (this.CurrentUser.Permission.PermissionName == "admin")
            {
                value = (int)CmbUser.Value;
                record.User = users.Single(t => t.Oid == value);
            }
            else
            {
                record.User = users.Single(u => u.Oid == this.CurrentUser.Oid);
            }

            record.Timestamp = DateTime.Now;
            record.Save();

            var userId = TeamMembersLookup.GridView.GetSelectedFieldValues("Oid");
            foreach (int id in userId)
            {
                Records duplicate = new Records(unitOfWork);
                duplicate.Duration = record.Duration;
                duplicate.Date = record.Date;
                duplicate.Project = record.Project;
                duplicate.PspCode = record.PspCode;
                duplicate.Activity = record.Activity;
                duplicate.Memo = record.Memo;
                duplicate.Timestamp = record.Timestamp;
                duplicate.User = users.Single(u => u.Oid == id);
                duplicate.Save();
            }

            unitOfWork.CommitChanges();
            this.UpdateDaySummaryDataSource();
            this.records = new XPQuery<Records>(this.DataSession);
        }

        protected void OnCbpBuchungCallback(object sender, CallbackEventArgsBase e)
        {
            var values = e.Parameter.Split(new char[] { ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                var projectId = int.Parse(values[0]);
                var pspCodeId = values.Length < 4 ? -1 : int.Parse(values[1]);
                var activityId = values.Length < 4 ? -1 : int.Parse(values[2]);
                var memoText = values.Length < 4 ? null : values[3].ToString();
                this.FillNewRecordComboboxes(projectId, pspCodeId, activityId, memoText);
            }
        }

        private void FillNewRecordComboboxes(int projectId, int pspCodeId = -1, int activityId = -1, string memoText = null)
        {
            var actualProject = this.projects.First(p => p.Oid == projectId);

            this.CmbProject.SelectedItem = this.CmbProject.Items.FindByValue(projectId);
            this.CmbProject.DataBind();

            this.CmbPsPCode.DataSource = actualProject.PspCodes.Where(p => p.Status.StatusName == "aktiv");
            this.CmbPsPCode.DataBind();

            if (pspCodeId > -1)
            {
                var selItem = this.CmbPsPCode.Items.FindByValue(pspCodeId);
                this.CmbPsPCode.SelectedItem = selItem;
            }
            else
            {
                this.CmbPsPCode.SelectedItem = null;
            }

            this.CmbPsPCode.DataBind();

            this.CmbActivity.DataSource = actualProject.Activities;
            this.CmbActivity.DataBind();

            if (activityId > -1)
            {
                var selItem = this.CmbActivity.Items.FindByValue(activityId);
                this.CmbActivity.SelectedItem = selItem;
            }
            else
            {
                this.CmbActivity.SelectedItem = null;
            }

            this.CmbActivity.DataBind();

            this.ASPxMemo1.Text = memoText;
            this.ASPxMemo1.DataBind();
        }

        protected void OnFavCmbPspCode_Callback(object source, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillFavCmbPsPCode(value);
        }

        protected void FillFavCmbPsPCode(int oid)
        {
            var column = FavoritesGrid.Columns["Project"] as GridViewDataColumn;
            var favCmbPspCode = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbPspCode"));

            this.PspCodesXpoData.Criteria = "Projects.Oid = ? AND Status.StatusName = ?";
            this.PspCodesXpoData.CriteriaParameters.Add("Projects.Oid", oid.ToString());
            this.PspCodesXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");

            favCmbPspCode.DataBind();
        }

        protected void OnFavCmbActivity_Callback(object source, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillFavCmbActivity(value);
        }

        protected void OnSummaryFieldCallback(object source, CallbackEventArgsBase e)
        {
            this.FillSummaryField();
        }

        protected void OnCbpBuchungActualDaySummaryCallback(object sender, CallbackEventArgsBase e)
        {
            var day = this.days.FirstOrDefault(r => (r.UserID == this.selectedUser) && (r.LoggingDay.Date == TENewRecordDate.DateTime.Date));
            if (day != null)
            {
                this.ActualDayAttendanceTime.DateTime = new DateTime(1, 1, 1, day.AttendenceHours, day.AttendenceMinutes, 0);
                this.ActualDayBookedTime.DateTime = new DateTime(1, 1, 1, day.BookedHours, day.BookedMinutes, 0);

                int diffTime = day.AttendenceTime - day.BookedTime;
                if (diffTime > 0)
                {
                    this.ActualDayBookedTime.ForeColor = Color.FromArgb(69, 69, 69);
                    this.ActualDayDifferenceTime.DateTime = new DateTime(1, 1, 1, diffTime / 60, diffTime % 60, 0);
                    this.ActualDayDifferenceTime.DateTime.AddMinutes(diffTime);
                }
                else
                {
                    this.ActualDayBookedTime.ForeColor = Color.Red;
                    this.ActualDayDifferenceTime.DateTime = new DateTime(1, 1, 1);
                }
            }
        }

        protected void FillFavCmbActivity(int oid)
        {
            var column = FavoritesGrid.Columns["Project"] as GridViewDataColumn;
            var favCmbActivity = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbActivity"));

            this.ActivitiesXpoData.Criteria = "Projects[Oid = ?]";
            this.ActivitiesXpoData.CriteriaParameters.Add("Oid", oid.ToString());

            favCmbActivity.DataBind();
        }

        private IEnumerable GetFavoriteRecords()
        {
            XPQuery<Favorites> favoriteQuery = new XPQuery<Favorites>(this.DataSession);

            if (this.CurrentUser.Permission.PermissionName == "admin" && CmbUser.Value != null)
            {
                int value = (int)CmbUser.Value;
                User selsectedUser = userCollection.SingleOrDefault(t => t.Oid == value);

                var lastEntries = favoriteQuery.Where(r => r.User == selsectedUser)
                     .OrderByDescending(r => r.Timestamp);
                return lastEntries;
            }
            else
            {
                var lastEntries = favoriteQuery.Where(r => r.User == this.CurrentUser)
                                 .OrderByDescending(r => r.Timestamp);
                return lastEntries;
            }
        }

        protected void CmbUser_ValueChanged(object sender, EventArgs e)
        {
            this.SetupRecordsGridData();
            this.InternalHistoryGrid.DataBind();
        }

        protected void FavoritesGridRowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            GridViewDataColumn column = FavoritesGrid.Columns["Project"] as GridViewDataColumn;
            Favorites favorite = FavoritesGrid.GetRow(FavoritesGrid.EditingRowVisibleIndex) as Favorites;

            ASPxComboBox favCmbProject = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbProject"));
            ASPxComboBox favCmbPspCode = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbPspCode"));
            ASPxComboBox favCmbActivity = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbActivity"));
            var favMemo = (ASPxTextEdit)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavTbMemo"));

            favorite.Project = this.projects.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbProject.SelectedItem.Value));
            if (favCmbPspCode.SelectedItem != null)
            {
                favorite.PspCode = favorite.Project.PspCodes.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbPspCode.SelectedItem.Value));
            }
            else
            {
                favorite.PspCode = null;
            }

            if (favCmbActivity.SelectedItem != null)
            {
                favorite.Activity = favorite.Project.Activities.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbActivity.SelectedItem.Value));
            }
            else
            {
                favorite.Activity = null;
            }

            favorite.Memo = favMemo.Text;

            favorite.Timestamp = DateTime.Now;

            favorite.Save();
            e.Cancel = true;
            FavoritesGrid.CancelEdit();
        }

        protected void FavoritesGridRowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            GridViewDataColumn column = FavoritesGrid.Columns["Project"] as GridViewDataColumn;
            Favorites favorite = new Favorites(this.DataSession);

            ASPxComboBox favCmbProject = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbProject"));
            ASPxComboBox favCmbPspCode = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbPspCode"));
            ASPxComboBox favCmbActivity = (ASPxComboBox)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavCmbActivity"));

            var favMemo = (ASPxTextEdit)(FavoritesGrid.FindEditRowCellTemplateControl(column, "FavTbMemo"));


            favorite.User = this.CurrentUser;
            if (favCmbProject.SelectedItem == null)
            {
                favorite.Project = null;
                favorite.PspCode = null;
                favorite.Activity = null;
            }
            else
            {
                favorite.Project = this.projects.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbProject.SelectedItem.Value));

                if (favCmbPspCode.SelectedItem != null)
                {
                    favorite.PspCode = favorite.Project.PspCodes.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbPspCode.SelectedItem.Value));
                }
                else
                {
                    favorite.PspCode = null;
                }

                if (favCmbActivity.SelectedItem != null)
                {
                    favorite.Activity = favorite.Project.Activities.SingleOrDefault(p => p.Oid == Convert.ToInt32(favCmbActivity.SelectedItem.Value));
                }
                else
                {
                    favorite.Activity = null;
                }

            }

            favorite.Memo = favMemo.Text;

            favorite.Timestamp = DateTime.Now;

            favorite.Save();
            e.Cancel = true;
            FavoritesGrid.CancelEdit();

        }

        protected void FavoritesGridRowDeleteing(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            Favorites favorite = FavoritesGrid.GetRow(FavoritesGrid.FocusedRowIndex) as Favorites;
            favorite.Delete();
            e.Cancel = true;
        }

        protected void FavoritesGrid_DataBinding(object sender, EventArgs e)
        {
            this.FavoritesGridDataBind();
        }

        private void FavoritesGridDataBind()
        {
            var favRecords = GetFavoriteRecords();
            if (favRecords != null)
            {
                FavoritesGrid.DataSource = favRecords;
                FavoritesGrid.Focus();
            }
            else
            {
                FavoritesGrid.DataSource = FavoritesXpoData;
            }
        }

        protected string GetProjectNameByOid(object selProject)
        {
            if (selProject != null)
            {
                int oid = (int)selProject;
                var project = this.projects.FirstOrDefault<Projects>(x => x.Oid == oid);
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
                var pspCode = this.projects.First(proj => proj.Oid == projectOid).PspCodes.FirstOrDefault(psp => psp.Oid == pspOid);
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
                var activity = this.projects.First(proj => proj.Oid == projectOid).Activities.First(act => act.Oid == activityOid);

                return activity.ActivityName;
            }
            else
            {
                return string.Empty;
            }
        }

        protected void recordCalender_DayCellPrepared(object sender, CalendarDayCellPreparedEventArgs e)
        {
            var day = this.days.FirstOrDefault(x => (x.UserID == this.selectedUser) && (x.LoggingDay == e.Date));
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

        private void UpdateDaySummaryDataSource()
        {
            XpoHelper.NotifyDirtyTables("DaySummary");
            this.days = new XPCollection<DaySummary>(this.DataSession);
        }

        private void FillSummaryField()
        {
            int daysCount = 0;
            int attendenceTime = 0;
            int bookedTime = 0;
            int diffTime = 0;
            DateTime firstDate, lastDate;
            bool enableLockBtn = this.selectedUser == this.CurrentUser;
            bool enableFreeBtn = this.selectedUser == this.CurrentUser;

            if (this.recordCalender.SelectedDates.Count > 0)
            {
                firstDate = this.recordCalender.SelectedDates.First();
                lastDate = this.recordCalender.SelectedDates.Last().AddDays(1.0);
            }
            else
            {
                firstDate = lastDate = DateTime.Today.Date;
            }

            this.days.Where(item =>
                    item.UserID == this.selectedUser &&
                    firstDate.Ticks <= item.LoggingDay.Ticks &&
                    item.LoggingDay.Ticks < lastDate.Date.Ticks
                ).ToList().ForEach(d =>
                    {
                        daysCount++;
                        attendenceTime += d.AttendenceTime;
                        bookedTime += d.BookedTime;
                        enableLockBtn &= this.EnableLockButton(d);
                        enableFreeBtn &= this.EnableFreeButton(d);
                    });

            enableLockBtn &= daysCount > 0;
            enableFreeBtn &= daysCount > 0;

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

            this.BtnLock.Enabled = enableLockBtn;
            this.BtnLock.Visible = !enableFreeBtn;
            this.BtnFree.Enabled = enableFreeBtn;
            this.BtnFree.Visible = enableFreeBtn;
        }

        private bool EnableLockButton(DaySummary day)
        {
            var date = DateTime.Today;
            var sundayOfLastWeek = date.AddDays(-(int)date.DayOfWeek - 7);
            var mondayOfThisWeek = sundayOfLastWeek.AddDays(+8);
            if (!day.TimeVerified && ((date.Ticks <= mondayOfThisWeek.Ticks && day.LoggingDay >= sundayOfLastWeek) || day.LoggingDay >= mondayOfThisWeek))
            {
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
                case (int)DayStatus1.NotBooked:
                case (int)DayStatus1.OverBooked:
                default:
                    return false;
            }
        }

        private bool EnableFreeButton(DaySummary day)
        {
            if (day.DayStatus == (int)DayStatus1.MALocked)
            {
                return true;
            }

            return false;
        }

        protected void OnBtnLockClick(object sender, EventArgs e)
        {
            var dates = this.recordCalender.SelectedDates;
            foreach (var date in dates)
            {
                var records = this.records.Where(x => (x.User.Oid == this.CurrentUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.EmployeeLocked = DateTime.Now;
                    record.Save();
                }
            }

            this.UpdateDaySummaryDataSource();

            BtnFree.Enabled = true;
            BtnLock.Enabled = false;
        }

        protected void OnBtnFreeClick(object sender, EventArgs e)
        {
            var dates = this.recordCalender.SelectedDates;
            foreach (var date in dates)
            {
                var records = this.records.Where(x => (x.User.Oid == this.CurrentUser.Oid) && (x.Date == date));
                foreach (var record in records)
                {
                    record.EmployeeLocked = default(DateTime);
                    record.Save();
                }
            }

            this.UpdateDaySummaryDataSource();
            BtnLock.Enabled = true;
            BtnFree.Enabled = false;
        }
    }
}