using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace Rougelike
{
    public class MasterData : MonoBehaviour
    {
        public static GameObject[] tiles { get; private set; }

        public static GameObject playerObject;
        public static GameObject[] enemyList;
        public static GameObject scarecrowObject;
        public static GameObject[][] itemList;

        public static int[] itemTypeWeightTable { get; private set; }
        public static List<int[]> weightTables { get; private set; }

        public static HashSet<int> nodeCandidates { get; private set; }

    private void Start()
        {
            _SetObjects();
            _SetEnemyList();
            _SetItemList();
            _SetWeightTables();
            nodeCandidates = new HashSet<int>() { (int)Tile.floor, (int)Tile.path, (int)Tile.door };
        }

        private void _SetObjects()
        {
            int tileLength = Enum.GetValues(typeof(Tile)).Length - 2;
            tiles = new GameObject[tileLength];
            for (int n = 0; n < tileLength; n++)  // set itemList
            {
                name = Enum.GetName(typeof(Tile), (Tile)Enum.ToObject(typeof(Tile), n));
                tiles[n] = Resources.Load("Tiles/" + name.ToString()) as GameObject;
            }

            playerObject = Resources.Load("Characters/player") as GameObject;
            scarecrowObject = Resources.Load("Characters/scarecrow") as GameObject;
        }

        private void _SetEnemyList()
        {
            string name = "";
            int enemyLength = Enum.GetValues(typeof(Enemies)).Length;
            enemyList = new GameObject[enemyLength];
            for (int n = 0; n < enemyLength; n++)  // set itemList
            {
                name = Enum.GetName(typeof(Enemies), (Enemies)Enum.ToObject(typeof(Enemies), n));
                var enemy = Resources.Load("Characters/Enemies/" + name.ToString()) as GameObject;
                enemyList[n] = enemy;
            }
        }

        private void _SetItemList()
        {
            int itemLength = Enum.GetValues(typeof(ItemType)).Length;
            itemList = new GameObject[itemLength][];
            for (int m = 0; m < itemLength; m++)
            {
                switch (m)
                {
                    case (int)ItemType.weapon:
                        __SetItemList<Weapon>(m);
                        break;
                    case (int)ItemType.armor:
                        __SetItemList<Armor>(m);
                        break;
                    case (int)ItemType.miscellaneous:
                        __SetItemList<Miscellaneous>(m);
                        break;
                    case (int)ItemType.accessory:
                        __SetItemList<Accessory>(m);
                        break;
                }
            }
        }

        private void __SetItemList<Type>(int m)
        {
            int n_item = Enum.GetValues(typeof(Type)).Length;
            itemList[m] = new GameObject[n_item];
            for (int n = 0; n < n_item; n++)
            {
                name = Enum.GetName(typeof(Type), (Type)Enum.ToObject(typeof(Type), n));
                var item = Resources.Load("Items/"+ typeof(Type).Name+ "/" + name.ToString()) as GameObject;
                itemList[m][n] = item;
            }
        }

        private void _SetWeightTables()
        {
            var cr = new CsvReader();
            var data = cr.ReadFile("Items/WeightTable");

            itemTypeWeightTable = data[1].Map(e => int.Parse(e)).ToArray();
            weightTables = new List<int[]>();
            weightTables.Add(data[(int)ItemType.weapon * 2 + 3].Map(e => int.Parse(e)).ToArray());
            weightTables.Add(data[(int)ItemType.armor * 2 + 3].Map(e => int.Parse(e)).ToArray());
            weightTables.Add(data[(int)ItemType.miscellaneous * 2 + 3].Map(e => int.Parse(e)).ToArray());
            weightTables.Add(data[(int)ItemType.accessory * 2 + 3].Map(e => int.Parse(e)).ToArray());
        }


    }
}