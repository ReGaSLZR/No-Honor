using Photon.Pun;

namespace ReGaSLZR
{

    public class PhotonGameMaster : GameMaster
    {

        #region Unity Callbacks

        protected override void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            base.Start();
        }

        #endregion //Unity Callbacks

        #region Protected Virtuals

        protected override void DoExitGame()
        {
            PhotonNetwork.Disconnect();
            base.DoExitGame();
        }

        #endregion //Protected Virtuals

    }

}