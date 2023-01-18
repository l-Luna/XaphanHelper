using System;
using System.Collections;
using System.Linq;
using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    [Tracked(true)]
    public class LobbyMapDisplay : Entity
    {
        public static int DefaultZoomLevel => Scales.Length / 2;
        public static readonly float[] Scales = { 0.4f, 0.8f, 1.4f };
        public float ScaleRatio => (Scale - Scales[0]) / (Scales[Scales.Length - 1] - Scales[0]); 
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public int AreaId { get; }
        public string Room { get; }
        public int LobbyIndex { get; private set; }
        public int ZoomLevel { get; private set; }

        private float targetScale = 1f;
        private Vector2 targetOrigin = Vector2.Zero;
        private Vector2 selectedOrigin = Vector2.Zero;
        private bool shouldCentreOrigin;
        private float scaleTimeRemaining;
        private float translateTimeRemaining;
        private const float scale_time_seconds = 0.3f;
        private const float translate_time_seconds = 0.3f;

        public int ExplorationRadius { get; private set; }
        public LobbyMapSprite Sprite { get; private set; }
        public LobbyMapOverlay Overlay { get; private set; }
        public LobbyMapIconDisplay IconDisplay { get; private set; }
        public WarpInfo SelectedWarp => warpScreen.SelectedWarp;

        private readonly WarpScreen warpScreen;
        private LevelData levelData;
        private WarpInfo lastSelectedWarpInfo;
        public LobbyHeartsDisplay heartDisplay;
        private VirtualRenderTarget target;
        private bool disposed;

        public Vector2 OriginForPosition(Vector2 point)
        {
            var tileX = point.X / 8f;
            var tileY = point.Y / 8f;
            return new Vector2(tileX / Sprite.WidthInTiles, tileY / Sprite.HeightInTiles);
        }

        public LobbyMapDisplay(WarpScreen warpScreen, int areaId, string room, int zoomLevel)
        {
            this.warpScreen = warpScreen;
            AreaId = areaId;
            Room = room;
            Tag = Tags.HUD;
            ZoomLevel = zoomLevel;
            Scale = Scales[zoomLevel];

            shouldCentreOrigin = zoomLevel == 0;
        }

        public void Finished()
        {
            // we dispose here to make sure it doesn't render both maps at once for a frame
            Dispose();
            RemoveSelf();
        }
        
        public override void Added(Scene scene)
        {
            base.Added(scene);

            var mapData = AreaData.Areas.ElementAtOrDefault(AreaId)?.Mode.FirstOrDefault()?.MapData;
            levelData = mapData?.Get(Room);

            if (levelData?.GetEntityData("XaphanHelper/LobbyMapController") is not { } lobbyMapControllerData) return;

            var directory = lobbyMapControllerData.Attr("directory");
            var totalMaps = lobbyMapControllerData.Int("totalMaps");
            var imageScaleX = lobbyMapControllerData.Int("imageScaleX", 4);
            var imageScaleY = lobbyMapControllerData.Int("imageScaleY", 4);
            var levelSet = lobbyMapControllerData.Attr("levelSet");
            LobbyIndex = lobbyMapControllerData.Int("lobbyIndex");
            ExplorationRadius = lobbyMapControllerData.Int("explorationRadius", 20);

            scene.Add(heartDisplay = new LobbyHeartsDisplay(new Vector2(100, 180), levelSet, totalMaps, LobbyIndex, warpScreen.currentLobbyHeartAnimation));

            Add(Sprite = new LobbyMapSprite(directory, imageScaleX, imageScaleY));
            Add(Overlay = new LobbyMapOverlay());

            var miniHeartDoorIcon = lobbyMapControllerData.Attr("miniHeartDoorIcon");
            var journalIcon = lobbyMapControllerData.Attr("journalIcon");
            var mapIcon = lobbyMapControllerData.Attr("mapIcon");
            var rainbowsBerryIcon = lobbyMapControllerData.Attr("rainbowsBerryIcon");
            var warpIcon = lobbyMapControllerData.Attr("warpIcon");
            var extraEntitiesNames = lobbyMapControllerData.Attr("extraEntitiesNames");
            var extraEntitiesIcons = lobbyMapControllerData.Attr("extraEntitiesIcons");
            Add(IconDisplay = new LobbyMapIconDisplay(levelData, SaveData.Instance.Areas[AreaId], miniHeartDoorIcon, journalIcon, mapIcon, rainbowsBerryIcon, warpIcon, extraEntitiesNames, extraEntitiesIcons));

            var tex = Sprite.MapTexture;
            target = VirtualContent.CreateRenderTarget("map", tex.Width, tex.Height);
            Add(new BeforeRenderHook(BeforeRender));
            
            Add(new Coroutine(MapFocusRoutine()));
        }

        private void BeforeRender()
        {
            if (disposed) return;
            
            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);

            Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, new BlendState
            {
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero,
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
            });
            Draw.SpriteBatch.Draw(Overlay.OverlayTexture, new Rectangle(0, 0, target.Width, target.Height), Color.White);
            Draw.SpriteBatch.End();
            
            Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, new BlendState
            {
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.DestinationAlpha,
                ColorDestinationBlend = Blend.Zero,
            });
            var tex = Sprite.MapTexture;
            Draw.SpriteBatch.Draw(tex, new Rectangle(0, 0, target.Width, target.Height), Color.White);
            Draw.SpriteBatch.End();
        }

        public override void Update()
        {
            var first = lastSelectedWarpInfo.ID == default;
            
            if (lastSelectedWarpInfo.ID != warpScreen.SelectedWarp.ID)
            {
                IconDisplay.ResetPlayerVisible();
                
                lastSelectedWarpInfo = warpScreen.SelectedWarp;
                selectedOrigin = OriginForPosition(warpScreen.SelectedWarp.Position);

                if (first)
                    Origin = shouldCentreOrigin ? new Vector2(0.5f) : selectedOrigin;
                else if (!shouldCentreOrigin)
                {
                    targetOrigin = selectedOrigin;
                    translateTimeRemaining = translate_time_seconds;
                }
            }

            if (Input.MenuJournal.Pressed)
            {
                ZoomLevel--;
                if (ZoomLevel < 0) ZoomLevel += Scales.Length;

                targetScale = Scales[ZoomLevel];
                scaleTimeRemaining = scale_time_seconds;
                shouldCentreOrigin = ZoomLevel == 0;
                
                if (shouldCentreOrigin || ZoomLevel == Scales.Length - 1)
                {
                    targetOrigin = shouldCentreOrigin ? new Vector2(0.5f) : selectedOrigin;
                    translateTimeRemaining = translate_time_seconds;
                }
            }
            
            base.Update();
        }

        private IEnumerator MapFocusRoutine()
        {
            float scaleFrom = Scale;
            Vector2 translateFrom = Origin;
            
            while (true)
            {
                if (scaleTimeRemaining == scale_time_seconds) scaleFrom = Scale;
                if (translateTimeRemaining == translate_time_seconds) translateFrom = Origin;

                if (scaleTimeRemaining > 0)
                {
                    Scale = Calc.LerpClamp(scaleFrom, targetScale, Ease.QuintOut(1 - scaleTimeRemaining / scale_time_seconds));
                    scaleTimeRemaining -= Engine.DeltaTime;
                    if (scaleTimeRemaining <= 0) Scale = targetScale;
                }
                
                if (translateTimeRemaining > 0)
                {
                    Origin = Vector2.Lerp(translateFrom, targetOrigin, Ease.QuintOut(1 - translateTimeRemaining / translate_time_seconds));
                    translateTimeRemaining -= Engine.DeltaTime;
                    if (translateTimeRemaining <= 0) Origin = targetOrigin;
                }

                yield return null;
            }
        }

        public override void Render()
        {
            Draw.Rect(new Vector2(100, 180), 1720, 840, Color.Black * 0.9f);
            
            // if we've been removed, don't try to draw anything other than the dark tint
            if (disposed) return;
            
            Draw.SpriteBatch.Draw(target, new Vector2(Engine.Width / 2f, Engine.Height / 2f), null, Color.White, 0, new Vector2(Origin.X * target.Width, Origin.Y * target.Height), new Vector2(Scale), SpriteEffects.None, 0);
            base.Render();
            ActiveFont.DrawOutline(Dialog.Clean(warpScreen.SelectedWarp.DialogKey), new Vector2(Celeste.TargetCenter.X, Celeste.TargetHeight - 110f), new Vector2(0.5f, 0.5f), Vector2.One, Color.White, 2f, Color.Black);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            heartDisplay?.RemoveSelf();
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;
            target?.Dispose();
            target = null;
            disposed = true;
        }
    }
}
