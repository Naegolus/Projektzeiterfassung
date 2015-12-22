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
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Mail;
    using System.Web;
    using System.Web.UI.WebControls;
    using com.commend.tools.PZE.Data;
    using com.commend.tools.PZE.View;
    using DevExpress.Xpo;

    public partial class ConfirmBMDRecords : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["caller"] == "Server")
            {
                ListDictionary repl = new ListDictionary();
                repl.Add("<%month%>", (DateTime.Today.Month - 1).ToString());
                var md = new MailDefinition()
                {
                    Subject = "Bestätigung der BMD-Buchungen",
                    From = "projektzeit@commend.com",
                    BodyFileName = HttpContext.Current.Server.MapPath("~/App_Data/templates/RequireBMDConfirmation.txt")
                };

                new SmtpClient("192.168.170.12 ").Send(
                    md.CreateMailMessage("l.stranger@commend.com",
                    repl,
                    this));
            }
            else if (Request.QueryString["month"] == (DateTime.Today.Month - 1).ToString())
            {
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                new XPCollection<BMDTime>(this.DataSession).Where(d => d.TimeVerified == false && d.LoggingDay.Ticks < firstDayOfMonth.Ticks).ToList().ForEach(
                    day =>
                    {
                        day.TimeVerified = true;
                        day.Save();
                    });
            }
        }
    }
}