using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NLTechTest.Camera
{
    public class MouseClickController : MonoBehaviour
    {
        [SerializeField]
        PanelControlDisplay panelControlDisplay;

        UnityEngine.Camera mainCam;

        void Start()
        {
            mainCam = UnityEngine.Camera.main;
        }

        public void OnMouseClick()
        {
            IClickable item = GetClickableItem();

            if (item != null) {
                ActivateClickableItem(item);
                panelControlDisplay.DisplayForNSecond(3f);
            }
                
        }

        private IClickable GetClickableItem()
        {

            RaycastHit hit;
            Ray ray;
            IClickable item;
            //mainCam.ScreenToWorldPoint(Mouse.current.position);

            ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                item = hit.transform.GetComponent<IClickable>();
                if (item != null)
                    return (item);
                return (null);
            }

            return null;
        }

        private void ActivateClickableItem(IClickable item)
        {
            if (item != null)
                item.OnClick();
        }
    }
}