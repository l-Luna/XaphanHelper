using System;
using System.Collections.Generic;
using System.Xml;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Hooks
{
    internal class DecalsRegisteryUpdate
    {
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
                    if (item.Key == "XaphanHelper_flagsHide" || item.Key == "XaphanHelper_flagsHideRoom")
                    {
                        string flag = "";
                        string room = "";
                        bool inverted = false;
                        foreach (XmlAttribute attribute in item.Value)
                        {
                            if (attribute.Name == "flag")
                            {
                                flag = attribute.Value;
                            }
                            if (attribute.Name == "inverted")
                            {
                                inverted = bool.Parse(attribute.Value);
                            }
                            if (item.Key == "XaphanHelper_flagsHideRoom" && attribute.Name == "room")
                            {
                                room = attribute.Value;
                            }
                        }
                        if (item.Key == "XaphanHelper_flagsHideRoom" ? self.SceneAs<Level>().Session.Level == room : true)
                        {
                            self.Visible = inverted ? self.SceneAs<Level>().Session.GetFlag(flag) : !self.SceneAs<Level>().Session.GetFlag(flag);
                        }
                    }
                }
            }
        }

        private static void onDecalUpdate(On.Celeste.Decal.orig_Update orig, Decal self)
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
                        }
                        foreach (string flag in flags.Split(','))
                        {
                            self.Visible = inverted ? self.SceneAs<Level>().Session.GetFlag(flag) : !self.SceneAs<Level>().Session.GetFlag(flag);
                        }
                    }
                    if (item.Key == "XaphanHelper_flagsHideRoom")
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
                        if (self.SceneAs<Level>().Session.Level == room)
                        {
                            foreach (string flag in flags.Split(','))
                            {
                                self.Visible = inverted ? self.SceneAs<Level>().Session.GetFlag(flag) : !self.SceneAs<Level>().Session.GetFlag(flag);
                            }
                        }
                    }
                    if (item.Key == "XaphanHelper_flagSwapOffset")
                    {
                        string flag = "";
                        string offPath = "";
                        string onPath = "";
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
                        }
                        if (!string.IsNullOrEmpty(flag) && !string.IsNullOrEmpty(offPath) && !string.IsNullOrEmpty(onPath) && self.SceneAs<Level>().Session.GetFlag(flag))
                        {
                            self.MakeFlagSwap(flag, offPath, onPath);
                        }
                    }

                    if (item.Key == "XaphanHelper_flagSwapRoom")
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
                        if (!string.IsNullOrEmpty(flag) && !string.IsNullOrEmpty(offPath) && !string.IsNullOrEmpty(onPath) && self.SceneAs<Level>().Session.GetFlag(flag) && self.SceneAs<Level>().Session.Level == room)
                        {
                            self.MakeFlagSwap(flag, offPath, onPath);
                        }
                    }
                }
            }
            orig(self);
        }
    }
}
