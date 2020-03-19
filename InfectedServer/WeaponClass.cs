using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace InfectedServer
{
    public static class WeaponClass
    {
        /// <summary>
        /// List all assault rifle for Humans
        /// </summary>
        public static List<String> AssaultRifles = new List<String>()
        {
           "iw5_ak47_mp_reflex_xmags",
           "iw5_m4_mp_reflex_xmags",
           "iw5_cm901_mp_reflex_xmags",
           "iw5_g36c_mp_reflex_xmags",
           "iw5_scar_mp_reflex_xmags",
           "iw5_fad_mp_reflex_xmags",
           "iw5_acr_mp_reflex_xmags",
           "iw5_ak47_mp_acog_xmags",
           "iw5_m4_mp_acog_xmags",
           "iw5_cm901_mp_acog_xmags",
           "iw5_g36c_mp_acog_xmags",
           "iw5_scar_mp_acog_xmags",
           "iw5_fad_mp_acog_xmags",
           "iw5_acr_mp_acog_xmags",
           "iw5_ak47_mp_eotech_xmags",
           "iw5_m4_mp_eotech_xmags",
           "iw5_cm901_mp_eotech_xmags",
           "iw5_g36c_mp_eotech_xmags",
           "iw5_scar_mp_eotech_xmags",
           "iw5_fad_mp_eotech_xmags",
           "iw5_acr_mp_eotech_xmags"
        };

        /// <summary>
        /// List all smg for Humans
        /// </summary>
        public static List<String> SMG = new List<String>()
        {
           "iw5_mp5_mp_xmags",
           "iw5_ump45_mp_xmags",
           "iw5_p90_mp_rof",
           "iw5_mp7_mp_rof",
           "iw5_pp90m1_mp",
           "iw5_m9_mp_rof_xmags",
           "iw5_skorpion_mp_xmags",
           "iw5_fmg9_mp",
           "iw5_mp9_mp"
        };

        /// <summary>
        /// List all LMG for Humans
        /// </summary>
        public static List<String> LMG = new List<String>()
        {
            "iw5_sa80_mp_thermal_heartbeat_xmags",
            "iw5_mg36_mp_thermal_heartbeat_xmags",
            "iw5_pecheneg_mp_thermal_heartbeat_xmags",
            "iw5_mk46_mp_thermal_heartbeat_xmags"
            //"iw5_m60_mp_thermal_heartbeat_xmags"
        };

        /// <summary>
        /// List all shotguns for Humans
        /// </summary>
        public static List<String> ShotGuns = new List<String>()
        {
            "iw5_spas12_mp",
            "iw5_striker_mp",
            "iw5_1887_mp",
            "iw5_ksg_mp"
        };

        /// <summary>
        /// List all Sniper Rifles for Humans
        /// </summary>
        public static List<String> SniperRifles = new List<String>()
        {
            "iw5_msr_mp_xmags_msrscope",
            "iw5_l96a1_mp_xmags_l96a1scope",
            "iw5_dragunov_mp_xmags_dragunovscope",
           // "iw5_type95_mp_reflex_xmags",            
            //"iw5_mk14_mp_reflex_xmags",
            //"iw5_type95_mp_acog_xmags",          
            "iw5_mk14_mp_acog_xmags"
            //"iw5_type95_mp_eotech_xmags",         
            //"iw5_mk14_mp_eotech_xmags"
        };

        /// <summary>
        /// Build random weapon
        /// </summary>
        /// <param name="itemType">Type weapon</param>
        /// <param range="0-11" name="camo">Num camo [0 - 11]</param>
        /// <returns>Weapon name</returns>
        public static string BuildWeapon(Type itemType, int camo)
        {
            switch (itemType)
            {
                case Type.AssaultRifle:
                    return AssaultRifles[Function.Call<int>("RandomInt", AssaultRifles.Count)] + BuildCamo(camo);
                case Type.LMG:
                    return LMG[Function.Call<int>("RandomInt", LMG.Count)] + BuildCamo(camo);
                case Type.SMG:
                    return SMG[Function.Call<int>("RandomInt", SMG.Count)] + BuildCamo(camo);
                case Type.ShotGuns:
                    return ShotGuns[Function.Call<int>("RandomInt", ShotGuns.Count)] + BuildCamo(camo);
                case Type.SniperRifle:
                    return SniperRifles[Function.Call<int>("RandomInt", SniperRifles.Count)] + BuildCamo(camo);
            }
            return null;
        }

        /// <summary>
        /// Weapons types
        /// </summary>
        public enum Type
        {
            SMG,
            LMG,
            AssaultRifle,
            SniperRifle,
            ShotGuns
        }

        /// <summary>
        /// Bulding camo for weapon
        /// </summary>
        /// <param name="camo">Num camo [0 - 11]</param>
        /// <returns>camo string, example: _camo00</returns>
        public static string BuildCamo(int camo)
        {
            if (camo < 10)
                return "_camo0" + camo.ToString();
            else
                return "_camo" + camo.ToString();
        }
    }
}
