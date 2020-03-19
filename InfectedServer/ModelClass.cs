/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace InfectedServer
{
    public static class ModelClass
    {
        
        public static void SetModelAlive(this Entity player)
        {
            switch (Function.Call<string>("GetMapCustom", "axischar"))
            {
                case "opforce_air":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_russian_b");
                    player.Call("SetModel", "mp_body_russian_military_shotgun_a_airborne");
                    player.Call("Attach", "head_russian_military_" + new string[] { "aa", "bb", "d", "d" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    break;

                case "opforce_snow":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_russian_d");
                    player.Call("SetModel", "mp_body_russian_military_shotgun_a_arctic");
                    player.Call("Attach", "head_russian_military_" + new string[] { "aa_arctic", "b_arctic", "dd_arctic", "d_arctic" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    break;

                case "opforce_urban":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_russian_a");
                    player.Call("SetModel", "mp_body_russian_military_shotgun_a");
                    player.Call("Attach", "head_russian_military_" + new string[] { "aa", "b", "dd", "dd" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    break;

                case "opforce_woodland":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_russian_a");
                    player.Call("SetModel", "mp_body_russian_military_shotgun_a_woodland");
                    player.Call("Attach", "head_russian_military_" + new string[] { "a", "b", "cc", "cc" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    break;

                case "opforce_africa":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_african_militia");
                    player.Call("SetModel", "mp_body_africa_militia_smg_b");
                    player.Call("Attach", "head_africa_militia_" + new string[] { "a_mp", "b_mp", "d", "d" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    player.Call("HidePart", "j_helmet");
                    break;

                case "opforce_henchmen":
                    player.Call("DetachAll");
                    player.Call("SetViewModel", "viewhands_henchmen");
                    player.Call("SetModel", "mp_body_henchmen_shotgun_b");
                    player.Call("Attach", "head_henchmen_" + new string[] { "a", "b", "c", "cc" }[Function.Call<int>("RandomIntRange", 0, 3)], "j_eyeball", true);
                    player.Call("HidePart", "j_helmet");
                    break;
            }
        }
        public static void SetModelMarksman(this Entity self)
        {
            switch (Function.Call<string>("GetMapCustom", "axischar"))
            {
                case "opforce_air":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_russian_air_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_russian_military_assault_a_airborne");
                    break;

                case "opforce_snow":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_russian_arctic_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_russian_military_assault_a_arctic");
                    break;

                case "opforce_urban":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_russian_urban_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_russian_military_assault_a");
                    break;

                case "opforce_woodland":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_russian_woodland_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_russian_military_assault_a_woodland");
                    break;

                case "opforce_africa":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_africa_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_africa_militia_lmg_b");
                    break;

                case "opforce_henchmen":
                    self.Call("DetachAll");
                    self.Call("Attach", "head_opforce_henchmen_sniper", "j_eyeball", true);
                    self.Call("SetModel", "mp_body_henchmen_smg_b");
                    break;
            }
        }
        public static void SetModelBlastshield(this Entity self)
        {
            switch (Function.Call<string>("GetMapCustom", "axischar"))
            {
                case "opforce_air":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_russian_air_sniper");
                    self.Call("Attach", "head_russian_military_f", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");
                    break;

                case "opforce_snow":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_russian_arctic_sniper");
                    self.Call("Attach", "head_russian_military_f", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");

                    break;

                case "opforce_urban":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_russian_urban_sniper");
                    self.Call("Attach", "head_russian_military_f", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");
                    break;

                case "opforce_woodland":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_russian_woodland_sniper");
                    self.Call("Attach", "head_russian_military_e", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");
                    break;

                case "opforce_africa":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_africa_militia_sniper");
                    self.Call("Attach", "head_ghillie_africa_militia_sniper", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");
                    break;

                case "opforce_henchmen":
                    self.Call("DetachAll");
                    self.Call("SetModel", "mp_body_opforce_henchmen_sniper");
                    self.Call("Attach", "head_henchmen_aa", "j_eyeball", true);
                    self.Call("SetViewModel", "viewhands_juggernaut_ally");
                    break;
            }

        }
        public static void SetModelInfected(this Entity player)
        {
            switch (Function.Call<string>("GetMapCustom", "environment"))
            {
                case "desert":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_desert");
                    player.Call("SetModel", "mp_body_ally_ghillie_desert_sniper");
                    break;
                case "arctic":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_arctic");
                    player.Call("SetModel", "mp_body_ally_ghillie_desert_sniper");
                    break;
                case "urban":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_urban");
                    player.Call("SetModel", "mp_body_ally_ghillie_urban_sniper");
                    break;
                case "forest":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_woodland");
                    player.Call("SetModel", "mp_body_ally_ghillie_woodland_sniper");
                    break;
                case "forest_militia":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_woodland");
                    player.Call("SetModel", "mp_body_ally_ghillie_woodland_sniper");
                    player.Call("HidePart", "j_helmet");
                    break;
                case "desert_militia":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_desert");
                    player.Call("SetModel", "mp_body_ally_ghillie_desert_sniper");
                    player.Call("HidePart", "j_helmet");
                    break;
                case "arctic_militia":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_arctic");
                    player.Call("SetModel", "mp_body_ally_ghillie_desert_sniper");
                    player.Call("HidePart", "j_helmet");
                    break;
                case "urban_militia":
                    player.Call("SetViewModel", "viewhands_iw5_ghillie_urban");
                    player.Call("SetModel", "mp_body_ally_ghillie_urban_sniper");
                    player.Call("HidePart", "j_helmet");
                    break;
            }
            player.Call("HidePart", "j_helmet"); //На всякий случай!
        }
        
    }
}
*/