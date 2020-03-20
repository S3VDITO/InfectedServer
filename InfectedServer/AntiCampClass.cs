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
        private List<Entity> AnticamperList = new List<Entity>();

        private int AnticampExplosive = Function.Call<int>("loadfx", "explosions/tanker_explosion");

        public AntiCampClass()
        {
            OnNotify("anticamp_start", (player) => StartAntiCamp(player.As<Entity>()));
        }

        private void StartAntiCamp(Entity player)
        {
            if (AnticamperList.Contains(player))
                return;

            AnticamperList.Add(player);

            Vector3 oldPos = player.Origin;

            player.Call("iPrintLnBold", "^1Ruuuuuuuuuuuuuun!");
            PlayLeaderDialog(player, "pushforward");

            player.OnInterval(7500, p =>
            {
                Vector3 newPos = player.Origin;

                if (player.CurrentWeapon.Contains("ac130"))
                    return true;

                if (oldPos.DistanceTo(player.Origin) < 512)
                {
                 player.Call("iPrintLnBold", "^1Run or die!");

                    PlayLeaderDialog(player, "pushforward");


                    if(player.Health > 200)
                        Function.Call("RadiusDamage", player.Origin, 10, 100, 100, player, "MOD_EXPLOSIVE", "bomb_site_mp");

                    if (player.Health <= 100)
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
        }
    }
}
