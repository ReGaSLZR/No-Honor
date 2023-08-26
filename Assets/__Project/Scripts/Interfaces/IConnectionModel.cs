using System.Collections.Generic;
using UniRx;
using UnityEngine.SceneManagement;

namespace ReGaSLZR
{

    public interface IConnectionModel
    {

        public interface ISetter
        {
            void Disconnect();
            void CreateAndJoinNewMatch(string playerName, string matchCode);
            void JoinExistingMatch(string playerName, string matchCode);
            void SynchLoadScene(string scene);
            void SetRoomProperty(string key, object value);
        }

        public interface IGetter
        {
            IReadOnlyReactiveProperty<bool> IsConnected();
            IReadOnlyReactiveProperty<MatchModel> GetMatchModel();
            IReadOnlyReactiveProperty<bool> IsHost();
            IReadOnlyReactiveProperty<string> GetMatchCode();
            IReadOnlyReactiveProperty<string> GetConnectionIssue();
            IReadOnlyReactiveProperty<List<PlayerModel>> GetPlayersInMatch();
            
        }
       
    }

}