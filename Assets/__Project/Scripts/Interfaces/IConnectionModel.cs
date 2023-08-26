using System.Collections.Generic;
using UniRx;

namespace ReGaSLZR
{

    public interface IConnectionModel
    {

        public interface ISetter
        {
            void Disconnect();
            void CreateAndJoinNewMatch(string playerName, string matchCode);
            void JoinExistingMatch(string playerName, string matchCode);
        }

        public interface IGetter
        {
            IReadOnlyReactiveProperty<bool> IsConnected();
            IReadOnlyReactiveProperty<bool> IsHost();
            IReadOnlyReactiveProperty<string> GetMatchCode();
            IReadOnlyReactiveProperty<string> GetConnectionIssue();
            IReadOnlyReactiveProperty<List<PlayerModel>> GetPlayersInMatch();
            
        }
       
    }

}