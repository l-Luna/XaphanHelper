using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/CustomRefill")]
    public class CustomRefill : Entity
    {
        public static ParticleType P_Shatter;

        public static ParticleType P_Glow;

        private Sprite sprite;

        private Sprite flash;

        private Sprite outline;

        private Wiggler wiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private Level level;

        private SineWave sine;

        private bool twoDashes;

        private bool oneUse;

        private ParticleType p_shatter;

        private ParticleType p_regen;

        private ParticleType p_glow;

        private float respawnTimer;

        private float respawnTime;

        public CustomRefill(Vector2 position, bool twoDashes, bool oneUse, float respawnTime) : base(position)
        {
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(OnPlayer));
            this.twoDashes = twoDashes;
            this.oneUse = oneUse;
            this.respawnTime = respawnTime;
            P_Shatter = new ParticleType(Refill.P_Shatter)
            {
                Color = Calc.HexToColor("DAECFA"),
                Color2 = Calc.HexToColor("8FC7EF")
            };
            P_Glow = new ParticleType(Refill.P_Glow)
            {
                Color = Calc.HexToColor("BED6E9"),
                Color2 = Calc.HexToColor("73A5CE")
            };
            string path;
            if (oneUse)
            {
                if (twoDashes)
                {
                    path = "objects/XaphanHelper/CustomRefill/refillTwoOnce/";
                }
                else
                {
                    path = "objects/XaphanHelper/CustomRefill/refillOnce/";
                }
                p_shatter = P_Shatter;
                p_glow = P_Glow;
            }
            else
            {
                if (twoDashes)
                {
                    path = "objects/XaphanHelper/CustomRefill/refillTwo/";
                    p_shatter = Refill.P_ShatterTwo;
                    p_regen = Refill.P_RegenTwo;
                    p_glow = Refill.P_GlowTwo;
                }
                else
                {
                    path = "objects/XaphanHelper/CustomRefill/refill/";
                    p_shatter = Refill.P_Shatter;
                    p_regen = Refill.P_Regen;
                    p_glow = Refill.P_Glow;
                }
            }
            if (!oneUse)
            {
                Add(outline = new Sprite(GFX.Game, path + "outline"));
                outline.AddLoop("idle", "", respawnTime / (twoDashes ? 25f : 21f));
                outline.Visible = false;
                outline.CenterOrigin();
            }
            Add(sprite = new Sprite(GFX.Game, path + "idle"));
            sprite.AddLoop("idle", "", 0.1f);
            sprite.Play("idle");
            sprite.CenterOrigin();
            Add(flash = new Sprite(GFX.Game, path + "flash"));
            flash.Add("flash", "", 0.05f);
            flash.OnFinish = delegate
            {
                flash.Visible = false;
            };
            flash.CenterOrigin();
            Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v)
            {
                sprite.Scale = (flash.Scale = Vector2.One * (1f + v * 0.2f));
            }));
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f, 0f));
            sine.Randomize();
            UpdateY();
            Depth = -100;
        }

        public CustomRefill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("twoDash"), data.Bool("oneUse"), data.Float("respawnTime"))
        {

        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            else if (Scene.OnInterval(0.1f))
            {
                level.ParticlesFG.Emit(p_glow, 1, Position, Vector2.One * 5f);
            }
            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            if (Scene.OnInterval(2f) && sprite.Visible)
            {
                flash.Play("flash", restart: true);
                flash.Visible = true;
            }
        }

        private void Respawn()
        {
            if (!Collidable)
            {
                Collidable = true;
                sprite.Visible = true;
                outline.Visible = false;
                outline.Stop();
                Depth = -100;
                wiggler.Start();
                Audio.Play(twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_return" : "event:/game/general/diamond_return", Position);
                level.ParticlesFG.Emit(p_regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateY()
        {
            Sprite obj = flash;
            Sprite obj2 = sprite;
            float num2 = (bloom.Y = sine.Value * 2f);
            float num5 = (obj.Y = (obj2.Y = num2));
        }

        public override void Render()
        {
            if (sprite.Visible)
            {
                sprite.DrawOutline();
            }
            base.Render();
        }

        private void OnPlayer(Player player)
        {
            if (player.UseRefill(twoDashes))
            {
                Audio.Play(twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Collidable = false;
                Add(new Coroutine(RefillRoutine(player)));
                respawnTimer = respawnTime;
            }
        }

        private IEnumerator RefillRoutine(Player player)
        {
            Celeste.Freeze(0.05f);
            yield return null;
            level.Shake();
            sprite.Visible = (flash.Visible = false);
            if (!oneUse)
            {
                outline.Visible = true;
                outline.Play("idle");
            }
            Depth = 8999;
            yield return 0.05f;
            float num = player.Speed.Angle();
            level.ParticlesFG.Emit(p_shatter, 5, Position, Vector2.One * 4f, num - (float)Math.PI / 2f);
            level.ParticlesFG.Emit(p_shatter, 5, Position, Vector2.One * 4f, num + (float)Math.PI / 2f);
            SlashFx.Burst(Position, num);
            if (oneUse)
            {
                RemoveSelf();
            }
        }
    }
}
