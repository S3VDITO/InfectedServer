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
            /*
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
            */
            PlayerConnected += new Action<Entity>(entity =>
            {
                Player player = new Player(entity, new PlayerLoadout());
                ServerData.Players.Add(player);
            });

            PlayerDisconnected += new Action<Entity>(entity =>
            {
                ServerData.Players.Remove(ServerData.Players.Where(p => p.GUID == entity.GUID).First());
            });
        }

        public override void OnSay(Entity player, string name, string message)
        {
            switch (message)
            {
                case "info":
                    Utilities.PrintToConsole($"Players info:");
                    Utilities.PrintToConsole($"ID  ClanTag  UserName GUID");
                    foreach (Player p in ServerData.Players)
                    {
                        Utilities.PrintToConsole($"{p.ID} {p.Entity.ClanTag} {p.Name} {p.GUID}");
                    }
                    break;
            }
        }
    }

    public static class ServerData
    {
        internal static List<Player> Players { get; set; } = new List<Player>();
    }
}
