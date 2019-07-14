using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace Rougelike
{
    public class MasterData : MonoBehaviour
    {
        // for Dungeon.cs
        public static GameObject[] tiles { get; private set; }
        public static GameObject mapInstance;

        // for Spawn.cs
        public static GameObject playerObject;
        public static GameObject[] enemyList;
        public static GameObject scarecrowObject;
        public static GameObject[] items;
        public static Sprite[][] itemSprites;

        public static int[] itemTypeWeightTable { get; private set; }
        public static List<int[]> weightTables { get; private set; }

        // for Move.cs
        public static HashSet<int> nodeCandidates { get; private set; }

        // for CharacterControl.cs
        public static GameObject pointer;

        //for UI.cs
        public static GameObject canvas;
        public static GameObject scrollViewNode;
        public static GameObject command;

        private void Start()
        {
            _SetObjects();
            _SetEnemyList();
            _SetItems();
            _SetWeightTables();
            nodeCandidates = new HashSet<int>() { (int)Tile.floor, (int)Tile.path, (int)Tile.door, (int)Tile.hPath };
        }

        private void _SetObjects()
        {
            string objectName;
            int tileLength = Enum.GetValues(typeof(Tile)).Length - 2;
            tiles = new GameObject[tileLength];
            for (int n = 0; n < tileLength; n++)  // set itemList
            {
                objectName = Enum.GetName(typeof(Tile), (Tile)Enum.ToObject(typeof(Tile), n));
                tiles[n] = Resources.Load("Tiles/" + objectName) as GameObject;
            }
            mapInstance = Resources.Load("Tiles/mapInstance") as GameObject;

            playerObject = Resources.Load("Characters/player") as GameObject;
            scarecrowObject = Resources.Load("Characters/scarecrow") as GameObject;

            pointer = Resources.Load("Objects/pointer") as GameObject;

            canvas = GameObject.FindGameObjectWithTag("Canvas");
            scrollViewNode = Resources.Load("Items/scrollViewNode") as GameObject;
        }

        private void _SetEnemyList()
        {
            string enemyName = "";
            int enemyLength = Enum.GetValues(typeof(Enemies)).Length;
            enemyList = new GameObject[enemyLength];
            for (int n = 0; n < enemyLength; n++)  // set itemList
            {
                enemyName = Enum.GetName(typeof(Enemies), (Enemies)Enum.ToObject(typeof(Enemies), n));
                var enemy = Resources.Load("Characters/Enemies/" + enemyName) as GameObject;
                enemyList[n] = enemy;
            }
        }

        private void _SetItems()
        {
            int itemLength = Enum.GetValues(typeof(ItemType)).Length;
            items = new GameObject[itemLength];
            itemSprites = new Sprite[itemLength][];
            for (int m = 0; m < itemLength; m++)
            {
                switch (m)
                {
                    case (int)ItemType.weapon:
                        items[m] = Resources.Load("Items/Weapon/weapon") as GameObject;
                        _SetItemSprite<Weapon>(m);
                        break;
                    case (int)ItemType.armor:
                        items[m] = Resources.Load("Items/Armor/armor") as GameObject;
                        _SetItemSprite<Armor>(m);
                        break;
                    case (int)ItemType.miscellaneous:
                        items[m] = Resources.Load("Items/Miscellaneous/miscellaneous") as GameObject;
                        _SetItemSprite<Miscellaneous>(m);
                        break;
                    case (int)ItemType.accessory:
                        items[m] = Resources.Load("Items/Accessory/accessory") as GameObject;
                        _SetItemSprite<Accessory>(m);
                        break;
                }
            }
        }

        private void _SetItemSprite<Type>(int m)
        {
            string itemName;
            int n_item = Enum.GetValues(typeof(Type)).Length;
            itemSprites[m] = new Sprite[n_item];
            for (int n = 0; n < n_item; n++)
            {
                itemName = Enum.GetName(typeof(Type), (Type)Enum.ToObject(typeof(Type), n));
                var item = Resources.Load<Sprite>("Items/"+ typeof(Type).Name+ "/" + itemName);
                itemSprites[m][n] = item;
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