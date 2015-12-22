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
using System.Linq;
using com.commend.tools.PZE.Data;
using DevExpress.Xpo;

namespace com.commend.tools.PZE.Misc
{

    public class UpdateDatabase
    {
        public UpdateDatabase()
        {
        }

        public void TransformDivisionToGroup(Session dataSession)
        {
            var divisions = new XPCollection<Division>(dataSession);
            foreach (Division div in divisions)
            {
                Group gp = new Group(dataSession);
                gp.GroupName = div.DivisionName;
                //gp.Leader = div.Leader;
                gp.Save();
            }
        }

        public void ExtractDivisionFromUser2GroupMember(Session dataSession)
        {
            var users = new XPCollection<User>(dataSession);
            foreach (User user in users)
            {
                GroupMembers gm = new GroupMembers(dataSession);
                //gm.Users.Add(user);
                gm.User = user;
                var group = new XPCollection<Group>(dataSession).SingleOrDefault(g => g.GroupName == user.Division.DivisionName);
                //gm.Groups.Add(group);
                gm.Group = group;
                gm.Save();
            }
        }

        public void CreateDayStatus(Session dataSesson)
        {
            DayStatus status1 = new DayStatus(dataSesson);
            status1.StatusName = "NoAttendence";
            status1.Save();

            DayStatus status2 = new DayStatus(dataSesson);
            status2.StatusName = "Notbooked";
            status2.Save();

            DayStatus status3 = new DayStatus(dataSesson);
            status3.StatusName = "UnderBooked";
            status3.Save();

            DayStatus status4 = new DayStatus(dataSesson);
            status4.StatusName = "OverBooked";
            status4.Save();

            DayStatus status5 = new DayStatus(dataSesson);
            status5.StatusName = "OK";
            status5.Save();

            DayStatus status6 = new DayStatus(dataSesson);
            status6.StatusName = "EmployeeLocked";
            status6.Save();

            DayStatus status7 = new DayStatus(dataSesson);
            status7.StatusName = "LeaderLocked";
            status7.Save();

            DayStatus status8 = new DayStatus(dataSesson);
            status8.StatusName = "ERROR";
            status8.Save();
        }

        public void LockAllRecordsCreatedBefore2015()
        {
            UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork();
            XPCollection<Records> records = new XPCollection<Records>(unitOfWork);
            var reference = new DateTime(2015, 1, 1).Ticks;
            foreach (var rec in records)
            {
                if (rec.Date.Ticks < reference)
                {
                    rec.EmployeeLocked = DateTime.Now;
                    rec.LeaderLocked = DateTime.Now;
                    rec.Save();
                }
            }

            unitOfWork.CommitChanges();
        }

        public void SetExternalStaff()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            var gerhard = new XPCollection<User>(uow).Single(u => u.SurName == "Brennsteiner");
            gerhard.ExternalStaff = true;
            gerhard.Save();

            var jozo = new XPCollection<User>(uow).Single(u => u.SurName == "Lagetar");
            jozo.ExternalStaff = true;
            jozo.Save();
            uow.CommitChanges();
        }
    }
}