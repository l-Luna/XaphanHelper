using System.Collections;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Entities;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    class HeatIndicator : Entity
    {
        public float heat;

        public float maxDuration;

        public float timer;

        public MTexture heatIndicator;

        public string inactiveFlag;

        public Coroutine HeatDamageRoutine = new();

        public HeatIndicator(float maxDuration, string inactiveFlag)
        {
            AddTag(Tags.Persistent | Tags.HUD | Tags.PauseUpdate | Tags.TransitionUpdate);
            this.maxDuration = maxDuration;
            this.inactiveFlag = inactiveFlag;
            Depth = -20000;
        }

        public void updateMaxDuration(float newDuration)
        {
            maxDuration = newDuration;
        }

        public override void Update()
        {
            base.Update();
            Level level = SceneAs<Level>();
            Player player = Scene.Tracker.GetEntity<Player>();
            HeatController controller = level.Tracker.GetEntity<HeatController>();
            if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene || level.InCutscene) || (player != null && !player.Sprite.Visible) || (level.Tracker.GetEntity<MapScreen>() != null && level.Tracker.GetEntity<MapScreen>().ShowUI)
                || (level.Tracker.GetEntity<StatusScreen>() != null && level.Tracker.GetEntity<StatusScreen>().ShowUI) || (level.Tracker.GetEntity<WarpScreen>() != null && level.Tracker.GetEntity<WarpScreen>().ShowUI) || !XaphanModule.CanOpenMap(level) || level.Session.GetFlag(inactiveFlag))
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            }
            if (!level.FrozenOrPaused && level.Tracker.GetEntity<WarpScreen>() == null && level.Tracker.GetEntity<MapScreen>() == null && level.Tracker.GetEntity<StatusScreen>() == null && !level.Session.GetFlag(inactiveFlag))
            {
                if (!XaphanModule.useMetroidGameplay)
                {
                    if (player != null && player.CanRetry && controller != null && !level.Transitioning && !VariaJacket.Active(level) && !XaphanModule.PlayerIsControllingRemoteDrone())
                    {
                        timer = 0;
                        heat += Engine.DeltaTime;
                        if (heat >= maxDuration && !player.Dead)
                        {
                            player.Die(Vector2.Zero);
                        }
                    }
                    if ((controller == null || VariaJacket.Active(level) || XaphanModule.PlayerIsControllingRemoteDrone()) && heat > 0)
                    {
                        heat -= Engine.DeltaTime;
                    }
                    if (heat < 0)
                    {
                        heat = 0;
                    }
                    if (controller == null && heat == 0)
                    {
                        timer += Engine.DeltaTime;
                    }
                    if (timer >= 3f)
                    {
                        RemoveSelf();
                    }
                }
                else
                {
                    if (player != null && player.CanRetry && controller != null && !level.Transitioning && !VariaJacket.Active(level) && !XaphanModule.PlayerIsControllingRemoteDrone() && !HeatDamageRoutine.Active)
                    {
                        Add(HeatDamageRoutine = new Coroutine(HeatDamage()));
                    }
                }
            }
        }

        public bool LiquidDamageRoutineActive()
        {
            foreach (Liquid liquid in SceneAs<Level>().Tracker.GetEntities<Liquid>())
            {
                if (liquid.LiquidDamageRoutine.Active)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator HeatDamage()
        {
            HealthDisplay healthDisplay = SceneAs<Level>().Tracker.GetEntity<HealthDisplay>();
            while (healthDisplay != null && healthDisplay.CurrentHealth > 0 && !VariaJacket.Active(SceneAs<Level>()) && !SceneAs<Level>().Transitioning && !SceneAs<Level>().FrozenOrPaused && SceneAs<Level>().Tracker.GetEntity<WarpScreen>() == null && SceneAs<Level>().Tracker.GetEntity<MapScreen>() == null && SceneAs<Level>().Tracker.GetEntity<StatusScreen>() == null && !SceneAs<Level>().Session.GetFlag(inactiveFlag))
            {
                healthDisplay.playDamageSfx();
                healthDisplay.CurrentHealth -= 1;
                healthDisplay.GetEnergyTanks();
                float tickTimer = 0.066f;
                while (tickTimer > 0)
                {
                    tickTimer -= Engine.DeltaTime;
                    yield return null;
                }
            }
            if (!LiquidDamageRoutineActive())
            {
                healthDisplay.stopDamageSfx();
            }
        }

        public override void Render()
        {
            base.Render();
            if (heat == 0)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator00"];
            }
            else if (heat < maxDuration * 0.055f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator01"];
            }
            else if (heat < maxDuration * 0.11f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator02"];
            }
            else if (heat < maxDuration * 0.165f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator03"];
            }
            else if (heat < maxDuration * 0.22f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator04"];
            }
            else if (heat < maxDuration * 0.275f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator05"];
            }
            else if (heat < maxDuration * 0.33f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator06"];
            }
            else if (heat < maxDuration * 0.385f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator07"];
            }
            else if (heat < maxDuration * 0.44f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator08"];
            }
            else if (heat < maxDuration * 0.495f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator09"];
            }
            else if (heat < maxDuration * 0.55f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator10"];
            }
            else if (heat < maxDuration * 0.605f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator11"];
            }
            else if (heat < maxDuration * 0.66f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator12"];
            }
            else if (heat < maxDuration * 0.715f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator13"];
            }
            else if (heat < maxDuration * 0.77f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator14"];
            }
            else if (heat < maxDuration * 0.825f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator15"];
            }
            else if (heat < maxDuration * 0.88f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator16"];
            }
            else if (heat < maxDuration * 0.935f)
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator17"];
            }
            else
            {
                heatIndicator = GFX.Gui["upgrades/heatindicator18"];
            }
            if (heatIndicator != null && XaphanModule.ModSettings.ShowHeatLevel && !XaphanModule.useMetroidGameplay)
            {
                heatIndicator.Draw(new Vector2(1840, 5 + (SceneAs<Level>().Tracker.GetEntity<MiniMap>() != null && XaphanModule.ModSettings.ShowMiniMap ? 150 : 0)));
            }
        }
    }
}
