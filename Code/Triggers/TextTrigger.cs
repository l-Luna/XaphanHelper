using System.Collections;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/TextTrigger", "XaphanHelper/SubAreaNameTrigger")]
    class TextTrigger : Trigger
    {
        private string dialogID;

        private string textPositionX;

        private int textPositionY;

        private CustomText message;

        private float timer;

        public TextTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tag = Tags.Global;
            textPositionX = data.Attr("textPositionX");
            textPositionY = data.Int("textPositionY");
            if (textPositionY < 80)
            {
                textPositionY = 80;
            }
            else if (textPositionY > 1080)
            {
                textPositionY = 1080;
            }
            dialogID = data.Attr("dialogID");
            timer = data.Float("timer");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (XaphanModule.ModSaveData.CurrentSubArea != dialogID)
            {
                XaphanModule.ModSaveData.CurrentSubArea = dialogID;
                Add(new Coroutine(Display()));
            }
        }

        public IEnumerator Display()
        {
            if (SceneAs<Level>().Entities.FindAll<CustomText>() != null)
            {
                foreach (CustomText customText in SceneAs<Level>().Entities.FindAll<CustomText>())
                {
                    if (customText.message != dialogID && customText.Show == true)
                    {
                        customText.Show = false;
                    }
                }
            }
            SceneAs<Level>().Add(message = new CustomText(dialogID, textPositionX, textPositionY));
            message.Show = true;
            while (timer > 0f)
            {
                yield return null;
                timer -= Engine.DeltaTime;
            }
            message.Show = false;
            RemoveSelf();
        }
    }
}
