using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPI311.GameEngine
{
    public class AStarSearch
    {
        public int Cols { get; set; }
        public int Rows { get; set; }
        public AStarNode[,] Nodes { get; set; }
        public AStarNode Start { get; set; }
        public AStarNode End { get; set; }

        private SortedDictionary<float, List<AStarNode>> openList; // Need to store multiple nodes with same F-value, so need List<> for SortedDictionary

        public AStarSearch(int rows, int cols)
        {
            openList = new SortedDictionary<float, List<AStarNode>>();
            Rows = rows;
            Cols = cols;
            Nodes = new AStarNode[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Nodes[r, c] = new AStarNode(c, r, new Vector3(c, 0, r));
        }

        public AStarSearch()
        {
            // TODO: Complete member initialization
        }

        public void Search()
        {
            #region Initialize grid
            openList.Clear();
            foreach (AStarNode node in Nodes)
            {
                node.Closed = false;
                node.Cost = Single.MaxValue;
                node.Parent = null;
                node.Heuristic = Vector3.Distance(node.Position, End.Position);
            }
            #endregion

            AddToOpenList(Start); // only first search

            while (openList.Count > 0) // if openSet is empty, break (p191, last if-statement)
            {
                AStarNode node = GetBestNode(); // (p192: 1-3)

                if (node == End)  // Until currentNode == endNode (p192: 4)
                    break;
                if (node.Row < Rows - 1)   // foreach node n adjacent to currentNode (p191: first of do-loop)
                    AddToOpenList(Nodes[node.Row + 1, node.Col], node);
                if (node.Row > 0)
                    AddToOpenList(Nodes[node.Row - 1, node.Col], node);
                if (node.Col < Cols - 1)
                    AddToOpenList(Nodes[node.Row, node.Col + 1], node);
                if (node.Col > 0)
                    AddToOpenList(Nodes[node.Row, node.Col - 1], node);
            }
        }

        private void AddToOpenList(AStarNode node, AStarNode parent = null)
        {
            if (!node.Passable || node.Closed) return;  // if closedSet contains n, continue (p191: 5)
            if (parent == null) node.Cost = 0; // *** only for the startNode case
            else
            {
                float cost = parent.Cost + 1;  // *** compute new_g, g(x) value for n with currentNode as parent
                if (node.Cost > cost)  // if (new_g < n.g)
                {
                    RemoveFromOpenList(node);
                    node.Cost = cost;
                    node.Parent = parent;
                }
                else
                    return;
            }
            float key = node.Cost + node.Heuristic; //add n to openSet (p191: 19)
            if (!openList.ContainsKey(key)) openList[key] = new List<AStarNode>();
            openList[key].Add(node);
        }

        private AStarNode GetBestNode()
        {
            AStarNode node = openList.ElementAt(0).Value[0]; // node with lowest F in openSet (p192: 1- 3)
            openList.ElementAt(0).Value.Remove(node);        // remove current node (minimum F) from openSet 
            if (openList.ElementAt(0).Value.Count == 0)
                openList.Remove(node.Cost + node.Heuristic); //*** remove the empty list from openSet
            node.Closed = true; // add current node to closedSet
            return node;
        }

        private void RemoveFromOpenList(AStarNode node)
        {
            float key = node.Cost + node.Heuristic;
            if (openList.ContainsKey(key))
            {
                openList[key].Remove(node);
                if (openList[key].Count == 0)
                    openList.Remove(key);
            }
        }
    }
}
