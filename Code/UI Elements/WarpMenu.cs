using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class WarpMenu : TextMenu
    {
        public string ConfirmSfx = "event:/game/xaphan/warp";
        public string CurrentWarp;
        public string WipeType = "Fade";
        public float WipeDuration = 0.75f;

        public WarpMenu()
        {
            AutoScroll = false;
            Depth = -20000;
        }

        public WarpInfo SelectedWarp => ((WarpButton)Current).Warp;

        public override void Update()
        {
            base.Update();
            FormationBackdrop formationBackdrop = SceneAs<Level>().FormationBackdrop;
            Alpha = SceneAs<Level>().FormationBackdrop.Display ? (float)DynamicData.For(formationBackdrop).Get("fade") : 1f;
        }

        public void UpdateWarps(List<WarpInfo> warps)
        {
            Clear();
            Selection = -1;
            BuildMenu(warps);
        }

        private void BuildMenu(List<WarpInfo> warps)
        {
            Add(new SubHeader(Dialog.Clean("XaphanHelper_Warp_Select_Destination"), topPadding: false));
            foreach (WarpInfo warp in warps)
            {
                if (warp.ID == CurrentWarp)
                {
                    Insert(1, new WarpButton(warp)
                    {
                        ConfirmSfx = SFX.ui_game_unpause,
                        Label = Dialog.Clean("XaphanHelper_Warp_Stay"),
                        OnPressed = () => OnStay()
                    });
                }
                else
                {
                    Add(new WarpButton(warp)
                    {
                        ConfirmSfx = ConfirmSfx,
                        OnPressed = () => OnConfirm(warp)
                    });
                }
            }
        }

        private void OnStay()
        {
            Focused = false;
            Add(new Coroutine(OnStayRoutine()));
        }

        private IEnumerator OnStayRoutine()
        {
            float timer = 0.04f;
            while (timer > 0)
            {
                timer -= Engine.DeltaTime;
                yield return null;
            }
            OnCancel();
        }

        private void OnConfirm(WarpInfo warp)
        {
            Focused = false;
            MapData mapData = AreaData.Areas[SceneAs<Level>().Session.Area.ID].Mode[0].MapData;
            if ((SceneAs<Level>().Session.Level == warp.Room && !mapData.HasEntity("XaphanHelper/InGameMapController")))
            {
                WarpScreen warpScreen = SceneAs<Level>().Tracker.GetEntity<WarpScreen>();
                if (warpScreen != null)
                {
                    warpScreen.UninitializeScreen();
                    warpScreen.StartDelay();
                }
            }
            WarpManager.Teleport(warp, WipeType, WipeDuration);
        }

        public class WarpButton : Button
        {
            public WarpInfo Warp;

            public WarpButton(WarpInfo warp)
                : base(Dialog.Clean(warp.DialogKey))
            {
                Warp = warp;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                base.Render(position, highlighted);
            }
        }
    }
}
