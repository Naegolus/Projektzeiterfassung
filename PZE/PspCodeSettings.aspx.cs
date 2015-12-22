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
using DevExpress.Web;
using DevExpress.Xpo;
using com.commend.tools.PZE.View;
using com.commend.tools.PZE.Data;


namespace com.commend.tools.PZE
{

    public partial class _PspCodeSettings : ExtendedPage
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

            PspCodeGridview.Focus();
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.ProjectsXpoData.Session = this.DataSession;
            this.PspCodesXpoData.Session = this.DataSession;
            this.StatusXpoData.Session = this.DataSession;
        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn column in PspCodeGridview.Columns)
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

            if (e.NewValues["PspCodeName"] != null && e.NewValues["PspCodeName"].ToString().Length < 2)
            {
                AddError(e.Errors, PspCodeGridview.Columns["PspCodeName"], "Der PspCodeName muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["PspCodeName"] != null && e.NewValues["PspCodeName"].ToString().Length > 100)
            {
                AddError(e.Errors, PspCodeGridview.Columns["PspCodeName"], "Der PspCodeName darf nicht mehr als 100 Zeichen beinhalten!");
            }

            if (PspCodeGridview.IsNewRowEditing)
            {
                if (e.NewValues["PspCodeNumber"] != null && CheckeIfPspCodeExists(e.NewValues["PspCodeNumber"].ToString(), e.NewValues["Projects!Key"].ToString()) == true)
                {
                    AddError(e.Errors, PspCodeGridview.Columns["PspCodeNumber"], "Den angegebenen PsPCode gibt es bereits für das zugeordnete Projekt!");
                }
            }
            else
            {
                if (CheckeIfPspCodeChanged(e.NewValues["PspCodeNumber"].ToString()) || CheckeIfProjectChanged(e.NewValues["Projects!Key"].ToString()))
                {
                    if (e.NewValues["PspCodeNumber"] != null && CheckeIfPspCodeExists(e.NewValues["PspCodeNumber"].ToString(), e.NewValues["Projects!Key"].ToString()) == true)
                    {
                        AddError(e.Errors, PspCodeGridview.Columns["PspCodeNumber"], "Den angegebenen PsPCode gibt es bereits für das zugeordnete Projekt!");
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

        private bool CheckeIfPspCodeExists(string pspCodeNumber, string projectId)
        {
            int oid;
            int.TryParse(projectId, out oid);

            XPCollection<Projects> projects = new XPCollection<Projects>(this.DataSession);
            Projects project = projects.SingleOrDefault(p => p.Oid == oid);

            XPCollection<PspCodes> pspcodes = new XPCollection<PspCodes>(this.DataSession);
            PspCodes pspcode = pspcodes.SingleOrDefault(p => (p.PspCodeNumber == pspCodeNumber) && (p.Projects == project));

            if (pspcode != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckeIfPspCodeChanged(string pspCodeNumber)
        {
            PspCodes pspCode = PspCodeGridview.GetRow(PspCodeGridview.EditingRowVisibleIndex) as PspCodes;

            if (pspCode.PspCodeNumber.Equals(pspCodeNumber))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckeIfProjectChanged(string projectOid)
        {
            PspCodes pspCode = PspCodeGridview.GetRow(PspCodeGridview.EditingRowVisibleIndex) as PspCodes;

            int oid;
            int.TryParse(projectOid, out oid);

            if (pspCode.Projects.Oid == oid)
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
            PspCodeGridview.DetailRows.CollapseAllRows();

            if (!PspCodeGridview.IsNewRowEditing)
            {
                PspCodeGridview.DoRowValidation();
            }

        }

        //protected void grid_UnGrouping(object sender, EventArgs e)
        //{
        //    PspCodeGridview.ClearSort();
        //}

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

        protected void newPspCode_Click(object sender, EventArgs e)
        {
            PspCodeGridview.AddNewRow();
        }

        [WebMethod]
        public static bool hasPspCodeRecords(int oid)
        {
            _PspCodeSettings pspcs = new _PspCodeSettings();
            return pspcs.getRecords(oid);
        }

        private bool getRecords(int oid)
        {
            XPCollection<PspCodes> pspCodes = new XPCollection<PspCodes>(this.DataSession);
            PspCodes pspCode = pspCodes.SingleOrDefault(pc => pc.Oid == oid);
            if (pspCode.Records.Count == 0)
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