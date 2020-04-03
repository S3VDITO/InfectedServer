using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.KILLSTREAKS
{
    public class PaveLow : BaseScript
    {
        public PaveLow()
        {
            OnNotify("final_heli", (player) => StartPavelow(player.As<Entity>()));
        }

        public void StartPavelow(Entity player)
        {
            player.SetField("old_pos", player.Origin);

            player.GiveWeapon("heli_remote_mp");
            player.SwitchToWeapon("heli_remote_mp");

            Entity heli = HeliSetup(player, player.GetField<Vector3>("angles").Y);
            heli.Call("SetMaxPitchRoll", 35, 35);
            heli.Call("SetYawSpeed", 80, 80);
            heli.Call("SetSpeed", 100, 95);

            heli.SetField("owner", player);

            int MaxGoals = 1;
            Action goal = new Action(() => { });
            goal = new Action(() =>
            {
                if (MaxGoals == 0)
                    return;

                if (MaxGoals == 1)
                {
                    DamageHandler(heli);
                    HeliMove(player, heli);
                }

                MaxGoals--;
            });
            RemoteTurret(player, heli);
            SetVehGoalPos(goal, heli, new Vector3(player.Origin.X, player.Origin.Y, AIR_HEIGHT));

            player.ExplosiveBullets();
        }

        public void RemoteTurret(Entity player, Entity heli)
        {
            Entity mgTurret = Function.Call<Entity>("spawnTurret", "misc_turret", heli.Origin, "pavelow_minigun_mp");
            mgTurret.Call("linkTo", heli, "tag_origin", new Vector3(310, 0, -120), heli.GetField<Vector3>("angles"));
            mgTurret.Call("setModel", "weapon_minigun");
            mgTurret.Call("HideAllParts");
            player.Call("HideAllParts");
            mgTurret.Call("makeTurretOperable");

            heli.SetField("turret", mgTurret);

            player.Call("PlayerLinkWeaponViewToDelta", mgTurret, "tag_player", 0, 180, 180, 180, 80, false);
            player.Call("PlayerLinkedSetViewZNear", false);
            player.Call("PlayerLinkedSetUseBaseAngleForViewClamp", true);
            player.Call("RemoteControlTurret", mgTurret);

            player.Call("PlayerLinkTo", heli, "tag_origin", 0, 180, 180, 180, 80, false);
            player.Call("ThermalVisionFOFOverlayOn");
            ThermalVision(player);
        }

        public void ThermalVision(Entity player)
        {
            bool inverted = false;

            player.Call("notifyOnPlayerCommand", "switch thermal", "+usereload");
            player.Call("notifyOnPlayerCommand", "switch thermal", "+activate");

            player.OnNotify("switch thermal", ent =>
            {
                if (!player.CurrentWeapon.Contains("heli_remote_mp"))
                    return;

                if (!inverted)
                    player.Call("ThermalVisionOn");
                else
                    player.Call("ThermalVisionOff");

                inverted = !inverted;
            });
        }

        public void SetVehGoalPos(Action action_goal, Entity heli, Vector3 endPos, bool stopAtPos = true)
        {
            if (heli.HasField("delete"))
                return;

            heli.Call("SetVehGoalPos", endPos, stopAtPos);
            heli.OnInterval(500, heli_ =>
            {
                if (heli.HasField("delete"))
                    return false;

                if (heli.Origin.DistanceTo2D(endPos) <= 50)
                {
                    action_goal.Invoke();
                    return false;
                }
                return true;
            });
        }

        public void HeliMove(Entity player, Entity heli)
        {
            bool forward_move = false;
            bool back_move = false;

            bool right_move = false;
            bool left_move = false;

            player.Call("notifyOnPlayerCommand", "forward_press", "+forward");
            player.Call("notifyOnPlayerCommand", "forward_unpress", "-forward");

            player.Call("notifyOnPlayerCommand", "back_press", "+back");
            player.Call("notifyOnPlayerCommand", "back_unpress", "-back");

            player.Call("notifyOnPlayerCommand", "moveleft_press", "+moveleft");
            player.Call("notifyOnPlayerCommand", "moveleft_unpress", "-moveleft");

            player.Call("notifyOnPlayerCommand", "moveright_press", "+moveright");
            player.Call("notifyOnPlayerCommand", "moveright_unpress", "-moveright");

            player.OnNotify("forward_press", ent => forward_move = true);
            player.OnNotify("forward_unpress", ent => forward_move = false);

            player.OnNotify("back_press", ent => back_move = true);
            player.OnNotify("back_unpress", ent => back_move = false);

            player.OnNotify("moveleft_press", ent => left_move = true);
            player.OnNotify("moveleft_unpress", ent => left_move = false);

            player.OnNotify("moveright_press", ent => right_move = true);
            player.OnNotify("moveright_unpress", ent => right_move = false);



            Vector3 lbAngles;
            Vector3 origin;
            Vector3 forward;

            OnInterval(100, () =>
            {

                if (!(forward_move || back_move || right_move || left_move))
                    return true;

                lbAngles = heli.GetField<Vector3>("Angles");
                origin = heli.Origin;
                forward = Function.Call<Vector3>("AnglesToForward", lbAngles);


                if (forward_move)
                    heli.Call("SetVehGoalPos", new Vector3((origin + forward * 256).X, (origin + forward * 256).Y, heli.Origin.Z), true);

                if (back_move)
                    heli.Call("SetVehGoalPos", new Vector3((origin + forward * -256).X, (origin + forward * -256).Y, heli.Origin.Z), true);

                if (left_move)
                    heli.Call("SetGoalYaw", heli.GetField<Vector3>("Angles").Y + 25);

                if (right_move)
                    heli.Call("SetGoalYaw", heli.GetField<Vector3>("Angles").Y - 25);

                return true;
            });
        }

        public void DamageHandler(Entity heli)
        {
            heli.Call("setCanDamage", true);
            heli.Health = 1000; // keep it from dying anywhere in code
            heli.SetField("maxHealth", 1000); // this is the health we'll check

            heli.OnNotify("damage", (ent, damage, attacker, direction_vec, point, sMeansOfDeath, modelName, tagName, partName, iDFlags, sWeapon) =>
            {
                if (heli.HasField("delete"))
                    return;

                string weapon = (string)sWeapon;

                Notify("damage_feedback", attacker, "damage_feedback");

                if (weapon.Contains("stinger"))
                    DestroyPavelow(heli, true);
            });
        }

        public void DestroyPavelow(Entity heli, bool isExplosive)
        {
            if (isExplosive)
            {
                double rot = new Random().Next(0, 360);
                Entity explosionEffect = Function.Call<Entity>("spawnFx", Function.Call<int>("loadfx", "explosions/tanker_explosion"), heli.Origin, new Vector3(0, 0, 1), new Vector3((float)Math.Cos(rot), (float)Math.Sin(rot), 0));
                Function.Call("triggerFx", explosionEffect);
                Function.Call("playSoundAtPos", heli.Origin, "exp_suitcase_bomb_main");
            }

            if (heli.GetField<Entity>("owner").IsPlayer)
            {
                Function.Call("RadiusDamage", heli.GetField<Entity>("owner").Origin, 10, 10000, 10000, heli.GetField<Entity>("owner"), "MOD_EXPLOSIVE", "stinger_mp");
                heli.GetField<Entity>("owner").Call("ThermalVisionFOFOverlayOff");
                heli.GetField<Entity>("owner").Call("ThermalVisionOff");
            }

            VEHICLES.Remove(heli);
            heli.SetField("delete", 1);
            heli.GetField<Entity>("turret").Call("Delete");
            heli.Call("Delete");
        }

        public Entity HeliSetup(Entity owner, float yaw)
        {
            Entity heli = Function.Call<Entity>("spawnHelicopter", owner, GetPathStart(owner.Origin, yaw), new Vector3(0, yaw, 0), "pavelow_mp", "vehicle_pavelow");
            VEHICLES.Add(heli);
            return heli;
        }

        public Vector3 GetPathStart(Vector3 coord, float yaw)
        {
            int pathRandomness = 100;
            int lbHalfDistance = 15000;

            Vector3 direction = new Vector3(0, yaw, 0);

            Vector3 startPoint = coord + (Function.Call<Vector3>("AnglesToForward", direction) * (-1 * lbHalfDistance));
            startPoint += new Vector3((Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, (Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, AIR_HEIGHT);

            startPoint.Z = AIR_HEIGHT;

            return startPoint;
        }

        public Vector3 GetPathEnd(Vector3 coord, float yaw)
        {
            int pathRandomness = 100;
            int lbHalfDistance = 15000;

            Vector3 direction = new Vector3(0, yaw, 0);

            Vector3 endPoint = coord + (Function.Call<Vector3>("AnglesToForward", direction) * (lbHalfDistance));
            endPoint += new Vector3((Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, (Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, AIR_HEIGHT);

            endPoint.Z = AIR_HEIGHT;

            return endPoint;
        }
    }
}
