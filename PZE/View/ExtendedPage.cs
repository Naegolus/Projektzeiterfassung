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

using System.Linq;
using System.Web.UI;
using com.commend.tools.PZE.Data;
using com.commend.tools.PZE.Misc;
using DevExpress.Xpo;

namespace com.commend.tools.PZE.View
{

    /// <summary>
    /// Extends the default asp.net-page.
    /// </summary>
    public class ExtendedPage : Page
    {
        /// <summary>
        /// XPO data session - backing field.
        /// </summary>
        private Session dataSession;

        /// <summary>
        /// Current logged in user.
        /// </summary>
        private User currentUser;

        public ExtendedPage()
        {
        }

        /// <summary>
        /// Gets the XPO data session.
        /// </summary>
        public Session DataSession
        {
            get
            {
                if (this.dataSession == null)
                {
                    this.dataSession = XpoHelper.GetNewSession();
                    this.dataSession.CreateObjectTypeRecords(typeof(Projects).Assembly);
                }

                return this.dataSession;
            }
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if ((Session["UserId"] == null) ||
                    (string.IsNullOrEmpty(Session["UserId"].ToString())))
                {
                    return null;
                }

                int userId;
                if (!int.TryParse(Session["UserId"].ToString(), out userId))
                {
                    return null;
                }

                if (this.currentUser == null)
                {
                    XPQuery<User> query = new XPQuery<User>(this.DataSession);

                    this.currentUser = query.SingleOrDefault(u => u.Oid == userId);
                }

                return this.currentUser;
            }
        }

        /// <summary>
        /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter"/> object and calls on the child controls of the <see cref="T:System.Web.UI.Page"/> to render.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (this.dataSession != null)
            {
                this.dataSession.Dispose();
            }
        }
    }
}