using InfinityScript;
using System;
using System.Collections.Generic;
using System.Collections;

using static InfinityScript.ThreadScript;

namespace InfectedServer
{
    public class Player : PlayerModel
    {
        public event Action<Player> SpawnedPlayer;

        public Player(Entity player) : base(player, new Dictionary<string, string>()
        {
            ["allies_weapon_primary"] = RandomWeapon.GenerateWeapon(RandomWeapon.RifleType.ShotGun),
            ["allies_weapon_secondary"] = RandomWeapon.GenerateWeapon(RandomWeapon.RifleType.Pistol),
            ["allies_grenade_primaryGrenade"] = "",
            ["allies_grenade_secondaryGrenade"] = "",
        })
        {
            if (!player.IsPlayer)
            {
                Log.Error("Player(Entity) - Entity is not player!");
                return;
            }

            ServerData.Players.Add(this);

            #region SpawnedPlayer
            Thread(spawnedPlayer(), (entRef, notify, paras) =>
            {
                if (entRef == Entity.EntRef && notify == "disconnect")
                {
                    ServerData.Players.Remove(this);
                    return false;
                }

                return true;
            });
            IEnumerator spawnedPlayer()
            {
                while (true)
                    yield return Entity.WaitTill("spawned_player", (paras) => SpawnedPlayer?.Invoke(this));
            }
            #endregion
        }

        public void GiveKit()
        {
            if (IsHuman)
                Thread(GiveHumanPack());

            if (IsInfected)
                Thread(GiveInfectPack());
        }

        private IEnumerator GiveHumanPack()
        {
            Entity.TakeAllWeapons();

            yield return Wait(.5f);

            Entity.GiveWeapon(Kit["allies_weapon_primary"]);
            Entity.SwitchToWeapon(Kit["allies_weapon_primary"]);

            Entity.GiveWeapon(Kit["allies_weapon_secondary"]);

            Entity.GivePrimaryOffhand(Kit["allies_grenade_primaryGrenade"]);
            Entity.GiveSecondaryOffhand(Kit["allies_grenade_secondaryGrenade"]);
        }

        private IEnumerator GiveInfectPack()
        {
            throw new NotImplementedException();
        }

        public void IncKillStreak()
        {
            KillStreaks++;
        }
        public void RemoveKillStreak()
        {
            KillStreaks = 0;
        }

        public void IncDeathStreak()
        {
            DeathStreaks++;
        }
        public void RemoveDeathStreak()
        {
            DeathStreaks = 0;
        }
    }
}
