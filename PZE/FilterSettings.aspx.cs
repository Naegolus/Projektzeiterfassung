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
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.View;
    using DevExpress.Web;
    using DevExpress.Xpo;

    public partial class FilterSettings : ExtendedPage
    {
        private XPCollection<Filter> filter;

        protected void Page_Init(object sender, EventArgs e)
        {
            this.filter = new XPCollection<Filter>(this.DataSession);
            this.FilterGridview.DataSource = this.filter.Where(f => f.Owner == this.CurrentUser);
            this.FilterGridview.DataBind();
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

            FilterGridview.Focus();
        }

        protected void FilterGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            var filter = (Filter)FilterGridview.GetRow(FilterGridview.EditingRowVisibleIndex);
            var filterExpressionColumn = (GridViewDataColumn)FilterGridview.Columns["FilterExpression"];
            var filterControl = (ASPxFilterControl)FilterGridview.FindEditRowCellTemplateControl(filterExpressionColumn, "RoundPanel").FindControl("FilterControl");

            filter.Public = (bool)e.NewValues["Public"];
            filter.FilterExpression = filterControl.FilterExpression;
            filter.Save();
            e.Cancel = true;
            ((ASPxGridView)sender).CancelEdit();
        }

        protected void FilterGridview_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            var filterName = (string)e.NewValues["FilterName"];
            var filter = new Filter(this.DataSession, filterName, this.CurrentUser);
            var filterExpressionColumn = (GridViewDataColumn)FilterGridview.Columns["FilterExpression"];
            var filterControl = (ASPxFilterControl)FilterGridview.FindEditRowCellTemplateControl(filterExpressionColumn, "RoundPanel").FindControl("FilterControl");

            filter.FilterExpression = filterControl.FilterExpression;
            filter.Save();

            e.Cancel = true;
            ((ASPxGridView)sender).CancelEdit();
        }

        protected void OnFilterGridviewRowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            var filter = this.filter.FirstOrDefault(f => f.Oid == (int)e.Keys["Oid"]);
            filter.Delete();
            e.Cancel = true;
            ((ASPxGridView)sender).CancelEdit();
        }

        protected void grid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn column in FilterGridview.Columns)
            {
                GridViewDataColumn dataColumn = column as GridViewDataColumn;
                if (dataColumn == null) continue;
                if (dataColumn.FieldName == "FilterName")
                {
                    if (e.NewValues[dataColumn.FieldName] == null)
                    {
                        e.Errors[dataColumn] = "Dieses Feld bitte ausfüllen!";
                    }
                }
            }

            if (e.NewValues["FilterName"] != null && e.NewValues["FilterName"].ToString().Length < 2)
            {
                AddError(e.Errors, FilterGridview.Columns["FilterName"], "Der Filtername muss mindestens aus zwei Zeichen bestehen!");
            }
            if (e.NewValues["FilterName"] != null && e.NewValues["FilterName"].ToString().Length > 40)
            {
                AddError(e.Errors, FilterGridview.Columns["FilterName"], "Der Filtername darf nicht mehr als 40 Zeichen beinhalten!");
            }

            if (e.Errors.Count > 0) e.RowError = "Um genauere Informationen ihrer falschen Eingabe zu erhalten, bewegen Sie bitte ihre Maus auf das entsprechende FehlerSymbol!";
        }

        void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
        {
            if (errors.ContainsKey(column)) return;
            errors[column] = errorText;
        }

        protected void newFilter_Click(object sender, EventArgs e)
        {
            FilterGridview.AddNewRow();
        }
    }
}