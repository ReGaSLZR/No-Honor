using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class LobbyPlayerListView : APlayerListView
    {

        #region Inspector Fields

        [Header("Lobby Player Elements")]

        [SerializeField]
        private TextMeshProUGUI textMatchCode;

        [Header("Footer Views")]

        [SerializeField]
        private GameObject footerViewNonHost;

        [SerializeField]
        private GameObject footerViewHost;

        [Header("Buttons")]

        [SerializeField]
        private Button buttonStartGame;

        #endregion //Inspector Fields

        #region Public API

        public void SetMatchCode(string matchCode) => textMatchCode.text = matchCode;

        public void RegisterOnStartGame(Action action) =>
            buttonStartGame.onClick.AddListener(action.Invoke);

        public void SetViewIsHost(bool isHost)
        {
            footerViewHost.SetActive(isHost);
            footerViewNonHost.SetActive(!isHost);
        }

        #endregion //Public API

    }

}