using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/Slope")]
    class Slope : Entity
    {
        protected static XaphanModuleSettings Settings => XaphanModule.Settings;

        public string Side;

        public string TilesTop;

        public string TilesBottom;

        public string Directory;

        public string Texture;

        public bool Gentle;

        public bool CanSlide;

        public bool UpsideDown;

        public bool NoRender;

        public bool StickyDash;

        public int SoundIndex;

        public int SlopeHeight;

        public int[] variation = new int[35];

        public int[] variationInner = new int[35];

        private MTexture[,] BaseTextures;

        private MTexture[] SlopeTextures;

        public bool Rainbow;

        public bool CanJumpThrough;

        public bool VisualOnly;

        public Slope(Vector2 position, Vector2 offset, bool gentle, string side, int soundIndex, int slopeHeight, string tilesTop, string tilesBottom, string texture, bool canSlide, string directory, bool upsideDown, bool noRender, bool stickyDash, bool rainbow, bool canJumpThrough, bool visualOnly = false) : base(position + offset)
        {
            Tag = Tags.TransitionUpdate;
            Gentle = gentle;
            Side = side;
            SoundIndex = soundIndex;
            SlopeHeight = slopeHeight;
            TilesTop = tilesTop;
            TilesBottom = tilesBottom;
            Texture = texture;
            CanSlide = canSlide;
            Directory = directory;
            UpsideDown = upsideDown;
            NoRender = noRender;
            StickyDash = stickyDash;
            Rainbow = rainbow;
            CanJumpThrough = canJumpThrough;
            VisualOnly = visualOnly;
            if (SlopeHeight < 1)
            {
                SlopeHeight = 1;
            }
            else if (SlopeHeight > 30)
            {
                SlopeHeight = 30;
            }
            if (string.IsNullOrEmpty(Directory))
            {
                Directory = "objects/XaphanHelper/Slope";
            }
            MTexture mtexture = GFX.Game[Directory + "/" + Texture];
            BaseTextures = new MTexture[6, 15];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    BaseTextures[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            SlopeTextures = new MTexture[32];
            if (!UpsideDown)
            {
                for (int k = 0; k < 32; k++)
                {
                    SlopeTextures[k] = mtexture.GetSubtexture(new Rectangle(k, 0, 1, 8));
                }
            }
            else
            {
                for (int k = 0; k < 32; k++)
                {
                    SlopeTextures[k] = mtexture.GetSubtexture(new Rectangle(k, 8, 1, 8));
                }
            }
            int Variation;
            Math.DivRem((int)Position.X / 8, 4, out Variation);
            Variation = Math.Abs(Variation);
            variation[0] = Variation;

            for (int l = 1; l < 35; l++)
            {
                Variation++;
                if (Variation >= 4)
                {
                    Variation = 0;
                }
                variation[l] = Variation;
            }
            Math.DivRem((int)Position.X / 8, 12, out Variation);
            Variation = Math.Abs(Variation);
            variationInner[0] = Variation;

            for (int m = 1; m < 35; m++)
            {
                Variation++;
                if (Variation >= 12)
                {
                    Variation = 0;
                }
                variationInner[m] = Variation;
            }
            Depth = -10000;
        }

        public Slope(EntityData data, Vector2 offset) : this(data.Position, offset, data.Bool("gentle"), data.Attr("side"), data.Int("soundIndex"), data.Int("slopeHeight", 1), data.Attr("tilesTop"), data.Attr("tilesBottom"),
            data.Attr("texture", "cement"), data.Bool("canSlide", false), data.Attr("customDirectory", ""), data.Bool("upsideDown", false), data.Bool("noRender", false), data.Bool("stickyDash", false), data.Bool("rainbow", false), data.Bool("canJumpThrough", false))
        {
            
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!VisualOnly)
            {
                if (!UpsideDown)
                {
                    if (Side == "Left")
                    {
                        for (int i = 0; i <= SlopeHeight - 1; i++)
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 0f + i * 8), Gentle ? 8 : 8, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 1f + i * 8), Gentle ? 10 : 9, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 2f + i * 8), Gentle ? 12 : 10, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 3f + i * 8), Gentle ? 14 : 11, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 4f + i * 8), Gentle ? 16 : 12, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 5f + i * 8), Gentle ? 18 : 13, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 6f + i * 8), Gentle ? 20 : 14, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 7f + i * 8), Gentle ? 22 : 15, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));

                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, i * 8), 4, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? i * 16 : i * 8) + 4, 4f + i * 8), Gentle ? 8 : 4, 4, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }
                        /*if (TilesBottom != "Horizontal")
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? SlopeHeight * 16 : SlopeHeight * 8, SlopeHeight * 8), 8, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }*/
                    }
                    else
                    {
                        for (int j = 0; j <= SlopeHeight - 1; j++)
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 16f - j * 16 : 16f - j * 8, 0f + j * 8), Gentle ? 8 : 8, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 14f - j * 16 : 15f - j * 8, 1f + j * 8), Gentle ? 10 : 9, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 12f - j * 16 : 14f - j * 8, 2f + j * 8), Gentle ? 12 : 10, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 10f - j * 16 : 13f - j * 8, 3f + j * 8), Gentle ? 14 : 11, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 8f - j * 16 : 12f - j * 8, 4f + j * 8), Gentle ? 16 : 12, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 6f - j * 16 : 11f - j * 8, 5f + j * 8), Gentle ? 18 : 13, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 4f - j * 16 : 10f - j * 8, 6f + j * 8), Gentle ? 20 : 14, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 2f - j * 16 : 9f - j * 8, 7f + j * 8), Gentle ? 22 : 15, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));

                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - j * 16 : 16f - j * 8) + 4, j * 8), 4, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - j * 16 : 16f - j * 8) - (Gentle ? 8 : 4) + 4, 4f + j * 8), Gentle ? 8 : 4, 4, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }
                        /*if (TilesBottom != "Horizontal")
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - SlopeHeight * 16 : 16f - SlopeHeight * 8), SlopeHeight * 8), 8, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }*/
                    }
                }
                else
                {
                    if (Side == "Left")
                    {
                        for (int i = 0; i <= SlopeHeight - 1; i++)
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 15f - i * 8), Gentle ? 8 : 8, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 14f - i * 8), Gentle ? 10 : 9, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 13f - i * 8), Gentle ? 12 : 10, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 12f - i * 8), Gentle ? 14 : 11, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 11f - i * 8), Gentle ? 16 : 12, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 10f - i * 8), Gentle ? 18 : 13, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 9f - i * 8), Gentle ? 20 : 14, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 8f - i * 8), Gentle ? 22 : 15, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));

                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? i * 16 : i * 8, 8f - i * 8), 4, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? i * 16 : i * 8) + 4, 8f - i * 8), Gentle ? 8 : 4, 4, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }
                        /*if (TilesBottom != "Horizontal")
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? SlopeHeight * 16 : SlopeHeight * 8, 8 + SlopeHeight * -8), 8, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }*/
                    }
                    else
                    {
                        for (int j = 0; j <= SlopeHeight - 1; j++)
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 16f - j * 16 : 16f - j * 8, 15f - j * 8), Gentle ? 8 : 8, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 14f - j * 16 : 15f - j * 8, 14f - j * 8), Gentle ? 10 : 9, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 12f - j * 16 : 14f - j * 8, 13f - j * 8), Gentle ? 12 : 10, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 10f - j * 16 : 13f - j * 8, 12f - j * 8), Gentle ? 14 : 11, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 8f - j * 16 : 12f - j * 8, 11f - j * 8), Gentle ? 16 : 12, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 6f - j * 16 : 11f - j * 8, 10f - j * 8), Gentle ? 18 : 13, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 4f - j * 16 : 10f - j * 8, 9f - j * 8), Gentle ? 20 : 14, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2(Gentle ? 2f - j * 16 : 9f - j * 8, 8f - j * 8), Gentle ? 22 : 15, 1, SoundIndex, Side, UpsideDown, CanJumpThrough));

                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - j * 16 : 16f - j * 8) + 4, 8f - j * 8), 4, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - j * 16 : 16f - j * 8) - (Gentle ? 8 : 4) + 4, 8f - j * 8), Gentle ? 8 : 4, 4, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }
                        /*if (TilesBottom != "Horizontal")
                        {
                            SceneAs<Level>().Add(new ActorBarrier(Position + new Vector2((Gentle ? 16f - SlopeHeight * 16 : 16f - SlopeHeight * 8), 8 + SlopeHeight * -8), 8, 8, SoundIndex, Side, UpsideDown, CanJumpThrough));
                        }*/
                    }
                }
                SceneAs<Level>().Add(new PlayerPlatform(Position + new Vector2(Side == "Right" ? ((Gentle ? -(SlopeHeight - 1) * 16 : -(SlopeHeight - 1) * 8) + 8) * (UpsideDown ? -1 : 1) : 0 + 0, (8 * (SlopeHeight - 1) + 4)) * (UpsideDown ? -1 : 1), Gentle ? 8 + 16 * SlopeHeight : 8 + 8 * SlopeHeight, Gentle, Side, SoundIndex, SlopeHeight, CanSlide, Top, UpsideDown, StickyDash, CanJumpThrough));
                SceneAs<Level>().Add(new FakePlayerPlatform(Position + new Vector2(Side == "Right" ? ((Gentle ? -(SlopeHeight - 1) * 16 : -(SlopeHeight - 1) * 8) + 8) * (UpsideDown ? -1 : 1) : 0 + 0, (8 * (SlopeHeight - 1) + 4)) * (UpsideDown ? -1 : 1), Gentle ? 8 + 16 * SlopeHeight : 8 + 8 * SlopeHeight, Gentle, Side, SoundIndex, SlopeHeight, Top, UpsideDown, StickyDash, CanJumpThrough));
                SceneAs<Level>().Add(new PlayerBlocker(new Vector2(Side == "Right" ? Left + 20f : Left, UpsideDown ? Top + 12f : Top), 4f, 4f, false, SoundIndex, UpsideDown, true));
            }
        }

        private Color GetHue(Vector2 position)
        {
            float num = 280f;
            float value = (position.Length() + Scene.TimeActive * 50f) % num / num;
            return Calc.HsvToColor(0.4f + Calc.YoYo(value) * 0.4f, 0.4f, 0.9f);
        }

        public override void Render()
        {
            base.Render();
            if (!NoRender)
            {
                Vector2 Pos = Vector2.Zero;

                if (!UpsideDown)
                {
                    if (Side == "Left")
                    {
                        // Top tiles

                        if (TilesTop == "Horizontal")
                        {
                            BaseTextures[0 + variation[0], 0].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[1], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[0]].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[1]].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[1], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Horizontal Corner")
                        {
                            BaseTextures[4, 1].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[2], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[2]].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[3]].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[2], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Vertical")
                        {
                            BaseTextures[4, 1].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[4]].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge")
                        {
                            BaseTextures[0 + variation[3], 11].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[4], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[5], 2].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[5]].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge Corner")
                        {
                            BaseTextures[0 + variation[6], 11].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[7], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 3].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[6]].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge")
                        {
                            BaseTextures[0 + variation[8], 11].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[9], 2].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge Corner")
                        {
                            BaseTextures[0 + variation[10], 11].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 3].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        // Special case for bottom tiles

                        if (Gentle && TilesBottom != "Small Edge" && TilesBottom != "Small Edge Corner")
                        {
                            BaseTextures[5, 0 + variationInner[7]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        if (TilesBottom == "Horizontal")
                        {
                            if (Gentle)
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[8]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[9]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[10]].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[11]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[12]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[13]].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Slope Tiles

                        if (Gentle)
                        {
                            for (int i = 1; i <= SlopeHeight - 1; i++)
                            {
                                if (i > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[i]].Draw(Pos = Position + new Vector2(-24f + i * 16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[i + 1]].Draw(Pos = Position + new Vector2(-16f + i * 16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[i + 2]].Draw(Pos = Position + new Vector2(-8f + i * 16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[i + 3]].Draw(Pos = Position + new Vector2(i * 16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int j = 0; j <= SlopeHeight - 1; j++)
                            {
                                SlopeTextures[0 + variation[j] * 8].Draw(Pos = Position + new Vector2(8f + j * 16f, j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j] * 8].Draw(Pos = Position + new Vector2(9f + j * 16f, j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j] * 8].Draw(Pos = Position + new Vector2(10f + j * 16f, 1f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j] * 8].Draw(Pos = Position + new Vector2(11f + j * 16f, 1f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j] * 8].Draw(Pos = Position + new Vector2(12f + j * 16f, 2f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j] * 8].Draw(Pos = Position + new Vector2(13f + j * 16f, 2f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j] * 8].Draw(Pos = Position + new Vector2(14f + j * 16f, 3f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j] * 8].Draw(Pos = Position + new Vector2(15f + j * 16f, 3f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(16f + j * 16f, 4f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(17f + j * 16f, 4f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(18f + j * 16f, 5f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(19f + j * 16f, 5f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(20f + j * 16f, 6f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(21f + j * 16f, 6f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(22f + j * 16f, 7f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(23f + j * 16f, 7f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            for (int k = 1; k <= SlopeHeight - 1; k++)
                            {
                                if (k > 1)
                                {
                                    if (TilesTop != "Small Edge" || k > 2)
                                    {
                                        BaseTextures[5, 0 + variationInner[k]].Draw(Pos = Position + new Vector2(-16 + k * 8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    }
                                    BaseTextures[5, 0 + variationInner[k + 1]].Draw(Pos = Position + new Vector2(-8 + k * 8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[k + 2]].Draw(Pos = Position + new Vector2(k * 8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int l = 0; l <= SlopeHeight - 1; l++)
                            {
                                SlopeTextures[0 + variation[l] * 8].Draw(Pos = Position + new Vector2(8f + l * 8f, l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[l] * 8].Draw(Pos = Position + new Vector2(9f + l * 8f, 1f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[l] * 8].Draw(Pos = Position + new Vector2(10f + l * 8f, 2f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[l] * 8].Draw(Pos = Position + new Vector2(11f + l * 8f, 3f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[l] * 8].Draw(Pos = Position + new Vector2(12f + l * 8f, 4f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[l] * 8].Draw(Pos = Position + new Vector2(13f + l * 8f, 5f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[l] * 8].Draw(Pos = Position + new Vector2(14f + l * 8f, 6f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[l] * 8].Draw(Pos = Position + new Vector2(15f + l * 8f, 7f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Bottom Tiles

                        if (Gentle)
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[14]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[15]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[11], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[16]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[17]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[12], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[18]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[13], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[14], 1].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[15], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[19]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[20]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[16], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[17], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[18], 1].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[19], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[21]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[22]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[20], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[23]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[24]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[21], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[25]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[26]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[22], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[27]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[23], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[24], 1].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[25], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[28]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[26], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[27], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[28], 1].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[29], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[29]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[30], 14].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                    }
                    else if (Side == "Right")
                    {
                        // Top tiles

                        if (TilesTop == "Horizontal")
                        {
                            BaseTextures[0 + variation[0], 0].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[1], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[0]].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[1]].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[1], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Horizontal Corner")
                        {
                            BaseTextures[4, 3].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[2], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[2]].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[3]].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[2], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Vertical")
                        {
                            BaseTextures[4, 3].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[4]].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge")
                        {
                            BaseTextures[0 + variation[3], 12].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[4], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[5], 3].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[5]].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge Corner")
                        {
                            BaseTextures[0 + variation[6], 12].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[7], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 1].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[6]].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge")
                        {
                            BaseTextures[0 + variation[8], 12].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[9], 3].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge Corner")
                        {
                            BaseTextures[0 + variation[10], 12].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        // Special case for bottom tiles

                        if (Gentle && TilesBottom != "Small Edge" && TilesBottom != "Small Edge Corner")
                        {
                            BaseTextures[5, 0 + variationInner[7]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        if (TilesBottom == "Horizontal")
                        {
                            if (Gentle)
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[8]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[9]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[10]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);

                            }
                            else
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[11]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[12]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[13]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Slope Tiles

                        if (Gentle)
                        {
                            for (int i = 1; i <= SlopeHeight - 1; i++)
                            {
                                if (i > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[i]].Draw(Pos = Position + new Vector2(40f + i * -16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[i + 1]].Draw(Pos = Position + new Vector2(32f + i * -16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[i + 2]].Draw(Pos = Position + new Vector2(24f + i * -16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[i + 3]].Draw(Pos = Position + new Vector2(16f + i * -16f, i * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int j = 0; j <= SlopeHeight - 1; j++)
                            {
                                SlopeTextures[7 + variation[j] * 8].Draw(Pos = Position + new Vector2(15f + j * -16f, j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j] * 8].Draw(Pos = Position + new Vector2(14f + j * -16f, j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j] * 8].Draw(Pos = Position + new Vector2(13f + j * -16f, 1f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j] * 8].Draw(Pos = Position + new Vector2(12f + j * -16f, 1f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j] * 8].Draw(Pos = Position + new Vector2(11f + j * -16f, 2f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j] * 8].Draw(Pos = Position + new Vector2(10f + j * -16f, 2f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j] * 8].Draw(Pos = Position + new Vector2(9f + j * -16f, 3f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j] * 8].Draw(Pos = Position + new Vector2(8f + j * -16f, 3f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(7f + j * -16f, 4f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(6f + j * -16f, 4f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(5f + j * -16f, 5f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(4f + j * -16f, 5f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(3f + j * -16f, 6f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(2f + j * -16f, 6f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(1f + j * -16f, 7f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(j * -16f, 7f + j * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            for (int k = 1; k <= SlopeHeight - 1; k++)
                            {
                                if (k > 1)
                                {
                                    if (TilesTop != "Small Edge" || k > 2)
                                    {
                                        BaseTextures[5, 0 + variationInner[k]].Draw(Pos = Position + new Vector2(32f + k * -8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    }
                                    BaseTextures[5, 0 + variationInner[k + 1]].Draw(Pos = Position + new Vector2(24f + k * -8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[k + 2]].Draw(Pos = Position + new Vector2(16f + k * -8f, k * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int l = 0; l <= SlopeHeight - 1; l++)
                            {
                                SlopeTextures[7 + variation[l] * 8].Draw(Pos = Position + new Vector2(15f + l * -8f, l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[l] * 8].Draw(Pos = Position + new Vector2(14f + l * -8f, 1f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[l] * 8].Draw(Pos = Position + new Vector2(13f + l * -8f, 2f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[l] * 8].Draw(Pos = Position + new Vector2(12f + l * -8f, 3f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[l] * 8].Draw(Pos = Position + new Vector2(11f + l * -8f, 4f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[l] * 8].Draw(Pos = Position + new Vector2(10f + l * -8f, 5f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[l] * 8].Draw(Pos = Position + new Vector2(9f + l * -8f, 6f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[l] * 8].Draw(Pos = Position + new Vector2(8f + l * -8f, 7f + l * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Bottom Tiles

                        if (Gentle)
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[14]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[15]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[11], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[16]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[17]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[12], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[18]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[13], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[14], 1].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[15], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[19]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[20]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[16], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[17], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[18], 1].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[19], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[21]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[22]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[20], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[23]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[24]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[21], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[25]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[26]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[22], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[27]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[23], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[24], 1].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[25], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[28]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[26], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[27], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[28], 1].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[29], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[29]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * 8f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[30], 13].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                    }
                }
                else
                {
                    if (Side == "Left")
                    {
                        // Top tiles

                        if (TilesTop == "Horizontal")
                        {
                            BaseTextures[0 + variation[0], 1].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[1], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[0]].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[1]].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[1], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Horizontal Corner")
                        {
                            BaseTextures[4, 0].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[2], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[2]].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[3]].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[2], 0].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Vertical")
                        {
                            BaseTextures[4, 0].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[4]].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge")
                        {
                            BaseTextures[0 + variation[3], 13].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[4], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[5], 2].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[5]].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge Corner")
                        {
                            BaseTextures[0 + variation[6], 13].Draw(Pos = Position + new Vector2(-8f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[7], 1].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 2].Draw(Pos = Position + new Vector2(-8f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[6]].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge")
                        {
                            BaseTextures[0 + variation[8], 13].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[9], 2].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge Corner")
                        {
                            BaseTextures[0 + variation[10], 13].Draw(Pos = Position + new Vector2(0f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 2].Draw(Pos = Position + new Vector2(0f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        // Special case for bottom tiles

                        if (Gentle && TilesBottom != "Small Edge" && TilesBottom != "Small Edge Corner")
                        {
                            BaseTextures[5, 0 + variationInner[7]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        if (TilesBottom == "Horizontal")
                        {
                            if (Gentle)
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[8]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[9]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[10]].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[11]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[12]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[13]].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Slope Tiles

                        if (Gentle)
                        {
                            for (int i = 1; i <= SlopeHeight - 1; i++)
                            {
                                if (i > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[i]].Draw(Pos = Position + new Vector2(-24f + i * 16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[i + 1]].Draw(Pos = Position + new Vector2(-16f + i * 16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[i + 2]].Draw(Pos = Position + new Vector2(-8f + i * 16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[i + 3]].Draw(Pos = Position + new Vector2(i * 16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int j = 0; j <= SlopeHeight - 1; j++)
                            {
                                SlopeTextures[0 + variation[j] * 8].Draw(Pos = Position + new Vector2(8f + j * 16f, j * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j] * 8].Draw(Pos = Position + new Vector2(9f + j * 16f, j * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j] * 8].Draw(Pos = Position + new Vector2(10f + j * 16f, 1f + j * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j] * 8].Draw(Pos = Position + new Vector2(11f + j * 16f, 1f + j * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j] * 8].Draw(Pos = Position + new Vector2(12f + j * 16f, 2f + j * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j] * 8].Draw(Pos = Position + new Vector2(13f + j * 16f, 2f + j * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j] * 8].Draw(Pos = Position + new Vector2(14f + j * 16f, 3f + j * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j] * 8].Draw(Pos = Position + new Vector2(15f + j * 16f, 3f + j * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(16f + j * 16f, 4f + j * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(17f + j * 16f, 4f + j * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(18f + j * 16f, 5f + j * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(19f + j * 16f, 5f + j * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(20f + j * 16f, 6f + j * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(21f + j * 16f, 6f + j * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(22f + j * 16f, 7f + j * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(23f + j * 16f, 7f + j * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            for (int k = 1; k <= SlopeHeight - 1; k++)
                            {
                                if (k > 1)
                                {
                                    if (TilesTop != "Small Edge" || k > 2)
                                    {
                                        BaseTextures[5, 0 + variationInner[k]].Draw(Pos = Position + new Vector2(-16 + k * 8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    }
                                    BaseTextures[5, 0 + variationInner[k + 1]].Draw(Pos = Position + new Vector2(-8 + k * 8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[k + 2]].Draw(Pos = Position + new Vector2(k * 8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int l = 0; l <= SlopeHeight - 1; l++)
                            {
                                SlopeTextures[0 + variation[l] * 8].Draw(Pos = Position + new Vector2(8f + l * 8f, l * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[l] * 8].Draw(Pos = Position + new Vector2(9f + l * 8f, 1f + l * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[l] * 8].Draw(Pos = Position + new Vector2(10f + l * 8f, 2f + l * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[l] * 8].Draw(Pos = Position + new Vector2(11f + l * 8f, 3f + l * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[l] * 8].Draw(Pos = Position + new Vector2(12f + l * 8f, 4f + l * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[l] * 8].Draw(Pos = Position + new Vector2(13f + l * 8f, 5f + l * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[l] * 8].Draw(Pos = Position + new Vector2(14f + l * 8f, 6f + l * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[l] * 8].Draw(Pos = Position + new Vector2(15f + l * 8f, 7f + l * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Bottom Tiles

                        if (Gentle)
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[14]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[15]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[11], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[16]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[17]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[12], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[18]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[13], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[14], 0].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[15], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[19]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[20]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[16], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[17], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[18], 0].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[19], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[21]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[22]].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 16 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[20], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 16, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[23]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[24]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[21], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[25]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[26]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[22], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[27]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 0].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[23], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[24], 0].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[25], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[28]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[26], 3].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[27], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[28], 0].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[29], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[29]].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[4, 1].Draw(Pos = Position + new Vector2(SlopeHeight * 8 - 8f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[30], 12].Draw(Pos = Position + new Vector2(SlopeHeight * 8, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                    }
                    else if (Side == "Right")
                    {
                        // Top tiles

                        if (TilesTop == "Horizontal")
                        {
                            BaseTextures[0 + variation[0], 1].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[1], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[0]].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[1]].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            /*if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[1], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }*/
                        }
                        else if (TilesTop == "Horizontal Corner")
                        {
                            BaseTextures[4, 2].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[2], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            if (SlopeHeight != 1 || TilesBottom != "Small Edge")
                            {
                                BaseTextures[5, 0 + variationInner[2]].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[3]].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                            if (SlopeHeight == 1 && TilesBottom == "Small Edge" && Gentle)
                            {
                                BaseTextures[0 + variation[2], 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                            }
                        }
                        else if (TilesTop == "Vertical")
                        {
                            BaseTextures[4, 2].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[4]].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge")
                        {
                            BaseTextures[0 + variation[3], 14].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[4], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[5], 3].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[5]].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Edge Corner")
                        {
                            BaseTextures[0 + variation[6], 14].Draw(Pos = Position + new Vector2(24f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[7], 1].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 0].Draw(Pos = Position + new Vector2(24f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[5, 0 + variationInner[6]].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge")
                        {
                            BaseTextures[0 + variation[8], 14].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[0 + variation[9], 3].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }
                        else if (TilesTop == "Small Edge Corner")
                        {
                            BaseTextures[0 + variation[10], 14].Draw(Pos = Position + new Vector2(16f, 8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            BaseTextures[4, 0].Draw(Pos = Position + new Vector2(16f, 0f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        // Special case for bottom tiles

                        if (Gentle && TilesBottom != "Small Edge" && TilesBottom != "Small Edge Corner")
                        {
                            BaseTextures[5, 0 + variationInner[7]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                        }

                        if (TilesBottom == "Horizontal")
                        {
                            if (Gentle)
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[8]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[9]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[10]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);

                            }
                            else
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[11]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[12]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[13]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Slope Tiles

                        if (Gentle)
                        {
                            for (int i = 1; i <= SlopeHeight - 1; i++)
                            {
                                if (i > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[i]].Draw(Pos = Position + new Vector2(40f + i * -16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[i + 1]].Draw(Pos = Position + new Vector2(32f + i * -16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[i + 2]].Draw(Pos = Position + new Vector2(24f + i * -16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[i + 3]].Draw(Pos = Position + new Vector2(16f + i * -16f, i * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int j = 0; j <= SlopeHeight - 1; j++)
                            {
                                SlopeTextures[7 + variation[j] * 8].Draw(Pos = Position + new Vector2(15f + j * -16f, j * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j] * 8].Draw(Pos = Position + new Vector2(14f + j * -16f, j * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j] * 8].Draw(Pos = Position + new Vector2(13f + j * -16f, 1f + j * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j] * 8].Draw(Pos = Position + new Vector2(12f + j * -16f, 1f + j * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j] * 8].Draw(Pos = Position + new Vector2(11f + j * -16f, 2f + j * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j] * 8].Draw(Pos = Position + new Vector2(10f + j * -16f, 2f + j * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j] * 8].Draw(Pos = Position + new Vector2(9f + j * -16f, 3f + j * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j] * 8].Draw(Pos = Position + new Vector2(8f + j * -16f, 3f + j * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[7 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(7f + j * -16f, 4f + j * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(6f + j * -16f, 4f + j * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(5f + j * -16f, 5f + j * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(4f + j * -16f, 5f + j * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(3f + j * -16f, 6f + j * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(2f + j * -16f, 6f + j * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(1f + j * -16f, 7f + j * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[j + 1] * 8].Draw(Pos = Position + new Vector2(j * -16f, 7f + j * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            for (int k = 1; k <= SlopeHeight - 1; k++)
                            {
                                if (k > 1)
                                {
                                    if (TilesTop != "Small Edge" || k > 2)
                                    {
                                        BaseTextures[5, 0 + variationInner[k]].Draw(Pos = Position + new Vector2(32f + k * -8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                    }
                                    BaseTextures[5, 0 + variationInner[k + 1]].Draw(Pos = Position + new Vector2(24f + k * -8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[k + 2]].Draw(Pos = Position + new Vector2(16f + k * -8f, k * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            for (int l = 0; l <= SlopeHeight - 1; l++)
                            {
                                SlopeTextures[7 + variation[l] * 8].Draw(Pos = Position + new Vector2(15f + l * -8f, l * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[6 + variation[l] * 8].Draw(Pos = Position + new Vector2(14f + l * -8f, 1f + l * -8f + 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[5 + variation[l] * 8].Draw(Pos = Position + new Vector2(13f + l * -8f, 2f + l * -8f + 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[4 + variation[l] * 8].Draw(Pos = Position + new Vector2(12f + l * -8f, 3f + l * -8f + 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[3 + variation[l] * 8].Draw(Pos = Position + new Vector2(11f + l * -8f, 4f + l * -8f + 0), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[2 + variation[l] * 8].Draw(Pos = Position + new Vector2(10f + l * -8f, 5f + l * -8f - 2), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[1 + variation[l] * 8].Draw(Pos = Position + new Vector2(9f + l * -8f, 6f + l * -8f - 4), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                SlopeTextures[0 + variation[l] * 8].Draw(Pos = Position + new Vector2(8f + l * -8f, 7f + l * -8f - 6), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }

                        // Bottom Tiles

                        if (Gentle)
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[14]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[15]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[11], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[16]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[17]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[12], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[18]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[13], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[14], 0].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[15], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[19]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[20]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[16], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[17], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[18], 0].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[19], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[21]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 40f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[5, 0 + variationInner[22]].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[20], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -16 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                        else
                        {
                            if (TilesBottom == "Vertical")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[23]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[24]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[21], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Vertical Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[25]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                    BaseTextures[5, 0 + variationInner[26]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[22], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[5, 0 + variationInner[27]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge")
                            {
                                BaseTextures[0 + variation[23], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[24], 0].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[25], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[28]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[0 + variation[26], 2].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[27], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge")
                            {
                                BaseTextures[0 + variation[28], 0].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[29], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                            else if (TilesBottom == "Small Edge Corner")
                            {
                                if (SlopeHeight > 1)
                                {
                                    BaseTextures[5, 0 + variationInner[29]].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 32f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow ? GetHue(Pos) : Color.White);
                                }
                                BaseTextures[4, 3].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 24f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                                BaseTextures[0 + variation[30], 11].Draw(Pos = Position + new Vector2(SlopeHeight * -8 + 16f, SlopeHeight * -8f + 8), Vector2.Zero, Rainbow? GetHue(Pos) : Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}
