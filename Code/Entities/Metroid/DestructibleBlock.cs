using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Colliders;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Enemies;
using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/DestructibleBlock")]
    public class DestructibleBlock : Solid
    {
        public class BlockIndicator : Entity
        {
            private DestructibleBlock Block;

            public Sprite indicatorSprite;

            private string mode;

            private bool startRevealed;

            public BlockIndicator(Vector2 Position, DestructibleBlock block, string mode, bool startRevealed) : base(Position)
            {
                Block = block;
                this.mode = mode;
                this.startRevealed = startRevealed;
                Collidable = true;
                Add(indicatorSprite = new Sprite(GFX.Game, "objects/XaphanHelper/DestructibleBlock/"));
                indicatorSprite.AddLoop("Shoot", "Shoot", 1f);
                indicatorSprite.AddLoop("Bomb", "Bomb", 1f);
                indicatorSprite.AddLoop("Missile", "Missile", 1f);
                indicatorSprite.AddLoop("SuperMissile", "SuperMissile", 1f);
                indicatorSprite.AddLoop("PowerBomb", "PowerBomb", 1f);
                indicatorSprite.AddLoop("SpeedBooster", "SpeedBooster", 1f);
                indicatorSprite.AddLoop("ScrewAttack", "ScrewAttack", 1f);
                indicatorSprite.AddLoop("Crumble", "Crumble", 1f);
                if (mode == "SpeedBooster")
                {
                    Add(new PlayerCollider(OnPlayer, new Hitbox(8, 8)));
                }
                if (mode == "ScrewAttack")
                {
                    Add(new ScrewAttackCollider(OnScrewAttack, new Hitbox(8, 8)));
                }
                Depth = -10002;
            }

            public override void Added(Scene scene)
            {
                base.Added(scene);
                if (startRevealed)
                {
                    RevealSequence();
                }
            }

            private void OnPlayer(Player player)
            {
                if (!Block.breaking)
                {
                    Block.Break();
                }
            }

            private void OnScrewAttack(ScrewAttackManager manager)
            {
                if (!Block.breaking)
                {
                    Block.Break();
                }
            }

            public void RevealSequence()
            {
                indicatorSprite.Play(mode);
            }
        }

        private Sprite blockSprite;

        private string baseSprite;

        private string texture;

        private MTexture[,] tilesetTextures;

        private bool startRevealed;

        public bool revealed;

        public string mode;

        public bool breaking;

        public bool respawn;

        public float respawnTimer = 0f;

        public int index;

        private bool respawnAsBaseSprite;

        public BlockIndicator indicator;

        private PlayerCollider pc;

        private ScrewAttackCollider sac;

        private EnemyBarrier barrier;

        private Coroutine BreakRoutine = new Coroutine();

        private string tilesetMask = "000-010-000";

        public DestructibleBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: true)
        {
            Tag = Tags.TransitionUpdate;
            SurfaceSoundIndex = data.Int("soundIndex");
            baseSprite = data.Attr("baseSprite");
            texture = data.Attr("texture");
            startRevealed = data.Bool("startRevealed");
            index = data.Int("index", 0);
            mode = data.Attr("mode");
            if (!string.IsNullOrEmpty(baseSprite))
            {
                Add(blockSprite = new Sprite(GFX.Game, baseSprite));
                blockSprite.AddLoop("idle", "", 1f);
                blockSprite.Play("idle");
            }
            else if (!string.IsNullOrEmpty(texture))
            {
                MTexture mtexture = GFX.Game["tilesets/" + texture];
                tilesetTextures = new MTexture[6, 15];
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        tilesetTextures[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                    }
                }
            }
            respawn = data.Bool("respawn");
            respawnTimer = data.Float("respawnTimer");
            respawnAsBaseSprite = data.Bool("respawnAsBaseSprite");
            Collider = new Hitbox(8, 8);
            if (mode == "SpeedBooster")
            {
                Add(pc = new PlayerCollider(OnCollidePlayer, Collider));
            }
            if (mode == "ScrewAttack" || mode == "Bomb")
            {
                Add(sac = new ScrewAttackCollider(OnCollideScrewAttack, Collider));
            }
            Add(new LightOcclude(1f));
            Depth = -10002;
        }

        private void OnCollidePlayer(Player player)
        {
            if (SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))
            {
                Break();
            }
        }

        private void OnCollideScrewAttack(ScrewAttackManager manager)
        {
            if (manager.StartedScrewAttack)
            {
                Break();
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (mode == "SpeedBooster" || mode == "ScrewAttack")
            {
                SceneAs<Level>().Add(barrier = new EnemyBarrier(Position, 8, 8, SurfaceSoundIndex));
            }
            if (startRevealed)
            {
                revealed = true;
                if (blockSprite != null)
                {
                    blockSprite.Visible = false;
                }
            }
            if (CollideCheck<Player>())
            {
                if (barrier != null)
                {
                    barrier.RemoveSelf();
                }
                RemoveSelf();
            }
            else
            {
                SceneAs<Level>().Add(indicator = new BlockIndicator(Position, this, mode, startRevealed));
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (!string.IsNullOrEmpty(texture) && tilesetTextures != null)
            {
                string SolidTopLeft = "0";
                string SolidTop = "0";
                string SolidTopRight = "0";
                string SolidLeft = "0";
                string SolidRight = "0";
                string SolidBottomLeft = "0";
                string SolidBottom = "0";
                string SolidBottomRight = "0";
                string SolidFarTop = "0";
                string SolidFarLeft = "0";
                string SolidFarRight = "0";
                string SolidFarBottom = "0";
                if (CollideCheck<Solid>(Position + new Vector2(-8f, -8f)))
                {
                    SolidTopLeft = "1";
                }
                else
                {
                    SolidTopLeft = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(0f, -8f)))
                {
                    SolidTop = "1";
                }
                else
                {
                    SolidTop = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(8f, -8f)))
                {
                    SolidTopRight = "1";
                }
                else
                {
                    SolidTopRight = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(-8f, 0f)))
                {
                    SolidLeft = "1";
                }
                else
                {
                    SolidLeft = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(8f, 0f)))
                {
                    SolidRight = "1";
                }
                else
                {
                    SolidRight = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(-8f, 8f)))
                {
                    SolidBottomLeft = "1";
                }
                else
                {
                    SolidBottomLeft = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(0f, 8f)))
                {
                    SolidBottom = "1";
                }
                else
                {
                    SolidBottom = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(8f, 8f)))
                {
                    SolidBottomRight = "1";
                }
                else
                {
                    SolidBottomRight = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(0f, -16f)))
                {
                    SolidFarTop = "1";
                }
                else
                {
                    SolidFarTop = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(-16f, 0f)))
                {
                    SolidFarLeft = "1";
                }
                else
                {
                    SolidFarLeft = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(16f, 0f)))
                {
                    SolidFarRight = "1";
                }
                else
                {

                    SolidFarRight = "0";
                }
                if (CollideCheck<Solid>(Position + new Vector2(0f, 16f)))
                {
                    SolidFarBottom = "1";
                }
                else
                {
                    SolidFarBottom = "0";
                }
                tilesetMask = SolidTopLeft + SolidTop + SolidTopRight + "-" + SolidLeft + "1" + SolidRight + "-" + SolidBottomLeft + SolidBottom + SolidBottomRight + "-" + SolidFarTop + SolidFarLeft + SolidFarRight + SolidFarBottom;
            }
        }

        public override void Update()
        {
            base.Update();
            ScrewAttackManager screwAttackManager = SceneAs<Level>().Tracker.GetEntity<ScrewAttackManager>();
            if ((mode == "SpeedBooster" && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking")) || (mode == "ScrewAttack" && screwAttackManager != null && screwAttackManager.StartedScrewAttack) || breaking)
            {
                Collidable = false;
            }
            else
            {
                Collidable = true;
            }
            if (mode == "Crumble")
            {
                Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
                if (player != null && GetPlayerOnTop() != null && !breaking && !BreakRoutine.Active)
                {
                    Add(BreakRoutine = new Coroutine(BreakSequence()));
                }
                else if (CollideCheck<Player>() && GetPlayerOnTop() == null)
                {
                    Collidable = false;
                }
            }
            if (index != 0 && mode != "Crumble")
            {
                foreach (AmmoCollectable ammoCollectable in SceneAs<Level>().Tracker.GetEntities<AmmoCollectable>())
                {
                    if (ammoCollectable.index == index)
                    {
                        if (breaking)
                        {
                            ammoCollectable.Visible = true;
                        }
                        else
                        {
                            ammoCollectable.Visible = false;
                        }
                    }
                }
                foreach (SamusUpgradeCollectable upgradeCollectable in SceneAs<Level>().Tracker.GetEntities<SamusUpgradeCollectable>())
                {
                    if (upgradeCollectable.index == index)
                    {
                        if (breaking)
                        {
                            upgradeCollectable.Visible = true;
                        }
                        else
                        {
                            upgradeCollectable.Visible = false;
                        }
                    }
                }
            }
        }

        public void Reveal()
        {
            if (CollideCheck<Solid>(Position))
            {
                return;
            }
            revealed = true;
            if (blockSprite != null)
            {
                blockSprite.Visible = false;
            }
            indicator.RevealSequence();
        }

        public void Break()
        {
            if (CollideCheck<Solid>(Position))
            {
                return;
            }
            if (index != 0)
            {
                foreach (DestructibleBlock destructibleBlock in SceneAs<Level>().Tracker.GetEntities<DestructibleBlock>())
                {
                    if (destructibleBlock.index == index && destructibleBlock.mode == mode && !destructibleBlock.breaking)
                    {
                        destructibleBlock.StartBreakSequence();
                    }
                }
            }
            else
            {
                StartBreakSequence();
            }
        }

        public void StartBreakSequence()
        {
            Add(BreakRoutine = new Coroutine(BreakSequence()));
        }

        public IEnumerator BreakSequence()
        {
            
            indicator.RevealSequence();
            if (mode == "Crumble")
            {
                float timer = MetroidGameplayController.MaxRunSpeed < 1.25f ? 0.01f : 0.06f;
                while (timer > 0f)
                {
                    yield return null;
                    timer -= Engine.DeltaTime;
                }
            }
            breaking = true;
            //Audio.Play("event:/game/xaphan/block_break");
            Audio.Play("event:/game/general/platform_disintegrate", Center);
            SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 1, Position + new Vector2(2f, 2f), new Vector2(2f, 0f));
            SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 1, Position + new Vector2(4f, 2f), new Vector2(2f, 0f));
            SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 1, Position + new Vector2(6f, 2f), new Vector2(2f, 0f));
            SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 1, Position + new Vector2(8f, 2f), new Vector2(2f, 0f));
            float distance = (indicator.indicatorSprite.X * 7f % 3f + 1f) * 12f;
            Vector2 from = indicator.indicatorSprite.Position;
            for (float time = 0f; time < 1f; time += Engine.DeltaTime / 0.4f)
            {
                yield return null;
                indicator.indicatorSprite.Position = from + Vector2.UnitY * Ease.CubeIn(time) * distance;
                indicator.indicatorSprite.Color = Color.Gray * (1f - time);
                indicator.indicatorSprite.Scale = Vector2.One * (1f - time * 0.5f);
            }
            indicator.indicatorSprite.Visible = false;
            if (blockSprite != null)
            {
                blockSprite.Visible = false;
            }
            if (respawn)
            {
                yield return respawnTimer;
                while ((CollideCheck<Player>() || CollideCheck<Enemy>()) && indicator.indicatorSprite.Scale != Vector2.One)
                {
                    yield return null;
                }
                breaking = false;
                if (respawnAsBaseSprite && blockSprite != null && mode != "Crumble")
                {
                    blockSprite.Visible = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(texture))
                    {
                        tilesetTextures = null;
                    }
                    Audio.Play("event:/game/general/platform_return", Center);
                    indicator.indicatorSprite.Visible = true;
                    indicator.indicatorSprite.Play(mode);
                    indicator.indicatorSprite.Color = Color.White;
                    indicator.indicatorSprite.Position = Vector2.Zero;
                    if (GetPlayerOnTop() == null)
                    {
                        for (float time = 0f; time < 1f; time += Engine.DeltaTime / 0.25f)
                        {
                            yield return null;
                            indicator.indicatorSprite.Scale = Vector2.One * (1f + Ease.BounceOut(1f - time) * 0.2f);
                        }
                    }
                    indicator.indicatorSprite.Scale = Vector2.One;
                }
            }
            else
            {
                if (barrier != null)
                {
                    barrier.RemoveSelf();
                }
                RemoveSelf();
                SceneAs<Level>().Remove(indicator);
            }
        }

        public override void Render()
        {
            base.Render();
            int positionXLastDigit = (int)Position.X / 8 % 10;
            int positionYLastDigit = (int)Position.Y / 8 % 10;
            int seed = positionXLastDigit + positionYLastDigit;
            int variation = 0;
            int variationPadding = 0;
            if (seed == 1 || seed == 5 || seed ==  9 || seed == 13 || seed == 17)
            {
                variation = 1;
            } else if (seed == 2 || seed == 6 || seed == 10 || seed == 14 || seed == 18)
            {
                variation = 2;
            }
            else if (seed == 3 || seed == 7 || seed == 11 || seed == 15)
            {
                variation = 3;
            }
            if (seed <= 11)
            {
                variationPadding = seed;
            }
            else
            {
                variationPadding = seed - 12;
            }
            if (tilesetTextures != null && !revealed && !breaking)
            {
                if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    if (tilesetMask[12] == '1' || tilesetMask[13] == '1' || tilesetMask[14] == '1' || tilesetMask[15] == '1')
                    {
                        tilesetTextures[5, variationPadding].Draw(Position);
                    }
                    else
                    {
                        tilesetTextures[5, 12].Draw(Position);
                    }
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 0].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 1].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 2].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 3].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 4].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 5].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 6].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 7].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 8].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 9].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 10].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 11].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 12].Draw(Position);
                }
                else if (tilesetMask[0] == '1' && tilesetMask[1] == '1' && tilesetMask[2] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '0' && tilesetMask[9] == '1' && tilesetMask[10] == '1')
                {
                    tilesetTextures[4, 13].Draw(Position);
                }
                else if (tilesetMask[0] == '0' && tilesetMask[1] == '1' && tilesetMask[2] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[8] == '1' && tilesetMask[9] == '1' && tilesetMask[10] == '0')
                {
                    tilesetTextures[4, 14].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 0].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 1].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '0' && tilesetMask[6] == '1' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 2].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '0' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 3].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '1' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 4].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '0' && tilesetMask[6] == '0' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 5].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '0' && tilesetMask[6] == '0' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 6].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '0' && tilesetMask[6] == '0' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 7].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '0' && tilesetMask[6] == '1' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 8].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '0' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 9].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '0' && tilesetMask[6] == '0' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 10].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '0' && tilesetMask[6] == '1' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 11].Draw(Position);
                }
                else if (tilesetMask[1] == '0' && tilesetMask[4] == '1' && tilesetMask[6] == '0' && tilesetMask[9] == '1')
                {
                    tilesetTextures[variation, 12].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '0' && tilesetMask[6] == '1' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 13].Draw(Position);
                }
                else if (tilesetMask[1] == '1' && tilesetMask[4] == '1' && tilesetMask[6] == '0' && tilesetMask[9] == '0')
                {
                    tilesetTextures[variation, 14].Draw(Position);
                }
            }
        }
    }
}
