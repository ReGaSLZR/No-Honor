using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace ReGaSLZR
{

    [System.Serializable]
    public class SceneModel
    {

        #region Inspector Fields

        [SerializeField]
        [Scene]
        private string sceneLobby;

        [SerializeField]
        [Scene]
        private string sceneGameplay;

        #endregion //Inspector Fields

        #region Accessors

        public string SceneLobby => sceneLobby;

        public string SceneGameplay => sceneGameplay;

        #endregion //Accessors

    }

}