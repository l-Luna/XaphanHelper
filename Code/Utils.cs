using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper {
    public static class Utils {
        public static EntityData GetEntityData(this MapData mapData, string entityName) {
            foreach (LevelData levelData in mapData.Levels) {
                if (levelData.GetEntityData(entityName) is EntityData entityData) {
                    return entityData;
                }
            }

            return null;
        }

        public static List<EntityData> GetEntityDatas(this MapData mapData, string entityName) {
            List<EntityData> entityDatas = new();
            foreach (LevelData levelData in mapData.Levels) {
                if (levelData.GetEntityDatas(entityName) is List<EntityData> entityDataList) {
                    entityDatas.AddRange(entityDataList);
                }
            }

            return entityDatas;
        }

        public static bool HasEntity(this MapData mapData, string entityName) {
            return mapData.GetEntityData(entityName) != null;
        }

        public static EntityData GetEntityData(this LevelData levelData, string entityName) {
            foreach (EntityData entity in levelData.Entities) {
                if (entity.Name == entityName) {
                    return entity;
                }
            }

            return null;
        }

        public static List<EntityData> GetEntityDatas(this LevelData levelData, string entityName) {
            List<EntityData> entityDatas = new();
            foreach (EntityData entity in levelData.Entities) {
                if (entity.Name == entityName) {
                    entityDatas.Add(entity);
                }
            }

            return entityDatas;
        }

        public static bool HasEntity(this LevelData levelData, string entityName) {
            return levelData.GetEntityData(entityName) != null;
        }
    }
}
