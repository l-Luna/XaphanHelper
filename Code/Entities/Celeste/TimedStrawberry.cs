using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [RegisterStrawberry(true, false)]
    [CustomEntity("XaphanHelper/TimedStrawberry")]
    class TimedStrawberry : Strawberry
    {
        public bool keepEvenIfTimerRunOut;

        public TimedStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid)
        {
            keepEvenIfTimerRunOut = data.Bool("keepEvenIfTimerRunOut", false);
        }

        public static void Load()
        {
            On.Celeste.Strawberry.OnAnimate += OnStrawberryOnAnimate;
        }

        public static void Unload()
        {
            On.Celeste.Strawberry.OnAnimate -= OnStrawberryOnAnimate;
        }

        private static void OnStrawberryOnAnimate(On.Celeste.Strawberry.orig_OnAnimate orig, Strawberry self, string id)
        {
            if ((self is TimedStrawberry && self.Visible && self.Collidable) || !(self is TimedStrawberry))
            {
                orig(self, id);
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            DynData<Strawberry> strawberryData = new(this);
            BloomPoint strawberryBloom = strawberryData.Get<BloomPoint>("bloom");
            bool isGhostBerry = strawberryData.Get<bool>("isGhostBerry");
            strawberryBloom.Visible = false;
            Visible = false;
            Collidable = false;
            SceneAs<Level>().Add(new StrawberryIndicator(Position, isGhostBerry));
        }

        public void Appear()
        {
            for (int i = 0; i < 6; i++)
            {
                float num = Calc.Random.NextFloat((float)Math.PI * 2f);
                SceneAs<Level>().ParticlesFG.Emit(StrawberrySeed.P_Burst, 1, Position + Calc.AngleToVector(num, 4f), Vector2.Zero, num);
            }
            Visible = true;
            DynData<Strawberry> strawberryData = new(this);
            BloomPoint strawberryBloom = strawberryData.Get<BloomPoint>("bloom");
            strawberryBloom.Visible = true;
            Collidable = true;
            foreach (StrawberryIndicator outline in Scene.Tracker.GetEntities<StrawberryIndicator>())
            {
                outline.Hide();
            }
        }

        public void Hide()
        {
            Leader leader = Follower.Leader;
            if (leader != null)
            {
                leader.LoseFollower(Follower);
            }
            Audio.Play("event:/game/general/seed_poof", Position);
            for (int i = 0; i < 6; i++)
            {
                float num = Calc.Random.NextFloat((float)Math.PI * 2f);
                SceneAs<Level>().ParticlesFG.Emit(StrawberrySeed.P_Burst, 1, Position + Calc.AngleToVector(num, 4f), Vector2.Zero, num);
            }
            Visible = false;
            DynData<Strawberry> strawberryData = new(this);
            BloomPoint strawberryBloom = strawberryData.Get<BloomPoint>("bloom");
            strawberryBloom.Visible = false;
            Collidable = false;
            foreach (StrawberryIndicator outline in Scene.Tracker.GetEntities<StrawberryIndicator>())
            {
                outline.Appear();
            }
        }
    }
}
