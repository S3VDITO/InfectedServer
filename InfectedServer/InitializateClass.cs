using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InfinityScript;

using static InfectedServer.MemoryClass;
using static InfectedServer.LevelClass;

namespace InfectedServer
{
    public class InitializateClass
    {
        public InitializateClass()
        {
            Function.Call("PreCacheShader", "specialty_carepackage_crate");
            Function.Call("PreCacheShader", "iw5_cardicon_medkit");
            Function.Call("PreCacheShader", "iw5_cardicon_juggernaut_a");

            Function.Call("PreCacheItem", "at4_mp");

            InitializateMapping();
        }
    }
}
