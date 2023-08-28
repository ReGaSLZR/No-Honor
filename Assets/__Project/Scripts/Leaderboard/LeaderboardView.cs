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

        #region Public API

        public void SetIsLocalWinner(bool isWinner)
        {
            childOnGameLost.SetActive(!isWinner);
            childOnGameWon.SetActive(isWinner);
        }

        #endregion //Public API

    }

}