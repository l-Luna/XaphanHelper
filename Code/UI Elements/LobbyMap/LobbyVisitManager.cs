using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyVisitManager
    {
        /// <summary>
        /// Each of the points the player has visited and thus generated circular snapshots.
        /// These points are in 8x8 pixel tile offsets from the top left of the map.
        /// </summary>
        public List<VisitedPoint> VisitedPoints { get; private set; }

        public string Key { get; private set; }

        private VisitedPoint lastVisitedPoint = new VisitedPoint(Vector2.Zero);

        public static string KeyForLobby(AreaKey area, int lobbyIndex) =>
            $"{area.LevelSet}/Ch{lobbyIndex}";

        public static LobbyVisitManager ForLobby(AreaKey area, int lobbyIndex) =>
            ForKey(KeyForLobby(area, lobbyIndex));

        public static LobbyVisitManager ForKey(string key)
        {
            if (XaphanModule.ModSaveData.VisitedLobbyPositions.TryGetValue(key, out var value))
            {
                var manager = new LobbyVisitManager
                {
                    VisitedPoints = FromBase64(value),
                    Key = key,
                };
                return manager;
            }

            return new LobbyVisitManager
            {
                VisitedPoints = new(),
                Key = key,
            };
        }

        public void Save()
        {
            XaphanModule.ModSaveData.VisitedLobbyPositions[Key] = ToBase64(VisitedPoints);
        }

        private static List<VisitedPoint> FromBase64(string str)
        {
            const int size = sizeof(short);

            var bytes = Convert.FromBase64String(str);
            if (bytes.Length % size * 2 != 0) return new();

            var list = new List<VisitedPoint>();
            for (int offset = 0; offset < bytes.Length; offset += size * 2)
            {
                var x = BitConverter.ToInt16(bytes, offset);
                var y = BitConverter.ToInt16(bytes, offset + size);
                list.Add(new VisitedPoint(new Vector2(x, y)));
            }

            return list;
        }

        private static string ToBase64(List<VisitedPoint> list)
        {
            const int size = sizeof(short);

            var bytes = new byte[list.Count * size * 2];
            int offset = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i];
                var b = BitConverter.GetBytes((short) v.Point.X);
                bytes[offset++] = b[0];
                bytes[offset++] = b[1];
                b = BitConverter.GetBytes((short) v.Point.Y);
                bytes[offset++] = b[0];
                bytes[offset++] = b[1];
            }

            return Convert.ToBase64String(bytes);
        }

        public void VisitPoint(Vector2 point)
        {
            const float generate_distance = 10f;
            const float sort_threshold = 20f;
            const int nearby_point_count = 50;

            var lenSq = lastVisitedPoint == null ? float.MaxValue : (point - lastVisitedPoint.Point).LengthSquared();
            var shouldGenerate = !VisitedPoints.Any();
            if (!shouldGenerate && lenSq > generate_distance * generate_distance)
            {
                Logger.Log(LogLevel.Warn, nameof(XaphanModule), $"We may need to generate a new point at {point.X},{point.Y}");
                // if the distance has gone past the sort threshold, recalculate and sort the list
                if (lenSq > sort_threshold * sort_threshold)
                {
                    Logger.Log(LogLevel.Warn, nameof(XaphanModule), $"=== Sorting ===");
                    foreach (var vp in VisitedPoints)
                    {
                        vp.DistanceSquared = (vp.Point - point).LengthSquared();
                    }

                    VisitedPoints.Sort((a, b) => Math.Sign(b.DistanceSquared - a.DistanceSquared));
                }

                // update last visited to closest of the first 20
                lastVisitedPoint = VisitedPoints.Take(nearby_point_count).FirstOrDefault(v => (v.Point - point).LengthSquared() < generate_distance * generate_distance);
                // generate if it still passes the threshold
                shouldGenerate = lastVisitedPoint == null;
            }

            if (shouldGenerate)
            {
                Logger.Log(LogLevel.Warn, nameof(XaphanModule), $"*** Generating point at {point.X},{point.Y} ***");
                VisitedPoints.Add(lastVisitedPoint = new VisitedPoint(point, 0f));
            }
        }

        public class VisitedPoint
        {
            public Vector2 Point;
            public float DistanceSquared;

            public VisitedPoint(Vector2 point, float distanceSquared = float.MaxValue)
            {
                Point = point;
                DistanceSquared = distanceSquared;
            }
        }
    }
}
