using Celeste.Mod.XaphanHelper.Managers;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.UI_Elements {
    public class WarpMenu : TextMenu {
        public string ConfirmSfx = "event:/game/xaphan/warp";
        public string CurrentWarp;
        public string WipeType = "Fade";
        public float WipeDuration = 0.75f;

        public WarpMenu() {
            AutoScroll = false;
            Depth = -20000;
        }

        public WarpInfo SelectedWarp => ((WarpButton)Current).Warp;

        public void UpdateWarps(List<WarpInfo> warps) {
            Clear();
            Selection = -1;
            BuildMenu(warps);
        }

        private void BuildMenu(List<WarpInfo> warps) {
            Add(new SubHeader(Dialog.Clean("XaphanHelper_Warp_Select_Destination"), topPadding: false));
            foreach (WarpInfo warp in warps) {
                if (warp.ID == CurrentWarp) {
                    Insert(1, new WarpButton(warp) {
                        ConfirmSfx = SFX.ui_game_unpause,
                        Label = Dialog.Clean("XaphanHelper_Warp_Stay"),
                        OnPressed = OnCancel
                    });
                } else {
                    Add(new WarpButton(warp) {
                        ConfirmSfx = ConfirmSfx,
                        OnPressed = () => WarpManager.Teleport(warp, WipeType, WipeDuration)
                    });
                }
            }
        }

        public class WarpButton : Button {
            public WarpInfo Warp;

            public WarpButton(WarpInfo warp)
                : base(Dialog.Clean(warp.DialogKey)) {
                Warp = warp;
            }
        }
    }
}
