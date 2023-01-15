using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyHeartsDisplay : Entity
    {
        public Sprite Heart;

        public string levelSet;

        public int TotalMaps;

        public LobbyHeartsDisplay(Vector2 position, string levelSet, int totalMaps, float lobbyIndex) : base(position)
        {
            Tag = Tags.HUD;
            this.levelSet = levelSet;
            TotalMaps = totalMaps;
            Add(Heart = new Sprite(GFX.Gui, (lobbyIndex <= 3) ? "collectables/heartgem/" + (lobbyIndex - 1) + "/" : "CollabUtils2/crystalHeart/" + (lobbyIndex == 4 ? "expert" : "grandmaster") + "/"));
            Heart.AddLoop("spin", "spin", 0.08f);
            Heart.Scale = Vector2.One / 2;
            Heart.Play("spin");
        }

        public override void Render()
        {
            base.Render();
            if (Heart != null)
            {
                Heart.Render();
            }
            string currentHeartAmount = "0";
            foreach (LevelSetStats levelSet in SaveData.Instance.GetLevelSets())
            {
                if (levelSet.Name == this.levelSet)
                {
                    currentHeartAmount = Math.Min(SaveData.Instance.GetLevelSetStatsFor(this.levelSet).TotalHeartGems, TotalMaps).ToString();
                    break;
                }
            }
            string totalHeartAmount = TotalMaps.ToString();
            ActiveFont.DrawOutline(currentHeartAmount + " / " + totalHeartAmount, Position + new Vector2(Heart.Width / 2 + 10f + (ActiveFont.Measure(currentHeartAmount + " / " + totalHeartAmount).X / 2), Heart.Height / 4), new Vector2(0.5f, 0.5f), Vector2.One, currentHeartAmount == totalHeartAmount ? Color.Gold : Color.White, 2f, Color.Black);
        }
    }
}