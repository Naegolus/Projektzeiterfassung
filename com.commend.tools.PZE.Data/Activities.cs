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
    public class Activities : XPObject
    {
        public Activities()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Activities(Session session)
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

        [Association("Project2Activities")]
        [Persistent("ActivityNumber")]
        public int ActivityNumber { get; set; }

        public string ActivityName { get; set; }

        [Association("Activity2Records"), Aggregated]
        public XPCollection<Records> Records
        {
            get { return this.GetCollection<Records>("Records"); }
        }

        [Association("Activity2Favorites"), Aggregated]
        public XPCollection<Favorites> Favorites
        {
            get { return this.GetCollection<Favorites>("Favorites"); }
        }

        [Association("ProjectsActivities")]
        public XPCollection<Projects> Projects
        {
            get { return this.GetCollection<Projects>("Projects"); }
        }
    }
}