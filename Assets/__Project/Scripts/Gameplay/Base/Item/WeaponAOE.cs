using System.Collections;
using UnityEngine;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class WeaponAOE : AWeaponAsUsable
    {

        [SerializeField]
        [Range(1f, 10f)]
        private float activeDuration = 5f;

        #region Overrides

        [NaughtyAttributes.Button]
        protected override void Use()
        {
            gameObject.SetActive(true);
            rIsInUse.Value = true;
            StopAllCoroutines();
            StartCoroutine(C_ActiveCountdown());
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
            rIsInUse.Value = false;
        }

        protected override bool ShouldDeactivateOnCollision() => false;
        protected override bool ShouldDeactivateOnTargetDetection() => false;

        #endregion //Overrides

        #region Client Impl

        private IEnumerator C_ActiveCountdown()
        {
            yield return new WaitForSeconds(activeDuration);
            Deactivate();
        }

        #endregion //Client Impl
    }

}