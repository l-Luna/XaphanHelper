using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/SpeedrunModeSpecialTrigger")]
    class SpeedrunModeSpecialTrigger : Trigger
    {
        public string[] flags;

        public bool triggered;

        public SpeedrunModeSpecialTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flags = data.Attr("flags").Split(',');
        }

        public override void Update()
        {
            base.Update();
            if (XaphanModule.ModSettings.SpeedrunMode && !SceneAs<Level>().Session.StartedFromBeginning && !triggered)
            {
                foreach (string flag in flags)
                {
                    if (!SceneAs<Level>().Session.GetFlag(flag))
                    {
                        SceneAs<Level>().Session.SetFlag(flag, true);
                    }
                    if (flag == "Upgrade_DashBoots")
                    {
                        XaphanModule.ModSettings.DashBoots = true;
                    }
                    if (flag == "Upgrade_PowerGrip")
                    {
                        XaphanModule.ModSettings.PowerGrip = true;
                    }
                    if (flag == "Upgrade_ClimbingKit")
                    {
                        XaphanModule.ModSettings.ClimbingKit = true;
                    }
                    if (flag == "Upgrade_Bombs")
                    {
                        XaphanModule.ModSettings.Bombs = true;
                    }
                    if (flag == "Upgrade_SpaceJump")
                    {
                        XaphanModule.ModSettings.SpaceJump = 2;
                    }
                    if (flag == "Upgrade_LightningDash")
                    {
                        XaphanModule.ModSettings.LightningDash = true;
                    }
                    if (flag == "Upgrade_GravityJacket")
                    {
                        XaphanModule.ModSettings.GravityJacket = true;
                    }
                }
                triggered = true;
            }
        }
    }
}
