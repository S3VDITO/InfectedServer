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
        public List<Entity> JuggernautList = new List<Entity>();

        public Juggernaut()
        {
            OnNotify("set_juggernaut_armor", (player) => SetJuggArmor(player.As<Entity>()));
            OnNotify("remove_juggernaut_armor", (player) => RemoveJuggernaut(player.As<Entity>()));
        }

        public void SetJuggArmor(Entity player)
        {
            player.SetField("juggernaut", new Parameter(player.CreateTemplateOverlay("goggles_overlay")));

            if(player.HasField("combathigh_overlay"))
                Notify("remove_vest_armor", player);

            player.SetField("maxhealth", 500);
            player.SetField("health", 2000);

            player.SetField("maxhealth", 500);
            player.SetField("health", 2000);

            AfterDelay(500, () =>
            {
                player.SetField("maxhealth", 500);
                player.SetField("health", 2000);
            });

            AfterDelay(2500, () =>
            {
                player.SetField("maxhealth", 500);
                player.SetField("health", 2000);
            });

            JuggernautList.Add(player);

            Notify("anticamp_start", player);
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            try
            {
                if (player.IsPlayer && attacker.IsPlayer  && JuggernautList.Contains(player))
                {
                    if (attacker.GetField<string>("SessionTeam") == "axis")
                        Notify("damage_feedback", attacker, "damage_feedback_juggernaut");

                    
                }
            }
            catch (Exception ex)
            {
                //SendConsole("[FUCKED EXCEPTION] [JuggClass] Info: " + ex.ToString());
            }

        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (JuggernautList.Contains(player))
                RemoveJuggernaut(player);

            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }

        public void RemoveJuggernaut(Entity player)
        {
            player.SetField("maxhealth", 100);
            player.SetField("health", 100);

            JuggernautList.Remove(player);
            player.GetField<HudElem>("juggernaut").Call("destroy");
        }
    }
}
