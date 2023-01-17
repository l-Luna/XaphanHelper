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
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public int AreaId { get; }
        public string Room { get; }
        public int LobbyIndex { get; private set; }

        public int ExplorationRadius { get; private set; }
        public LobbyMapSprite Sprite { get; private set; }
        public LobbyMapOverlay Overlay { get; private set; }
        public LobbyMapIconDisplay IconDisplay { get; private set; }
        public WarpInfo SelectedWarp => warpScreen.SelectedWarp;

        private readonly WarpScreen warpScreen;
        private LevelData levelData;
        private Vector2 tweenSource;
        private Vector2 tweenTarget;
        private WarpInfo lastSelectedWarpInfo;
        private float tweenRemainingSeconds;
        private const float tweenTimeSeconds = 0.5f;
        private Coroutine zoomRoutine = new();
        private LobbyHeartsDisplay heartDisplay;
        private VirtualRenderTarget target;
        private bool disposed;

        public Vector2 OriginForPosition(Vector2 point)
        {
            var tileX = point.X / 8f;
            var tileY = point.Y / 8f;
            return new Vector2(tileX / Sprite.WidthInTiles, tileY / Sprite.HeightInTiles);
        }

        public LobbyMapDisplay(WarpScreen warpScreen, int areaId, string room, float scale)
        {
            this.warpScreen = warpScreen;
            AreaId = areaId;
            Room = room;
            Tag = Tags.HUD;
            Scale = scale;
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

            scene.Add(heartDisplay = new LobbyHeartsDisplay(new Vector2(100, 180), levelSet, totalMaps, LobbyIndex));

            Add(Sprite = new LobbyMapSprite(directory, imageScaleX, imageScaleY));
            Add(Overlay = new LobbyMapOverlay());
            Add(IconDisplay = new LobbyMapIconDisplay(levelData, SaveData.Instance.Areas[AreaId]));

            Sprite.Visible = Overlay.Visible = false;
            
            var tex = Sprite.Animations[Sprite.CurrentAnimationID].Frames[0].Texture.Texture;
            target = VirtualContent.CreateRenderTarget("map", tex.Width, tex.Height);
            Add(new BeforeRenderHook(BeforeRender));
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
            Overlay.Render();
            Draw.SpriteBatch.End();
            
            Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, new BlendState
            {
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.DestinationAlpha,
                ColorDestinationBlend = Blend.Zero,
            });
            Sprite.Render();
            Draw.SpriteBatch.End();
        }

        public override void Update()
        {
            base.Update();

            if (Input.MenuJournal.Pressed && !zoomRoutine.Active)
            {
                Add(zoomRoutine = new Coroutine(ChangeZoom()));
            }

            if (Sprite != null && !zoomRoutine.Active)
            {
                if (lastSelectedWarpInfo.ID == default)
                {
                    lastSelectedWarpInfo = warpScreen.SelectedWarp;
                    tweenTarget = OriginForPosition(warpScreen.SelectedWarp.Position);
                }

                if (lastSelectedWarpInfo.ID != warpScreen.SelectedWarp.ID)
                {
                    lastSelectedWarpInfo = warpScreen.SelectedWarp;
                    tweenSource = Origin;
                    tweenTarget = OriginForPosition(warpScreen.SelectedWarp.Position);
                    tweenRemainingSeconds = tweenTimeSeconds;
                }

                if (tweenRemainingSeconds > 0)
                {
                    tweenRemainingSeconds -= Engine.DeltaTime;
                    var tweenAmount = 1 - tweenRemainingSeconds / tweenTimeSeconds;
                    Origin = Vector2.Lerp(tweenSource, tweenTarget, Ease.QuintOut(tweenAmount));
                }

                if (tweenRemainingSeconds <= 0)
                {
                    Origin = OriginForPosition(warpScreen.SelectedWarp.Position);
                }
            }
        }

        private IEnumerator ChangeZoom()
        {
            tweenRemainingSeconds = 0;

            if (Scale != 0.5f)
            {
                float finalScale = Scale - 0.25f;
                while (Scale > finalScale)
                {
                    Scale -= Engine.DeltaTime * 1.25f;
                    yield return null;
                }

                Scale = finalScale;
            }
            else
            {
                float finalScale = 1f;
                while (Scale < finalScale)
                {
                    Scale += Engine.DeltaTime * 2.5f;
                    yield return null;
                }

                Scale = finalScale;
            }

            yield return null;
        }

        public override void Render()
        {
            Draw.Rect(new Vector2(100, 180), 1720, 840, Color.Black * 0.9f);
            
            // if we've been removed, don't try to draw anything other than the dark tint
            if (disposed) return;
            Draw.SpriteBatch.Draw(target, Vector2.Zero, Color.White);
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
