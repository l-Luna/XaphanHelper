using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/JumpBlocksFlipSoundController")]
    class JumpBlocksFlipSoundController : Entity
    {
        public string onSound;

        public string offSound;

        public JumpBlocksFlipSoundController(EntityData data, Vector2 position) : base(data.Position + position)
        {
            onSound = data.Attr("onSound");
            offSound = data.Attr("offSound");
        }

        public override void Update()
        {
            base.Update();
            if (SceneAs<Level>().Tracker.GetEntities<JumpBlock>().Count == 0)
            {
                RemoveSelf();
            }
        }
    }
}
