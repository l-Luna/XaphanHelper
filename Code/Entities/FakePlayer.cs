using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    public class FakePlayer : Player
    {
        public FakePlayer(Vector2 position, PlayerSpriteMode spriteMode) : base (position, spriteMode)
        {

        }

        public override void Update()
        {
            List<Entity> fakePlayerPlatforms = Scene.Tracker.GetEntities<FakePlayerPlatform>().ToList();
            List<Entity> playerPlatforms = Scene.Tracker.GetEntities<PlayerPlatform>().ToList();
            fakePlayerPlatforms.ForEach(entity => entity.Collidable = true);
            playerPlatforms.ForEach(entity => entity.Collidable = false);
            base.Update();
            fakePlayerPlatforms.ForEach(entity => entity.Collidable = false);
            playerPlatforms.ForEach(entity => entity.Collidable = true);
        }
    }
}
