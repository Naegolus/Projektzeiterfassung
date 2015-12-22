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
using System.Web;
using System.Web.Security;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc;
using com.commend.tools.PZE.View;
using DevExpress.Xpo;

namespace com.commend.tools.PZE.Account
{
    public partial class Account_Login : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["User"] as User;
            txtUserName.Focus();


            if (user != null)
            {
                Response.Redirect("~/UserRecords.aspx");
            }
        }


        protected void LoginButton_Click(object sender, EventArgs e)
        {
            XPQuery<User> userQuery = new XPQuery<User>(this.DataSession);

            string domain = System.Configuration.ConfigurationManager.AppSettings["LdapDomain"];
            bool IsAuthenticated = false;
            User user;

            if (string.IsNullOrEmpty(domain))
            {
                user = userQuery.SingleOrDefault(u => u.UserName == txtUserName.Text && u.Password == txtPassword.Text);
                IsAuthenticated = (user != null);
            }
            else
            {
                user = userQuery.SingleOrDefault(u => u.UserName == txtUserName.Text);
                IsAuthenticated = LDAPHelper.IsAuthenticated(domain, txtUserName.Text, txtPassword.Text);
            }

#if DEBUG
            user = userQuery.SingleOrDefault(u => u.UserName == txtUserName.Text);
            IsAuthenticated = (user != null);
#endif

            if (IsAuthenticated)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                Session["User"] = user;
                Session["UserId"] = user.Oid;

                if (chkRememberMe.Checked)
                {
                    string sessionId = string.IsNullOrEmpty(user.SessionId) ? Guid.NewGuid().ToString() : user.SessionId;

                    user.SessionId = sessionId;
                    user.Save();

                    if (Request.Cookies.AllKeys.Contains(WebApplication.SessionIdCookieName))
                    {
                        Response.Cookies.Remove(WebApplication.SessionIdCookieName);
                    }

                    HttpCookie cookie = new HttpCookie(WebApplication.SessionIdCookieName);
                    Response.Cookies.Add(cookie);

                    cookie.Value = sessionId;
                    cookie.Expires = DateTime.Now.AddMonths(1);
                }
                Response.Redirect(@"../UserRecords.aspx");
            }
            else
            {
                litInfo.Text = "Falscher Benutzername oder Passwort!";
            }
        }
    }
}