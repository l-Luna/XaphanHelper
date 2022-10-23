using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class SpiderMagnet : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.SpiderMagnet ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.SpiderMagnet = (value != 0);
        }

        public override void Load()
        {
            IL.Celeste.Player.NormalUpdate += modNormalUpdate;
        }

        public override void Unload()
        {
            IL.Celeste.Player.NormalUpdate -= modNormalUpdate;
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.SpiderMagnet && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpiderMagnetInactive.Contains(level.Session.Area.GetLevelSet());
        }

        private void modNormalUpdate(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // find out where the constant 900 (downward acceleration) is loaded into the stack
            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(900f)))
            {
                // add two instructions to multiply those constants with the "gravity factor"
                cursor.EmitDelegate<Func<float>>(determineGravityFactor);
                cursor.Emit(OpCodes.Mul);
            }          
        }

        private float determineGravityFactor()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (level.Session.GetFlag("Xaphan_Helper_Ceiling") && !(Active(level) || !XaphanModule.useUpgrades))
                {
                    return 0f;
                }
            }
            return 1f;
        }
    }
}
