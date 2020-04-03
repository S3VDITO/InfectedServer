using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.WeaponClass;


namespace InfectedServer.KILLSTREAKS
{
    public class CarePackage : BaseScript
    {
        private static Entity _airdropCollision;

        public CarePackage()
        {
            Entity ent = Function.Call<Entity>("GetEnt", "care_package", "targetname");
            _airdropCollision = Function.Call<Entity>("GetEnt", ent.GetField<string>("target"), "targetname");

            OnNotify("run_crate", (owner, sourcePos, force, model, streakName, solid) =>
                PhysCP(owner.As<Entity>(),
                    sourcePos.As<Vector3>(),
                    force.As<Vector3>(),
                    model.As<string>(),
                    streakName.As<string>(),
                    solid.As<bool>()));

            OnNotify("run_init_crate", (crate) => 
            {
                if (!Crates.ContainsKey(crate.As<Entity>()))
                    Crates.Add(crate.As<Entity>(), new Dictionary<string, object>()
                    {
                        ["streakName"] = crate.As<Entity>().GetField<string>("streakName"),
                        ["owner"] = crate.As<Entity>().GetField<Entity>("owner"),
                        ["team"] = crate.As<Entity>().GetField<Entity>("owner").GetField<string>("SessionTeam"),
                        ["isUse"] = 0,
                        ["useRate"] = 0
                    });

                InitCrate(crate.As<Entity>()); 
            });

            UsableLogic();
        }

        Dictionary<Entity, Dictionary<string, object>> Crates = new Dictionary<Entity, Dictionary<string, object>>();
        public static Dictionary<Entity, Dictionary<string, HudElem>> ProgressBarForPlayer = new Dictionary<Entity, Dictionary<string, HudElem>>();

        public Entity SpawnCarePackege(Entity owner, Vector3 sourcePos, string streakName, string model, bool solid = true)
        {
            Entity dropCrate = Function.Call<Entity>("Spawn", "script_model", sourcePos);
            dropCrate.SetField("TargetName", "care_package");
            dropCrate.Call("SetModel", model);

            if(!Crates.ContainsKey(dropCrate))
                Crates.Add(dropCrate, new Dictionary<string, object>() 
                {
                    ["streakName"] = streakName,
                    ["owner"] = owner,
                    ["team"] = owner.GetField<string>("SessionTeam"),
                    ["isUse"] = 0,
                    ["useRate"] = 0
                });

            if (solid)
                dropCrate.Call("CloneBrushModelToScriptModel", _airdropCollision);

            return dropCrate;
        }

        public void UsableLogic()
        {
            OnInterval(100, () =>
            {
                try
                {
                    foreach (Entity player in Players)
                    {
                        if (!player.IsPlayer)
                            continue;

                        if (SortByDistance(Crates.Keys.ToList(), player.Origin).Count > 0)
                            player.SetField("best_dist_package", SortByDistance(Crates.Keys.ToList(), player.Origin)[0]);
                        else
                            player.SetField("best_dist_package", Entity.GetEntity(2048));

                        foreach (Entity crate in Crates.Keys)
                        {
                            if (!((Entity)Crates[crate]["owner"]).IsPlayer || ((Entity)Crates[crate]["owner"]).HasField("juggernaut"))
                                DeleteCrate(crate);
                        }

                        if (player.Origin.DistanceTo(player.GetField<Entity>("best_dist_package").Origin) <= 120 &&
                           player.IsAlive &&
                           !player.HasField("juggernaut"))
                        {
                            if (((Entity)Crates[player.GetField<Entity>("best_dist_package")]["owner"]) == player)
                            {
                                if (player.GetField<string>("SessionTeam") == "axis")
                                    return true;

                                if (player.Call<int>("UseButtonPressed") == 1)
                                {
                                    Crates[player.GetField<Entity>("best_dist_package")]["isUse"] = 1;

                                    Crates[player.GetField<Entity>("best_dist_package")]["useRate"] = (((int)Crates[player.GetField<Entity>("best_dist_package")]["useRate"]) + 1);

                                    if ((int)Crates[player.GetField<Entity>("best_dist_package")]["useRate"] == 1)
                                    {
                                        foreach (HudElem bars in ProgressBarForPlayer[player].Values)
                                            bars.Alpha = 1;

                                        player.UpdateBarScale(ProgressBarForPlayer[player]["Progress"], 0.8f);

                                        player.Call("PlayerLinkTo", player.GetField<Entity>("best_dist_package"));
                                        player.Call("PlayerLinkedOffsetEnable");
                                        player.Call("DisableWeapons");

                                        return true;
                                    }
                                    else if ((int)Crates[player.GetField<Entity>("best_dist_package")]["useRate"] == 10)
                                    {
                                        foreach (HudElem bars in ProgressBarForPlayer[player].Values)
                                            bars.Alpha = 0;

                                        player.Call("Unlink");
                                        player.Call("EnableWeapons");

                                        player.Call("PlayLocalSound", "ammo_crate_use");

                                        switch (Crates[player.GetField<Entity>("best_dist_package")]["streakName"])
                                        {
                                            case "helicopter":
                                                AmmoBox(player);
                                                break;
                                            case "airdrop_assault":
                                                Sniper(player);
                                                break;
                                            case "airdrop_juggernaut":
                                                Juggernaut(player);
                                                break;
                                            case "deployable_vest":
                                                BallisticVest(player);
                                                break;
                                            default:

                                                break;
                                        }

                                        DeleteCrate(player.GetField<Entity>("best_dist_package"));
                                        return true;
                                    }
                                }
                                else
                                {
                                    Crates[player.GetField<Entity>("best_dist_package")]["useRate"] = 0;

                                    foreach (HudElem bars in ProgressBarForPlayer[player].Values)
                                        bars.Alpha = 0;

                                    player.Call("Unlink");
                                    player.Call("EnableWeapons");
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
                catch
                { 

                }
                return true;
            });
        }

        private List<Entity> SortByDistance(List<Entity> array, Vector3 origin)
        {
            Dictionary<Entity, float> distances = new Dictionary<Entity, float>();

            foreach (Entity target in array)
                distances.Add(target, target.Origin.DistanceTo(origin));

            return distances.OrderBy(x => x.Value).Select(x => x.Key).ToList();
        }

        public void PhysCP(Entity owner, Vector3 sourcePos, Vector3 force, string model = "com_plasticcase_friendly", string streakName = "", bool solid = true)
        {
            Entity drop_crate = SpawnCarePackege(owner, sourcePos, streakName, model, solid);
            drop_crate.Call("PhysicsLaunchServer", new Vector3(), force);
            WaitForDropCrateMsg(drop_crate);
        }

        public void WaitForDropCrateMsg(Entity drop_crate)
        {
            Vector3 oldPos = drop_crate.Origin;
            Vector3 nowPos = new Vector3();
            OnInterval(500, () =>
            {
                if (!((Entity)Crates[drop_crate]["owner"]).IsPlayer)
                    return false;

                if (oldPos.DistanceTo(nowPos) < 5)
                {
                    InitCrate(drop_crate);
                    return false;
                }
                else
                {
                    nowPos = drop_crate.Origin;
                    AfterDelay(250, () => oldPos = drop_crate.Origin);

                    return true;
                }
            });
        }

        public void InitCrate(Entity crate)
        {
            crate.Call("setCursorHint", "HINT_NOICON");
            crate.Call("setHintString", GetHintText(((string)Crates[crate]["streakName"])));
            crate.Call("makeUsable");
            Crates[crate].Add("wayPoint", HUDClass.HeadIcon(crate.Origin, ((Entity)Crates[crate]["owner"]), GetCrateIcon(((string)Crates[crate]["streakName"]))));
        }

        public void DeleteCrate(Entity crate)
        {
            crate.Call("Delete");

            ((HudElem)Crates[crate]["wayPoint"]).Call("Destroy");

            foreach (HudElem bars in ProgressBarForPlayer[((Entity)Crates[crate]["owner"])].Values)
                bars.Alpha = 0;

            Crates.Remove(crate);
        }
        public void BallisticVest(Entity self)
        {
            Notify("set_vest_armor", self);

            AfterDelay(750, () =>
            { 

                self.Call("SetViewModel", "viewhands_juggernaut_ally");

                Notify("ShowStreakHUD", self, "Blast Shield Pro", GetKillstreakDpadIcon("_specialty_blastshield_ks_pro"), "Proficiency: Kick", "achieve_blastshield", "earn_perk");

                self.SetPerk("_specialty_blastshield", true, true);
                self.SetPerk("specialty_stun_resistance", true, true);
                self.SetPerk("specialty_marksman", true, true);
                self.SetPerk("specialty_fastoffhand", true, false);

                Say(self, "^8" + self.Name + "^7: I got a ballistic vest^7!", "^9" + self.Name + "^7: I got a ballistic vest^7!");
                self.TeamPlayerCardSplash("used_deployable_vest", "");

                AfterDelay(450, () =>
                {
                    self.Call("OpenMenu", "perk_display");

                    self.TakeWeapon("flash_grenade_mp");
                    self.TakeWeapon("concussion_grenade_mp");
                    self.TakeWeapon("claymore_mp");
                    self.TakeWeapon("bouncingbetty_mp");

                    self.Call("SetOffhandPrimaryClass", "other");
                    self.GiveWeapon("semtex_mp");

                    self.Call("SetOffhandSecondaryClass", "flash");
                    self.GiveWeapon("trophy_mp");

                });

                AfterDelay(2750, () =>
                {
                    self.Call("PlayLocalSound", "new_weapon_unlocks");
                    Notify("XpEventPopup", self, "New Equipment unlock", new Vector3(1.0f, 1.0f, 0.5f), 0);
                });
            });
        }
        public void AmmoBox(Entity self)
        {
            self.Health = 100;

            AfterDelay(500, () =>
            {
                self.Call("SetWeaponAmmoClip", self.CurrentWeapon, 0);


                AfterDelay(250, () =>
                {
                    Notify("ShowStreakHUD", self, "Scavenger Pro", GetKillstreakDpadIcon("specialty_scavenger_ks_pro"), "Proficiency: Damage", "achieve_scavenger", "earn_perk");

                    self.SetPerk("specialty_scavenger", true, true);
                    self.SetPerk("specialty_extraammo", true, true);
                    self.SetPerk("specialty_moredamage", true, true);

                    Say(self, "^8" + self.Name + "^7: I have a full ammo!", "^9" + self.Name + "^7: I have a full ammo!");
                });

                AfterDelay(700, () =>
                {
                    self.Call("OpenMenu", "perk_display");

                    self.Call("SetWeaponAmmoStock", self.CurrentWeapon, 300);
                    self.Call("GiveMaxAmmo", self.CurrentWeapon);

                    self.TakeWeapon("claymore_mp");
                    self.TakeWeapon("semtex_mp");
                    self.TakeWeapon("concussion_grenade_mp");
                    self.TakeWeapon("trophy_mp");

                    self.Call("SetOffhandPrimaryClass", "other");
                    self.GiveWeapon("bouncingbetty_mp");
                    self.Call("GiveMaxAmmo", "bouncingbetty_mp");

                    self.Call("SetOffhandSecondaryClass", "flash");
                    self.GiveWeapon("flash_grenade_mp");
                });

                AfterDelay(3000, () =>
                {
                    self.Call("PlayLocalSound", "new_weapon_unlocks");
                    Notify("XpEventPopup", self, "New Equipment unlock", new Vector3(1.0f, 1.0f, 0.5f), 0);
                });
            });
        }
        public void Sniper(Entity self)
        {
            string AssaultWeapon = BuildWeapon(WeaponClass.Type.AssaultRifle, 5);
            string Rifle = BuildWeapon(WeaponClass.Type.SniperRifle, 5);

            self.TakeAllWeapons();
            self.Health = 100;

            self.GiveWeapon(Rifle);
            self.Call("GiveMaxAmmo", Rifle);
            self.SwitchToWeapon(Rifle);

            self.GiveWeapon(AssaultWeapon);

            AfterDelay(150, () =>
            {
                Say(self, "^8" + self.Name + "^7: I got a new weapons!", "^9" + self.Name + "^7: I got a new weapons!"); //Выводим сообщение в чат, одно команде игрока, другое команде вражеской

                self.TeamPlayerCardSplash("giveaway_airdrop", "");

                AfterDelay(750, () =>
                {
                    Notify("ShowStreakHUD", self, "Marksman Pro", GetKillstreakDpadIcon("specialty_autospot_ks_pro"), "Proficiency: Range", "achieve_marksman", "earn_perk");

                    self.SetPerk("specialty_autospot", true, true);
                    self.SetPerk("specialty_holdbreath", true, true);
                    self.SetPerk("specialty_longerrange", true, true);
                    self.SetPerk("specialty_overkillpro", false, true);

                    self.Call("SetOffhandPrimaryClass", "other");
                    self.GiveWeapon("claymore_mp");

                    self.Call("SetOffhandSecondaryClass", "smoke");
                    self.GiveWeapon("concussion_grenade_mp");
                });

                AfterDelay(1200, () => self.Call("OpenMenu", "perk_display"));

                AfterDelay(3350, () =>
                {
                    self.Call("PlayLocalSound", "new_weapon_unlocks");
                    Notify("XpEventPopup", self, "New Equipment unlock", new Vector3(1.0f, 1.0f, 0.5f), 0);
                });
            });
        }
        public void Juggernaut(Entity self)
        {
            Notify("set_juggernaut_armor", self);

            AfterDelay(150, () =>
            {
                self.Call("DetachAll");
                self.Call("ShowAllParts");

                self.Call("SetViewModel", "viewhands_juggernaut_opforce");
                self.Call("SetModel", "mp_fullbody_opforce_juggernaut");

                self.TakeAllWeapons();

                string PrimaryWeapon = LMG[new Random().Next(0, LMG.Count)];
                string SecondaryWeapon = "iw5_fmg9_mp_akimbo_xmags_silencer";

                string PrimaryOffhand = "";
                string SecondaryOffhand = "smoke_grenade_mp";

                self.GiveWeapon(PrimaryWeapon);
                self.Call("GiveMaxAmmo", PrimaryWeapon);
                self.SwitchToWeapon(PrimaryWeapon);

                self.GiveWeapon(SecondaryWeapon);
                self.Call("GiveMaxAmmo", SecondaryWeapon);

                self.Call("VisionSetNakedForPlayer", "default");

                self.Call("SetOffhandSecondaryClass", "smoke");
                self.GiveWeapon(SecondaryOffhand);
                self.Call("SetWeaponAmmoClip", SecondaryOffhand, 1);

                self.RemoveKillstreak();
                self.AllPerksBonus();

                self.SetPerk("specialty_radarjuggernaut", true, true);

                AfterDelay(750, () =>
                {
                    Notify("ShowStreakHUD", self, "All Specialist Perks", GetKillstreakDpadIcon("all_perks_bonus"), "All Proficiency!", "achieve_specialty_bonus", "mp_bonus_start");

                    Say(self, "^8" + self.Name + "^7: I got a juggernaut^7 !", "^9" + self.Name + "^7: I got a juggernaut^7 !");

                    self.TeamPlayerCardSplash("used_juggernaut", "");
                });
            });
        }
        public void Say(Entity self, string messageFriendly, string messageEnemy)
        {
            for (int i = 0; i < 18; i++)
            {
                if (Entity.GetEntity(i).IsPlayer)
                {
                    if (Entity.GetEntity(i).GetField<string>("SessionTeam") == self.GetField<string>("SessionTeam"))
                        Utilities.RawSayTo(Entity.GetEntity(i), messageFriendly);
                    else
                        Utilities.RawSayTo(Entity.GetEntity(i), messageEnemy);
                }
                else
                    break;
            }
        }
    }
}
