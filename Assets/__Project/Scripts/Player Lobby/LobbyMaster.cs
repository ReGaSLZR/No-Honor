using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace ReGaSLZR
{

    public class LobbyMaster : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private LobbyStarterView viewStarter;

        [SerializeField]
        private LobbyPlayerListView viewPlayerList;

        [Space]

        [SerializeField]
        private PopUpView popUpNetworkIssue;

        [SerializeField]
        private GameObject viewLoading;

        #endregion //Inspector Fields

        #region Private Fields

        [Inject] private readonly IConnectionModel.IGetter connectionGetter;
        [Inject] private readonly IConnectionModel.ISetter connectionSetter;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Awake()
        {
            viewStarter.PopUpViewCreate.RegisterOnFinishForm(OnFinishForm);
            viewStarter.PopUpViewJoin.RegisterOnFinishForm(OnFinishForm);

            viewPlayerList.RegisterOnExit(AttemptExitMatch);
        }

        private void Start()
        {
            connectionGetter.GetMatchCode()
                .Where(code => string.IsNullOrEmpty(code))
                .Where(_ => !connectionGetter.IsConnected().Value)
                .Where(_ => connectionGetter.GetPlayersInMatch().Value.Count == 0)
                .Subscribe(_ => InitView())
                .AddTo(this);

            connectionGetter.GetConnectionIssue()
                .Where(issue => !string.IsNullOrEmpty(issue))
                .Subscribe(RestartProgressAndShowIssue)
                .AddTo(this);

            connectionGetter.IsConnected()
                .Subscribe(isConnected => Debug.Log($"Is Connected: {isConnected}"))
                .AddTo(this);

            connectionGetter.GetPlayersInMatch().AsObservable()
                .Where(_ => connectionGetter.IsConnected().Value)
                .Where(players => players.Count > 0)
                .Subscribe(DisplayLobbyList)
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void InitView()
        {
            viewPlayerList.SetIsDisplayed(false);
            viewLoading.gameObject.SetActive(false);
            viewStarter.SetIsDisplayed(true);
        }

        private void RestartProgressAndShowIssue(string issue)
        {
            InitView();
            popUpNetworkIssue.DisplayWithText(issue);
        }

        private void AttemptExitMatch()
        {
            viewStarter.SetIsDisplayed(true);
            viewPlayerList.SetIsDisplayed(false);

            connectionSetter.Disconnect();
            viewLoading.SetActive(true);
        }

        private void DisplayLobbyList(List<PlayerModel> players)
        {
            Debug.LogWarning($"{GetType().Name} updating list on view");

            viewStarter.SetIsDisplayed(false);
            viewLoading.gameObject.SetActive(true);

            viewPlayerList.ClearList();
            viewPlayerList.PopulateList(players);

            viewPlayerList.SetMatchCode(connectionGetter.GetMatchCode().Value);
            viewPlayerList.SetViewIsHost(connectionGetter.IsHost().Value);

            viewPlayerList.SetIsDisplayed(true);
            viewLoading.gameObject.SetActive(false);
        }

        private void OnFinishForm(string playerName, string matchCode)
        {
            viewLoading.SetActive(true);
            viewStarter.SetIsDisplayed(false);
            viewPlayerList.SetIsDisplayed(false);

            if (viewStarter.PopUpViewCreate.IsDisplayed())
            {
                Debug.LogWarning($"{GetType().Name} Creating New Match...");
                connectionSetter.CreateAndJoinNewMatch(playerName, matchCode);
            }
            else
            {
                Debug.LogWarning($"{GetType().Name} Joining Existing Match...");
                connectionSetter.JoinExistingMatch(playerName, matchCode);
            }
        }

        #endregion //Client Impl

    }

}