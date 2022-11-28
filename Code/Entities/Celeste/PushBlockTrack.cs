using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/PushBlockTrack")]
    class PushBlockTrack : Entity
    {
        public string directory;

        private float alpha = 0f;

        Sprite Sprite;

        Sprite GlowSprite;

        Dictionary<Vector2, string> tiles = new();

        Dictionary<Vector2, Vector2> tilesSpritePos = new();

        public PushBlockTrack(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, data.Height, 0f, 0f);
            directory = data.Attr("directory");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/PushBlock";
            }
            Sprite = new Sprite(GFX.Game, directory + "/");
            Sprite.AddLoop("track", "track", 0.08f);
            Sprite.Play("track");
            GlowSprite = new Sprite(GFX.Game, directory + "/");
            GlowSprite.AddLoop("glowTrack", "glowTrack", 0.08f);
            GlowSprite.Play("glowTrack");
            Depth = 8999;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            for (int i = 0; i < Width / 8; i++)
            {
                for (int j = 0; j < Height / 8; j++)
                {
                    bool N = false;
                    bool S = false;
                    bool E = false;
                    bool W = false;
                    if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8, (int)Y + j * 8 - 8, 1, 1)))
                    {
                        N = true;
                    }
                    if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8, (int)Y + j * 8 + 8, 1, 1)))
                    {
                        S = true;
                    }
                    if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 + 8, (int)Y + j * 8, 1, 1)))
                    {
                        E = true;
                    }
                    if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 - 8, (int)Y + j * 8, 1, 1)))
                    {
                        W = true;
                    }
                    if (N && S && E && W)
                    {
                        bool TL = false;
                        bool TR = false;
                        bool BL = false;
                        bool BR = false;
                        if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 - 8, (int)Y + j * 8 - 8, 1, 1)))
                        {
                            TL = true;
                        }
                        if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 + 8, (int)Y + j * 8 - 8, 1, 1)))
                        {
                            TR = true;
                        }
                        if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 - 8, (int)Y + j * 8 + 8, 1, 1)))
                        {
                            BL = true;
                        }
                        if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X + i * 8 + 8, (int)Y + j * 8 + 8, 1, 1)))
                        {
                            BR = true;
                        }

                        tiles.Add(new Vector2(i, j), "NSEW" + (!TL && TR && BL && BR ? "-TL" : (!TR && TL && BL && BR ? "-TR" : (!BL && TL && TR && BR ? "-BL" : (!BR && TL && TR && BL ? "-BR" : "")))));
                    }
                    else
                    {
                        tiles.Add(new Vector2(i, j), (N ? "N" : "") + (S ? "S" : "") + (E ? "E" : "") + (W ? "W" : ""));
                    }
                }
            }
            GetSpritePos();
        }

        public override void Update()
        {
            base.Update();
            alpha += Engine.DeltaTime * 4f;
        }

        public void GetSpritePos()
        {
            foreach (KeyValuePair<Vector2, string> tile in tiles)
            {
                switch (tile.Value)
                {
                    case "SE":
                        tilesSpritePos.Add(tile.Key, new Vector2(0, 0));
                        break;
                    case "SW":
                        tilesSpritePos.Add(tile.Key, new Vector2(2, 0));
                        break;
                    case "NE":
                        tilesSpritePos.Add(tile.Key, new Vector2(0, 2));
                        break;
                    case "NW":
                        tilesSpritePos.Add(tile.Key, new Vector2(2, 2));
                        break;
                    case "NSE":
                        tilesSpritePos.Add(tile.Key, new Vector2(0, 1));
                        break;
                    case "SEW":
                        tilesSpritePos.Add(tile.Key, new Vector2(1, 0));
                        break;
                    case "NEW":
                        tilesSpritePos.Add(tile.Key, new Vector2(1, 2));
                        break;
                    case "NSW":
                        tilesSpritePos.Add(tile.Key, new Vector2(2, 1));
                        break;
                    case "NSEW":
                        tilesSpritePos.Add(tile.Key, new Vector2(1, 1));
                        break;
                    case "NSEW-TL":
                        tilesSpritePos.Add(tile.Key, new Vector2(3, 1));
                        break;
                    case "NSEW-TR":
                        tilesSpritePos.Add(tile.Key, new Vector2(3, 0));
                        break;
                    case "NSEW-BL":
                        tilesSpritePos.Add(tile.Key, new Vector2(3, 3));
                        break;
                    case "NSEW-BR":
                        tilesSpritePos.Add(tile.Key, new Vector2(3, 2));
                        break;
                }
            }
        }

        public override void Render()
        {
            base.Render();
            for (int i = 0; i < Width / 8; i++)
            {
                for (int j = 0; j < Height / 8; j++)
                {
                    Sprite.RenderPosition = GlowSprite.RenderPosition = Position + new Vector2(i * 8, j * 8);
                    Sprite.DrawSubrect(Vector2.Zero, new Rectangle((int)tilesSpritePos[new Vector2(i, j)].X * 8, (int)tilesSpritePos[new Vector2(i, j)].Y * 8, 8, 8));
                    GlowSprite.Color = Color.White * (0.9f * (0.9f + ((float)Math.Sin(alpha) + 1f) * 0.125f));
                    GlowSprite.DrawSubrect(Vector2.Zero, new Rectangle((int)tilesSpritePos[new Vector2(i, j)].X * 8, (int)tilesSpritePos[new Vector2(i, j)].Y * 8, 8, 8));
                }
            }
        }
    }
}