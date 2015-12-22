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
    public class Status : XPObject
    {
        public Status()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Status(Session session)
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

        [Association("Status2Users"), Aggregated]
        public XPCollection<User> Users
        {
            get { return this.GetCollection<User>("Users"); }
        }

        [Association("Status2PspCodes"), Aggregated]
        public XPCollection<PspCodes> PspCodes
        {
            get { return this.GetCollection<PspCodes>("PspCodes"); }
        }

        [Association("Status2Projects"), Aggregated]
        public XPCollection<Projects> Projects
        {
            get { return this.GetCollection<Projects>("Projects"); }
        }
    }
}