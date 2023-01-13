using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/TouchSwitchFlagController")]
    class TouchSwitchFlagController : Entity
    {
        string flag;

        public TouchSwitchFlagController(EntityData data, Vector2 position) : base(data.Position + position)
        {
            Tag = Tags.TransitionUpdate;
            flag = data.Attr("flag");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!string.IsNullOrEmpty(flag) && !SceneAs<Level>().Session.GetFlag("switches_" + SceneAs<Level>().Session.Level))
            {
                SceneAs<Level>().Session.SetFlag(flag, false);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!string.IsNullOrEmpty(flag))
            {
                foreach (Switch switchCmp in SceneAs<Level>().Tracker.GetComponents<Switch>())
                {
                    if (switchCmp.Finished)
                    {
                        SceneAs<Level>().Session.SetFlag(flag, true);
                    }
                }
            }
        }
    }
}
