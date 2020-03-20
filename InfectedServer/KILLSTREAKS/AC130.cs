using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InfinityScript;
using static InfectedServer.LevelClass.INFO;
using static InfectedServer.SoundClass;

namespace InfectedServer.KILLSTREAKS
{
	public class AC130 : BaseScript
	{
		private static bool AC130_IS_USE = true;

		private static int Duration = 45;

		public Dictionary<string, int> WeaponReloadTime = new Dictionary<string, int>()
		{
			//in milliseconds
			["ac130_25mm_mp"] = 1500,
			["ac130_40mm_mp"] = 3000,
			["ac130_105mm_mp"] = 5000
		};

		public AC130()
		{
			OnNotify("ac130", (owner, pos) => StartAC130(owner.As<Entity>()));
		}

		public void StartAC130(Entity player)
		{
			player.TeamPlayerCardSplash("used_ac130", "ac130");

			player.GiveWeapon("ac130_105mm_mp");
			player.GiveWeapon("ac130_40mm_mp");
			player.GiveWeapon("ac130_25mm_mp");
			player.SwitchToWeapon("ac130_105mm_mp");

			player.Call("SetPlayerData", "ac130Ammo105mm", player.Call<int>("GetWeaponAmmoClip", "ac130_105mm_mp"));
			player.Call("SetPlayerData", "ac130Ammo40mm", player.Call<int>("GetWeaponAmmoClip", "ac130_40mm_mp"));
			player.Call("SetPlayerData", "ac130Ammo25mm", player.Call<int>("GetWeaponAmmoClip", "ac130_25mm_mp"));

			player.Call("ThermalVisionFOFOverlayOn");

			AttachPlayer(player);
			WeaponFiredThread(player);
			ShotFired(player);

			Dictionary<string, HudElem> overlay = Overlay(player);
			HudElem timer = Timer(player, Duration);

			AC130UseWaiter(player, overlay, timer);

			ThermalVision(player, overlay);
		}

		public void ThermalVision(Entity player, Dictionary<string, HudElem> overlay)
		{
			bool inverted = false;

			player.Call("VisionSetThermalForPlayer", "ac130_enhanced_mp", 1);
			overlay["thermal_vision"].Alpha = 0.25f;
			overlay["enhanced_vision"].Alpha = 1;

			player.Call("notifyOnPlayerCommand", "switch thermal", "+usereload");
			player.Call("notifyOnPlayerCommand", "switch thermal", "+activate");

			player.OnNotify("switch thermal", ent =>
			{
				if (!player.CurrentWeapon.Contains("ac130"))
					return;

				if (!inverted)
				{
					player.Call("ThermalVisionOn");
					overlay["thermal_vision"].Alpha = 1;
					overlay["enhanced_vision"].Alpha = 0.25f;
				}
				else
				{
					player.Call("ThermalVisionOff");
					overlay["thermal_vision"].Alpha = 0.25f;
					overlay["enhanced_vision"].Alpha = 1;
				}

				inverted = !inverted;
			});
		}

		public void AC130UseWaiter(Entity player, Dictionary<string, HudElem> overlay, HudElem timer)
		{
			float dur = Duration;
			OnInterval(500, () =>
			{
				dur -= 0.5f;

				if (!player.IsAlive)
				{
					AC130End(player, overlay, timer);
					return false;
				}

				if(dur == 0)
				{
					AC130End(player, overlay, timer);
					return false;
				}

				return player.IsPlayer;
			});
		}

		public void AC130End(Entity player, Dictionary<string, HudElem> overlay, HudElem timer)
		{
			foreach (var over in overlay.Values)
				over.Call("Destroy");

			timer.Call("Destroy");

			player.Call("UnLink");
			player.Call("ThermalVisionOff");
			player.Call("ThermalVisionFOFOverlayOff");
			player.Call("setBlurForPlayer", 0, 0);

			player.TakeWeapon("ac130_105mm_mp");
			player.TakeWeapon("ac130_40mm_mp");
			player.TakeWeapon("ac130_25mm_mp");
			player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
		}

		public void AttachPlayer(Entity player)
		{
			if (!AC130_IS_USE)
				return;

			if (!player.IsPlayer)
				return;

			if (!player.IsAlive)
				return;

			player.Call("PlayerLinkWeaponviewToDelta", AC130_LEVEL, "tag_player", 1.0f, 35, 35, 35, 35);
			player.Call("SetPlayerAngles", AC130_LEVEL.Call<Vector3>("GetTagAngles", "tag_player"));
		}

		public void WeaponFiredThread(Entity player)
		{
			OnNotify("weapon_fired", (weaponName) =>
			{
				if (!AC130_IS_USE)
					return;

				if (!player.IsPlayer)
					return;

				if (!player.IsAlive)
					return;

				if (player.CurrentWeapon.Contains("ac130"))
				{
					string weapon = player.CurrentWeapon;

					switch (weapon)
					{
						case "ac130_105mm_mp":
							Function.Call("earthquake", .2f, 1, AC130_MODEL_LEVEL.Origin, 1000);
							player.Call("SetPlayerData", "ac130Ammo105mm", player.Call<int>("GetWeaponAmmoClip", weapon));
							break;
						case "ac130_40mm_mp":
							Function.Call("earthquake", .1f, .5f, AC130_MODEL_LEVEL.Origin, 1000);
							player.Call("SetPlayerData", "ac130Ammo40mm", player.Call<int>("GetWeaponAmmoClip", weapon));
							break;
						case "ac130_25mm_mp":
							player.Call("SetPlayerData", "ac130Ammo25mm", player.Call<int>("GetWeaponAmmoClip", weapon));
							break;
					}

					WeaponReload(player, weapon);
				}
				else
					return;
			});
		}

		public void ShotFired(Entity player)
		{
			OnNotify("projectile_impact", (weaponName, position, radius) =>
			{
				if (!AC130_IS_USE)
					return;

				if (!player.IsPlayer)
					return;

				if (!player.IsAlive)
					return;

				string weapName = weaponName.As<string>();

				if (weapName.Contains("105"))
					Function.Call("earthquake", 0.5f, 1, position, 3500);

				if (weapName.Contains("40"))
					Function.Call("earthquake", 0.2f, .5f, position, 2000);
			});
		}

		public void WeaponReload(Entity player, string weapon)
		{
			AfterDelay(WeaponReloadTime[weapon], () =>
			{
				if (!AC130_IS_USE)
					return;

				if (!player.IsPlayer)
					return;

				if (!player.IsAlive)
					return;

				player.Call("SetWeaponAmmoClip", weapon, 99999);

				switch (weapon)
				{
					case "ac130_105mm_mp":
						player.Call("SetPlayerData", "ac130Ammo105mm", player.Call<int>("GetWeaponAmmoClip", weapon));
						break;
					case "ac130_40mm_mp":
						player.Call("SetPlayerData", "ac130Ammo40mm", player.Call<int>("GetWeaponAmmoClip", weapon));
						break;
					case "ac130_25mm_mp":
						player.Call("SetPlayerData", "ac130Ammo25mm", player.Call<int>("GetWeaponAmmoClip", weapon));
						break;
				}
			});
		}

		public Dictionary<string, HudElem> Overlay(Entity player)
		{
			Dictionary<string, HudElem> overlay = new Dictionary<string, HudElem>()
			{
				["thermal_vision"] = HudElem.NewClientHudElem(player),
				["enhanced_vision"] = HudElem.NewClientHudElem(player)
			};

			overlay["thermal_vision"].X = 200;
			overlay["thermal_vision"].Y = 0;
			overlay["thermal_vision"].AlignX = "left";
			overlay["thermal_vision"].AlignY = "top";
			overlay["thermal_vision"].HorzAlign = "left";
			overlay["thermal_vision"].VertAlign = "top";
			overlay["thermal_vision"].FontScale = 2.5f;
			overlay["thermal_vision"].SetText("FLIR");
			overlay["thermal_vision"].Alpha = 1;

			overlay["enhanced_vision"].X = -200;
			overlay["enhanced_vision"].Y = 0;
			overlay["enhanced_vision"].AlignX = "right";
			overlay["enhanced_vision"].AlignY = "top";
			overlay["enhanced_vision"].HorzAlign = "right";
			overlay["enhanced_vision"].VertAlign = "top";
			overlay["enhanced_vision"].FontScale = 2.5f;
			overlay["enhanced_vision"].SetText("OPT");
			overlay["enhanced_vision"].Alpha = 1;

			player.Call("setBlurForPlayer", 1.2f, 0);

			return overlay;
		}

		public HudElem Timer(Entity player, int duration)
		{
			HudElem timer = HudElem.NewClientHudElem(player);
			timer.X = -100;
			timer.Y = 0;
			timer.AlignX = "right";
			timer.AlignY = "bottom";
			timer.HorzAlign = "right_adjustable";
			timer.VertAlign = "bottom_adjustable";
			timer.FontScale = 2.5f;
			timer.Call("SetTimer", duration);
			timer.Alpha = 1;
			return timer;
		}
	}
}

