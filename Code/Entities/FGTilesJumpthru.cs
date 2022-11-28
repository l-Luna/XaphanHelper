using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/FGTilesJumpthru")]
    class FGTilesJumpthru : JumpThru
    {
        private int columns;

        private string overrideTexture;

        private int overrideSoundIndex = -1;

        public FGTilesJumpthru(EntityData data, Vector2 position) : base(data.Position + position, data.Width, true)
        {
            columns = data.Width / 8;
            overrideTexture = data.Attr("texture", "default");
            overrideSoundIndex = data.Int("surfaceIndex", -1);
            Depth = -60;
        }

        public override void Awake(Scene scene)
        {
            string jumpthru = AreaData.Get(scene).Jumpthru;
            if (!string.IsNullOrEmpty(overrideTexture) && !overrideTexture.Equals("default"))
            {
                jumpthru = overrideTexture;
            }
            if (overrideSoundIndex > 0)
            {
                SurfaceSoundIndex = overrideSoundIndex;
            }
            else
            {
                switch (jumpthru.ToLower())
                {
                    case "dream":
                        SurfaceSoundIndex = 32;
                        break;
                    case "temple":
                    case "templeb":
                        SurfaceSoundIndex = 8;
                        break;
                    case "core":
                        SurfaceSoundIndex = 3;
                        break;
                    default:
                        SurfaceSoundIndex = 5;
                        break;
                }
            }
            MTexture mTexture = GFX.Game["objects/jumpthru/" + jumpthru];
            int num = mTexture.Width / 8;
            for (int i = 0; i < columns; i++)
            {
                int num2;
                int num3;
                if (i == 0)
                {
                    num2 = 0;
                    num3 = ((!CollideCheck<SolidTiles>(Position + new Vector2(-1f, 0f))) ? 1 : 0);
                }
                else if (i == columns - 1)
                {
                    num2 = num - 1;
                    num3 = ((!CollideCheck<SolidTiles>(Position + new Vector2(1f, 0f))) ? 1 : 0);
                }
                else
                {
                    num2 = 1 + Calc.Random.Next(num - 2);
                    num3 = Calc.Random.Choose(0, 1);
                }
                Image image = new(mTexture.GetSubtexture(num2 * 8, num3 * 8, 8, 8));
                image.X = i * 8;
                Add(image);
            }
        }
    }
}
