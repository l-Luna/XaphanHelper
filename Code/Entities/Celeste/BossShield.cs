using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.Entities
{
    public class BossShield : Entity
    {
        public readonly int maxQuantity;

        private readonly float radius;

        private readonly float rotationTime;

        private readonly bool clockwise;

        public readonly List<BossShieldOrb> orbs;

        private CustomFinalBoss boss;

        private float rotationPercent;

        public BossShield(CustomFinalBoss boss, int maxQuantity, int radius, float rotationTime, bool clockwise)
        {
            this.boss = boss;
            this.maxQuantity = maxQuantity;
            this.radius = radius;
            this.rotationTime = rotationTime;
            this.clockwise = clockwise;
            rotationPercent = 0f;
            orbs = new List<BossShieldOrb>();
        }

        private static float GetAngle(float rotationPercent)
        {
            return MathHelper.Lerp(3.141593f, -3.141593f, Easer(rotationPercent));
        }

        private static float Easer(float value)
        {
            return value;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (boss == null)
            {
                RemoveSelf();
            }
            for (int i = 0; i < maxQuantity; i++)
            {
                AddOrb(i != 0);
            }
        }

        private void ResetPosition()
        {
            if (clockwise)
            {
                rotationPercent -= Engine.DeltaTime / rotationTime;
                rotationPercent += 1f;
            }
            else
            {
                rotationPercent += Engine.DeltaTime / rotationTime;
            }
            rotationPercent %= 1f;
            for (int i = 0; i < orbs.Count; i++)
            {
                BossShieldOrb shieldOrb = orbs[i];
                float num = (rotationPercent + i * 1f / orbs.Count) % 1f;
                shieldOrb.Position = boss.Center + Calc.AngleToVector(GetAngle(num), radius);
            }
        }

        public override void Update()
        {
            base.Update();
            if (boss == null)
            {
                RemoveSelf();
            }
            ResetPosition();
        }

        public void AddOrb(bool silent = false)
        {
            BossShieldOrb shieldOrb = new BossShieldOrb();
            shieldOrb.Add(new Coroutine(Appear(shieldOrb)));
            Scene.Add(shieldOrb);
        }

        private IEnumerator Appear(BossShieldOrb orb)
        {
            orbs.Add(orb);
            ResetPosition();
            yield break;
        }

        public void Disappear(BossShieldOrb orb)
        {
            orb.RemoveSelf();
        }

        public override void Removed(Scene scene)
        {
            orbs.ForEach(delegate (BossShieldOrb orb)
            {
                Disappear(orb);
            });
            base.Removed(scene);
        }
    }

}