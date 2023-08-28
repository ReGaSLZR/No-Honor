using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using UnityEngine.SceneManagement;

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
        private ObjectPooler itemPool;

        [Space]

        [SerializeField]
        [Tooltip("The delay is to wait for the Start() in the Character to be done first.")]
        [Range(0, 10)]
        private uint frameDelayRegister = 1;

        [SerializeField]
        [Range(10, 30)]
        private uint itemSpawnsClearInterval;

        [SerializeField]
        [Range(0, 10)]
        private uint itemSpawnInterval;

        [SerializeField]
        [Range(1, 10)]
        private uint itemSpawnCount;

        #endregion //Inspector Fields

        [Inject]
        private readonly SceneModel sceneModel;

        private readonly List<Character> characters = new List<Character>();
        private Character localPlayer;

        private WaitForSeconds itemSpawnDelay;
        private WaitForSeconds itemClearDelay;

        #region Unity Callbacks

        private void Awake()
        {
            itemSpawnDelay = new WaitForSeconds(itemSpawnInterval);
            itemClearDelay = new WaitForSeconds(itemSpawnsClearInterval);

            viewLeaderboard.RegisterOnExit(OnExitGame);
            viewHud.RegisterOnExit(OnExitGame);

            viewLoading.SetActive(false);
            viewLeaderboard.SetIsDisplayed(false);
        }

        protected virtual void Start()
        {
            StartCoroutine(C_SpawnItems());
            StartCoroutine(C_ClearItems());
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private IEnumerator C_SpawnItems()
        { 
            while(characters.Count > 0)
            {
                var itemCount = 0;
                while (itemCount < itemSpawnCount)
                {
                    itemPool.GetRandomObjectToRandomPosition();
                    itemCount++;
                }

                yield return itemSpawnDelay;
            }
        }

        private IEnumerator C_ClearItems()
        {
            while (characters.Count > 0)
            {
                yield return itemClearDelay;
                itemPool.HideAllObjectsInPool();
            }
        }

        private void OnExitGame()
        {
            if (localPlayer != null)
            {
                localPlayer.Stats.RecordSurviveTime();
            }

            viewLoading.SetActive(true);
            viewLeaderboard.SetIsDisplayed(false);

            DoExitGame();
        }

        private IEnumerator C_DelayedSetUpOfCharacter(Character character, bool isBot)
        {
            uint delay = 0;
            while (delay < frameDelayRegister)
            {
                yield return null;
                delay++;
            }

            if (localPlayer == null && !isBot)
            {
                localPlayer = character;
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

            characters.Clear();
        }

        #endregion //Client Impl

        #region Public API

        public void RegisterCharacter(Character character, bool isBot)
        {
            characters.Add(character);
            StartCoroutine(C_DelayedSetUpOfCharacter(character, isBot));
        }

        #endregion //Public API

        #region Protected Virtuals

        protected virtual void DoExitGame() =>
            SceneManager.LoadScene(sceneModel.SceneLobby);

        #endregion //Protected Virtuals

    }

}