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
    public class Projects : XPObject
    {
        public Projects()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Projects(Session session)
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

        public int ProjectNumber { get; set; }

        public string ProjectName { get; set; }

        public string AdditionalInfo { get; set; }

        [Persistent("StatusId")]
        [Association("Status2Projects")]
        public Status Status { get; set; }

        [Persistent("LeaderId")]
        public User Leader { get; set; }

        [Persistent("ProjectType")]
        [Association("ProjectType2Projects")]
        public ProjectType ProjectType { get; set; }

        [Persistent("ResearchPercentage")]
        public int ResearchPercentage { get; set; }

        [Persistent("Activatable")]
        public bool Activatable { get; set; }

        [Persistent("CostEstimate")]
        [DbTypeAttribute("money")]
        public int CostEstimate { get; set; }

        [Persistent("StartDate")]
        public DateTime StartDate { get; set; }

        [Persistent("EndDate")]
        public DateTime EndDate { get; set; }

        [Association("Project2PspCodes"), Aggregated]
        public XPCollection<PspCodes> PspCodes
        {
            get { return this.GetCollection<PspCodes>("PspCodes"); }
        }

        [Association("Project2Records"), Aggregated]
        public XPCollection<Records> Records
        {
            get { return this.GetCollection<Records>("Records"); }
        }

        [Association("Project2Favorites"), Aggregated]
        public XPCollection<Favorites> Favorites
        {
            get { return this.GetCollection<Favorites>("Favorites"); }
        }

        [Association("ResearchProject2Project")]
        public ResearchProjects ResearchProject { get; set; }

        [Association("ProjectsActivities")]
        public XPCollection<Activities> Activities
        {
            get { return this.GetCollection<Activities>("Activities"); }
        }
    }
}