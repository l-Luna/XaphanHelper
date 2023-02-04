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
        private Sprite saw;

        private Sprite node;

        private float radius;

        private Vector2[] nodes;

        private int currentNode;

        public bool Moving = true;

        private float totalLength;

        private List<float> nodesInfo = new();

        public float Percent { get; private set; }

        private bool Clockwise = true;

        private float TotalTime;

        private bool SwapDirection;

        private bool StopAtEachNode;

        private string stopFlag;

        private string swapFlag;

        private string resumeFlag;

        private bool drawTrack;

        private bool swapped;

        private float index;

        private int amount;

        private float startOffset;

        private float spacingOffset;

        public float alpha = 0f;

        private string directory;

        private string lineColorA;

        private string lineColorB;

        public Sawblade(EntityData data, Vector2 offset) : this(data.NodesWithPosition(offset), data.Attr("directory", "danger/XaphanHelper/Sawblade"), data.Float("radius", 12f), data.Bool("swapDirection", false), data.Float("totalTime", 2f),
            data.Attr("stopFlag", ""), data.Attr("swapFlag", ""), data.Attr("resumeFlag", ""), data.Bool("drawTrack", true), 0,
            data.Int("amount", 3), data.Float("startOffset", 0f), data.Float("spacingOffset", 0.5f), data.Bool("stopAtEachNode", false), data.Attr("lineColorA", "2A251F"), data.Attr("lineColorB", "C97F35"))
        {
            
        }

        public Sawblade(Vector2[] nodes, string directory, float radius, bool swapDirection, float totalTime, string stopFlag, string swapFlag, string resumeFlag, bool drawTrack, int index, int amount, float startOffset, float spacingOffset, bool stopAtEachNode, string lineColorA, string lineColorB)
        {
            Tag = Tags.TransitionUpdate;
            Position = nodes[0];
            Collider = new Circle(radius);
            Add(new PlayerCollider(OnPlayer));
            this.nodes = nodes;
            this.radius = radius;
            SwapDirection = swapDirection;
            TotalTime = totalTime;
            this.stopFlag = stopFlag;
            this.swapFlag = swapFlag;
            this.resumeFlag = resumeFlag;
            this.drawTrack = drawTrack;
            this.index = index;
            this.amount = amount;
            this.startOffset = startOffset;
            this.spacingOffset = spacingOffset;
            StopAtEachNode = stopAtEachNode;
            this.directory = directory;
            this.lineColorA = lineColorA;
            this.lineColorB = lineColorB;
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
            GenerateNodeInfo(Clockwise, true);
            if (index == 0 && nodes.Count() > 1 && !StopAtEachNode)
            {
                for (int i = 1; i < amount; i++)
                {
                    Scene.Add(new Sawblade(nodes, directory, radius, SwapDirection, TotalTime, stopFlag, swapFlag, resumeFlag, false, i, amount, startOffset, spacingOffset * i, false, lineColorA, lineColorB));
                }
            }
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
            UpdatePosition();
        }

        private void GenerateNodeInfo(bool Clockwise, bool generateTotalLength = false)
        {
            nodesInfo.Clear();
            for (int i = 0; i < nodes.Length; i++)
            {
                float distance = 0f;
                distance = Vector2.Distance(nodes[i], nodes[Clockwise ? (i + 1) % nodes.Length : ((i - 1) % nodes.Length < 0) ? nodes.Length - 1 : (i - 1) % nodes.Length]);
                if (generateTotalLength && i < (nodes.Length - 1))
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
            if (!SwapDirection && node == nodes.Length - 1)
            {
                node = 0;
            }
            currentNode = node;
            float distanceOnCurrentSection = targetDistance - (previousSectionsTotalDistance);
            Percent = distanceOnCurrentSection * 100 / nodesInfo[currentNode] / 100;
            if (!SwapDirection && index != 0 && Percent >= 1f && currentNode == (nodes.Length - 2))
            {
                RemoveSelf();
            }
        }

        public void UpdatePosition()
        {
            var start = nodes[currentNode];
            var end = nodes[Clockwise ? (currentNode + 1) % nodes.Length : ((currentNode - 1) % nodes.Length < 0) ? nodes.Length - 1 : (currentNode - 1) % nodes.Length] ;
            Position = Vector2.Lerp(start, end, Ease.Linear(Percent));
        }

        public override void Update()
        {
            base.Update();
            alpha += Engine.DeltaTime * 4f;
            if (!string.IsNullOrEmpty(swapFlag) && SceneAs<Level>().Session.GetFlag(swapFlag) && !swapped)
            {
                swapped = true;
                GenerateNodeInfo(!Clockwise);
                currentNode = Clockwise ? currentNode + 1 : currentNode - 1;
                if (currentNode < 0)
                {
                    currentNode = nodes.Length - 1;
                }
                else if (currentNode > nodes.Length - 1)
                {
                    currentNode = 0;
                }
                Clockwise = !Clockwise;
                Percent = 1 - Percent;
            }
            else if (!string.IsNullOrEmpty(swapFlag) && !SceneAs<Level>().Session.GetFlag(swapFlag) && swapped)
            {
                swapped = false;
                GenerateNodeInfo(!Clockwise);
                currentNode = Clockwise ? currentNode + 1 : currentNode - 1;
                if (currentNode < 0)
                {
                    currentNode = nodes.Length - 1;
                }
                else if (currentNode > nodes.Length - 1)
                {
                    currentNode = 0;
                }
                Clockwise = !Clockwise;
                Percent = 1 - Percent;
            }
            if (!Moving ||nodes.Count() == 1 || (!string.IsNullOrEmpty(stopFlag) && SceneAs<Level>().Session.GetFlag(stopFlag)) || (StopAtEachNode && Percent == 0 && !string.IsNullOrEmpty(resumeFlag) && !SceneAs<Level>().Session.GetFlag(resumeFlag)))
            {
                return;
            }
            Percent = Calc.Approach(Percent, 1, Engine.DeltaTime / (nodesInfo[currentNode] * TotalTime / totalLength));
            UpdatePosition();
            if (Percent >= 1f)
            {
                currentNode = Clockwise ? (currentNode + 1) % nodes.Length : ((currentNode - 1) % nodes.Length < 0) ? nodes.Length - 1 : (currentNode - 1) % nodes.Length;
                if (!SwapDirection && currentNode == nodes.Length - 1 && !swapped)
                {
                    currentNode = 0;
                }
                else if (!SwapDirection && currentNode == 0 && swapped)
                {
                    currentNode = nodes.Length - 1;
                }
                else if (SwapDirection && currentNode == (nodes.Length - 1))
                {
                    GenerateNodeInfo(!Clockwise);
                    currentNode = nodes.Length - 1;
                    Clockwise = !Clockwise;
                }
                else if (SwapDirection && currentNode == 0)
                {
                    GenerateNodeInfo(!Clockwise);
                    currentNode = 0;
                    Clockwise = !Clockwise;
                }
                Percent = 0f;
                if (StopAtEachNode && !string.IsNullOrEmpty(resumeFlag))
                {
                    SceneAs<Level>().Session.SetFlag(resumeFlag, false);
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
