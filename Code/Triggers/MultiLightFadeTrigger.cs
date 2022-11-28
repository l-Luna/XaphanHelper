using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/MultiLightFadeTrigger")]
    class MultiLightFadeTrigger : Trigger
    {
        public float LightAddFrom;

        public float LightAddTo;

        public float FlagLightAddFrom;

        public float FlagLightAddTo;

        public string flag;

        public PositionModes PositionMode;

        public MultiLightFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            AddTag(Tags.TransitionUpdate);
            LightAddFrom = data.Float("lightAddFrom");
            LightAddTo = data.Float("lightAddTo");
            FlagLightAddFrom = data.Float("flaglightAddFrom");
            FlagLightAddTo = data.Float("flaglightAddTo");
            flag = data.Attr("flag");
            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);
        }

        public override void OnStay(Player player)
        {
            Level level = Scene as Level;
            Session session = level.Session;
            float num = 0f;
            if (session.GetFlag(flag))
            {
                num = session.LightingAlphaAdd = FlagLightAddFrom + (FlagLightAddTo - FlagLightAddFrom) * MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
            }
            else
            {
                num = session.LightingAlphaAdd = LightAddFrom + (LightAddTo - LightAddFrom) * MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
            }
            level.Lighting.Alpha = level.BaseLightingAlpha + num;
        }
    }
}
