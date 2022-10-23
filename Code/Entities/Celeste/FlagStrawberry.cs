using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [RegisterStrawberry(true, false)]
    [CustomEntity("XaphanHelper/FlagStrawberry")]
    class FlagStrawberry : Strawberry
    {
        private bool tryToSetFlag;

        private string Flag;

        private FieldInfo Strawberry_sprite = typeof(Strawberry).GetField("sprite", BindingFlags.Instance | BindingFlags.NonPublic);

        public FlagStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid)
        {
            Flag = data.Attr("flag");
        }

        public override void Update()
        {
            base.Update();
            if (this is FlagStrawberry)
            {
                Sprite sprite = (Sprite)Strawberry_sprite.GetValue(this);
                if (sprite.CurrentAnimationID == "collect" && !string.IsNullOrEmpty(Flag) && !tryToSetFlag)
                {
                    tryToSetFlag = true;
                    SceneAs<Level>().Session.SetFlag(Flag);
                }
            }
        }
    }
}
