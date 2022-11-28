using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/FlagMusicFadeTrigger")]
    public class FlagMusicFadeTrigger : Trigger
    {
        public bool LeftToRight;

        public float FadeA;

        public float FadeB;

        public string Parameter;

        public string flag;

        public bool inverted;

        public FlagMusicFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            LeftToRight = (data.Attr("direction", "leftToRight") == "leftToRight");
            FadeA = data.Float("fadeA");
            FadeB = data.Float("fadeB", 1f);
            Parameter = data.Attr("parameter");
            flag = data.Attr("flag");
            inverted = data.Bool("inverted");
        }

        public override void OnStay(Player player)
        {
            Level level = base.Scene as Level;
            if ((level.Session.GetFlag(flag) && !inverted) || (!level.Session.GetFlag(flag) && inverted))
            {
                Audio.SetMusicParam((!string.IsNullOrEmpty(Parameter)) ? Parameter : "fade", LeftToRight ? Calc.ClampedMap(player.Center.X, Left, Right, FadeA, FadeB) : Calc.ClampedMap(player.Center.Y, Top, Bottom, FadeA, FadeB));
            }
        }
    }
}