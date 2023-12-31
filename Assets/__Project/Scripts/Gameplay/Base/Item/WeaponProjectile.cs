using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class WeaponProjectile : AWeaponAsUsable
    {

        #region Inspector Fields

        [Header("Elements")]

        [SerializeField]
        private SpriteRenderer spriteForDirection;

        [SerializeField]
        private GameObject viewNormal;

        [SerializeField]
        private GameObject viewOnImpact;

        [Header("Settings")]

        [SerializeField]
        private float delayBeforeInactivity;

        [SerializeField]
        private Vector2 fireDirection;

        [SerializeField]
        [Range(0f, 5f)]
        private float force;

        [SerializeField]
        [Range(0f, 5f)]
        private float torque;

        #endregion //Inspector Fields

        private Vector3 startingPosition;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            startingPosition = gameObject.transform.localPosition;
        }

        protected override void Start()
        {
            base.Start();
            viewNormal.SetActive(false);
            viewOnImpact.SetActive(false);
        }

        #endregion //Unity Callbacks

        #region Overrides

        protected override bool ShouldDeactivateOnTargetDetection() => true;
        protected override bool ShouldDeactivateOnCollision() => true;

        private IEnumerator C_Deactivate()
        {
            rigidBody2D.velocity = Vector2.zero;
            yield return new WaitForSeconds(delayBeforeInactivity);

            viewOnImpact.SetActive(false);
            gameObject.SetActive(false);
            rIsInUse.SetValueAndForceNotify(false);
        }

        #endregion //Overrides

        #region Public API

        public override void Deactivate()
        {
            viewNormal.SetActive(false);
            viewOnImpact.SetActive(true);

            if (gameObject.activeInHierarchy || gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(C_Deactivate());
            }
            else
            {
                viewOnImpact.SetActive(false);
                gameObject.SetActive(false);
                rIsInUse.SetValueAndForceNotify(false);
            }
        }

        [NaughtyAttributes.Button] //only in Editor, debug
        protected override void Use()
        {
            gameObject.transform.localPosition = startingPosition;
            rigidBody2D.velocity = Vector2.zero;

            viewNormal.SetActive(true);
            viewOnImpact.SetActive(false);
            gameObject.SetActive(true);
            rIsInUse.SetValueAndForceNotify(true);

            var direction = new Vector2(fireDirection.x * 
                (spriteForDirection.flipX ? -1f : 1f), fireDirection.y);
            rigidBody2D.AddForce(direction * force, ForceMode2D.Impulse);
            rigidBody2D.AddTorque(torque);

        }

        #endregion //Public API

    }

}