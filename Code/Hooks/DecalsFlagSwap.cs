using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Hooks
{
    internal static class DecalsRegisteryUpdate
    {
        public static List<Decal> SwapedDecals = new();

        public static void Load()
        {
            On.Celeste.Decal.Added += onDecalAdded;
            On.Celeste.Decal.Update += onDecalUpdate;
        }

        public static void Unload()
        {
            On.Celeste.Decal.Added -= onDecalAdded;
            On.Celeste.Decal.Update -= onDecalUpdate;
        }

        private static void onDecalAdded(On.Celeste.Decal.orig_Added orig, Decal self, Scene scene)
        {
            orig(self, scene);
            UpdateDecal(self);
        }

        private static void onDecalUpdate(On.Celeste.Decal.orig_Update orig, Decal self)
        {
            UpdateDecal(self);
            orig(self);
        }

        private static void UpdateDecal(Decal self)
        {
            string text = self.Name.ToLower();
            if (text.StartsWith("decals/"))
            {
                text = text.Substring(7);
            }
            if (DecalRegistry.RegisteredDecals.ContainsKey(text))
            {
                DecalRegistry.DecalInfo decalInfo = DecalRegistry.RegisteredDecals[text];
                foreach (KeyValuePair<string, XmlAttributeCollection> item in decalInfo.CustomProperties)
                {
                    if (item.Key == "XaphanHelper_flagsHide")
                    {
                        string flags = "";
                        string room = "";
                        bool inverted = false;
                        foreach (XmlAttribute attribute in item.Value)
                        {
                            if (attribute.Name == "flags")
                            {
                                flags = attribute.Value;
                            }
                            if (attribute.Name == "inverted")
                            {
                                inverted = bool.Parse(attribute.Value);
                            }
                            if (attribute.Name == "room")
                            {
                                room = attribute.Value;
                            }
                        }
                        if (!string.IsNullOrEmpty(room) ? self.SceneAs<Level>().Session.Level == room : true)
                        {
                            foreach (string flag in flags.Split(','))
                            {
                                self.Visible = inverted ? self.SceneAs<Level>().Session.GetFlag(flag) : !self.SceneAs<Level>().Session.GetFlag(flag);
                            }
                        }
                    }
                    if (item.Key == "XaphanHelper_flagSwap")
                    {
                        string flag = "";
                        string offPath = "";
                        string onPath = "";
                        string room = "";
                        foreach (XmlAttribute attribute in item.Value)
                        {
                            if (attribute.Name == "flag")
                            {
                                flag = attribute.Value;
                            }
                            if (attribute.Name == "offPath")
                            {
                                offPath = attribute.Value;
                            }
                            if (attribute.Name == "onPath")
                            {
                                onPath = attribute.Value;
                            }
                            if (attribute.Name == "room")
                            {
                                room = attribute.Value;
                            }
                        }
                        string[] onPaths = onPath.Split(',');
                        if (!string.IsNullOrEmpty(flag) && !string.IsNullOrEmpty(offPath) && !string.IsNullOrEmpty(onPath) && (!string.IsNullOrEmpty(room) ? self.SceneAs<Level>().Session.Level == room : true))
                        {
                            if (self.SceneAs<Level>().Session.GetFlag(flag) && !SwapedDecals.Contains(self))
                            {
                                SwapedDecals.Add(self);
                                self.Remove(self.Image);
                                int textures = onPaths.Length;
                                if (textures > 1)
                                {
                                    int texture = Calc.Random.Next(textures);
                                    self.MakeFlagSwap(flag, offPath, onPaths[texture]);
                                }
                                else
                                {
                                    self.MakeFlagSwap(flag, offPath, onPath);
                                }
                            }
                            else if (!self.SceneAs<Level>().Session.GetFlag(flag) && SwapedDecals.Contains(self))
                            {
                                SwapedDecals.Remove(self);
                                self.Remove(self.Image);
                                self.MakeFlagSwap(flag, offPath, onPath);
                            }
                        }
                    }
                }
            }
        }
    }
}
