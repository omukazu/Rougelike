using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Player : Character
    {
        public Dictionary<Coordinates, GameObject> obstacles;
        public List<Coordinates> path;
        public GameObject targetObject;

        public Player()
        {
            obstacles = new Dictionary<Coordinates, GameObject>();
            path = new List<Coordinates>();
            targetObject = null;

            hp = 10;
            attack = 4;
            defense = 2;
            speed = 1.0f;
        }
    }
}
