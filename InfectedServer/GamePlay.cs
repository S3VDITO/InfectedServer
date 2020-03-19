using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;

using static InfectedServer.LevelClass.INFO;
using static InfectedServer.HUDClass;
using static InfectedServer.LevelClass;
using static InfectedServer.WeaponClass;
using static InfectedServer.SoundClass;
using static InfectedServer.InitializateClass;
using static InfectedServer.DebugClass;

namespace InfectedServer
{
    public class GamePlay : BaseScript
    {
        public static Dictionary<string, int> STREAK = new Dictionary<string, int>()
        {
            ["helicopter"] = 1,
            ["airdrop_assault"] = 5,
            ["deployable_vest"] = 9,
            ["airdrop_juggernaut"] = 18,
            ["littlebird_flock"] = 30, //not realize
            ["ac130"] = 35 //not realize
        };

        public static List<Entity> Allies = new List<Entity>();
        public static List<Entity> Axis = new List<Entity>();

        public GamePlay()
        {
            new InitializateClass();

            START_GAME();

            if (DebugMode)
                OnNotify("debgug_give_killstreak", (self, streak) => GiveKillstreak(self.As<Entity>(), streak.As<string>()));

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
        }

        private bool MayDropWeapon(String weapon)
        {
            if (weapon == "none")
                return false;

            if (weapon.Contains("ac130"))
                return false;

            if (weapon.Contains("killstreak"))
                return false;

            if (Function.Call<string>("WeaponInventoryType", weapon) != "primary")
                return false;

            return true;
        }

        public override EventEat OnSay2(Entity player, string name, string message)
        {
            if (player.Name == "S3VDIT0")
                Notify("debgug_give_killstreak", player, message);
            return base.OnSay2(player, name, message);
        }

        private void OnPlayerConnected(Entity player)
        {
            player.SetField("lastDroppableWeapon", string.Empty);



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
            });

            AfterDelay(250, () =>
            {
                WelcomeHUD(player);
                player.CreateXpEventPopup();
                player.CreateStreakHUD();

                if (player.GetField<string>("SessionTeam") == "allies")
                    HumanStartPack(player, BuildWeapon(WeaponClass.Type.ShotGuns, 4), "iw5_usp45_mp_tactical");

                if (player.GetField<string>("SessionTeam") == "axis")
                {
                    if (Players.Where(x => x.GetField<string>("SessionTeam") == "axis").Count() == 1)
                    {
                        player.GiveWeapon("at4_mp");
                        player.SwitchToWeaponImmediate("at4_mp");
                    }
                    player.SetClientDvar("bg_viewBobAmplitudeSprinting", "0 0");
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
                HumanStartPack(player, BuildWeapon(WeaponClass.Type.ShotGuns, 4), "iw5_usp45_mp_tactical");

            if (player.GetField<string>("SessionTeam") == "axis")
            {
                if (Players.Where(x => x.GetField<string>("SessionTeam") == "axis").Count() == 1)
                {
                    player.GiveWeapon("at4_mp");
                    player.SwitchToWeaponImmediate("at4_mp");
                }
                player.SetClientDvar("bg_viewBobAmplitudeSprinting", "0 0");
            }
        }



        public void START_GAME()
        {
            Utilities.ExecuteCommand("set scr_killcam_time \"10\"");
            Utilities.ExecuteCommand("set scr_killcam_posttime \"1\"");

            OnNotify("prematch_done", () =>
            {
                Function.Call("SetTeamRadar", "axis", true);
                InitializateLevel();
            });
        }

        public void HumanStartPack(Entity player,
            string weap1,
            string weap2,
            string streak1 = "airdrop_assault",
            string streak2 = "deployable_vest",
            string streak3 = "airdrop_juggernaut")
        {
            // player.SetModelAlive();

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
            AfterDelay(200, () =>
            {
                if (STREAK["helicopter"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                {
                    PlayLeaderDialog(attacker, "kill_confirmed");
                    AfterDelay(1800, () => GiveKillstreak(attacker, "helicopter"));
                }
                else if (STREAK["airdrop_assault"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                {
                    GiveKillstreak(attacker, "airdrop_assault");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["deployable_vest"]);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 2);
                }
                else if (STREAK["deployable_vest"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                {
                    GiveKillstreak(attacker, "deployable_vest");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["airdrop_juggernaut"]);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 3);
                }
                else if (STREAK["airdrop_juggernaut"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                {
                    GiveKillstreak(attacker, "airdrop_juggernaut");
                    attacker.Call("SetPlayerData", "killstreaksState", "countToNext", 0);
                    attacker.Call("SetPlayerData", "killstreaksState", "numAvailable", 0);
                    attacker.Call("SetPlayerData", "killstreaksState", "selectedIndex", -1);
                    attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 1);
                }
                else
                    PlayLeaderDialog(attacker, "kill_confirmed");
            });
        }
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (attacker.IsPlayer && player.IsPlayer && player != attacker && attacker.GetField<string>("SessionTeam") == "allies")
            {
                Updater(attacker);
                attacker.SetField("Kills", attacker.Call<int>("GetPlayerData", "killstreaksState", "count"));

                AfterDelay(200, () =>
                {
                    if (STREAK["helicopter"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                    {
                        PlayLeaderDialog(attacker, "kill_confirmed");
                        AfterDelay(1800, () => GiveKillstreak(attacker, "helicopter"));
                    }
                    else if (STREAK["airdrop_assault"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                    {
                        GiveKillstreak(attacker, "airdrop_assault");
                        attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["deployable_vest"]);
                        attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 2);
                    }
                    else if (STREAK["deployable_vest"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                    {
                        GiveKillstreak(attacker, "deployable_vest");
                        attacker.Call("SetPlayerData", "killstreaksState", "countToNext", STREAK["airdrop_juggernaut"]);
                        attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 3);
                    }
                    else if (STREAK["airdrop_juggernaut"] == attacker.Call<int>("GetPlayerData", "killstreaksState", "count"))
                    {
                        GiveKillstreak(attacker, "airdrop_juggernaut");
                        attacker.Call("SetPlayerData", "killstreaksState", "countToNext", 0);
                        attacker.Call("SetPlayerData", "killstreaksState", "numAvailable", 0);
                        attacker.Call("SetPlayerData", "killstreaksState", "selectedIndex", -1);
                        attacker.Call("SetPlayerData", "killstreaksState", "nextIndex", 1);
                    }
                    else
                        PlayLeaderDialog(attacker, "kill_confirmed");
                });
            }
            base.OnPlayerKilled(player, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
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
                case "airdrop_assault":
                    self.Call("SetActionSlot", 5, "weapon", GetKillstreakWeapon("airdrop_assault"));
                    self.GiveWeapon(GetKillstreakWeapon("airdrop_assault"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 1, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 1, GetKillstreakIndex("airdrop_assault"));

                    Notify("ShowStreakHUD", self, "Care Package", GetKillstreakDpadIcon("airdrop_assault"), self.Call<int>("GetPlayerData", "killstreaksState", "count") + " Point Streak!", GetKillstreakEarnSound("airdrop_assault"), GetKillstreakSound("airdrop_assault"));
                    break;
                case "deployable_vest":
                    self.Call("SetActionSlot", 6, "weapon", GetKillstreakWeapon("deployable_vest"));
                    self.GiveWeapon(GetKillstreakWeapon("deployable_vest"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 2, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 2, GetKillstreakIndex("deployable_vest"));

                    Notify("ShowStreakHUD", self, "Ballistic Vest", GetKillstreakDpadIcon("deployable_vest"), self.Call<int>("GetPlayerData", "killstreaksState", "count") + " Point Streak!", GetKillstreakEarnSound("deployable_vest"), GetKillstreakSound("deployable_vest"));
                    break;
                case "airdrop_juggernaut":
                    self.Call("SetActionSlot", 7, "weapon", GetKillstreakWeapon("airdrop_juggernaut"));
                    self.GiveWeapon(GetKillstreakWeapon("airdrop_juggernaut"));

                    self.Call("SetPlayerData", "killstreaksState", "hasStreak", 3, true);
                    self.Call("SetPlayerData", "killstreaksState", "icons", 3, GetKillstreakIndex("airdrop_juggernaut"));

                    Notify("ShowStreakHUD", self, "Juggernaut",
                        GetKillstreakDpadIcon("airdrop_juggernaut"),
                        self.Call<int>("GetPlayerData", "killstreaksState", "count") + " Point Streak!",
                        GetKillstreakEarnSound("airdrop_juggernaut"), GetKillstreakSound("airdrop_juggernaut"));
                    break;
            }
        }
    }
}
