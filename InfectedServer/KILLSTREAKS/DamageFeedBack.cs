using System;
using InfinityScript;

namespace InfectedServer.KILLSTREAKS
{
    public class DamageFeedBack : BaseScript
    {
        /// <summary>
        /// Входная точка
        /// </summary>
        public DamageFeedBack()
        {
            OnNotify("damage_feedback", (attacker, type) => UpdateDamageFeedBack(attacker.As<Entity>(), type.As<string>()));
            PlayerConnected += new Action<Entity>(player => OnPlayerConnected(player));
        }

        /// <summary>
        /// Обработка события подключения игрока
        /// </summary>
        /// <param name="player">Подключенный игрок</param>
        private void OnPlayerConnected(Entity player)
        {
            CreateDamageHud(player);
        }

        /// <summary>
        /// Функция создания HUD-элеметов для трэкинга дамага на экране
        /// </summary>
        /// <param name="self">Игрок</param>
        private void CreateDamageHud(Entity self)
        {
            self.SetField("hud_damagefeedback", new Parameter(HudElem.NewClientHudElem(self)));
            self.GetField<HudElem>("hud_damagefeedback").HorzAlign = "center";
            self.GetField<HudElem>("hud_damagefeedback").VertAlign = "middle";
            self.GetField<HudElem>("hud_damagefeedback").X = -12;
            self.GetField<HudElem>("hud_damagefeedback").Y = -12;
            self.GetField<HudElem>("hud_damagefeedback").Alpha = 0;
            self.GetField<HudElem>("hud_damagefeedback").SetShader("damage_feedback", 24, 48);

        }

        /// <summary>
        /// Функция анимации трэкинга дамага
        /// </summary>
        /// <param name="self">Игрок(атакующий)</param>
        /// <param name="typeHit">Тип брони(повреждаемого)</param>
        private void UpdateDamageFeedBack(Entity self, string typeHit)
        {
            self.GetField<HudElem>("hud_damagefeedback").SetShader(typeHit, 24, 48);

            self.Call("PlayLocalSound", "MP_hit_alert");

            self.GetField<HudElem>("hud_damagefeedback").Alpha = 1;
            self.GetField<HudElem>("hud_damagefeedback").Call("FadeOverTime", 1);
            self.GetField<HudElem>("hud_damagefeedback").Alpha = 0;
        }
    }
}