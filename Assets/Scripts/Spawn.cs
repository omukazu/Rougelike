using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace Rougelike
{
    public class Spawn : MonoBehaviour
    {
        public static GameObject player;
        public static Player pCache;
        private static int maxEnemies;
        public static List<GameObject> enemies;
        public static List<Enemy> eCaches;
        public static List<GameObject> scarecrows;
        public static Dictionary<Coordinates, GameObject> characters;

        private static int maxItems;
        public static Dictionary<Coordinates, List<GameObject>> items;

        void Start()
        {
            _Initialize();
            _SpawnPlayerAndEnemies();
            _SpawnItems();
        }

        void Update()
        {

        }

        public void _Initialize()
        {
            maxEnemies = 4;
            maxItems = 36;

            enemies = new List<GameObject>();
            eCaches = new List<Enemy>();
            scarecrows = new List<GameObject>();
            characters = new Dictionary<Coordinates, GameObject>();
            items = new Dictionary<Coordinates, List<GameObject>>();
        }

        void _SpawnPlayerAndEnemies()
        {
            int index = 0;
            var roomIndices = Dungeon.roomIndices.OrderBy(x => Guid.NewGuid()).ToList();
            var p = new Coordinates(0, 0);  //manage gamaobjects by Coordinates
            var position = new Vector3(0, 0, 0);
            var q = Quaternion.identity;
            for (int n = 0; n < 1; n++)
            {
                p.X = Dungeon.rooms[roomIndices[n].X, roomIndices[n].Y][(int)Index.xLeft] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndices[n].X, roomIndices[n].Y][(int)Index.width]);
                p.Y = Dungeon.rooms[roomIndices[n].X, roomIndices[n].Y][(int)Index.yBottom] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndices[n].X, roomIndices[n].Y][(int)Index.height]);
                position.x = p.X;
                position.y = p.Y;
                if (n == 0)
                {
                    player = Instantiate(MasterData.playerObject, position, q) as GameObject;
                    pCache = player.GetComponent<Player>();
                    pCache.p = p;
                    player.name = "player";
                    characters[p] = player;
                }
                else
                {
                    index = UnityEngine.Random.Range(0, MasterData.enemyList.Length);
                    var enemy = Instantiate(MasterData.enemyList[index], position, q) as GameObject;
                    var eCache = enemy.GetComponent<Enemy>();
                    eCaches.Add(eCache);
                    eCache.p = p;
                    enemy.name = "enemy/" + index.ToString() + "/" + n.ToString();
                    characters[p] = enemy;
                    enemies.Add(enemy);
                }
            }
        }

        void _SpawnEnemy()
        {
            int count = 0;
            var roomIndex = new Coordinates(0, 0);
            var p = new Coordinates(0, 0);
            do
            {
                roomIndex = Dungeon.roomIndices[UnityEngine.Random.Range(0, Dungeon.roomIndices.Count)];
                p.X = Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.xLeft] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.width]);
                p.Y = Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.yBottom] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.height]);
                count += 1;
            } while (characters.ContainsKey(p) && count < 9);

            if (enemies.Count < maxEnemies && count < 10)
            {
                int index = UnityEngine.Random.Range(0, MasterData.enemyList.Length);
                var position = new Vector3(p.X, p.Y, 0);
                var enemy = Instantiate(MasterData.enemyList[index], position, Quaternion.identity);
                var eCache = enemy.GetComponent<Enemy>();
                eCaches.Add(eCache);
                eCache.p = p;
                enemy.name = "enemy/" + index.ToString() + "/Spawn";
                characters[p] = enemy;
                enemies.Add(enemy);
            }
        }

        void _SpawnScarecrow(Coordinates p)
        {
            var position = new Vector3(p.X, p.Y, 0);
            var scarecrow = Instantiate(MasterData.scarecrowObject, position, Quaternion.identity);
            scarecrow.GetComponent<Character>().p = p;
            scarecrow.name = "scarecrow/Spawn";
            characters[p] = scarecrow;
            scarecrows.Add(scarecrow);
        }

        void _SpawnItems()
        {
            int type, id;
            var roomIndex = new Coordinates(0, 0);
            var p = new Coordinates(0, 0);
            var position = new Vector3(0, 0, 0);
            var q = Quaternion.identity;

            for (int n = 0; n < maxItems; n++)
            {
                roomIndex = Dungeon.roomIndices[UnityEngine.Random.Range(0, Dungeon.roomIndices.Count)];
                roomIndex = new Coordinates(2,2);
                p.X = Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.xLeft] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.width]);
                p.Y = Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.yBottom] + UnityEngine.Random.Range(0, Dungeon.rooms[roomIndex.X, roomIndex.Y][(int)Index.height]);
                type = MasterData.itemTypeWeightTable.WeightedRandomSelect();
                id = MasterData.weightTables[type].WeightedRandomSelect();
                position.x = p.X;
                position.y = p.Y;
                var item = Instantiate(MasterData.items[type], position, q);
                item.name = type + "/" + id;
                if (!items.ContainsKey(p))
                {
                    items[p] = new List<GameObject>();
                }    
                items[p].Add(item);
            }
        }
    }
}
