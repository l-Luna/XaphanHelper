using System;
using System.Reflection;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class ClimbingKit : Upgrade
    {
        private ILHook wallJumpHook;

        public override int GetDefaultValue()
        {
            return 1;
        }

        public override int GetValue()
        {
            return XaphanModule.ModSettings.ClimbingKit ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            XaphanModule.ModSettings.ClimbingKit = (value != 0);
        }

        public override void Load()
        {
            IL.Celeste.Player.ClimbUpdate += onPlayerClimbUpdate;
            wallJumpHook = new ILHook(typeof(Player).GetMethod("orig_WallJump", BindingFlags.Instance | BindingFlags.NonPublic), modWallJump);
        }

        public override void Unload()
        {
            IL.Celeste.Player.ClimbUpdate -= onPlayerClimbUpdate;
            if (wallJumpHook != null)
            {
                wallJumpHook.Dispose();
            }
        }

        public bool Active(Level level)
        {
            if (XaphanModule.useUpgrades)
            {
                return XaphanModule.ModSettings.ClimbingKit && !XaphanModule.ModSaveData.ClimbingKitInactive.Contains(level.Session.Area.GetLevelSet());
            }
            return true;
        }

        private void onPlayerClimbUpdate(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<VirtualButton>("get_Pressed")))
            {
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldfld, typeof(Player).GetField("moveX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
                cursor.EmitDelegate<Func<bool, Player, int, bool>>(modJumpButtonCheck);
            }

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(Input), "MoveY"), instr => instr.MatchLdfld<VirtualIntegerAxis>("Value")))
            {
                cursor.EmitDelegate<Func<int, int>>(orig =>
                {
                    if (Engine.Scene is Level)
                    {
                        Level level = (Level)Engine.Scene;
                        if (Active(level))
                        {
                            return orig;
                        }
                    }

                    return 0;
                });
            }
        }

        private bool modJumpButtonCheck(bool actualValue, Player self, int moveX)
        {
            if (Active(self.SceneAs<Level>()))
            {
                // nothing to do
                return actualValue;
            }

            if (moveX == -(int)self.Facing)
            {
                // This will lead to a wall jump. We want to kill climb jumping. So let it go
                return actualValue;
            }

            // let the game believe Jump is not pressed, so it won't return the player to the Normal state (leading to a weird animation / sound effect).
            return false;
        }

        private void modWallJump(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(MoveType.Before, instr => instr.OpCode == OpCodes.Ldarg_0, instr => instr.MatchLdfld<Player>("moveX")))
            {
                cursor.Index++;
                ILCursor cursorAfterBranch = cursor.Clone();
                if (cursorAfterBranch.TryGotoNext(MoveType.After, instr => instr.OpCode == OpCodes.Brfalse_S))
                {
                    cursor.Emit(OpCodes.Pop);
                    cursor.EmitDelegate<Func<bool>>(neutralJumpingEnabled);
                    cursor.Emit(OpCodes.Brfalse_S, cursorAfterBranch.Next);
                    cursor.Emit(OpCodes.Ldarg_0);
                }
            }
        }

        private bool neutralJumpingEnabled()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                return Active(level);
            }
            return false;
        }
    }
}
