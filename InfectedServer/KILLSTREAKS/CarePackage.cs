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

            OnNotify("run_init_crate", (crate) => InitCrate(crate.As<Entity>()));
        }

        public Entity SpawnCarePackege(Entity owner, Vector3 sourcePos, string streakName, string model, bool solid = true)
        {
            Entity dropCrate = Function.Call<Entity>("Spawn", "script_model", sourcePos);
            dropCrate.SetField("TargetName", "care_package");
            dropCrate.SetField("streakName", streakName);
            dropCrate.SetField("owner", owner);

            dropCrate.Call("SetModel", model);

            if (solid)
                dropCrate.Call("CloneBrushModelToScriptModel", _airdropCollision);

            return dropCrate;
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
            crate.Call("setHintString", GetHintText(crate.GetField<string>("streakName")));
            crate.Call("makeUsable");

            byte useRate = 0;
            int time = 600;
            Entity owner = crate.GetField<Entity>("owner");

            Dictionary<string, HudElem> BARS = owner.CreateBar();
            HudElem WayPoint = HUDClass.HeadIcon(crate.Origin, owner, GetCrateIcon(crate.GetField<string>("streakName")));

            OnInterval(100, () =>
            {
                time--;

                if (!owner.IsAlive || !owner.IsPlayer || time == 0 || owner.HasField("juggernaut"))
                {
                    DeleteCrate(crate, WayPoint, BARS);
                    return false;
                }

                if (crate.Origin.DistanceTo(owner.Origin) <= 120 &&
                owner.IsAlive &&
                owner.GetField<string>("SessionTeam") == "allies" &&
                owner.Call<int>("UseButtonPressed") == 1)
                {
                    useRate++;
                    if (useRate == 1)
                    {
                        foreach (HudElem bars in BARS.Values)
                            bars.Alpha = 1;

                        owner.UpdateBarScale(BARS["Progress"], 0.8f);

                        owner.Call("PlayerLinkTo", crate);
                        owner.Call("PlayerLinkedOffsetEnable");
                        owner.Call("DisableWeapons");

                        return true;
                    }
                    else if (useRate == 10)
                    {
                        foreach (HudElem bars in BARS.Values)
                            bars.Alpha = 0;

                        owner.Call("Unlink");
                        owner.Call("EnableWeapons");

                        owner.Call("PlayLocalSound", "ammo_crate_use");

                        switch (crate.GetField<string>("streakName"))
                        {
                            case "airdrop_assault":
                                Sniper(owner);
                                break;
                            case "airdrop_juggernaut":
                                Juggernaut(owner);
                                break;
                            case "helicopter":
                                AmmoBox(owner);
                                break;
                            case "deployable_vest":
                                BallisticVest(owner);
                                break;
                            default:
                                
                                break;
                        }

                        DeleteCrate(crate, WayPoint, BARS);
                        return false;
                    }
                }
                else
                {
                    foreach (HudElem bars in BARS.Values)
                        bars.Alpha = 0;

                    owner.Call("Unlink");
                    owner.Call("EnableWeapons");

                    useRate = 0;

                    return true;
                }

                return true;
            });
        }

        public void DeleteCrate(Entity crate, HudElem wayPoint, Dictionary<string, HudElem> progressBar)
        {
            crate.Call("Delete");
            wayPoint.Call("Destroy");

            foreach (var pg in progressBar.Values)
                pg.Call("Destroy");
        }

        public void BallisticVest(Entity self)
        {
            self.Health = 100;

            //self.SetModelBlastshield();

            AfterDelay(750, () =>
            {
                Notify("set_vest_armor", self);

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
            AfterDelay(150, () =>
            {
                self.Call("DetachAll");
                self.Call("ShowAllParts");

                Notify("set_juggernaut_armor", self);

                self.Call("SetViewModel", "viewhands_juggernaut_opforce");
                self.Call("SetModel", "mp_fullbody_opforce_juggernaut");

                self.TakeAllWeapons();

                string PrimaryWeapon = "iw5_m60jugg_mp_thermal_heartbeat_silencer_xmags_camo07";
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

                self.SetClientDvar("r_filmtweaks", "1");
                self.SetClientDvar("r_filmUseTweaks", "1");
                self.SetClientDvar("r_filmTweakEnable", "1");

                self.SetClientDvar("r_filmtweakcontrast", "1.3");
                self.SetClientDvar("r_filmtweakdarktint", "1.3 1.0 1.0");
                self.SetClientDvar("r_filmtweaklighttint", "1.3 1.0 1.0");
                self.SetClientDvar("r_filmtweakdesaturation", "0.1");

                self.SetClientDvar("r_filmTweakBrightness", "0.1");

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
