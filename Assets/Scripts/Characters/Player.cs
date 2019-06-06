using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Player : Character
    {
        public Player()
        {
            hp = 10;
            attack = 4;
            defense = 2;
            speed = 1.0f;
        }
    }
}
