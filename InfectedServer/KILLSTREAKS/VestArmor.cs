using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.DebugClass;
using static InfectedServer.HUDClass;

namespace InfectedServer.KILLSTREAKS
{
    public class VestArmor : BaseScript
    {
        public static Dictionary<Entity, int> VestList = new Dictionary<Entity, int>();

		public VestArmor()
		{
			OnNotify("set_vest_armor", (player) => SetLightArmor(player.As<Entity>()));
			OnNotify("remove_vest_armor", (player) => RemoveLightArmor(player.As<Entity>()));
		}

		public void SetLightArmor(Entity player)
		{
			VestList.Add(player, 200);
			GiveLightArmor(player);
		}

		public void GiveLightArmor(Entity player)
		{
			player.Notify("give_light_armor");

			int lightArmorHP = VestList[player];

			player.SetField("combathigh_overlay", new Parameter(player.CreateTemplateOverlay("combathigh_overlay")));

			player.SetField("maxhealth", lightArmorHP);
			player.SetField("health", player.GetField<int>("maxhealth"));
		}

		public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
		{
			try
			{
				if (player.IsPlayer && attacker.IsPlayer && VestList.ContainsKey(player))
				{
					if (attacker != player && attacker.GetField<string>("SessionTeam") == "axis")
						Notify("damage_feedback", attacker, "damage_feedback_lightarmor");
				}
			}
			catch (Exception ex)
			{
				SendConsole("[FUCKED EXCEPTION] [VestClass] Info: " + ex.ToString());
			}

			base.OnPlayerDamage(player, inflictor, attacker, damage, dFlags, mod, weapon, point, dir, hitLoc);
		}

		public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
		{
			if (VestList.ContainsKey(player))
				RemoveLightArmor(player);

			base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
		}

		public void RemoveLightArmor(Entity player)
		{
			VestList.Remove(player);
			player.GetField<HudElem>("combathigh_overlay").Call("destroy");
			player.SetField("maxhealth", 100);
			player.Notify("remove_light_armor");
		}
	}
}
