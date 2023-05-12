using System;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper
{
    class SaveUpdater
    {
        private static List<string> upgradesToRemove = new();

        public static void UpdateSave(Level level)
        {
            if (XaphanModule.SoCMVersion >= new Version(3, 0, 0))
            {
                if (XaphanModule.ModSaveData.SoCMVer < 300)
                {
                    // Remove Bombs upgrade

                    if (XaphanModule.BombsCollected(level))
                    {
                        upgradesToRemove.Add("Bombs");
                    }

                    Dictionary<string, string> RoomsNamesConversion = new();
                    for (int i = SaveData.Instance.GetLevelSetStats().AreaOffset; i < SaveData.Instance.GetLevelSetStats().AreaOffset + SaveData.Instance.GetLevelSetStats().Areas.Count; i++)
                    {
                        // Get list of new room names

                        RoomsNamesConversion.Clear();
                        if (i == SaveData.Instance.GetLevelSetStats().AreaOffset + 1) // Ancient Ruins
                        {
                            RoomsNamesConversion.Add("A-02", "A-15");
                            RoomsNamesConversion.Add("A-03", "A-07");
                            RoomsNamesConversion.Add("A-04", "A-08");
                            RoomsNamesConversion.Add("A-05", "A-16");
                            RoomsNamesConversion.Add("A-06", "A-10");
                            RoomsNamesConversion.Add("A-07", "A-11");
                            RoomsNamesConversion.Add("B-00", "A-12");
                            RoomsNamesConversion.Add("B-01", "A-13");
                            RoomsNamesConversion.Add("B-02", "A-14");
                            RoomsNamesConversion.Add("B-03", "A-03");
                            RoomsNamesConversion.Add("B-04", "A-18");
                            RoomsNamesConversion.Add("B-05", "A-19");
                            RoomsNamesConversion.Add("B-06", "A-20");
                            RoomsNamesConversion.Add("B-07", "A-21");
                            RoomsNamesConversion.Add("B-08", "A-22");
                            RoomsNamesConversion.Add("B-09", "A-23");
                            RoomsNamesConversion.Add("B-10", "A-24");
                            RoomsNamesConversion.Add("B-11", "A-25");
                            RoomsNamesConversion.Add("B-12", "A-26");
                            RoomsNamesConversion.Add("B-W0", "A-W2");
                            RoomsNamesConversion.Add("C-00", "A-27");
                            RoomsNamesConversion.Add("C-01", "A-28");
                            RoomsNamesConversion.Add("C-02", "A-29");
                            RoomsNamesConversion.Add("C-03", "A-30");
                            RoomsNamesConversion.Add("C-04", "A-35");
                            RoomsNamesConversion.Add("C-05", "A-39");
                            RoomsNamesConversion.Add("C-06", "A-38");
                            RoomsNamesConversion.Add("C-07", "A-32");
                            RoomsNamesConversion.Add("C-08", "A-33");
                            RoomsNamesConversion.Add("C-09", "A-34");
                            RoomsNamesConversion.Add("C-10", "A-37");
                            RoomsNamesConversion.Add("C-11", "A-43");
                            RoomsNamesConversion.Add("C-12", "A-44");
                            RoomsNamesConversion.Add("C-13", "A-45");
                            RoomsNamesConversion.Add("C-W0", "A-W3");
                            RoomsNamesConversion.Add("C-W1", "A-W4");
                        }
                        else if (i == SaveData.Instance.GetLevelSetStats().AreaOffset + 2) // Forgotten Abysses
                        {
                            RoomsNamesConversion.Add("D-00", "C-18");
                            RoomsNamesConversion.Add("D-01", "C-19");
                            RoomsNamesConversion.Add("D-02", "C-20");
                            RoomsNamesConversion.Add("D-03", "C-21");
                            RoomsNamesConversion.Add("D-04", "C-22");
                            RoomsNamesConversion.Add("D-05", "C-23");
                            RoomsNamesConversion.Add("D-06", "C-24");
                            RoomsNamesConversion.Add("D-07", "C-25");
                            RoomsNamesConversion.Add("D-08", "C-26");
                            RoomsNamesConversion.Add("D-09", "C-27");
                            RoomsNamesConversion.Add("D-10", "C-28");
                            RoomsNamesConversion.Add("D-11", "C-29");
                            RoomsNamesConversion.Add("D-12", "C-30");
                            RoomsNamesConversion.Add("D-13", "C-31");
                            RoomsNamesConversion.Add("D-14", "C-32");
                            RoomsNamesConversion.Add("D-15", "C-33");
                            RoomsNamesConversion.Add("D-16", "C-34");
                            RoomsNamesConversion.Add("D-17", "C-35");
                            RoomsNamesConversion.Add("D-18", "C-36");
                            RoomsNamesConversion.Add("D-19", "C-37");
                            RoomsNamesConversion.Add("D-20", "C-38");
                            RoomsNamesConversion.Add("D-21", "C-39");
                            RoomsNamesConversion.Add("D-W0", "C-W2");
                            RoomsNamesConversion.Add("D-W1", "C-W3");
                            RoomsNamesConversion.Add("D-W2", "C-W4");
                        }

                        // Adjust collected strawberries

                        HashSet<EntityID> oldStrawberries = new();
                        HashSet<EntityID> newStrawberries = new();
                        foreach (EntityID strawberry in SaveData.Instance.Areas_Safe[i].Modes[0].Strawberries)
                        {
                            foreach (KeyValuePair<string, string> oldRoom in RoomsNamesConversion)
                            {
                                if (strawberry.Level == oldRoom.Key)
                                {
                                    oldStrawberries.Add(strawberry);
                                    newStrawberries.Add(new EntityID(oldRoom.Value, strawberry.ID));
                                }
                            }
                        }
                        foreach (EntityID strawberry in oldStrawberries)
                        {
                            SaveData.Instance.Areas_Safe[i].Modes[0].Strawberries.Remove(strawberry);
                            level.Session.DoNotLoad.Remove(strawberry);
                        }
                        foreach (EntityID strawberry in newStrawberries)
                        {
                            SaveData.Instance.Areas_Safe[i].Modes[0].Strawberries.Add(strawberry);
                            level.Session.DoNotLoad.Add(strawberry);
                        }

                        // Adjust in-game map explored and warps

                        int index = i - SaveData.Instance.GetLevelSetStats().AreaOffset;
                        HashSet<string> oldVisitedRooms = new();
                        HashSet<string> oldVisitedRoomsTiles = new();
                        HashSet<string> newVisitedRooms = new();
                        HashSet<string> newVisitedRoomsTiles = new();
                        HashSet<string> oldUnlockedWarps = new();
                        HashSet<string> newUnlockedWarps = new();
                        foreach (KeyValuePair<string, string> oldRoom in RoomsNamesConversion)
                        {
                            foreach (string visitedRoom in XaphanModule.ModSaveData.VisitedRooms)
                            {
                                if (visitedRoom == "Xaphan/0/Ch" + index + "/" + oldRoom.Key)
                                {
                                    oldVisitedRooms.Add(visitedRoom);
                                    newVisitedRooms.Add(visitedRoom.Replace(oldRoom.Key, oldRoom.Value));
                                }
                            }
                            foreach (string visitedRoomTile in XaphanModule.ModSaveData.VisitedRoomsTiles)
                            {
                                if (visitedRoomTile.Contains("Xaphan/0/Ch" + index + "/" + oldRoom.Key))
                                {
                                    oldVisitedRoomsTiles.Add(visitedRoomTile);
                                    newVisitedRoomsTiles.Add(visitedRoomTile.Replace(oldRoom.Key, oldRoom.Value));
                                }
                            }
                            foreach (string unlockedWarp in XaphanModule.ModSaveData.UnlockedWarps)
                            {
                                if (unlockedWarp == "Xaphan/0_Ch" + index + "_" + oldRoom.Key)
                                {
                                    oldUnlockedWarps.Add(unlockedWarp);
                                    newUnlockedWarps.Add(unlockedWarp.Replace(oldRoom.Key, oldRoom.Value));
                                }
                            }
                        }
                        foreach (string visitedRoom in oldVisitedRooms)
                        {
                            XaphanModule.ModSaveData.VisitedRooms.Remove(visitedRoom);
                        }
                        foreach (string visitedRoomTile in oldVisitedRoomsTiles)
                        {
                            XaphanModule.ModSaveData.VisitedRoomsTiles.Remove(visitedRoomTile);
                        }
                        foreach (string unlockedWarp in oldUnlockedWarps)
                        {
                            XaphanModule.ModSaveData.UnlockedWarps.Remove(unlockedWarp);
                        }
                        foreach (string visitedRoom in newVisitedRooms)
                        {
                            XaphanModule.ModSaveData.VisitedRooms.Add(visitedRoom);
                        }
                        foreach (string visitedRoomTile in newVisitedRoomsTiles)
                        {
                            XaphanModule.ModSaveData.VisitedRoomsTiles.Add(visitedRoomTile);
                        }
                        foreach (string unlockedWarp in newUnlockedWarps)
                        {
                            XaphanModule.ModSaveData.UnlockedWarps.Add(unlockedWarp);
                        }
                    }
                }
            }
            XaphanModule.ModSaveData.SoCMVer = XaphanModule.SoCMVersion.Major * 100 + XaphanModule.SoCMVersion.Minor * 10 + XaphanModule.SoCMVersion.Build;
        }

        public static void RemoveUpgrades()
        {
            foreach (string upgrade in upgradesToRemove)
            {
                Commands.Cmd_Remove_Upgrades(upgrade);
            }
            upgradesToRemove.Clear();
        }
    }
}
