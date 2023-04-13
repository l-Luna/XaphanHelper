using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.Data;

namespace Celeste.Mod.XaphanHelper
{
    class Achievements
    {
        public static List<AchievementData> GenerateAchievementsList(Session session)
        {
            StatsFlags.GetStats(session);
            List<AchievementData> list = new();

            // General
            list.Add(new AchievementData(0, "321Dash", "achievements/Xaphan/321Dash", "Upgrade_DashBoots", session.GetFlag("Upgrade_DashBoots") ? 1 : 0, 1, 5));
            list.Add(new AchievementData(0, "FirmlyGraspIt", "achievements/Xaphan/FirmlyGraspIt", "Upgrade_PowerGrip", session.GetFlag("Upgrade_PowerGrip") ? 1 : 0, 1, 5));
            list.Add(new AchievementData(0, "TimeToAscend", "achievements/Xaphan/TimeToAscend", "Upgrade_ClimbingKit", session.GetFlag("Upgrade_ClimbingKit") ? 1 : 0, 1, 5));
            list.Add(new AchievementData(0, "WheresTheKaboom", "achievements/Xaphan/WheresTheKaboom", "Upgrade_Bombs", session.GetFlag("Upgrade_Bombs") ? 1 : 0, 1, 5));
            list.Add(new AchievementData(0, "FeelingSpacey", "achievements/Xaphan/FeelingSpacey", "Upgrade_SpaceJump", session.GetFlag("Upgrade_SpaceJump") ? 1 : 0, 1, 5));
            list.Add(new AchievementData(0, "MysteriousExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh0", StatsFlags.CurrentTiles[0], StatsFlags.TotalTiles[0], 10));
            list.Add(new AchievementData(0, "ItIsTimeForARemix", "achievements/Xaphan/ItIsTimeForARemix", "XaphanHelper_StatFlag_Cassettes", StatsFlags.cassetteCount, SaveData.Instance.GetLevelSetStatsFor(SaveData.Instance.GetLevelSet()).MaxCassettes, 15));
            list.Add(new AchievementData(0, "MasterOfSecrets", "achievements/Xaphan/MasterOfSecrets", "XaphanHelper_StatFlag_Hearts", StatsFlags.heartCount, StatsFlags.TotalASideHearts, 25));

            // Area 1
            list.Add(new AchievementData(1, "Reunited", "achievements/Xaphan/Reunited", "XaphanHelper_StatFlag_GemCh1", session.GetFlag("XaphanHelper_StatFlag_GemCh1") ? 1 : 0, 1, 10));
            list.Add(new AchievementData(1, "StrawberriesInTheRuins", "achievements/Xaphan/TheStrawberryHunt", "XaphanHelper_StatFlag_StrawberriesCh1", StatsFlags.CurrentStrawberries[1], StatsFlags.TotalStrawberries[1], 20));
            list.Add(new AchievementData(1, "RuinsExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh1", StatsFlags.CurrentTiles[1], StatsFlags.TotalTiles[1], 25));
            list.Add(new AchievementData(1, "Inferno", "achievements/Xaphan/BSide", "XaphanHelper_StatFlag_BSideCh1", session.GetFlag("XaphanHelper_StatFlag_BSideCh1") ? 1 : 0, 1, 25));
            list.Add(new AchievementData(1, "SurvivorOfTheRuins", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh1-0", session.GetFlag("XaphanHelper_StatFlag_GoldenCh1-0") ? 1 : 0, 1, 50));
            list.Add(new AchievementData(1, "LegendOfTheRuins", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh1-1", session.GetFlag("XaphanHelper_StatFlag_GoldenCh1-1") ? 1 : 0, 1, 50));

            // Area 2
            list.Add(new AchievementData(2, "FreeAtLast", "achievements/Xaphan/FreeAtLast", "XaphanHelper_StatFlag_GemCh2", session.GetFlag("XaphanHelper_StatFlag_GemCh2") ? 1 : 0, 1, 10));
            list.Add(new AchievementData(2, "StrawberriesInTheAbysses", "achievements/Xaphan/TheStrawberryHunt", "XaphanHelper_StatFlag_StrawberriesCh2", StatsFlags.CurrentStrawberries[2], StatsFlags.TotalStrawberries[2], 20));
            list.Add(new AchievementData(2, "AbyssExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh2", StatsFlags.CurrentTiles[2], StatsFlags.TotalTiles[2], 25));
            list.Add(new AchievementData(2, "NotAsLonelyAsIThought", "achievements/Xaphan/Boss", "XaphanHelper_StatFlag_BossCh2", session.GetFlag("XaphanHelper_StatFlag_BossCh2") ? 1 : 0, 1, 25));
            list.Add(new AchievementData(2, "BringBackTheLight", "achievements/Xaphan/BossCM", "XaphanHelper_StatFlag_BossCMCh2", session.GetFlag("XaphanHelper_StatFlag_BossCMCh2") ? 1 : 0, 1, 50));
            list.Add(new AchievementData(2, "SurvivorOfTheAbysses", "achievements/Xaphan/Survivor", "XaphanHelper_StatFlag_GoldenCh2-0", session.GetFlag("XaphanHelper_StatFlag_GoldenCh2-0") ? 1 : 0, 1, 50));

            // Area 5
            list.Add(new AchievementData(5, "StrawberriesInTheTerminal", "achievements/Xaphan/TheStrawberryHunt", "XaphanHelper_StatFlag_StrawberriesCh5", StatsFlags.CurrentStrawberries[5], StatsFlags.TotalStrawberries[5], 20));
            list.Add(new AchievementData(5, "TerminalExplorer", "achievements/Xaphan/Explorer", "XaphanHelper_StatFlag_MapCh5", StatsFlags.CurrentTiles[5], StatsFlags.TotalTiles[5], 25));
            return list;
        }
    }
}
