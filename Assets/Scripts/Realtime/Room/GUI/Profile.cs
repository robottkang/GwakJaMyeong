using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Room.UI
{
    public class Profile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private FieldController fieldController;
        [SerializeField]
        private TextMeshProUGUI hp;

        private void Update()
        {
            hp.text = fieldController.Hp.ToString();
        }
    }
}
