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
    public partial class _ActivitySettings : ExtendedPage
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

            ActivityGridview.Focus();
        }

        /// <summary>
        /// Initilizes the xpo data source.
        /// </summary>
        private void SetupDataSource()
        {
            this.ActivitiesXpoData.Session = this.DataSession;
        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn column in ActivityGridview.Columns)
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

            if (e.NewValues["ActivityName"] != null && e.NewValues["ActivityName"].ToString().Length < 2)
            {
                AddError(e.Errors, ActivityGridview.Columns["ActivityName"], "Der Projektname muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["ActivityName"] != null && e.NewValues["ActivityName"].ToString().Length > 300)
            {
                AddError(e.Errors, ActivityGridview.Columns["ActivityName"], "Der Projektname darf nicht mehr als 40 Zeichen beinhalten!");
            }

            if (ActivityGridview.IsNewRowEditing)
            {
                if (e.NewValues["ActivityNumber"] != null && CheckeIfActivityNumberExists(e.NewValues["ActivityNumber"].ToString()) == true)
                {
                    AddError(e.Errors, ActivityGridview.Columns["ActivityNumber"], "Die angegebene Tätigkeitsnummer ist bereits vergeben!");
                }
            }
            else
            {
                if (CheckeIffActivityNumberChanged(e.NewValues["ActivityNumber"].ToString()))
                {
                    if (e.NewValues["ActivityNumber"] != null && CheckeIfActivityNumberExists(e.NewValues["ActivityNumber"].ToString()) == true)
                    {
                        AddError(e.Errors, ActivityGridview.Columns["ActivityNumber"], "Die angegebene Tätigkeitsnummer ist bereits vergeben!");
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

        private bool CheckeIfActivityNumberExists(string activityNumber)
        {
            int actNumber;
            int.TryParse(activityNumber, out actNumber);

            XPCollection<Activities> activities = new XPCollection<Activities>(this.DataSession);
            Activities activity = activities.SingleOrDefault(p => p.ActivityNumber == actNumber);

            if (activity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckeIffActivityNumberChanged(string activityNumber)
        {
            Activities activity = ActivityGridview.GetRow(ActivityGridview.EditingRowVisibleIndex) as Activities;

            int actNumber;
            int.TryParse(activityNumber, out actNumber);

            if (activity.ActivityNumber == actNumber)
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
            ActivityGridview.DetailRows.CollapseAllRows();

            if (!ActivityGridview.IsNewRowEditing)
            {
                ActivityGridview.DoRowValidation();
            }
        }

        protected void grid_ParseValue(object sender, DevExpress.Web.Data.ASPxParseValueEventArgs e)
        {
            if (e.FieldName.Equals("ActivityNumber"))
            {
                int personalId = 0;
                if (e.Value != null && !int.TryParse(e.Value.ToString(), out personalId))
                {
                    throw new Exception("Die Tätigkeitsnummer darf nur Ziffern beinhalten!");
                }
            }
        }

        protected void newActivities_Click(object sender, EventArgs e)
        {
            ActivityGridview.AddNewRow();
        }


        [WebMethod]
        public static bool hasActivityRecords(int oid)
        {
            _ActivitySettings acset = new _ActivitySettings();
            return acset.getRecords(oid);
        }

        private bool getRecords(int oid)
        {
            XPCollection<Activities> activities = new XPCollection<Activities>(this.DataSession);
            Activities activity = activities.SingleOrDefault(a => a.Oid == oid);
            if (activity.Records.Count == 0)
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