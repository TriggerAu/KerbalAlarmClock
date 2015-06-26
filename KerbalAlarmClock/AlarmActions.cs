using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    public class AlarmActions
    {
        public AlarmActions() { }
        public AlarmActions(WarpEnum Warp, MessageEnum Message, Boolean PlaySound, Boolean DeleteWhenDone) {
            this.Warp = Warp;
            this.Message = Message;
            this.PlaySound = PlaySound;
            this.DeleteWhenDone = DeleteWhenDone;
        }

        [Persistent] public WarpEnum Warp = WarpEnum.KillWarp;
        [Persistent] public MessageEnum Message = MessageEnum.Yes;
        [Persistent] public Boolean DeleteWhenDone = false;
        [Persistent] public Boolean PlaySound = false;

        public override string ToString()
        {
            return String.Format("{0}-{1}-{2}-{3}",Warp,Message,PlaySound,DeleteWhenDone);
        }

        public enum WarpEnum
        {
            [Description("Do Nothing")]             DoNothing,
            [Description("Kill Warp")]              KillWarp,
            [Description("Pause Game")]             PauseGame
        }

        public enum MessageEnum
        {
            [Description("No Message")]                             No,
            [Description("Display Message")]                        Yes,
            [Description("Show Message if vessel not active")]      YesIfOtherVessel

        }

        public AlarmActions Duplicate()
        {
            return new AlarmActions(this.Warp, this.Message, this.PlaySound, this.DeleteWhenDone);
        }

        public static AlarmActions DefaultsKillWarpOnly()
        {
            return new AlarmActions(WarpEnum.KillWarp, MessageEnum.No, false, false);
        }
        public static AlarmActions DefaultsKillWarp()
        {
            return new AlarmActions(WarpEnum.KillWarp, MessageEnum.Yes, false, false);
        }
        public static AlarmActions DefaultsPauseGame()
        {
            return new AlarmActions(WarpEnum.PauseGame, MessageEnum.Yes, false, false);
        }
        public static AlarmActions DefaultsDoNothing()
        {
            return new AlarmActions(WarpEnum.DoNothing, MessageEnum.No, false, false);
        }
    }
}
