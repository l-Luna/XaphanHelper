using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;
using On.Celeste;
using System;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class ActorBarrier : Solid
    {
        public string Side;

        private bool UpsideDown;

        private static bool CanJumpThrough;

        public ActorBarrier(Vector2 position, int width, int height, int soundIndex, string side, bool upsideDown, bool canJumpThrough) : base(position, width, height, true)
        {
            Collider = new Hitbox(width, height);
            SurfaceSoundIndex = soundIndex;
            UpsideDown = upsideDown;
            Side = side;
            CanJumpThrough = canJumpThrough;
            Add(new LightOcclude(1f));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Collidable = false;
        }

        public static void Load()
        {
            On.Celeste.TheoCrystal.Update += TheoCrystalOnUpdate;
            On.Celeste.TheoCrystal.OnCollideH += TheoCrystalOnOnCollideH;
            On.Celeste.Glider.Update += GliderOnUpdate;
            On.Celeste.Glider.OnCollideH += GliderOnOnCollideH;
            On.Celeste.Puffer.Update += PufferOnUpdate;
            On.Celeste.Seeker.Update += SeekerOnUpdate;
            On.Celeste.Debris.Update += DebrisOnUpdate;
        }

        public static void Unload()
        {
            On.Celeste.TheoCrystal.Update -= TheoCrystalOnUpdate;
            On.Celeste.TheoCrystal.OnCollideH -= TheoCrystalOnOnCollideH;
            On.Celeste.Glider.Update -= GliderOnUpdate;
            On.Celeste.Glider.OnCollideH -= GliderOnOnCollideH;
            On.Celeste.Puffer.Update -= PufferOnUpdate;
            On.Celeste.Seeker.Update -= SeekerOnUpdate;
            On.Celeste.Debris.Update -= DebrisOnUpdate;
        }

        private static void TheoCrystalOnUpdate(On.Celeste.TheoCrystal.orig_Update orig, TheoCrystal self)
        {
            if (self.GetType() != typeof(TheoCrystal))
            {
                orig(self);
                return;
            }
            SetCollisionBeforeUpdate(self);
            orig(self);
            SetCollisionAfterUpdate(self);
        }

        private static void TheoCrystalOnOnCollideH(On.Celeste.TheoCrystal.orig_OnCollideH orig, TheoCrystal self, CollisionData data)
        {
            if (data.Hit is ActorBarrier)
            {
                self.Speed.X *= -0.4f;
            }
            else
            {
                orig(self, data);
            }
        }

        private static void GliderOnUpdate(On.Celeste.Glider.orig_Update orig, Glider self)
        {
            if (self.GetType() != typeof(Glider))
            {
                orig(self);
                return;
            }
            SetCollisionBeforeUpdate(self);
            orig(self);
            SetCollisionAfterUpdate(self);
        }

        private static void GliderOnOnCollideH(On.Celeste.Glider.orig_OnCollideH orig, Glider self, CollisionData data)
        {
            if (data.Hit is ActorBarrier)
            {
                if (self.Speed.X < -50f)
                {
                    Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_left", self.Position);
                }
                else if (self.Speed.X > 50f)
                {
                    Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_right", self.Position);
                }
                self.Speed.X *= -1f;
            }
            else
            {
                orig(self, data);
            }
        }

        private static void PufferOnUpdate(On.Celeste.Puffer.orig_Update orig, Puffer self)
        {
            if (self.GetType() != typeof(Puffer))
            {
                orig(self);
                return;
            }
            SetCollisionBeforeUpdate(self);
            orig(self);
            SetCollisionAfterUpdate(self);
        }

        private static void SeekerOnUpdate(On.Celeste.Seeker.orig_Update orig, Seeker self)
        {
            if (self.GetType() != typeof(Seeker))
            {
                orig(self);
                return;
            }
            SetCollisionBeforeUpdate(self);
            orig(self);
            SetCollisionAfterUpdate(self);
        }

        private static void DebrisOnUpdate(On.Celeste.Debris.orig_Update orig, Debris self)
        {
            if (self.GetType() != typeof(Debris))
            {
                orig(self);
                return;
            }
            SetCollisionBeforeUpdate(self);
            orig(self);
            SetCollisionAfterUpdate(self);
        }

        public static void SetCollisionBeforeUpdate(Actor actor)
        {
            List<Entity> actorBarriers = actor.Scene.Tracker.GetEntities<ActorBarrier>().ToList();
            List<Entity> playerPlatforms = actor.Scene.Tracker.GetEntities<PlayerPlatform>().ToList();
            foreach (Entity entity in actorBarriers)
            {
                ActorBarrier barrier = (ActorBarrier)entity;
                if (barrier.Height > 1 || barrier.UpsideDown)
                {
                    if (CanJumpThrough)
                    {
                        if (barrier.Side == "Left")
                        {
                            if (actor.Left >= barrier.Right)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                        else
                        {
                            if (actor.Right <= barrier.Left)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                    }
                    else
                    {
                        barrier.Collidable = true;
                    }
                }
                else if (actor.Center.X > entity.Left && actor.Center.X < entity.Right)
                {
                    if (CanJumpThrough)
                    {
                        if (!barrier.UpsideDown)
                        {
                            if (actor.Bottom <= barrier.Top)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                        else
                        {
                            if (actor.Top >= barrier.Bottom)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                    }
                    else
                    {
                        barrier.Collidable = true;
                    }
                }
            }
            foreach (PlayerPlatform platform in playerPlatforms)
            {
                platform.Collidable = false;
            }
        }

        /*public static void SetCollisionBeforeUpdate(MovableEntity actor)
        {
            List<Entity> actorBarriers = actor.Scene.Tracker.GetEntities<ActorBarrier>().ToList();
            List<Entity> playerPlatforms = actor.Scene.Tracker.GetEntities<PlayerPlatform>().ToList();
            foreach (Entity entity in actorBarriers)
            {
                ActorBarrier barrier = (ActorBarrier)entity;
                if (barrier.Height > 1 || barrier.UpsideDown)
                {
                    if (CanJumpThrough)
                    {
                        if (barrier.Side == "Left")
                        {
                            if (actor.Left >= barrier.Right)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                        else
                        {
                            if (actor.Right <= barrier.Left)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                    }
                    else
                    {
                        barrier.Collidable = true;
                    }
                }
                else if (actor.Center.X > entity.Left && actor.Center.X < entity.Right)
                {
                    if (CanJumpThrough)
                    {
                        if (!barrier.UpsideDown)
                        {
                            if (actor.Bottom <= barrier.Top)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                        else
                        {
                            if (actor.Top >= barrier.Bottom)
                            {
                                barrier.Collidable = true;
                            }
                            else
                            {
                                barrier.Collidable = false;
                            }
                        }
                    }
                    else
                    {
                        barrier.Collidable = true;
                    }
                }
            }
            foreach (PlayerPlatform platform in playerPlatforms)
            {
                platform.Collidable = false;
            }
        }*/

        public static void SetCollisionAfterUpdate(Actor actor)
        {
            List<Entity> actorBarriers = actor.Scene.Tracker.GetEntities<ActorBarrier>().ToList();
            Player player = actor.Scene.Tracker.GetEntity<Player>();
            actorBarriers.ForEach(entity => entity.Collidable = false);
            foreach (PlayerPlatform platform in actor.Scene.Tracker.GetEntities<PlayerPlatform>())
            {
                platform.SetCollision(player);
            }
        }

        /*public static void SetCollisionAfterUpdate(MovableEntity actor)
        {
            List<Entity> actorBarriers = actor.Scene.Tracker.GetEntities<ActorBarrier>().ToList();
            Player player = actor.Scene.Tracker.GetEntity<Player>();
            actorBarriers.ForEach(entity => entity.Collidable = false);
            foreach (PlayerPlatform platform in actor.Scene.Tracker.GetEntities<PlayerPlatform>())
            {
                platform.SetCollision(player);
            }
        }*/

        public override void Update()
        {
            base.Update();
            if (SceneAs<Level>().Tracker.GetEntities<DroneDebris>().Count > 0 && !CanJumpThrough)
            {
                Collidable = true;
            }
            else
            {
                Collidable = false;
            }
        }

    }
}
