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

using DevExpress.Xpo;

namespace com.commend.tools.PZE.Data
{
    public class PspCodes : XPObject
    {
        public PspCodes()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PspCodes(Session session)
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

        [Persistent("StatusId")]
        [Association("Status2PspCodes")]
        public Status Status { get; set; }

        [Persistent("ProjectId")]
        [Association("Project2PspCodes")]
        public Projects Projects { get; set; }

        public string PspCodeNumber { get; set; }

        public string PspCodeName { get; set; }

        [Association("PspCode2Records"), Aggregated]
        public XPCollection<Records> Records
        {
            get { return this.GetCollection<Records>("Records"); }
        }

        [Association("PspCode2Favorites"), Aggregated]
        public XPCollection<Favorites> Favorites
        {
            get { return this.GetCollection<Favorites>("Favorites"); }
        }
    }
}