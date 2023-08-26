using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ReGaSLZR
{

    public class CharacterMovement : MonoBehaviour
    {

        #region Inspector Fields

        [Header("Components")]

        [SerializeField]
        [Required]
        private Rigidbody2D rigidBody2D;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private TargetDetector groundDetector;

        [Header("Animations")]

        [SerializeField]
        private Animator anima;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animBoolWalk;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animBoolJump;

        [Header("Calibrations")]

        [SerializeField]
        private float movementSpeed = 1f;

        [SerializeField]
        private float jumpFallMultiplier = 1f;

        [SerializeField]
        private float jumpVelocity = 1f;

        #endregion //Inspector Fields

        #region Private Fields

        //private bool isGr

        #endregion //Private Fields

        #region Unity Callbacks

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Subscribe(_ => CheckMovementInput())
                .AddTo(this);

            //TODO make the keys detection better. If have more time, use new InputSystem
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Where(_ => groundDetector.IsTargetDetected().Value)
                .Where(_ => !anima.GetBool(animBoolJump))
                .Subscribe(_ => JumpUp())
                .AddTo(this);

            groundDetector.IsTargetDetected()
                .Where(isGrounded => isGrounded)
                .Subscribe(_ => anima.SetBool(animBoolJump, false))
                .AddTo(this);

            //jump fall - hasten with fall multiplier to prevent "floaty" default effect
            //concept reference: "Better Jumping in Unity >> https://www.youtube.com/watch?v=7KiK0Aqtmzc
            this.FixedUpdateAsObservable()
                .Select(_ => rigidBody2D.velocity)
                .Where(velocity => (velocity.y < 0))
                .Subscribe(_ => JumpFall())
                .AddTo(this);
        }

        #endregion //Unity Callbacks

        #region Client Impl

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