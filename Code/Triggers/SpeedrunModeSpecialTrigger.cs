using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/SpeedrunModeSpecialTrigger")]
    class SpeedrunModeSpecialTrigger : Trigger
    {
        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        public string[] flags;

        public bool triggered;

        public SpeedrunModeSpecialTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flags = data.Attr("flags").Split(',');
        }

        public override void Update()
        {
            base.Update();
            if (Settings.SpeedrunMode && !SceneAs<Level>().Session.StartedFromBeginning && !triggered)
            {
                foreach (string flag in flags)
                {
                    if (!SceneAs<Level>().Session.GetFlag(flag))
                    {
                        SceneAs<Level>().Session.SetFlag(flag, true);
                    }
                    if (flag == "Upgrade_DashBoots")
                    {
                        Settings.DashBoots = true;
                    }
                    if (flag == "Upgrade_PowerGrip")
                    {
                        Settings.PowerGrip = true;
                    }
                    if (flag == "Upgrade_ClimbingKit")
                    {
                        Settings.ClimbingKit = true;
                    }
                    if (flag == "Upgrade_Bombs")
                    {
                        Settings.Bombs = true;
                    }
                    if (flag == "Upgrade_SpaceJump")
                    {
                        Settings.SpaceJump = 2;
                    }
                    if (flag == "Upgrade_LightningDash")
                    {
                        Settings.LightningDash = true;
                    }
                    if (flag == "Upgrade_GravityJacket")
                    {
                        Settings.GravityJacket = true;
                    }
                }
                triggered = true;
            }
        }
    }
}
