using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;
using static InfectedServer.LevelClass.INFO;

namespace InfectedServer
{
    public static class SoundClass
    {
        /// <summary>
        /// Play leader dialog for player
        /// </summary>
        /// <param name="self">Player</param>
        /// <param name="sound">Sound</param>
        public static void PlayLeaderDialog(Entity self, string sound)
        {
            if (self.GetField<string>("SessionTeam") == "allies")
                self.Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "allieschar")) + "1mc_" + sound);
            else
                self.Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "axischar")) + "1mc_" + sound);
        }

        /// <summary>
        /// Play leader dialog for all players
        /// </summary>
        /// <param name="self">Owner sound</param>
        /// <param name="soundFriendly">Friendly sound</param>
        /// <param name="soundEnemy">Enemy sound</param>
        public static void PlayLeaderDialogForPlayers(Entity self, string soundFriendly, string soundEnemy)
        {
            for (int i = 0; i < 18; i++)
            {
                if (!(Entity.GetEntity(i).IsAlive || Entity.GetEntity(i).IsPlayer))
                    continue;

                if (Entity.GetEntity(i).GetField<string>("SessionTeam") == self.GetField<string>("SessionTeam") && self.GetField<string>("SessionTeam") == "allies")
                    Entity.GetEntity(i).Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "allieschar")) + "1mc_" + soundFriendly);
                else
                    Entity.GetEntity(i).Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "axischar")) + "1mc_" + soundEnemy);
            }
        }

        /// <summary>
        /// Radio for AC130
        /// </summary>
        /// <param name="self">Player</param>
        /// <param name="sound">Sound</param>
        public static void PlayRadio(Entity self, String sound)
        {
            if (self.GetField<string>("SessionTeam") == "allies")
                self.Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "allieschar") + sound));
            else
                self.Call("PlayLocalSound", GetTeamVoicePrefix(Function.Call<string>("GetMapCustom", "axischar")) + sound);
        }
    }
}
