using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DisplayManager : SingletonMonoBehaviour<DisplayManager>
    {
        [SerializeField]
        private List<GameObject> infoPanels = null;

        private List<IInfoDisplay> infoDisplays = new();
        private bool isDisplaying = false;
        private IInfoDisplay displayingPanel;

        private void Start()
        {
            foreach (var infoPanel in infoPanels)
            {
                var infoDisplay = infoPanel.GetComponent<IInfoDisplay>();
                infoDisplays.Add(infoDisplay);
                infoDisplay.SetActiveDisplay(false);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isDisplaying)
                {
                    displayingPanel.SetActiveDisplay(false);
                    displayingPanel = null;
                    isDisplaying = false;
                }
                else
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = -100f;

                    // raycast UI Object
                    PointerEventData pointerData = new(EventSystem.current) { position = mousePosition };
                    List<RaycastResult> raycastResults = new();
                    EventSystem.current.RaycastAll(pointerData, raycastResults);

                    // raycast 3d Object
                    RaycastHit[] screenPointHits = Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePosition));

                    // contain all raycasted Object
                    List<GameObject> resultList = new();

                    resultList.AddRange(raycastResults.ConvertAll(raycastResult => raycastResult.gameObject));
                    resultList.AddRange(System.Array.ConvertAll(screenPointHits, hit => hit.collider.gameObject));

                    // check for objects to display on the UI
                    if (resultList.Count == 0) return;

                    for (int i = 0; i < resultList.Count && displayingPanel == null; i++)
                    {
                        foreach (IInfoDisplay infoDisplay in infoDisplays)
                        {
                            if (infoDisplay.TrySetDisplayData(resultList[i]))
                            {
                                displayingPanel = infoDisplay;
                                displayingPanel.SetActiveDisplay(true);
                                displayingPanel.DisplayInfo();
                                isDisplaying = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public interface IInfoDisplay
    {
        public bool TrySetDisplayData(GameObject target);
        public void DisplayInfo();
        public void SetActiveDisplay(bool value);
    }
}
