using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class LobbyStartPopUpView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private GameObject viewParent;

        [Header("Starter Sub views")]

        [SerializeField]
        private Button buttonOk;

        [SerializeField]
        private Button buttonExit;

        [Header("Input Fields")]

        [SerializeField]
        private TMP_InputField inputPlayerName;

        [SerializeField]
        private TMP_InputField inputMatchCode;

        #endregion //Inspector Fields

        #region Accessors

        public string InputPlayerName => inputPlayerName.text;
        public string InputMatchCode => inputMatchCode.text;

        #endregion //Accessors

        #region Unity Callbacks

        private void Awake()
        {
            buttonExit.onClick.AddListener(() => SetIsDisplayed(false));
            inputPlayerName.onValueChanged.AddListener(_ => AssessOkayButton());
            inputMatchCode.onValueChanged.AddListener(_ => AssessOkayButton());
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void AssessOkayButton() => buttonOk.interactable = 
            !string.IsNullOrEmpty(InputPlayerName) && !string.IsNullOrEmpty(InputMatchCode);

        #endregion //Client Impl

        #region Public API

        public void RegisterOnClose(Action action)
            => buttonExit.onClick.AddListener(action.Invoke);

        /// <summary>
        /// Register Action<playerName, matchCode> when popUp form has been filled in.
        /// </summary>
        public void RegisterOnFinishForm(Action<string, string> action)
            => buttonOk.onClick.AddListener(() => action.Invoke(InputPlayerName, InputMatchCode));

        public void SetIsDisplayed(bool isDisplayed)
        {
            AssessOkayButton();
            viewParent.SetActive(isDisplayed);
        }

        public bool IsDisplayed() => viewParent.activeInHierarchy || viewParent.activeSelf;

        #endregion //Public API

    }

}