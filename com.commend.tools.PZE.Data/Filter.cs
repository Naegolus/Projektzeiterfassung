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

namespace com.commend.tools.PZE.Data
{
    using DevExpress.Xpo;

    public class Filter : XPObject
    {
        public Filter(User owner, string name)
            : base()
        {
            this.FilterName = name;
            this.Owner = owner;
            this.Public = false;
        }

        public Filter(Session session)
            : base(session)
        {
        }

        public Filter(Session session, string name, User owner)
            : base(session)
        {
            this.FilterName = name;
            this.Owner = owner;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        public string FilterName { get; set; }

        [Persistent("OwnerId")]
        public User Owner { get; set; }

        [Persistent("Public")]
        public bool Public { get; set; }

        [Size(500)]
        public string FilterExpression { get; set; }

        [DbType("varchar(4096)")]
        public string GridSettings { get; set; }
    }
}