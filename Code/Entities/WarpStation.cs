using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.XaphanHelper.UI_Elements;
using System.Collections;
using System;
using Celeste.Mod.XaphanHelper.Controllers;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/WarpStation")]
    public class WarpStation : Solid
    {
        private FieldInfo BirdTutorialGui_infoHeight = typeof(BirdTutorialGui).GetField("infoHeight", BindingFlags.Instance | BindingFlags.NonPublic);

        public class WarpBeam : Entity
        {
            private WarpStation Station;

            public static ParticleType P_Glow;

            private MTexture texture = GFX.Game["util/lightbeam"];

            private Color color;

            public int LightWidth;

            public int LightLength;

            public float Rotation;

            private float timer = Calc.Random.NextFloat(1000f);

            public WarpBeam(Vector2 position, WarpStation station) : base(position)
            {
                Station = station;
                Tag = Tags.TransitionUpdate;
                Depth = -9998;
                Position = position;
                LightWidth = 28;
                LightLength = 24;
                color = Calc.HexToColor(Station.beamColor);
                Rotation = 180 * ((float)Math.PI / 180f);
            }

            public override void Update()
            {
                timer += Engine.DeltaTime;
                Level level = Scene as Level;
                Player entity = Scene.Tracker.GetEntity<Player>();
                if (entity != null)
                {
                    Vector2 value = Calc.AngleToVector(Rotation + (float)Math.PI / 2f, 1f);
                    Vector2 value2 = Calc.ClosestPointOnLine(Position, Position + value * 10000f, entity.Center);
                    float target = Math.Min(1f, Math.Max(0f, (value2 - Position).Length() - 8f) / LightLength);
                    if ((value2 - entity.Center).Length() > LightWidth / 2f)
                    {
                        target = 1f;
                    }
                    if (level.Transitioning)
                    {
                        target = 0f;
                    }
                }
                if (level.OnInterval(0.5f))
                {
                    Vector2 vector = Calc.AngleToVector(Rotation + (float)Math.PI / 2f, 1f);
                    Vector2 position = Position - vector * 4f;
                    float scaleFactor = Calc.Random.Next(LightWidth - 4) + 2 - LightWidth / 2;
                    position += scaleFactor * vector.Perpendicular();
                    level.Particles.Emit(LightBeam.P_Glow, position, Rotation + (float)Math.PI / 2f);
                }
                base.Update();
            }

            public override void Render()
            {
                if (Visible)
                {
                    DrawTexture(0f, LightWidth, (LightLength - 4) + (float)Math.Sin(timer * 2f) * 4f, 0.4f);
                    for (int i = 0; i < LightWidth; i += 4)
                    {
                        float num = timer + i * 0.3f;
                        float num2 = 4f + (float)Math.Sin(num * 0.5f + 1.2f) * 4f;
                        float offset = (float)Math.Sin(((num + (i * 32)) * 0.1f) + Math.Sin(num * 0.05f + i * 0.1f) * 0.25) * (LightWidth / 2f - num2 / 2f);
                        float length = LightLength + (float)Math.Sin(num * 0.25f) * 8f;
                        float a = 0.6f + (float)Math.Sin(num + 0.8f) * 0.3f;
                        DrawTexture(offset, num2, length, a);
                    }
                }
            }

            private void DrawTexture(float offset, float width, float length, float a)
            {
                float rotation = Rotation + (float)Math.PI / 2f;
                if (width >= 1f)
                {
                    texture.Draw(Position + Calc.AngleToVector(Rotation, 1f) * offset, new Vector2(0f, 0.5f), color * a, new Vector2(1f / texture.Width * length, width), rotation);
                }
            }
        }

        private TalkComponent talk;

        private Sprite warpStationSprite;

        public Sprite PlayerSprite;

        public Sprite PlayerHairSprite;

        private WarpBeam beam;

        private bool activated;

        private bool noBeam;

        public string beamColor;

        private string flag;

        private string sprite;

        public int index;

        public string ConfirmSfx;

        private string wipeType;

        private float wipeDuration;

        private List<Vector2> CustomImageTilesCoordinates = new List<Vector2>();

        private MTexture CustomImage;

        private BirdTutorialGui tutorialGui;

        private float tutorialTimer = 0f;

        public bool hideTutorial;

        private Vector2 spawnPoint = Vector2.Zero;

        private bool ShouldSave;

        private Level level => (Level)Scene;

        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        public WarpStation(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: true)
        {
            Tag = Tags.TransitionUpdate;
            index = data.Int("index", 0);
            sprite = data.Attr("sprite", "objects/XaphanHelper/WarpStation");
            ConfirmSfx = data.Attr("confirmSfx");
            wipeType = data.Attr("wipeType", "fade");
            wipeDuration = data.Float("wipeDuration", 0.75f);
            if (string.IsNullOrEmpty(sprite))
            {
                sprite = "objects/XaphanHelper/WarpStation";
            }
            beamColor = data.Attr("beamColor", "FFFFFF");
            noBeam = data.Bool("noBeam");
            flag = data.Attr("flag");
            Collider.Width = 32f;
            Collider.Height = 16f;
            SurfaceSoundIndex = 8;
            Add(warpStationSprite = new Sprite(GFX.Game, sprite + "/"));
            warpStationSprite.AddLoop("idle", "idle", 0.08f);
            warpStationSprite.AddLoop("active", "active", 0.08f);
            warpStationSprite.Play("idle");
            Depth = -9000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            foreach (string warp in XaphanModule.ModSaveData.UnlockedWarps)
            {
                if (warp.Contains(level.Session.Area.GetLevelSet()))
                {
                    hideTutorial = true;
                    break;
                }
            }
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
            foreach (LevelData levelData in MapData.Levels)
            {
                if (levelData.Name == level.Session.Level)
                {
                    foreach (EntityData entity in levelData.Entities)
                    {
                        if (entity.Name == "XaphanHelper/WarpStation" && entity.Int("index") == index)
                        {
                            spawnPoint = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
            string prefix = level.Session.Area.GetLevelSet();
            string room = level.Session.Level;
            int chapterIndex = level.Session.Area.ChapterIndex == -1 ? 0 : level.Session.Area.ChapterIndex;
            if (!activated)
            {
                if (LobbyMapController != null)
                {
                    warpStationSprite.Visible = false;
                    if (!hideTutorial && tutorialGui == null)
                    {
                        tutorialGui = new BirdTutorialGui(this, new Vector2(16f, -20f), Dialog.Clean("XaphanHelper_UI_fastTravel"));
                        tutorialGui.Open = false;
                        BirdTutorialGui_infoHeight.SetValue(tutorialGui, 0);
                        level.Add(tutorialGui);
                    }
                    if (tutorialGui != null)
                    {
                        Player player = Scene.Tracker.GetEntity<Player>();
                        if (player != null && player.StateMachine.State != 11 && player.Left >= Left - 48f && player.Right <= Right + 48f && player.Top >= Top - 64f && player.Bottom <= Bottom + 32f)
                        {
                            tutorialTimer += Engine.DeltaTime;
                        }
                        else
                        {
                            tutorialTimer = 0f;
                        }
                        tutorialGui.Open = (tutorialTimer > 0.25f);
                    }
                    if (XaphanModule.ModSaveData.UnlockedWarps.Contains(prefix + "_Ch" + LobbyMapController.lobbyIndex + "_" + room + (index != 0 ? "_" + index : "")))
                    {
                        activated = true;
                        if (!noBeam)
                        {
                            level.Add(beam = new WarpBeam(Position + new Vector2(16, 0), this));
                        }
                        Activate();
                        warpStationSprite.Play("active");
                    }
                }
                else
                {
                    if (!Settings.SpeedrunMode && !XaphanModule.PlayerHasGolden ? XaphanModule.ModSaveData.UnlockedWarps.Contains(prefix + "_Ch" + chapterIndex + "_" + room + (index != 0 ? "_" + index : "")) : XaphanModule.ModSaveData.SpeedrunModeUnlockedWarps.Contains(prefix + "_Ch" + chapterIndex + "_" + room + (index != 0 ? "_" + index : "")))
                    {
                        activated = true;
                        if (!noBeam)
                        {
                            level.Add(beam = new WarpBeam(Position + new Vector2(16, 0), this));
                        }
                        Activate();
                        warpStationSprite.Play("active");
                    }
                }
            }
            else
            {
                if (XaphanModule.PlayerHasGolden && !XaphanModule.ModSaveData.SpeedrunModeUnlockedWarps.Contains(prefix + "_Ch" + chapterIndex + "_" + room + (index != 0 ? "_" + index : "")))
                {
                    activated = false;
                    if (talk != null)
                    {
                        talk.RemoveSelf();
                    }
                    if (beam != null)
                    {
                        beam.RemoveSelf();
                    }
                    warpStationSprite.Play("idle");
                }
            }
            if (tutorialGui != null)
            {
                tutorialGui.Open = (tutorialTimer > 0.25f);
            }
            if (beam != null && activated && beam.Visible == false)
            {
                beam.Visible = true;
            }
            if (HasPlayerOnTop() && !activated)
            {
                if (!string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag))
                {
                    Activate();
                }
                else if (string.IsNullOrEmpty(flag))
                {
                    Activate();
                }
            }
            if (XaphanModule.useMergeChaptersController && XaphanModule.MergeChaptersControllerMode == "Warps" && HasPlayerOnTop() && !XaphanModule.PlayerHasGolden && !level.Frozen && level.Tracker.GetEntity<CountdownDisplay>() == null && level.Tracker.GetEntity<Player>() != null && level.Tracker.GetEntity<Player>().StateMachine.State != Player.StDummy && !XaphanModule.PlayerIsControllingRemoteDrone() && activated && (level.Session.Area.LevelSet == "Xaphan/0" ? !XaphanModule.ModSaveData.SpeedrunMode : true) && !ShouldSave && !((XaphanModule.MergeChaptersControllerKeepPrologue && level.Session.Area.ID == SaveData.Instance.GetLevelSetStats().AreaOffset)))
            {
                if (!XaphanModule.ModSaveData.SavedChapter.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedChapter.Add(prefix, level.Session.Area.ChapterIndex);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedChapter[prefix] != level.Session.Area.ChapterIndex)
                    {
                        XaphanModule.ModSaveData.SavedChapter[prefix] = level.Session.Area.ChapterIndex;
                        ShouldSave = true;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedRoom.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedRoom.Add(prefix, level.Session.Level);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedRoom[prefix] != level.Session.Level)
                    {
                        XaphanModule.ModSaveData.SavedRoom[prefix] = level.Session.Level;
                        ShouldSave = true;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedSpawn.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedSpawn.Add(prefix, spawnPoint);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedSpawn[prefix] != spawnPoint)
                    {
                        XaphanModule.ModSaveData.SavedSpawn[prefix] = spawnPoint;
                        ShouldSave = true;
                    }                    
                }
                if (!XaphanModule.ModSaveData.SavedLightingAlphaAdd.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedLightingAlphaAdd.Add(prefix, level.Lighting.Alpha - level.BaseLightingAlpha);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedLightingAlphaAdd[prefix] != level.Lighting.Alpha - level.BaseLightingAlpha)
                    {
                        XaphanModule.ModSaveData.SavedLightingAlphaAdd[prefix] = level.Lighting.Alpha - level.BaseLightingAlpha;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedBloomBaseAdd.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedBloomBaseAdd.Add(prefix, level.Bloom.Base - AreaData.Get(level).BloomBase);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedBloomBaseAdd[prefix] != level.Bloom.Base - AreaData.Get(level).BloomBase)
                    {
                        XaphanModule.ModSaveData.SavedBloomBaseAdd[prefix] = level.Bloom.Base - AreaData.Get(level).BloomBase;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedCoreMode.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedCoreMode.Add(prefix, level.Session.CoreMode);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedCoreMode[prefix] != level.Session.CoreMode)
                    {
                        XaphanModule.ModSaveData.SavedCoreMode[prefix] = level.Session.CoreMode;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedMusic.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedMusic.Add(prefix, level.Session.Audio.Music.Event);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedMusic[prefix] != level.Session.Audio.Music.Event)
                    {
                        XaphanModule.ModSaveData.SavedMusic[prefix] = level.Session.Audio.Music.Event;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedAmbience.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedAmbience.Add(prefix, level.Session.Audio.Ambience.Event);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedAmbience[prefix] != level.Session.Audio.Ambience.Event)
                    {
                        XaphanModule.ModSaveData.SavedAmbience[prefix] = level.Session.Audio.Ambience.Event;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedNoLoadEntities.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedNoLoadEntities.Add(prefix, level.Session.DoNotLoad);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedNoLoadEntities[prefix] != level.Session.DoNotLoad)
                    {
                        XaphanModule.ModSaveData.SavedNoLoadEntities[prefix] = level.Session.DoNotLoad;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedFromBeginning.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedFromBeginning.Add(prefix, level.Session.StartedFromBeginning);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedFromBeginning[prefix] != level.Session.StartedFromBeginning)
                    {
                        XaphanModule.ModSaveData.SavedFromBeginning[prefix] = level.Session.StartedFromBeginning;
                    }
                }
                string sessionFlags = "";
                foreach(string flag in level.Session.Flags)
                {
                    if (sessionFlags == "")
                    {
                        sessionFlags += flag;
                    }
                    else
                    {
                        sessionFlags += "," + flag;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedSesionFlags.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedSesionFlags.Add(prefix, sessionFlags);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedSesionFlags[prefix] != sessionFlags)
                    {
                        XaphanModule.ModSaveData.SavedSesionFlags[prefix] = sessionFlags;
                    }
                }
                if (!XaphanModule.ModSaveData.SavedSessionStrawberries.ContainsKey(prefix))
                {
                    XaphanModule.ModSaveData.SavedSessionStrawberries.Add(prefix, level.Session.Strawberries);
                }
                else
                {
                    if (XaphanModule.ModSaveData.SavedSessionStrawberries[prefix] != level.Session.Strawberries)
                    {
                        XaphanModule.ModSaveData.SavedSessionStrawberries[prefix] = level.Session.Strawberries;
                    }
                }
                if (ShouldSave)
                {
                    level.AutoSave();
                }
            }
            if (!HasPlayerOnTop() && ShouldSave)
            {
                ShouldSave = false;
            }
        }

        private void Interact(Player player)
        {
            Add(new Coroutine(InteractRoutine(player)));
        }

        public IEnumerator InteractRoutine(Player player)
        {
            if (player != null)
            {
                player.StateMachine.State = 11;
                yield return player.DummyWalkToExact((int)X + 16, false, 1f, true);
                player.Facing = Facings.Right;
                LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
                if (LobbyMapController != null)
                {
                    Add(PlayerSprite = new Sprite(GFX.Game, "characters/Xaphan/player/"));
                    PlayerSprite.Add("sit", "sitBench", 0.08f);
                    PlayerSprite.CenterOrigin();
                    PlayerSprite.Position += new Vector2(16f, -16f);
                    Add(PlayerHairSprite = new Sprite(GFX.Game, "characters/Xaphan/player/"));
                    PlayerHairSprite.Add("sit", "sitBenchHair", 0.08f);
                    PlayerHairSprite.CenterOrigin();
                    PlayerHairSprite.Position += new Vector2(16f, -16f);
                    PlayerHairSprite.Color = player.Hair.Color;
                    player.Sprite.Visible = player.Hair.Visible = false;
                    PlayerSprite.Visible = true;
                    PlayerHairSprite.Visible = true;
                    PlayerSprite.Play("sit");
                    PlayerHairSprite.Play("sit");
                    CustomImage = LobbyMapController.CustomImage;
                    PlayerSprite.OnLastFrame += OpenWarpScreen;
                }
                else
                {
                    SceneAs<Level>().Add(new WarpScreen(level, !string.IsNullOrEmpty(ConfirmSfx) ? ConfirmSfx : "event:/game/xaphan/warp", wipeType, wipeDuration));
                }
            }           
        }

        private void OpenWarpScreen(string s)
        {
            SceneAs<Level>().Add(new WarpScreen(level, !string.IsNullOrEmpty(ConfirmSfx) ? ConfirmSfx : "event:/game/xaphan/warp", wipeType, wipeDuration, true));
        }

        public void Activate()
        {
            if (!activated && !noBeam)
            {
                SceneAs<Level>().Add(beam = new WarpBeam(Position + new Vector2(16, 0), this));
            }
            foreach (WarpStation warpStation in level.Tracker.GetEntities<WarpStation>())
            {
                warpStation.hideTutorial = true;
                if (warpStation.tutorialGui != null)
                {
                    level.Remove(warpStation.tutorialGui);
                }
            }
            activated = true;
            tutorialTimer = 0f;
            Add(talk = new TalkComponent(new Rectangle(4, -8, 24, 8), new Vector2(16f, -16f), Interact));
            talk.PlayerMustBeFacing = false;
            string prefix = level.Session.Area.GetLevelSet();
            int chapterIndex = SceneAs<Level>().Session.Area.ChapterIndex == -1 ? 0 : SceneAs<Level>().Session.Area.ChapterIndex;
            string room = SceneAs<Level>().Session.Level;
            LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
            if (!XaphanModule.ModSaveData.UnlockedWarps.Contains(prefix + "_Ch" + ((LobbyMapController != null && LobbyMapController.lobbyIndex != 0) ? LobbyMapController.lobbyIndex : chapterIndex) + "_" + room + (index != 0 ? "_" + index : "")))
            {
                XaphanModule.ModSaveData.UnlockedWarps.Add(prefix + "_Ch" + ((LobbyMapController != null && LobbyMapController.lobbyIndex != 0) ? LobbyMapController.lobbyIndex : chapterIndex) + "_" + room + (index != 0 ? "_" + index : ""));
                warpStationSprite.Play("active");
            }
            if (Settings.SpeedrunMode || XaphanModule.PlayerHasGolden)
            {
                if (!XaphanModule.ModSaveData.SpeedrunModeUnlockedWarps.Contains(prefix + "_Ch" + ((LobbyMapController != null && LobbyMapController.lobbyIndex != 0) ? LobbyMapController.lobbyIndex : chapterIndex) + "_" + room + (index != 0 ? "_" + index : "")))
                {
                    XaphanModule.ModSaveData.SpeedrunModeUnlockedWarps.Add(prefix + "_Ch" + ((LobbyMapController != null && LobbyMapController.lobbyIndex != 0) ? LobbyMapController.lobbyIndex : chapterIndex) + "_" + room + (index != 0 ? "_" + index : ""));
                    warpStationSprite.Play("active");
                }
            }
        }

        public override void Render()
        {
            base.Render();
            if (PlayerSprite != null && PlayerSprite.Visible)
            {
                PlayerSprite.Render();
            }
            if (PlayerHairSprite != null && PlayerHairSprite.Visible)
            {
                PlayerHairSprite.Render();
            }
        }
    }
}
