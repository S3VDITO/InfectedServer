using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfectedServer
{
    public static class RandomWeapon
    {
        private static Random random = new Random();
        private static readonly Dictionary<RifleType, List<string>> Weapons = new Dictionary<RifleType, List<string>>()
        {
            [RifleType.AssaultRifle] = new List<string>()
            {
                "iw5_acr",
                "iw5_type95",
                "iw5_m4",
                "iw5_ak47",
                "iw5_m16",
                "iw5_mk14",
                "iw5_g36c",
                "iw5_scar",
                "iw5_fad",
                "iw5_cm901"
            },
            [RifleType.SMG] = new List<string>()
            {
                "iw5_mp5",
                "iw5_p90",
                "iw5_m9",
                "iw5_pp90m1",
                "iw5_ump45",
                "iw5_mp7"
            },
            [RifleType.ShotGun] = new List<string>()
            {
                "iw5_spas12",
                "iw5_aa12",
                "iw5_striker",
                "iw5_1887",
                "iw5_usas12",
                "iw5_ksg"
            },
            [RifleType.LMG] = new List<string>()
            {
                "iw5_m60",
                "iw5_mk46",
                "iw5_pecheneg",
                "iw5_sa80",
                "iw5_mg36"
            },
            [RifleType.SniperRifle] = new List<string>()
            {
                "iw5_barrett",
                "iw5_msr",
                "iw5_rsass",
                "iw5_dragunov",
                "iw5_as50",
                "iw5_l96a1"
            },
            [RifleType.Pistol] = new List<string>()
            {
                "iw5_usp45",
                "iw5_mp412",
                "iw5_44magnum",
                "iw5_deserteagle",
                "iw5_p99",
                "iw5_fnfiveseven"
            },
            [RifleType.MachinePistol] = new List<string>()
            {
                "iw5_fmg9",
                "iw5_g18",
                "iw5_mp9",
                "iw5_skorpion"
            },
        };
        private static readonly Dictionary<string, List<string>> Attachments = new Dictionary<string, List<string>>()
        {
            ["iw5_acr"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_type95"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "rof",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_m4"] = new List<string>()
            {
                "reflex",
                "silencer",
                "gl",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_ak47"] = new List<string>()
            {
                "reflex",
                "silencer",
                "gp25",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_m16"] = new List<string>()
            {
                "reflex",
                "silencer",
                "gl",
                "acog",
                "rof",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_mk14"] = new List<string>()
            {
                "reflex",
                "silencer",
                "gl",
                "acog",
                "rof",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal",
                "rof"
            },
            ["iw5_g36c"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_scar"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_fad"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_cm901"] = new List<string>()
            {
                "reflex",
                "silencer",
                "m320",
                "acog",
                "heartbeat",
                "eotech",
                "shotgun",
                "hybrid",
                "xmags",
                "thermal"
            },
            ["iw5_mp5"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_p90"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_m9"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_pp90m1"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_ump45"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_mp7"] = new List<string>()
            {
                "reflexsmg",
                "silencer",
                "rof",
                "acogsmg",
                "eotechsmg",
                "hamrhybrid",
                "xmags",
                "thermalsmg"
            },
            ["iw5_spas12"] = new List<string>()
            {
                "grip",
                "reflex",
                "eotech",
                "xmags",
                "silencer03"
            },
            ["iw5_aa12"] = new List<string>()
            {
                "grip",
                "reflex",
                "eotech",
                "xmags",
                "silencer03"
            },
            ["iw5_striker"] = new List<string>()
            {
                "grip",
                "reflex",
                "eotech",
                "xmags",
                "silencer03"
            },
            ["iw5_1887"] = new List<string>(),
            ["iw5_usas12"] = new List<string>()
            {
                "grip",
                "reflex",
                "eotech",
                "xmags",
                "silencer03"
            },
            ["iw5_ksg"] = new List<string>()
            {
                "grip",
                "reflex",
                "eotech",
                "xmags",
                "silencer03"
            },
            ["iw5_m60"] = new List<string>()
            {
                "reflexlmg",
                "silencer",
                "grip",
                "acog",
                "rof",
                "eotechlmg",
                "xmags",
                "thermal"
            },
            ["iw5_mk46"] = new List<string>()
            {
                "reflexlmg",
                "silencer",
                "grip",
                "acog",
                "rof",
                "heartbeat",
                "eotechlmg",
                "xmags",
                "thermal"
            },
            ["iw5_pecheneg"] = new List<string>()
            {
                "reflexlmg",
                "silencer",
                "grip",
                "acog",
                "rof",
                "eotechlmg",
                "xmags",
                "thermal"
            },
            ["iw5_sa80"] = new List<string>()
            {
                "reflexlmg",
                "silencer",
                "grip",
                "acog",
                "rof",
                "heartbeat",
                "eotechlmg",
                "xmags",
                "thermal"
            },
            ["iw5_mg36"] = new List<string>()
            {
                "reflexlmg",
                "silencer",
                "grip",
                "acog",
                "rof",
                "heartbeat",
                "eotechlmg",
                "xmags",
                "thermal"
            },
            ["iw5_barrett"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "barrettscopevz",
                "silencer03"
            },
            ["iw5_msr"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "msrscopevz",
                "silencer03"
            },
            ["iw5_rsass"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "rsassscopevz",
                "silencer03"
            },
            ["iw5_dragunov"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "dragunovscopevz",
                "silencer03"
            },
            ["iw5_as50"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "as50scopevz",
                "silencer03"
            },
            ["iw5_l96a1"] = new List<string>()
            {
                "acog",
                "heartbeat",
                "xmags",
                "thermal",
                "l96a1scopevz",
                "silencer03"
            },
            ["iw5_usp45"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "tactical",
                "xmags"
            },
            ["iw5_mp412"] = new List<string>()
            {
                "akimbo",
                "tactical"
            },
            ["iw5_44magnum"] = new List<string>()
            {
                "akimbo",
                "tactical"
            },
            ["iw5_deserteagle"] = new List<string>()
            {
                "akimbo",
                "tactical"
            },
            ["iw5_p99"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "tactical",
                "xmags"
            },
            ["iw5_fnfiveseven"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "tactical",
                "xmags"

            },
            ["iw5_fmg9"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "reflexsmg",
                "xmags"
            },
            ["iw5_g18"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "reflexsmg",
                "xmags"
            },
            ["iw5_mp9"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "reflexsmg",
                "xmags"
            },
            ["iw5_skorpion"] = new List<string>()
            {
                "silencer02",
                "akimbo",
                "reflexsmg",
                "xmags"
            }
        };
        private static readonly Dictionary<string, string> DefaultSniperScopes = new Dictionary<string, string>()
        {
            ["iw5_barrett"] = "barrettscope",
            ["iw5_as50"] = "as50scope",
            ["iw5_l96a1"] = "l96a1scope",
            ["iw5_msr"] = "msrscope",
            ["iw5_dragunov"] = "dragunovscope",
            ["iw5_rsass"] = "rsassscope"
        };
        private static readonly Dictionary<string, List<string>> AttachmentExclusions = new Dictionary<string, List<string>>()
        {
            ["reflex"] = new List<string>()
            {
                "reflex",
                "acog",
                "thermal",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["reflexsmg"] = new List<string>()
            {
                "reflexsmg",
                "acogsmg",
                "thermalsmg",
                "eotechsmg",
                "vzscope",
                "hamrhybrid",
                "hybrid",
                "akimbo"
            },
            ["reflexlmg"] = new List<string>()
            {
                "reflexlmg",
                "acog",
                "thermal",
                "eotechlmg",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["silencer"] = new List<string>()
            {
                "silencer",
                "silencer02",
                "silencer03",
                "akimbo"
            },
            ["silencer02"] = new List<string>()
            {
                "silencer",
                "silencer02",
                "silencer03",
                "akimbo"
            },
            ["silencer03"] = new List<string>()
            {
                "silencer",
                "silencer02",
                "silencer03akimbo"
            },
            ["acog"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid",
                "akimbo"
            },
            ["acogsmg"] = new List<string>()
            {
                "acogsmg",
                "reflexsmg",
                "thermalsmg",
                "eotechsmg",
                "vzscope",
                "hamrhybrid",
                "hybrid",
                "akimbo"
            },
            ["grip"] = new List<string>()
            {
                "grip"
            },
            ["akimbo"] = new List<string>()
            {
                "akimbo",
                "acog",
                "acogsmg",
                "reflex",
                "reflexsmg",
                "thermal",
                "thermalsmg",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid",
                "tactical",
                "silencer",
                "silencer02",
                "silencer03"
            },
            ["thermal"] = new List<string>()
            {
                "akimbo",
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["thermalsmg"] = new List<string>()
            {
                "akimbo",
                "acogsmg",
                "reflexsmg",
                "thermalsmg",
                "eotechsmg",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["shotgun"] = new List<string>()
            {
                "akimbo",
                "gl",
                "gp25",
                "m320",
                "shotgun"
            },
            ["heartbeat"] = new List<string>()
            {
                "heartbeat"
            },
            ["xmags"] = new List<string>()
            {
                "xmags"
            },
            ["rof"] = new List<string>()
            {
                "rof"
            },
            ["eotech"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["eotechsmg"] = new List<string>()
            {
                "acogsmg",
                "reflexsmg",
                "thermalsmg",
                "eotechsmg",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["eotechlmg"] = new List<string>()
            {
                "acog",
                "reflexlmg",
                "thermal",
                "eotechlmg",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["tactical"] = new List<string>()
            {
                "tactical",
                "akimbo"
            },
            ["barrettscopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "barrettscopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["as50scopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "as50scopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["l96a1scopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "l96a1scopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["msrscopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "msrscopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["dragunovscopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "dragunovscopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["rsassscopevz"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "rsassscopevz",
                "hamrhybrid",
                "hybrid"
            },
            ["hamrhybrid"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "thermalsmg",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid",
                "reflexsmg",
                "eotechsmg"
            },
            ["hybrid"] = new List<string>()
            {
                "acog",
                "reflex",
                "thermal",
                "eotech",
                "vzscope",
                "hamrhybrid",
                "hybrid"
            },
            ["gl"] = new List<string>()
            {
                "gl",
                "gp25",
                "m320",
                "shotgun"
            },
            ["gp25"] = new List<string>()
            {
                "gl",
                "gp25",
                "m320",
                "shotgun"
            },
            ["m320"] = new List<string>()
            {
                "gl",
                "gp25",
                "m320",
                "shotgun"
            }
        };
        private static readonly List<string> Camouflages = new List<string>()
        {
            "none",
            "camo01",
            "camo02",
            "camo03",
            "camo04",
            "camo05",
            "camo06",
            "camo07",
            "camo08",
            "camo09",
            "camo10",
            "camo11",
            "camo12",
            "camo13"
        };
        private static readonly List<string> SniperSights = new List<string>()
        {
            "barrettscopevz",
            "as50scopevz",
            "l96a1scopevz",
            "msrscopevz",
            "dragunovscopevz",
            "rsassscopevz",
            "acog",
            "thermal"
        };
        public enum RifleType
        {
            SniperRifle,
            AssaultRifle,
            MachinePistol,
            ShotGun,
            SMG,
            LMG,
            Pistol
        }

        public static string GenerateWeapon(RifleType type, bool akimbo = false)
        {
            string WeaponTechName = Weapons[type][random.Next(Weapons[type].Count)];
            string WeaponFullName = WeaponTechName + "_mp" + (akimbo ? "_akimbo" : string.Empty);

            if (Attachments[WeaponTechName].Count != 0)
            {
                string Attachmen_1 = Attachments[WeaponTechName][random.Next(Attachments[WeaponTechName].Count)];
                string Attachmen_2 = Attachments[WeaponTechName][random.Next(Attachments[WeaponTechName].Count)];

                while (AttachmentExclusions[Attachmen_1].Contains(Attachmen_2))
                    Attachmen_2 = Attachments[WeaponTechName][random.Next(Attachments[WeaponTechName].Count)];

                WeaponFullName += "_" + Attachmen_1;
                WeaponFullName += "_" + Attachmen_2;

                if (type == RifleType.SniperRifle && !SniperSights.Contains(Attachmen_1) && !SniperSights.Contains(Attachmen_2))
                    WeaponFullName += "_" + DefaultSniperScopes[WeaponTechName];

                if (!(type == RifleType.Pistol || type == RifleType.MachinePistol))
                {
                    string camo = Camouflages[random.Next(Camouflages.Count)];

                    if (camo != "none")
                        WeaponFullName += "_" + camo;
                }
            }
            return WeaponFullName;
        }
    }
}
