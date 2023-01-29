using System;
using System.Collections;
using Celeste.Mod.XaphanHelper.Entities;
using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class HoverBoots : Upgrade
    {
        public static bool CanFloat = true;

        public static bool Floating;

        public static float floatTimer;

        public static Tween tween;

        public static Coroutine FloatTimerRoutine = new();

        public ParticleType P_Dust = new();

        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return XaphanModule.ModSettings.HoverBoots ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            XaphanModule.ModSettings.HoverBoots = (value != 0);
        }

        public override void Load()
        {
            IL.Celeste.Player.NormalUpdate += ilPlayerNormalUpdate;
            IL.Celeste.Player.Render += ilPlayerRender;
            On.Celeste.Player.Update += onPlayerUpdate;
        }

        public override void Unload()
        {
            IL.Celeste.Player.NormalUpdate -= ilPlayerNormalUpdate;
            IL.Celeste.Player.Render -= ilPlayerRender;
            On.Celeste.Player.Update -= onPlayerUpdate;
        }

        public static bool Active(Level level)
        {
            return XaphanModule.ModSettings.HoverBoots && !XaphanModule.ModSaveData.HoverBootsInactive.Contains(level.Session.Area.GetLevelSet());
        }

        private static void ilPlayerNormalUpdate(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.65f)))
            {
                cursor.EmitDelegate<Func<float>>(determineFrictionFactor);
                cursor.Emit(OpCodes.Mul);
            }

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(900f)))
            {
                cursor.EmitDelegate<Func<float>>(determineGravityFactor);
                cursor.Emit(OpCodes.Mul);
            }
        }

        private static void ilPlayerRender(ILContext il)
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(instr => instr.MatchCallvirt<StateMachine>("get_State"), instr => instr.MatchLdcI4(19)))
            {
                cursor.Index++;
                cursor.EmitDelegate<Func<int, int>>(orig =>
                {
                    if (Floating)
                    {
                        return 19;
                    }
                    return orig;
                });
            }
        }

        private static float determineFrictionFactor()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (Active(level) && XaphanModule.PlayerIsControllingRemoteDrone())
                {
                    if (Floating)
                    {
                        return 0.5f;
                    }
                }
            }
            return 1f;
        }

        private static float determineGravityFactor()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (Active(level) && XaphanModule.PlayerIsControllingRemoteDrone())
                {
                    if (Floating)
                    {
                        return 0f;
                    }
                }
            }
            return 1f;
        }

        private void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (self.GetType() == typeof(Player))
            {
                if (Active(self.SceneAs<Level>()) && XaphanModule.PlayerIsControllingRemoteDrone())
                {
                    if (self.Speed.Y >= 0 && Input.Grab.Check && !self.OnGround() && !self.DashAttacking && self.StateMachine.State == 0 && CanFloat && !Liquid.determineIfInQuicksand())
                    {
                        if (!Floating)
                        {
                            Floating = true;
                            Vector2 start = self.Position;
                            Vector2 end = self.Position + new Vector2(0f, 4f);
                            float num = Vector2.Distance(start, end) / 12.5f;
                            tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.SineInOut, num, start: true);
                            tween.OnUpdate = delegate (Tween t)
                            {
                                self.MoveToY(Vector2.Lerp(start, end, t.Eased).Y);
                            };
                            self.Add(tween);
                        }
                        self.Speed.Y = 0f;
                        P_Dust = new ParticleType()
                        {
                            Source = GFX.Game["particles/zappysmoke00"],
                            Color = Calc.HexToColor("D9A066"),
                            Color2 = Calc.HexToColor("873724"),
                            ColorMode = ParticleType.ColorModes.Blink,
                            Acceleration = new Vector2(0f, 4f),
                            LifeMin = 0.3f,
                            LifeMax = 0.5f,
                            Size = 0.5f,
                            SizeRange = 0.2f,
                            Direction = (float)Math.PI / 2f,
                            DirectionRange = 0.5f,
                            SpeedMin = 5f,
                            SpeedMax = 15f,
                            RotationMode = ParticleType.RotationModes.Random,
                            ScaleOut = true,
                            UseActualDeltaTime = true
                        };
                        Dust.Burst(self.BottomCenter, -(float)Math.PI / 2f, 1, P_Dust);
                        if (!FloatTimerRoutine.Active)
                        {
                            self.Add(FloatTimerRoutine = new Coroutine(FloatTimer(self)));
                        }
                    }
                    else
                    {
                        StopFloating(self);
                    }
                }
                else
                {
                    StopFloating(self);
                }
            }
        }

        private void StopFloating(Player player)
        {
            Floating = false;
            if (tween != null)
            {
                tween.Stop();
                player.Position.Y = (float)Math.Floor(player.Position.Y);
            }
            if (player.OnGround())
            {
                FloatTimerRoutine.Cancel();
                Floating = false;
                floatTimer = 1f;
                CanFloat = true;
            }
        }

        private IEnumerator FloatTimer(Player player)
        {
            while (floatTimer > 0f)
            {
                if (Floating)
                {
                    if (floatTimer <= 0.65f && player.Scene.OnRawInterval(0.06f))
                    {
                        if (player.Sprite.Color == Color.Red)
                        {
                            player.Sprite.Color = Color.White;
                        }
                        else if (player.Sprite.Color == Color.White)
                        {
                            player.Sprite.Color = Color.Red;
                        }
                    }
                    floatTimer -= Engine.DeltaTime;
                    yield return null;
                }
                else
                {
                    yield return null;
                }
            }
            player.Sprite.Color = Color.White;
            CanFloat = false;
        }
    }
}
