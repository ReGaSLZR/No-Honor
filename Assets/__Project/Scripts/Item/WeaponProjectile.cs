using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using NaughtyAttributes;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class WeaponProjectile : MonoBehaviour
    {

        #region Inspector Fields

        [Header("Elements")]

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

        private Rigidbody2D rigidBody2D;
        private readonly ReactiveProperty<CharacterBrain> rVictim = new ReactiveProperty<CharacterBrain>();

        private bool isAboutToBeInactive;
        private Vector3 startingPosition;

        #region Unity Callbacks

        private void Awake()
        {
            startingPosition = gameObject.transform.localPosition;
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            this.OnCollisionEnter2DAsObservable()
                .Where(coll => !isAboutToBeInactive)
                .Where(coll => coll.gameObject.tag.Equals(Tag.Player.ToString()))
                .Select(coll => coll.gameObject.GetComponent<CharacterBrain>())
                .Where(brain => brain != rVictim.Value)
                .Subscribe(brain => rVictim.Value = brain)
                .AddTo(this);

            this.OnCollisionExit2DAsObservable()
                .Subscribe(coll => rVictim.Value = null)
                .AddTo(this);

            this.OnCollisionEnter2DAsObservable()
                .Subscribe(_ => Pop())
                .AddTo(this);

            viewNormal.SetActive(false);
            viewOnImpact.SetActive(false);
            gameObject.SetActive(false);
        }

        #endregion //Unity Callbacks

        #region Client Impl

        private void Pop()
        {
            isAboutToBeInactive = true;

            viewNormal.SetActive(false);
            viewOnImpact.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(C_Pop());
        }

        private IEnumerator C_Pop()
        {
            yield return new WaitForSeconds(delayBeforeInactivity);

            rigidBody2D.velocity = Vector2.zero;

            isAboutToBeInactive = false;
            viewOnImpact.SetActive(false);
            gameObject.SetActive(false);
        }

        #endregion //Client Impl

        #region Public API

        public IReadOnlyReactiveProperty<CharacterBrain> GetVictim() => rVictim;

        //TODO delete these later. Coded only for quick play-testing.
        [Button] public void FireLeft() => AttemptFire(false);
        [Button] public void FireRight() => AttemptFire(true);

        public bool AttemptFire(bool isRightDirection)
        {
            if (gameObject.activeInHierarchy || gameObject.activeSelf)
            {
                return false;
            }

            gameObject.transform.localPosition = startingPosition;
            rigidBody2D.velocity = Vector2.zero;

            viewNormal.SetActive(true);
            viewOnImpact.SetActive(false);
            gameObject.SetActive(true);

            var direction = new Vector2(fireDirection.x * 
                (isRightDirection ? 1f : -1f), fireDirection.y);
            rigidBody2D.AddForce(direction * force, ForceMode2D.Impulse);
            rigidBody2D.AddTorque(torque);

            return true;
        }

        #endregion //Public API

    }

}