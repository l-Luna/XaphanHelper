using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/CustomFollower")]
    class CustomFollower : Entity
    {
        public EntityID ID;

        private Vector2 start;

        public Follower Follower;

        private Sprite sprite;

        private bool collected;

        public bool ReturnHomeWhenLost = true;

        private Wiggler wiggler;

        private Wiggler rotateWiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private Tween lightTween;

        private float wobble;

        private float collectTimer;

        private string type;

        private string Prefix;

        private int chapterIndex;

        public CustomFollower(EntityData data, Vector2 offset, EntityID gid)
        {
            ID = gid;
            string str = data.Attr("type").Replace(" ", "");
            type = (char.ToLower(str[0]) + str.Substring(1));
            Position = (start = data.Position + offset);
            Depth = -100;
            Collider = new Hitbox(14f, 14f, -7f, -7f);
            Add(new PlayerCollider(OnPlayer));
            Add(new MirrorReflection());
            Add(Follower = new Follower(ID, null, OnLoseLeader));
            Follower.FollowDelay = 0.3f;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Prefix = SceneAs<Level>().Session.Area.GetLevelSet();
            chapterIndex = SceneAs<Level>().Session.Area.ChapterIndex;
            switch (type)
            {
                case "missile":
                    {
                        if ((!XaphanModule.PlayerHasGolden && !XaphanModule.Settings.SpeedrunMode && XaphanModule.ModSaveData.DroneMissilesUpgrades.Contains(Prefix + "_Ch" + chapterIndex + "_" + ID)) || ((XaphanModule.PlayerHasGolden || XaphanModule.Settings.SpeedrunMode) && XaphanModule.ModSaveData.SpeedrunModeDroneMissilesUpgrades.Contains(Prefix + "_Ch" + chapterIndex + "_" + ID)))
                        {
                            RemoveSelf();
                        }
                        break;
                    }
                case "superMissile":
                    {
                        if ((!XaphanModule.PlayerHasGolden && !XaphanModule.Settings.SpeedrunMode && XaphanModule.ModSaveData.DroneSuperMissilesUpgrades.Contains(Prefix + "_Ch" + chapterIndex + "_" + ID)) || ((XaphanModule.PlayerHasGolden || XaphanModule.Settings.SpeedrunMode) && XaphanModule.ModSaveData.SpeedrunModeDroneSuperMissilesUpgrades.Contains(Prefix + "_Ch" + chapterIndex + "_" + ID)))
                        {
                            RemoveSelf();
                        }
                        break;
                    }
            }
            Add(sprite = new Sprite(GFX.Game, "collectables/XaphanHelper/CustomFollower/" + type + "/"));
            sprite.AddLoop("idle", "idle", 0.1f, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3);
            sprite.Add("collect", "collect", 0.05f);
            sprite.CenterOrigin();
            sprite.Play("idle");
            sprite.OnFrameChange = OnAnimate;
            Add(wiggler = Wiggler.Create(0.4f, 4f, delegate (float v)
            {
                sprite.Scale = Vector2.One * (1f + v * 0.35f);
            }));
            Add(rotateWiggler = Wiggler.Create(0.5f, 4f, delegate (float v)
            {
                sprite.Rotation = v * 30f * ((float)Math.PI / 180f);
            }));
            Add(bloom = new BloomPoint(1f, 12f));
            Add(light = new VertexLight(Color.White, 1f, 16, 24));
            Add(lightTween = light.CreatePulseTween());
            if ((scene as Level).Session.BloomBaseAdd > 0.1f)
            {
                bloom.Alpha *= 0.5f;
            }
        }

        public override void Update()
        {
            if (!collected)
            {
                wobble += Engine.DeltaTime * 4f;
                Sprite obj = sprite;
                BloomPoint bloomPoint = bloom;
                float num2 = (light.Y = (float)Math.Sin(wobble) * 2f);
                float num5 = (obj.Y = (bloomPoint.Y = num2));
                int followIndex = Follower.FollowIndex;
                if (Follower.Leader != null && Follower.DelayTimer <= 0f && StrawberryRegistry.IsFirstStrawberry(this))
                {
                    Player player = Follower.Leader.Entity as Player;
                    bool flag = false;
                    if (player != null && player.Scene != null && !player.StrawberriesBlocked)
                    {
                        if (player.OnSafeGround && player.StateMachine.State != 13)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        collectTimer += Engine.DeltaTime;
                        if (collectTimer > 0.15f)
                        {
                            OnCollect();
                        }
                    }
                    else
                    {
                        collectTimer = Math.Min(collectTimer, 0f);
                    }
                }
                else
                {
                    if (followIndex > 0)
                    {
                        collectTimer = -0.15f;
                    }
                }
            }
            base.Update();
            if (Follower.Leader != null && Scene.OnInterval(0.08f))
            {
                ParticleType type = Strawberry.P_Glow;
                SceneAs<Level>().ParticlesFG.Emit(type, Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
            }
        }

        public void OnPlayer(Player player)
        {
            if (Follower.Leader != null || collected)
            {
                return;
            }
            ReturnHomeWhenLost = true;
            Audio.Play("event:/game/general/strawberry_touch", Position);
            player.Leader.GainFollower(Follower);
            wiggler.Start();
            Depth = -1000000;
        }

        private void OnAnimate(string id)
        {
            if (sprite.CurrentAnimationFrame == 27)
            {
                lightTween.Start();
                if (!collected && (CollideCheck<FakeWall>() || CollideCheck<Solid>()))
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.1f);
                }
                else
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f);
                }
            }
        }
        public void OnCollect()
        {
            if (!collected)
            {
                int collectIndex = 0;
                collected = true;
                if (Follower.Leader != null)
                {
                    Player obj = Follower.Leader.Entity as Player;
                    collectIndex = obj.StrawberryCollectIndex;
                    obj.StrawberryCollectIndex++;
                    obj.StrawberryCollectResetTimer = 2.5f;
                    Follower.Leader.LoseFollower(Follower);
                }
                Session session = (Scene as Level).Session;
                //session.DoNotLoad.Add(ID);
                session.UpdateLevelStartDashes();
                Add(new Coroutine(CollectRoutine(collectIndex)));
            }
        }

        private IEnumerator CollectRoutine(int collectIndex)
        {
            Tag = Tags.TransitionUpdate;
            Depth = -2000010;
            Audio.Play("event:/game/general/strawberry_get", Position, "colour", 0, "count", collectIndex);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            sprite.Play("collect");
            while (sprite.Animating)
            {
                yield return null;
            }
            Scene.Add(new StrawberryPoints(Position, false, collectIndex, false));
            switch (type)
            {
                case "missile":
                {
                        
                    XaphanModule.ModSaveData.DroneMissilesUpgrades.Add(Prefix + "_Ch" + chapterIndex + "_" + ID);
                    if (XaphanModule.PlayerHasGolden || XaphanModule.Settings.SpeedrunMode)
                    {
                        XaphanModule.ModSaveData.SpeedrunModeDroneMissilesUpgrades.Add(Prefix + "_Ch" + chapterIndex + "_" + ID);
                    }
                    break;
                }
                case "superMissile":
                    {

                        XaphanModule.ModSaveData.DroneSuperMissilesUpgrades.Add(Prefix + "_Ch" + chapterIndex + "_" + ID);
                        if (XaphanModule.PlayerHasGolden || XaphanModule.Settings.SpeedrunMode)
                        {
                            XaphanModule.ModSaveData.SpeedrunModeDroneSuperMissilesUpgrades.Add(Prefix + "_Ch" + chapterIndex + "_" + ID);
                        }
                        break;
                    }
            }
            RemoveSelf();
        }

        private void OnLoseLeader()
        {
            if (collected || !ReturnHomeWhenLost)
            {
                return;
            }
            Alarm.Set(this, 0.15f, delegate
            {
                Vector2 vector = (start - Position).SafeNormalize();
                float num = Vector2.Distance(Position, start);
                float num2 = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
                Vector2 control = start + vector * 16f + vector.Perpendicular() * num2 * Calc.Random.Choose(1, -1);
                SimpleCurve curve = new SimpleCurve(Position, start, control);
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(num / 100f, 0.4f), start: true);
                tween.OnUpdate = delegate (Tween f)
                {
                    Position = curve.GetPoint(f.Eased);
                };
                tween.OnComplete = delegate
                {
                    Depth = 0;
                };
                Add(tween);
            });
        }
    }
}
