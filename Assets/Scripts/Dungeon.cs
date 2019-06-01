using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace Rougelike
{
    public class Dungeon : MonoBehaviour
    {
        private static int unitW;
        private static int unitH;
        private static int nDivW;
        private static int nDivH;
        private static int width;
        private static int height;

        public static int[,] map;
        public static SpriteRenderer[,] sprites;
        public static List<int> nRoom;
        public static int[,][] rooms;
        public static int nDoor;
        public static Dictionary<int, Coordinates> doors;
        public static int nHDoor;
        public static Dictionary<int, Coordinates> hDoors;
        public static Dictionary<Coordinates, GameObject> chars;
        public static Dictionary<Coordinates, List<GameObject>> items;

        void Start()
        {
            _Initialize(unitW = 10, unitH = 8, nDivW = 4, nDivH = 3);
            int[] dx = Enumerable.Range(1, nDivW).ToArray();
            int[] dr = GeneratePermutation(nDivH, nDivW);
            int[] dp = GeneratePermutation(nDivH - 1, nDivW);
            _Modify(ref dp, dr);

            _Divide();
            Room();
            _OverwriteRLUDPath((int)Direction.right);
            _OverwriteRLUDPath((int)Direction.left);
            _OverwriteRLUDPath((int)Direction.up);
            _OverwriteRLUDPath((int)Direction.down);
            _OverwriteHVPath(true);
            _OverwriteHVPath(false);
            _DeletePath(dp);
            _DeleteRoom(dr);
            _Connect(dr, dp);
            HiddenRoom(dr, dp);
            _Door();
            Generate();
        }

        void _Initialize(int unitW, int unitH, int nDivW, int nDivH)
        {
            Dungeon.unitW = unitW;
            Dungeon.unitH = unitH;
            Dungeon.nDivW = nDivW + 2;
            Dungeon.nDivH = nDivH + 2;
            width = (unitW + 1) * Dungeon.nDivW + 1;
            height = (unitH + 1) * Dungeon.nDivH + 1;
            map = new int[width, height];
            sprites = new SpriteRenderer[width, height];
            nRoom = new List<int>();
            int length = Enum.GetValues(typeof(Index)).Length;
            rooms = new int[Dungeon.nDivW, Dungeon.nDivH][];
            for (int x = 0; x < Dungeon.nDivW; x++)
            {
                for (int y = 0; y < Dungeon.nDivH; y++)
                {
                    rooms[x, y] = new int[length];
                }
            }
            nDoor = 0;
            doors = new Dictionary<int, Coordinates>();
            nHDoor = 0;
            hDoors = new Dictionary<int, Coordinates>();
            chars = new Dictionary<Coordinates, GameObject>();
            items = new Dictionary<Coordinates, List<GameObject>>();
        }

        void _Divide()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = (int)Tile.end;
                    }
                    else if (x == unitW + 1 || x == width - unitW - 2 || y == unitH + 1 || y == height - unitH - 2)
                    {
                        map[x, y] = (int)Tile.border;
                    }
                    else if (x % (unitW + 1) == 0 || y % (unitH + 1) == 0)
                    {
                        map[x, y] = (int)Tile.division;
                    }
                }
            }
        }

        void Room()
        {
            Coordinates roomIndex = new Coordinates(0, 0);
            for (int x = 1; x < nDivW - 1; x++)
            {
                roomIndex.X = x;
                for (int y = 1; y < nDivH - 1; y++)
                {
                    roomIndex.Y = y;
                    _SquareRoom(roomIndex, false);
                }
            }
        }

        void _SquareRoom(Coordinates roomIndex, bool hidden)
        {
            int roomW = UnityEngine.Random.Range(unitW * 2 / 4, unitW - 2);
            int roomH = UnityEngine.Random.Range(unitH * 2 / 4, unitH - 2);
            int xLeft = 1 + (unitW + 1) * roomIndex.X + UnityEngine.Random.Range(1, unitW - roomW);
            int yBottom = 1 + (unitH + 1) * roomIndex.Y + UnityEngine.Random.Range(1, unitH - roomH);
            for (int dx = 0; dx < roomW; dx++)
            {
                for (int dy = 0; dy < roomH; dy++)
                {
                    map[xLeft + dx, yBottom + dy] = (hidden) ? (int)Tile.hFloor : (int)Tile.floor;
                }
            }
            rooms[roomIndex.X, roomIndex.Y][(int)Index.width] = roomW;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.height] = roomH;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.xLeft] = xLeft;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.yBottom] = yBottom;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.shape] = (int)Shape.square;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.type] = (hidden) ? (int)Type.hidden : (int)Type.normal;
        }

        void _CircleRoom(Coordinates roomIndex)
        {
            int xLeft = 1 + (unitW + 1) * roomIndex.X;
            int yBottom = 1 + (unitH + 1) * roomIndex.Y;
            int size = Math.Min(unitW, unitH);
            int abs_add_min = size / 3;
            int abs_add_max = (size - 1) * 2 - abs_add_min;
            int abs_sub_max = size - abs_add_min - 1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (abs_add_min <= Math.Abs(i + j) && Math.Abs(i + j) <= abs_add_max && Math.Abs(i - j) <= abs_sub_max)
                    {
                        map[xLeft + i, yBottom + j] = (int)Tile.hFloor;
                    }
                }
            }
            rooms[roomIndex.X, roomIndex.Y][(int)Index.width] = size - abs_add_min * 2;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.height] = size - abs_add_min * 2;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.xLeft] = xLeft + abs_add_min;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.yBottom] = yBottom + abs_add_min ;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.shape] = (int)Shape.circle;
            rooms[roomIndex.X, roomIndex.Y][(int)Index.type] = (int)Type.hidden;
        }

        void _OverwriteRLUDPath(int rlud)
        {
            int xLeft, dx, yBottom, dy;
            int[] forth = { (int)Tile.wall, (int)Tile.floor };
            int[] back = { (int)Tile.division, (int)Tile.path };
            var direction = new int[2];
            for (int x = 1; x < nDivW - 1; x++)
            {
                for (int y = 1; y < nDivH - 1; y++)
                {
                    xLeft = rooms[x, y][(int)Index.xLeft];
                    dx = UnityEngine.Random.Range(0, rooms[x, y][(int)Index.width]);
                    yBottom = rooms[x, y][(int)Index.yBottom];
                    dy = UnityEngine.Random.Range(0, rooms[x, y][(int)Index.height]);
                    _SetDirection(ref direction, rlud, true);
                    while (forth.Contains(map[xLeft + dx, yBottom + dy]))
                    {
                        dx += direction[0];
                        dy += direction[1];
                    }

                    if (back.Contains(map[xLeft + dx, yBottom + dy]))
                    {
                        _SetDirection(ref direction, rlud, false);
                        rooms[x, y][rlud] = (rlud >= (int)Direction.up) ? xLeft + dx : yBottom + dy;
                        do
                        {
                            map[xLeft + dx, yBottom + dy] = (int)Tile.path;
                            dx += direction[0];
                            dy += direction[1];
                        } while (map[xLeft + dx, yBottom + dy] == (int)Tile.wall);
                    }
                }
            }
        }

        void _SetDirection(ref int[] xy, int rlud, bool dir)
        {
            int _dir = (dir) ? 1 : -1;
            if (rlud <= (int)Direction.left)
            {
                xy[0] = (rlud == (int)Direction.left) ? -1 * _dir : 1 * _dir;
            }
            else if (rlud >= (int)Direction.up)
            {
                xy[1] = (rlud == (int)Direction.up) ? 1 * _dir : -1 * _dir;
            }

        }

        void _OverwriteHVPath(bool is_horizontal)
        {
            int xLeft, yBottom, firstPath, secondPath;
            for (int x = 1; x < nDivW - 1; x++)
            {
                xLeft = 1 + (unitW + 1) * x;
                for (int y = 1; y < nDivH - 1; y++)
                {
                    yBottom = 1 + (unitH + 1) * y;
                    firstPath = 0;
                    secondPath = 0;
                    for (int d = 0; d < ((is_horizontal) ? unitW : unitH); d++)
                    {
                        if ((is_horizontal ? map[xLeft + d, yBottom + unitH] : map[xLeft + unitW, yBottom + d]) == (int)Tile.path)
                        {
                            if (firstPath == 0)
                            {
                                firstPath = d;
                            }
                            else
                            {
                                secondPath = d;
                            }
                        }
                    }

                    if (secondPath != 0)
                    {
                        for (int d = (firstPath + 1); d < secondPath; d++)
                        {
                            map[xLeft + (is_horizontal ? d : unitW), yBottom + (is_horizontal ? unitH : d)] = (int)Tile.path;
                        }
                    }
                }
            }
        }

        int[] GeneratePermutation(int max, int length)
        {
            int[] d = new int[length];
            int[] array = Enumerable.Range(1, max - 2).ToArray();
            array = array.OrderBy(i => System.Guid.NewGuid()).ToArray();
            for (int i = 1; i < d.Length - 1; i++)
            {
                d[i] = array[i % (max - 2)];
            }
            return d;
        }

        void _Modify(ref int[] dp, int[] dr)
        {
            int k = 1;
            for (int _ = 0; _ < 2; _++)
            {
                if (dr[k] == 1 && dp[k] == 1)
                {
                    dp[k] = UnityEngine.Random.Range(2, nDivH - 2);
                }
                else if (dr[k] == nDivH - 2 && dp[k] == nDivH - 3)
                {
                    dp[k] = UnityEngine.Random.Range(1, nDivH - 3);
                }
                k = nDivW - 2;
            }
        }

        void _DeletePath(int[] dp)
        {
            int x, bottomH, topH;
            for (int n = 1; n < nDivW - 1; n++)
            {
                x = 1 + (unitW + 1) * n;
                bottomH = rooms[n, dp[n]][(int)Index.yBottom] + rooms[n, dp[n]][(int)Index.height];
                topH = rooms[n, dp[n] + 1][(int)Index.yBottom];
                for (int i = 0; i < unitW; i++)
                {
                    for (int j = bottomH; j < topH; j++)
                    {
                        map[x + i, j] = 0;
                    }
                }
                rooms[n, dp[n]][(int)Index.topDoor] = 0;
                rooms[n, dp[n] + 1][(int)Index.bottomDoor] = 0;
            }
        }

        void _DeleteRoom(int[] dr)
        {
            int xLeft, yBottom;
            for (int n = 1; n < nDivW - 1; n++)
            {
                xLeft = n * (unitW + 1) + 1;
                yBottom = (dr[n]) * (unitH + 1) + 1;
                for (int j = 1; j < unitH - 1; j++)
                {
                    for (int i = 1; i < unitW - 1; i++)
                    {
                        map[xLeft + i, yBottom + j] = 0;
                    }
                }
                Array.Clear(rooms[n, dr[n]], 0, 4);
                rooms[n, dr[n]][(int)Index.type] = (int)Type.deleted;
            }
        }

        RL CountSteps(RL x, int n, int[] dr, int notNoneIndex, bool isCross)
        {
            var steps = new RL(0, 0);
            if (rooms[n, dr[n]][(int)Index.rightDoor] == 0)
            {
                int maxLeft = (isCross) ? Math.Max(rooms[n, dr[n]][(int)Index.topDoor], rooms[n, dr[n]][(int)Index.bottomDoor]) : rooms[n, dr[n]][notNoneIndex];
                steps.R = maxLeft - x.L + 1;
            }
            else if (rooms[n, dr[n]][(int)Index.leftDoor] == 0)
            {
                int minLeft = (isCross) ? Math.Min(rooms[n, dr[n]][(int)Index.topDoor], rooms[n, dr[n]][(int)Index.bottomDoor]) : rooms[n, dr[n]][notNoneIndex];
                steps.L = x.R - minLeft + 1;
            }
            else
            {
                steps.R = rooms[n, dr[n]][notNoneIndex] - x.L;
                steps.L = x.R - rooms[n, dr[n]][notNoneIndex];
                if ((rooms[n, dr[n]][(int)Index.rightDoor] < rooms[n, dr[n]][(int)Index.leftDoor]) ^ notNoneIndex == (int)Index.bottomDoor)
                {
                    steps.L += 1;
                }
                else
                {
                    steps.R += 1;
                }
            }
            return steps;
        }

        void _Connect(int[] dr, int[] dp)
        {
            var x = new RL(0, 0);
            for (int n = 1; n < nDivW - 1; n++)
            {
                var steps = new RL(0, 0);
                x.L = (unitW + 1) * n;
                x.R = (unitW + 1) * (n + 1) - 1;
                if (rooms[n, dr[n]][(int)Index.topDoor] != 0 && rooms[n, dr[n]][(int)Index.bottomDoor] != 0)
                {
                    steps = CountSteps(x, n, dr, (int)Index.bottomDoor, true);
                }
                else if (rooms[n, dr[n]][(int)Index.topDoor] != 0 || rooms[n, dr[n]][(int)Index.bottomDoor] != 0)
                {
                    int notNoneIndex = (rooms[n, dr[n]][(int)Index.topDoor] != 0) ? (int)Index.topDoor : (int)Index.bottomDoor;
                    steps = CountSteps(x, n, dr, notNoneIndex, false);
                }
                else
                {
                    steps.R = UnityEngine.Random.Range(2, unitW - 1);
                    steps.L = unitW - steps.R;
                    steps.R += 1;
                    steps.L += 1;
                    rooms[n, dr[n]][(int)Index.bottomDoor] = x.L + steps.R - 1;
                }
                _ConnectHorizontalPath(x, steps, n, dr);
            }

            int yTop, yBottom;
            for (int n = 1; n < nDivW - 1; n++)
            {
                yTop = (unitH + 1) * (dr[n] + 1) - 1;
                yBottom = (unitH + 1) * dr[n] + 1;
                _ConnectVerticalPath(yTop, yBottom, n, dr, dp);
            }
        }

        void _ConnectHorizontalPath(RL x, RL steps, int n, int[] dr)
        {
            if (rooms[n, dr[n]][(int)Index.leftDoor] != 0)
            {
                for (int r = 0; r < steps.R; r++)
                {
                    map[x.L + r, rooms[n, dr[n]][(int)Index.leftDoor]] = (int)Tile.path;
                }
            }

            if (rooms[n, dr[n]][(int)Index.rightDoor] != 0)
            {
                for (int l = 0; l < steps.L; l++)
                {
                    map[x.R - l, rooms[n, dr[n]][(int)Index.rightDoor]] = (int)Tile.path;
                }
            }
        }

        void _ConnectVerticalPath(int yTop, int yBottom, int n, int[] dr, int[] dp)
        {
            int u = 0, d = 0;
            if ((dr[n] == 1 && dp[n] == 1) || (dr[n] == nDivH - 2 && dp[n] == nDivH - 3))
            {
                if (rooms[n, dr[n]][(int)Index.rightDoor] == rooms[n, dr[n]][(int)Index.leftDoor]) { }
                else
                {
                    yBottom = Math.Min(rooms[n, dr[n]][(int)Index.rightDoor], rooms[n, dr[n]][(int)Index.leftDoor]);
                    do
                    {
                        map[rooms[n, dr[n]][(int)Index.bottomDoor], yBottom + d] = (int)Tile.path;
                        d += 1;
                    } while (map[rooms[n, dr[n]][(int)Index.bottomDoor], yBottom + d] == (int)Tile.wall);
                }
                rooms[n, dr[n]][(int)Index.bottomDoor] = 0;
            }
            else
            {
                if (rooms[n, dr[n]][(int)Index.bottomDoor] != 0)
                {
                    do
                    {
                        map[rooms[n, dr[n]][(int)Index.bottomDoor], yBottom + d] = (int)Tile.path;
                        d += 1;
                    } while (map[rooms[n, dr[n]][(int)Index.bottomDoor], yBottom + d] == 0);
                }
                if (rooms[n, dr[n]][(int)Index.topDoor] != 0)
                {
                    do
                    {
                        map[rooms[n, dr[n]][(int)Index.topDoor], yTop - u] = (int)Tile.path;
                        u += 1;
                    } while (map[rooms[n, dr[n]][(int)Index.topDoor], yTop - u] == 0);
                }
            }
        }

        void HiddenRoom(int[] dr, int[] dp)
        {
            int rlud;
            var roomIndex = new Coordinates(0, 0);
            int length = Enum.GetValues(typeof(HiddenSubCategory)).Length;
            int[] types = Enumerable.Range(1, length).ToArray();
            types = types.OrderBy(i => Guid.NewGuid()).ToArray();
            for (int n = 1; n < nDivW - 1; n++)
            {
                roomIndex.X = n;
                if (dr[n] == 1 || dr[n] == nDivH - 2)
                {
                    roomIndex.Y = (dr[n] == 1) ? 0 : nDivH - 1;
                    _HiddenRoom(roomIndex, types[n - 1]);
                    rlud = (dr[n] == 1) ? (int)Direction.up : (int)Direction.down;
                    _ConnectHiddenRoom(roomIndex, rlud);
                }
                else if (n == 1 || n == nDivW - 2)
                {
                    roomIndex.X = (n == 1) ? n - 1 : n + 1;
                    roomIndex.Y = dr[n];
                    _HiddenRoom(roomIndex, types[n - 1]);
                    rlud = (n == 1) ? (int)Direction.right : (int)Direction.left;
                    _ConnectHiddenRoom(roomIndex, rlud);
                }
            }
        }

        void _HiddenRoom(Coordinates roomIndex, int subCategory)
        {
            switch (subCategory)
            {
                case (int)HiddenSubCategory.challenge:
                case (int)HiddenSubCategory.secret:
                case (int)HiddenSubCategory.warehouse:
                    _SquareRoom(roomIndex, true);
                    break;
                case (int)HiddenSubCategory.garden:
                case (int)HiddenSubCategory.spring:
                case (int)HiddenSubCategory.tree:
                    _CircleRoom(roomIndex);
                    break;
                default:
                    break;
            }
            rooms[roomIndex.X, roomIndex.Y][(int)Index.category] = subCategory;
        }

        void _ConnectHiddenRoom(Coordinates roomIndex, int rlud)
        {
            var start = new Coordinates(0, 0);
            start.X = rooms[roomIndex.X, roomIndex.Y][(int)Index.xLeft] + UnityEngine.Random.Range(0, rooms[roomIndex.X, roomIndex.Y][(int)Index.width]);
            start.Y = rooms[roomIndex.X, roomIndex.Y][(int)Index.yBottom] + UnityEngine.Random.Range(0, rooms[roomIndex.X, roomIndex.Y][(int)Index.height]);

            var direction = new int[2];
            _SetDirection(ref direction, rlud, true);
            roomIndex.X += direction[0];
            roomIndex.Y += direction[1];

            var goal = new Coordinates(0, 0);
            bool is_horizontal = true;
            switch (rlud)
            {
                case (int)Direction.right:
                case (int)Direction.left:
                    goal.X = (rlud == (int)Direction.left) ? 1 + (unitW + 1) * roomIndex.X : -1 + (unitW + 1) * (roomIndex.X + 1);
                    if (rooms[roomIndex.X, roomIndex.Y][(int)Index.topDoor] != 0 && rooms[roomIndex.X, roomIndex.Y][(int)Index.bottomDoor] != 0)
                    {
                        goal.Y = (UnityEngine.Random.Range(0, 100) < 50) ? 1 + (unitH + 1) * roomIndex.Y : -1 + (unitH + 1) * (roomIndex.Y + 1);
                    }
                    else
                    {
                        goal.Y = (rooms[roomIndex.X, roomIndex.Y][(int)Index.topDoor] == 0) ? 1 + (unitH + 1) * roomIndex.Y : -1 + (unitH + 1) * (roomIndex.Y + 1);
                    }
                    break;
                case (int)Direction.up:
                case (int)Direction.down:
                    goal.Y = (rlud == (int)Direction.down) ? 1 + (unitH + 1) * roomIndex.Y : -1 + (unitH + 1) * (roomIndex.Y + 1);
                    if (rooms[roomIndex.X, roomIndex.Y][(int)Index.rightDoor] != 0 && rooms[roomIndex.X, roomIndex.Y][(int)Index.leftDoor] != 0)
                    {
                        goal.X = (UnityEngine.Random.Range(0, 100) < 50) ? 1 + (unitW + 1) * roomIndex.X : -1 + (unitW + 1) * (roomIndex.X + 1);
                    }
                    else
                    {
                        goal.X = (rooms[roomIndex.X, roomIndex.Y][(int)Index.rightDoor] == 0) ? 1 + (unitW + 1) * roomIndex.X : -1 + (unitW + 1) * (roomIndex.X + 1);
                    }
                    is_horizontal = false;
                    break;
            }
            _ConnectDots(start, goal, is_horizontal);
        }

        void _ConnectDots(Coordinates start, Coordinates goal, bool is_horizontal)
        {
            int[] destinations = { (int)Tile.floor, (int)Tile.path };
            Coordinates p = start;
            int x = (start.X - goal.X < 0) ? (int)Direction.right : (int)Direction.left;
            int y = (start.Y - goal.Y < 0) ? (int)Direction.up : (int)Direction.down;
            var prior = (is_horizontal) ? Enumerable.Repeat(x, Math.Abs(start.X - goal.X)).ToList() : Enumerable.Repeat(y, Math.Abs(start.Y - goal.Y)).ToList();
            var posterior = (is_horizontal) ? Enumerable.Repeat(y, Math.Abs(start.Y - goal.Y)).ToList() : Enumerable.Repeat(x, Math.Abs(start.X - goal.X)).ToList();

            while (map[p.X, p.Y] != (int)Tile.hPath)
            {
                _Step(ref prior, ref p);
            }
            _Step(ref prior, ref p);

            int max = prior.Count + posterior.Count;

            for (int _ = 0; _ < max; _++)
            {
                if(prior.Count == 0)
                {
                    _Step(ref posterior, ref p);
                }
                else if(posterior.Count == 0)
                {
                    _Step(ref prior, ref p);
                }
                else
                {
                    if(UnityEngine.Random.Range(0, 100) < 50)
                    {
                        _Step(ref prior, ref p);
                    }
                    else
                    {
                        _Step(ref posterior, ref p);
                    }
                }

                if(Quarters(p.X, p.Y, (int)Tile.floor) || Quarters(p.X, p.Y, (int)Tile.path))
                {
                    break;
                }
            }
        }

        void _Step(ref List<int> list, ref Coordinates p)
        {
            int dir = list.Pop();
            switch (dir)
            {
                case (int)Direction.right:
                    p.X += 1;
                    break;
                case (int)Direction.left:
                    p.X -= 1;
                    break;
                case (int)Direction.up:
                    p.Y += 1;
                    break;
                case (int)Direction.down:
                    p.Y -= 1;
                    break;
            }

            if (map[p.X, p.Y] == (int)Tile.wall || map[p.X, p.Y] == (int)Tile.border)
            {
                map[p.X, p.Y] = (int)Tile.hPath;
            }
        }

        bool Edge(int i, int j)
        {
            return (i == 0 || i == width - 1 || j == 0 || j == height - 1) ? true : false;
        }

        bool Quarters(int i, int j, int target)
        {
            return (map[i - 1, j] == target || map[i + 1, j] == target || map[i, j - 1] == target || map[i, j + 1] == target) ? true : false;
        }

        void _Door()
        {
            int[] disused = new int[] { (int)Tile.end, (int)Tile.border, (int)Tile.division };
            Coordinates door = new Coordinates(0, 0);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (map[i, j] == (int)Tile.path && Quarters(i, j, (int)Tile.floor) && !Edge(i, j))
                    {
                        map[i, j] = (int)Tile.door;
                        door.X = i;
                        door.Y = j;
                        nDoor += 1;
                        doors[nDoor] = door;
                    }
                    else if (map[i, j] == (int)Tile.hPath && Quarters(i, j, (int)Tile.hFloor) && !Edge(i, j))
                    {
                        map[i, j] = (int)Tile.hDoor;
                        door.X = i;
                        door.Y = j;
                        nHDoor += 1;
                        hDoors[nHDoor] = door;
                    }
                    else if (disused.Contains(map[i, j]))
                    {
                        map[i, j] = (int)Tile.wall;
                    }
                }
            }
        }

        public GameObject none;
        public GameObject wall;
        public GameObject floor;
        public GameObject path;
        public GameObject door;
        public GameObject hFloor;
        public GameObject hPath;
        public GameObject hDoor;

        void Generate()
        {
            var p = new Vector3(0, 0, 0);
            var q = Quaternion.identity;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p.x = x;
                    p.y = y;
                    switch (map[x, y])
                    {
                        case (int)Tile.wall:
                            Instantiate(wall, p, q);
                            break;
                        case (int)Tile.floor:
                            Instantiate(floor, p, q);
                            break;
                        case (int)Tile.path:
                            Instantiate(path, p, q);
                            break;
                        case (int)Tile.door:
                            Instantiate(door, p, q);
                            break;
                        case (int)Tile.hFloor:
                            Instantiate(hFloor, p, q);
                            break;
                        case (int)Tile.hPath:
                            Instantiate(hPath, p, q);
                            break;
                        case (int)Tile.hDoor:
                            Instantiate(hDoor, p, q);
                            break;
                        default:
                            Instantiate(none, p, q);
                            break;
                    }
                }
            }
        }
    }
}