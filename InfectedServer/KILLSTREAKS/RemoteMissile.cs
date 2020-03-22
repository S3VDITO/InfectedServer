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
		private static Entity UAV_MODEL;
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
			player.SetField("is_rider_missile_cam", 0);

			player.TeamPlayerCardSplash("used_remote_mortar", "remote_mortar");

			player.Call("ThermalVisionFOFOverlayOn");
			player.Call("ThermalVisionOn");

			HudElem timer = Timer(player, Duration);
			AttachPlayer(player);
			Overlay(player);
			ShotFiredDarkScreenOverlay(player);

			UAVShotingWaiter(player, timer);
			UAVUseWaiter(player, timer);
		}

		public void AttachPlayer(Entity player)
		{
			player.Call("PlayerLinkWeaponviewToDelta", UAV_MODEL, "tag_player", 1.0f, 40, 40, 25, 40);
			AfterDelay(250, () => player.Call("SetPlayerAngles", Function.Call<Vector3>("vectorToAngles", UAV.Origin - player.Call<Vector3>("GetEye"))));
		}

		public void UAVUseWaiter(Entity player, HudElem timer)
		{
			float dur = Duration;
			OnInterval(500, () =>
			{
				dur -= 0.5f;

				if (!player.IsAlive)
				{
					UAVEnd(player);
					timer.Call("Destroy");
					player.GetField<HudElem>("overlay_uav_ac130_105mm").Call("destroy");
					return false;
				}

				if (dur == 0)
				{
					UAVEnd(player);
					FireMissile(player);
					timer.Call("Destroy");
					player.GetField<HudElem>("overlay_uav_ac130_105mm").Call("destroy");
					return false;
				}

				if (player.GetField<int>("is_rider_missile_cam") == 1)
					return false;

				return player.IsAlive && player.IsPlayer && player.CurrentWeapon == GetKillstreakWeapon("predator_missile");
			});
		}

		public void UAVEnd(Entity player)
		{
			if (player.IsAlive && player.IsPlayer && player.CurrentWeapon == GetKillstreakWeapon("predator_missile"))
				return;

			player.Call("UnLink");

			player.Call("ThermalVisionOff");
			player.Call("ThermalVisionFOFOverlayOff");
			player.Call("setBlurForPlayer", 0, 0);

			player.Call("EnableOffhandWeapons");
			player.Call("EnableWeaponSwitch");
			player.Call("EnableWeapons");

			player.Call("CameraUnlink");

			player.TakeWeapon(GetKillstreakWeapon("predator_missile"));
			player.SwitchToWeapon(player.GetField<string>("lastDroppableWeapon"));
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

		public void UAVShotingWaiter(Entity player, HudElem timer)
		{
			OnInterval(100, () =>
			{
				if (player.Call<int>("attackbuttonpressed") == 1)
				{
					FireMissile(player);
					timer.Call("Destroy");
					player.GetField<HudElem>("overlay_uav_ac130_105mm").Call("destroy");
					return false;
				}
				return player.IsAlive && player.IsPlayer && player.CurrentWeapon == GetKillstreakWeapon("predator_missile");
			});
		}

		public void FireMissile(Entity player)
		{
			player.SetField("is_rider_missile_cam", 1);

			Vector3 asd = Call<Vector3>("anglestoforward", player.Call<Vector3>("getplayerangles"));
			Vector3 end = new Vector3(asd.X * 15000, asd.Y * 15000, asd.Z * 15000);

			ShotFiredDarkScreenOverlay(player);
			player.GetField<HudElem>("overlay_uav_ac130_105mm").Call("destroy");

			player.Call("Unlink");

			AfterDelay(250, () =>
			{
				Entity missile = Call<Entity>("magicBullet", "remotemissile_projectile_mp", UAV_MODEL.Origin - new Vector3(0, 0, 25), end, player);

				missile.Call("setCanDamage", true);

				player.Call("CameraLinkTo", missile, "tag_origin");
				player.Call("ControlsLinkTo", missile);

				missile.OnNotify("death", (rocket) =>
				{
					player.Call("ControlsUnlink");
					player.Call("freezeControls", true);

					ShotFiredDarkScreenOverlay(player);

					AfterDelay(500, () =>
					{
						player.SetField("is_rider_missile_cam", 0);
						UAVEnd(player);
					});
				});
			});
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

			AfterDelay(500, () =>
			{
				darkScreenOverlay.Call("fadeOverTime", 0.5f);
				darkScreenOverlay.Alpha = 0;
				AfterDelay(500, () => darkScreenOverlay.Call("destroy"));
			});

		}
	}
}
