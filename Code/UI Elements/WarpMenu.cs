using System;
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
        public new float Height;
        public bool center;

        public WarpMenu()
        {
            AutoScroll = false;
            Depth = -20000;
        }

        public WarpInfo SelectedWarp => ((WarpButton)Current).Warp;

        public override void Update()
        {
            base.Update();
            if (!center)
            {
                Position = new Vector2(Celeste.TargetWidth - 445f, Celeste.TargetHeight / 2f + 38f);
            }
            FormationBackdrop formationBackdrop = SceneAs<Level>().FormationBackdrop;
            Alpha = SceneAs<Level>().FormationBackdrop.Display ? (float)DynamicData.For(formationBackdrop).Get("fade") : 1f;
            if (IndexOf(Current) <= 5)
            {
                foreach (Item item in Items)
                {
                    if (item is WarpButton)
                    {
                        WarpButton warpButton = (WarpButton)item;
                        if (IndexOf(warpButton) > 10)
                        {
                            warpButton.Hide = true;
                        }
                        else
                        {
                            warpButton.Hide = false;
                        }
                    }
                }
            }
            if (IndexOf(Current) > 5 && IndexOf(Current) <= LastPossibleSelection - 5)
            {
                foreach (Item item in Items)
                {
                    if (item is WarpButton)
                    {
                        WarpButton warpButton = (WarpButton)item;
                        if (IndexOf(warpButton) <= IndexOf(Current) - 6 || IndexOf(warpButton) >= IndexOf(Current) + 5)
                        {
                            warpButton.Hide = true;
                        }
                        else
                        {
                            warpButton.Hide = false;
                        }
                    }
                }
            }
            if (IndexOf(Current) > LastPossibleSelection - 5)
            {
                foreach (Item item in Items)
                {
                    if (item is WarpButton)
                    {
                        WarpButton warpButton = (WarpButton)item;
                        if (IndexOf(warpButton) <= LastPossibleSelection - 10)
                        {
                            warpButton.Hide = true;
                        }
                        else
                        {
                            warpButton.Hide = false;
                        }
                    }
                }
            }
        }

        public new void RecalculateSize()
        {
            Height = 0f;
            base.RecalculateSize();
            foreach (Item item in Items)
            {
                if (item is WarpButton)
                {
                    WarpButton warpButton = (WarpButton)item;
                    if (warpButton.Visible && !warpButton.Hide)
                    {
                        Height += warpButton.Height() + ItemSpacing;
                    }
                }
                else
                {
                    if (item.Visible)
                    {
                        Height += item.Height() + ItemSpacing;
                    }
                }
            }
            Height -= ItemSpacing;
            Height = Math.Min(Height, 10 * ActiveFont.LineHeight + 37);
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

        public override void Render()
        {
            RecalculateSize();
            Vector2 value = Position - Justify * new Vector2(Width, Height);
            foreach (Item item in Items)
            {
                if (item is WarpButton)
                {
                    WarpButton warpButton = (WarpButton)item;
                    if (warpButton.Visible && !warpButton.Hide)
                    {
                        float num = warpButton.Height();
                        Vector2 position = value + new Vector2(0f, num * 0.5f + warpButton.SelectWiggler.Value * 8f);
                        if (position.Y + num * 0.5f > 0f && position.Y - num * 0.5f < Engine.Height)
                        {
                            warpButton.Render(position, Focused && Current == warpButton);
                        }
                        value.Y += num + ItemSpacing;
                    }
                }
                else
                {
                    if (item.Visible)
                    {
                        float num = item.Height();
                        Vector2 position = value + new Vector2(0f, num * 0.5f + item.SelectWiggler.Value * 8f);
                        if (position.Y + num * 0.5f > 0f && position.Y - num * 0.5f < Engine.Height)
                        {
                            item.Render(position, Focused && Current == item);
                        }
                        value.Y += num + ItemSpacing;
                    }
                }
            }
        }

        public class WarpButton : Button
        {
            public WarpInfo Warp;

            public bool Hide;

            public WarpButton(WarpInfo warp)
                : base(Dialog.Clean(warp.DialogKey))
            {
                Warp = warp;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                if (!Hide)
                {
                    base.Render(position, highlighted);
                }
            }
        }
    }
}
