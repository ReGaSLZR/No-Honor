using NaughtyAttributes;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ReGaSLZR
{

    public class CharacterMovement : MonoBehaviour
    {

        #region Constants

        private const float MIN_CALIBRATION = 0f;
        private const float MAX_CALIBRATION = 10f;

        #endregion //Constants

        #region Inspector Fields

        [Header("Components")]

        [SerializeField]
        [Required]
        private Rigidbody2D rigidBody2D;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private TargetDetector groundDetector;

        [SerializeField]
        private CharacterStatsView statsView;

        [Header("Animations")]

        [SerializeField]
        private Animator anima;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animBoolWalk;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animBoolJump;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animTriggerDead;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animTriggerHurt;

        [Header("Calibrations")]

        [SerializeField]
        [Range(MIN_CALIBRATION, MAX_CALIBRATION)]
        private float movementSpeed = 1f;

        [SerializeField]
        [Range(MIN_CALIBRATION, MAX_CALIBRATION)]
        private float jumpFallMultiplier = 1f;

        [SerializeField]
        [Range(MIN_CALIBRATION, MAX_CALIBRATION)]
        private float jumpVelocity = 1f;

        [SerializeField]
        [Range(MIN_CALIBRATION, MAX_CALIBRATION)]
        private float velocityPushOnStagger;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            statsView.GetHealthDiminished()
                .Where(dim => dim < 0)
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => Stagger())
                .AddTo(this);

            statsView.IsPlayerDead()
                .Where(dead => dead)
                .Subscribe(_ => anima.SetTrigger(animTriggerDead))
                .AddTo(this);

            this.FixedUpdateAsObservable()
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => CheckMovementInput())
                .AddTo(this);

            //TODO make the keys detection better. If have more time, use new InputSystem
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Where(_ => groundDetector.IsTargetDetected().Value)
                .Where(_ => !anima.GetBool(animBoolJump))
                .Subscribe(_ => JumpUp())
                .AddTo(this);

            groundDetector.IsTargetDetected()
                .Where(isGrounded => isGrounded)
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => anima.SetBool(animBoolJump, false))
                .AddTo(this);

            groundDetector.IsTargetDetected()
                .Where(isGrounded => !isGrounded)
                .Where(_ => !Input.GetKeyDown(KeyCode.Space))
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => anima.SetBool(animBoolJump, true))
                .AddTo(this);

            //jump fall - hasten with fall multiplier to prevent "floaty" default effect
            //concept reference: "Better Jumping in Unity >> https://www.youtube.com/watch?v=7KiK0Aqtmzc
            this.FixedUpdateAsObservable()
                .Select(_ => rigidBody2D.velocity)
                .Where(velocity => (velocity.y < 0))
                .Where(_ => !statsView.IsPlayerDead().Value)
                .Subscribe(_ => JumpFall())
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void Die()
        {
            anima.SetTrigger(animTriggerDead);
            //anima.Sto
        }

        private void Stagger()
        {
            anima.SetBool(animTriggerDead, false);
            anima.SetTrigger(animTriggerHurt);
            rigidBody2D.velocity *= (rigidBody2D.velocity/-velocityPushOnStagger);
        }

        private void JumpUp()
        {
            anima.SetBool(animBoolJump, true);
            rigidBody2D.velocity = (Vector2.up * jumpVelocity);
        }

        private void JumpFall()
        {
            anima.SetBool(animBoolJump, false);
            rigidBody2D.velocity += PhysicsUtil.GetFallVectorWithMultiplier(jumpFallMultiplier);
        }

        //TODO make the keys detection better. If have more time, use new InputSystem
        private void CheckMovementInput()
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                MoveAndAnimateSide(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                MoveAndAnimateSide(Vector2.right);
            }
            else
            {
                Idle();
            }
        }

        private void Idle() => anima.SetBool(animBoolWalk, false);

        private void MoveAndAnimateSide(Vector2 movementDirection)
        {
            rigidBody2D.position = (rigidBody2D.position +
                    (movementDirection * movementSpeed * Time.fixedDeltaTime));
            anima.SetBool(animBoolWalk, true);
            spriteRenderer.flipX = (movementDirection != Vector2.right);
        }

        #endregion //Client Impl

    }

}