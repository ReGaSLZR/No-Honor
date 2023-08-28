using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public abstract class APlayerListView : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        protected Canvas canvasRoot;

        [SerializeField]
        protected Transform playerListParent;

        [SerializeField]
        protected Button buttonExit;

        [Space]

        [SerializeField]
        protected PlayerListItemView prefabListItem;

        #endregion //Inspector Fields

        #region Public API

        public virtual void RefreshList(List<PlayerModel> players)
        {
            playerListParent.DestroyAllChildren();

            foreach (var player in players)
            {
                var newItem = Instantiate(prefabListItem, playerListParent);
                newItem.SetUp(player);
            }
        }

        public void RegisterOnExit(Action action) =>
            buttonExit.onClick.AddListener(action.Invoke);

        public void SetIsDisplayed(bool isDisplayed) => canvasRoot.gameObject.SetActive(isDisplayed);

        #endregion //Public API

    }

}