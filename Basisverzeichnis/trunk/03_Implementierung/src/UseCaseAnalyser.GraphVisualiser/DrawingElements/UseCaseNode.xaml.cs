﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    ///     Interaction logic for UseCaseNode.xaml
    /// </summary>
    internal partial class UseCaseNode : ISelectableObject
    {
        public readonly List<UseCaseEdge> mEdges = new List<UseCaseEdge>();

        public UseCaseNode(INode node)
        {
            Selected = false;
            InitializeComponent();
            LblIndex.Content = node.Attributes.First(attr =>
                attr.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]).Value;
            Node = node;
            mDrawingBrush = NodeBorder.BorderBrush = Brushes.Black;
        }



        public INode Node { get; private set; }

        public bool Selected { get; set; }

        public void RenderEdges()
        {
            foreach (UseCaseEdge ucEdge in mEdges)
            {
                ucEdge.RecalcBezier();
                if (ucEdge.mDestUseCaseNode != this)
                    ucEdge.mDestUseCaseNode.RenderEdgesExceptNode(this);
                else
                    ucEdge.mSourceUseCaseNode.RenderEdgesExceptNode(this);
            }
        }
        public void RenderEdgesExceptNode(UseCaseNode notRenderNode)
        {
            foreach (UseCaseEdge ucEdge in mEdges)
            {
                if (ucEdge.mSourceUseCaseNode == notRenderNode || ucEdge.mDestUseCaseNode == notRenderNode)
                    continue;

                ucEdge.RecalcBezier();
                //if (ucEdge.mDestUseCaseNode != this)
                //    ucEdge.mDestUseCaseNode.RenderEdges();
                //else
                //    ucEdge.mSourceUseCaseNode.RenderEdges();
            }
        }

        public void AddEdge(UseCaseEdge newEdge)
        {
            if (!mEdges.Contains(newEdge))
                mEdges.Add(newEdge);
        }

        public int GetEdgeIndex(UseCaseEdge sourceKante)
        {
            UseCaseEdge.DockedStatus currentDockedStatus = sourceKante.mSourceUseCaseNode.Equals(this)
                ? sourceKante.StatusSourceElement
                : sourceKante.StatusDestElement;

            for (int i = 0, index = 1; i < mEdges.Count; i++)
            {
                if (mEdges[i].mSourceUseCaseNode.Equals(this) && mEdges[i].StatusSourceElement == currentDockedStatus ||
                    mEdges[i].mDestUseCaseNode.Equals(this) && mEdges[i].StatusDestElement == currentDockedStatus)
                {
                    if (sourceKante.Equals(mEdges[i]))
                        return index;
                    index++;
                }
            }
            return 0;
        }

        public int GetCountOfEdges(UseCaseEdge sourceKante)
        {
            int index = 1;

            UseCaseEdge.DockedStatus currentDockedStatus = sourceKante.mSourceUseCaseNode.Equals(this)
                ? sourceKante.StatusSourceElement
                : sourceKante.StatusDestElement;


            // ReSharper disable once LoopCanBeConvertedToQuery
            // [Fettstorch] keep more readable version of loop
            foreach (UseCaseEdge useCaseEdge in mEdges)
            {
                if (useCaseEdge.mSourceUseCaseNode.Equals(this) &&
                    useCaseEdge.StatusSourceElement == currentDockedStatus ||
                    useCaseEdge.mDestUseCaseNode.Equals(this) && useCaseEdge.StatusDestElement == currentDockedStatus)
                {
                    index++;
                }
            }
            return index;
        }


        private Brush mDrawingBrush;

        public void SetDrawingBrush(Brush newBrush)
        {
            NodeBorder.BorderBrush = mDrawingBrush = newBrush;
        }


        public void Select()
        {
            Selected = true;
            NodeBorder.BorderBrush = Brushes.Orange;
        }

        public void Unselect()
        {
            Selected = false;
            NodeBorder.BorderBrush = mDrawingBrush;
        }
        public void ChangeSelection()
        {
            if (Selected)
                Unselect();
            else
                Select();
        }
        public IGraphElement CurrentElement
        {
            get
            {
                return Node;
            }
        }
    }
}