using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace Rougelike
{
    public class Node
    {
        int heuristicCost;
        Coordinates nodeCoordinates;
        State state;
        Node parentNode;

        public int X { get { return nodeCoordinates.X; } }
        public int Y { get { return nodeCoordinates.Y; } }
        public int Cost { get; private set; }
        public int Score { get { return Cost + heuristicCost; } }

        public Node(Coordinates nodeCoordinates, Coordinates goal)
        {
            heuristicCost = Math.Max(Math.Abs(goal.X - nodeCoordinates.X), Math.Abs(goal.Y - nodeCoordinates.Y));
            this.nodeCoordinates.X = nodeCoordinates.X;
            this.nodeCoordinates.Y = nodeCoordinates.Y;
            state = State.unopened;
        }

        public void _Open(int cost, Node parentNode)
        {
            Cost = cost;
            this.parentNode = parentNode;
            state = State.opened;
        }

        public void _Close()
        {
            state = State.closed;
        }

        public bool IsUnopened()
        {
            return state == State.unopened;
        }

        public bool IsDoor()
        {
            return Dungeon.map[nodeCoordinates.X, nodeCoordinates.Y] == (int)Tile.door;
        }

        public void _Path(List<Coordinates> path)
        {
            path.Add(nodeCoordinates);
            if (parentNode != null)
            {
                parentNode._Path(path);
            }
        }

        public void _Door(List<Coordinates> door)
        {
            if (IsDoor())
            {
                door.Add(nodeCoordinates);
            }

            if (parentNode != null)
            {
                parentNode._Door(door);
            }
        }
    }

    public class NodeControl
    {
        Dictionary<Coordinates, GameObject> obstacles;
        Dictionary<Coordinates, Node> nodes;
        List<Node> openedNodes;
        List<Node> closedNodes;

        public NodeControl(Dictionary<Coordinates, GameObject> obstacles)
        {
            this.obstacles = obstacles;
            nodes = new Dictionary<Coordinates, Node>();
            openedNodes = new List<Node>();
            closedNodes = new List<Node>();
        }

        public void _AddOpenList(Node node)
        {
            openedNodes.Add(node);
        }

        public void _AddClosedList(Node node)
        {
            openedNodes.Remove(node);
            closedNodes.Add(node);
        }

        public Node CreateNode(Coordinates nodeCoordinates, Coordinates goal)
        {
            if (nodes.ContainsKey(nodeCoordinates))
            {
                return nodes[nodeCoordinates];
            }
            var node = new Node(nodeCoordinates, goal);
            nodes[nodeCoordinates] = node;
            return node;
        }

        public Node OpenNode(Coordinates nodeCoordinates, Coordinates goal, int cost, Node parentNode)
        {
            if (!MasterData.nodeCandidates.Contains(Dungeon.map[nodeCoordinates.X, nodeCoordinates.Y]) || obstacles.ContainsKey(nodeCoordinates))
            {
                return null;
            }

            var node = CreateNode(nodeCoordinates, goal);
            if (node.IsUnopened() == false)
            {
                return null;
            }
            node._Open(cost, parentNode);
            _AddOpenList(node);
            return node;
        }

        public int[,] SetDirection(Coordinates nodeCoordinates, Coordinates goal)
        {
            if (goal.X > nodeCoordinates.X && goal.Y >= nodeCoordinates.Y)
            {
                return new int[8, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 1, 1 } };
            }
            else if (goal.X >= nodeCoordinates.X && goal.Y < nodeCoordinates.Y)
            {
                return new int[8, 2] { { 1, 0 }, { 0, -1 }, { -1, 0 }, { 0, 1 }, { -1, 1 }, { 1, 1 }, { -1, -1 }, { 1, -1 } };
            }
            else if (goal.X <= nodeCoordinates.X && goal.Y > nodeCoordinates.Y)
            {
                return new int[8, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 }, { 1, -1 }, { -1, -1 }, { 1, 1 }, { -1, 1 } };
            }
            else  // if (goal.X < nodeCoordinates.X && goal.Y <= nodeCoordinates.Y)
            {
                return new int[8, 2] { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 }, { 1, -1 }, { -1, -1 } };
            }
        }

        public void _OpenAround(Coordinates nodeCoordinates, Coordinates goal, Node parentNode)
        {
            int px = parentNode.X;
            int py = parentNode.Y;
            int cost = parentNode.Cost + 1;
            int[,] direction = SetDirection(nodeCoordinates, goal);

            for (int i = 0; i < 8; i++)
            {
                nodeCoordinates.X = px + direction[i, 0];
                nodeCoordinates.Y = py + direction[i, 1];
                OpenNode(nodeCoordinates, goal, cost, parentNode);
            }
        }

        public Node MinimumScoreNode(Coordinates goal)
        {
            int minCost = 9999;
            int minScore = 9999;
            int score = minScore;
            Node minNode = null;
            foreach (Node node in openedNodes)
            {
                score = node.Score;
                if (score > minScore)
                {
                    continue;
                }
                else if (score == minScore && node.Cost >= minCost)
                {
                    continue;
                }
                minScore = score;
                minCost = node.Cost;
                minNode = node;
            }
            return minNode;
        }
    }

    public static class AstarAlgorithm
    {
        public static List<Coordinates> GetPath(Coordinates start, Coordinates goal, Dictionary<Coordinates, GameObject> obstacles, bool seek)
        {
            var path = new List<Coordinates>();
            if (MasterData.nodeCandidates.Contains(Dungeon.map[goal.X, goal.Y]))
            {
                var door = new List<Coordinates>();
                var partialPath = new List<Coordinates>();
                _AstarAlgorithm(start, goal, ref door, obstacles, door: true);

                if (door.Count == 0)
                {
                    _AstarAlgorithm(start, goal, ref path, obstacles, door: false);
                }
                else
                {
                    int index = 0;
                    var destination = goal;
                    while (index <= door.Count)
                    {
                        start = (index > 0) ? door[index - 1] : start;
                        goal = (index < door.Count) ? door[index] : goal;

                        if (index == door.Count)
                        {
                            if (seek) { return path; }
                            else { goal = destination; }
                        }

                        _AstarAlgorithm(start, goal, ref partialPath, obstacles, door: false);
                        path.AddRange(partialPath);
                        partialPath.Clear();
                        index++;
                    }
                }
            }
            else
            {
                return null;
            }
            return path;
        }

        static void _AstarAlgorithm(Coordinates start, Coordinates goal, ref List<Coordinates> list, Dictionary<Coordinates, GameObject> obstacles, bool door)
        {
            var nodeControl = new NodeControl(obstacles);
            var node = nodeControl.OpenNode(start, goal, 0, null);

            int count = 0;
            while (count < 9999)
            {
                nodeControl._AddClosedList(node);
                nodeControl._OpenAround(start, goal, node);
                node = nodeControl.MinimumScoreNode(goal);
                if (node == null)
                {
                    break;
                }
                else if (node.X == goal.X && node.Y == goal.Y)
                {
                    nodeControl._AddClosedList(node);
                    if (door)
                    {
                        node._Door(list);
                        list.Reverse();
                    }
                    else
                    {
                        node._Path(list);
                        list.Reverse();
                        list.RemoveAt(0);
                    }
                    break;
                }
                count++;
            }
        }
    }

    public static class ObstacleSetter
    {
        public static Dictionary<Coordinates, GameObject> GetObstacles(Coordinates start, GameObject target)
        {
            var obstacles = new Dictionary<Coordinates, GameObject>();
            var sight = Sight(start);
            sight.Remove(start);

            foreach (var p in sight)
            {
                if (Spawn.characters.ContainsKey(p))
                {
                    obstacles[p] = Spawn.characters[p];
                }
            }

            if(target != null)
            {
                obstacles.RemoveByValue(target);
            }
            return obstacles;
        }

        public static List<Coordinates> Sight(Coordinates start)
        {
            var sight = new List<Coordinates>();
            int x = 1 + (start.X / (Dungeon.unitW + 1)) * (Dungeon.unitW + 1);
            int y = 1 + (start.Y / (Dungeon.unitH + 1)) * (Dungeon.unitH + 1);
            var p = new Coordinates(x, y);

            if (Dungeon.map[start.X, start.Y] == (int)Tile.floor)
            {
                for (int i = 0; i < Dungeon.unitW; i++)
                {
                    for (int j = 0; j < Dungeon.unitH; j++)
                    {
                        p.X = x + i;
                        p.Y = y + j;
                        if (Dungeon.map[p.X, p.Y] == (int)Tile.floor || Dungeon.map[p.X, p.Y] == (int)Tile.door)
                        {
                            sight.Add(p);
                        }
                    }
                }
            }
            else if (Dungeon.map[start.X, start.Y] == (int)Tile.door)
            {
                for (int i = 0; i < Dungeon.unitW; i++)
                {
                    for (int j = 0; j < Dungeon.unitH; j++)
                    {
                        p.X = x + i;
                        p.Y = y + j;
                        if (Dungeon.map[p.X, p.Y] == (int)Tile.floor || Dungeon.map[p.X, p.Y] == (int)Tile.door)
                        {
                            sight.Add(p);
                        }
                    }
                }
                if (Dungeon.map[start.X - 1, start.Y] == (int)Tile.floor)
                {
                    _ExpandSight(start, Direction.right, ref sight);
                }
                else if (Dungeon.map[start.X + 1, start.Y] == (int)Tile.floor)
                {
                    _ExpandSight(start, Direction.left, ref sight);
                }
                else if (Dungeon.map[start.X, start.Y - 1] == (int)Tile.floor)
                {
                    _ExpandSight(start, Direction.up, ref sight);
                }
                else if (Dungeon.map[start.X, start.Y + 1] == (int)Tile.floor)
                {
                    _ExpandSight(start, Direction.down, ref sight);
                }
            }
            else if (Dungeon.map[start.X, start.Y] == (int)Tile.path)
            {
                sight.Add(start);
                _ExpandSight(start, Direction.right, ref sight);
                _ExpandSight(start, Direction.left, ref sight);
                _ExpandSight(start, Direction.up, ref sight);
                _ExpandSight(start, Direction.down, ref sight);
            }

            return sight;
        }

        static void _ExpandSight(Coordinates start, Direction rlud, ref List<Coordinates> sight)
        {
            int[] ij = new int[2], xy = new int[2];
            _SetParameters(ref ij, ref xy, rlud);
            int i = ij[0], j = ij[1];
            var p = start;
            List<int> one = new List<int>(), theOther = new List<int>();

            while (Dungeon.map[start.X + i, start.Y + j] == (int)Tile.path || Dungeon.map[start.X + i, start.Y + j] == (int)Tile.door)
            {
                p.X = start.X + i;
                p.Y = start.Y + j;
                sight.Add(p);
                if (Dungeon.map[p.X + xy[0], p.Y + xy[1]] == (int)Tile.path || Dungeon.map[p.X + xy[0], p.Y + xy[1]] == (int)Tile.door)
                {
                    one.Add(j * xy[0] + i * xy[1]);
                }

                if (Dungeon.map[p.X - xy[0], p.Y - xy[1]] == (int)Tile.path || Dungeon.map[p.X - xy[0], p.Y - xy[1]] == (int)Tile.door)
                {
                    theOther.Add(j * xy[0] + i * xy[1]);
                }
                i += ij[0];
                j += ij[1];

                if (Math.Abs(i) > 7 || Math.Abs(j) > 7)
                {
                    break;
                }
            }

            __ExpandSight(start, one, xy, 1, ref sight);
            __ExpandSight(start, theOther, xy, -1, ref sight);
        }

        static void __ExpandSight(Coordinates start, List<int> list, int[] xy, int rw, ref List<Coordinates> sight)
        {
            if (list.Count > 0)
            {
                int x = 0, y = 0;
                var p = start;
                foreach (int l in list)
                {
                    x = l * xy[1];
                    y = l * xy[0];
                    p.X = start.X + x + rw * xy[0];
                    p.Y = start.Y + y + rw * xy[1];
                    sight.Add(p);
                    if (Math.Abs(l) == 1)
                    {
                        p.X = start.X + x + rw * xy[0] * 2;
                        p.Y = start.Y + y + rw * xy[1] * 2;
                        if (MasterData.nodeCandidates.Contains(Dungeon.map[p.X, p.Y]))
                        {
                            sight.Add(p);
                        }
                        p.X = start.X + x * 2 + rw * xy[0];
                        p.Y = start.Y + y * 2 + rw * xy[1];
                        if (MasterData.nodeCandidates.Contains(Dungeon.map[p.X, p.Y]))
                        {
                            sight.Add(p);
                        }
                    }
                }
            }
        }

        static void _SetParameters(ref int[] ij, ref int[] xy, Direction rlud)
        {
            if (rlud == Direction.right)
            {
                ij[0] = 1;
                xy[1] = 1;
            }
            else if (rlud == Direction.left)
            {
                ij[0] = -1;
                xy[1] = 1;
            }
            else if (rlud == Direction.up)
            {
                ij[1] = 1;
                xy[0] = 1;
            }
            else if (rlud == Direction.down)
            {
                ij[1] = -1;
                xy[0] = 1;
            }
        }
    }
}
