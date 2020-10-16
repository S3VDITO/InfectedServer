using InfinityScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfectedServer
{
    public class PlayerModel
    {
        public Entity Entity { get; }

        public ushort KillStreaks { get; set; } = 0;
        public ushort DeathStreaks { get; set; } = 0;

        public string CurrentSessionTeam => Entity.SessionTeam;

        public Dictionary<string, string> Kit { get; }

        public bool IsHuman => CurrentSessionTeam == "allies";
        public bool IsInfected => CurrentSessionTeam == "axis";

        public PlayerModel(Entity player, Dictionary<string, string> kit)
        {
            if (!player.IsPlayer)
            {
                Log.Error("Player(Entity) - Entity is not player!");
                return;
            }

            Kit = kit;
            Entity = player;
        }
    }
}
