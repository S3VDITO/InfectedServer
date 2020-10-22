using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfectedServer
{
    internal static class PlayerSuits
    {
        public static readonly Dictionary<string, Dictionary<string, string[]>> PlayerModels = new Dictionary<string, Dictionary<string, string[]>>()
        {
            #region Ghille model
            ["desert"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_desert" },
                ["body"] = new string[] { "mp_body_ally_ghillie_desert_sniper" },
            },
            ["arctic"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_arctic" },
                ["body"] = new string[] { "mp_body_ally_ghillie_desert_sniper" },
            },
            ["urban"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_urban" },
                ["body"] = new string[] { "mp_body_ally_ghillie_urban_sniper" },
            },
            ["forest"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_woodland" },
                ["body"] = new string[] { "mp_body_ally_ghillie_woodland_sniper" },
            },
            ["forest_militia"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_woodland" },
                ["body"] = new string[] { "mp_body_ally_ghillie_woodland_sniper" },
            },
            ["desert_militia"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_desert" },
                ["body"] = new string[] { "mp_body_ally_ghillie_desert_sniper" },
            },
            ["arctic_militia"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_arctic" },
                ["body"] = new string[] { "mp_body_ally_ghillie_desert_sniper" },
            },
            ["urban_militia"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_iw5_ghillie_urban" },
                ["body"] = new string[] { "mp_body_ally_ghillie_urban_sniper" },
            },
            #endregion

            #region Allies team model
            ["delta_multicam"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_delta" },
                ["head"] = new string[]
                {
                    "head_delta_elite_a",
                    "head_delta_elite_b",
                    "head_delta_elite_c",
                    "head_delta_elite_d"
                },
                ["body"] = new string[]
                {
                    "mp_body_delta_elite_assault_ab",
                    "mp_body_delta_elite_assault_ba",
                    "mp_body_delta_elite_assault_bb",
                    "mp_body_delta_elite_lmg_a",
                    "mp_body_delta_elite_lmg_b",
                    "mp_body_delta_elite_smg_a",
                    "mp_body_delta_elite_smg_b",
                    "mp_body_delta_elite_shotgun_a",
                    "mp_body_ally_delta_sniper"
                }
            },
            ["sas_urban"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_sas" },
                ["head"] = new string[]
                {
                    "head_sas_a",
                    "head_sas_b",
                    "head_sas_c",
                    "head_ally_sas_sniper"
                },
                ["body"] = new string[]
                {
                    "mp_body_sas_urban_assault",
                    "mp_body_sas_urban_lmg",
                    "mp_body_sas_urban_shotgun",
                    "mp_body_sas_urban_smg",
                    "mp_body_ally_sas_sniper"
                }
            },
            ["pmc_africa"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_pmc" },
                ["head"] = new string[]
                {
                    "head_pmc_africa_a",
                    "head_pmc_africa_b",
                    "head_pmc_africa_c",
                    "head_pmc_africa_cc",
                    "head_pmc_africa_d",
                    "head_pmc_africa_dd",
                    "head_pmc_africa_e",
                    "head_pmc_africa_f",
                    "head_pmc_africa_ff"
                },
                ["body"] = new string[]
                {
                    "mp_body_pmc_africa_assault_aa",
                    "mp_body_pmc_africa_lmg_a",
                    "mp_body_pmc_africa_lmg_aa",
                    "mp_body_pmc_africa_smg_a",
                    "mp_body_pmc_africa_smg_aa",
                    "mp_body_pmc_africa_shotgun_a",
                    "mp_body_ally_pmc_sniper"
                }
            },
            ["gign_paris"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_sas" },
                ["head"] = new string[]
                {
                    "head_gign_a",
                    "head_gign_b",
                    "head_gign_c",
                    "head_gign_d",
                    "head_gign_saber_gasmask"
                },
                ["body"] = new string[]
                {
                    "mp_body_gign_paris_assault",
                    "mp_body_gign_paris_lmg",
                    "mp_body_gign_paris_shotgun",
                    "mp_body_gign_paris_smg",
                    "mp_body_gign_paris_smg"
                }
            },
            #endregion

            #region Axis team model
            ["opforce_air"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_russian_b" },
                ["head"] = new string[]
                {
                    "head_russian_military_bb",
                    "head_russian_military_d",
                    "head_russian_military_aa"
                },
                ["body"] = new string[]
                {
                    "mp_body_russian_military_assault_a_airborne",
                    "mp_body_russian_military_lmg_a_airborne",
                    "mp_body_russian_military_shotgun_a_airborne",
                    "mp_body_russian_military_smg_a_airborne"
                }
            },
            ["opforce_snow"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_russian_d" },
                ["head"] = new string[]
                {
                    "head_russian_military_aa_arctic",
                    "head_russian_military_b_arctic",
                    "head_russian_military_dd_arctic",
                    "head_russian_military_d_arctic"
                },
                ["body"] = new string[]
                {
                    "mp_body_russian_military_lmg_a_arctic",
                    "mp_body_russian_military_shotgun_a_arctic",
                    "mp_body_russian_military_smg_a_arctic",
                    "mp_body_russian_military_assault_a_arctic"
                }
            },
            ["opforce_urban"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_russian_a" },
                ["head"] = new string[]
                {
                    "head_russian_military_aa",
                    "head_russian_military_b",
                    "head_russian_military_dd"
                },
                ["body"] = new string[]
                {
                    "mp_body_russian_military_assault_a",
                    "mp_body_russian_military_lmg_a",
                    "mp_body_russian_military_shotgun_a",
                    "mp_body_russian_military_smg_a",
                    "mp_body_opforce_russian_urban_sniper"
                }
            },
            ["opforce_woodland"] = new Dictionary<string, string[]>()
            {
                ["hand"] = new string[] { "viewhands_russian_c" },
                ["head"] = new string[]
                {
                    "head_russian_military_cc",
                    "head_russian_military_a",
                    "head_russian_military_b"
                },
                ["body"] = new string[]
                {
                    "mp_body_russian_military_assault_a_woodland",
                    "mp_body_russian_military_lmg_a_woodland",
                    "mp_body_russian_military_shotgun_a_woodland",
                    "mp_body_russian_military_smg_a_woodland",
                    "mp_body_opforce_russian_woodland_sniper"
                }
            },
            #endregion
        };
    }
}
