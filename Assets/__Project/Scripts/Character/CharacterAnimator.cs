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

        [Header("Animation - Locomotion")]

        [SerializeField]
        [AnimatorParam("animator")]
        private string animBoolWalk;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animBoolJump;

        [Header("Animation - Status")]

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerDead;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerHurt;

        [Header("Animation - Attack")]

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerAttackRock;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerAttackFire;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animTriggerAttackPoison;

        [Header("Other Settings")]

        [SerializeField]
        private CharacterStats stats;

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
            stats.GetHealthDiminished()
                .Where(dim => dim < 0)
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => animator.SetTrigger(animTriggerHurt))
                .AddTo(this);

            stats.IsPlayerDead()
                .Where(dead => dead)
                .Subscribe(_ => animator.SetTrigger(animTriggerDead))
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Public API

        public void AnimateAttackWithWeapon(Weapon weapon)
        {
            if (weapon == Weapon.None)
            {
                return;
            }

            animator.SetTrigger(
                (Weapon.Rock == weapon) ? animTriggerAttackRock
                : (Weapon.Fire == weapon) ? animTriggerAttackFire
                : (Weapon.Poison == weapon) ? animTriggerAttackPoison
                : animTriggerAttackRock);
        }

        #endregion //Public API

    }

}