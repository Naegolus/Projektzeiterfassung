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
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using DevExpress.Web;
using com.commend.tools.PZE.View;
using com.commend.tools.PZE.Data;


namespace com.commend.tools.PZE
{

    public partial class _ResearchProjectSettings : ExtendedPage
    {
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

            ResearchProjectsGridview.Focus();
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.ResearchProjectsXpoData.Session = this.DataSession;
        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn column in ResearchProjectsGridview.Columns)
            {
                GridViewDataColumn dataColumn = column as GridViewDataColumn;
                if (dataColumn == null) continue;
                if (dataColumn.FieldName != "Oid")
                {
                    if (e.NewValues[dataColumn.FieldName] == null)
                    {
                        e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                    }
                }

            }

            if (e.NewValues["ResearchProjectName"] != null && e.NewValues["ResearchProjectName"].ToString().Length < 2)
            {
                AddError(e.Errors, ResearchProjectsGridview.Columns["ResearchProjectName"], "Der Projektname muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["ResearchProjectName"] != null && e.NewValues["ResearchProjectName"].ToString().Length > 300)
            {
                AddError(e.Errors, ResearchProjectsGridview.Columns["ResearchProjectName"], "Der Projektname darf nicht mehr als 300 Zeichen beinhalten!");
            }

            if (ResearchProjectsGridview.IsNewRowEditing)
            {
                if (e.NewValues["ResearchProjectNumber"] != null && CheckeIfResearchProjectNumberExists(e.NewValues["ResearchProjectNumber"].ToString()) == true)
                {
                    AddError(e.Errors, ResearchProjectsGridview.Columns["ResearchProjectNumber"], "Die angegebene ForschungsProjektnummer ist bereits vergeben!");
                }
            }
            else
            {
                if (CheckeIfResearchProjectNumberChanged(e.NewValues["ResearchProjectNumber"].ToString()))
                {
                    if (e.NewValues["ResearchProjectNumber"] != null && CheckeIfResearchProjectNumberExists(e.NewValues["ResearchProjectNumber"].ToString()) == true)
                    {
                        AddError(e.Errors, ResearchProjectsGridview.Columns["ResearchProjectNumber"], "Die angegebene ForschungsProjektnummer ist bereits vergeben!");
                    }
                }
            }

            if (e.Errors.Count > 0) e.RowError = "Um genauere Informationen ihrer falschen Eingabe zu erhalten, bewegen Sie bitte ihre Maus auf das entsprechende FehlerSymbol!";
        }
        void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
        {
            if (errors.ContainsKey(column)) return;
            errors[column] = errorText;
        }

        private bool CheckeIfResearchProjectNumberExists(string researchProjectNumber)
        {
            int resProjNumber;
            int.TryParse(researchProjectNumber, out resProjNumber);

            XPCollection<ResearchProjects> researchProjects = new XPCollection<ResearchProjects>(this.DataSession);
            ResearchProjects researchProject = researchProjects.SingleOrDefault(p => p.ResearchProjectNumber == resProjNumber);

            if (researchProject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckeIfResearchProjectNumberChanged(string researchProjectNumber)
        {
            ResearchProjects researchProject = ResearchProjectsGridview.GetRow(ResearchProjectsGridview.EditingRowVisibleIndex) as ResearchProjects;

            int resProjNumber;
            int.TryParse(researchProjectNumber, out resProjNumber);

            if (researchProject.ResearchProjectNumber == resProjNumber)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        protected void grid_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            ResearchProjectsGridview.DetailRows.CollapseAllRows();

            if (!ResearchProjectsGridview.IsNewRowEditing)
            {
                ResearchProjectsGridview.DoRowValidation();
            }

        }

        protected void grid_ParseValue(object sender, DevExpress.Web.Data.ASPxParseValueEventArgs e)
        {
            if (e.FieldName.Equals("ResearchProjectNumber"))
            {
                int personalId = 0;
                if (e.Value != null && !int.TryParse(e.Value.ToString(), out personalId))
                {
                    throw new Exception("Die ProjektNummer darf nur Ziffern beinhalten!");
                }
            }
        }

        protected void newResearchProject_Click(object sender, EventArgs e)
        {
            ResearchProjectsGridview.AddNewRow();
        }

        [WebMethod]
        public static bool hasResearchProjectProjects(int oid)
        {
            _ResearchProjectSettings rps = new _ResearchProjectSettings();
            return rps.getRecords(oid);
        }

        private bool getRecords(int oid)
        {
            XPCollection<ResearchProjects> researchProjects = new XPCollection<ResearchProjects>(this.DataSession);
            ResearchProjects researchProject = researchProjects.SingleOrDefault(rp => rp.Oid == oid);
            if (researchProject.Projects.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    } 
}