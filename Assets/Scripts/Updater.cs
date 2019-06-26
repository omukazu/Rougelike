using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Updater : MonoBehaviour
    {
        public static bool update;

        void Start()
        {
            update = true;
        }

        void Update()
        {
            if (update)
            {
                var sight = ObstacleSetter.Sight(Spawn.pCache.p);
                Dungeon._UpdateSight(sight);
                Dungeon._Illuminate();
                update = false;
            }
        }
    }
}
