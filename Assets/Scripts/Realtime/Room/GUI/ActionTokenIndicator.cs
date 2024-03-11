using UnityEngine;

namespace Room.UI
{
    public class ActionTokenIndicator : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerTurnDisplay;
        [SerializeField]
        private GameObject opponentTurnDisplay;

        private void Start()
        {
            playerTurnDisplay.SetActive(false);
            opponentTurnDisplay.SetActive(false);

            DuelManager.OnActionTokenChanged.AddListener(token =>
            {
                if (token == UserType.Player)
                {
                    playerTurnDisplay.SetActive(true);
                    opponentTurnDisplay.SetActive(false);
                }
                else
                {
                    playerTurnDisplay.SetActive(false);
                    opponentTurnDisplay.SetActive(true);
                }
            });
        }
    }
}
