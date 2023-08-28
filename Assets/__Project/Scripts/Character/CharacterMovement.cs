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
        private new Collider2D collider2D;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private TargetDetector groundDetector;

        [SerializeField]
        private CharacterStats stats;

        [SerializeField]
        private CharacterAnimator anima;

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

        private CompositeDisposable controlledDisposables = new CompositeDisposable();

        #region Unity Callbacks

        private void OnEnable()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => CheckMovementInput())
                .AddTo(controlledDisposables);

            //TODO make the keys detection better. If have more time, use new InputSystem
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Where(_ => !stats.IsPlayerDead().Value)
                .Where(_ => groundDetector.IsTargetDetected().Value)
                .Where(_ => !anima.Animator.GetBool(anima.AnimBoolJump))
                .Subscribe(_ => JumpUp())
                .AddTo(controlledDisposables);
        }

        private void OnDisable()
        {
            controlledDisposables.Dispose();
            controlledDisposables.Clear();
            controlledDisposables = new CompositeDisposable();
        }

        private void Start()
        {
            stats.GetHealthDiminished()
                .Where(dim => dim < 0)
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => Stagger())
                .AddTo(this);

            stats.IsPlayerDead()
                .Where(isDead => isDead)
                .Subscribe(_ => OnDeath())
                .AddTo(this);

            groundDetector.IsTargetDetected()
                .Where(isGrounded => isGrounded)
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => anima.Animator.SetBool(anima.AnimBoolJump, false))
                .AddTo(this);

            groundDetector.IsTargetDetected()
                .Where(isGrounded => !isGrounded)
                .Where(_ => !Input.GetKeyDown(KeyCode.Space))
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => anima.Animator.SetBool(anima.AnimBoolJump, true))
                .AddTo(this);

            //jump fall - hasten with fall multiplier to prevent "floaty" default effect
            //concept reference: "Better Jumping in Unity >> https://www.youtube.com/watch?v=7KiK0Aqtmzc
            this.FixedUpdateAsObservable()
                .Select(_ => rigidBody2D.velocity)
                .Where(velocity => (velocity.y < 0))
                .Where(_ => !stats.IsPlayerDead().Value)
                .Subscribe(_ => JumpFall())
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void OnDeath()
        {
            rigidBody2D.bodyType = RigidbodyType2D.Static;
            collider2D.isTrigger = true;
            collider2D.enabled = false;
        }

        private void Stagger() =>
            rigidBody2D.velocity *= (rigidBody2D.velocity/-velocityPushOnStagger);

        private void JumpUp()
        {
            anima.Animator.SetBool(anima.AnimBoolJump, true);
            rigidBody2D.velocity = (Vector2.up * jumpVelocity);
        }

        private void JumpFall()
        {
            anima.Animator.SetBool(anima.AnimBoolJump, false);
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

        private void Idle() => anima.Animator.SetBool(anima.AnimBoolWalk, false);

        private void MoveAndAnimateSide(Vector2 movementDirection)
        {
            rigidBody2D.position = (rigidBody2D.position +
                    (movementDirection * movementSpeed * Time.fixedDeltaTime));
            anima.Animator.SetBool(anima.AnimBoolWalk, true);
            spriteRenderer.flipX = (movementDirection != Vector2.right);
        }

        #endregion //Client Impl

    }

}