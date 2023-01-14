using System;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers;

[Tracked(true)]
[CustomEntity("XaphanHelper/LobbyMapController")]
public class LobbyMapController : Entity
{
    public readonly string Directory;
    public readonly string LevelSet;
    public readonly int LobbyIndex;
    public readonly int TotalMaps;
    public readonly Facings Facing;
    public readonly int ImageScaleX;
    public readonly int ImageScaleY;
    public LobbyVisitManager VisitManager;

    public LobbyMapController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Tag = Tags.Persistent;
        Directory = data.Attr("directory");
        LobbyIndex = data.Int("lobbyIndex");
        LevelSet = data.Attr("levelSet");
        TotalMaps = data.Int("totalMaps");
        ImageScaleX = data.Int("imageScaleX", 4);
        ImageScaleY = data.Int("imageScaleY", 4);
        Facing = (Facings) data.Int("facing", (int) Facings.Right);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);

        if (scene is Level level)
        {
            VisitManager = LobbyVisitManager.ForLobby(level.Session.Area, LobbyIndex);
        }
    }

    public override void Update()
    {
        base.Update();
        if (Scene is Level level && level.Tracker.GetEntity<WarpScreen>() == null)
        {
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                if (!Scene.OnInterval(0.2f) || player.StateMachine.State == Player.StDummy)
                {
                    return;
                }
            }

            if (player != null && !level.Paused && !level.Transitioning)
            {
                var playerPosition = new Vector2(Math.Min((float) Math.Floor((player.Center.X - level.Bounds.X) / 8f), (float) Math.Round(level.Bounds.Width / 8f, MidpointRounding.AwayFromZero) - 1),
                    Math.Min((float) Math.Floor((player.Center.Y - level.Bounds.Y) / 8f), (float) Math.Round(level.Bounds.Height / 8f, MidpointRounding.AwayFromZero) + 1));
                VisitManager?.VisitPoint(playerPosition);
            }
        }
    }

    public override void SceneEnd(Scene scene)
    {
        VisitManager.Save();
        base.SceneEnd(scene);
    }
}