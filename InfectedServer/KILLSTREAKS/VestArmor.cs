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
		public static List<Entity> VestList = new List<Entity>();

		public VestArmor()
		{
			OnNotify("set_vest_armor", (player) => SetLightArmor(player.As<Entity>()));
			OnNotify("remove_vest_armor", (player) => RemoveLightArmor(player.As<Entity>()));
		}

		public void SetLightArmor(Entity player)
		{
			VestList.Add(player);
			GiveLightArmor(player);
		}

		public void GiveLightArmor(Entity player)
		{
			player.Notify("give_light_armor");

			int lightArmorHP = 200;

			player.SetField("combathigh_overlay", new Parameter(player.CreateTemplateOverlay("combathigh_overlay")));

			player.SetField("maxhealth", lightArmorHP);
			player.SetField("health", player.GetField<int>("maxhealth"));
		}

		public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
		{
			try
			{
				if (player.IsPlayer && attacker.IsPlayer && VestList.Contains(player))
				{
					if (attacker.GetField<string>("SessionTeam") == "axis")
						Notify("damage_feedback", attacker, "damage_feedback_lightarmor");
				}
			}
			catch (Exception ex)
			{
				//SendConsole("[FUCKED EXCEPTION] [VestClass] Info: " + ex.ToString());
			}
		}

		public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
		{
			if (VestList.Contains(player))
				RemoveLightArmor(player);
		}

		public void RemoveLightArmor(Entity player)
		{
			VestList.Remove(player);
			player.GetField<HudElem>("combathigh_overlay").Call("destroy");

			player.SetField("maxhealth", 100);
		}
	}
}
