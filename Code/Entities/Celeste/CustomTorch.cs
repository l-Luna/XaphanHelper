using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/CustomTorch")]
    class CustomTorch : Entity
    {
        public ParticleType P_OnLight;

        public Color Color;

        private EntityID eid;

        private bool lit;

        private bool playLitSound;

        private VertexLight light;

        private BloomPoint bloom;

        private bool startLit;

        private Sprite TorchSprite;

        private string sprite;

        private string flag;

        private string FlagName => "torch_" + eid.Key;

        private float alpha;

        private int startFade;

        private int endFade;

        private string sound;

        public CustomTorch(EntityData data, Vector2 position, EntityID ID) : base(data.Position + position)
        {
            eid = ID;
            playLitSound = data.Bool("playLitSound");
            startLit = data.Bool("startLit");
            flag = data.Attr("flag");
            sprite = data.Attr("sprite");
            alpha = data.Float("alpha", 1f);
            if (alpha < 0)
            {
                alpha = 0;
            }
            else if (alpha > 1)
            {
                alpha = 1;
            }
            startFade = data.Int("startFade", 48);
            endFade = data.Int("endFade", 64);
            if (endFade < startFade)
            {
                endFade = startFade;
            }
            sound = data.Attr("sound", "event:/game/05_mirror_temple/torch_activate");
            if (string.IsNullOrEmpty(sound))
            {
                sound = "event:/game/05_mirror_temple/torch_activate";
            }
            Color = Color.Lerp(Color.White, Calc.HexToColor(data.Attr("color", "ffa500")), 0.5f);
            P_OnLight = new ParticleType
            {
                Color = Calc.HexToColor(data.Attr("color", "ffa500")),
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.6f,
                LifeMax = 0.8f,
                Size = 1f,
                SpeedMin = 80f,
                SpeedMax = 90f,
                SpeedMultiplier = 0.03f,
                DirectionRange = (float)Math.PI * 2f
            };
            Collider = new Hitbox(32f, 32f, -8f, -8f);
            Add(new PlayerCollider(OnPlayer));
            Add(light = new VertexLight(Color, alpha, startFade, endFade));
            Add(bloom = new BloomPoint(0.5f, 8f));
            bloom.Visible = false;
            light.Visible = false;
            bloom.Position += new Vector2(8, 8);
            light.Position += new Vector2(8, 8);
            light.Color = Color;
            Depth = 2000;
            if (!string.IsNullOrEmpty(sprite))
            {
                Add(TorchSprite = new Sprite(GFX.Game, sprite));
                TorchSprite.AddLoop("off", "", 0.1f, 0);
                TorchSprite.AddLoop("turnOn", "", 0.08f, 1, 2);
                TorchSprite.AddLoop("on", "", 0.08f, 3, 4, 5, 6, 7);
                TorchSprite.CenterOrigin();
                TorchSprite.Position = TorchSprite.Position + new Vector2(8, 8);
                TorchSprite.Play("off");
            }
            else
            {
                Add(TorchSprite = GFX.SpriteBank.Create("torch"));
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (startLit || SceneAs<Level>().Session.GetFlag(FlagName) || SceneAs<Level>().Session.GetFlag(flag))
            {
                bloom.Visible = (light.Visible = true);
                lit = true;
                Collidable = false;
                TorchSprite.Play("on");
            }
            if (!string.IsNullOrEmpty(flag))
            {
                if (!SceneAs<Level>().Session.GetFlag(flag))
                {
                    lit = false;
                    bloom.Visible = false;
                    light.Visible = false;
                    SceneAs<Level>().Session.SetFlag(FlagName, false);
                    TorchSprite.Play("off");
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (!string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag))
            {
                if (!lit)
                {
                    if (playLitSound)
                    {
                        Audio.Play(sound, Position);
                    }
                    lit = true;
                    bloom.Visible = true;
                    light.Visible = true;
                    Collidable = false;
                    TorchSprite.Play("turnOn");
                    TorchSprite.OnLastFrame = onLastFrame;
                    Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 1f, start: true);
                    tween.OnUpdate = delegate (Tween t)
                    {
                        light.Color = Color.Lerp(Color.White, Color, t.Eased);
                        light.StartRadius = startFade + (1f - t.Eased) * 32f;
                        light.EndRadius = endFade + (1f - t.Eased) * 32f;
                        bloom.Alpha = alpha + 0.5f * (1f - t.Eased);
                    };
                    Add(tween);
                    SceneAs<Level>().Session.SetFlag(FlagName);
                    SceneAs<Level>().ParticlesFG.Emit(P_OnLight, 12, Position + new Vector2(8, 8), new Vector2(3f, 3f));
                }
            }
            else if (lit && !string.IsNullOrEmpty(flag) && !SceneAs<Level>().Session.GetFlag(flag))
            {
                lit = false;
                bloom.Visible = false;
                light.Visible = false;
                SceneAs<Level>().Session.SetFlag(FlagName, false);
                SceneAs<Level>().ParticlesFG.Emit(P_OnLight, 12, Position + new Vector2(8, 8), new Vector2(3f, 3f));
                TorchSprite.Play("off");
            }
        }

        private void onLastFrame(string s)
        {
            if (SceneAs<Level>().Session.GetFlag(flag))
            {
                TorchSprite.Play("on");
            }
        }

        private void OnPlayer(Player player)
        {
            if (flag == "" && !lit)
            {
                if (playLitSound)
                {
                    Audio.Play(sound, Position);
                }
                lit = true;
                bloom.Visible = true;
                light.Visible = true;
                Collidable = false;
                TorchSprite.Play("turnOn");
                TorchSprite.OnLastFrame = onLastFrame;
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 1f, start: true);
                tween.OnUpdate = delegate (Tween t)
                {
                    light.Color = Color.Lerp(Color.White, Color, t.Eased);
                    light.StartRadius = startFade + (1f - t.Eased) * 32f;
                    light.EndRadius = endFade + (1f - t.Eased) * 32f;
                    bloom.Alpha = alpha + 0.5f * (1f - t.Eased);
                };
                Add(tween);
                SceneAs<Level>().Session.SetFlag(FlagName);
                SceneAs<Level>().ParticlesFG.Emit(P_OnLight, 12, Position + new Vector2(8, 8), new Vector2(3f, 3f));
            }
        }
    }
}
