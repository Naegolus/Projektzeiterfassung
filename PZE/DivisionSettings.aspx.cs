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

    public partial class _DivisionSettings : ExtendedPage
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

            DivisionGridview.Focus();
        }

        private void SetupDataSource()
        {
            this.DivisionXpoData.Session = this.DataSession;
            this.UserXpoData.Session = this.DataSession;
        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            if (e.NewValues["DivisionName"] != null && e.NewValues["DivisionName"].ToString().Length < 2)
            {
                AddError(e.Errors, DivisionGridview.Columns["DivisionName"], "Der BereichsName muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["DivisionName"] != null && e.NewValues["DivisionName"].ToString().Length > 40)
            {
                AddError(e.Errors, DivisionGridview.Columns["DivisionName"], "Der BereichsName darf nicht mehr als 40 Zeichen beinhalten!");
            }

            if (e.Errors.Count > 0) e.RowError = "Um genauere Informationen ihrer falschen Eingabe zu erhalten, bewegen Sie bitte ihre Maus auf das entsprechende FehlerSymbol!";
        }

        protected void Grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            var division = DivisionGridview.GetRow(DivisionGridview.EditingRowVisibleIndex) as Division;
            division.DivisionName = e.NewValues["DivisionName"].ToString();

            ASPxComboBox CmbLeader = (ASPxComboBox)(DivisionGridview.FindEditRowCellTemplateControl(null, "CmbLeader"));
            division.Leader = this.DataSession.Query<User>().SingleOrDefault(p => p.Oid == Convert.ToInt32(CmbLeader.SelectedItem.Value));
            division.Save();
            e.Cancel = true;
            DivisionGridview.CancelEdit();
        }

        void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
        {
            if (errors.ContainsKey(column)) return;
            errors[column] = errorText;
        }

        protected void grid_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e)
        {
            if (!DivisionGridview.IsNewRowEditing)
            {
                DivisionGridview.DoRowValidation();
            }

        }

        protected void newDivision_Click(object sender, EventArgs e)
        {
            DivisionGridview.AddNewRow();
        }

        protected string GetUserNameByOid(object oidObject)
        {
            if (oidObject != null)
            {
                int oid = (int)oidObject;

                User user = this.DataSession.Query<User>().SingleOrDefault(p => p.Oid == oid);

                return user.UserName;
            }
            else
            {
                return "";
            }
        }

        [WebMethod]
        public static bool hasDivisionUsers(int oid)
        {
            _DivisionSettings ds = new _DivisionSettings();
            return ds.getUsers(oid);
        }

        private bool getUsers(int oid)
        {
            XPCollection<Division> divisions = new XPCollection<Division>(this.DataSession);
            Division division = divisions.SingleOrDefault(d => d.Oid == oid);
            if (division.Users.Count == 0)
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