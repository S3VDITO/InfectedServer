using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace InfectedServer
{
    public static class DebugClass
    {
        public static bool DebugMode = false;
        public static void SendConsole(string message) => Log.Info(message);
    }
}
