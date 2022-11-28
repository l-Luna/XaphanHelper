using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/TransitionTrigger")]
    class TransitionTrigger : Trigger
    {
        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        private string DestinationLevel;

        private string Direction;

        private bool PushPlayer;

        public TransitionTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            DestinationLevel = data.Attr("destinationLevel");
            Direction = data.Attr("direction");
            PushPlayer = data.Bool("pushPlayer");
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            Rectangle bounds = new((int)X, (int)Y, (int)Width, (int)Height);
            if (SceneAs<Level>().Transitioning)
            {
                return;
            }
            if (Direction == "Left" && player.Left < Left && player.Speed.X < 0)
            {
                player.BeforeSideTransition();
                SceneAs<Level>().OnEndOfFrame += delegate
                {
                    Engine.TimeRate = 1f;
                    Distort.Anxiety = 0f;
                    Distort.GameRate = 1f;
                    AreaKey area = SceneAs<Level>().Session.Area;
                    MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                    foreach (LevelData levelData in MapData.Levels)
                    {
                        if (levelData.Name == DestinationLevel)
                        {
                            SceneAs<Level>().TransitionTo(levelData, -Vector2.UnitX);
                            break;
                        }
                    }
                };
                if (PushPlayer)
                {
                    player.Left -= 8;
                }
                else
                {
                    player.Left = Left;
                }
            }
            else if (Direction == "Right" && player.Right > Right && player.Speed.X > 0)
            {
                player.BeforeSideTransition();
                SceneAs<Level>().OnEndOfFrame += delegate
                {
                    Engine.TimeRate = 1f;
                    Distort.Anxiety = 0f;
                    Distort.GameRate = 1f;
                    AreaKey area = SceneAs<Level>().Session.Area;
                    MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                    foreach (LevelData levelData in MapData.Levels)
                    {
                        if (levelData.Name == DestinationLevel)
                        {
                            SceneAs<Level>().TransitionTo(levelData, Vector2.UnitX);
                            break;
                        }
                    }
                };
                if (PushPlayer)
                {
                    player.Right += 8;
                }
                else
                {
                    player.Right = Right;
                }
            }
        }
    }
}
