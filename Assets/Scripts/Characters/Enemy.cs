using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Enemy : Character
    {
        public Dictionary<Coordinates, GameObject> obstacles;
        public List<Coordinates> path;
        public GameObject targetObject;

        public int targetDoorNumber;
        public List<int> doorNumbers;

        public int patient;
        public bool collisionDetected;
        public GameObject collisionObject;
        public bool chase;
        public bool swap;

        public Enemy()
        {
            obstacles = new Dictionary<Coordinates, GameObject>();
            path = new List<Coordinates>();
            targetObject = null;
            patient = 0;
            collisionDetected = false;
            chase = false;
            swap = false;
        }
    }
}
