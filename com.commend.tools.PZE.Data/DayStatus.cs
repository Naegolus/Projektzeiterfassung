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
using DevExpress.Xpo;

namespace com.commend.tools.PZE.Data
{
    public class DayStatus : XPObject
    {
        public DayStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DayStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public string StatusName { get; set; }

        //[Association("DayStatus2DaySummary"), Aggregated]
        public XPCollection<DaySummary> Days
        {
            get { return this.GetCollection<DaySummary>("Days"); }
        }
    }

    public enum DayStatus1
    {
        NoAttendence = 0,
        NotBooked = 1,
        Underbooked = 2,
        OverBooked = 3,
        OK = 4,
        MALocked = 5,
        VLocked = 6,
        ERROR = 7
    } 
}