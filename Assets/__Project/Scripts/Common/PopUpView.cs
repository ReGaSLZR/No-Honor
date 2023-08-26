using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class PopUpView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private GameObject rootParent;

        [SerializeField]
        private TextMeshProUGUI textMessage;

        [SerializeField]
        private Button buttonExit;

        #endregion //Inspector Fields

        private Action onCloseListener;

        #region Unity Callbacks

        private void Awake()
            => buttonExit.onClick.AddListener(Close);

        #endregion //Unity Callbacks

        #region Public API

        public void Close()
        {
            rootParent.SetActive(false);
            onCloseListener?.Invoke();
        }

        public void DisplayWithText(string text, Action onCloseListener = null)
        {
            textMessage.text = text;
            rootParent.SetActive(true);

            if (onCloseListener != null)
            {
                this.onCloseListener = onCloseListener;
            }
        }

        #endregion //Public API

    }

}