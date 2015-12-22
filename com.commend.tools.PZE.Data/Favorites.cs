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
    public class Favorites : XPObject
    {
        public Favorites()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Favorites(Session session)
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

        [Persistent("UserId")]
        [Association("User2Favorites")]
        public User User { get; set; }

        public DateTime Timestamp { get; set; }

        [Persistent("ProjectId")]
        [Association("Project2Favorites")]
        public Projects Project { get; set; }

        [Persistent("ActivityId")]
        [Association("Activity2Favorites")]
        public Activities Activity { get; set; }

        [Persistent("PspCodeId")]
        [Association("PspCode2Favorites")]
        public PspCodes PspCode { get; set; }

        [DbTypeAttribute("nvarchar(MAX)")]
        public string Memo { get; set; }
    }

    
}