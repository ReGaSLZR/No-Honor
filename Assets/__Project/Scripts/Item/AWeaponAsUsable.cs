using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AWeaponAsUsable : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private Weapon weaponType;

        [SerializeField]
        [Range(PlayerModel.PLAYER_HEALTH_DEAD, PlayerModel.PLAYER_HEALTH_MAX)]
        private int damage;

        [SerializeField]
        private bool isDamageOverTime;

        [Space]

        [SerializeField]
        protected TargetDetector[] targetDetectors;

        #endregion //Inspector Fields

        protected Rigidbody2D rigidBody2D;

        protected readonly ReactiveProperty<bool> rIsTargetDetected = new ReactiveProperty<bool>();

        #region Abstracts

        protected abstract void Use();
        protected abstract bool ShouldDeactivateOnTargetDetection();
        protected abstract bool ShouldDeactivateOnCollision();
        public abstract void Deactivate();

        #endregion //Abstracts

        #region Unity Callbacks

        protected virtual void Awake() => rigidBody2D = GetComponent<Rigidbody2D>();

        protected virtual void Start()
        {
            foreach (var detector in targetDetectors)
            {
                detector.IsTargetDetected()
                    .Where(detected => detected)
                    .Subscribe(_ => rIsTargetDetected.SetValueAndForceNotify(true))
                    .AddTo(this);
            }

            if (ShouldDeactivateOnTargetDetection())
            {
                foreach (var detector in targetDetectors)
                {
                    detector.IsTargetDetected()
                        .Where(detected => detected)
                        .Subscribe(_ => Deactivate())
                        .AddTo(this);
                }
            }

            if (ShouldDeactivateOnCollision())
            {
                this.OnCollisionEnter2DAsObservable()
                    .Subscribe(coll => Deactivate())
                    .AddTo(this);
            }

            gameObject.SetActive(false);
        }

        private void OnDisable() => rIsTargetDetected.SetValueAndForceNotify(false);

        #endregion //Unity Callbacks

        #region Public API

        public Weapon WeaponType => weaponType;
        public int DamageValue => damage;
        public bool IsDamageOverTime => isDamageOverTime;

        public IReactiveProperty<bool> IsTargetDetected => rIsTargetDetected;

        public bool AttemptUse()
        {
            if (gameObject.activeInHierarchy || gameObject.activeSelf)
            {
                return false;
            }

            Use();
            return true;
        }

        public List<CharacterBrain> GetTargets()
        {
            var targets = new List<CharacterBrain>();

            foreach (var detector in targetDetectors)
            {
                var genTargets = detector.GetTargets();
                foreach (var gen in genTargets)
                {
                    var isChar = gen.gameObject.TryGetComponent<CharacterBrain>(
                        out var brain);
                    if (isChar)
                    {
                        targets.Add(brain);
                    }
                }
            }

            return targets;
        }

        #endregion //Public API

    }

}