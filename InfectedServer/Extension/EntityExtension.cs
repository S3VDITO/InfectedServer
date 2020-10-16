using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace InfectedServer
{
    public static class EntityExtension
    {
        private static readonly Dictionary<string, string> SecondaryOffhand = new Dictionary<string, string>()
        {
            ["lightstick_mp"] = "flash",
            ["flash_grenade_mp"] = "flash",
            ["concussion_grenade_mp"] = "smoke",
            ["smoke_grenade_mp"] = "smoke",
            ["flare_mp"] = "flash",
            ["trophy_mp"] = "flash",
            ["scrambler_mp"] = "flash",
            ["portable_radar_mp"] = "flash",
            ["emp_grenade_mp"] = "flash"
        };
        private static readonly Dictionary<string, string> PrimaryOffhand = new Dictionary<string, string>()
        {
            ["frag_grenade_mp"] = "frag",
            ["throwingknife_mp"] = "throwingknife",
            ["c4_mp"] = "other",
            ["semtex_mp"] = "other",
            ["claymore_mp"] = "other",
            ["bouncingbetty_mp"] = "other",
        };

        public static void GivePrimaryOffhand(this Entity player, string offhandname)
        {
            if (!PrimaryOffhand.ContainsKey(offhandname))
                throw new Exception("Offhand not found!");

            player.SetOffhandPrimaryClass(PrimaryOffhand[offhandname]);
            player.GiveWeapon(offhandname);
        }

        public static void GiveSecondaryOffhand(this Entity player, string offhandname)
        {
            if (!SecondaryOffhand.ContainsKey(offhandname))
                throw new Exception("Offhand not found!");

            player.SetOffhandSecondaryClass(SecondaryOffhand[offhandname]);
            player.GiveWeapon(offhandname);
        }
    }
}
