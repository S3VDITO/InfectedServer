using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfinityScript.ThreadScript;

namespace InfectedServer
{
    public class InfectedServer : BaseScript
    {
        public InfectedServer()
        {
            Utilities.PrintToConsole("###############################");
            Utilities.PrintToConsole("InfectedServer by S3VDITO");
            Utilities.PrintToConsole("Source code: ....");
            Utilities.PrintToConsole("###############################");
            Utilities.PrintToConsole("\n\n");
            Utilities.PrintToConsole("###############################");
            Utilities.PrintToConsole("InfectedServer use only IS v2.0");
            Utilities.PrintToConsole("Source code: ....");
            Utilities.PrintToConsole("###############################");
            Utilities.PrintToConsole("\n\n");

            PlayerConnected += OnPlayerConnected;
        }

        private void OnPlayerConnected(Entity entity)
        {
            Player player = new Player(entity);
            player.GiveKit();

            player.SpawnedPlayer += OnPlayerSpawned;
        }

        private void OnPlayerSpawned(Player player)
        {
            player.GiveKit();
        }

        public override string OnPlayerRequestConnection(string playerName, string playerHWID, string playerXUID, string playerIP, string playerSteamID, string playerXNAddress)
        {
            return base.OnPlayerRequestConnection(playerName, playerHWID, playerXUID, playerIP, playerSteamID, playerXNAddress);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            base.OnPlayerDamage(player, inflictor, attacker, damage, dFlags, mod, weapon, point, dir, hitLoc);
        }

        public override void OnSay(Entity player, string name, string message)
        {
            switch (message)
            {
                case "info":
                    Utilities.PrintToConsole($"Players info:");
                    Utilities.PrintToConsole($"ID  ClanTag  UserName");
                    foreach (Player p in ServerData.Players)
                    {
                        Utilities.PrintToConsole($"{p.Entity.EntRef} {p.Entity.ClanTag} {p.Entity.Name}");
                    }
                    break;
            }
        }
    }

    public static class ServerData
    {
        public static List<Player> Players { get; set; } = new List<Player>();
    }
}
