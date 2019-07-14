using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class CameraControl : MonoBehaviour
    {
        Vector3 position;

        void Start()
        {
            position = new Vector3(Spawn.player.transform.position.x, Spawn.player.transform.position.y, -10);
            transform.position = position;
        }

        void LateUpdate()
        {
            position.x = Spawn.player.transform.position.x;
            position.y = Spawn.player.transform.position.y;
            transform.position = position;
        }
    }
}