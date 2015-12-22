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

    public class User : XPObject
    {

        public User()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public User(Session session)
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

        [Persistent("ForName"), Size(255)]
        public string ForName { get; set; }

        public string SurName { get; set; }


        [Persistent("DivisionId")]
        [Association("Division2Users")]
        public Division Division { get; set; }

        public int PersonalId { get; set; }

        [Persistent("PermissionId")]
        [Association("Permissions2Users")]
        public Permissions Permission { get; set; }

        public string UserName { get; set; }

        public string SessionId { get; set; }

        public string Password { get; set; }

        public bool ExternalStaff { get; set; }

        [Persistent("StatusId")]
        [Association("Status2Users")]
        public Status Status { get; set; }

        [Association("User2Records"), Aggregated]
        public XPCollection<Records> Records
        {
            get { return this.GetCollection<Records>("Records"); }
        }

        [Association("User2BMDTime"), Aggregated]
        public XPCollection<BMDTime> BMDTimes
        {
            get { return this.GetCollection<BMDTime>("BMDTimes"); }
        }

        [Association("User2Favorites"), Aggregated]
        public XPCollection<Favorites> Favorites
        {
            get { return this.GetCollection<Favorites>("Favorites"); }
        }

        [Association("User2GroupMember")]
        public XPCollection<GroupMembers> GroupMembers
        {
            get { return this.GetCollection<GroupMembers>("GroupMembers"); }
        }
    }
}
