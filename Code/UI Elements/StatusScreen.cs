using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    class StatusScreen : Entity
    {
        protected static XaphanModuleSettings XaphanSettings => XaphanModule.Settings;

        private Level level;

        private StatusDisplay statusDisplay;

        public bool ShowUI;

        private bool NoInput;

        public string Title;

        public BigTitle BigTitle;

        private Wiggler mapWiggle;

        private Wiggler closeWiggle;

        private Wiggler actionWiggle;

        private float mapWiggleDelay;

        private float closeWiggleDelay;

        private float actionWiggleDelay;

        private bool fromMap;

        private float switchTimer;

        public int Selection;

        public StatusScreen(Level level, bool fromMap)
        {
            this.level = level;
            this.fromMap = fromMap;
            Tag = Tags.HUD;
            Title = (SaveData.Instance == null || !Dialog.Language.CanDisplay(SaveData.Instance.Name)) ? "FILE_DEFAULT" : SaveData.Instance.Name;
            Add(mapWiggle = Wiggler.Create(0.4f, 4f));
            Add(closeWiggle = Wiggler.Create(0.4f, 4f));
            Add(actionWiggle = Wiggler.Create(0.4f, 4f));
            Selection = 0;
            Depth = -10001;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = Scene as Level;
            level.PauseLock = true;
            level.Session.SetFlag("Map_Opened", true);
            if (!fromMap)
            {
                Audio.Play("event:/ui/game/pause");
            }
            Add(new Coroutine(TransitionToStatus(level)));
        }

        public override void Update()
        {
            if (ShowUI)
            {
                foreach (Player player in SceneAs<Level>().Tracker.GetEntities<Player>())
                {
                    player.StateMachine.State = Player.StDummy;
                    player.DummyAutoAnimate = false;
                }
            }
            if (XaphanModule.useIngameMap)
            {
                if (Input.Pause.Check && mapWiggleDelay <= 0f && switchTimer <= 0)
                {
                    mapWiggle.Start();
                    mapWiggleDelay = 0.5f;
                }
            }
            if (Input.MenuCancel.Check && closeWiggleDelay <= 0f)
            {
                closeWiggle.Start();
                closeWiggleDelay = 0.5f;
            }
            if (Input.MenuConfirm.Check && actionWiggleDelay <= 0f)
            {
                actionWiggle.Start();
                actionWiggleDelay = 0.5f;
            }
            mapWiggleDelay -= Engine.DeltaTime;
            closeWiggleDelay -= Engine.DeltaTime;
            actionWiggleDelay -= Engine.DeltaTime;
            base.Update();
        }

        private IEnumerator TransitionToStatus(Level level)
        {
            float duration = 0.5f;
            if (!fromMap)
            {
                FadeWipe Wipe = new(SceneAs<Level>(), false)
                {
                    Duration = duration
                };
                SceneAs<Level>().Add(Wipe);
                duration = duration - 0.25f;
                while (duration > 0f)
                {
                    yield return null;
                    duration -= Engine.DeltaTime;
                }
            }
            CountdownDisplay timerDisplay = SceneAs<Level>().Tracker.GetEntity<CountdownDisplay>();
            if (timerDisplay != null)
            {
                timerDisplay.StopTimer(true, false);
            }
            ShowUI = true;
            duration = 0.25f;
            FadeWipe Wipe2 = new(SceneAs<Level>(), true)
            {
                Duration = duration
            };
            Add(new Coroutine(StatusRoutine(level)));
            switchTimer = 0.35f;
            while (switchTimer > 0f)
            {
                yield return null;
                switchTimer -= Engine.DeltaTime;
            }
        }

        private IEnumerator TransitionToGame()
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            float duration = 0.5f;
            FadeWipe Wipe = new(SceneAs<Level>(), false)
            {
                Duration = duration
            };
            SceneAs<Level>().Add(Wipe);
            duration = duration - 0.25f;
            while (duration > 0f)
            {
                yield return null;
                duration -= Engine.DeltaTime;
            }
            CountdownDisplay timerDisplay = SceneAs<Level>().Tracker.GetEntity<CountdownDisplay>();
            if (timerDisplay != null)
            {
                timerDisplay.StopTimer(false, true);
            }
            BagDisplay bagDisplay = SceneAs<Level>().Tracker.GetEntity<BagDisplay>();
            if (bagDisplay != null)
            {
                if (!bagDisplay.CheckIfUpgradeIsActive(bagDisplay.currentSelection))
                {
                    bagDisplay.SetToFirstActiveUpgrade();
                }
            }
            ShowUI = false;
            duration = 0.25f;
            Wipe = new FadeWipe(SceneAs<Level>(), true)
            {
                Duration = duration
            };
            Add(new Coroutine(CloseStatus(false)));
        }

        private IEnumerator TransitionToMapScreen()
        {
            if (!NoInput)
            {
                NoInput = true;
                Player player = Scene.Tracker.GetEntity<Player>();
                float duration = 0.5f;
                FadeWipe Wipe = new(SceneAs<Level>(), false)
                {
                    Duration = duration
                };
                SceneAs<Level>().Add(Wipe);
                duration = duration - 0.25f;
                while (duration > 0f)
                {
                    yield return null;
                    duration -= Engine.DeltaTime;
                }
                Add(new Coroutine(CloseStatus(true)));
                level.Add(new MapScreen(level, true));
            }
        }

        private IEnumerator StatusRoutine(Level level)
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            Scene.Add(BigTitle = new BigTitle(Title, new Vector2(960, 80), true));
            Scene.Add(statusDisplay = new StatusDisplay(level, XaphanModule.useIngameMap));
            yield return statusDisplay.GennerateUpgradesDisplay();
            int firstLeftUpgradeIndex = 11;
            int firstRightUpgradeIndex = 22;
            int lastLeftUpgradeIndex = 1;
            int lastRightUpgradeIndex = 12;
            foreach (int upgradeIndex in statusDisplay.LeftDisplays)
            {
                if (upgradeIndex < firstLeftUpgradeIndex)
                {
                    firstLeftUpgradeIndex = upgradeIndex;
                }
                if (upgradeIndex > lastLeftUpgradeIndex)
                {
                    lastLeftUpgradeIndex = upgradeIndex;
                }
            }
            foreach (int upgradeIndex in statusDisplay.RightDisplays)
            {
                if (upgradeIndex < firstRightUpgradeIndex)
                {
                    firstRightUpgradeIndex = upgradeIndex;
                }
                if (upgradeIndex > lastRightUpgradeIndex)
                {
                    lastRightUpgradeIndex = upgradeIndex;
                }
            }
            if (statusDisplay.LeftDisplays.Count > 0)
            {
                Selection = firstLeftUpgradeIndex;
            }
            else if (statusDisplay.RightDisplays.Count > 0)
            {
                Selection = firstRightUpgradeIndex;
            }
            while (switchTimer > 0)
            {
                yield return null;
            }
            while (!Input.ESC.Pressed && !Input.MenuCancel.Pressed && !XaphanSettings.OpenMap.Pressed && player != null)
            {
                if (Selection > 0)
                {
                    if (Selection <= 11)
                    {
                        if (Input.MenuUp.Pressed && Selection > firstLeftUpgradeIndex)
                        {
                            while (!statusDisplay.LeftDisplays.Contains(Selection - 1))
                            {
                                Selection--;
                            }
                            Selection--;
                            Audio.Play("event:/ui/main/rollover_up");
                        }
                        else if (Input.MenuDown.Pressed && Selection < lastLeftUpgradeIndex)
                        {
                            while (!statusDisplay.LeftDisplays.Contains(Selection + 1))
                            {
                                Selection++;
                            }
                            Selection++;
                            Audio.Play("event:/ui/main/rollover_down");
                        }
                        else
                        {
                            if (Input.MenuRight.Pressed && statusDisplay.RightDisplays.Count > 0)
                            {
                                Selection += 11;
                                if (Selection == 12)
                                {
                                    while (!statusDisplay.RightDisplays.Contains(Selection))
                                    {
                                        Selection++;
                                    }
                                }
                                else
                                {
                                    while (!statusDisplay.RightDisplays.Contains(Selection))
                                    {
                                        if (Selection == 22)
                                        {
                                            Selection = statusDisplay.RightDisplays.Count;
                                        }
                                        else
                                        {
                                            Selection++;
                                        }
                                    }
                                }
                                Audio.Play("event:/ui/main/rollover_up");
                            }
                        }
                    }
                    else
                    {
                        if (Input.MenuUp.Pressed && Selection > firstRightUpgradeIndex)
                        {
                            while (!statusDisplay.RightDisplays.Contains(Selection - 1))
                            {
                                Selection--;
                            }
                            Selection--;
                            Audio.Play("event:/ui/main/rollover_up");
                        }
                        else if (Input.MenuDown.Pressed && Selection < lastRightUpgradeIndex)
                        {
                            while (!statusDisplay.RightDisplays.Contains(Selection + 1))
                            {
                                Selection++;
                            }
                            Selection++;
                            Audio.Play("event:/ui/main/rollover_down");
                        }
                        else
                        {
                            if (Input.MenuLeft.Pressed && statusDisplay.LeftDisplays.Count > 0)
                            {
                                Selection -= 11;
                                if (Selection == 1)
                                {
                                    while (!statusDisplay.LeftDisplays.Contains(Selection))
                                    {
                                        Selection--;
                                    }
                                }
                                else
                                {
                                    while (!statusDisplay.LeftDisplays.Contains(Selection))
                                    {
                                        if (Selection == 11)
                                        {
                                            Selection = statusDisplay.LeftDisplays.Count;
                                        }
                                        else
                                        {
                                            Selection++;
                                        }
                                    }
                                }
                                Audio.Play("event:/ui/main/rollover_up");
                            }
                        }
                    }
                }
                if (Input.Pause.Check && XaphanModule.useIngameMap && XaphanModule.CanOpenMap(level) && switchTimer <= 0)
                {
                    Add(new Coroutine(TransitionToMapScreen()));
                }
                yield return null;
            }
            Audio.Play("event:/ui/game/unpause");
            Add(new Coroutine(TransitionToGame()));
        }

        private IEnumerator CloseStatus(bool switchtoMap)
        {
            Level level = Scene as Level;
            Player player = Scene.Tracker.GetEntity<Player>();
            level.Remove(BigTitle);
            level.Remove(statusDisplay);
            if (!switchtoMap)
            {
                if (player != null)
                {
                    player.StateMachine.State = Player.StNormal;
                    player.DummyAutoAnimate = true;
                }
                level.PauseLock = false;
                yield return 0.1f;
                level.Session.SetFlag("Map_Opened", false);
            }
            RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            if (ShowUI)
            {
                Draw.Rect(new Vector2(-10, -10), 1940, 182, Color.Black);
                Draw.Rect(new Vector2(-10, 172), 100, 856, Color.Black);
                Draw.Rect(new Vector2(1830, 172), 100, 856, Color.Black);
                Draw.Rect(new Vector2(-10, 1028), 1940, 62, Color.Black);
                Draw.Rect(new Vector2(90, 172), 1740, 8, Color.White);
                Draw.Rect(new Vector2(90, 180), 10, 840, Color.White);
                Draw.Rect(new Vector2(1820, 180), 10, 840, Color.White);
                Draw.Rect(new Vector2(90, 1020), 1740, 8, Color.White);
                float inputEase = 0f;
                inputEase = Calc.Approach(inputEase, 1, Engine.DeltaTime * 4f);
                if (inputEase > 0f)
                {
                    float scale = 0.5f;
                    string label = Dialog.Clean("XaphanHelper_UI_close");
                    string label2 = Dialog.Clean("XaphanHelper_UI_map");
                    string label3 = Dialog.Clean("XaphanHelper_UI_activate");
                    string label4 = Dialog.Clean("XaphanHelper_UI_deactivate");
                    float num = ButtonUI.Width(label, Input.MenuCancel);
                    float num2 = ButtonUI.Width(label2, Input.Pause);
                    Vector2 position = new(1830f, 1055f);
                    ButtonUI.Render(position, label, Input.MenuCancel, scale, 1f, closeWiggle.Value * 0.05f);
                    if (XaphanModule.useIngameMap && XaphanModule.CanOpenMap(level))
                    {
                        position.X -= num / 2 + 32;
                        ButtonUI.Render(position, label2, Input.Pause, scale, 1f, mapWiggle.Value * 0.05f);
                    }
                    if (Selection != 0 && statusDisplay.SelectedDisplay != null)
                    {
                        position.X -= num2 / 2 + 32;
                        ButtonUI.Render(position, statusDisplay.SelectedDisplay.InactiveList.Contains(level.Session.Area.GetLevelSet()) ? label3 : label4, Input.MenuConfirm, scale, 1f, actionWiggle.Value * 0.05f);
                    }
                }
            }
        }
    }
}
