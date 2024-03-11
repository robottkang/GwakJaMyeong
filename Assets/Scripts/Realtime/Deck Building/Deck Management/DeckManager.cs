using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using TMPro;
using Card;

namespace DeckBuilding
{
    public class DeckManager : MonoBehaviour
    {
        [Header("- References")]
        [SerializeField]
        private TMP_InputField userSearchField;
        [Header("- Settings")]
        [SerializeField]
        private string serverURL = "http://localhost:3000";

        private void Start()
        {
            SetActiveBoard(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                SetActiveBoard(false);
            }
        }

        public void SetActiveBoard(bool value)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(value);
            }
        }

        public void GetDeckList() => UniTask.Void(async () =>
        {
            if (!CheckSpecialCharacters(userSearchField.text))
            {
                Debug.Log("Invalid name");
                userSearchField.text = string.Empty;
                return;
            }
            try
            {
                var encodedUrl = System.Web.HttpUtility.UrlEncode(serverURL + "/" + userSearchField.text + "/deck");
                using UnityWebRequest www = UnityWebRequest.Get(encodedUrl);
                await www.SendWebRequest();

                //JsonUtility.FromJson<CardData[]>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
            }
            catch (System.Exception err)
            {
                Debug.Log(err);
            }
        });

        /// <summary>
        /// if name incluedes special characters, return false
        /// </summary>
        /// <returns></returns>
        public bool CheckSpecialCharacters(string input)
        {
            foreach (char character in input)
            {
                if (!char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
