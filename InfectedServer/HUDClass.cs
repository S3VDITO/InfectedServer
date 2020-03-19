using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;
using System.Text;
using static InfectedServer.LevelClass.INFO;
using static InfectedServer.SoundClass;
using System.Timers;

namespace InfectedServer
{
    public static class HUDClass
    {
        public static HudElem CreateTemplateOverlay(this Entity self, String shader = "")
        {
            HudElem overlay = HudElem.NewClientHudElem(self);
            overlay.X = 0;
            overlay.Y = 0;
            overlay.AlignX = "Left";
            overlay.AlignY = "Top";
            overlay.HorzAlign = "Fullscreen";
            overlay.VertAlign = "Fullscreen";
            overlay.SetShader(shader, 640, 480);
            overlay.Sort = -10;
            overlay.Alpha = 1;
            return overlay;
        }
        public static void GlobalHeaderText()
        {
            HudElem globalText = HudElem.CreateServerFontString("HudBig", 0.4f);
            globalText.SetPoint("top right", "top right", -3, 1);
            globalText.SetText(Function.Call<string>("GetDvar", "sv_hostname"));
            globalText.HideWhenInMenu = false;
            globalText.Archived = false;
        }

        public static void CreateStreakHUD(this Entity self)
        {
            if (self.HasField("streaknameHUD") ||
                self.HasField("streakiconHUD") ||
                self.HasField("pointstreakHUD"))
                return;

            HudElem StreakIcon = HudElem.CreateIcon(self, "", 40, 40);
            StreakIcon.SetPoint("center top", "center top", 0, 40);
            StreakIcon.HideWhenInMenu = false;
            StreakIcon.Archived = false;
            StreakIcon.Alpha = 0;

            HudElem StreakName = HudElem.CreateFontString(self, "HudBig", 0.7f);
            StreakName.SetPoint("center top", "center top", 0, 85);
            StreakName.HideWhenInMenu = false;
            StreakName.GlowAlpha = 1;
            StreakName.Archived = false;
            StreakName.GlowColor = new Vector3(0, 1, 0);
            StreakName.Alpha = 0;

            HudElem PointStreak = HudElem.CreateFontString(self, "HudSmall", 1.2f);
            PointStreak.SetPoint("center top", "center top", 0, 105);
            PointStreak.HideWhenInMenu = false;
            PointStreak.GlowAlpha = 1;
            PointStreak.Archived = false;
            PointStreak.GlowColor = new Vector3(0, 0, 0);
            PointStreak.Alpha = 0;


            self.SetField("streaknameHUD", new Parameter(StreakName));
            self.SetField("streakiconHUD", new Parameter(StreakIcon));
            self.SetField("pointstreakHUD", new Parameter(PointStreak));
        }

        public static Dictionary<string, HudElem> CreateBar(this Entity self, byte width = 120, byte height = 9)
        {
            HudElem backGround = HudElem.CreateIcon(self, "black", width, height);
            backGround.SetPoint("center left", "center", -61, 0);
            backGround.Sort = -3;
            backGround.Alpha = 0;

            HudElem whiteProgres = HudElem.CreateIcon(self, "progress_bar_bg", 1, height - 3);
            whiteProgres.SetPoint("center left", "center", -60, 0);
            whiteProgres.Sort = -2;
            whiteProgres.Color = new Vector3(1, 1, 1);
            whiteProgres.Alpha = 0;

            HudElem textCapturing = HudElem.CreateFontString(self, "HudSmall", 0.8f);
            textCapturing.SetPoint("center", "center", 0, -12);
            textCapturing.SetText("Capturing...");
            textCapturing.Sort = -1;
            textCapturing.Color = new Vector3(1, 1, 1);
            textCapturing.Alpha = 0;

            return new Dictionary<string, HudElem>()
            {
                {"BackGround", backGround},
                {"Progress", whiteProgres},
                {"Text", textCapturing}
            };
        }

        public static void UpdateBarScale(this Entity self, HudElem bar, float duration, bool reset = true)
        {
            switch (reset)
            {
                case true:
                    bar.SetShader("white", 1, 6);
                    goto case false;
                case false:
                    bar.Call("ScaleOverTime", duration, 117, bar.Height);
                    break;
            }
        }
        public static void TeamPlayerCardSplash(this Entity owner, string splash, string killstreak)
        {
            for (int i = 0; i < 18; i++)
            {
                Entity.GetEntity(i).Call("SetCardDisplaySlot", owner, 5);
                Entity.GetEntity(i).Call("ShowHudSplash", splash, 1);
            }
            PlayLeaderDialogForPlayers(owner, GetKillstreakFriendlySound(killstreak), GetKillstreakEnemySound(killstreak));
        }
        public static HudElem HeadIcon(Vector3 pos, Entity self, string shader, byte width = 15, byte height = 15)
        {
            HudElem _headicon = HudElem.NewClientHudElem(self);
            _headicon.X = pos.X;
            _headicon.Y = pos.Y;
            _headicon.Z = pos.Z + 35;
            _headicon.Alpha = 1;
            _headicon.SetShader(shader, width, height);
            _headicon.Call("SetWaypoint", true, true);
            return _headicon;
        }



        public static void CreateXpEventPopup(this Entity self)
        {
            HudElem hud_xpEventPopup = HudElem.NewClientHudElem(self);
            hud_xpEventPopup.Children = new List<HudElem>();
            hud_xpEventPopup.HorzAlign = "Right";
            hud_xpEventPopup.VertAlign = "Bottom";
            hud_xpEventPopup.AlignX = "Right";
            hud_xpEventPopup.AlignY ="Bottom";
            hud_xpEventPopup.X = -20;
            hud_xpEventPopup.Y = -15;
            hud_xpEventPopup.Font = "HudBig";
            hud_xpEventPopup.FontScale = 0.55f;
            hud_xpEventPopup.Archived = false;
            hud_xpEventPopup.Color = new Vector3(0.5f, 0.5f, 0.5f);
            hud_xpEventPopup.Sort = 10000;
            self.SetField("hud_xpEventPopup", new Parameter(hud_xpEventPopup));
        }
    }
}
