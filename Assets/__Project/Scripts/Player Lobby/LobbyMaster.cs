using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class LobbyMaster : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private LobbyStarterView viewStarter;

        [SerializeField]
        private LobbyPlayerListView viewPlayerList;

        [SerializeField]
        private GameObject viewLoading;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            viewPlayerList.SetIsDisplayed(false);
            viewLoading.gameObject.SetActive(false);
            viewStarter.SetIsDisplayed(true);   
        }

        #endregion //Unity Callbacks

    }

}