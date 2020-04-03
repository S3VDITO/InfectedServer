using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using InfectedServer.KILLSTREAKS;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.WEAPONS
{
	public class Stinger : BaseScript
	{
		public Stinger()
		{
			OnNotify("stinger_init", (player) => StingerUsageLoop((Entity)player));
		}

		public void InitStingerUsage(Entity player)
		{
			player.SetField("stingerStage", 0);
			player.SetField("stingerTarget", Entity.GetEntity(-1));
			player.SetField("stingerLockStartTime", 0);
			player.SetField("stingerLostSightlineTime", 0);
		}


		public void ResetStingerLocking(Entity player)
		{
			player.Notify("stop_javelin_locking_feedback");
			player.Notify("stop_javelin_locked_feedback");

			player.Call("WeaponLockFree");

			InitStingerUsage(player);
		}

		public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
		{
			if (player.IsPlayer)
				ResetStingerLocking(player);

			base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
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

		public bool StillValidStingerLock(Entity self, Entity ent)
		{
			if (ent.Call<int>("IsDefined") == 0)
				return false;
			if (self.Call<int>("WorldPointInReticle_Circle", ent.Origin, 65, 85) == 0)
				return false;

			return true;
		}

		public void LoopStingerLockingFeedback(Entity player)
		{
			player.Call("playLocalSound", "stinger_locking");
		}

		public void LoopStingerLockedFeedback(Entity player)
		{
			player.Call("playLocalSound", "stinger_locked");
		}

		public bool SoftSightTest(Entity player)
		{
			int LOST_SIGHT_LIMIT = 500;

			if (LockSightTest(player, player.GetField<Entity>("stingerTarget")))
			{
				player.SetField("stingerLostSightlineTime", 0);
				return true;
			}

			if (player.GetField<int>("stingerLostSightlineTime") == 0)
				player.SetField("stingerLostSightlineTime", Function.Call<int>("getTime"));

			int timePassed = Function.Call<int>("getTime") - player.GetField<int>("stingerLostSightlineTime");

			if (timePassed >= LOST_SIGHT_LIMIT)
			{
				ResetStingerLocking(player);
				return false;
			}

			return true;
		}

		public void StingerUsageLoop(Entity player)
		{
			int LOCK_LENGTH = 1000;

			InitStingerUsage(player);

			player.SetField("targetEnts", new Parameter(new List<Entity>()));

			Entity targetEntity = null;

			List<Entity> TargetsInReticle = new List<Entity>();

			player.OnInterval(500, e =>
			{
				if (player.CurrentWeapon != "stinger_mp")
				{
					ResetStingerLocking(player);
					return true;
				}

				if (player.Call<float>("PlayerAds") < 0.95f)
				{
					ResetStingerLocking(player);
					return true;
				}

				foreach (Entity target in GetTargets())
				{
					bool insideReticle = player.Call<int>("WorldPointInReticle_Circle", target.Origin, 65, 75) > 0;

					if (!insideReticle)
						continue;

					TargetsInReticle.Add(target);
				}

				if (TargetsInReticle.Count != 0)
				{
					LoopStingerLockedFeedback(player);

					targetEntity = SortByDistance(TargetsInReticle, player.Origin)[0];

					if (targetEntity == RemoteMissile.UAV_MODEL)
						BadLock(player, targetEntity);
					else if (targetEntity == AC130_MODEL_LEVEL)
						BadLock(player, targetEntity);
					else
						player.Call("WeaponLockFinalize", targetEntity, new Vector3(0, 0, 0));

					LoopStingerLockedFeedback(player);
				}
				else
					ResetStingerLocking(player);

				TargetsInReticle.Clear();
				return true;
			});
		}

		public void BadLock(Entity player, Entity target)
		{
			player.Call("WeaponLockFinalize", target, new Vector3(0, 0, -256));
		}

		public List<Entity> GetTargets()
		{
			List<Entity> result = new List<Entity>();

			result.Add(AC130_MODEL_LEVEL);
			result.Add(RemoteMissile.UAV_MODEL);

			foreach (Entity veh in VEHICLES)
				result.Add(veh);

			return result;
		}

		private List<Entity> SortByDistance(List<Entity> targetsInReticle, Vector3 origin)
		{
			Dictionary<Entity, float> distances = new Dictionary<Entity, float>();

			foreach (Entity target in targetsInReticle)
				distances.Add(target, target.Origin.DistanceTo(origin));

			return distances.OrderBy(x => x.Value).Select(x => x.Key).ToList();
		}
	}
}
