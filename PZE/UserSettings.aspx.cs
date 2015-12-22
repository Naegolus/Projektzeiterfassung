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
using DevExpress.Xpo;


namespace com.commend.tools.PZE
{

    public partial class _UserSettings : ExtendedPage
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

            UserGridview.Focus();

        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn column in UserGridview.Columns)
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

            if (e.NewValues["ForName"] != null && e.NewValues["ForName"].ToString().Length < 2)
            {
                AddError(e.Errors, UserGridview.Columns["ForName"], "Der Vorname muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["ForName"] != null && e.NewValues["ForName"].ToString().Length > 40)
            {
                AddError(e.Errors, UserGridview.Columns["ForName"], "Der Vorname darf nicht mehr als 40 Zeichen beinhalten!");
            }
            if (e.NewValues["SurName"] != null && e.NewValues["SurName"].ToString().Length < 2)
            {
                AddError(e.Errors, UserGridview.Columns["SurName"], "Der Nachname muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["SurrName"] != null && e.NewValues["SurName"].ToString().Length > 40)
            {
                AddError(e.Errors, UserGridview.Columns["SurName"], "Der Nachname darf nicht mehr als 40 Zeichen beinhalten!");
            }
            if (e.NewValues["UserName"] != null && e.NewValues["UserName"].ToString().Length < 2)
            {
                AddError(e.Errors, UserGridview.Columns["UserName"], "Der User Name muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["UserName"] != null && e.NewValues["UserName"].ToString().Length > 40)
            {
                AddError(e.Errors, UserGridview.Columns["UserName"], "Der User Name darf nicht mehr als 40 Zeichen beinhalten!");
            }
            if ((e.NewValues["UserName"] != null && !LDAPHelper.IsValidUserName(e.NewValues["UserName"].ToString())) && e.NewValues["Status!Key"].Equals(1)) //allow invalid UserID on inactive Users
            {
                AddError(e.Errors, UserGridview.Columns["UserName"], "Der User Name muss einem gültigen Windows Benutzer entsprechen oder inaktiv sein!");
            }
            if (e.NewValues["Password"] != null && e.NewValues["Password"].ToString().Length < 2)
            {
                AddError(e.Errors, UserGridview.Columns["Password"], "Das Passwort muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["Password"] != null && e.NewValues["Password"].ToString().Length > 40)
            {
                AddError(e.Errors, UserGridview.Columns["Password"], "Das Passwort darf nicht mehr als 40 Zeichen beinhalten!");
            }

            int personalId = 0;
            int.TryParse(e.NewValues["PersonalId"] == null ? string.Empty : e.NewValues["PersonalId"].ToString(), out personalId);
            if (personalId > 999)
            {
                AddError(e.Errors, UserGridview.Columns["PersonalId"], "Die Personalnummer darf nicht mehr als drei Stellen besitzen!");
            }

            if (e.Errors.Count > 0) e.RowError = "Um genauere Informationen ihrer falschen Eingabe zu erhalten, bewegen Sie bitte ihre Maus auf das entsprechende FehlerSymbol!";
        }
        void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
        {
            if (errors.ContainsKey(column)) return;
            errors[column] = errorText;
        }
        protected void grid_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            if (!UserGridview.IsNewRowEditing)
            {
                UserGridview.DoRowValidation();
            }
        }

        protected void grid_ParseValue(object sender, DevExpress.Web.Data.ASPxParseValueEventArgs e)
        {
            if (e.FieldName.Equals("PersonalId"))
            {
                int personalId = 0;
                if (e.Value != null && !int.TryParse(e.Value.ToString(), out personalId))
                {
                    throw new Exception("Die PersonalNummer darf nur Ziffern beinhalten!");
                }

            }
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.UserXpoData.Session = this.DataSession;
            this.DivisionXpoData.Session = this.DataSession;
            this.PermissionsXpoData.Session = this.DataSession;
            this.StatusXpoData.Session = this.DataSession;
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

        protected void newUser_Click(object sender, EventArgs e)
        {
            UserGridview.AddNewRow();
        }

        [WebMethod]
        public static bool hasUserRecords(int oid)
        {
            _UserSettings us = new _UserSettings();
            return us.getRecords(oid);
        }

        private bool getRecords(int oid)
        {
            XPCollection<User> users = new XPCollection<User>(this.DataSession);
            User user = users.SingleOrDefault(u => u.Oid == oid);
            if (user.Records.Count == 0)
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