using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike {
    public class Character : MonoBehaviour
    {
        public Coordinates p;

        public int hp;
        public int attack;
        public int defense;
        public float speed;

        public void _Dead()
        {
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
