using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;
using static InfectedServer.DebugClass;

namespace InfectedServer.KILLSTREAKS
{
    public class Juggernaut : BaseScript
    {
        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            try
            {
                if (player.IsPlayer && attacker.IsPlayer && player.HasField("juggernaut_use") && !player.HasField("juggernaut_dead"))
                {
                    if (player.GetField<int>("juggernaut_armor") >= 0)
                    {

                        player.Health += damage;
                        AfterDelay(100, () => player.Health = player.GetField<int>("MaxHealth"));

                        if (attacker.GetField<string>("SessionTeam") == "axis")
                            player.SetField("juggernaut_armor", player.GetField<int>("juggernaut_armor") - damage / 3);
                        else
                        {
                            player.SetField("juggernaut_armor", attacker == player ?
                                player.GetField<int>("juggernaut_armor") - damage / (weapon == "bomb_site_mp" ? 1 : 10) :
                                player.GetField<int>("juggernaut_armor"));
                        }

                        if (attacker != player && attacker.GetField<string>("SessionTeam") == "axis")
                            Notify("damage_feedback", attacker, "damage_feedback_juggernaut");
                    }
                }
            }
            catch (Exception ex)
            {
                SendConsole("[FUCKED EXCEPTION] [JuggClass] Info: " + ex.ToString());
            }

            base.OnPlayerDamage(player, inflictor, attacker, damage, dFlags, mod, weapon, point, dir, hitLoc);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (player.IsPlayer && attacker.IsPlayer && player.HasField("juggernaut_use") && !player.HasField("juggernaut_dead"))
            {
                player.SetField("juggernaut_dead", true);
                player.SetField("juggernaut_armor", -999);
                player.GetField<HudElem>("juggernaut").Call("destroy");
            }
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }
    }
}
