using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;
using InfectedServer.KILLSTREAKS;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.HUDClass;
using static InfectedServer.LevelClass;
using static InfectedServer.WeaponClass;
using static InfectedServer.SoundClass;
using static InfectedServer.InitializateClass;
using static InfectedServer.DebugClass;
//using static InfectedServer.ModelClass;

namespace InfectedServer
{
    public class GamePlay : BaseScript
    {
        public static readonly Dictionary<string, int> STREAK = new Dictionary<string, int>()
        {
            ["helicopter"] = 1,
            ["airdrop_assault"] = 5,
            ["deployable_vest"] = 9,
            ["airdrop_juggernaut"] = 18,
            ["ac130"] = 30,
        };

        public GamePlay()
        {
            new InitializateClass();

            START_GAME();

            //OnNotify("debgug_give_killstreak", (self, streak) => GiveKillstreak(self.As<Entity>(), streak.As<string>()));

            OnNotify("XpEventPopup", (self, message, hudColor, glowAlpha) =>
            XpEventPopup(self.As<Entity>(), message.As<string>(), hudColor.As<Vector3>(), glowAlpha.As<float>()));

            OnNotify("ShowStreakHUD", (self, streakname, streakicon, pointstreak, earnSound, killstreakSound) =>
                ShowStreakHUD(self.As<Entity>(),
                streakname.As<string>(),
                streakicon.As<string>(),
                pointstreak.As<string>(),
                earnSound.As<string>(),
                killstreakSound.As<string>()));

            PlayerConnected += OnPlayerConnected;
            PlayerDisconnected += GamePlay_PlayerDisconnected;
        }

        private void GamePlay_PlayerDisconnected(Entity player)
        {
            if (CarePackage.ProgressBarForPlayer.ContainsKey(player))
                CarePackage.ProgressBarForPlayer.Remove(player);
        }

        private bool MayDropWeapon(String weapon)
        {
            if (weapon == "none")
                return false;

            if (weapon.Contains("ac130"))
                return false;

            if (weapon.Contains("killstreak"))
                return false;

            if (weapon.Contains("gl_mp"))
                return false;

            if (Function.Call<string>("WeaponInventoryType", weapon) != "primary")
                return false;

            return true;
        }

        private void OnPlayerConnected(Entity player)
        {
            Notify("stinger_init", player);
            player.SetField("lastDroppableWeapon", string.Empty);

            if (!CarePackage.ProgressBarForPlayer.ContainsKey(player))
                CarePackage.ProgressBarForPlayer.Add(player, player.CreateBar());

            player.OnNotify("weapon_change", (_p, _weapon) =>
            {
                String weaponName = _weapon.As<String>();

                if (MayDropWeapon(weaponName))
                    player.SetField("lastDroppableWeapon", new Parameter(weaponName));

                if (weaponName == GetKillstreakWeapon("helicopter"))
                {
                    Notify("cobra", player, player.Origin);
                    AfterDelay(500, () =>
                    {
                        player.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, false);
                        player.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("helicopter"));
                        player.SwitchToWeapon(player.GetField<String>("lastDroppableWeapon"));
                        player.TakeWeapon(GetKillstreakWeapon("helicopter"));
                    });
                }


                if (weaponName == GetKillstreakWeapon("ac130"))
                {
                    Notify("ac130", player, player.Origin);
                    AfterDelay(500, () =>
                    {
                        player.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, false);
                        player.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("ac130"));
                        player.TakeWeapon(GetKillstreakWeapon("ac130"));
                    });//predator_missile
                }

                if (weaponName == GetKillstreakWeapon("predator_missile"))
                {
                    AfterDelay(500, () =>
                    {
                        Notify("remote_mortar", player);
                        player.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, false);
                        player.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("predator_missile"));
                    });
                }

                if (weaponName == "killstreak_remote_turret_laptop_mp")
                {
                    AfterDelay(500, () =>
                    {
                        Notify("final_heli", player);
                        player.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, false);
                        player.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("helicopter_flares"));
                    });
                }//killstreak_remote_turret_laptop_mp
            });

            AfterDelay(250, () =>
            {
                WelcomeHUD(player);
                player.CreateXpEventPopup();
                player.CreateStreakHUD();

                if (player.GetField<string>("SessionTeam") == "allies")
                    HumanStartPack(player, BuildWeapon(WeaponClass.Type.ShotGuns, 4), "iw5_fnfiveseven_mp_xmags_tactical");

                if (player.GetField<string>("SessionTeam") == "axis")
                {
                    player.Call("setMoveSpeedScale", 1.35f);
                    player.Call("SetPlayerData", "killstreaksState", "isSpecialist", true);

                    if (player.CurrentWeapon.Contains("iw5_deserteagle"))
                    {
                        player.Call("setWeaponAmmoClip", player.CurrentWeapon, 0);
                        player.Call("setWeaponAmmoStock", player.CurrentWeapon, 0);
                        AfterDelay(500, () =>
                        {
                            player.Call("setWeaponAmmoClip", player.CurrentWeapon, 0);
                            player.Call("setWeaponAmmoStock", player.CurrentWeapon, 0);
                        });
                    }

                    player.SetClientDvar("bg_viewBobAmplitudeSprinting", "0 0");

                    player.SetField("maxHealth", 65);
                    player.Health = 65;
                }
            });

            player.SpawnedPlayer += new Action(() =>
            {
                AfterDelay(250, () =>
                {
                    OnPlayerSpawned(player);
                });
            });
        }

        private void OnPlayerSpawned(Entity player)
        {
            if (player.GetField<string>("SessionTeam") == "allies")
                HumanStartPack(player, BuildWeapon(WeaponClass.Type.ShotGuns, 4), "iw5_fnfiveseven_mp_xmags_tactical");

            if (player.GetField<string>("SessionTeam") == "axis")
            {
                if (player.CurrentWeapon.Contains("iw5_deserteagle"))
                {
                    player.Call("setWeaponAmmoClip", player.CurrentWeapon, 0);
                    player.Call("setWeaponAmmoStock", player.CurrentWeapon, 0);
                    AfterDelay(500, () =>
                    {
                        player.Call("setWeaponAmmoClip", player.CurrentWeapon, 0);
                        player.Call("setWeaponAmmoStock", player.CurrentWeapon, 0);
                    });
                }

                player.SetClientDvar("bg_viewBobAmplitudeSprinting", "0 0");

                player.SetField("maxHealth", 65);
                player.Health = 65;
            }
        }

        public void START_GAME()
        {
            Utilities.ExecuteCommand("set scr_killcam_time \"5\"");
            Utilities.ExecuteCommand("set scr_killcam_posttime \"1\"");

            OnNotify("prematch_done", () =>
            {
                //Function.Call("SetTeamRadar", "axis", true);
                InitializateLevel();
                Notify("spawn_uav");
            });
        }

        public void HumanStartPack(Entity player,
            string weap1,
            string weap2,
            string streak1 = "airdrop_assault",
            string streak2 = "deployable_vest",
            string streak3 = "airdrop_juggernaut")
        {
            player.Call("SetViewModel", "viewmodel_base_viewhands");

            player.TakeAllWeapons();

            player.GiveWeapon(weap1);
            player.GiveWeapon(weap2);

            player.SwitchToWeaponImmediate(weap1);

            

            player.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["airdrop_assault"]);
            player.Call("SetPlayerData", "killstreaksState", "nextIndex", 1);

            player.Call("SetPlayerData", "killstreaksState", "hasStreak", 1, false);
            player.Call("SetPlayerData", "killstreaksState", "icons", 1, GetKillstreakIndex(streak1));

            player.Call("SetPlayerData", "killstreaksState", "hasStreak", 2, false);
            player.Call("SetPlayerData", "killstreaksState", "icons", 2, GetKillstreakIndex(streak2));

            player.Call("SetPlayerData", "killstreaksState", "hasStreak", 3, false);
            player.Call("SetPlayerData", "killstreaksState", "icons", 3, GetKillstreakIndex(streak3));
        }

        public void ShowStreakHUD(Entity self, String streakname, String streakicon, String pointstreak, String earnSound, String killstreakSound)
        {
            self.GetField<HudElem>("streaknameHUD").SetText(streakname);
            self.GetField<HudElem>("streaknameHUD").Alpha = 1;

            self.GetField<HudElem>("streakiconHUD").SetPoint("center top", "center top", 0, 40);
            self.GetField<HudElem>("streakiconHUD").SetShader(streakicon, 40, 40);
            self.GetField<HudElem>("streakiconHUD").Alpha = 1;

            self.GetField<HudElem>("pointstreakHUD").SetText(pointstreak);
            self.GetField<HudElem>("pointstreakHUD").Alpha = 1;

            self.Call("PlayLocalSound", killstreakSound);
            PlayLeaderDialog(self, earnSound);

            AfterDelay(2000, () =>
            {
                self.GetField<HudElem>("streaknameHUD").Call("FadeOverTime", .2f);
                self.GetField<HudElem>("streaknameHUD").Alpha = 0;

                self.GetField<HudElem>("streakiconHUD").Call("ScaleOverTime", 1.0f, 96, 96);
                self.GetField<HudElem>("streakiconHUD").Call("FadeOverTime", .5f);
                self.GetField<HudElem>("streakiconHUD").Alpha = 0;

                self.GetField<HudElem>("pointstreakHUD").Call("FadeOverTime", .2f);
                self.GetField<HudElem>("pointstreakHUD").Alpha = 0;
            });
        }

        public void XpEventPopup(Entity self, String message, Vector3 hudColor, Single glowAlpha)
        {
            self.GetField<HudElem>("hud_xpEventPopup").Color = hudColor;
            self.GetField<HudElem>("hud_xpEventPopup").GlowColor = hudColor;
            self.GetField<HudElem>("hud_xpEventPopup").GlowAlpha = glowAlpha;

            self.GetField<HudElem>("hud_xpEventPopup").SetText(message);
            self.GetField<HudElem>("hud_xpEventPopup").Alpha = 0.85f;

            AfterDelay(5000, () =>
            {
                self.GetField<HudElem>("hud_xpEventPopup").Call("FadeOverTime", 0.75f);
                self.GetField<HudElem>("hud_xpEventPopup").Alpha = 0;
            });
        }

        public void WelcomeHUD(Entity self)
        {
            if (self.HasField("isWelcomed"))
                return;

            HudElem Welcome = HudElem.CreateFontString(self, "Objective", 1.7f);
            Welcome.SetPoint("TOPCENTER", "TOPCENTER", 0, 145); //150
            Welcome.SetText("Hi " + self.Name + " !");
            Welcome.HideWhenInMenu = true;
            Welcome.Archived = false;
            Welcome.GlowAlpha = 1;
            Welcome.GlowColor = new Vector3(1, 0, 0);
            Welcome.Call("SetPulseFX", 100, 3700, 1000);

            AfterDelay(5000, () =>
            {
                Welcome.Call("Destroy");
                self.SetField("isWelcomed", true);
            });
        }

        public void Updater(Entity attacker)
        {
            AfterDelay(150, () =>
            {
                if (STREAK["helicopter"] == attacker.GetField<int>("Kills"))
                {
                    PlayLeaderDialog(attacker, "kill_confirmed");
                    AfterDelay(1800, () => GiveKillstreak(attacker, "helicopter"));
                }
                else if (STREAK["airdrop_assault"] == attacker.GetField<int>("Kills"))
                {
                    GiveKillstreak(attacker, "airdrop_assault");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["deployable_vest"]);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 2);
                }
                else if (STREAK["deployable_vest"] == attacker.GetField<int>("Kills"))
                {
                    GiveKillstreak(attacker, "deployable_vest");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["airdrop_juggernaut"]);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 3);
                }
                else if (STREAK["airdrop_juggernaut"] == attacker.GetField<int>("Kills"))
                {
                    GiveKillstreak(attacker, "airdrop_juggernaut");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", 0);
                    attacker.Call("SetPlayerData", "killstreaksState", "numAvailable", 0);
                    attacker.Call("SetPlayerData", "killstreaksState", "selectedIndex", -1);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 1);
                }
                else if (STREAK["ac130"] == attacker.GetField<int>("Kills"))
                {
                    GiveKillstreak(attacker, "ac130");
                }
            });
        }
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (attacker.IsPlayer && player.IsPlayer && player != attacker && attacker.GetField<string>("SessionTeam") == "allies")
            {
                Updater(attacker);
                //attacker.SetField("Kills", attacker.Call<int>("GetPlayerData", "killstreaksState", "count"));

                if (attacker.Call<int>("GetPlayerData", "killstreaksState", "icons", 0) == MOAB_INDEX &&
                    attacker.Call<int>("GetPlayerData", "killstreaksState", "hasStreak", 0) == 1)
                    Notify("anticamp_start", attacker);
            }
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }

        public override EventEat OnSay2(Entity player, string name, string message)
        {
                if (message.Split(' ')[0] == "!gs")
            {
                if(Entity.GetEntity(int.Parse(message.Split(' ')[1])).IsPlayer)
                    GiveKillstreak(Entity.GetEntity(int.Parse(message.Split(' ')[1])), message.Split(' ')[2]);
            }

            if (message == "pos")
                Log.Info(player.Origin.ToString());
            return base.OnSay2(player, name, message);
        }

        private void GiveKillstreak(Entity self, string streak)
        {
            switch (streak)
            {
                case "helicopter":
                    self.Call("SetActionSlot", 4, "weapon", GetKillstreakWeapon("helicopter"));
                    self.GiveWeapon(GetKillstreakWeapon("helicopter"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("helicopter"));

                    Notify("ShowStreakHUD", self, "Emergency airdrop", GetKillstreakDpadIcon("helicopter"), "First Point Streak!", "achieve_emergairdrop", "mp_killstreak_osprey");

                    self.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["airdrop_assault"]);
                    self.Call("SetPlayerData", "killstreaksState", "nextIndex", 1);
                    break;
                case "helicopter_flares":
                    self.Call("SetActionSlot", 4, "weapon", "killstreak_remote_turret_laptop_mp");
                    self.GiveWeapon("killstreak_remote_turret_laptop_mp");

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("helicopter_flares"));

                    Notify("ShowStreakHUD", self, "Remote PaveLow",
                        GetKillstreakDpadIcon("helicopter_flares"),
                        "PaveLow",
                        "achieve_pavelow",
                        GetKillstreakSound("helicopter_flares"));
                    break;
                case "airdrop_assault":
                    self.Call("SetActionSlot", 5, "weapon", GetKillstreakWeapon("airdrop_assault"));
                    self.GiveWeapon(GetKillstreakWeapon("airdrop_assault"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 1, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 1, GetKillstreakIndex("airdrop_assault"));

                    Notify("ShowStreakHUD", self, "Care Package", GetKillstreakDpadIcon("airdrop_assault"),
                        self.GetField<int>("Kills") + " Point Streak!",
                        "achieve_harriers", GetKillstreakSound("airdrop_assault"));
                    break;
                case "deployable_vest":
                    self.Call("SetActionSlot", 6, "weapon", GetKillstreakWeapon("deployable_vest"));
                    self.GiveWeapon(GetKillstreakWeapon("deployable_vest"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 2, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 2, GetKillstreakIndex("deployable_vest"));

                    Notify("ShowStreakHUD", self, "Ballistic Vest", GetKillstreakDpadIcon("deployable_vest"),
                        self.GetField<int>("Kills") + " Point Streak!",
                        GetKillstreakEarnSound("deployable_vest"),
                        GetKillstreakSound("deployable_vest"));
                    break;
                case "airdrop_juggernaut":
                    self.Call("SetActionSlot", 7, "weapon", GetKillstreakWeapon("airdrop_juggernaut"));
                    self.GiveWeapon(GetKillstreakWeapon("airdrop_juggernaut"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 3, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 3, GetKillstreakIndex("airdrop_juggernaut"));

                    Notify("ShowStreakHUD", self, "Juggernaut",
                        GetKillstreakDpadIcon("airdrop_juggernaut"),
                        self.GetField<int>("Kills") + " Point Streak!",
                        GetKillstreakEarnSound("airdrop_juggernaut"), GetKillstreakSound("airdrop_juggernaut"));
                    break;

                case "ac130":
                    self.Call("SetActionSlot", 4, "weapon", GetKillstreakWeapon("ac130"));
                    self.GiveWeapon(GetKillstreakWeapon("ac130"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("ac130"));

                    Notify("ShowStreakHUD", self, "AC130",
                        GetKillstreakDpadIcon("ac130"),
                        self.GetField<int>("Kills") + " Point Streak!",
                        GetKillstreakEarnSound("ac130"), GetKillstreakSound("ac130"));
                    break;

                case "predator_missile":
                    self.Call("SetActionSlot", 4, "weapon", GetKillstreakWeapon("predator_missile"));
                    self.GiveWeapon(GetKillstreakWeapon("predator_missile"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 0, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 0, GetKillstreakIndex("predator_missile"));

                    Notify("ShowStreakHUD", self, "Predator missile",
                        GetKillstreakDpadIcon("predator_missile"),
                        self.GetField<int>("Kills") + " Point Streak!",
                        GetKillstreakEarnSound("predator_missile"), GetKillstreakSound("predator_missile"));
                    break;
            }
        }
    }
}
