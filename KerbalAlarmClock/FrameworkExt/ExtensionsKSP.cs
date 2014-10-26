using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contracts;

namespace KerbalAlarmClock
{
    public static class ExtensionsKSP
    {
        #region Contracts

        //public static Double TimeNext(this Contract c)
        //{
        //    if (c.ContractState == Contract.State.Offered)
        //        return c.TimeExpiry;
        //    else if (c.ContractState == Contract.State.Active)
        //        return c.TimeDeadline;
        //    else
        //        return 0;
        //}
        public static Double DateNext(this Contract c)
        {
            if (c.ContractState == Contract.State.Offered)
                return c.DateExpire;
            else if (c.ContractState == Contract.State.Active)
                return c.DateDeadline;
            else
                return 0;
        }

        public static KACAlarm.ContractAlarmTypeEnum AlarmType(this Contract c)
        {
            if (c.ContractState == Contract.State.Offered)
                return KACAlarm.ContractAlarmTypeEnum.Expire;
            else if (c.ContractState == Contract.State.Active)
                return KACAlarm.ContractAlarmTypeEnum.Deadline;
            else
                return KACAlarm.ContractAlarmTypeEnum.Expire;
        }

        #endregion


    }
}
