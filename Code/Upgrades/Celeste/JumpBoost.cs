using System;
using System.Reflection;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class JumpBoost : Upgrade
    {
        public bool Boosted;

        public static MethodInfo Player_launchBegin = typeof(Player).GetMethod("LaunchBegin", BindingFlags.Instance | BindingFlags.NonPublic);

        private ILHook wallJumpHook;

        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.JumpBoost ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.JumpBoost = (value != 0);
        }

        public override void Load()
        {
            IL.Celeste.Player.Jump += ilPlayerJump;
            wallJumpHook = new ILHook(typeof(Player).GetMethod("orig_WallJump", BindingFlags.Instance | BindingFlags.NonPublic), ilWallJump);
            On.Celeste.Player.Update += onPlayerUpdate;
            On.Celeste.Player.Jump += modPlayerJump;
            On.Celeste.Player.WallJump += modPlayerWallJump;
        }

        public override void Unload()
        {
            IL.Celeste.Player.Jump -= ilPlayerJump;
            if (wallJumpHook != null) wallJumpHook.Dispose();
            On.Celeste.Player.Update -= onPlayerUpdate;
            On.Celeste.Player.Jump -= modPlayerJump;
            On.Celeste.Player.WallJump -= modPlayerWallJump;
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.JumpBoost && !XaphanModule.ModSaveData.JumpBoostInactive.Contains(level.Session.Area.GetLevelSet());
        }

        private static void ilPlayerJump(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(-105f)))
            {
                cursor.EmitDelegate<Func<float>>(determineJumpHeightFactor);
                cursor.Emit(OpCodes.Mul);
            }
        }

        private void ilWallJump(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(-105f)))
            {
                cursor.EmitDelegate<Func<float>>(determineJumpHeightFactor);
                cursor.Emit(OpCodes.Mul);
            }
        }

        private static float determineJumpHeightFactor()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (Active(level) && Input.MenuUp.Check && Input.Jump.Pressed && !XaphanModule.PlayerIsControllingRemoteDrone())
                {
                    return 1.8f;
                }
            }
            return 1f;
        }

        private void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (Active(self.SceneAs<Level>()) && !XaphanModule.PlayerIsControllingRemoteDrone())
            {
                if (Boosted)
                {
                    Boosted = false;
                    Player_launchBegin.Invoke(self, new object[0]);
                }
            }
        }

        private void modPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
        {
            CheckIfBoosted(self);
            orig(self, particles, playSfx);
        }

        private void modPlayerWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
        {
            CheckIfBoosted(self);
            orig(self, dir);
        }

        public void CheckIfBoosted(Player player)
        {
            if (Active(player.SceneAs<Level>()) && Input.MenuUp.Check && !XaphanModule.PlayerIsControllingRemoteDrone())
            {
                Boosted = true;
                Audio.Play("event:/char/badeline/jump_superwall", player.Position);
            }
        }
    }
}
