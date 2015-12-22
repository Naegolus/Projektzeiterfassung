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
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using com.commend.tools.PZE.Data;

namespace com.commend.tools.PZE.View
{

    /// <summary>
    /// Summary description for HistoryGridView
    /// </summary>
    public class HistoryGridView : XPLiteObject
    {
        public HistoryGridView(Session session)
            : base(session)
        {
        }

        [Key, Persistent, Browsable(false)]
        public int RecordID { get; set; }

        [Persistent("UserID")]
        public User User { get; set; }

        [Persistent("RecordDate")]
        public DateTime RecordDate { get; set; }

        [Persistent("Project")]
        public Projects Project { get; set; }

        [Persistent("PspCode")]
        public PspCodes PspCode { get; set; }

        [Persistent("Activity")]
        public Activities Activity { get; set; }

        [Persistent("Memo")]
        public string Memo { get; set; }

        [Persistent("Duration")]
        public int Duration { get; set; }
    } 
}