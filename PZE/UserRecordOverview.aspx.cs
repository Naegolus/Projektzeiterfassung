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
using com.commend.tools.PZE.View;
using DevExpress.Web;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;


namespace com.commend.tools.PZE
{

    public partial class UserRecordOverview : ExtendedPage
    {
        private XPCollection<User> userCollection;
        private bool shouldExpand = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            this.SetupDataSource();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.CurrentUser == null)
            {
                if (Page.IsCallback)
                {
                    ASPxWebControl.RedirectOnCallback("~/Account/Login.aspx");
                }
                else
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
            else
            {
                //if (this.CurrentUser.Permission.PermissionName == "admin")
                //{
                //    UserLabel.Visible = true;
                //    CmbUser.Visible = true;

                //    if (CmbUser.Value != null)
                //    {
                //        int value = (int)CmbUser.Value;
                //        RecordsXpoData.Criteria = "User = ?";
                //        RecordsXpoData.CriteriaParameters.Add("User", userCollection.SingleOrDefault(t => t.Oid == value).Oid.ToString());
                //        RecordGrid.DataBind();
                //    }
                //    else
                //    {
                //        CmbUser.Value = this.CurrentUser.Oid;
                //        RecordsXpoData.Criteria = "User = ?";
                //        RecordsXpoData.CriteriaParameters.Add("User", this.CurrentUser.Oid.ToString());
                //    }
                //}
                //else
                //{
                RecordsXpoData.Criteria = "User = ?";
                RecordsXpoData.CriteriaParameters.Add("User", this.CurrentUser.Oid.ToString());

                //RecordsXpoData.CriteriaParameters.Add()
                //}

            }

            RecordGrid.Focus();

            try
            {
                List<object> list = new List<object>();
                list = Session["RecordGridLayout"] as List<object>;
                this.shouldExpand = (bool)list[1];
            }
            catch
            {
            }
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.UserXpoData.Session = this.DataSession;
            this.RecordsXpoData.Session = this.DataSession;
            this.ProjectsXpoData.Session = this.DataSession;
            this.ActivitiesXpoData.Session = this.DataSession;
            this.PspCodesXpoData.Session = this.DataSession;
            this.userCollection = new XPCollection<User>(this.DataSession);

            //*_*
            this.ProjectsXpoData.Criteria = "Status.StatusName = ?";
            this.ProjectsXpoData.CriteriaParameters.Add("Status.StatusName", "aktiv");
            //*_*
        }

        protected void CmbPsPCode_Callback(object source, CallbackEventArgsBase e)
        {
            int value = 0;
            int.TryParse(e.Parameter, out value);
            FillPsPCodeCombo(value);
        }

        protected void FillPsPCodeCombo(int oid)
        {
            GridViewDataColumn column = RecordGrid.Columns["Project"] as GridViewDataColumn;
            ASPxComboBox cbPspCode = (ASPxComboBox)(RecordGrid.FindEditRowCellTemplateControl(column, "CmbPspCode"));

            PspCodesXpoData.Criteria = "Projects.Oid = ?";
            PspCodesXpoData.CriteriaParameters.Add("Projects.Oid", oid.ToString());

            cbPspCode.DataBind();
        }

        protected void RecordGridRowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            XPQuery<Records> recordQuery = new XPQuery<Records>(this.DataSession);
            GridViewDataColumn column = RecordGrid.Columns["Date"] as GridViewDataColumn;
            ASPxDateEdit dateEdit = (ASPxDateEdit)(RecordGrid.FindEditRowCellTemplateControl(column, "DayEdit"));

            // Project
            column = RecordGrid.Columns["Project"] as GridViewDataColumn;
            ASPxTimeEdit timeEdit = (ASPxTimeEdit)(RecordGrid.FindEditRowCellTemplateControl(column, "TimeEdit"));

            TimeSpan time = new TimeSpan(timeEdit.DateTime.Hour, timeEdit.DateTime.Minute, 0);

            // The round number, here is a quarter...
            int Round = 5;

            // Count of round number in this total minutes...
            double CountRound = (time.TotalMinutes / Round);

            // The main formula to calculate round time...
            int totalMin = (int)Math.Truncate(CountRound + 0.5) * Round;

            Records record = RecordGrid.GetRow(RecordGrid.EditingRowVisibleIndex) as Records;

            IEnumerable<Records> entriesByGivenDate = recordQuery.Where(r => r.User == this.CurrentUser && r.Date == dateEdit.Date).OrderByDescending(r => r.Timestamp);

            int minutesBygivenDate = 0;

            foreach (var element in entriesByGivenDate)
            {
                minutesBygivenDate += element.Duration;
            }

            if (minutesBygivenDate - record.Duration + totalMin <= 1440)
            {
                ASPxComboBox cmbProject = (ASPxComboBox)(RecordGrid.FindEditRowCellTemplateControl(column, "CmbProject"));
                ASPxComboBox cmbPspCode = (ASPxComboBox)(RecordGrid.FindEditRowCellTemplateControl(column, "CmbPspCode"));
                ASPxComboBox cmbActivity = (ASPxComboBox)(RecordGrid.FindEditRowCellTemplateControl(column, "CmbActivity"));


                XPCollection<Projects> projects = new XPCollection<Projects>(this.DataSession);
                XPCollection<PspCodes> pspcodes = new XPCollection<PspCodes>(this.DataSession);
                XPCollection<Activities> activities = new XPCollection<Activities>(this.DataSession);

                record.Duration = totalMin;
                record.Date = dateEdit.Date;

                record.Project = projects.SingleOrDefault(p => p.Oid == Convert.ToInt32(cmbProject.SelectedItem.Value));
                if (cmbPspCode.SelectedItem != null)
                {
                    record.PspCode = pspcodes.SingleOrDefault(p => p.Oid == Convert.ToInt32(cmbPspCode.SelectedItem.Value));
                }
                else
                {
                    e.Cancel = true;
                    throw new Exception("Ihre Buchung muss einen PSP-Code beinhalten!");
                }

                if (cmbActivity.SelectedItem != null)
                {
                    record.Activity = activities.SingleOrDefault(p => p.Oid == Convert.ToInt32(cmbActivity.SelectedItem.Value));
                }
                else
                {
                    e.Cancel = true;
                    throw new Exception("Ihre Buchung muss eine Tätigkeit beinhalten!");
                }

                if (!string.IsNullOrWhiteSpace(e.NewValues["Memo"].ToString()))
                {
                    record.Memo = e.NewValues["Memo"].ToString();
                }
                else
                {
                    e.Cancel = true;
                    throw new Exception("Ihre Buchung muss eine Notiz beinhalten!");
                }

                record.Save();
            }
            else
            {
                e.Cancel = true;
                throw new Exception("Sie dürfen nicht mehr als 24h Stunden auf ein Datum buchen!");
            }
        }

        protected string GetProjectNameByOid(object oidObject)
        {
            if (oidObject != null)
            {
                int oid = (int)oidObject;

                XPCollection<Projects> projects = new XPCollection<Projects>(this.DataSession);

                Projects project = projects.SingleOrDefault(p => p.Oid == oid);

                return project.ProjectName;
            }
            else
            {
                return "";
            }
        }

        protected string GetPsPCodeNameByOid(object oidObject)
        {
            if (oidObject != null)
            {
                int oid = (int)oidObject;

                XPCollection<PspCodes> pspcodes = new XPCollection<PspCodes>(this.DataSession);

                PspCodes pspcode = pspcodes.SingleOrDefault(p => p.Oid == oid);

                return pspcode.PspCodeName;
            }
            else
            {
                return "";
            }
        }

        protected string GetActivityNameByOid(object oidObject)
        {
            if (oidObject != null)
            {
                int oid = (int)oidObject;

                XPCollection<Activities> activities = new XPCollection<Activities>(this.DataSession);

                Activities activity = activities.SingleOrDefault(p => p.Oid == oid);

                return activity.ActivityName;
            }
            else
            {
                return "";
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

        protected string GetHoursFromTotalMinutes(object totalMinutes)
        {
            int min = 0;
            int.TryParse(totalMinutes.ToString(), out min);

            TimeSpan span = System.TimeSpan.FromMinutes(min);
            String hours = ((int)span.TotalHours).ToString();

            return hours;
        }

        protected string GetMinutesFromTotalMinutes(object totalMinutes)
        {
            int min = 0;
            int.TryParse(totalMinutes.ToString(), out min);

            TimeSpan span = System.TimeSpan.FromMinutes(min);
            String hours = ((int)span.TotalHours).ToString();
            String minutes = span.Minutes.ToString();

            return minutes;
        }


        protected string GetHoursWithFloatingPoint(object totalMinutes)
        {
            float min = 0;
            float.TryParse(totalMinutes.ToString(), out min);
            float hours = 0;

            hours = min / 60;

            return hours.ToString();
        }

        protected string GetFornameAndSurame(object surName)
        {
            XPCollection<User> users = new XPCollection<User>(this.DataSession);
            User user = users.SingleOrDefault(p => p.SurName == surName.ToString());
            if (user != null)
            {
                return user.SurName + " " + user.ForName;
            }
            else
            {
                return null;
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

            e.TotalValue = "Sum = " + GetHoursAndMinutesFromMinutes(totalsum) + " h";

        }




        protected void OnExportButton_Click(object sender, EventArgs e)
        {
            if (CmbExport.SelectedItem.Value.ToString().Equals("Pdf"))
            {
                btnPdfExport_Click();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Xls"))
            {
                btnXlsExport_Click();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Xlsx"))
            {
                btnXlsxExport_Click();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Rtf"))
            {
                btnRtfExport_Click();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Csv"))
            {
                btnCsvExport_Click();
            }
        }

        protected void btnPdfExport_Click()
        {
            gridExport.WritePdfToResponse();
        }
        protected void btnXlsExport_Click()
        {
            gridExport.WriteXlsToResponse();
        }
        protected void btnXlsxExport_Click()
        {
            gridExport.WriteXlsxToResponse();
        }
        protected void btnRtfExport_Click()
        {
            gridExport.WriteRtfToResponse();
        }
        protected void btnCsvExport_Click()
        {
            gridExport.WriteCsvToResponse();
        }

        protected void OnRenderBrick(object sender, DevExpress.Web.ASPxGridViewExportRenderingEventArgs e)
        {
            if (e.RowType == GridViewRowType.Data && e.Column.Caption.Equals("Benutzer"))
            {
                e.Text = GetFornameAndSurame(e.Value);
            }

            if (e.RowType == GridViewRowType.Data && e.Column.Caption.Equals("Dauer"))
            {
                e.Text = GetHoursAndMinutesFromMinutes(e.Value);
                //e.Text = GetHoursWithFloatingPoint(e.Value);

            }

            if (e.RowType == GridViewRowType.Data && e.Column.Caption.Equals("Datum"))
            {
                e.Text = e.Text.Replace("/", ".");
            }
        }

        protected void grid_UnGrouping(object sender, EventArgs e)
        {
            RecordGrid.ClearSort();
            GridViewDataColumn column = RecordGrid.Columns["Date"] as GridViewDataColumn;
            column.SortDescending();
        }

        protected void grid_ShowAllColumns(object sender, EventArgs e)
        {
            for (int i = 0; i < RecordGrid.Columns.Count; i++)
            {
                if (i != 1 && i != 2)
                {
                    RecordGrid.Columns[i].Visible = true;
                }
            }
        }

        protected void OnApplyButtonClicked(object sender, EventArgs e)
        {
            if (ToDayEdit.Date != null && FromDayEdit.Date > ToDayEdit.Date)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "validation", "alert('Bitte korrigieren Sie Ihre Eingabe! (Das Startdatum ist größer als das Enddatum)');", true);
            }
            else
            {
                String filterexpression = "";

                if (FromDayEdit.Value != null && ToDayEdit.Value != null)
                {

                    filterexpression += "[Date] Between(#" + FromDayEdit.Date.ToString("yyyy-MM-dd") + "#, #" + ToDayEdit.Date.ToString("yyyy-MM-dd") + "#)";

                }
                else
                {
                    if (FromDayEdit.Value != null && ToDayEdit.Value == null)
                    {

                        filterexpression += "[Date] >= #" + FromDayEdit.Date.ToString("yyyy-MM-dd") + "#";

                    }
                    else
                    {
                        if (FromDayEdit.Value == null && ToDayEdit.Value != null)
                        {

                            filterexpression += "[Date] <= #" + ToDayEdit.Date.ToString("yyyy-MM-dd") + "#";

                        }
                    }
                }

                RecordGrid.FilterExpression = filterexpression;
            }
        }

        protected void RecordGrid_ClientLayout(object sender,
        DevExpress.Web.ASPxClientLayoutArgs e)
        {
            if (e.LayoutMode == DevExpress.Web.ClientLayoutMode.Saving)
            {
                List<object> list = new List<object>();
                list.Add(RecordGrid.SaveClientLayout());
                list.Add(this.shouldExpand);
                Session["RecordGridLayout"] = list;
            }
            else
            {
                try
                {
                    List<object> list = new List<object>();
                    list = Session["RecordGridLayout"] as List<object>;
                    RecordGrid.LoadClientLayout(list[0] as string);
                    if ((bool)list[1] == true)
                    {
                        RecordGrid.ExpandAll();
                        this.shouldExpand = true;
                    }
                }
                catch
                {
                }
            }
        }

        protected void CollapseButton_Clicked(object sender, EventArgs e)
        {
            RecordGrid.CollapseAll();
            this.shouldExpand = false;
        }

        protected void ExpandButton_Clicked(object sender, EventArgs e)
        {
            RecordGrid.ExpandAll();
            this.shouldExpand = true;
        }
    }
}