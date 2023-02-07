using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked()]
    [CustomEntity("XaphanHelper/Sawblade")]
    class Sawblade : Entity
    {
        private string Mode;
        
        public Sprite saw;

        private Sprite node;

        private float radius;

        private Vector2[] nodes;

        public int currentNode;

        public bool Moving = true;

        private float totalLength;

        private List<float> nodesInfo = new();

        public float Percent { get; private set; }

        private float TotalTime;

        private string stopFlag;

        private string swapFlag;

        private string resumeFlag;

        private string moveFlag;

        private bool drawTrack;

        private bool swapped;

        private bool flagSwapped;

        public float index;

        private int amount;

        private float startOffset;

        private float spacingOffset;

        public float alpha = 0f;

        private string directory;

        private string lineColorA;

        private string lineColorB;

        private int ID;

        private int StartNode;

        private float StartPercent;

        public bool NodePaused;

        public bool AtStartOfTrack;

        public bool AtEndOfTrack;

        public Sawblade(EntityData data, Vector2 offset, EntityID eid) : this(data.NodesWithPosition(offset), data.Attr("mode", "Restart"), data.Attr("directory", "danger/XaphanHelper/Sawblade"), data.Float("radius", 12f),
            data.Float("totalTime", 2f), data.Attr("stopFlag", ""), data.Attr("swapFlag", ""), data.Attr("resumeFlag", ""), data.Attr("moveFlag", ""), data.Bool("drawTrack", true), 0, data.Int("amount", 3),
            data.Float("startOffset", 0f), data.Float("spacingOffset", 0.5f), data.Attr("lineColorA", "2A251F"), data.Attr("lineColorB", "C97F35"), eid.ID)
        {
            
        }

        public Sawblade(Vector2[] nodes, string mode, string directory, float radius, float totalTime, string stopFlag, string swapFlag, string resumeFlag, string moveFlag, bool drawTrack, int index, int amount, float startOffset, float spacingOffset, string lineColorA, string lineColorB, int id)
        {
            Tag = Tags.TransitionUpdate;
            Position = nodes[0];
            Collider = new Circle(radius);
            Add(new PlayerCollider(OnPlayer));
            this.nodes = nodes;
            this.radius = radius;
            Mode = mode;
            TotalTime = totalTime;
            this.stopFlag = stopFlag;
            this.swapFlag = swapFlag;
            this.resumeFlag = resumeFlag;
            this.moveFlag = moveFlag;
            this.drawTrack = drawTrack;
            this.index = index;
            this.amount = amount;
            this.startOffset = startOffset;
            this.spacingOffset = spacingOffset;
            this.directory = directory;
            this.lineColorA = lineColorA;
            this.lineColorB = lineColorB;
            ID = id;
            if (string.IsNullOrEmpty(this.directory))
            {
                this.directory = "danger/XaphanHelper/Sawblade";
            }
            Add(saw = new Sprite(GFX.Game, this.directory + "/"));
            saw.AddLoop("saw", "saw", 0.01f);
            saw.CenterOrigin();
            saw.Play("saw");
            Add(node = new Sprite(GFX.Game, this.directory + "/"));
            node.AddLoop("node", "node", 0.15f);
            node.CenterOrigin();
            node.Play("node");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (Mode.Contains("Pause At Each Node"))
            {
                NodePaused = true;
            }
            GenerateNodeInfo(true);
            if (index == 0)
            {
                if (startOffset != 0)
                {
                    SetPositionOnTrack(startOffset);
                }
                else
                {
                    Percent = 0f;
                }
            }
            else
            {
                SetPositionOnTrack(spacingOffset);
            }
            if (index == 0 && nodes.Count() > 1)
            {
                for (int i = 1; i < amount; i++)
                {
                    Scene.Add(new Sawblade(nodes, Mode, directory, radius, TotalTime, stopFlag, swapFlag, resumeFlag, moveFlag, false, i, amount, startOffset, spacingOffset * i, lineColorA, lineColorB, ID));
                }
            }
            UpdatePosition();
        }

        public void GenerateNodeInfo(bool generateTotalLength = false)
        {
            nodesInfo.Clear();
            for (int i = 0; i < (nodes.Length - 1); i++)
            {
                float distance = 0f;
                distance = Vector2.Distance(nodes[i], nodes[(i + 1) % nodes.Length]);
                if (generateTotalLength)
                {
                    totalLength += distance;
                }
                nodesInfo.Add(distance);
            }
        }

        private void SetPositionOnTrack(float offset)
        {
            float targetDistance = totalLength * offset + (index != 0 ? totalLength * startOffset : 0);
            float previousSectionsTotalDistance = 0;
            int node = 0;
            for (int i = 0; i < (nodesInfo.Count - 1); i++)
            {
                if (targetDistance > previousSectionsTotalDistance + nodesInfo[i])
                {
                    previousSectionsTotalDistance += nodesInfo[i];
                    node++;
                }
                else
                {
                    break;
                }
            }
            if (Mode == "Restart" && node == nodes.Length - 1)
            {
                node = 0;
            }
            currentNode = StartNode = node;
            float distanceOnCurrentSection = targetDistance - (previousSectionsTotalDistance);
            Percent = StartPercent = distanceOnCurrentSection * 100 / nodesInfo[currentNode] / 100;
            if (Mode == "Restart" && index != 0 && Percent >= 1f && currentNode == (nodes.Length - 2))
            {
                RemoveSelf();
            }
        }

        public void UpdatePosition()
        {

            Vector2 start = nodes[Math.Max(0, currentNode)];
            Vector2 end = nodes[Math.Min(currentNode + 1, (nodes.Length - 1))];
            Position = Vector2.Lerp(start, end, Ease.Linear(Percent));
        }

        public override void Update()
        {
            base.Update();
            alpha += Engine.DeltaTime * 4f;
            if (!string.IsNullOrEmpty(swapFlag))
            {
                flagSwapped = SceneAs<Level>().Session.GetFlag(swapFlag);
            }
            if (Mode == "Flag To Move" && !string.IsNullOrEmpty(moveFlag))
            {
                if (!SceneAs<Level>().Session.GetFlag(moveFlag))
                {
                    swapped = true;
                    if (AtEndOfTrack)
                    {
                        AtEndOfTrack = false;
                    }
                }
                else
                {
                    swapped = false;
                    if (AtStartOfTrack)
                    {
                        AtStartOfTrack = false;
                    }
                }
            }
            if (!Moving ||nodes.Count() == 1 || (!string.IsNullOrEmpty(stopFlag) && SceneAs<Level>().Session.GetFlag(stopFlag)) || (NodePaused && !string.IsNullOrEmpty(resumeFlag) && !SceneAs<Level>().Session.GetFlag(resumeFlag)) || AtStartOfTrack || AtEndOfTrack)
            {
                return;
            }
            NodePaused = false;
            if (!flagSwapped || Mode == "Flag To Move")
            {
                if (swapped)
                {
                    Percent = Calc.Approach(Percent, 0, Engine.DeltaTime / (nodesInfo[Math.Max(0, currentNode)] * TotalTime / totalLength));
                }
                else
                {
                    Percent = Calc.Approach(Percent, 1, Engine.DeltaTime / (nodesInfo[Math.Min(currentNode, (nodesInfo.Count - 1))] * TotalTime / totalLength));
                }
            }
            else
            {
                if (swapped)
                {
                    Percent =  Calc.Approach(Percent, 1, Engine.DeltaTime / (nodesInfo[Math.Min(currentNode, (nodesInfo.Count - 1))] * TotalTime / totalLength));
                }
                else
                {
                    Percent = Calc.Approach(Percent, 0, Engine.DeltaTime / (nodesInfo[Math.Max(0, currentNode)] * TotalTime / totalLength));
                }
            }
            UpdatePosition();
            if (Percent >= 1f)
            {
                if (!swapped)
                {
                    if (Mode == "Restart")
                    {
                        if (currentNode < nodes.Length - 2)
                        {
                            currentNode++;
                        }
                        else
                        {
                            currentNode = 0;
                        }
                        Percent = 0f;
                    }
                    else if (Mode.Contains("Back And Forth"))
                    {
                        if (currentNode < (nodes.Length - 2))
                        {
                            currentNode++;
                            Percent = 0f;
                        }
                        else
                        {
                            if (Mode.Contains("All Sawblades"))
                            {
                                foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                                {
                                    if (blade.ID == ID)
                                    {
                                        blade.swapped = true;
                                    }
                                }
                            }
                            else
                            {
                                swapped = true;
                            }
                        }
                    }
                    else if (Mode == "Flag To Move" && SceneAs<Level>().Session.GetFlag(moveFlag))
                    {
                        if (currentNode < nodes.Length - 2)
                        {
                            currentNode++;
                            Percent = 0f;
                        }
                        else
                        {
                            Percent = 1f;
                            foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                            {
                                if (blade.ID == ID)
                                {
                                    blade.AtEndOfTrack = true;
                                }
                            }
                        }
                    }
                }
                if (flagSwapped)
                {

                    if (currentNode < (nodes.Length - 2))
                    {
                        currentNode++;
                        Percent = 0f;
                    }
                    else if (Mode.Contains("Back And Forth"))
                    {
                        if (Mode.Contains("All Sawblades"))
                        {
                            foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                            {
                                if (blade.ID == ID)
                                {
                                    blade.swapped = false;
                                }
                            }
                        }
                        else
                        {
                            swapped = false;
                        }
                    }
                }
                if (Mode.Contains("Pause At Each Node") && !string.IsNullOrEmpty(resumeFlag))
                {
                    SceneAs<Level>().Session.SetFlag(resumeFlag, false);
                    foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                    {
                        if (blade.ID == ID)
                        {
                            blade.NodePaused = true;
                        }
                    }
                }
            }
            else if (Percent <= 0)
            {
                if (swapped)
                {
                    if (Mode == "Restart")
                    {
                        if (currentNode > 0)
                        {
                            currentNode--;
                        }
                        else
                        {
                            currentNode = (nodes.Length - 2);
                        }
                        Percent = 1f;
                    }
                    else if (Mode.Contains("Back And Forth"))
                    {
                        if (currentNode > 0)
                        {
                            currentNode--;
                            Percent = 1f;
                        }
                        else
                        {
                            if (Mode.Contains("All Sawblades"))
                            {
                                foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                                {
                                    if (blade.ID == ID)
                                    {
                                        blade.swapped = false;
                                        blade.currentNode = blade.StartNode;
                                        blade.Percent = blade.StartPercent;
                                    }
                                }
                            }
                            else
                            {
                                swapped = false;
                            }
                        }
                    }
                    else if (Mode == "Flag To Move" && !SceneAs<Level>().Session.GetFlag(moveFlag))
                    {
                        if (currentNode > 0)
                        {
                            currentNode--;
                            Percent = 1f;
                        }
                        else
                        {
                            Percent = 0f;
                            foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                            {
                                if (blade.ID == ID)
                                {
                                    blade.currentNode = blade.StartNode;
                                    blade.Percent = blade.StartPercent;
                                    blade.AtStartOfTrack = true;
                                }
                            }
                        }
                    }
                }
                if (flagSwapped)
                {
                    if (Mode == "Restart")
                    {
                        if (currentNode > 0)
                        {
                            currentNode--;
                        }
                        else
                        {
                            currentNode = nodes.Length - 2;
                        }
                        Percent = 1f;
                    }
                    else if (Mode.Contains("Back And Forth"))
                    {
                        if (currentNode > 0)
                        {
                            currentNode--;
                            Percent = 1f;
                        }
                        else
                        {
                            if (Mode.Contains("All Sawblades"))
                            {
                                foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                                {
                                    if (blade.ID == ID)
                                    {
                                        blade.swapped = true;
                                        blade.currentNode = blade.StartNode;
                                        blade.Percent = blade.StartPercent;
                                    }
                                }
                            }
                            else
                            {
                                swapped = true;
                            }
                        }
                    }
                }
                if (Mode.Contains("Pause At Each Node") && !string.IsNullOrEmpty(resumeFlag))
                {
                    SceneAs<Level>().Session.SetFlag(resumeFlag, false);
                    foreach (Sawblade blade in SceneAs<Level>().Tracker.GetEntities<Sawblade>())
                    {
                        if (blade.ID == ID)
                        {
                            blade.NodePaused = true;
                        }
                    }
                }
            }
        }

        public virtual void OnPlayer(Player player)
        {
            if (player.Die((player.Position - Position).SafeNormalize()) != null)
            {
                Moving = false;
            }
        }

        public override void Render()
        {
            if (nodes.Count() > 1 && drawTrack)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (i + 1 < nodes.Length)
                    {
                        Draw.Line(nodes[i], nodes[i + 1], Calc.HexToColor(lineColorA), 4);
                        Draw.Line(nodes[i], nodes[i + 1], Calc.HexToColor(lineColorB) * (0.7f * (0.7f + ((float)Math.Sin(alpha) + 1f) * 0.125f)), 2);
                    }
                    if (i < nodes.Length)
                    {
                        node.RenderPosition = nodes[i];
                        node.Render();
                    }
                }                
            }
            saw.Render();
        }
    }
}
