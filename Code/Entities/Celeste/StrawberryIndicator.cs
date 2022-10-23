using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class StrawberryIndicator : Entity
    {
        private Sprite sprite;

        public StrawberryIndicator(Vector2 position, bool ghost) : base(position)
        {
            Add(sprite = new Sprite(GFX.Game, "collectables/" + (ghost ? "ghostberry" : "strawberry") + "/" + (ghost ? "idle" : "normal")));
            sprite.AddLoop("normal", "", 0.1f, 0, 1, 2, 3, 4, 5, 6);
            sprite.CenterOrigin();
            sprite.Color = Color.White * 0.3f;
            sprite.Play("normal");
            Depth = 8999;
        }

        public void Appear()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }
    }
}
