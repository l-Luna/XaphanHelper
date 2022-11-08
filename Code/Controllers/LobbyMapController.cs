using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/LobbyMapController")]
    class LobbyMapController : Entity
    {
        public MTexture CustomImage;

        public string Directory;

        public int CustomImagesTilesSizeX;

        public int CustomImagesTilesSizeY;

        public Vector2 PlayerPosition;

        public LobbyMapController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = Tags.Persistent;
            Directory = data.Attr("directory");
            CustomImagesTilesSizeX = 4;
            CustomImagesTilesSizeY = 4;
            CustomImage = GFX.Gui[Directory];
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            GenerateLobbyTiles(SceneAs<Level>().Session.Area.ID, CustomImage);
        }

        public override void Update()
        {
            base.Update();
            if (CustomImage!= null && SceneAs<Level>().Tracker.GetEntity<WarpScreen>() == null)
            {
                Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
                if (player != null)
                {
                    if (!Scene.OnInterval(0.2f) || player.StateMachine.State == Player.StDummy)
                    {
                        return;
                    }
                }
                Level level = SceneAs<Level>();
                string Prefix = level.Session.Area.GetLevelSet();
                int chapterIndex = level.Session.Area.ChapterIndex == -1 ? 0 : level.Session.Area.ChapterIndex;
                if (player != null && !level.Paused && !level.Transitioning)
                {
                    PlayerPosition = new Vector2(Math.Min((float)Math.Floor((player.Center.X - level.Bounds.X) / 8f), (float)Math.Round(level.Bounds.Width / 8f, MidpointRounding.AwayFromZero) - 1), Math.Min((float)Math.Floor((player.Center.Y - level.Bounds.Y) / 8f), (float)Math.Round(level.Bounds.Height / 8f, MidpointRounding.AwayFromZero) + 1));
                    if (PlayerPosition.X >= 0 && PlayerPosition.X < Math.Floor((float)CustomImage.Width / CustomImagesTilesSizeX) && PlayerPosition.Y >= 0 && PlayerPosition.Y < Math.Floor((float)CustomImage.Height / CustomImagesTilesSizeY) && !XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles2.Contains(new Vector2(PlayerPosition.X, PlayerPosition.Y)))
                    {
                        List<Vector2> TmpGeneratedVisitedLobbyMapTiles = new List<Vector2>();
                        List<Vector2> Tmp2GeneratedVisitedLobbyMapTiles = new List<Vector2>();
                        List<Vector2> Tmp3GeneratedVisitedLobbyMapTiles = new List<Vector2>();
                        List<Vector2> Tmp4GeneratedVisitedLobbyMapTiles = new List<Vector2>();
                        XaphanModule.ModSaveData.VisitedLobbyMapTiles.Add(Prefix + "/Ch" + chapterIndex + "/" + level.Session.Level + "/" + PlayerPosition.X + "-" + PlayerPosition.Y);
                        Circle circle = new Circle(15, PlayerPosition.X, PlayerPosition.Y);
                        for (int i = Math.Max(0, (int)(PlayerPosition.X - circle.Radius)); i < Math.Min((int)(PlayerPosition.X + circle.Radius), CustomImage.Width / 4); i++)
                        {
                            for (int j = Math.Max(0, (int)(PlayerPosition.Y - circle.Radius)); j < Math.Min((int)(PlayerPosition.Y + circle.Radius), CustomImage.Height / 4); j++)
                            {
                                if (Math.Sqrt(Math.Pow(i - circle.Center.X, 2) + Math.Pow((circle.Center.Y - j), 2)) < circle.Radius)
                                {
                                    TmpGeneratedVisitedLobbyMapTiles.Add(new Vector2(i, j));
                                }
                                if (Math.Sqrt(Math.Pow(i - circle.Center.X, 2) + Math.Pow((circle.Center.Y - j), 2)) < circle.Radius - 10)
                                {
                                    Tmp3GeneratedVisitedLobbyMapTiles.Add(new Vector2(i, j));
                                }
                            }
                        }
                        Tmp2GeneratedVisitedLobbyMapTiles.AddRange(XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles);
                        Tmp2GeneratedVisitedLobbyMapTiles.AddRange(TmpGeneratedVisitedLobbyMapTiles.Distinct().ToList());
                        XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles = Tmp2GeneratedVisitedLobbyMapTiles.Distinct().ToList();

                        Tmp4GeneratedVisitedLobbyMapTiles.AddRange(XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles2);
                        Tmp4GeneratedVisitedLobbyMapTiles.AddRange(Tmp3GeneratedVisitedLobbyMapTiles.Distinct().ToList());
                        XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles2 = Tmp4GeneratedVisitedLobbyMapTiles.Distinct().ToList();
                    }
                }
            }
        }

        public static void GenerateLobbyTiles(int areaId, MTexture mapTexture)
        {
            AreaKey area = new(areaId);
            string Prefix = area.LevelSet;
            int chapterIndex = area.ChapterIndex == -1 ? 0 : area.ChapterIndex;

            if (XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles.Count == 0)
            {
                List<Vector2> TmpGeneratedVisitedLobbyMapTiles = new List<Vector2>();
                foreach (string tile in XaphanModule.ModSaveData.VisitedLobbyMapTiles)
                {
                    string[] str = tile.Split('/');
                    if (Prefix == str[0] + "/" + str[1] && str[2] == "Ch" + chapterIndex)
                    {
                        string[] str2 = str[4].Split('-');
                        float cordX = float.Parse(str2[0]);
                        float cordY = float.Parse(str2[1]);
                        Rectangle rectangle = new Rectangle((int)cordX - 15, (int)cordY - 15, 30, 30);
                        TmpGeneratedVisitedLobbyMapTiles.Add(new Vector2(cordX, cordY));
                        for (int i = Math.Max(0, (int)(cordX - rectangle.Width / 2)); i < Math.Min((int)(cordX + rectangle.Width / 2), mapTexture.Width / 4); i++)
                        {
                            for (int j = Math.Max(0, (int)(cordY - rectangle.Height / 2)); j < Math.Min((int)(cordY + rectangle.Height / 2), mapTexture.Height / 4); j++)
                            {
                                if (rectangle.Contains(i, j))
                                {
                                    if ((i >= rectangle.X + 11 && i <= rectangle.X + 18 && j == rectangle.Y) || (i >= rectangle.X + 8 && i <= rectangle.X + 21 && j == rectangle.Y + 1) || (i >= rectangle.X + 7 && i <= rectangle.X + 22 && j == rectangle.Y + 2) || (i >= rectangle.X + 5 && i <= rectangle.X + 24 && j == rectangle.Y + 3) ||
                                        (i >= rectangle.X + 4 && i <= rectangle.X + 25 && j == rectangle.Y + 4) || (i >= rectangle.X + 3 && i <= rectangle.X + 26 && j >= rectangle.Y + 5 && j <= rectangle.Y + 6) || (i >= rectangle.X + 2 && i <= rectangle.X + 27 && j == rectangle.Y + 7) || (i >= rectangle.X + 1 && i <= rectangle.X + 28 && j >= rectangle.Y + 8 && j <= rectangle.Y + 10) ||
                                        (i >= rectangle.X && i <= rectangle.X + 29 && j >= rectangle.Y + 11 && j <= rectangle.Y + 18) || (i >= rectangle.X + 1 && i <= rectangle.X + 28 && j >= rectangle.Y + 19 && j <= rectangle.Y + 21) || (i >= rectangle.X + 2 && i <= rectangle.X + 27 && j == rectangle.Y + 22) || (i >= rectangle.X + 3 && i <= rectangle.X + 26 && j >= rectangle.Y + 23 && j <= rectangle.Y + 24) ||
                                        (i >= rectangle.X + 4 && i <= rectangle.X + 25 && j == rectangle.Y + 25) || (i >= rectangle.X + 5 && i <= rectangle.X + 24 && j == rectangle.Y + 26) || (i >= rectangle.X + 7 && i <= rectangle.X + 22 && j == rectangle.Y + 27) || (i >= rectangle.X + 8 && i <= rectangle.X + 21 && j == rectangle.Y + 28) || (i >= rectangle.X + 11 && i <= rectangle.X + 18 && j == rectangle.Y + 29))
                                    {
                                        TmpGeneratedVisitedLobbyMapTiles.Add(new Vector2(i, j));
                                    }
                                }
                            }
                        }
                    }
                }
                XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles = TmpGeneratedVisitedLobbyMapTiles.Distinct().ToList();
            }
        }
    }
}
