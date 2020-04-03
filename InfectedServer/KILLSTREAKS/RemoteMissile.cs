using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.SoundClass;

namespace InfectedServer.KILLSTREAKS
{
	public class RemoteMissile : BaseScript
	{
		public static Entity UAV_MODEL;
		private static int Duration = 15;

		public RemoteMissile()
		{
			Call("precacheShader", "ac130_overlay_105mm");

			OnNotify("spawn_uav", () => SpawnUAV());

			OnNotify("remote_mortar", (player) => RideUAV(player.As<Entity>()));
		}

		public void SpawnUAV()
		{
			UAV_MODEL = Function.Call<Entity>("spawn", "script_model", UAV.Call<Vector3>("getTagOrigin", "tag_origin"));
			UAV_MODEL.Call("setModel", "vehicle_predator_b");

			double zOffset = 6300;
			double angle = 0;
			double radiusOffset = 6100;
			double xOffset = (double)Math.Cos(angle) * radiusOffset;
			double yOffset = (double)Math.Sin(angle) * radiusOffset;
			Vector3 angleVector = Function.Call<Vector3>("vectorNormalize", new Vector3((float)xOffset, (float)yOffset, (float)zOffset));
			angleVector = (angleVector * 6100);

			UAV_MODEL.Call("linkTo", UAV, "tag_origin", angleVector, new Vector3(0, (float)angle - 90, 10));
		}

		public void RideUAV(Entity player)
		{
			player.TeamPlayerCardSplash("used_remote_mortar", "remote_mortar");

			player.Call("ThermalVisionFOFOverlayOn");
			player.Call("ThermalVisionOn");

			AttachPlayer(player);
			Overlay(player);
			ShotFiredDarkScreenOverlay(player);

			UAVShotingWaiter(player);
		}

		public void AttachPlayer(Entity player)
		{
			player.Call("PlayerLinkWeaponviewToDelta", UAV_MODEL, "tag_player", 1.0f, 40, 40, 25, 40);
			AfterDelay(250, () => player.Call("SetPlayerAngles", Function.Call<Vector3>("vectorToAngles", UAV.Origin - player.Call<Vector3>("GetEye"))));
		}

		public void Overlay(Entity player)
		{
			HudElem overlay = HudElem.NewClientHudElem(player);
			overlay.X = 0;
			overlay.Y = 0;
			overlay.AlignX = "left";
			overlay.AlignY = "top";
			overlay.HorzAlign = "fullscreen";
			overlay.VertAlign = "fullscreen";
			overlay.SetShader("ac130_overlay_105mm", 640, 480);
			overlay.Sort = -10;
			overlay.Archived = true;

			player.SetField("overlay_uav_ac130_105mm", new Parameter(overlay));
		}

		public void UAVShotingWaiter(Entity player)
		{
			player.Call("DisableOffhandWeapons");
			player.Call("DisableWeaponSwitch");

			OnInterval(100, () =>
			{
				if (player.Call<int>("attackbuttonpressed") == 1)
				{
					ShotFiredDarkScreenOverlay(player);

					Entity missile = Call<Entity>("magicBullet", 
						"remotemissile_projectile_mp",
						UAV_MODEL.Origin - new Vector3(0, 0, 25), 
						Call<Vector3>("anglestoforward", player.Call<Vector3>("getplayerangles")) * 15000,
						player);

					MissileEyes(player, missile);

					player.GetField<HudElem>("overlay_uav_ac130_105mm").Call("destroy");
					return false;
				}
				return player.IsAlive && player.IsPlayer;
			});
		}

		public void MissileEyes(Entity player, Entity rocket)
		{
			player.Call("unlink");

			player.Call("VisionSetMissileCamForPlayer", "black_bw", 0);

			if (!player.IsPlayer)
				return;

			player.Call("CameraLinkTo", rocket, "tag_origin");
			player.Call("ControlsLinkTo", rocket);

			rocket.OnNotify("death", ent =>
			{
				if (rocket.HasField("delete"))
					return;

				rocket.SetField("delete", 1);
				player.Call("ControlsUnlink");
				player.Call("FreezeControls", true);
				StaticEffect(player, 500);

				AfterDelay(500, () =>
				{
					player.Call("ThermalVisionFOFOverlayOff");
					player.Call("ThermalVisionOff");
					player.Call("CameraUnlink");
					StopUsingRemote(player);

					player.Call("EnableOffhandWeapons");
					player.Call("EnableWeaponSwitch");
					player.Call("EnableWeapons");

					player.TakeWeapon(GetKillstreakWeapon("predator_missile"));
					player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
				});

			});

		}

		private void StaticEffect(Entity player, int duration)
		{
			if (!player.IsPlayer) 
				return;

			HudElem staticBG = HudElem.NewClientHudElem(player);
			staticBG.HorzAlign = "Fullscreen";
			staticBG.VertAlign = "Fullscreen";
			staticBG.SetShader("white", 640, 480);
			staticBG.Archived = true;
			staticBG.Sort = 10;

			HudElem staticFG = HudElem.NewClientHudElem(player);
			staticFG.HorzAlign = "Fullscreen";
			staticFG.VertAlign = "Fullscreen";
			staticFG.SetShader("ac130_overlay_grain", 640, 480);
			staticFG.Archived = true;
			staticFG.Sort = 20;
			AfterDelay(duration, () =>
			{
				staticFG.Call("Destroy");
				staticBG.Call("Destroy");
			});

			return;
		}

		public void ShotFiredDarkScreenOverlay(Entity player)
		{
			HudElem darkScreenOverlay = HudElem.NewClientHudElem(player);
			darkScreenOverlay.X = 0;
			darkScreenOverlay.Y = 0;
			darkScreenOverlay.AlignX = "left";
			darkScreenOverlay.AlignY = "top";
			darkScreenOverlay.HorzAlign = "fullscreen";
			darkScreenOverlay.VertAlign = "fullscreen";
			darkScreenOverlay.SetShader("black", 640, 480);
			darkScreenOverlay.Sort = -10;
			darkScreenOverlay.Alpha = 1;

			AfterDelay(1000, () =>
			{
				darkScreenOverlay.Call("fadeOverTime", 1f);
				darkScreenOverlay.Alpha = 0;
				AfterDelay(500, () => darkScreenOverlay.Call("destroy"));
			});

		}
	}
}
