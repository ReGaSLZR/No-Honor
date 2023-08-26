using UnityEngine;
using Zenject;

namespace ReGaSLZR
{

    public class PhotonConnectionInstaller : MonoInstaller
    {

        [SerializeField]
        private PhotonConnectionModel photonConnModel;

        public override void InstallBindings()
        {
            Container.BindInstance<IConnectionModel.IGetter>(photonConnModel);
            Container.BindInstance<IConnectionModel.ISetter>(photonConnModel);
        }

    }

}