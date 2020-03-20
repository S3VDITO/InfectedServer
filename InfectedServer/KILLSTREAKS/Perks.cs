using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

using static InfectedServer.LevelClass.INFO;

namespace InfectedServer.KILLSTREAKS
{
    public static class Perks
    {
        private static List<string> _Perks = new List<string>()
        {
            "specialty_longersprint","specialty_fastreload","specialty_scavenger","specialty_blindeye","specialty_paint","specialty_hardline","specialty_coldblooded","specialty_quickdraw",
            "specialty_detectexplosive","specialty_autospot","specialty_bulletaccuracy","specialty_quieter","specialty_stalker","specialty_bulletpenetration","specialty_marksman",
            "specialty_holdbreathwhileads","specialty_longerrange","specialty_fastermelee","specialty_reducedsway","specialty_lightweight", "specialty_sharp_focus" /* "specialty_explosivebullets" */
        };

        public static void AllPerksBonus(this Entity self) //Бонус спеца
        {
            foreach (string perk in _Perks)
            {
                self.SetPerk(perk, true, true);
                self.SetPerk(UpgradePerk(perk), true, true);
            }

            self.Call("SetPlayerData", "killstreaksState", "hasStreak", 1, true);
            self.Call("SetPlayerData", "killstreaksState", "icons", 1, GetKillstreakIndex("specialty_autospot_ks_pro"));

            self.Call("SetPlayerData", "killstreaksState", "hasStreak", 2, true);
            self.Call("SetPlayerData", "killstreaksState", "icons", 2, GetKillstreakIndex("_specialty_blastshield_ks_pro"));

            self.Call("SetPlayerData", "killstreaksState", "hasStreak", 3, true);
            self.Call("SetPlayerData", "killstreaksState", "icons", 3, GetKillstreakIndex("specialty_scavenger_ks_pro"));

            self.Call("SetPlayerData", "killstreaksState", "hasStreak", 4, true);
            self.Call("SetPlayerData", "killstreaksState", "isSpecialist", true);

            self.Notify("changed_kit"); // It is not working, detecExplosive =(
        }

        public static void RemoveKillstreak(this Entity self)//Удаляем киллтсрики
        {
            self.TakeWeapon(GetKillstreakWeapon("airdrop_assault"));

            self.TakeWeapon(GetKillstreakWeapon("deployable_vest"));

            self.TakeWeapon(GetKillstreakWeapon("airdrop_juggernaut"));

            self.Call("SetPlayerData", "killstreaksState", "hasStreak", 4, false);
            self.Call("SetPlayerData", "killstreaksState", "isSpecialist", false);
        }

        public static string UpgradePerk(this String perk)
        { return Function.Call<string>("TableLookup", "mp/perktable.csv", 1, perk, 8); }
    }
}
