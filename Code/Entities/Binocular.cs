using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class Binocular : Lookout
    {
        private MethodInfo LookoutInteract = typeof(Lookout).GetMethod("Interact", BindingFlags.Instance | BindingFlags.NonPublic);

        public Binocular(EntityData data, Vector2 offset) : base(data, offset)
        {
            
        }

        public static void Load()
        {
            On.Celeste.Lookout.LookRoutine += onLookoutLookRoutine;
        }

        public static void Unload()
        {
            On.Celeste.Lookout.LookRoutine -= onLookoutLookRoutine;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (SceneAs<Level>().Tracker.CountEntities<Binocular>() > 1)
            {
                foreach (Entity binocular in SceneAs<Level>().Tracker.GetEntities<Binocular>())
                {
                    if (binocular != this)
                    {
                        binocular.RemoveSelf();
                    }
                }
            }
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            LookoutInteract.Invoke(this, new object[] { player });
        }

        private static IEnumerator onLookoutLookRoutine(On.Celeste.Lookout.orig_LookRoutine orig, Lookout self, Player player)
        {
            yield return new SwapImmediately(orig(self, player));
            if (self is Binocular)
            {
                player.SceneAs<Level>().Displacement.AddBurst(player.Center, 0.5f, 8f, 32f, 0.5f);
                self.RemoveSelf();
            }
        }
    }
}
