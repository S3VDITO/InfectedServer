/* using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.MapInfo;
using static InfectedServer.DebugClass;

namespace InfectedServer
{
    public static class MapInfo
    {
        /// <summary>
        /// Jumping zones list
        /// </summary>
        public static List<Entity> Platforms = new List<Entity>();


        /// <summary>
        /// Jumping
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="platform">Jumping zone</param>
        public static void JumpFunction(this Entity player, Entity platform)
        {
            Vector3 velocity = platform.GetField<Vector3>("vel");

            player.Call("setVelocity", velocity);
        }
    }
    public class MapClass : BaseScript
    {
        /// <summary>
        /// Load class
        /// </summary>
        public MapClass()
        {
            string mapname = Function.Call<string>("getDvar", "mapname");

            OnInterval(1500, () =>
            {
                foreach (var player in Players)
                    foreach (var platform in Platforms)
                        if (player.Origin.DistanceTo2D(platform.Origin) <= 175 && player.Call<int>("UseButtonPressed") == 1)
                            player.JumpFunction(platform);

                return true;
            });

            AfterDelay(1000, () =>
            {
                switch (mapname)
                {
                    case "mp_terminal_cls":
                        Turret(new Vector3(1457.542f, 4395.517f, 304.125f), new Vector3(0, -143.1793f, 0));
                        RedPlatform(new Vector3(747.1371f, 3730.814f, 189.3806f), new Vector3(), new Vector3(50, 0, 500));
                        break;
                    case "mp_paris":
                        RedPlatform(new Vector3(-1036.623f, -739.0071f, 145.1301f), new Vector3(), new Vector3(0, 0, 1250));
                        RedPlatform(new Vector3(-1888.126f, 632.3943f, 289.125f), new Vector3(), new Vector3(-250, 0, 1150));
                        break;
                    case "mp_underground":
                        RedPlatform(new Vector3(-843.5818f, -189.2902f, 8.124998f), new Vector3(), new Vector3(-20, 100, 600));
                        break;
                    default:
                        break;
                }

            });


        }

        /// <summary>
        /// Create new jumping zone
        /// </summary>
        /// <param name="position">Origin zone</param>
        /// <param name="angles">Angles [BUT WHY?!]</param>
        /// <param name="velocity">Velocity jumping</param>
        /// <returns></returns>
        public Entity RedPlatform(Vector3 position, Vector3 angles, Vector3 velocity)
        {
            Entity box = Function.Call<Entity>("Spawn", "script_model", position);
            box.SetField("angles", angles);
            box.SetField("vel", velocity);

            box.Call("setModel", "weapon_c4_bombsquad");

            box.Call("setCursorHint", "HINT_NOICON");
            box.Call("setHintString", "Press and hold ^1[{+activate}] ^7for jump");
            box.Call("makeUsable");


            Platforms.Add(box);
            return box;
        }


        /// <summary>
        /// Create new Turrent
        /// </summary>
        /// <param name="position">Origin turret</param>
        /// <param name="angles">Angles turret</param>
        /// <returns></returns>
        public Entity Turret(Vector3 position, Vector3 angles)
        {
            Entity turret = Function.Call<Entity>("spawnTurret", "misc_turret", position, "sentry_minigun_mp");
            turret.SetField("angles", angles);
            turret.Call("setModel", "sentry_minigun");
            turret.Call("makeTurretOperable");
            turret.Call("SetDefaultDropPitch", 0);

            turret.Call("setCursorHint", "HINT_NOICON");
            turret.Call("setHintString", "Press and hold ^1[{+activate}] ^7for use ^1Turret!");
            turret.Call("makeUsable");
            turret.Call("laserOn");

            return turret;
        }
    }
}
*/