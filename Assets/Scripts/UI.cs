using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Rougelike
{
    public class UI : MonoBehaviour
    {
        public static GameObject itemScroller;
        public static GameObject itemWindow;
        
        void Start()
        {

        }

        public static bool isUI(List<RaycastResult> raycastResult, ref GameObject targetUI)
        {
            foreach (var result in raycastResult)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    targetUI = result.gameObject;
                    return true;
                }
            }
            return false;
        }
    }
}
