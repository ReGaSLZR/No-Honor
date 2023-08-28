using UnityEngine;
using Zenject;

namespace ReGaSLZR
{

    [CreateAssetMenu(
        menuName = "ReGaSLZR / Create new Scene Model", 
        fileName = "Scene Model")]
    public class SceneInstaller : ScriptableObjectInstaller
    {

        [SerializeField]
        private SceneModel sceneModel;

        public override void InstallBindings()
        {
            Container.BindInstance(sceneModel);
        }

    }

}