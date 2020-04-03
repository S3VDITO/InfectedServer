using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.KILLSTREAKS
{
    public class Vehicles : BaseScript
    {
        public Vehicles()
        {
            // C130 Jugg
            OnNotify("jugg", (owner, pos) => C130_START(owner.As<Entity>(), pos.As<Vector3>()));

            // Osprey Jugg
           // OnNotify("jugg", (owner, pos) => CallOsprey(owner.As<Entity>(), pos.As<Vector3>()));
        }

        #region C130
        public Entity C130_SETUP(Entity owner, Vector3 start)
        {
            Entity c130 = Function.Call<Entity>("spawnplane", owner, "script_model", start, "compass_objpoint_c130_friendly", "compass_objpoint_c130_enemy");
            c130.Call("SetModel", "vehicle_ac130_low_mp");
            return c130;
        }

        public void C130_START(Entity owner, Vector3 goal)
        {
            float flyTime = 15;

            float yaw = owner.GetField<Vector3>("angles").Y;

            Entity c130 = C130_SETUP(owner, GetPathStart(goal, yaw));
            c130.Call("playloopsound", "veh_ac130_dist_loop");

            c130.SetField("angles", new Vector3(0, yaw, 0));
            c130.Call("moveTo", GetPathEnd(goal, yaw), flyTime, 0, 0);

            float minDist = c130.Origin.DistanceTo2D(goal);

            float time = flyTime * 2;

            AfterDelay(7500, () =>
            {
                Notify("run_crate", owner, new Vector3(goal.X, goal.Y, AIR_HEIGHT), new Vector3(), "com_plasticcase_friendly", "airdrop_juggernaut", true);
            });
            AfterDelay(15000, () => { c130.Call("delete"); });
        }
        #endregion

        #region Osprey Drop
        private static Entity Osprey(Entity self, Vector3 startPath, Vector3 forward)
        {
            Entity osprey = Function.Call<Entity>("SpawnHelicopter", self, startPath, forward, "osprey_player_mp", "vehicle_v22_osprey_body_mp");
            osprey.Call("SetSpeed", 70, 60, 35);
            osprey.Call("SetMaxPitchRoll", 50, 60);

            return osprey;
        }

        public void CallOsprey(Entity self, Vector3 goalPos)
        {
            self.TeamPlayerCardSplash("", "airdrop_juggernaut");
            int yaw = new Random().Next(0, 360);
            Vector3 startPos = HELI_START_NODES[new Random().Next(0, HELI_START_NODES.Count)].Origin;
            Entity osprey = Osprey(self, GetPathStart(goalPos, yaw), new Vector3(1, Function.Call<Vector3>("VectorToAngles", goalPos - GetPathStart(goalPos, yaw)).Y, 0));

            osprey.Call("SetSpeedImmediate", 80, 60, 30);
            osprey.Call("SetYawSpeed", 30, 30, 30, .3f);
            osprey.Call("SetVehGoalPos", new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT), true);

            OspreyRun(self, osprey, goalPos);
        }

        private void OspreyRun(Entity self, Entity osprey, Vector3 goalPos)
        {
            AirShipPitchPropsDown(osprey);
            AfterDelay(11000, () =>
            {
                TurretAI(self, osprey, goalPos);
                AirShipPitchPropsUp(osprey);
                Notify("run_crate", self, new Vector3(goalPos.X, goalPos.Y, osprey.Origin.Z), new Vector3(0, 0, 0), "com_plasticcase_friendly", "airdrop_juggernaut", true);
                osprey.Call("SetVehGoalPos", new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT - 256), true);
                AfterDelay(7500, () =>
                {
                    osprey.Call("SetVehGoalPos", GetPathEnd(goalPos, osprey.GetField<Vector3>("angles").Y), true);
                    AirShipPitchPropsDown(osprey);
                    AfterDelay(12500, () => {
                        osprey.SetField("bb_all", true);
                        osprey.Call("Delete"); });
                });
            });
        }

        private void TurretAI(Entity owner, Entity osprey, Vector3 pos)
        {
            OnInterval(500, () =>
            {
                foreach (Entity player in Players)
                {
                    if (player.GetField<string>("SessionTeam") == "axis" &&
                    LockSightTest(player, osprey) && 
                    player.Origin.DistanceTo(pos) < 256)
                    {
                            osprey.Call("SetTurretTargetEnt", player);
                            osprey.Call("FireWeapon", "tag_flash", player);
                    }
                }

                return !osprey.HasField("bb_all");
            });
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (weapon == "osprey_player_minigun_mp")
                if (player.GetField<string>("SessionTeam") == "allies")
                {
                    damage = 0;
                    player.Health += damage;
                    AfterDelay(250, () => player.Health = 100);
                }
            base.OnPlayerDamage(player, inflictor, attacker, damage, dFlags, mod, weapon, point, dir, hitLoc);
        }

        public void AirShipPitchPropsUp(Entity osprey)
        {
            Function.Call("StopFXOnTag", Animation["blades_static_down"], osprey, "TAG_BLADES_ATTACH");
            Function.Call("PlayFXOnTag", Animation["blades_anim_up"], osprey, "TAG_BLADES_ATTACH");
            AfterDelay(1000, () => Function.Call("PlayFXOnTag", Animation["blades_static_up"], osprey, "TAG_BLADES_ATTACH"));
        }

        public void AirShipPitchPropsDown(Entity osprey)
        {
            Function.Call("StopFXOnTag", Animation["blades_static_up"], osprey, "TAG_BLADES_ATTACH");
            Function.Call("PlayFXOnTag", Animation["blades_anim_down"], osprey, "TAG_BLADES_ATTACH");
            AfterDelay(1000, () => Function.Call("PlayFXOnTag", Animation["blades_static_down"], osprey, "TAG_BLADES_ATTACH"));
        }

        private static Dictionary<string, int> Animation = new Dictionary<string, int>()
        {
            {"blades_anim_up", Function.Call<int>("LoadFX", "props/osprey_blades_anim_up")},
            {"blades_anim_down", Function.Call<int>("LoadFX","props/osprey_blades_anim_down")},
            {"blades_static_up", Function.Call<int>("LoadFX","props/osprey_blades_up")},
            {"blades_static_down", Function.Call<int>("LoadFX","props/osprey_blades_default")},
        };
        #endregion

        #region Osprey Gunner
        #endregion

        #region PaveLow
        #endregion

        public Vector3 GetPathStart(Vector3 coord, float yaw )
        {
            int pathRandomness = 100;
            int lbHalfDistance = 15000;

            Vector3 direction = new Vector3(0, yaw, 0);

            Vector3 startPoint = coord + (Function.Call<Vector3>("AnglesToForward", direction) * (-1 * lbHalfDistance));
            startPoint += new Vector3((Function.Call <float>("randomfloat", 2) - 1) * pathRandomness, (Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, AIR_HEIGHT);

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

        public static bool LockSightTest(Entity self, Entity target)
        {
            Vector3 eyePos = self.Call<Vector3>("GetEye");

            if (Function.Call<int>("SightTracePassed", eyePos, target.Origin, false, target) == 1)
                return true;

            if (Function.Call<int>("SightTracePassed", eyePos, target.Call<Vector3>("GetPointInBounds", new Vector3(1, 0, 0)), false, target) == 1)
                return true;

            if (Function.Call<int>("SightTracePassed", eyePos, target.Call<Vector3>("GetPointInBounds", new Vector3(-1, 0, 0)), false, target) == 1)
                return true;

            return false;
        }
    }
}
