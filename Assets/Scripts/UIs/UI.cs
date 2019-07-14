using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Rougelike
{
    public class UI : MonoBehaviour
    {
        public static GameObject background;
        public static GameObject scrollView;
        public static GameObject dropWindow;
        public static Image dropWindowSprite;
        public static GameObject dropImage;
        public static Image dropImageSprite;
        
        void Start()
        {
            background = MasterData.canvas.transform.Find("Background").gameObject;
            scrollView = MasterData.canvas.transform.Find("ScrollView").gameObject;
            dropWindow = GameObject.Find("DropWindow");
            dropWindowSprite = dropWindow.GetComponent<Image>();
            dropImage = GameObject.Find("DropImage");
            dropImageSprite = dropImage.GetComponent<Image>();
        }

        public static bool isUI(Vector3 mousePosition)
        {
            var ped = new PointerEventData(EventSystem.current) { position = mousePosition };
            var raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, raycastResult);

            foreach (var result in raycastResult)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return true;
                }
            }
            return false;
        }

        public void ClickBackground()
        {
            foreach (Transform c in MasterData.canvas.transform)
            {
                if (c.name != "DropWindow")
                {
                    c.gameObject.SetActive(false);
                }
            }
            background.SetActive(false);
        }

        public void ClickDrop()
        {
            var p = Spawn.pCache.p;
            if(Spawn.items.ContainsKey(p) && Spawn.items[p].Count >= 2 && !scrollView.activeInHierarchy)
            {
                background.SetActive(true);
                scrollView.SetActive(true);
                var content = scrollView.transform.Find("Content");
                // 残っているノードの削除
                foreach (Transform c in content.transform)
                {
                    Destroy(c.gameObject);
                }

                var position = new Vector3(0, 0, 0);
                var q = Quaternion.identity;
                string[] itemName = null;
                GameObject node = null;
                for (int n = 0; n < Spawn.items[p].Count; n++)
                {
                    node = Instantiate(MasterData.scrollViewNode, position, q);
                    node.name = n.ToString();
                    node.transform.SetParent(content.transform, false);

                    itemName = Spawn.items[p][n].name.Split('/');
                    node.GetComponent<Image>().sprite = MasterData.itemSprites[int.Parse(itemName[0])][int.Parse(itemName[1])];
                }
            }
        }

        public void ClickNode()
        {

        }

        public void DoubleClickDrop()
        {

        }
    }
}
