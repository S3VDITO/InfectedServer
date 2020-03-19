using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InfectedServer.DebugClass;
using static InfectedServer.LevelClass.INFO;
using InfinityScript;

namespace InfectedServer.KILLSTREAKS
{
    public class SignalGrenade : BaseScript
    {
        public SignalGrenade()
        {
            OnNotify("grenade_fire", (ent, weapon) =>
            {
                try
                {
                    String weaponName = (String)weapon;
                    Entity grenade = (Entity)ent;

                    if (weaponName == GetKillstreakWeapon("airdrop_assault"))
                    {
                        grenade.OnNotify("missile_stuck", (g, pos) =>
                        {
                            if (Function.Call<Entity>("GetMissileOwner", grenade).HasWeapon(GetKillstreakWeapon("airdrop_assault")))
                            {
                                Notify("harrier", Function.Call<Entity>("GetMissileOwner", grenade), grenade.Origin);

                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "hasStreak", 1, false);
                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "icons", 1, GetKillstreakIndex("airdrop_assault"));

                                Function.Call<Entity>("GetMissileOwner", grenade).TakeWeapon(GetKillstreakWeapon("airdrop_assault"));

                                g.Call("Detonate");

                                return;
                            }
                        });
                    }

                    if (weaponName == GetKillstreakWeapon("airdrop_juggernaut"))
                    {
                        grenade.OnNotify("missile_stuck", (g, pos) =>
                        {
                            if (Function.Call<Entity>("GetMissileOwner", grenade).HasWeapon(GetKillstreakWeapon("airdrop_juggernaut")))
                            {
                                Notify("jugg", Function.Call<Entity>("GetMissileOwner", grenade), grenade.Origin);

                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "hasStreak", 3, false);
                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "icons", 3, GetKillstreakIndex("airdrop_juggernaut"));

                                Function.Call<Entity>("GetMissileOwner", grenade).TakeWeapon(GetKillstreakWeapon("airdrop_juggernaut"));

                                g.Call("Detonate");

                                return;
                            }
                        });
                    }

                    if (weaponName == GetKillstreakWeapon("deployable_vest"))
                    {
                        grenade.OnNotify("missile_stuck", (g, pos) =>
                        {
                            if (Function.Call<Entity>("GetMissileOwner", grenade).HasWeapon(GetKillstreakWeapon("deployable_vest")))
                            {
                                Notify("Deploy_Stuck", Function.Call<Entity>("GetMissileOwner", grenade), grenade);

                                Entity box = Function.Call<Entity>("Spawn", "script_model", grenade.Origin);
                                box.Call("SetModel", "com_deploy_ballistic_vest_friend_world");
                                box.SetField("owner", Function.Call<Entity>("GetMissileOwner", grenade));
                                box.SetField("streakName", "deployable_vest");

                                Notify("run_init_crate", box);

                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "hasStreak", 2, false);
                                Function.Call<Entity>("GetMissileOwner", grenade).Call("SetPlayerData", "killstreaksState", "icons", 2, GetKillstreakIndex("deployable_vest"));

                                Function.Call<Entity>("GetMissileOwner", grenade).TakeWeapon(GetKillstreakWeapon("deployable_vest"));
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    SendConsole("[FUCKED EXCEPTION] [Grenade] Info: " + ex.ToString());
                }
            });
        }
    }
}
