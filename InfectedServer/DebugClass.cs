#define DEBUG_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.DebugClass;

namespace InfectedServer
{
    public static class DebugClass
    {
        public static void SendConsole(string message) => Log.Info(message);
    }

    #if(DEBUG_MODE)
    public class Debug : BaseScript
    {
        public Debug()
        {
            SendConsole("[InfectedServer] Info: Debug mode is enabled!");
        }

        public override EventEat OnSay2(Entity player, string name, string message)
        {
            player.GiveWeapon(GetKillstreakWeapon(message));
            player.SwitchToWeaponImmediate(GetKillstreakWeapon(message));
            return base.OnSay2(player, name, message);
        }
    }
    #endif
}
