using NaughtyAttributes;
using UniRx;
using UnityEngine;

namespace ReGaSLZR
{

    public class CharacterAnimator : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Animator animator;

        [Space]

        [SerializeField]
        [AnimatorParam("animator")]
        private string animBoolWalk;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animBoolJump;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerDead;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerHurt;

        [Header("Other Settings")]

        [SerializeField]
        private CharacterStatsView statsView;

        #endregion //Inspector Fields

        #region Accessors

        public Animator Animator => animator;
        public string AnimBoolWalk => animBoolWalk;
        public string AnimBoolJump => animBoolJump;
        public string AnimTriggerDead => animTriggerDead;
        public string AnimTriggerHurt => animTriggerHurt;

        #endregion //Accessors

        #region Unity Callbacks

        private void Start()
        {
            statsView.GetHealthDiminished()
                .Where(dim => dim < 0)
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => animator.SetTrigger(animTriggerHurt))
                .AddTo(this);

            statsView.IsPlayerDead()
                .Where(dead => dead)
                .Subscribe(_ => animator.SetTrigger(animTriggerDead))
                .AddTo(this);
        }

        #endregion //Unity Callbacks

    }

}