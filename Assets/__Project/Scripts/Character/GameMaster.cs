using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ReGaSLZR
{

    public class GameMaster : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private PlayerHUD viewHud;

        [SerializeField]
        private LeaderboardView viewLeaderboard;

        [SerializeField]
        private GameObject viewLoading;

        [Space]

        [SerializeField]
        [Tooltip("The delay is to wait for the Start() in the Character to be done first.")]
        [Range(0, 10)]
        private uint frameDelayRegister = 1;

        #endregion //Inspector Fields

        private readonly List<Character> characters = new List<Character>();

        #region Unity Callbacks

        private void Start()
        {
            viewLoading.SetActive(false);
            viewLeaderboard.SetIsDisplayed(false);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private IEnumerator C_DelayedSetUpOfCharacter(Character character, bool isBot)
        {
            uint delay = 0;
            while (delay < frameDelayRegister)
            {
                yield return null;
                delay++;
            }

            character.SetUp(isBot, viewHud);
            character.Stats.IsPlayerDead()
                .Where(isDead => isDead)
                .Subscribe(_ => CheckGameStatus())
                .AddTo(this);
        }

        private void CheckGameStatus()
        {
            var survivors = 0;
            Character localPlayer = null;
            Character lastDetectedSurvivor = null;
            var models = new List<PlayerModel>();

            foreach(var chara in characters)
            {
                var model = chara.Stats.Model.Value;
                models.Add(model);

                if (model.health != PlayerModel.PLAYER_HEALTH_DEAD)
                {
                    survivors++;
                    lastDetectedSurvivor = chara;
                }

                if(model.isLocalPlayer)
                {
                    localPlayer = chara;
                }

                if (survivors > 1)
                {
                    return;
                }
            }

            if (survivors == 1)
            {
                lastDetectedSurvivor.Stats.MarkAsWinner();
                EndGame(localPlayer == lastDetectedSurvivor, models);
            }
        }

        private void EndGame(bool isLocalPlayerWinner, List<PlayerModel> playerModels)
        {
            viewLoading.SetActive(false);
            viewLeaderboard.SetIsLocalWinner(isLocalPlayerWinner);
            viewLeaderboard.RefreshList(playerModels);
            viewLeaderboard.SetIsDisplayed(true);

            DisableAllCharacters();
        }

        private void DisableAllCharacters()
        {
            foreach (var chara in characters)
            {
                chara.gameObject.SetActive(false);
            }
        }

        #endregion //Client Impl

        #region Public API

        public void RegisterCharacter(Character character, bool isBot)
        {
            characters.Add(character);
            StartCoroutine(C_DelayedSetUpOfCharacter(character, isBot));
        }

        #endregion //Public API

    }

}