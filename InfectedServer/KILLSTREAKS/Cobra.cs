using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.KILLSTREAKS
{
    public class Cobra : BaseScript
    {
        public Cobra()
        {
            OnNotify("cobra", (owner, pos) => CallCobra(owner.As<Entity>(), pos.As<Vector3>()));
        }
        public void CallCobra(Entity self, Vector3 goalPos)
        {
            float yaw = Function.Call<float>("RandomFloat", 360);
            Vector3 startPos = GetPathStart(goalPos, yaw);
            Vector3 endPos = new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT);

            Entity cobra = COBRA_SETUP(self, startPos, new Vector3(0, yaw, 0));
            cobra.SetField("owner", self);
            DamageHandler(cobra);

            int MaxGoals = 1;

            Action goal = new Action(() => { });

            goal = new Action(() =>
            {
                if (cobra.HasField("delete"))
                    return;

                if (MaxGoals == 1)
                {
                    if (!cobra.HasField("dropped"))
                    {
                        cobra.SetField("dropped", 1);
                        Notify("run_crate", cobra.GetField<Entity>("owner"), endPos, new Vector3(), "com_deploy_ballistic_vest_friend_world", "helicopter", false);
                    }

                    AfterDelay(1500, () => SetVehGoalPos(goal, cobra, GetPathEnd(endPos, yaw - 90)));
                }
                if (MaxGoals == 0)
                {
                    DestroyCobra(cobra, false);
                    return;
                }

                MaxGoals--;
            });

            SetVehGoalPos(goal, cobra, endPos);
        }
        public Entity COBRA_SETUP(Entity self, Vector3 startPos, Vector3 forward)
        {
            Entity cobra = Function.Call<Entity>("SpawnHelicopter", self, startPos, forward, "cobra_mp", "vehicle_mi24p_hind_mp");
            cobra.Call("SetMaxPitchRoll", 45, 85);
            cobra.Call("SetYawSpeed", 120, 80);
            cobra.Call("SetSpeed", 130, 125);

            VEHICLES.Add(cobra);

            return cobra;
        }
        public void DamageHandler(Entity cobra)
        {
            cobra.Call("setCanDamage", true);
            cobra.Health = 1000; // keep it from dying anywhere in code
            cobra.SetField("maxHealth", 1000); // this is the health we'll check

            cobra.OnNotify("damage", (ent, damage, attacker, direction_vec, point, sMeansOfDeath, modelName, tagName, partName, iDFlags, sWeapon) =>
            {
                if (cobra.HasField("delete"))
                    return;

                string weapon = (string)sWeapon;

                Notify("damage_feedback", attacker, "damage_feedback");

                if (weapon.Contains("stinger"))
                    DestroyCobra(cobra, true);
            });
        }
        public void DestroyCobra(Entity cobra, bool isExplosive)
        {
            if (isExplosive)
            {
                double rot = new Random().Next(0, 360);
                Entity explosionEffect = Function.Call<Entity>("spawnFx", Function.Call<int>("loadfx", "explosions/tanker_explosion"), cobra.Origin, new Vector3(0, 0, 1), new Vector3((float)Math.Cos(rot), (float)Math.Sin(rot), 0));
                Function.Call("triggerFx", explosionEffect);
                Function.Call("playSoundAtPos", cobra.Origin, "exp_suitcase_bomb_main");
            }

            if (!cobra.HasField("dropped"))
            {
                cobra.SetField("dropped", 1);
                Notify("run_crate", cobra.GetField<Entity>("owner"), cobra.Origin, new Vector3(), "com_deploy_ballistic_vest_friend_world", "helicopter", false);
            }
            VEHICLES.Remove(cobra);
            cobra.SetField("delete", 1);
            cobra.Call("Delete");
        }
        public void SetVehGoalPos(Action action_goal, Entity cobra, Vector3 endPos, bool stopAtPos = true)
        {
            if (cobra.HasField("delete"))
                return;

            cobra.Call("SetVehGoalPos", endPos, stopAtPos);
            cobra.OnInterval(500, heli =>
            {
                if (cobra.HasField("delete"))
                    return false;

                if (cobra.Origin.DistanceTo2D(endPos) <= 50)
                {
                    action_goal.Invoke();
                    return false;
                }
                return true;
            });
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
