using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer
{
    public static class LevelClass
    {
        /// <summary>
        /// Initializate main mapping for Function.Call thx Slvr99 for IS 1.5
        /// </summary>
        public static void InitializateMapping()
        {
            Function.AddMapping("SetHoverParams", 33317);
            Function.AddMapping("SetTurningAbility", 33377);
            Function.AddMapping("SetYawSpeed", 33377);
            Function.AddMapping("SetSpeed", 33363);
            Function.AddMapping("SetMaxPitchRoll", 33379);
            Function.AddMapping("SetVehGoalPos", 33325);
            Function.AddMapping("SetSpeedImmediate", 33364);

            Function.AddMapping("SetTurretTargetEnt", 33332);
            Function.AddMapping("FireWeapon", 33338);
        }

        public static void InitializateLevel()
        {
            InitializateMapping();
            for (int id = 0; id < 2048; id++)
            {
                if (Entity.GetEntity(id).GetField<string>("targetname") == "heli_start")
                    HELI_START_NODES.Add(Entity.GetEntity(id));

                if (Entity.GetEntity(id).GetField<string>("targetname") == "heli_loop_start")
                    HELI_LOOP_NODES.Add(Entity.GetEntity(id));

                if (Entity.GetEntity(id).GetField<string>("targetname") == "minimap_corner")
                    MinimapOrigins.Add(Entity.GetEntity(id));

                if (Entity.GetEntity(id).GetField<string>("targetname") == "ac130rig_script_model")
                    AC130_LEVEL = Entity.GetEntity(id);

                if (Entity.GetEntity(id).GetField<string>("targetname") == "vehicle_ac130_coop")
                    AC130_MODEL_LEVEL = Entity.GetEntity(id);

                if (Entity.GetEntity(id).GetField<string>("targetname") == "uavrig_script_model")
                    UAV = Entity.GetEntity(id);
            }

            AC130_MODEL_LEVEL.Call("SHOW");
            
        }


        public static class INFO
        {
            public static Entity UAV;

            public static Entity AC130_LEVEL;
            public static Entity AC130_MODEL_LEVEL;

            public static int MOAB_INDEX = GetKillstreakIndex("nuke");

            public static List<Entity> VEHICLES = new List<Entity>();
            public static List<Entity> TURRETS = new List<Entity>();

            public static List<Entity> CRATES = new List<Entity>();

            public static List<Entity> HELI_START_NODES = new List<Entity>();
            public static List<Entity> HELI_LOOP_NODES = new List<Entity>();

            public static int MAX_CRATE_COUNT_ON_LEVEL = 8;

            public static int MAX_VEHICLE_COUNT_ON_LEVEL = 8;
            public static int MAX_TURRET_COUNT_ON_LEVEL = 32;

            public static float AIR_HEIGHT = Function.Call<Entity>("GetEnt", "airstrikeheight", "targetname").Origin.Z;
            public static float GET_Z_ORIGIN()
            {
                if (VEHICLES.Count == 0)
                    return AIR_HEIGHT;

                return VEHICLES.Last().Origin.Z + 256;
            }

            public static List<Entity> MinimapOrigins = new List<Entity>();

            public static string GetHintText(string streakName)
            {
                switch (streakName)
                {
                    case "airdrop_assault":
                        return "Press and hold ^3[{+activate}] ^7for ^5Marksman";
                    case "airdrop_juggernaut":
                        return "Press and hold ^3[{+activate}] ^7for ^5Juggernaut";
                    case "helicopter":
                        return "Press and hold ^3[{+activate}] ^7for ^5Scavenger";
                    case "deployable_vest":
                        return "Press and hold ^3[{+activate}] ^7for ^5Blast Shield";
                    default:
                        return "^1STREAK NOT FOUND!";
                }
            }

            public static Vector3 FindBoxCenter(Vector3 mins, Vector3 maxs)
            {
                Vector3 center = new Vector3(0, 0, 0);
                center = maxs - mins;
                center = new Vector3(center.X / 2, center.Y / 2, center.Z / 2) + mins;
                return center;
            }


            public static string GetCrateIcon(string streakName)
            {
                switch (streakName)
                {
                    case "airdrop_assault":
                        return "specialty_carepackage_crate";
                    case "helicopter":
                        return "waypoint_ammo_friendly";
                    default:
                        return GetKillstreakCrateIcon(streakName);
                }
            }
            public static string GetKillstreakWeapon(string streakName)
            { return Function.Call<string>("TableLookup", "mp/killstreakTable.csv", 1, streakName, 12); }

            public static string GetKillstreakCrateIcon(string streakName)
            { return Function.Call<string>("TableLookup", "mp/killstreakTable.csv", 1, streakName, 15); }

            public static string GetKillstreakEarnSound(string streakName)
            { return Function.Call<string>("TableLookup", "mp/killstreakTable.csv", 1, streakName, 8); }


            public static string GetKillstreakFriendlySound(string streakName)
            { return System.Text.RegularExpressions.Regex.Replace(GetKillstreakEarnSound(streakName), "achieve", "use"); }

            public static string GetKillstreakSound(string streakName)
            { return Function.Call<string>("TableLookup", "mp/killstreakTable.csv", 1, streakName, 7); }

            public static int GetKillstreakIndex(String streakName)
            { return Function.Call<int>("TableLookupRowNum", "mp/killstreakTable.csv", 1, streakName) - 1; }

            public static string GetKillstreakEnemySound(String streakName)
            { return System.Text.RegularExpressions.Regex.Replace(GetKillstreakEarnSound(streakName), "achieve", "enemy"); }

            public static string GetKillstreakDpadIcon(String streakName)
            { return Function.Call<string>("TableLookup", "mp/killstreakTable.csv", 1, streakName, 16); }

            public static string GetTeamVoicePrefix(String teamRef)
            { return Function.Call<string>("TableLookup", "mp/factionTable.csv", 0, teamRef, 7); }

            public static void SetUsingRemote(Entity self)
            { self.Notify("using_remote"); }

            public static void StopUsingRemote(Entity self)
            { self.Notify("stopped_using_remote"); }
        }
    }
}
