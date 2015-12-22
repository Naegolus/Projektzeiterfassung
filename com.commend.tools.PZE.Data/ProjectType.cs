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

    public class ProjectType : XPObject
    {
        public ProjectType()
            : base()
        {
        }

        public ProjectType(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        public string Name { get; set; }

        [Association("ProjectType2Projects"), Aggregated]
        public XPCollection<Projects> Projects
        {
            get { return this.GetCollection<Projects>("Projects"); }
        }
    }
}
