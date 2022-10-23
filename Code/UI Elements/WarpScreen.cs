using Monocle;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.Cutscenes;
using System.Linq;
using System;
using Celeste.Mod.XaphanHelper.Entities;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Data;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class WarpScreen : Entity
    {
        protected XaphanModuleSettings XaphanSettings => XaphanModule.Settings;

        private Level level;

        private MapDisplay mapDisplay;

        private LobbyMapDisplay lobbyMapDisplay;

        public bool ShowUI;

        public WarpMenu menu;

        public WarpDestinationDisplay destDisplay;

        private string currentRoom;

        private string DestinationRoom;

        private int DestinationIndex;

        private string lastGeneratedRoom;

        private int lastGeneratedChapter;

        public int currentChapter;

        public int chapterIndex;

        public int firstChapterIndex = 100;

        public int lastChapterIndex = 0;

        public HashSet<int> UnlockedChapters = new HashSet<int>();

        public bool UpdatingMenu;

        private bool Teleporting;

        public string Title;

        public BigTitle BigTitle;

        public MTexture mTexture;

        public float alpha;

        private string ConfirmSfx;

        private string WipeType;

        private float WipeDuration;

        public bool useIngameMap;

        private int indexMin = 1000000;

        private int indexMax = 1;

        private int Totalwarps = 0;

        private Wiggler closeWiggle;

        private Wiggler zoomWiggle;

        private Wiggler confirmWiggle;

        private Wiggler destinationWiggle;

        private Wiggler lobbyWiggle;

        private Wiggler progressWiggle;

        private float closeWiggleDelay;

        private float zoomWiggleDelay;

        private float confirmWiggleDelay;

        private float destinationWiggleDelay;

        private float lobbyWiggleDelay;

        private float progressWiggleDelay;

        private NormalText Message;

        public bool UseLobbymap;

        public Image playerIcon;

        public Image playerIconHair;

        public bool currentPositionIndicator;

        public Coroutine LobbyCoroutine = new Coroutine();

        public bool FromWarp;

        private MapProgressDisplay MapProgressDisplay;

        public WarpScreen(Level level, string confirmSfx, string wipeType, float wipeDuration, bool useLobbymap = false, bool fromWarp = true)
        {
            Tag = Tags.HUD;
            alpha = 1f;
            mTexture = GFX.Gui["towerarrow"];
            this.level = level;
            ConfirmSfx = confirmSfx;
            WipeType = wipeType;
            WipeDuration = wipeDuration;
            playerIcon = new Image(GFX.Gui["maps/player"]);
            playerIconHair = new Image(GFX.Gui["maps/player_hair"]);
            UseLobbymap = useLobbymap;
            FromWarp = fromWarp;
            Add(progressWiggle = Wiggler.Create(0.4f, 4f));
            if (UseLobbymap)
            {
                Add(closeWiggle = Wiggler.Create(0.4f, 4f));
                Add(zoomWiggle = Wiggler.Create(0.4f, 4f));
                Add(confirmWiggle = Wiggler.Create(0.4f, 4f));
                Add(destinationWiggle = Wiggler.Create(0.4f, 4f));
                Add(lobbyWiggle = Wiggler.Create(0.4f, 4f));
            }
            Depth = -10002;
        }

        public int currentLobbyIndex;

        public int lobbyIndex;

        public int currentLobbyGlobalID;

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = Scene as Level;
            Player player = Scene.Tracker.GetEntity<Player>();
            player.StateMachine.State = Player.StDummy;
            chapterIndex = level.Session.Area.ChapterIndex == -1 ? 0 : level.Session.Area.ChapterIndex;
            currentChapter = chapterIndex;
            level.PauseLock = true;
            level.Session.SetFlag("Map_Opened", true);
            Audio.Play("event:/ui/game/pause");
            currentRoom = level.Session.Level;
            useIngameMapCheck(level);
            if (UseLobbymap)
            {
                LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
                currentLobbyIndex = LobbyMapController.lobbyIndex;
                lobbyIndex = LobbyMapController.lobbyIndex;
                currentLobbyGlobalID = level.Session.Area.ID;
                useIngameMap = false;
            }
            List<string> UnlockedWarps = new List<string>();
            if (!XaphanSettings.SpeedrunMode && !XaphanModule.PlayerHasGolden)
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).UnlockedWarps;
            }
            else
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpeedrunModeUnlockedWarps;
            }
            foreach (string warp in UnlockedWarps)
            {
                string prefix = level.Session.Area.GetLevelSet();
                if (warp.Contains(prefix))
                {
                    Totalwarps++;
                }
            }
            if ((useIngameMap || UseLobbymap) && Totalwarps > 1)
            {
                Add(new Coroutine(TransitionToMap(level)));
            }
            else if (Totalwarps > 1)
            {
                level.FormationBackdrop.Display = true;
                ShowUI = true;
                Add(new Coroutine(MenuRoutine(level, currentRoom)));
            }
            else
            {
                level.FormationBackdrop.Display = true;
                ShowUI = true;
                Add(new Coroutine(NoWarpRoutine(player, UseLobbymap)));
            }
        }

        private IEnumerator TransitionToMap(Level level)
        {
            level.Session.SetFlag("Warp_Wipe", true);
            float duration = 0.5f;
            FadeWipe Wipe = new FadeWipe(level, false)
            {
                Duration = duration
            };
            level.Add(Wipe);
            duration = duration - 0.25f;
            while (duration > 0f)
            {
                yield return null;
                duration -= Engine.DeltaTime;
            }
            CountdownDisplay timerDisplay = level.Tracker.GetEntity<CountdownDisplay>();
            if (timerDisplay != null)
            {
                timerDisplay.StopTimer(true, false);
            }
            ShowUI = true;
            duration = 0.25f;
            Wipe = new FadeWipe(level, true)
            {
                Duration = duration
            };
            level.Session.SetFlag("Warp_Wipe", false);
            if (!UseLobbymap)
            {
                Add(new Coroutine(MenuRoutine(level, currentRoom)));
            }
            else
            {
                Add(LobbyCoroutine = new Coroutine(LobbyRoutine(level, currentRoom, lobbyIndex, currentLobbyGlobalID)));
            }
        }

        private IEnumerator TransitionToGame(Level level)
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            float duration = 0.5f;
            FadeWipe Wipe = new FadeWipe(level, false)
            {
                Duration = duration
            };
            level.Add(Wipe);
            duration = duration - 0.25f;
            while (duration > 0f)
            {
                yield return null;
                duration -= Engine.DeltaTime;
            }
            CountdownDisplay timerDisplay = level.Tracker.GetEntity<CountdownDisplay>();
            if (timerDisplay != null)
            {
                timerDisplay.StopTimer(false, true);
            }
            ShowUI = false;
            if (BigTitle != null)
            {
                BigTitle.RemoveSelf();
            }
            duration = 0.25f;
            Wipe = new FadeWipe(level, true)
            {
                Duration = duration
            };
            if (!UseLobbymap)
            {
                Add(new Coroutine(CloseMap()));
            }
            else
            {
                Add(new Coroutine(CloseLobbyMap()));
            }
        }

        private IEnumerator MapRoutine(Level level, string originRoom)
        {
            Scene.Add(mapDisplay = new MapDisplay(level, "warp")
            {
                currentRoom = originRoom
        });
            yield return mapDisplay.GenerateMap();
            if (mapDisplay.InGameMapControllerData.ShowProgress != "Never")
            {
                if (MapProgressDisplay != null)
                {
                    MapProgressDisplay.RemoveSelf();
                }
                level.Add(MapProgressDisplay = new MapProgressDisplay(new Vector2(mapDisplay.Grid.X + 18f, mapDisplay.Grid.Y), level, mapDisplay.InGameMapControllerData, mapDisplay.SubAreaControllerData, mapDisplay.RoomControllerData, mapDisplay.TilesControllerData, mapDisplay.EntitiesData, mapDisplay.chapterIndex, mapDisplay.currentRoom));
            }
        }

        private IEnumerator MenuRoutine(Level level, string currentRoom)
        {
            if (string.IsNullOrEmpty(GetSpecifiedMapName()))
            {
                Title = AreaData.Get(level.Session.Area.ID - currentChapter + chapterIndex).SID;
            }
            else
            {
                Title = GetSpecifiedMapName();
            }
            Scene.Add(BigTitle = new BigTitle(Title, new Vector2(960, 80)));
            Player player = level.Tracker.GetEntity<Player>();
            menu = new WarpMenu(ConfirmSfx);
            menu.Position = new Vector2(useIngameMap ? 1520f : Engine.Width / 2f, Engine.Height / 2f + 98);
            List<string> UnlockedWarps = new List<string>();
            if (!XaphanSettings.SpeedrunMode && !XaphanModule.PlayerHasGolden)
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).UnlockedWarps;
            }
            else
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpeedrunModeUnlockedWarps;
            }
            UnlockedWarps.Sort();
            if (currentChapter == chapterIndex)
            {
                menu.Add(new WarpMenu.Button(currentRoom, Dialog.Clean("XaphanHelper_Warp_Stay"), FromWarp ? level.Tracker.GetNearestEntity<WarpStation>(player.Position).index : 1000000).Pressed(delegate
                {
                    DestinationRoom = "";
                }));
            }
            foreach (string warp in UnlockedWarps)
            {
                string prefix = level.Session.Area.GetLevelSet();
                string[] str = warp.Split('_');
                if (str[0] == prefix && str[1] == "Ch" + chapterIndex)
                {
                    int index;
                    if (str.Count() == 3)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = int.Parse(str[3]);
                    }
                    if ((str[2] != currentRoom) || (str[2] == currentRoom && index != level.Tracker.GetNearestEntity<WarpStation>(player.Position).index))
                    {
                        string warpName = SetWarpName(str[0], str[1], str[2], index);
                        menu.Add(new WarpMenu.Button(str[2], Dialog.Clean(warpName), index).Pressed(delegate
                        {
                            DestinationRoom = str[2];
                            DestinationIndex = index;
                        }));
                    }
                }
            }
            if (level.Tracker.GetEntity<WarpMenu>() != null)
            {
                WarpMenu currentWarpMenu = level.Tracker.GetEntity<WarpMenu>();
                currentWarpMenu.RemoveSelf();
            }
            else
            {
                Scene.Add(menu);
            }
            yield return 0.1f;
            while ((!Input.ESC.Pressed && !Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && player != null))
            {
                yield return null;
            }
            menu.RemoveSelf();
            BigTitle.RemoveSelf();
            alpha = 0f;
            Add(new Coroutine(TeleportRoutine(player)));
        }

        private IEnumerator NoWarpRoutine(Player player, bool useLobbymap = false)
        {
            Scene.Add(Message = new NormalText(useLobbymap ? "XaphanHelper_Warp_Lobby_None" : "XaphanHelper_Warp_None", new Vector2(Engine.Width / 2, Engine.Height / 2), Color.Gray, 1f, 1.2f));
            menu = new WarpMenu("event:/ui/game/unpause", true);
            menu.Position = new Vector2(Engine.Width / 2f, Engine.Height / 2f + 150);
            menu.Add(new WarpMenu.Button(currentRoom, Dialog.Clean("XaphanHelper_UI_close"), FromWarp ? level.Tracker.GetNearestEntity<WarpStation>(player.Position).index : 1000000).Pressed(delegate
            {
                DestinationRoom = "";
            }));
            Scene.Add(menu);
            while ((!Input.ESC.Pressed && !Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed))
            {
                yield return null;
            }
            RestaurePlayerSkin(player);
            menu.RemoveSelf();
            Message.RemoveSelf();
            level.FormationBackdrop.Display = false;
            yield return 0.1f;
            player.StateMachine.State = Player.StNormal;
            level.PauseLock = false;
            level.Session.SetFlag("Map_Opened", false);
            RemoveSelf();
        }

        private IEnumerator TeleportRoutine(Player player, int globalLobbyID = -1)
        {
            Teleporting = true;
            if (level.Tracker.GetEntity<WarpMenu>() != null)
            {
                WarpMenu currentWarpMenu = level.Tracker.GetEntity<WarpMenu>();
                currentWarpMenu.RemoveSelf();
            }
            yield return 0.1f;
            level.Session.SetFlag("Map_Opened", false);
            if (string.IsNullOrEmpty(DestinationRoom))
            {
                Audio.Play("event:/ui/game/unpause");
                
                if (useIngameMap || UseLobbymap)
                {
                    Add(new Coroutine(TransitionToGame(level)));
                }
                else
                {
                    level.FormationBackdrop.Display = false;
                    player.StateMachine.State = Player.StNormal;
                    level.PauseLock = false;
                    yield return 0.1f;
                    RemoveSelf();
                }
            }
            else
            {
                if (!UseLobbymap)
                {
                    if (chapterIndex == currentChapter)
                    {
                        if ((level = (Engine.Scene as Level)) != null)
                        {
                            if (string.IsNullOrEmpty(DestinationRoom))
                            {
                                level.Add(new MiniTextbox("XaphanHelper_room_name_empty"));
                                yield break;
                            }

                            if (level.Session.MapData.Get(DestinationRoom) == null)
                            {
                                level.Add(new MiniTextbox("XaphanHelper_room_not_exist"));
                                yield break;
                            }
                        }
                        Vector2 spawnPoint = Vector2.Zero;
                        AreaKey area = level.Session.Area;
                        MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                        foreach (LevelData levelData in MapData.Levels)
                        {
                            if (levelData.Name == DestinationRoom)
                            {
                                foreach (EntityData entity in levelData.Entities)
                                {
                                    if (entity.Name == "XaphanHelper/WarpStation" && entity.Int("index") == DestinationIndex)
                                    {
                                        spawnPoint = new Vector2(entity.Position.X, entity.Position.Y);
                                    }
                                }
                                break;
                            }
                        }
                        Scene.Add(new TeleportCutscene(player, DestinationRoom, spawnPoint, 0, 0, true, 0f, string.IsNullOrEmpty(WipeType) ? "Fade" : WipeType, WipeDuration == 0 ? 0.75f : WipeDuration));
                    }
                    else
                    {
                        int chapterOffset = chapterIndex - currentChapter;
                        int currentChapterID = level.Session.Area.ID;
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).DestinationRoom = DestinationRoom;
                        AreaKey area = level.Session.Area;
                        MapData MapData = AreaData.Areas[currentChapterID + chapterOffset].Mode[(int)area.Mode].MapData;
                        foreach (LevelData levelData in MapData.Levels)
                        {
                            if (levelData.Name == DestinationRoom)
                            {
                                foreach (EntityData entity in levelData.Entities)
                                {
                                    if (entity.Name == "XaphanHelper/WarpStation" && entity.Int("index") == DestinationIndex)
                                    {
                                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).Spawn = new Vector2(entity.Position.X, entity.Position.Y);
                                    }
                                }
                                break;
                            }
                        }
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).Wipe = string.IsNullOrEmpty(WipeType) ? "Fade" : WipeType;
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WipeDuration = WipeDuration == 0 ? 1.35f : WipeDuration;
                        switch (WipeType)
                        {
                            case "Spotlight":
                                level.Add(new SpotlightWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Curtain":
                                level.Add(new CurtainWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Mountain":
                                level.Add(new MountainWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Dream":
                                level.Add(new DreamWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Starfield":
                                level.Add(new StarfieldWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Wind":
                                level.Add(new WindWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Drop":
                                level.Add(new DropWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Fall":
                                level.Add(new FallWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "KeyDoor":
                                level.Add(new KeyDoorWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Angled":
                                level.Add(new AngledWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Heart":
                                level.Add(new HeartWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            default:
                                level.Add(new FadeWipe(level, false, () => TeleportToChapter(currentChapterID + chapterOffset))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                        }
                    }
                }
                else
                {
                    if (lobbyIndex == currentLobbyIndex)
                    {
                        if ((level = (Engine.Scene as Level)) != null)
                        {
                            if (string.IsNullOrEmpty(DestinationRoom))
                            {
                                level.Add(new MiniTextbox("XaphanHelper_room_name_empty"));
                                yield break;
                            }

                            if (level.Session.MapData.Get(DestinationRoom) == null)
                            {
                                level.Add(new MiniTextbox("XaphanHelper_room_not_exist"));
                                yield break;
                            }
                        }
                        Vector2 spawnPoint = Vector2.Zero;
                        AreaKey area = level.Session.Area;
                        MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                        foreach (LevelData levelData in MapData.Levels)
                        {
                            if (levelData.Name == DestinationRoom)
                            {
                                foreach (EntityData entity in levelData.Entities)
                                {
                                    if (entity.Name == "XaphanHelper/WarpStation" && entity.Int("index") == DestinationIndex)
                                    {
                                        spawnPoint = new Vector2(entity.Position.X, entity.Position.Y);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        Scene.Add(new TeleportCutscene(player, DestinationRoom, spawnPoint, 0, 0, true, 0f, string.IsNullOrEmpty(WipeType) ? "Fade" : WipeType, WipeDuration == 0 ? 0.75f : WipeDuration));
                    }
                    else
                    {
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).DestinationRoom = DestinationRoom;
                        AreaKey area = level.Session.Area;
                        MapData MapData = AreaData.Areas[globalLobbyID].Mode[(int)area.Mode].MapData;
                        foreach (LevelData levelData in MapData.Levels)
                        {
                            if (levelData.Name == DestinationRoom)
                            {
                                foreach (EntityData entity in levelData.Entities)
                                {
                                    if (entity.Name == "XaphanHelper/WarpStation" && entity.Int("index") == DestinationIndex)
                                    {
                                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).Spawn = new Vector2(entity.Position.X, entity.Position.Y);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).Wipe = string.IsNullOrEmpty(WipeType) ? "Fade" : WipeType;
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WipeDuration = WipeDuration == 0 ? 1.35f : WipeDuration;
                        switch (WipeType)
                        {
                            case "Spotlight":
                                level.Add(new SpotlightWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Curtain":
                                level.Add(new CurtainWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Mountain":
                                level.Add(new MountainWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Dream":
                                level.Add(new DreamWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Starfield":
                                level.Add(new StarfieldWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Wind":
                                level.Add(new WindWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Drop":
                                level.Add(new DropWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Fall":
                                level.Add(new FallWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "KeyDoor":
                                level.Add(new KeyDoorWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Angled":
                                level.Add(new AngledWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            case "Heart":
                                level.Add(new HeartWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                            default:
                                level.Add(new FadeWipe(level, false, () => TeleportToChapter(globalLobbyID))
                                {
                                    Duration = WipeDuration < 1.35f ? 1.35f : WipeDuration
                                });
                                break;
                        }
                    }
                }
            }
        }

        private IEnumerator LobbyRoutine(Level level, string currentRoom, int lobbyIndex, int globalLobbyID)
        {
            if (lobbyMapDisplay != null)
            {
                lobbyMapDisplay.RemoveSelf();
            }
            Scene.Add(lobbyMapDisplay = new LobbyMapDisplay(level, this, lobbyIndex, globalLobbyID));
            yield return lobbyMapDisplay.GenerateMap();
            LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
            if (BigTitle != null)
            {
                BigTitle.RemoveSelf();
            }
            if (string.IsNullOrEmpty(GetSpecifiedMapName()))
            {
                Title = AreaData.Get(globalLobbyID).SID;
            }
            else
            {
                Title = GetSpecifiedMapName();
            }
            Scene.Add(BigTitle = new BigTitle(Title, new Vector2(960, 80)));
            Player player = level.Tracker.GetEntity<Player>();
            List<string> UnlockedWarps = new List<string>();
            UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).UnlockedWarps;
            UnlockedWarps.Sort();
            indexMin = 1000000;
            indexMax = 1;
            getMinMaxIndexes(UnlockedWarps, true);
            string prefix = level.Session.Area.GetLevelSet();
            if (lobbyIndex == currentLobbyIndex)
            {
                foreach (string warp in UnlockedWarps)
                {
                    string[] str = warp.Split('_');
                    if (str[0] == prefix && str[1] == "Ch" + lobbyIndex && str.Count() > 3)
                    {
                        int index = int.Parse(str[3]);
                        if (str[2] == currentRoom && index == level.Tracker.GetNearestEntity<WarpStation>(player.Position).index)
                        {
                            string warpName = SetWarpName(str[0], str[1], str[2], index);
                            destDisplay = new WarpDestinationDisplay(new Vector2(Engine.Width / 2, 970f), str[2], warpName, index);
                        }
                    }
                }
            }
            else
            {
                foreach (string warp in UnlockedWarps)
                {
                    string[] str = warp.Split('_');
                    if (str[0] == prefix && str[1] == "Ch" + lobbyIndex && str.Count() > 3)
                    {
                        if (str[2] == currentRoom)
                        {
                            int index = int.Parse(str[3]);
                            string warpName = SetWarpName(str[0], str[1], str[2], index);
                            destDisplay = new WarpDestinationDisplay(new Vector2(Engine.Width / 2, 970f), str[2], warpName, index);
                            break;
                        }
                    }
                }
            }
            while ((!Input.ESC.Pressed && !Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && player != null))
            {
                if (!lobbyMapDisplay.ZoomRoutine.Active && !lobbyMapDisplay.CameraIsMoving)
                {
                    if (Input.MenuDown.Pressed && !Input.MenuUp.Check)
                    {
                        if (destDisplay.Index != indexMax)
                        {
                            destinationWiggle.Start();
                            destinationWiggleDelay = 0.5f;
                        }
                        foreach (string warp in UnlockedWarps)
                        {
                            string[] str = warp.Split('_');
                            if (str[0] == prefix && str[1] == "Ch" + lobbyIndex && str.Count() > 3)
                            {
                                int index = int.Parse(str[3]);
                                int currentDestIndex = destDisplay.Index;
                                if (index > currentDestIndex)
                                {
                                    Audio.Play("event:/ui/main/rollover_up");
                                    string warpName = SetWarpName(str[0], str[1], str[2], index);
                                    destDisplay.UpdateDest(str[2], warpName, index);
                                    break;
                                }
                            }
                        }
                    }
                    else if (Input.MenuUp.Pressed && !Input.MenuDown.Check)
                    {
                        UnlockedWarps.Reverse();
                        if (destDisplay.Index != indexMin)
                        {
                            destinationWiggle.Start();
                            destinationWiggleDelay = 0.5f;
                        }
                        foreach (string warp in UnlockedWarps)
                        {
                            string[] str = warp.Split('_');
                            if (str[0] == prefix && str[1] == "Ch" + lobbyIndex && str.Count() > 3)
                            {
                                int index = int.Parse(str[3]);
                                int currentDestIndex = destDisplay.Index;
                                if (index < currentDestIndex)
                                {
                                    Audio.Play("event:/ui/main/rollover_down");
                                    string warpName = SetWarpName(str[0], str[1], str[2], index);
                                    destDisplay.UpdateDest(str[2], warpName, index);
                                    break;
                                }
                            }
                        }
                    }
                }
                yield return null;
            }
            if (Input.ESC.Pressed || Input.MenuCancel.Pressed || (destDisplay.Room == currentRoom && destDisplay.Index == level.Tracker.GetNearestEntity<WarpStation>(player.Position).index) && lobbyIndex == currentLobbyIndex)
            {
                DestinationRoom = "";
                RestaurePlayerSkin(player);
            }
            else
            {
                Audio.Play(ConfirmSfx);
                DestinationRoom = destDisplay.Room;
                DestinationIndex = destDisplay.Index;
            }
            lastGeneratedChapter = lobbyIndex;
            Add(new Coroutine(TeleportRoutine(player, globalLobbyID)));
        }

        private void RestaurePlayerSkin(Player player)
        {
            if (!player.Sprite.Visible)
            {
                player.Sprite.Visible = true;
            }
            if (!player.Hair.Visible)
            {
                player.Hair.Visible = true;
            }
            WarpStation nearestWarp = level.Tracker.GetNearestEntity<WarpStation>(player.Position);
            if (nearestWarp != null && nearestWarp.PlayerSprite != null && nearestWarp.PlayerHairSprite != null)
            {
                if (nearestWarp.PlayerSprite.Visible)
                {
                    nearestWarp.PlayerSprite.Visible = false;
                }
                if (nearestWarp.PlayerHairSprite.Visible)
                {
                    nearestWarp.PlayerHairSprite.Visible = false;
                }
            }
        }

        private void TeleportToChapter(int chapterIndex)
        {
            CountdownDisplay timerDisplay = level.Tracker.GetEntity<CountdownDisplay>();
            if (timerDisplay != null)
            {
                if (timerDisplay.SaveTimer)
                {
                    AreaKey area = level.Session.Area;
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownCurrentTime = timerDisplay.PausedTimer;
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownShake = timerDisplay.Shake;
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownExplode = timerDisplay.Explode;
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownActiveFlag = timerDisplay.activeFlag;
                    if ((XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownStartChapter == -1)
                    {
                        (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownStartChapter = area.ChapterIndex == -1 ? 0 : area.ChapterIndex;
                    }
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownStartRoom = timerDisplay.startRoom;
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownSpawn = timerDisplay.SpawnPosition;
                }
            }
            if (XaphanModule.useMergeChaptersController && (SceneAs<Level>().Session.Area.LevelSet == "Xaphan/0" ? !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpeedrunMode : true))
            {
                long currentTime = SceneAs<Level>().Session.Time;
                LevelEnter.Go(new Session(new AreaKey(chapterIndex))
                {
                    Time = currentTime,
                    DoNotLoad = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedNoLoadEntities[SceneAs<Level>().Session.Area.LevelSet],
                    Strawberries = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedSessionStrawberries[SceneAs<Level>().Session.Area.LevelSet]
                }
                , fromSaveData: false);
            }
            else
            {
                LevelEnter.Go(new Session(new AreaKey(chapterIndex)), fromSaveData: false);
            }
        }

        private IEnumerator CloseMap()
        {
            Level level = Scene as Level;
            Player player = Scene.Tracker.GetEntity<Player>();
            level.Remove(mapDisplay);
            level.Remove(MapProgressDisplay);
            player.StateMachine.State = Player.StNormal;
            level.PauseLock = false;
            yield return 0.1f;
            RemoveSelf();
        }

        private IEnumerator CloseLobbyMap()
        {
            Level level = Scene as Level;
            Player player = Scene.Tracker.GetEntity<Player>();
            level.Remove(lobbyMapDisplay);
            player.StateMachine.State = Player.StNormal;
            level.PauseLock = false;
            yield return 0.1f;
            RemoveSelf();
        }

        public string SetWarpName(string prefix, string chapter, string roomname, int index)
        {
            return prefix + "_Warp_" + chapter + "_" + roomname + (index != 0 ? "_" + index : "");
        }

        public override void Update()
        {
            base.Update();
            if (Totalwarps > 1)
            {
                if (!UpdatingMenu && !Teleporting)
                {
                    Add(new Coroutine(UpdateMenu(level)));
                }
                if (useIngameMap && !level.Session.GetFlag("Warp_Wipe"))
                {
                    if (mapDisplay != null && MapProgressDisplay != null)
                    {
                        if (XaphanSettings.MapScreenShowProgressDisplay.Check && progressWiggleDelay <= 0f)
                        {
                            progressWiggle.Start();
                            progressWiggleDelay = 0.5f;
                        }
                    }
                    if (lastGeneratedRoom == null)
                    {
                        Add(new Coroutine(MapRoutine(level, currentRoom)));
                        lastGeneratedRoom = "Ch" + chapterIndex + "_" + currentRoom;
                        lastGeneratedChapter = chapterIndex;
                    }
                    else if (!Input.MenuLeft.Pressed && !Input.MenuRight.Pressed)
                    {
                        if (lastGeneratedChapter != chapterIndex)
                        {
                            if (mapDisplay != null)
                            {
                                mapDisplay.Display = false;
                                if (MapProgressDisplay != null)
                                {
                                    MapProgressDisplay.RemoveSelf();
                                }
                                mapDisplay.UpdateMap(chapterIndex, menu.Current.Room, menu.Current.WarpIndex);
                                if (mapDisplay.InGameMapControllerData.ShowProgress != "Never")
                                {
                                    level.Add(MapProgressDisplay = new MapProgressDisplay(new Vector2(mapDisplay.Grid.X + 18f, mapDisplay.Grid.Y), level, mapDisplay.InGameMapControllerData, mapDisplay.SubAreaControllerData, mapDisplay.RoomControllerData, mapDisplay.TilesControllerData, mapDisplay.EntitiesData, mapDisplay.chapterIndex, mapDisplay.currentRoom));
                                }
                            }
                        }
                        else
                        {
                            if (mapDisplay != null)
                            {
                                mapDisplay.currentRoom = menu.Current.Room;
                                MapData MapData = AreaData.Areas[level.Session.Area.ID - currentChapter + chapterIndex].Mode[(int)level.Session.Area.Mode].MapData;
                                Vector2 roomOffset = mapDisplay.GetRoomOffset(MapData, menu.Current.Room, menu.Current.WarpIndex);
                                mapDisplay.SetCurrentRoomCoordinates(roomOffset);
                            }
                        }
                        lastGeneratedRoom = "Ch" + chapterIndex + "_" + menu.Current.Room;
                        lastGeneratedChapter = chapterIndex;
                    }
                    progressWiggleDelay -= Engine.DeltaTime;
                }
                if (UseLobbymap && !level.Session.GetFlag("Warp_Wipe"))
                {
                    if (Input.MenuCancel.Check && closeWiggleDelay <= 0f)
                    {
                        closeWiggle.Start();
                        closeWiggleDelay = 0.5f;
                    }
                    if (Input.MenuJournal.Check && zoomWiggleDelay <= 0f)
                    {
                        zoomWiggle.Start();
                        zoomWiggleDelay = 0.5f;
                    }
                    if (Input.MenuConfirm.Check && confirmWiggleDelay <= 0f)
                    {
                        confirmWiggle.Start();
                        confirmWiggleDelay = 0.5f;
                    }
                    closeWiggleDelay -= Engine.DeltaTime;
                    zoomWiggleDelay -= Engine.DeltaTime;
                    confirmWiggleDelay -= Engine.DeltaTime;
                    destinationWiggleDelay -= Engine.DeltaTime;
                    lobbyWiggleDelay -= Engine.DeltaTime;
                }
            }
            if (Engine.Scene.OnRawInterval(0.3f))
            {
                if (currentPositionIndicator)
                {
                    currentPositionIndicator = false;
                }
                else
                {
                    currentPositionIndicator = true;
                }
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (BigTitle != null)
            {
                BigTitle.RemoveSelf();
            }
        }

        public override void Render()
        {
            base.Render();
            if (ShowUI && Totalwarps > 1)
            {
                if (useIngameMap && !UseLobbymap)
                {
                    Draw.Rect(new Vector2(-10, -10), 1940, 182, Color.Black);
                    Draw.Rect(new Vector2(-10, 172), 100, 856, Color.Black);
                    Draw.Rect(new Vector2(1030, 172), 900, 856, Color.Black);
                    Draw.Rect(new Vector2(-10, 1028), 1940, 62, Color.Black);
                    Draw.Rect(new Vector2(90, 172), 940, 8, Color.White);
                    Draw.Rect(new Vector2(90, 180), 10, 840, Color.White);
                    Draw.Rect(new Vector2(1020, 180), 10, 840, Color.White);
                    Draw.Rect(new Vector2(90, 1020), 940, 8, Color.White);
                }
                if (UseLobbymap && !useIngameMap)
                {
                    Draw.Rect(new Vector2(-10, -10), 1940, 182, Color.Black);
                    Draw.Rect(new Vector2(-10, 172), 100, 856, Color.Black);
                    Draw.Rect(new Vector2(1830, 172), 100, 856, Color.Black);
                    Draw.Rect(new Vector2(-10, 1028), 1940, 62, Color.Black);
                    Draw.Rect(new Vector2(90, 172), 1740, 8, Color.White);
                    Draw.Rect(new Vector2(90, 180), 10, 840, Color.White);
                    Draw.Rect(new Vector2(1820, 180), 10, 840, Color.White);
                    Draw.Rect(new Vector2(90, 1020), 1740, 8, Color.White);
                }
                if (!UpdatingMenu && !UseLobbymap)
                {
                    if (BigTitle != null)
                    {
                        if (chapterIndex > firstChapterIndex)
                        {
                            mTexture.DrawCentered(new Vector2(960f - ActiveFont.Measure(BigTitle.Text).X - 100f, 80f), Color.White * alpha);
                        }
                        if (chapterIndex < lastChapterIndex)
                        {
                            mTexture.DrawCentered(new Vector2(960f + ActiveFont.Measure(BigTitle.Text).X + 100f, 80f), Color.White * alpha, 1f, (float)Math.PI);
                        }
                    }
                    if (menu != null)
                    {
                        if (menu.LastPossibleSelection > 10 && menu.Current.ID <= menu.LastPossibleSelection - 5)
                        {
                            mTexture.DrawCentered(new Vector2(menu.X, 1024f), Color.White * alpha, 1f, 4.712389f);
                        }
                        if (menu.Current.ID > 5 && menu.LastPossibleSelection > 10)
                        {
                            mTexture.DrawCentered(new Vector2(menu.X, 175f), Color.White * alpha, 1f, (float)Math.PI / 2f);
                        }
                    }
                }
                if (useIngameMap)
                {
                    float scale = 0.5f;
                    string label = Dialog.Clean("XaphanHelper_UI_progress_area");
                    string label1 = Dialog.Clean("XaphanHelper_UI_progress_subarea");
                    string label2 = Dialog.Clean("XaphanHelper_UI_showProgress");
                    string label3 = Dialog.Clean("XaphanHelper_UI_changeProgress");
                    string label4 = Dialog.Clean("XaphanHelper_UI_hideProgress");
                    Vector2 position = new Vector2(1030f, 1055f);
                    if (mapDisplay != null && MapProgressDisplay != null && !MapProgressDisplay.Hidden)
                    {
                        string progressDisplayStatus = MapProgressDisplay.mode == 0 ? (MapProgressDisplay.getSubAreaIndex() == -1 || MapProgressDisplay.SubAreaControllerData.Count == 1 ? label4 : label3) : MapProgressDisplay.mode == 1 ? label4 : label2;
                        ButtonBindingButtonUI.Render(position, progressDisplayStatus, XaphanSettings.MapScreenShowProgressDisplay, scale, 1f, progressWiggle.Value * 0.05f);
                        if (MapProgressDisplay.mode != 2)
                        {
                            string progressDisplayMode = MapProgressDisplay.mode == 0 ? label : label1;
                            float progressDisplayWidth = ActiveFont.Measure(progressDisplayMode).X;
                            ActiveFont.Draw(progressDisplayMode, new Vector2(90 + progressDisplayWidth / 4, position.Y), new Vector2(0.5f), new Vector2(scale), Color.White);
                        }
                    }
                }
                if (UseLobbymap)
                {
                    if (destDisplay != null)
                    {
                        destDisplay.Render();
                    }
                    float inputEase = 0f;
                    inputEase = Calc.Approach(inputEase, 1, Engine.DeltaTime * 4f);
                    if (inputEase > 0f && destDisplay != null)
                    {
                        float scale = 0.5f;
                        string labelLeft = Dialog.Clean("XaphanHelper_UI_changeDestination");
                        string labelLeft2 = Dialog.Clean("XaphanHelper_UI_changeLobby");
                        DoubleButtonUI.Render(new Vector2(100f + DoubleButtonUI.Width(labelLeft, Input.MenuUp, Input.MenuDown) / 2 - 8f, 1055f), labelLeft, Input.MenuUp, Input.MenuDown, scale, destDisplay.Index > indexMin, destDisplay.Index < indexMax, 1f, destinationWiggle.Value * 0.05f);
                        if (firstChapterIndex != lastChapterIndex)
                        {
                            DoubleButtonUI.Render(new Vector2(100f + DoubleButtonUI.Width(labelLeft, Input.MenuLeft, Input.MenuRight) - Input.GuiButton(Input.MenuRight, "controls/keyboard/oemquestion").Width / 2, 1055f), labelLeft2, Input.MenuLeft, Input.MenuRight, scale, lobbyIndex > firstChapterIndex, lobbyIndex < lastChapterIndex, 1f, lobbyWiggle.Value * 0.05f);
                        }
                        string labelRight = Dialog.Clean("XaphanHelper_UI_close");
                        string labelRight2 = Dialog.Clean("XaphanHelper_UI_confirm");
                        string labelRight4 = Dialog.Clean("XaphanHelper_UI_zoom");
                        float num = ButtonUI.Width(labelRight, Input.MenuCancel);
                        float num2 = ButtonUI.Width(labelRight2, Input.MenuConfirm);
                        Vector2 position = new Vector2(1830f, 1055f);
                        ButtonUI.Render(position, labelRight, Input.MenuCancel, scale, 1f, closeWiggle.Value * 0.05f);
                        position.X -= num / 2 + 32;
                        ButtonUI.Render(position, labelRight2, Input.MenuConfirm, scale, 1f, confirmWiggle.Value * 0.05f);
                        position.X -= num2 / 2 + 32;
                        if (lobbyMapDisplay != null)
                        {
                            ButtonUI.Render(position, labelRight4, Input.MenuJournal, scale, 1f, zoomWiggle.Value * 0.05f);
                        }
                    }
                }
                if (mapDisplay != null)
                {
                    if (currentPositionIndicator)
                    {
                        playerIcon.Position = playerIconHair.Position = new Vector2(mapDisplay.Grid.Left + 441, mapDisplay.Grid.Top + 401);
                        playerIconHair.Color = (level.Tracker.GetEntity<Player>() != null ? level.Tracker.GetEntity<Player>().Hair.Color : Color.White);
                        playerIcon.Render();
                        playerIconHair.Render();
                    }
                }
            }
        }

        public IEnumerator UpdateMenu(Level level)
        {
            List<string> UnlockedWarps = new List<string>();
            if (!XaphanSettings.SpeedrunMode && !XaphanModule.PlayerHasGolden)
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).UnlockedWarps;
            }
            else
            {
                UnlockedWarps = (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpeedrunModeUnlockedWarps;
            }
            UnlockedWarps.Sort();
            string prefix = level.Session.Area.GetLevelSet();
            foreach (string warp in UnlockedWarps)
            {
                string[] str = warp.Split('_');
                if (str[0] == prefix)
                {
                    string[] strCh = str[1].Split('h');
                    int chapter = short.Parse(strCh[1]);
                    if (!UnlockedChapters.Contains(chapter))
                    {
                        UnlockedChapters.Add(chapter);
                    }
                    foreach (int chapterIndex in UnlockedChapters)
                    {
                        if (chapterIndex < firstChapterIndex)
                        {
                            firstChapterIndex = chapterIndex;
                        }
                        if (chapterIndex > lastChapterIndex)
                        {
                            lastChapterIndex = chapterIndex;
                        }
                    }
                }
            }
            if (!UseLobbymap)
            {
                if (!level.Session.GetFlag("Warp_Wipe") && ((Input.MenuLeft.Pressed && chapterIndex > firstChapterIndex) || (Input.MenuRight.Pressed && chapterIndex < lastChapterIndex)))
                {
                    UpdatingMenu = true;
                    int direction = 0;
                    if (Input.MenuLeft.Pressed && chapterIndex > firstChapterIndex)
                    {
                        chapterIndex -= 1;
                        direction -= 1;
                        Audio.Play("event:/ui/main/rollover_up");
                    }
                    else if (Input.MenuRight.Pressed && chapterIndex < lastChapterIndex)
                    {
                        chapterIndex += 1;
                        direction += 1;
                        Audio.Play("event:/ui/main/rollover_up");
                    }
                    ContainChapter(direction);
                    if (menu != null)
                    {
                        menu.RemoveSelf();
                        BigTitle.RemoveSelf();
                        if (chapterIndex == currentChapter)
                        {
                            Add(new Coroutine(MenuRoutine(level, currentRoom)));
                        }
                        else
                        {
                            Add(new Coroutine(MenuRoutine(level, "")));
                        }
                    }
                    yield return 0.05f;
                    UpdatingMenu = false;
                }
            }
            else
            {
                if (!level.Session.GetFlag("Warp_Wipe") && ((Input.MenuLeft.Pressed && lobbyIndex > firstChapterIndex) || (Input.MenuRight.Pressed && lobbyIndex < lastChapterIndex)))
                {
                    int direction = 0;
                    if (Input.MenuLeft.Pressed && lobbyIndex > firstChapterIndex)
                    {
                        lobbyWiggle.Start();
                        lobbyWiggleDelay = 0.5f;
                        lobbyIndex -= 1;
                        direction -= 1;
                        Audio.Play("event:/ui/main/rollover_up");
                    }
                    else if (Input.MenuRight.Pressed && lobbyIndex < lastChapterIndex)
                    {
                        lobbyWiggle.Start();
                        lobbyWiggleDelay = 0.5f;
                        lobbyIndex += 1;
                        direction += 1;
                        Audio.Play("event:/ui/main/rollover_up");
                    }
                    ContainChapter(direction, true);
                    int GlobalLobbyID = -1;
                    LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
                    foreach (LobbyMapsLobbiesData lobbiesData in LobbyMapController.LobbiesData)
                    {
                        if (lobbiesData.LobbyIndex == lobbyIndex)
                        {
                            GlobalLobbyID = lobbiesData.GlobalLobbyID;
                        }
                    }
                    UnlockedWarps.Sort();
                    foreach (string warp in UnlockedWarps)
                    {
                        string[] str = warp.Split('_');
                        if (str[0] == prefix && str[1] == "Ch" + lobbyIndex && str.Count() > 3)
                        {
                            DestinationRoom = str[2];
                            break;
                        }
                    }
                    if (LobbyCoroutine.Active)
                    {
                        LobbyCoroutine.Cancel();
                    }
                    Add(LobbyCoroutine = new Coroutine(LobbyRoutine(level, lobbyIndex == currentLobbyIndex ? level.Session.Level : DestinationRoom, lobbyIndex, GlobalLobbyID)));
                    yield return 0.05f;
                }
            }
        }

        public void ContainChapter(int direction, bool lobby = false)
        {
            if (UnlockedChapters.Contains(lobby ? lobbyIndex : chapterIndex))
            {
                return;
            }
            else
            {
                if (lobby)
                {
                    lobbyIndex += direction;
                }
                else
                {
                    chapterIndex += direction;
                }
                ContainChapter(direction, lobby);
            }
        }

        private void useIngameMapCheck(Level level)
        {
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
            foreach (LevelData levelData in MapData.Levels)
            {
                foreach (EntityData entity in levelData.Entities)
                {
                    if (entity.Name == "XaphanHelper/InGameMapController")
                    {
                        useIngameMap = true;
                    }
                }
            }
        }

        public string GetSpecifiedMapName()
        {
            string mapName = "";
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[area.ID - currentChapter + chapterIndex].Mode[(int)area.Mode].MapData;
            foreach (LevelData levelData in MapData.Levels)
            {
                foreach (EntityData entity in levelData.Entities)
                {
                    if (entity.Name == "XaphanHelper/InGameMapController")
                    {
                        mapName = entity.Attr("mapName");
                        break;
                    }
                }
                if (mapName != "")
                {
                    break;
                }
            }
            return mapName;
        }

        public void getMinMaxIndexes(List<string> warps, bool lobby = false)
        {
            foreach (string warp in warps)
            {
                string prefix = level.Session.Area.GetLevelSet();
                string[] str = warp.Split('_');
                if (str[0] == prefix && str[1] == "Ch" + (lobby ? lobbyIndex : chapterIndex) && str.Count() > 3)
                {
                    int index = int.Parse(str[3]);
                    if (index < indexMin)
                    {
                        indexMin = index;
                    }
                    if (index > indexMax)
                    {
                        indexMax = index;
                    }
                }
            }
        }
    }
}