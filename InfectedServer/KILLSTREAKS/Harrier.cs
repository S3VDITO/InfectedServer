using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.KILLSTREAKS
{
    public class Harrier : BaseScript
    {
        public Harrier()
        {
            OnNotify("harrier", (owner, pos) => CallHarrier(owner.As<Entity>(), pos.As<Vector3>()));
        }
        public void CallHarrier(Entity self, Vector3 goalPos)
        {
            float yaw = Function.Call<float>("RandomFloat", 360);
            Vector3 endPos = new Vector3(goalPos.X, goalPos.Y, AIR_HEIGHT);

            Entity harrier = HARRIER_SETUP(self, GetPathStart(goalPos, yaw), new Vector3(0, yaw, 0));
            harrier.SetField("owner", self);

            DamageHandler(harrier);

            int MaxGoals = 1;

            Action goal = new Action(() => { });

            goal = new Action(() =>
            {
                if (harrier.HasField("delete"))
                    return;

                if (MaxGoals == 1)
                {
                    if (!harrier.HasField("dropped"))
                    {
                        harrier.SetField("dropped", 1);
                        Notify("run_crate", harrier.GetField<Entity>("owner"), harrier.Origin, new Vector3(0, 0, 0), "com_plasticcase_friendly", "airdrop_assault", true);
                    }

                    AfterDelay(1500, () => SetVehGoalPos(goal, harrier, GetPathEnd(endPos, yaw)));

                }

                if (MaxGoals == 0)
                {
                    DestroyHarrier(harrier, false);
                    return;
                }

                MaxGoals--;
            });

            SetVehGoalPos(goal, harrier, endPos);
        }
        public Entity HARRIER_SETUP(Entity self, Vector3 startPos, Vector3 forward)
        {
            Entity harrier = Function.Call<Entity>("SpawnHelicopter", self, startPos, forward, "harrier_mp", "vehicle_av8b_harrier_jet_opfor_mp");
            harrier.Call("SetHoverParams", 50, 100, 50);
            harrier.Call("SetTurningAbility", .05f);
            harrier.Call("SetYawSpeed", 250, 250, 250, .5f);
            harrier.Call("SetSpeed", 250, 175);
            harrier.Call("SetMaxPitchRoll", 0, 0);
            VEHICLES.Add(harrier);
            return harrier;
        }
        public void DamageHandler(Entity harrier)
        {
            harrier.Call("setCanDamage", true);
            harrier.Health = 1500; // keep it from dying anywhere in code
            harrier.SetField("maxHealth", 1500); // this is the health we'll check

            harrier.OnNotify("damage", (ent, damage, attacker, direction_vec, point, sMeansOfDeath, modelName, tagName, partName, iDFlags, sWeapon) =>
            {
                if (harrier.HasField("delete"))
                    return;

                string weapon = (string)sWeapon;

                Notify("damage_feedback", attacker, "damage_feedback");

                if (weapon.Contains("stinger"))
                    DestroyHarrier(harrier, true);

                Notify("damage_feedback", attacker, "damage_feedback");
            });
        }
        public void DestroyHarrier(Entity harrier, bool isExplosive)
        {
            if (isExplosive)
            {
                double rot = new Random().Next(0, 360);
                Entity explosionEffect = Function.Call<Entity>("spawnFx", Function.Call<int>("loadfx", "explosions/tanker_explosion"), harrier.Origin, new Vector3(0, 0, 1), new Vector3((float)Math.Cos(rot), (float)Math.Sin(rot), 0));
                Function.Call("triggerFx", explosionEffect);
                Function.Call("playSoundAtPos", harrier.Origin, "exp_suitcase_bomb_main");
            }

            if (!harrier.HasField("dropped"))
            {
                harrier.SetField("dropped", 1);
                Notify("run_crate", harrier.GetField<Entity>("owner"), harrier.Origin, new Vector3(0, 0, 0), "com_plasticcase_friendly", "airdrop_assault", true);
            }

            VEHICLES.Remove(harrier);
            harrier.SetField("delete", 1);
            harrier.Call("Delete");
        }
        public void SetVehGoalPos(Action action_goal, Entity harrier, Vector3 endPos, bool stopAtPos = true)
        {
            if (harrier.HasField("delete"))
                return;

            harrier.Call("SetVehGoalPos", endPos, stopAtPos);
            harrier.OnInterval(500, heli =>
            {
                if (harrier.HasField("delete"))
                    return false;

                if (harrier.Origin.DistanceTo2D(endPos) <= 50)
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
