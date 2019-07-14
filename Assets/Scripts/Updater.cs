using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Updater : MonoBehaviour
    {
        private static Color skeleton;
        public static bool update;

        void Start()
        {
            skeleton = new Color(1.0f, 1.0f, 1.0f, 100f / 255f);
            update = true;
        }

        void Update()
        {
            if (update)
            {
                var p = Spawn.pCache.p;
                if (Dungeon.map[p.X, p.Y] == (int)Tile.hPath)
                {
                    Dungeon.map[p.X, p.Y] = (int)Tile.path;
                    if (Dungeon.map[p.X + 1, p.Y] == (int)Tile.hPath) { Dungeon.map[p.X + 1, p.Y] = (int)Tile.path; }
                    if (Dungeon.map[p.X - 1, p.Y] == (int)Tile.hPath) { Dungeon.map[p.X - 1, p.Y] = (int)Tile.path; }
                    if (Dungeon.map[p.X, p.Y + 1] == (int)Tile.hPath) { Dungeon.map[p.X, p.Y + 1] = (int)Tile.path; }
                    if (Dungeon.map[p.X, p.Y - 1] == (int)Tile.hPath) { Dungeon.map[p.X, p.Y - 1] = (int)Tile.path; }
                }
                var sight = ObstacleSetter.Sight(p);
                Dungeon._UpdateSight(sight);
                Dungeon._Illuminate();

                if (Spawn.items.ContainsKey(p))
                {
                    string[] itemName = Spawn.items[p][0].name.Split('/');
                    UI.dropWindowSprite.color = (Spawn.items[p].Count == 1) ? skeleton : Color.red;
                    UI.dropImageSprite.sprite = MasterData.itemSprites[int.Parse(itemName[0])][int.Parse(itemName[1])];
                    UI.dropImageSprite.color = Color.white;
                }
                else
                {
                    UI.dropWindowSprite.color = skeleton;
                    var clear = new Color(1.0f, 1.0f, 1.0f, 0);
                    UI.dropImageSprite.color = clear;
                }

                update = false;
            }
        }
    }
}
