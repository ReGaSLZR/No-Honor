using System.Collections.Generic;
using UnityEngine;

namespace ReGaSLZR
{

    public class LeaderboardView : APlayerListView
    {

        #region Inspector Fields

        [Header("Leaderboard Player Elements")]

        [SerializeField]
        private GameObject childOnGameLost;

        [SerializeField]
        private GameObject childOnGameWon;

        #endregion //Inspector Fields

        public Character winner;

        #region Public API

        public override void RefreshList(List<PlayerModel> players)
        {
            base.RefreshList(players);
        }

        public void SetIsLocalWinner(bool isWinner)
        {
            childOnGameLost.SetActive(!isWinner);
            childOnGameWon.SetActive(isWinner);
        }

        #endregion //Public API

    }

}