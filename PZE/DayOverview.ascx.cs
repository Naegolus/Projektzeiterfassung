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

namespace com.commend.tools.PZE
{
    using System;

    public partial class DayOverview : System.Web.UI.UserControl
    {
        public int SelectedDays { get; set; }

        public int AttendenceHours { get; set; }

        public int BookedHours { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.LblSelDays.Text = this.SelectedDays.ToString();
            this.LblSelAttendenceHours.Text = this.AttendenceHours.ToString();
            this.LblSelBookedHours.Text = this.LblSelBookedHours.ToString();
        }
    }
}