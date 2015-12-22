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

namespace com.commend.tools.PZE.Misc.TTConnection
{
    public enum entryType
    { 
        entry,
        functionalRq,
        non_functionalRq,
        functionalSpc,
        non_functionalSpc,
        task,
        story
    }

/// <summary>
/// Summary description for TTImportData
/// </summary>
    public class TtImportData
    {
        static long _idCounter;
        private long _id;

        public TtImportData()
        {
            _id = _idCounter++;
        }

        public long Id
        {
            get
            {
                return _id;
            }

            private set
            {
                _id = value;
            }
        }

        public long Nr { get; set; }
        public String Summary { get; set; }
        public entryType EntryType { get; set; }

        public String Name { get; set; }
        public DateTime Date { get; set; }
        private String _note;
        public String Note
        {
            set
            {
                _note = value;
            }
        }

        public String Text
        {
            get
            {
                return Summary + '\n' + _note;
            }
        }


        //public String ProjectCode { get; set; }
        public String Project { get; set; }
        //public String PSP { get; set; }
        //public String PSPCode { get; set; }
        //public String Activity { get; set; }

        private Double _effort;
        public Double Effort
        {
            get
            {
                return _effort;
            }
            set
            {
                //keine negativen Werte zulassen
                if (value < 0)
                {
                    value = 0;
                }

                _effort = value;
            }
        }


        public DateTime EffortTime
        {
            get 
            {
                var returnDate = new DateTime();
                return returnDate.AddHours(Effort);
            }
            set
            {
                _effort = (value - new DateTime()).TotalHours;
            }
        }
    }
}

