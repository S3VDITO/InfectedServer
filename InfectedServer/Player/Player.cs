using InfinityScript;
using System;
using System.Collections.Generic;
using System.Collections;

using static InfinityScript.ThreadScript;

namespace InfectedServer
{
    public class Player
    {
        public Dictionary<string, string> Loadout { get; }

        public Entity Entity { get; }

        public int ID 
        {
            get => Entity.EntRef;
        }
        public long GUID 
        {
            get => Entity.GUID;
        }
        public string Name 
        { 
            get => Entity.Name;
            set => Entity.Name = value;
        }
        public string ClanTag
        {
            get => Entity.ClanTag;
            set => Entity.ClanTag = value;
        }

        public int KillStreak { get; set; } = 0;
        public int DeathStreak { get; set; } = 0;

        public event Action PlayerSpawned;

        public Player(Entity player, Dictionary<string, string> loadout)
        {
            if (!player.IsPlayer)
                throw new Exception($"{player.EntRef} is not player!");

            Entity = player;
            Loadout = loadout;

            #region PlayerSpawned event handling
            Thread(playerSpawned(), (entRef, notify, paras) =>
            {
                if (notify == "disconnect" && player.EntRef == entRef)
                    return false;

                return true;
            });

            IEnumerator playerSpawned()
            {
                while (true)
                    yield return player.WaitTill("spawned_player", paras => PlayerSpawned?.Invoke());
            }
            #endregion
        }

        public void GiveWeapon(string weaponName) => Entity.GiveWeapon(weaponName);
        public void GiveWeapon(string weaponName, int indexCamo) => Entity.GiveWeapon(weaponName, indexCamo);
        public void GiveWeapon(string weaponName, int indexCamo, bool akimbo) => Entity.GiveWeapon(weaponName, indexCamo, akimbo);

        public void SetOrigin(Vector3 origin) => Entity.SetOrigin(origin);

        public void SetPerk(string perkName) => Entity.SetPerk(perkName);
        public void SetPerk(string perkName, bool codePerk) => Entity.SetPerk(perkName, codePerk);
        public void SetPerk(string perkName, bool codePerk, bool useSlot) => Entity.SetPerk(perkName, codePerk, useSlot);

        public void SetPlayerAngles(Vector3 angles) => Entity.SetPlayerAngles(angles);

        public void SetViewModel(string viewModel) => Entity.SetViewModel(viewModel);
        public void SetBodyModel(string bodyModel) => Entity.SetModel(bodyModel);
        public void SetHeadModel(string headModel)
        {
            
        }

        public void SetPlayerModel(string handModel, string bodyModel)
        {
            SetViewModel(handModel);
            SetBodyModel(bodyModel);
        }
        public void SetPlayerModel(string handModel, string bodyModel, string headModel)
        {
            SetViewModel(handModel);
            SetBodyModel(bodyModel);
            SetHeadModel(headModel);
        }

        public void Suicie() => Entity.Suicide();
    }
}
