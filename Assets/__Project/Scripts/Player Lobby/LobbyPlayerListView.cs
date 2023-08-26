using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class LobbyPlayerListView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Canvas canvasRoot;

        [SerializeField]
        private TextMeshProUGUI textMatchCode;

        [SerializeField]
        private Transform playerListParent;

        [Header("Footer Views")]

        [SerializeField]
        private GameObject footerViewNonHost;

        [SerializeField]
        private GameObject footerViewHost;

        [Header("Buttons")]

        [SerializeField]
        private Button buttonStartGame;

        [SerializeField]
        private Button buttonExit;

        [Space]

        [SerializeField]
        private LobbyListItemPlayerName prefabListItem;

        #endregion //Inspector Fields

        #region Public API

        public void SetMatchCode(string matchCode) => textMatchCode.text = matchCode;

        public void ClearList()
        {
            while (playerListParent.childCount > 0)
            {
                DestroyImmediate(playerListParent.GetChild(0).gameObject);
            }
        }

        public void PopulateList(List<string> playerNames, 
            int indexLocalPlayer, int indexHost)
        {
            for (var x=0; x<playerNames.Count; x++)
            {
                var newItem = Instantiate(prefabListItem, playerListParent);
                newItem.SetUp(playerNames[x], x == indexLocalPlayer, x==indexHost);
            }
        }

        public void RegisterOnStartGame(Action action) =>
            buttonStartGame.onClick.AddListener(action.Invoke);

        public void RegisterOnExit(Action action) =>
            buttonExit.onClick.AddListener(action.Invoke);

        public void SetIsDisplayed(bool isDisplayed) => canvasRoot.gameObject.SetActive(isDisplayed);

        public void SetViewIsHost(bool isHost)
        {
            footerViewHost.SetActive(isHost);
            footerViewNonHost.SetActive(!isHost);
        }

        #endregion //Public API

    }

}