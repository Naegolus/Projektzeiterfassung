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

namespace com.commend.tools.PZE.Misc.TTConnection.TTFilters
{
    public enum TtFilterType
    { 
        entry,
        requirement
    }

    /// <summary>
    /// parent abstract filter class
    /// </summary>
    abstract public class TtFilter
    {
        protected TtFilterType FilterType
        {
            get;
            set;
        }

        /// <summary>
        /// returns the identifikation string of the TestTrack-Filter
        /// </summary>
        /// <returns>identifikation string</returns>
        public abstract String getFilterName(TtFilterType FilterType = TtFilterType.entry);


        /// <summary>
        /// get the start date of the Filter
        /// </summary>
        public abstract DateTime StartDate { get; }
        
        /// <summary>
        /// get the end date of the Filter
        /// </summary>
        public abstract DateTime EndDate { get; }
    } 
}
