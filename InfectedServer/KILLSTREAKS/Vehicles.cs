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
            OnNotify("cobra", (owner, pos) => CallCobra(owner.As<Entity>(), pos.As<Vector3>()));
            OnNotify("harrier", (owner, pos) => CallHarrier(owner.As<Entity>(), pos.As<Vector3>()));
           // OnNotify("jugg", (owner, pos) => C130_START(owner.As<Entity>(), pos.As<Vector3>()));
            OnNotify("jugg", (owner, pos) => CallOsprey(owner.As<Entity>(), pos.As<Vector3>()));
        }

        public override EventEat OnSay2(Entity player, string name, string message)
        {
                player.GiveWeapon(GetKillstreakWeapon(message));
                player.SwitchToWeaponImmediate(GetKillstreakWeapon(message));
            return base.OnSay2(player, name, message);
        }

        #region COBRA
        public Entity COBRA_SETUP(Entity self, Vector3 startPos, Vector3 forward)
        {
            Entity cobra = Function.Call<Entity>("SpawnHelicopter", self, startPos, forward, "cobra_mp", "vehicle_mi24p_hind_mp");
            cobra.Call("SetMaxPitchRoll", 45, 85);
            cobra.Call("SetYawSpeed", 120, 80);
            cobra.Call("SetSpeed", 130, 125);
            return cobra;
        }

        public void CallCobra(Entity self, Vector3 goalPos)
        {
            Vector3 startPos = HELI_START_NODES[new Random().Next(0, HELI_START_NODES.Count)].Origin;
            float yaw = Function.Call<float>("RandomFloat", 360);
            Vector3 endPos = new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT);

            Entity cobra = COBRA_SETUP(self, GetPathStart(goalPos, yaw), new Vector3(0, yaw, 0));
            cobra.Call("SetVehGoalPos", endPos, true);

            AfterDelay(7500, () =>
            {
                AfterDelay(500, () =>
                {
                    Notify("run_crate", self, cobra.Origin, new Vector3(0, 0, 1), "com_deploy_ballistic_vest_friend_world", "helicopter", false);
                    cobra.Call("SetSpeed", 130, 125);
                    cobra.Call("SetVehGoalPos", HELI_START_NODES[new Random().Next(0, HELI_START_NODES.Count)].Origin, true);
                    AfterDelay(7500, () => cobra.Call("Delete"));
                });
            });
        }
        #endregion

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
                Notify("run_crate", owner, goal + new Vector3(0, 0, AIR_HEIGHT), new Vector3(0,0, 1), "com_plasticcase_friendly", "airdrop_juggernaut", true);
            });
            AfterDelay(15000, () => { c130.Call("delete"); });
        }
        #endregion

        #region Harrier
        public Entity HARRIER_SETUP(Entity self, Vector3 startPos, Vector3 forward)
        {
            Entity harrier = Function.Call<Entity>( "SpawnHelicopter", self, startPos, forward, "harrier_mp", "vehicle_av8b_harrier_jet_opfor_mp");
            harrier.Call("SetHoverParams", 50, 100, 50);
            harrier.Call("SetTurningAbility", .05f);
            harrier.Call("SetYawSpeed", 250, 250, 250, .5f);
            harrier.Call("SetSpeed", 250, 175);
            harrier.Call("SetMaxPitchRoll", 0, 0);
            return harrier;
        }

        public void CallHarrier(Entity self, Vector3 goalPos)
        {

            Single yaw = Function.Call<float>("RandomFloat", 360);
            Entity harrier = HARRIER_SETUP(self, GetPathStart(goalPos, yaw), new Vector3(0, yaw, 0));
            harrier.Call("SetVehGoalPos", new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT), true);

            AfterDelay(6500, () =>
            {
                Notify("run_crate", self, harrier.Origin, new Vector3(0, 0, 1), "com_plasticcase_friendly", "airdrop_assault", true);
                harrier.Call("SetMaxPitchRoll", 20, 30);
                AfterDelay(1000, () =>
                {

                    harrier.Call("SetSpeed", 150, 125);
                    harrier.Call("SetVehGoalPos", GetPathEnd(goalPos, yaw), true);
                    OnInterval(1000, () =>
                    {
                        if (harrier.HasField("death"))
                        {
                            Function.Call("StopFXOnTag", Function.Call<int>("LoadFX", "fire/jet_afterburner"), harrier, "tag_engine_right");
                            Function.Call("StopFXOnTag", Function.Call<int>("LoadFX", "fire/jet_afterburner"), harrier, "tag_engine_left");
                            Function.Call("StopFXOnTag", Function.Call<int>("LoadFX", "smoke/jet_contrail"), harrier, "tag_left_wingtip");
                            Function.Call("StopFXOnTag", Function.Call<int>("LoadFX", "smoke/jet_contrail"), harrier, "tag_right_wingtip");
                            return false;
                        }
                        else
                        {
                            Function.Call("PlayFXOnTag", Function.Call<int>("LoadFX", "fire/jet_afterburner"), harrier, "tag_engine_right");
                            Function.Call("PlayFXOnTag", Function.Call<int>("LoadFX", "fire/jet_afterburner"), harrier, "tag_engine_left");
                            Function.Call("PlayFXOnTag", Function.Call<int>("LoadFX", "smoke/jet_contrail"), harrier, "tag_left_wingtip");
                            Function.Call("PlayFXOnTag", Function.Call<int>("LoadFX", "smoke/jet_contrail"), harrier, "tag_right_wingtip");
                            return true;
                        }
                    });
                    AfterDelay(5500, () => { harrier.SetField("death", true); harrier.Call("Delete"); });
                });
            });
        }

        #endregion

        #region GuardDrop
        private static Entity Osprey(Entity self, Vector3 startPath, Vector3 forward)
        {
            Entity osprey = Function.Call<Entity>("SpawnHelicopter", self, startPath, forward, "osprey_player_mp", "vehicle_v22_osprey_body_mp");
            osprey.Call("SetSpeed", 70, 60, 35);
            osprey.Call("SetMaxPitchRoll", 50, 60);

            return osprey;
        }

        public void CallOsprey(Entity self, Vector3 goalPos)
        {
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
                    player.Origin.DistanceTo(pos) < 512)
                    {
                            osprey.Call("SetTurretTargetEnt", player);
                            osprey.Call("FireWeapon", "tag_flash", player);
                    }
                }

                return !osprey.HasField("bb_all");
            });
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

        private static Dictionary<String, Int32> Animation = new Dictionary<String, Int32>()
        {
            {"blades_anim_up", Function.Call<int>("LoadFX", "props/osprey_blades_anim_up")},
            {"blades_anim_down", Function.Call<int>("LoadFX","props/osprey_blades_anim_down")},
            {"blades_static_up", Function.Call<int>("LoadFX","props/osprey_blades_up")},
            {"blades_static_down", Function.Call<int>("LoadFX","props/osprey_blades_default")},
        };
        #endregion

        public Vector3 GetPathStart(Vector3 coord, float yaw )
        {
            int pathRandomness = 100;
            int lbHalfDistance = 15000;

            Vector3 direction = new Vector3(0, yaw, 0);

            Vector3 startPoint = coord + (Function.Call<Vector3>("AnglesToForward", direction) * (-1 * lbHalfDistance));
            startPoint += new Vector3((Function.Call <float>("randomfloat", 2) - 1) * pathRandomness, (Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, AIR_HEIGHT);

            return startPoint;
        }

        public Vector3 GetPathEnd(Vector3 coord, float yaw)
        {
            int pathRandomness = 100;
            int lbHalfDistance = 15000;

            Vector3 direction = new Vector3(0, yaw, 0);

            Vector3 endPoint = coord + (Function.Call<Vector3>("AnglesToForward", direction) * (lbHalfDistance));
            endPoint += new Vector3((Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, (Function.Call<float>("randomfloat", 2) - 1) * pathRandomness, AIR_HEIGHT);

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
