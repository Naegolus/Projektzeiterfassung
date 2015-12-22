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
    using System.Web.UI;
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.View;
    using DevExpress.Web;
    using DevExpress.Xpo;
    using DevExpress.XtraPrinting;

    public partial class Report : ExtendedPage
    {
        private XPCollection<Records> records;
        private XPCollection<PspCodes> pspCodes;
        private XPCollection<User> users;
        private XPCollection<Activities> activities;
        private XPCollection<Projects> projects;
        private XPCollection<Filter> filter;

        #region Setup Page
        override protected void OnInit(EventArgs e)
        {
            this.FillXPObjects();
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
            else if (this.CurrentUser.Permission.PermissionName != "admin" &&
                this.CurrentUser.Permission.PermissionName != "report")
            {
                ASPxWebControl.RedirectOnCallback("~/Account/AccessDenied.aspx");
                return;
            }
            else
            {
                ReportGrid.SettingsBehavior.EnableCustomizationWindow = true;
                this.SetupDataSource();
            }

            this.HiddenField["ShowSettings"] = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var fromDate = this.FromDayEdit.Date;
            var toDate = this.ToDayEdit.Date;
            toDate = toDate == DateTime.MinValue ? DateTime.MaxValue : toDate.AddDays(1.0);

            this.ReportGrid.DataSource = this.records.Where(r => (fromDate.Ticks <= r.Date.Ticks) && (r.Date.Ticks < toDate.Ticks));
            this.ReportGrid.DataBind();
        }

        private void FillXPObjects()
        {
            this.records = new XPCollection<Records>(this.DataSession);
            this.pspCodes = new XPCollection<PspCodes>(this.DataSession);
            this.users = new XPCollection<User>(this.DataSession);
            this.activities = new XPCollection<Activities>(this.DataSession);
            this.projects = new XPCollection<Projects>(this.DataSession);
            this.filter = new XPCollection<Filter>(this.DataSession);
        }

        private void SetupDataSource()
        {
            SortingCollection sort = new SortingCollection();
            sort.Add(new SortProperty("PspCodeNumber", DevExpress.Xpo.DB.SortingDirection.Ascending));
            this.pspCodes.Sorting = sort;
        }

        protected void OnFromDayEditInit(object sender, EventArgs e)
        {
            var fromDayEdit = (ASPxDateEdit)sender;
            if (fromDayEdit.Date == DateTime.MinValue)
            {
                var today = DateTime.Today;
                fromDayEdit.Date = new DateTime(today.Year, 1, 1);    //1st of prev. month
            }
        }

        protected void OnToDayEditInit(object sender, EventArgs e)
        {
            var toDayEdit = (ASPxDateEdit)sender;
            if (toDayEdit.Date == DateTime.MinValue)
            {
                var today = DateTime.Today;
                toDayEdit.Date = new DateTime(today.Year, today.Month, 1).AddDays(-1);  //last day of prev. month
            }
        }

        protected void OnCmbFilterInit(object sender, EventArgs e)
        {
            var cmbFilter = (ASPxComboBox)sender;
            cmbFilter.DataSource = new XPCollection<Filter>(this.DataSession).Where(f => (f.Owner == this.CurrentUser) || (f.Public == true));
            cmbFilter.DataBind();
        }

        private Dictionary<string, ASPxCheckBox> BuildChkToGridConnection()
        {
            var dict = new Dictionary<string, ASPxCheckBox>();
            dict.Add("User.Division.DivisionName", this.CheckDivision);
            dict.Add("User.PersonalId", this.CheckPersNr);

            dict.Add("User.SurName", this.CheckUserSurName);
            dict.Add("User.ForName", this.CheckUserForName);
            dict.Add("Date", this.CheckDate);
            dict.Add("Duration", this.CheckDuration);
            dict.Add("Activity.ActivityNumber", this.CheckActivityNr);
            dict.Add("Activity.ActivityName", this.CheckActivityName);
            dict.Add("Memo", this.CheckMemo);
            dict.Add("PspCode.PspCodeNumber", this.CheckPSPCode);
            dict.Add("PspCode.PspCodeName", this.CheckPSPName);

            dict.Add("Project.ProjectNumber", this.CheckProjNr);
            dict.Add("Project.ProjectName", this.CheckProjName);
            dict.Add("ResearchProject.Number", this.CheckResearchProjNumber);
            dict.Add("ResearchProject.Name", this.CheckResearchProjName);
            dict.Add("EmployeeLocked", this.CheckEmployeeRelease);
            dict.Add("LeaderLocked", this.CheckLeaderRelease);
            dict.Add("User.ExternalStaff", this.CheckExternalStaff);
            dict.Add("Project.ProjectType.Name", this.CheckProjectType);
            dict.Add("Project.ResearchPercentage", this.CheckProjResearchPercentage);
            dict.Add("Project.Activatable", this.ChecProjkActivatable);
            dict.Add("Project.CostEstimate", this.CheckProjCostEstimate);
            dict.Add("Project.StartDate", this.CheckProjStartDate);
            dict.Add("Project.EndDate", this.CheckProjectEndDate);
            return dict;
        }
        #endregion

        #region Grid functionality
        protected void OnApplyButtonClicked(object source, EventArgs e)
        {
            this.ApplySelectedFilterToReportGrid();
        }

        protected void OnBeforeColumSortingGrouping(object sender, DevExpress.Web.ASPxGridViewBeforeColumnGroupingSortingEventArgs e)
        {
            ReportGrid.Settings.ShowFooter = false;
        }

        int totalsum;
        protected void CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                totalsum = 0;
            }
            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                totalsum += Convert.ToInt32(e.FieldValue);
            }

            e.TotalValue = "\u03a3 = " + GetHoursAndMinutesFromMinutes(totalsum) + " h";
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

        protected float GetHoursWithFloatingPoint(object totalMinutes)
        {
            float min = 0;
            float.TryParse(totalMinutes.ToString(), out min);
            return min / 60;
        }

        protected DateTime GetActualDate()
        {
            return DateTime.Now.Date;
        }

        protected void CollapseButton_Clicked(object sender, EventArgs e)
        {
            ReportGrid.CollapseAll();
        }

        protected void ExpandButton_Clicked(object sender, EventArgs e)
        {
            ReportGrid.ExpandAll();
        }

        private void ApplySelectedFilterToReportGrid()
        {
            var selItem = this.CmbFilter.SelectedItem;
            if (selItem != null)
            {
                var filter = this.filter.FirstOrDefault(f => f.Oid == (int)selItem.Value);
                ReportGrid.LoadClientLayout(filter.GridSettings);
                ReportGrid.FilterExpression = filter.FilterExpression;
            }

            this.SynchronizeGridWithCheckboxes();
            ReportGrid.Settings.ShowFooter = true;
            ReportGrid.DataBind();
        }

        private void SynchronizeGridWithCheckboxes(bool checkboxesAreSource = true)
        {
            var dict = this.BuildChkToGridConnection();
            foreach (var col in ReportGrid.Columns.OfType<GridViewDataColumn>())
            {
                if (dict.ContainsKey(col.FieldName))
                {
                    if (checkboxesAreSource)
                    {
                        col.Visible = dict[col.FieldName].Checked;
                    }
                    else
                    {
                        dict[col.FieldName].Checked = col.Visible;
                    }
                }
            }
        }
        #endregion

        #region Export functionality
        protected void OnExportButton_Click(object sender, EventArgs e)
        {
            this.Export();
        }

        protected void OnExportFilterClick(object sender, EventArgs e)
        {
            if (this.CmbFilter.SelectedItem != null)
            {
                var actualFilterSettings = this.ReportGrid.FilterExpression;
                this.ApplySelectedFilterToReportGrid();
                this.Export();
                this.ReportGrid.FilterExpression = actualFilterSettings;
                this.ReportGrid.DataBind();
            }
        }

        protected void OnRenderBrick(object sender, ASPxGridViewExportRenderingEventArgs e)
        {
            if (e.RowType == GridViewRowType.Data && e.Column.Caption.Equals("Dauer"))
            {
                //e.TextValue = GetHoursAndMinutesFromMinutes(e.Value);
                e.TextValue = GetHoursWithFloatingPoint(e.Value);
            }

            if (e.RowType == GridViewRowType.Data && e.Column.Caption.Equals("Datum"))
            {
                e.TextValue = e.Text.Replace("/", ".");
            }
        }

        protected void ReportPreview(object source, EventArgs e)
        {
            Response.Redirect("~/ReportPreview.aspx");
        }

        private void Export()
        {
            if (CmbExport.SelectedItem.Value.ToString().Equals("Pdf"))
            {
                gridExport.WritePdfToResponse();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Xls"))
            {
                gridExport.WriteXlsToResponse(new XlsExportOptionsEx() { ExportType = DevExpress.Export.ExportType.WYSIWYG });
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Xlsx"))
            {
                gridExport.WriteXlsxToResponse(new XlsxExportOptionsEx() { ExportType = DevExpress.Export.ExportType.WYSIWYG });
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Rtf"))
            {
                gridExport.WriteRtfToResponse();
            }
            else if (CmbExport.SelectedItem.Value.ToString().Equals("Csv"))
            {
                gridExport.WriteCsvToResponse();
            }
        }
        #endregion

        #region Edit filters

        protected void OnCmbEditFilterSelectedIndexChanged(object sender, EventArgs e)
        {
            var selItem = ((ASPxComboBox)sender).SelectedItem;
            if (selItem == null)
            {
                return;
            }

            var filter = this.filter.FirstOrDefault(f => f.Oid == (int)selItem.Value);
            this.BindFilterExpression(filter.FilterExpression);
            this.SynchronizeGridWithCheckboxes(false);
            this.HiddenField["ShowSettings"] = true;
        }

        protected void OnBtnSaveFilterClick(object sender, EventArgs e)
        {
            var filter = this.GetSelectedFilterToEdit();
            if (filter != null)
            {
                filter.GridSettings = ReportGrid.SaveClientLayout();
                filter.FilterExpression = this.FilterControl.FilterExpression;
                filter.Save();
                CmbFilter.DataSource =
                    CmbEditFilter.DataSource = new XPCollection<Filter>(this.DataSession)
                    .Where(f => (f.Owner == this.CurrentUser) || (f.Public == true));

                CmbEditFilter.DataBind();
                CmbFilter.DataBind();
            }

            this.HiddenField["ShowSettings"] = true;
            this.BindFilterExpression(filter.FilterExpression);
        }

        protected void OnSaveFilterNameClick(object sender, EventArgs e)
        {
            var filterName = this.GetValidFilterName(this.TENewFilterName.Text);
            Filter filter = null;
            if ((string)this.HiddenField["Mode"] == "Rename")
            {
                filter = this.GetSelectedFilterToEdit();
                if (filter != null)
                {
                    filter.FilterName = filterName;
                    filter.Save();
                }
            }
            else if ((string)this.HiddenField["Mode"] == "New")
            {
                if (!string.IsNullOrWhiteSpace(filterName))
                {
                    filter = new Filter(this.DataSession, filterName, this.CurrentUser);
                    filter.GridSettings = ReportGrid.SaveClientLayout();
                    filter.FilterExpression = this.FilterControl.FilterExpression;
                    filter.Save();
                }
            }

            CmbEditFilter.DataSource =
                CmbFilter.DataSource = new XPCollection<Filter>(this.DataSession)
                    .Where(f => (f.Owner == this.CurrentUser) || (f.Public == true));

            CmbEditFilter.DataBind();
            CmbFilter.DataBind();

            this.FilterNamePopup.ShowOnPageLoad = false;
            this.HiddenField["ShowSettings"] = true;

            this.BindFilterExpression(filter.FilterExpression);
        }

        private void BindFilterExpression(string filterExpression)
        {
            this.FilterControl.FilterExpression = string.Empty; // Otherwise new set doesnt work
            this.FilterControl.FilterExpression = filterExpression;
            this.FilterControl.DataBind();
        }

        private Filter GetSelectedFilterToEdit()
        {
            var selectedFilter = this.CmbEditFilter.SelectedItem;
            if (selectedFilter == null)
            {
                return null;
            }

            return this.filter.FirstOrDefault(f => f.Owner == this.CurrentUser && f.FilterName == selectedFilter.Text);
        }

        private string GetValidFilterName(string filterName)
        {
            if (this.filter.Where(f => f.FilterName == filterName).Count() > 0)
            {
                filterName = this.GetValidFilterName(filterName + "_");
            }

            return filterName;
        }
        #endregion
    }
}