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
using System.DirectoryServices;

namespace com.commend.tools.PZE.Misc
{

    /// <summary>
    /// Summary description for LDAPHelper
    /// </summary>
    public class LDAPHelper
    {
        static public bool IsAuthenticated(string domain, string username, string pwd)
        {

            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry("", domainAndUsername, pwd);

            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                    return false;
            }
            catch (Exception)   //TODO: Review mit flo
            {
                return false;
            }

            return true;
        }

        static public bool IsValidUserName(string userName)
        {
            var user = System.Configuration.ConfigurationManager.AppSettings["LdapUser"];
            var pwd = System.Configuration.ConfigurationManager.AppSettings["LdapUserPwd"];
            var domain = System.Configuration.ConfigurationManager.AppSettings["LdapDomain"];


            if (!string.IsNullOrEmpty(user) &&
                !string.IsNullOrEmpty(pwd) &&
                !string.IsNullOrEmpty(domain))
            {
                try
                {
                    var domainAndUsername = domain + @"\" + user;
                    var entry = new DirectoryEntry("", domainAndUsername, pwd);
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;

                    var search = new DirectorySearcher(entry) { Filter = "(SAMAccountName=" + userName + ")" };

                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();

                    if (null == result)
                        return false;
                }
                catch (Exception) //TODO: Review mit flo: try-catch nicht zur Programmkontrolle
                {
                    return false;
                }
            }
            return true;
        }
    }
}