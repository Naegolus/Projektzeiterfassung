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
using System.ComponentModel;
using DevExpress.Xpo;

namespace com.commend.tools.PZE.Data
{
    /// <summary>
    /// Summary description for DaySummary
    /// </summary>
    public class DaySummary : XPLiteObject
    {
        public DaySummary(Session session)
            : base(session)
        {
        }

        [Key, Persistent, Browsable(false)]
        public DaySummaryKey Key;

        public User UserID { get { return Key.UserID; } }

        public DateTime LoggingDay { get { return Key.LoggingDay; } }

        [Persistent("AttendenceTime")]
        public int AttendenceTime { get; private set; }

        [Persistent("AttendenceHours")]
        public int AttendenceHours { get; private set; }

        [Persistent("AttendenceMinutes")]
        public int AttendenceMinutes { get; private set; }

        [Persistent("BookedTime")]
        public int BookedTime { get; private set; }

        [Persistent("BookedHours")]
        public int BookedHours { get; private set; }

        [Persistent("BookedMinutes"), Browsable(false)]
        public int BookedMinutes { get; private set; }

        [Persistent("DayStatus")]
        public int DayStatus { get; private set; }

        [Persistent("TimeVerified")]
        public bool TimeVerified { get; private set; }
    }

    public struct DaySummaryKey
    {
        [Persistent("UserID"), Browsable(false)]
        public User UserID;

        [Persistent("LoggingDay"), Browsable(false)]
        public DateTime LoggingDay;
    }
}