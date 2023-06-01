using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Room
{
    public class PageChanger : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void ChangeNextPage()
        {
            GameManager gameManager = GameManager.gameManager;

            switch (gameManager.CurrentPage)
            {
                case Page.WaitPlayer:
                    gameManager.CurrentPage = Page.Drow;
                    break;
                case Page.Drow:
                    gameManager.CurrentPage = Page.StrategyPlan;
                    break;
                case Page.StrategyPlan:
                    gameManager.CurrentPage = Page.Duel;
                    break;
                case Page.Duel:
                    gameManager.CurrentPage = Page.End;
                    break;
                case Page.End:
                    gameManager.CurrentPage = Page.Drow;
                    break;
            }

            text.text = gameManager.CurrentPage.ToString();
        }
    }
}
