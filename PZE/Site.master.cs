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
using System.Linq;
using System.Web.Security;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc;
using DevExpress.Xpo;

namespace com.commend.tools.PZE
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        override protected void OnInit(EventArgs e)
        {
            this.Init += new System.EventHandler(this.Page_Init);
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            User user = Session["User"] as User;
            if (user == null)
            {
                if (Request.Cookies.AllKeys.Contains(WebApplication.SessionIdCookieName))
                {
                    SilentLogin(Request.Cookies[WebApplication.SessionIdCookieName].Value);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            User user = Session["User"] as User;
            if (user == null)
            {
                NavigationMenu.Visible = false;
                LogoutLinkButton.Visible = false;
            }
            else
            {
                if (user.Permission.PermissionName == "user")
                {
                    NavigationMenu.Items[1].Visible = false;
                    NavigationMenu.Items[2].Visible = false;
                }
                else
                {
                    if (user.Permission.PermissionName == "report")
                    {
                        NavigationMenu.Items[2].Visible = false;
                    }
                }

                accountInfo.Text = String.Format("{0} {1}", user.ForName, user.SurName);
                LogoutLinkButton.Visible = true;

                if (user.Division.Leader == user)
                {
                    NavigationMenu.Items[0].Items[3].Visible = true;
                }
                else
                {
                    NavigationMenu.Items[0].Items[3].Visible = false;
                }
            }
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            Session["User"] = null;
            Session["UserId"] = null;
            Response.Cookies[WebApplication.SessionIdCookieName].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies[WebApplication.SessionIdCookieName].Value = string.Empty;
            Response.Redirect("~/Account/Login.aspx");
        }

        private void SilentLogin(string sessionId)
        {
            XPQuery<User> userQuery = new XPQuery<User>(XpoHelper.GetNewSession());

            User user = userQuery.SingleOrDefault(u => u.SessionId == sessionId);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                Session["User"] = user;
                Session["UserId"] = user.Oid;

                Response.Cookies[WebApplication.SessionIdCookieName].Expires = DateTime.Now.AddMonths(1);
                Response.Cookies[WebApplication.SessionIdCookieName].Value = user.SessionId;
            }
        }
    }

}