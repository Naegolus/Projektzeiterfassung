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
using System.Web;


namespace com.commend.tools.PZE.Misc
{

    /// <summary>
    /// HttpApplication instance.
    /// </summary>
    public class WebApplication : HttpApplication
    {
        public const string SessionIdCookieName = "UserSessionId";

        public const string DefaultAdminUser = "admin";

        public const string DefaultAdminPermissionName = "admin";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplication"/> class.
        /// </summary>
        public WebApplication()
            : base()
        {
        }

        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Application_Start(Object sender, EventArgs e)
        {
            //this.CreateDefaultSetup();
        }

        public void Application_End(Object sender, EventArgs e)
        {
        }
    }
}