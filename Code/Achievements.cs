using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.XaphanHelper.Data;

namespace Celeste.Mod.XaphanHelper
{
    static class Achievements
    {
        static Version SoCMVersion()
        {
            foreach (EverestModule module in Everest.Modules)
            {
                if (module.Metadata.Name == "TheSecretOfCelesteMountain")
                {
                    return module.Metadata.Version;
                }
            }
            return null;
        }

        public static List<AchievementData> GenerateAchievementsList(Session session)
        {
            StatsFlags.GetStats(session);
            List<AchievementData> list = new();

            // General
            list.Add(new AchievementData("upg1", 0, "321Dash", "achievements/Xaphan/321Dash", "Upgrade_DashBoots", session.GetFlag("Upgrade_DashBoots") ? 1 : 0, 1, 5));
            list.Add(new AchievementData("upg2", 0, "FirmlyGraspIt", "achievements/Xaphan/FirmlyGraspIt", "Upgrade_PowerGrip", session.GetFlag("Upgrade_PowerGrip") ? 1 : 0, 1, 5));
            list.Add(new AchievementData("upg3", 0, "TimeToAscend", "achievements/Xaphan/TimeToAscend", "Upgrade_ClimbingKit", session.GetFlag("Upgrade_ClimbingKit") ? 1 : 0, 1, 5));
            list.Add(new AchievementData("upg4", 0, "FeelingSpacey", "achievements/Xaphan/FeelingSpacey", "Upgrade_SpaceJump", session.GetFlag("Upgrade_SpaceJump") ? 1 : 0, 1, 5));
            list.Add(new AchievementData("upg5", 0, "WheresTheKaboom", "achievements/Xaphan/WheresTheKaboom", "Upgrade_Bombs", session.GetFlag("Upgrade_Bombs") ? 1 : 0, 1, 5));

            if (SoCMVersion() >= new Version(3, 0, 0))
            {
                list.Add(new AchievementData("upg6", 0, "ImALittleSpider", "achievements/Xaphan/ImALittleSpider", "Upgrade_SpiderMagnet", session.GetFlag("Upgrade_SpiderMagnet") ? 1 : 0, 1, 5));
                list.Add(new AchievementData("upg7", 0, "Robopede", "achievements/Xaphan/Robopede", "Upgrade_RemoteDrone", session.GetFlag("Upgrade_RemoteDrone") ? 1 : 0, 1, 5));
                list.Add(new AchievementData("upg8", 0, "PackingAPunch", "achievements/Xaphan/PackingAPunch", "Upgrade_MissilesModule", session.GetFlag("Upgrade_MissilesModule") ? 1 : 0, 1, 5));
                list.Add(new AchievementData("upgOpt1", 0, "PlanningAhead", "achievements/Xaphan/PlanningAhead", "Upgrade_Binoculars", session.GetFlag("Upgrade_Binoculars") ? 1 : 0, 1, 5));
                list.Add(new AchievementData("upgOpt2", 0, "UnnaturalObserver", "achievements/Xaphan/UnnaturalObserver", "Upgrade_PulseRadar", session.GetFlag("Upgrade_PulseRadar") ? 1 : 0, 1, 5));
            }

            list.Add(new AchievementData("map0", 0, "MysteriousExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh0", StatsFlags.CurrentTiles[0], StatsFlags.TotalTiles[0], 10));
            list.Add(new AchievementData("cass", 0, "ItIsTimeForARemix", "achievements/Xaphan/ItIsTimeForARemix", "XaphanHelper_StatFlag_Cassettes", StatsFlags.cassetteCount, SaveData.Instance.GetLevelSetStatsFor(SaveData.Instance.GetLevelSet()).MaxCassettes, 15));
            list.Add(new AchievementData("hearts", 0, "MasterOfSecrets", "achievements/Xaphan/MasterOfSecrets", "XaphanHelper_StatFlag_Hearts", StatsFlags.heartCount, StatsFlags.TotalASideHearts, 25));

            // Area 1
            list.Add(new AchievementData("gem1-1", 1, "Reunited", "achievements/Xaphan/Reunited", "XaphanHelper_StatFlag_GemCh1", session.GetFlag("XaphanHelper_StatFlag_GemCh1") ? 1 : 0, 1, 10));
            list.Add(new AchievementData("strwb1", 1, "StrawberriesInTheRuins", "achievements/Xaphan/Strawberries", "XaphanHelper_StatFlag_StrawberriesCh1", StatsFlags.CurrentStrawberries[1], StatsFlags.TotalStrawberries[1], 20));
            list.Add(new AchievementData("map1", 1, "RuinsExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh1", StatsFlags.CurrentTiles[1], StatsFlags.TotalTiles[1], 25));
            list.Add(new AchievementData("bside1", 1, "Inferno", "achievements/Xaphan/BSide", "XaphanHelper_StatFlag_BSideCh1", session.GetFlag("XaphanHelper_StatFlag_BSideCh1") ? 1 : 0, 1, 25));
            list.Add(new AchievementData("golden1", 1, "SurvivorOfTheRuins", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh1-0", session.GetFlag("XaphanHelper_StatFlag_GoldenCh1-0") ? 1 : 0, 1, 50));
            list.Add(new AchievementData("golden1-b", 1, "LegendOfTheRuins", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh1-1", session.GetFlag("XaphanHelper_StatFlag_GoldenCh1-1") ? 1 : 0, 1, 50));

            // Area 2
            list.Add(new AchievementData("gem2-1", 2, "FreeAtLeast", "achievements/Xaphan/FreeAtLeast", "XaphanHelper_StatFlag_GemCh2", session.GetFlag("XaphanHelper_StatFlag_GemCh2") ? 1 : 0, 1, 10));
            list.Add(new AchievementData("strwb2", 2, "StrawberriesInTheAbysses", "achievements/Xaphan/Strawberries", "XaphanHelper_StatFlag_StrawberriesCh2", StatsFlags.CurrentStrawberries[2], StatsFlags.TotalStrawberries[2], 20));
            list.Add(new AchievementData("map2", 2, "AbyssExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh2", StatsFlags.CurrentTiles[2], StatsFlags.TotalTiles[2], 25));
            list.Add(new AchievementData("boss2-1", 2, "NotAsLonelyAsIThought", "achievements/Xaphan/Boss", "XaphanHelper_StatFlag_BossCh2", session.GetFlag("XaphanHelper_StatFlag_BossCh2") ? 1 : 0, 1, 25));
            list.Add(new AchievementData("boss2-1-cm", 2, "BringBackTheLight", "achievements/Xaphan/BossCM", "XaphanHelper_StatFlag_BossCMCh2", session.GetFlag("XaphanHelper_StatFlag_BossCMCh2") ? 1 : 0, 1, 50));
            list.Add(new AchievementData("golden2", 2, "SurvivorOfTheAbysses", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh2-0", session.GetFlag("XaphanHelper_StatFlag_GoldenCh2-0") ? 1 : 0, 1, 50));

            if (SoCMVersion() >= new Version(3, 0, 0))
            {
                // Area 4
                list.Add(new AchievementData("map4", 4, "BasinExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh4", StatsFlags.CurrentTiles[4], StatsFlags.TotalTiles[4], 25));

                // Area 5
                list.Add(new AchievementData("strwb5", 5, "StrawberriesInTheTerminal", "achievements/Xaphan/Strawberries", "XaphanHelper_StatFlag_StrawberriesCh5", StatsFlags.CurrentStrawberries[5], StatsFlags.TotalStrawberries[5], 20));
                list.Add(new AchievementData("map5", 5, "TerminalExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh5", StatsFlags.CurrentTiles[5], StatsFlags.TotalTiles[5], 25));
            }

            return list;
        }
    }
}
