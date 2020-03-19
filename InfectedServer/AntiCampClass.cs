using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.SoundClass;

using InfinityScript;

namespace InfectedServer
{
    public class AntiCampClass : BaseScript
    {
        private int AnticampExplosive = Function.Call<int>("loadfx", "explosions/tanker_explosion");

        public AntiCampClass()
        {
            // Player Status Cheker
            OnInterval(1000, () =>
            {
                foreach (Entity player in Players.Where(p => p.GetField<string>("SessionTeam") == "allies"))
                {
                    if (player.HasField("anticamp_status"))
                        continue;

                    if (player.Call<int>("GetPlayerData", "killstreaksState", "icons", 0) == MOAB_INDEX &&
                    player.Call<int>("GetPlayerData", "killstreaksState", "hasStreak", 0) == 1)
                    {
                        player.Call("iPrintLnBold", "^1ANTICAMP^7: ^2USE MOAB ^5AND ^1RUN");
                        player.SetField("anticamp_status", true);
                        AfterDelay(7500, () => StartAntiCamp(player));
                    }
                    else if (player.HasField("juggernaut_use"))
                    {
                        player.Call("iPrintLnBold", "^1ANTICAMP^7: ^2JUGGERNAUT ^5MUST ^1RUN");
                        player.SetField("anticamp_status", true);
                        AfterDelay(7500, () => StartAntiCamp(player));
                    }
                }
                return true;
            });

        }

        private void StartAntiCamp(Entity player)
        {
            Vector3 oldPos = player.Origin;

            player.OnInterval(7500, p =>
            {
                Vector3 newPos = player.Origin;

                if (oldPos.DistanceTo(player.Origin) < 720)
                {
                 player.Call("iPrintLnBold", "^1ANTICAMP^7: ^2RUN OR DIE!");

                    PlayLeaderDialog(player, "pushforward");


                    if(player.HasField("juggernaut_use"))
                        Function.Call("RadiusDamage", player.Origin, 10, 150, 150, player, "MOD_EXPLOSIVE", "bomb_site_mp");

                    if (!player.HasField("juggernaut_use"))
                        Function.Call("RadiusDamage", player.Origin, 10, 20, 20, player, "MOD_EXPLOSIVE", "bomb_site_mp");


                    return player.IsAlive && player.GetField<string>("SessionTeam") == "allies";
                }

                AfterDelay(250, () => oldPos = player.Origin);

                return player.IsAlive && player.GetField<string>("SessionTeam") == "allies";
            });
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (weapon == "bomb_site_mp")
            {
                double rot = new Random().Next(0, 360);
                Entity explosionEffect = Function.Call<Entity>("spawnFx", AnticampExplosive, player.Origin + new Vector3(0, 0, 50), new Vector3(0, 0, 1), new Vector3((float)Math.Cos(rot), (float)Math.Sin(rot), 0));
                Function.Call("triggerFx", explosionEffect);

                Function.Call("playSoundAtPos", player.Origin, "exp_suitcase_bomb_main");
            }
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }
    }
}
