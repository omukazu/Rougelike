using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Bat : Enemy
    {
        public Bat()
        {
            hp = 10;
            attack = 2;
            defense = 1;
            speed = 1.5f;
        }
    }
}
