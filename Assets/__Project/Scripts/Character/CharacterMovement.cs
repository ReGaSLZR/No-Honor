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

        [Header("Animations")]

        [SerializeField]
        private Animator anima;

        [SerializeField]
        [AnimatorParam("anima")]
        private string animBoolWalk;

        [Header("Calibrations")]

        [SerializeField]
        private float movementSpeed = 1f;

        #endregion //Inspector Fields

        #region Unity Callbacks

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Subscribe(_ => CheckMovementInput())
                .AddTo(this);


        }

        #endregion //Unity Callbacks

        #region Client Impl


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