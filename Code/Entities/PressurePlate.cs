using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/PressurePlate")]
    public class PressurePlate : Entity
    {
        Sprite sprite;

        string flag;

        string directory;

        public PressurePlate(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Collider = new Hitbox(12f, 2f, 2f, 6f);
            flag = data.Attr("flag");
            directory = data.Attr("directory", "objects/XaphanHelper/PressurePlate");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/PressurePlate";
            }
            Add(sprite = new Sprite(GFX.Game, directory + "/"));
            sprite.AddLoop("idle", "button", 0f);
            sprite.Play("idle");
        }

        public override void Update()
        {
            base.Update();
            if (CollideCheck<Actor>() || CollideCheck<WorkRobot>())
            {
                sprite.Position = Vector2.UnitY;
                if (!string.IsNullOrEmpty(flag))
                {
                    SceneAs<Level>().Session.SetFlag(flag, true);
                }
            }
            else
            {
                sprite.Position = Vector2.Zero;
                if (!string.IsNullOrEmpty(flag))
                {
                    SceneAs<Level>().Session.SetFlag(flag, false);
                }
            }
        }
    }
}
