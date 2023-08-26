using UnityEngine;
using UnityEngine.UI;

namespace ReGaSLZR
{

    public class LobbyStarterView : MonoBehaviour
    {

        #region Inspector Fields

        [Header("Canvases")]

        [SerializeField]
        private Canvas canvasStarter;

        [Header("Starter Sub views")]

        [SerializeField]
        private GameObject starterMain;

        [SerializeField]
        private LobbyStartPopUpView starterSubJoin;

        [SerializeField]
        private LobbyStartPopUpView starterSubCreate;

        [Header("Starter Buttons")]

        [SerializeField]
        private Button buttonDisplaySubJoin;

        [SerializeField]
        private Button buttonDisplaySubCreate;

        #endregion //Inspector Fields

        #region Accessors

        public LobbyStartPopUpView PopUpViewJoin => starterSubJoin;

        public LobbyStartPopUpView PopUpViewCreate => starterSubCreate;

        #endregion //Accessors

        #region Unity Callbacks

        private void Awake()
        {
            starterSubJoin.RegisterOnClose(() => starterMain.SetActive(true));
            starterSubCreate.RegisterOnClose(() => starterMain.SetActive(true));

            buttonDisplaySubJoin.onClick.AddListener(() => SwitchToSubView(starterSubCreate, starterSubJoin));
            buttonDisplaySubCreate.onClick.AddListener(() => SwitchToSubView(starterSubJoin, starterSubCreate));
        }

        private void Start()
        {
            InitView();
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void SwitchToSubView(LobbyStartPopUpView disabledView, LobbyStartPopUpView enabledView) 
        {

            disabledView.SetIsDisplayed(false);
            disabledView.SetIsDisplayed(true);
            starterMain.SetActive(false);
        }

        #endregion //Client Impl

        #region Public API

        public void InitView() 
        {
            starterSubJoin.SetIsDisplayed(false);
            starterSubCreate.SetIsDisplayed(false);
            SetIsDisplayed(true);
        }

        public void SetIsDisplayed(bool isDisplayed)
        {
            if (isDisplayed)
            {
                starterMain.SetActive(isDisplayed);
                InitView();
            }

            canvasStarter.gameObject.SetActive(isDisplayed);
        }

        #endregion //Public API

    }

}