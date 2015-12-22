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
using System.Web.Services;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc;
using com.commend.tools.PZE.View;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Xpo;


namespace com.commend.tools.PZE
{

    public partial class _ProjectSettings : ExtendedPage
    {
        private XPQuery<ResearchProjects> ResProjects;

        protected void Page_Init(object sender, EventArgs e)
        {
            this.SetupDataSource();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = Session["User"] as User;

            if (user == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (user.Permission.PermissionName != "admin")
                {
                    Response.Redirect("~/Account/AccessDenied.aspx");
                }
            }

            ProjectsGridview.Focus();
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.ProjectsXpoData.Session = this.DataSession;
            this.ResearchProjectsXpoData.Session = this.DataSession;
            this.StatusXpoData.Session = this.DataSession;
            this.ActivitiesXpoData.Session = this.DataSession;
            this.ProjectTypesXpoData.Session = this.DataSession;
            this.ResProjects = new XPQuery<ResearchProjects>(this.DataSession);
        }

        protected void LBActivities_DataBound(object sender, EventArgs e)
        {
            ASPxListBox listbox = sender as ASPxListBox;
            Projects project = ProjectsGridview.GetRow(ProjectsGridview.EditingRowVisibleIndex) as Projects;

            IEnumerable<ListEditItem> selectedItems = listbox.Items.Cast<ListEditItem>();

            if (project != null)
            {
                foreach (var activity in project.Activities)
                {
                    ListEditItem editItem = selectedItems.SingleOrDefault(item => int.Parse((string)item.Value) == activity.ActivityNumber);

                    if (editItem != null)
                    {
                        editItem.Selected = true;
                    }
                }
            }
        }

        protected void DetailRowExpandedChanged(object sender, ASPxGridViewDetailRowEventArgs e)
        {
            if (!e.Expanded)
            {
                return;
            }
        }

        protected void ResearchProjectsSelectionChanged(object sender, EventArgs e)
        {
            var listbox = sender as ASPxListBox;
            var selectedItem = listbox.SelectedItem;
            var project = ProjectsGridview.GetRow(ProjectsGridview.EditingRowVisibleIndex) as Projects;

            if (project != null)
            {
                ResearchProjects resProject = (new XPQuery<ResearchProjects>(this.DataSession)).SingleOrDefault(r => r.Oid == int.Parse((string)selectedItem.Value));
                project.ResearchProject = resProject;
            }
        }

        protected void LBActivities_SelIndexChanged(object sender, EventArgs e)
        {
            ASPxListBox listbox = sender as ASPxListBox;
            IEnumerable<ListEditItem> selectedItems = listbox.SelectedItems.Cast<ListEditItem>();
            Projects project = ProjectsGridview.GetRow(ProjectsGridview.EditingRowVisibleIndex) as Projects;

            if (project != null)
            {
                project.Activities.ToList().ForEach((activity) => project.Activities.Remove(activity));

                foreach (var item in selectedItems)
                {
                    Activities actActivity = (new XPQuery<Activities>(this.DataSession)).SingleOrDefault(r => r.ActivityNumber == int.Parse((string)item.Value));
                    project.Activities.Add(actActivity);
                }
            }
        }

        protected void grid_RowValidating(object sender, ASPxDataValidationEventArgs e)
        {
            foreach (GridViewDataColumn dataColumn in ProjectsGridview.Columns.OfType<GridViewDataColumn>())
            {
                int value = 0;
                //if (dataColumn.FieldName != "Oid" &&
                //    dataColumn.FieldName != string.Empty &&
                //    e.NewValues[dataColumn.FieldName] == null)
                //{
                //    e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                //    continue;
                //}

                switch (dataColumn.FieldName)
                {
                    case "ProjectName":
                        if (e.NewValues[dataColumn.FieldName] == null)
                        {
                            e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                        }
                        else if (e.NewValues[dataColumn.FieldName].ToString().Length < 2)
                        {
                            e.Errors[dataColumn] = "Der Projektname muss mindestens aus zwei Zeichen bestehen!";
                        }
                        else if (e.NewValues[dataColumn.FieldName].ToString().Length > 40)
                        {
                            e.Errors[dataColumn] = "Der Projektname darf nicht mehr als 40 Zeichen beinhalten!";
                        }
                        break;
                    case "ProjectNumber":
                        if (e.NewValues[dataColumn.FieldName] == null)
                        {
                            e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                            continue;
                        }

                        value = (int)e.NewValues[dataColumn.FieldName];
                        if (!IsProjectNumberValid(value))
                        {
                            e.Errors[dataColumn] = "Die angegebene Projektnummer ist bereits vergeben!";
                        }
                        break;
                    case "Status!Key":
                        if (e.NewValues[dataColumn.FieldName] == null)
                        {
                            e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                        }
                        break;
                    case "ResearchPercentage":
                        if (e.NewValues[dataColumn.FieldName] == null)
                        {
                            break;
                        }

                        value = (int)e.NewValues[dataColumn.FieldName];
                        if (value < 0 || value > 100)
                        {
                            e.Errors[dataColumn] = "Die Forschungsanteil muss in Prozent angegeben werden!";
                        }
                        break;
                    case "CostEstimate":
                        if (e.NewValues[dataColumn.FieldName] == null)
                        {
                            break;
                        }

                        if ((int)e.NewValues[dataColumn.FieldName] < 0)
                        {
                            e.Errors[dataColumn] = "Die Kosten können nicht negativ sein!";
                        }
                        break;
                    default:
                        break;
                }
            }

            if (e.Errors.Count > 0) e.RowError = "Um genauere Informationen ihrer falschen Eingabe zu erhalten, bewegen Sie bitte ihre Maus auf das entsprechende FehlerSymbol!";
        }


        private bool IsProjectNumberValid(int value)
        {
            var countOfSameProjNumber = new XPCollection<Projects>(this.DataSession).Count(p => p.ProjectNumber == value);
            var actProject = ProjectsGridview.GetRow(ProjectsGridview.EditingRowVisibleIndex) as Projects;
            if (actProject != null && actProject.ProjectNumber == value)
            {
                countOfSameProjNumber--;
            }

            return countOfSameProjNumber < 1;
        }

        protected void grid_StartRowEditing(object sender, ASPxStartRowEditingEventArgs e)
        {
            ProjectsGridview.DetailRows.CollapseAllRows();

            if (ProjectsGridview.IsNewRowEditing)
            {
                ProjectsGridview.DoRowValidation();
            }

            ProjectsGridview.EnableRowsCache = false;
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

        protected void newProject_Click(object sender, EventArgs e)
        {
            ProjectsGridview.AddNewRow();
        }

        protected void detailGrid_DataBinding(object sender, EventArgs e)
        {
            var grid = (ASPxGridView)sender;
            var id = (int)grid.GetMasterRowFieldValues("ProjectNumber");
            var pr = new XPCollection<Projects>(this.DataSession).SingleOrDefault(p => p.ProjectNumber == id);
            grid.DataSource = pr.Activities;
        }

        [WebMethod]
        public static bool hasProjectRecords(int oid)
        {
            return new XPCollection<Projects>(XpoHelper.GetNewSession())
                .SingleOrDefault(p => p.Oid == oid).Records.Count > 0;
        }
    }
}