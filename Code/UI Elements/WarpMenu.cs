using Celeste;
using Celeste.Mod.XaphanHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class WarpMenu : Entity
    {
        public static string ConfirmSfx;

        public enum InnerContentMode
        {
            OneColumn,
            TwoColumn
        }

        public abstract class Item
        {
            public bool Selectable = false;

            public bool Visible = true;

            public bool Disabled = false;

            public bool IncludeWidthInMeasurement = true;

            public string Label;

            public string Room;

            public int WarpIndex;

            public int ID;

            public bool Hide;

            public WarpMenu Container;

            public Wiggler SelectWiggler;

            public Wiggler ValueWiggler;

            public Action OnEnter;

            public Action OnLeave;

            public Action OnPressed;

            public Action OnAltPressed;

            public Action OnUpdate;

            public bool Hoverable => Selectable && Visible && !Disabled;

            public float Width => LeftWidth() + RightWidth();

            public Item Enter(Action onEnter)
            {
                OnEnter = onEnter;
                return this;
            }

            public Item Leave(Action onLeave)
            {
                OnLeave = onLeave;
                return this;
            }

            public Item Pressed(Action onPressed)
            {
                OnPressed = onPressed;
                return this;
            }

            public Item AltPressed(Action onPressed)
            {
                OnAltPressed = onPressed;
                return this;
            }

            public virtual void ConfirmPressed()
            {
            }

            public virtual void LeftPressed()
            {
            }

            public virtual void RightPressed()
            {
            }

            public virtual void Added()
            {
            }

            public virtual void Update()
            {
            }

            public virtual float LeftWidth()
            {
                return 0f;
            }

            public virtual float RightWidth()
            {
                return 0f;
            }

            public virtual float Height()
            {
                return 0f;
            }

            public virtual void Render(Vector2 position, bool highlighted)
            {
            }
        }

        public class Header : Item
        {
            public const float Scale = 2f;

            public string Title;

            public Header(string title)
            {
                Title = title;
                Selectable = false;
                IncludeWidthInMeasurement = false;
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Title).X * 2f;
            }

            public override float Height()
            {
                return ActiveFont.LineHeight * 2f;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                float alpha = Container.Alpha;
                Color strokeColor = Color.Black * (alpha * alpha * alpha);
                ActiveFont.DrawEdgeOutline(Title, position + new Vector2(Container.Width * 0.5f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.Gray * alpha, 4f, Color.DarkSlateBlue * alpha, 2f, strokeColor);
            }
        }

        public class SubHeader : Item
        {
            public const float Scale = 0.6f;

            public string Title;

            public SubHeader(string title)
            {
                Title = title;
                Selectable = false;
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Title).X * 0.6f;
            }

            public override float Height()
            {
                return ((Title.Length > 0) ? (ActiveFont.LineHeight * 0.6f) : 0f) + 28f;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                if (Title.Length > 0)
                {
                    float alpha = Container.Alpha;
                    Color strokeColor = Color.Black * (alpha * alpha * alpha);
                    Vector2 position2 = position + ((Container.InnerContent == InnerContentMode.TwoColumn) ? new Vector2(0f, 32f) : new Vector2(Container.Width * 0.5f, 32f));
                    Vector2 justify = new Vector2((Container.InnerContent == InnerContentMode.TwoColumn) ? 0f : 0.5f, 0.5f);
                    ActiveFont.DrawOutline(Title, position2, justify, Vector2.One * 0.6f, Color.Gray * alpha, 2f, strokeColor);
                }
            }
        }

        public class Option<T> : Item
        {
            public int Index;

            public Action<T> OnValueChange;

            public int PreviousIndex;

            public List<Tuple<string, T>> Values = new List<Tuple<string, T>>();

            private float sine = 0f;

            private int lastDir = 0;

            public Option(string label)
            {
                Label = label;
                Selectable = true;
            }

            public Option<T> Add(string label, T value, bool selected = false)
            {
                Values.Add(new Tuple<string, T>(label, value));
                if (selected)
                {
                    PreviousIndex = (Index = Values.Count - 1);
                }
                return this;
            }

            public Option<T> Change(Action<T> action)
            {
                OnValueChange = action;
                return this;
            }

            public override void Added()
            {
                Container.InnerContent = InnerContentMode.TwoColumn;
            }

            public override void LeftPressed()
            {
                if (Index > 0)
                {
                    Audio.Play("event:/ui/main/button_toggle_off");
                    PreviousIndex = Index;
                    Index--;
                    lastDir = -1;
                    ValueWiggler.Start();
                    OnValueChange?.Invoke(Values[Index].Item2);
                }
            }

            public override void RightPressed()
            {
                if (Index < Values.Count - 1)
                {
                    Audio.Play("event:/ui/main/button_toggle_on");
                    PreviousIndex = Index;
                    Index++;
                    lastDir = 1;
                    ValueWiggler.Start();
                    OnValueChange?.Invoke(Values[Index].Item2);
                }
            }

            public override void ConfirmPressed()
            {
                if (Values.Count == 2)
                {
                    if (Index == 0)
                    {
                        Audio.Play("event:/ui/main/button_toggle_on");
                    }
                    else
                    {
                        Audio.Play("event:/ui/main/button_toggle_off");
                    }
                    PreviousIndex = Index;
                    Index = 1 - Index;
                    lastDir = ((Index == 1) ? 1 : (-1));
                    ValueWiggler.Start();
                    OnValueChange?.Invoke(Values[Index].Item2);
                }
            }

            public override void Update()
            {
                sine += Engine.RawDeltaTime;
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Label).X + 32f;
            }

            public override float RightWidth()
            {
                float num = 0f;
                foreach (Tuple<string, T> value in Values)
                {
                    num = Math.Max(num, ActiveFont.Measure(value.Item1).X);
                }
                return num + 120f;
            }

            public override float Height()
            {
                return ActiveFont.LineHeight;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                float alpha = Container.Alpha;
                Color strokeColor = Color.Black * (alpha * alpha * alpha);
                Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
                if (Values.Count > 0)
                {
                    float num = RightWidth();
                    ActiveFont.DrawOutline(Values[Index].Item1, position + new Vector2(Container.Width - num * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);
                    Vector2 vector = Vector2.UnitX * (highlighted ? ((float)Math.Sin(sine * 4f) * 4f) : 0f);
                    bool flag = Index > 0;
                    Color color2 = flag ? color : (Color.DarkSlateGray * alpha);
                    Vector2 position2 = position + new Vector2(Container.Width - num + 40f + ((lastDir < 0) ? ((0f - ValueWiggler.Value) * 8f) : 0f), 0f) - (flag ? vector : Vector2.Zero);
                    ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);
                    bool flag2 = Index < Values.Count - 1;
                    Color color3 = flag2 ? color : (Color.DarkSlateGray * alpha);
                    Vector2 position3 = position + new Vector2(Container.Width - 40f + ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
                    ActiveFont.DrawOutline(">", position3, new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
                }
            }
        }

        public class Slider : Option<int>
        {
            public Slider(string label, Func<int, string> values, int min, int max, int value = -1)
                : base(label)
            {
                for (int i = min; i <= max; i++)
                {
                    Add(values(i), i, value == i);
                }
            }
        }

        public class OnOff : Option<bool>
        {
            public OnOff(string label, bool on)
                : base(label)
            {
                Add(Dialog.Clean("options_off"), false);
                Add(Dialog.Clean("options_on"), true);
            }
        }

        public class Setting : Item
        {
            public string ConfirmSfx = "event:/ui/main/button_select";

            //public string Label;

            public List<object> Values = new List<object>();

            public Setting(string label, string value = "")
            {
                Label = label;
                Values.Add(value);
                Selectable = true;
            }

            public Setting(string label, Keys key)
                : this(label)
            {
                Set(new List<Keys>
            {
                key
            });
            }

            public Setting(string label, List<Keys> keys)
                : this(label)
            {
                Set(keys);
            }

            public Setting(string label, Buttons btn)
                : this(label)
            {
                Set(new List<Buttons>
            {
                btn
            });
            }

            public Setting(string label, List<Buttons> buttons)
                : this(label)
            {
                Set(buttons);
            }

            public void Set(List<Keys> keys)
            {
                Values.Clear();
                int i = 0;
                for (int num = Math.Min(4, keys.Count); i < num; i++)
                {
                    if (keys[i] == Keys.None)
                    {
                        continue;
                    }
                    MTexture mTexture = Input.GuiKey(keys[i], null);
                    if (mTexture != null)
                    {
                        Values.Add(mTexture);
                        continue;
                    }
                    string text = keys[i].ToString();
                    string text2 = "";
                    for (int j = 0; j < text.Length; j++)
                    {
                        if (j > 0 && char.IsUpper(text[j]))
                        {
                            text2 += " ";
                        }
                        text2 += text[j].ToString();
                    }
                    Values.Add(text2);
                }
            }

            public void Set(List<Buttons> buttons)
            {
                Values.Clear();
                int i = 0;
                for (int num = Math.Min(4, buttons.Count); i < num; i++)
                {
                    MTexture mTexture = Input.GuiSingleButton(buttons[i], null);
                    if (mTexture != null)
                    {
                        Values.Add(mTexture);
                        continue;
                    }
                    string text = buttons[i].ToString();
                    string text2 = "";
                    for (int j = 0; j < text.Length; j++)
                    {
                        if (j > 0 && char.IsUpper(text[j]))
                        {
                            text2 += " ";
                        }
                        text2 += text[j].ToString();
                    }
                    Values.Add(text2);
                }
            }

            public override void Added()
            {
                Container.InnerContent = InnerContentMode.TwoColumn;
            }

            public override void ConfirmPressed()
            {
                Audio.Play(ConfirmSfx);
                base.ConfirmPressed();
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Label).X;
            }

            public override float RightWidth()
            {
                float num = 0f;
                foreach (object value in Values)
                {
                    if (value is MTexture)
                    {
                        num += (value as MTexture).Width;
                    }
                    else if (value is string)
                    {
                        num += ActiveFont.Measure(value as string).X * 0.7f + 16f;
                    }
                }
                return num;
            }

            public override float Height()
            {
                return ActiveFont.LineHeight * 1.2f;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                float alpha = Container.Alpha;
                Color strokeColor = Color.Black * (alpha * alpha * alpha);
                Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);
                float num = RightWidth();
                foreach (object value in Values)
                {
                    if (value is MTexture)
                    {
                        MTexture mTexture = value as MTexture;
                        mTexture.DrawJustified(position + new Vector2(Container.Width - num, 0f), new Vector2(0f, 0.5f), Color.White * alpha);
                        num -= mTexture.Width;
                    }
                    else if (value is string)
                    {
                        string text = value as string;
                        float num2 = ActiveFont.Measure(value as string).X * 0.7f + 16f;
                        ActiveFont.DrawOutline(text, position + new Vector2(Container.Width - num + num2 * 0.5f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, Color.LightGray * alpha, 2f, strokeColor);
                        num -= num2;
                    }
                }
            }
        }

        public class Button : Item
        {
            public bool AlwaysCenter;

            public Button(string room, string label, int index)
            {
                Room = room;
                WarpIndex = index;
                Label = label;
                Selectable = true;
            }

            public override void ConfirmPressed()
            {
                if (Label != Dialog.Clean("XaphanHelper_Warp_Stay"))
                {
                    Audio.Play(ConfirmSfx);
                }
                base.ConfirmPressed();
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Label).X;
            }

            public override float Height()
            {
                return ActiveFont.LineHeight;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                float alpha = Container.Alpha;
                Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
                Color strokeColor = Color.Black * (alpha * alpha * alpha);
                bool flag = Container.InnerContent == InnerContentMode.TwoColumn && !AlwaysCenter;
                Vector2 position2 = position + (flag ? Vector2.Zero : new Vector2(Container.Width * 0.5f, 0f));
                Vector2 justify = (flag && !AlwaysCenter) ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
                ActiveFont.DrawOutline(Label, position2, justify, Vector2.One, color, 2f, strokeColor);
            }
        }

        public class LanguageButton : Item
        {
            public string ConfirmSfx = "event:/ui/main/button_select";

            //public string Label;

            public Language Language;

            public bool AlwaysCenter;

            public LanguageButton(string label, Language language)
            {
                Label = label;
                Language = language;
                Selectable = true;
            }

            public override void ConfirmPressed()
            {
                Audio.Play(ConfirmSfx);
                base.ConfirmPressed();
            }

            public override float LeftWidth()
            {
                return ActiveFont.Measure(Label).X;
            }

            public override float RightWidth()
            {
                return Language.Icon.Width;
            }

            public override float Height()
            {
                return ActiveFont.LineHeight;
            }

            public override void Render(Vector2 position, bool highlighted)
            {
                float alpha = Container.Alpha;
                Color color = Color.Black * (alpha * alpha * alpha);
                ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha), 2f, color);
                position += new Vector2(Container.Width - RightWidth(), 0f);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            Language.Icon.DrawJustified(position + new Vector2(i * 2f, j * 2f), new Vector2(0f, 0.5f), color, 1f);
                        }
                    }
                }
                Language.Icon.DrawJustified(position, new Vector2(0f, 0.5f), Color.White * alpha, 1f);
            }
        }

        public bool Focused = true;

        public InnerContentMode InnerContent = InnerContentMode.OneColumn;

        private List<Item> items = new List<Item>();

        public int Selection = -1;

        public Vector2 Justify;

        public float ItemSpacing = 4f;

        public float MinWidth = 0f;

        public float Alpha = 1f;

        public Color HighlightColor = Color.White;

        public static readonly Color HighlightColorA = Calc.HexToColor("84FF54");

        public static readonly Color HighlightColorB = Calc.HexToColor("FCFF59");

        public Action OnESC;

        public Action OnCancel;

        public Action OnUpdate;

        public Action OnPause;

        public Action OnClose;

        public bool AutoScroll = false;

        public Item Current
        {
            get
            {
                return (items.Count > 0 && Selection >= 0) ? items[Selection] : null;
            }
            set
            {
                Selection = items.IndexOf(value);
            }
        }

        public new float Width
        {
            get;
            private set;
        }

        public new float Height
        {
            get;
            private set;
        }

        public float LeftColumnWidth
        {
            get;
            private set;
        }

        public float RightColumnWidth
        {
            get;
            private set;
        }

        public float ScrollableMinSize => Engine.Height - 300;

        public int FirstPossibleSelection
        {
            get
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null && items[i].Hoverable)
                    {
                        return i;
                    }
                }
                return 0;
            }
        }

        public int LastPossibleSelection
        {
            get
            {
                for (int num = items.Count - 1; num >= 0; num--)
                {
                    if (items[num] != null && items[num].Hoverable)
                    {
                        return num;
                    }
                }
                return 0;
            }
        }

        public float ScrollTargetY
        {
            get
            {
                float min = Engine.Height - 150 - Height * Justify.Y;
                float max = 150f + Height * Justify.Y;
                return Calc.Clamp(Engine.Height / 2 + Height * Justify.Y - GetYOffsetOf(Current), min, max);
            }
        }

        public List<Item> Items => items;

        public NormalText header;

        public int itemStartID = 0;

        public bool NoHeader;

        public WarpMenu(string confirmSfx, bool noHeader = false)
        {
            Tag = Tags.PauseUpdate | Tags.HUD;
            Justify = new Vector2(0.5f, 0.5f);
            ConfirmSfx = confirmSfx;
            NoHeader = noHeader;
            Depth = -20000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!NoHeader)
            {
                Scene.Add(header = new NormalText("XaphanHelper_Warp_Select_Destination", new Vector2(Position.X, Position.Y - Height / 2 - 50), Color.Gray, 0f, 0.7f));
            }
            if (AutoScroll)
            {
                if (Height > ScrollableMinSize)
                {
                    Position.Y = ScrollTargetY;
                }
                else
                {
                    Position.Y = 540f;
                }
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (header != null)
            {
                header.RemoveSelf();
            }
        }

        public WarpMenu Add(Item item)
        {
            items.Add(item);
            item.Container = this;
            item.ID = itemStartID;
            itemStartID += 1;
            Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f));
            Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f));
            item.ValueWiggler.UseRawDeltaTime = (item.SelectWiggler.UseRawDeltaTime = true);
            if (Selection == -1)
            {
                FirstSelection();
            }
            RecalculateSize();
            item.Added();
            return this;
        }

        public void Clear()
        {
            items = new List<Item>();
        }

        public int IndexOf(Item item)
        {
            return items.IndexOf(item);
        }

        public void FirstSelection()
        {
            Selection = -1;
            MoveSelection(1);
        }

        public void MoveSelection(int direction, bool wiggle = false)
        {
            Level level = Scene as Level;
            int selection = Selection;
            direction = Math.Sign(direction);
            int num = 0;
            foreach (Item item in items)
            {
                if (item.Hoverable)
                {
                    num++;
                }
            }
            bool flag = num > 2;
            do
            {
                Selection += direction;
                if (flag)
                {
                    if (Selection < 0)
                    {
                        Selection = items.Count - 1;
                    }
                    else if (Selection >= items.Count)
                    {
                        Selection = 0;
                    }
                }
                else if (Selection < 0 || Selection > items.Count - 1)
                {
                    Selection = Calc.Clamp(Selection, 0, items.Count - 1);
                    break;
                }
            }
            while (!Current.Hoverable);
            if (!Current.Hoverable)
            {
                Selection = selection;
            }
            if (Selection != selection && Current != null)
            {
                if (selection >= 0 && items[selection] != null && items[selection].OnLeave != null)
                {
                    items[selection].OnLeave();
                }
                Current.OnEnter?.Invoke();
                if (wiggle)
                {
                    Audio.Play((direction > 0) ? "event:/ui/main/rollover_down" : "event:/ui/main/rollover_up");
                    Current.SelectWiggler.Start();
                }
            }
        }

        public void RecalculateSize()
        {
            float num2 = Height = 0f;
            float num5 = LeftColumnWidth = (RightColumnWidth = num2);
            foreach (Item item in items)
            {
                if (item.IncludeWidthInMeasurement)
                {
                    LeftColumnWidth = Math.Max(LeftColumnWidth, item.LeftWidth());
                }
            }
            foreach (Item item2 in items)
            {
                if (item2.IncludeWidthInMeasurement)
                {
                    RightColumnWidth = Math.Max(RightColumnWidth, item2.RightWidth());
                }
            }
            foreach (Item item3 in items)
            {
                if (item3.Visible && !item3.Hide)
                {
                    Height += item3.Height() + ItemSpacing;
                }
            }
            Height -= ItemSpacing;
            Height = Math.Min(Height, 10 * ActiveFont.LineHeight + 37);
            Width = Math.Max(MinWidth, LeftColumnWidth + RightColumnWidth);
        }

        public float GetYOffsetOf(Item item)
        {
            if (item == null)
            {
                return 0f;
            }
            float num = 0f;
            foreach (Item item2 in items)
            {
                if (item2.Visible && !item2.Hide)
                {
                    num += item2.Height() + ItemSpacing;
                }
                if (item2 == item)
                {
                    break;
                }
            }
            return num - item.Height() * 0.5f - ItemSpacing;
        }

        public void Close()
        {
            if (Current != null && Current.OnLeave != null)
            {
                Current.OnLeave();
            }
            OnClose?.Invoke();
            RemoveSelf();
        }

        public void CloseAndRun(IEnumerator routine, Action onClose)
        {
            Focused = false;
            Visible = false;
            Add(new Coroutine(CloseAndRunRoutine(routine, onClose)));
        }

        private IEnumerator CloseAndRunRoutine(IEnumerator routine, Action onClose)
        {
            yield return routine;
            onClose?.Invoke();
            Close();
        }

        public override void Update()
        {
            base.Update();
            OnUpdate?.Invoke();
            if (Focused)
            {
                if (Input.MenuDown.Pressed)
                {
                    if (Selection != LastPossibleSelection)
                    {
                        MoveSelection(1, wiggle: true);
                    }
                }
                else if (Input.MenuUp.Pressed && Selection != FirstPossibleSelection)
                {
                    MoveSelection(-1, wiggle: true);
                }
                if (Current != null)
                {
                    if (Input.MenuLeft.Pressed)
                    {
                        Current.LeftPressed();
                    }
                    if (Input.MenuRight.Pressed)
                    {
                        Current.RightPressed();
                    }
                    if (Input.MenuConfirm.Pressed)
                    {
                        Current.ConfirmPressed();
                        Current.OnPressed?.Invoke();
                    }
                    if (Input.MenuJournal.Pressed && Current.OnAltPressed != null)
                    {
                        Current.OnAltPressed();
                    }
                }
                if (!Input.MenuConfirm.Pressed)
                {
                    if (Input.MenuCancel.Pressed && OnCancel != null)
                    {
                        OnCancel();
                    }
                    else if (Input.ESC.Pressed && OnESC != null)
                    {
                        OnESC();
                    }
                    else if (Input.Pause.Pressed && OnPause != null)
                    {
                        OnPause();
                    }
                }
            }
            foreach (Item item in items)
            {
                item.OnUpdate?.Invoke();
                item.Update();
            }
            if (Settings.Instance.DisableFlashes)
            {
                HighlightColor = HighlightColorA;
            }
            else if (Engine.Scene.OnRawInterval(0.1f))
            {
                if (HighlightColor == HighlightColorA)
                {
                    HighlightColor = HighlightColorB;
                }
                else
                {
                    HighlightColor = HighlightColorA;
                }
            }
            if (AutoScroll)
            {
                if (Height > ScrollableMinSize)
                {
                    Position.Y += (ScrollTargetY - Position.Y) * (1f - (float)Math.Pow(0.0099999997764825821, Engine.RawDeltaTime));
                }
                else
                {
                    Position.Y = 540f;
                }
            }
            if (Current.ID <= 5)
            {
                foreach (Item item in items)
                {
                    if (item.ID >= 10)
                    {
                        item.Hide = true;
                    }
                    else
                    {
                        item.Hide = false;
                    }
                }
            }
            if (Current.ID > 5 && Current.ID <= LastPossibleSelection - 5)
            {
                foreach (Item item in items)
                {
                    if (item.ID <= Current.ID - 6 || item.ID >= Current.ID + 5)
                    {
                        item.Hide = true;
                    }
                    else
                    {
                        item.Hide = false;
                    }
                }
            }
            if (Current.ID > LastPossibleSelection - 5)
            {
                foreach (Item item in items)
                {
                    if (item.ID <= LastPossibleSelection - 10)
                    {
                        item.Hide = true;
                    }
                    else
                    {
                        item.Hide = false;
                    }
                }
            }
            if (!NoHeader)
            {
                header.UpdatePosition(new Vector2(Position.X, Position.Y - Height / 2 - 50));
                header.UpdateAlpha(1f);
            }
        }

        public override void Render()
        {
            RecalculateSize();
            Vector2 value = Position - Justify * new Vector2(Width, Height);
            foreach (Item item in items)
            {
                if (item.Visible && !item.Hide)
                {
                    float num = item.Height();
                    Vector2 position = value + new Vector2(0f, num * 0.5f + item.SelectWiggler.Value * 8f);
                    if (position.Y + num * 0.5f > 0f && position.Y - num * 0.5f < Engine.Height)
                    {
                        item.Render(position, Focused && Current == item);
                    }
                    value.Y += num + ItemSpacing;
                }
            }
        }

        public WarpMenu Insert(int index, Item item)
        {
            items.Insert(index, item);
            item.Container = this;
            Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f));
            Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f));
            item.ValueWiggler.UseRawDeltaTime = (item.SelectWiggler.UseRawDeltaTime = true);
            if (Selection == -1)
            {
                FirstSelection();
            }
            RecalculateSize();
            item.Added();
            return this;
        }

        public WarpMenu Remove(Item item)
        {
            int num = items.IndexOf(item);
            if (num == -1)
            {
                return this;
            }
            items.RemoveAt(num);
            item.Container = null;
            Remove(item.ValueWiggler);
            Remove(item.SelectWiggler);
            RecalculateSize();
            return this;
        }
    }
}
