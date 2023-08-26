using UnityEngine;
using Zenject;

namespace ReGaSLZR
{

    public class SceneInstaller : MonoInstaller
    {

        [SerializeField]
        private SceneModel sceneModel;

        public override void InstallBindings()
        {
            Container.BindInstance(sceneModel);
        }

    }

}