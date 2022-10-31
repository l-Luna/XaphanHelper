using Celeste.Mod.XaphanHelper.UI_Elements;
using Monocle;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.XaphanHelper
{
    class Commands
    {
        private static Level level = Engine.Scene as Level;

        private static string prefix = level.Session.Area.GetLevelSet();

        private static int chapterIndex = level.Session.Area.ChapterIndex == -1 ? 0 : level.Session.Area.ChapterIndex;

        // In-Game Map commands

        [Command("clear_ingamemap", "Clear the In-Game map for the current chapter or all chapters")]
        public static void Cmd_Clear_InGameMap(bool allChapters = false, bool includeCurrentRoom = false)
        {
            List<string> TilesToRemove = new List<string>();
            List<string> ExtraUnexploredRoomsTilesToRemove = new List<string>();
            List<string> FlagsToRemove = new List<string>();
            if (!allChapters)
            {
                foreach (string visitedRoom in XaphanModule.ModSaveData.VisitedRooms)
                {
                    if (visitedRoom.Contains(prefix + "/Ch" + chapterIndex + "/") && (!includeCurrentRoom ? !visitedRoom.Contains(level.Session.Level) : true))
                    {
                        TilesToRemove.Add(visitedRoom);
                    }
                }
                foreach (string visitedRoomTile in XaphanModule.ModSaveData.VisitedRoomsTiles)
                {
                    if (visitedRoomTile.Contains(prefix + "/Ch" + chapterIndex + "/"))
                    {
                        TilesToRemove.Add(visitedRoomTile);
                    }
                }
                foreach (string extraUnexploredRoomTile in XaphanModule.ModSaveData.ExtraUnexploredRooms)
                {
                    if (extraUnexploredRoomTile.Contains(prefix + "/Ch" + chapterIndex + "/"))
                    {
                        ExtraUnexploredRoomsTilesToRemove.Add(extraUnexploredRoomTile);
                    }
                }
                foreach (string flag in XaphanModule.ModSaveData.SavedFlags)
                {
                    if (flag.Contains(prefix + "/Ch" + chapterIndex + "_MapShard"))
                    {
                        FlagsToRemove.Add(flag);
                    }
                }
            }
            else
            {
                foreach (string visitedRoom in XaphanModule.ModSaveData.VisitedRooms)
                {
                    if (visitedRoom.Contains(prefix) && (!includeCurrentRoom ? !visitedRoom.Contains(level.Session.Level) : true))
                    {
                        TilesToRemove.Add(visitedRoom);
                    }
                }
                foreach (string visitedRoomTile in XaphanModule.ModSaveData.VisitedRoomsTiles)
                {
                    if (visitedRoomTile.Contains(prefix))
                    {
                        TilesToRemove.Add(visitedRoomTile);
                    }
                }
                foreach (string extraUnexploredRoomTile in XaphanModule.ModSaveData.ExtraUnexploredRooms)
                {
                    if (extraUnexploredRoomTile.Contains(prefix))
                    {
                        ExtraUnexploredRoomsTilesToRemove.Add(extraUnexploredRoomTile);
                    }
                }
                foreach (string flag in XaphanModule.ModSaveData.SavedFlags)
                {
                    if (flag.Contains(prefix) && flag.Contains("_MapShard"))
                    {
                        FlagsToRemove.Add(flag);
                    }
                }
            }
            foreach (string value in TilesToRemove)
            {
                XaphanModule.ModSaveData.VisitedRooms.Remove(value);
                XaphanModule.ModSaveData.VisitedRoomsTiles.Remove(value);
            }
            foreach (string value in ExtraUnexploredRoomsTilesToRemove)
            {
                XaphanModule.ModSaveData.ExtraUnexploredRooms.Remove(value);
            }
            foreach ( string value in FlagsToRemove)
            {
                XaphanModule.ModSaveData.SavedFlags.Remove(value);
            }
        }

        [Command("reveal_ingamemap", "Reveal or hide all rooms on the In-Game map")]
        private static void Cmd_Reveal_InGameMap(bool state = true)
        {
            if (XaphanModule.useIngameMap)
            {
                MapDisplay.ForceRevealUnexploredRooms = state;
                MiniMap minimap = level.Tracker.GetEntity<MiniMap>();
                if (minimap != null && !level.Session.GetFlag("Map_Opened"))
                {
                    minimap.RemoveSelf();
                }
                if (level.Session.GetFlag("Map_Opened"))
                {
                    MapDisplay display = level.Tracker.GetEntity<MapDisplay>();
                    if (display != null)
                    {
                        display.UpdateMap(chapterIndex, level.Session.Level, 0);
                    }
                }
            }
        }

        // Warps Commands

        [Command("clear_warps", "Clear all warps unlocked in this campaign")]
        public static void Cmd_Clear_Warps()
        {
            List<string> ToRemove = new List<string>();
            foreach (string warp in XaphanModule.ModSaveData.UnlockedWarps)
            {
                if (warp.Contains(prefix))
                {
                    ToRemove.Add(warp);
                }
            }
            foreach (string value in ToRemove)
            {
                XaphanModule.ModSaveData.UnlockedWarps.Remove(value);
            }
        }

        // Upgrades Commands

        [Command("remove_upgrade", "Remove specified upgrades (Separated by a semicolon). If no upgrades are specified, remove all upgrades. Avaiable upgrades: PowerGrip, ClimbingKit, SpiderMagnet, DashBoots, SpaceJump, HoverBoots, LightningDash, LongBeam, IceBeam, WaveBeam, DroneTeleport, VariaJacket, GravityJacket, Bombs, MegaBombs, RemoteDrone, GoldenFeather, EtherealDash, ScrewAttack, Binoculars, PortableStation, PulseRadar")]
        public static void Cmd_Remove_Upgrades(string upg = "")
        {
            if (XaphanModule.useUpgrades)
            {
                if (upg == "")
                {
                    upg = "PowerGrip;ClimbingKit;SpiderMagnet;DashBoots;SpaceJump;HoverBoots;LightningDash;LongBeam;IceBeam;WaveBeam;DroneTeleport;VariaJacket;GravityJacket;Bombs;MegaBombs;RemoteDrone;GoldenFeather;EtherealDash;ScrewAttack;Binoculars;PortableStation;PulseRadar";
                }
                string[] upgrades = upg.Split(';');
                foreach(string u in upgrades)
                {
                    foreach (XaphanModule.Upgrades upgrade in XaphanModule.Instance.UpgradeHandlers.Keys)
                    {
                        if (u == upgrade.ToString())
                        {
                            if (u == "SpaceJump")
                            {
                                XaphanModule.Instance.UpgradeHandlers[upgrade].SetValue(1);
                            }
                            else
                            {
                                XaphanModule.Instance.UpgradeHandlers[upgrade].SetValue(0);
                            }
                            level.Session.SetFlag("Upgrade_" + upgrade.ToString(), false);
                            if (XaphanModule.ModSaveData.SavedFlags.Contains(prefix + "_Upgrade_" + upgrade.ToString()))
                            {
                                XaphanModule.ModSaveData.SavedFlags.Remove(prefix + "_Upgrade_" + upgrade.ToString());
                            }
                        }
                    }
                }
            }
        }

        [Command("give_upgrade", "Give specified upgrades (Separated by a semicolon). If no upgrades are specified, give all upgrades. Avaiable upgrades: PowerGrip, ClimbingKit, SpiderMagnet, DashBoots, SpaceJump, HoverBoots, LightningDash, LongBeam, IceBeam, WaveBeam, DroneTeleport, VariaJacket, GravityJacket, Bombs, MegaBombs, RemoteDrone, GoldenFeather, EtherealDash, ScrewAttack, Binoculars, PortableStation, PulseRadar")]
        private static void Cmd_Give_Upgrade(string upg = "")
        {
            if (XaphanModule.useUpgrades)
            {
                if (upg == "")
                {
                    upg = "PowerGrip;ClimbingKit;SpiderMagnet;DashBoots;SpaceJump;HoverBoots;LightningDash;LongBeam;IceBeam;WaveBeam;DroneTeleport;VariaJacket;GravityJacket;Bombs;MegaBombs;RemoteDrone;GoldenFeather;EtherealDash;ScrewAttack;Binoculars;PortableStation;PulseRadar";
                }
                string[] upgrades = upg.Split(';');
                foreach (string u in upgrades)
                {
                    foreach (XaphanModule.Upgrades upgrade in XaphanModule.Instance.UpgradeHandlers.Keys)
                    {
                        if (u == upgrade.ToString())
                        {
                            if (u == "SpaceJump")
                            {
                                XaphanModule.Instance.UpgradeHandlers[upgrade].SetValue(2);
                            }
                            else
                            {
                                XaphanModule.Instance.UpgradeHandlers[upgrade].SetValue(1);
                            }
                            level.Session.SetFlag("Upgrade_" + upgrade.ToString(), true);
                            if (!XaphanModule.ModSaveData.SavedFlags.Contains(prefix + "_Upgrade_" + upgrade.ToString()))
                            {
                                XaphanModule.ModSaveData.SavedFlags.Add(prefix + "_Upgrade_" + upgrade.ToString());
                            }
                        }
                    }
                    ReActivateUpgrade(u);
                }
            }
        }

        public static void ReActivateUpgrade(string upg)
        {
            switch (upg)
            {
                case "PowerGrip":
                    if (XaphanModule.ModSaveData.PowerGripInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.PowerGripInactive.Remove(prefix);
                    }
                    break;
                case "ClimbingKit":
                    if (XaphanModule.ModSaveData.ClimbingKitInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.ClimbingKitInactive.Remove(prefix);
                    }
                    break;
                case "SpiderMagnet":
                    if (XaphanModule.ModSaveData.SpiderMagnetInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.SpiderMagnetInactive.Remove(prefix);
                    }
                    break;
                case "DashBoots":
                    if (XaphanModule.ModSaveData.DashBootsInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.DashBootsInactive.Remove(prefix);
                    }
                    break;
                case "SpaceJump":
                    if (XaphanModule.ModSaveData.SpaceJumpInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.SpaceJumpInactive.Remove(prefix);
                    }
                    break;
                case "HoverBoots":
                    if (XaphanModule.ModSaveData.HoverBootsInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.HoverBootsInactive.Remove(prefix);
                    }
                    break;
                case "LightningDash":
                    if (XaphanModule.ModSaveData.LightningDashInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.LightningDashInactive.Remove(prefix);
                    }
                    break;
                case "DroneTeleport":
                    if (XaphanModule.ModSaveData.DroneTeleportInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.DroneTeleportInactive.Remove(prefix);
                    }
                    break;
                /*case "JumpBoost":
                    if (XaphanModule.ModSaveData.JumpBoostInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.JumpBoostInactive.Remove(prefix);
                    }
                    break;*/
                case "VariaJacket":
                    if (XaphanModule.ModSaveData.VariaJacketInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.VariaJacketInactive.Remove(prefix);
                    }
                    break;
                case "GravityJacket":
                    if (XaphanModule.ModSaveData.GravityJacketInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.GravityJacketInactive.Remove(prefix);
                    }
                    break;
                case "Bombs":
                    if (XaphanModule.ModSaveData.BombsInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.BombsInactive.Remove(prefix);
                    }
                    break;
                case "MegaBombs":
                    if (XaphanModule.ModSaveData.MegaBombsInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.MegaBombsInactive.Remove(prefix);
                    }
                    break;
                case "RemoteDrone":
                    if (XaphanModule.ModSaveData.RemoteDroneInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.RemoteDroneInactive.Remove(prefix);
                    }
                    break;
                case "GoldenFeather":
                    if (XaphanModule.ModSaveData.GoldenFeatherInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.GoldenFeatherInactive.Remove(prefix);
                    }
                    break;
                case "EtherealDash":
                    if (XaphanModule.ModSaveData.EtherealDashInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.EtherealDashInactive.Remove(prefix);
                    }
                    break;
                case "ScrewAttack":
                    if (XaphanModule.ModSaveData.ScrewAttackInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.ScrewAttackInactive.Remove(prefix);
                    }
                    break;
                case "Binoculars":
                    if (XaphanModule.ModSaveData.BinocularsInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.BinocularsInactive.Remove(prefix);
                    }
                    break;
                case "PortableStation":
                    if (XaphanModule.ModSaveData.PortableStationInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.PortableStationInactive.Remove(prefix);
                    }
                    break;
                case "PulseRadar":
                    if (XaphanModule.ModSaveData.PulseRadarInactive.Contains(prefix))
                    {
                        XaphanModule.ModSaveData.PulseRadarInactive.Remove(prefix);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
