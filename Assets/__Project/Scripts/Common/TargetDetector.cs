using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System.Collections.Generic;
using NaughtyAttributes;

namespace ReGaSLZR
{

    [RequireComponent(typeof(Collider2D))]
    public class TargetDetector : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField]
        private List<Collider2D> excludedObjects;

        [SerializeField]
        [Tag]
        private List<string> targetTags;

        [Tooltip("If set to FALSE, this will capture ALL targets within its range " +
            "upon detection. If set to TRUE, disregard the value of Range.")]
        [SerializeField]
        private bool isLockedToFirstSingleTarget;

        [SerializeField]
        [DisableIf("isLockedToFirstSingleTarget")]
        private float detectionRange = 5f;

        [Space]

        [SerializeField]
        private bool isAdjustingHorizontally;

        [SerializeField]
        [ShowIf("isAdjustingHorizontally")]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        [ShowIf("isAdjustingHorizontally")]
        [Range(0f, 20f)]
        private float offsetHorizontal = 1f;

        #endregion //Inspector Fields

        #region Private Fields

        private ReactiveProperty<bool> isTargetDetected = new ReactiveProperty<bool>(false);
        private List<Collider2D> targets = new List<Collider2D>();

        private CompositeDisposable disposables = new CompositeDisposable();

        private Collider2D coll2D;
        private bool currentFlipX;

        #endregion //Private Fields

        #region Unity Callbacks

        private void Awake()
        {
            coll2D = GetComponent<Collider2D>();
        }

        private void OnDisable()
        {
            disposables.Clear();
            ClearTargets();
        }

        private void OnEnable()
        {
            InitControlledObservers();
            InitUnControlledObservers();
        }

        #endregion //Unity Callbacks

        #region Public API

        public IReadOnlyReactiveProperty<bool> IsTargetDetected() => isTargetDetected;

        public List<Collider2D> GetTargets() => targets;

        public void CheckTargetsListForDestruction()
        {
            for (int x = (targets.Count - 1); x >= 0; x--)
            {
                if (targets[x] == null)
                {
                    targets.RemoveAt(x);
                }
            }

            if (targets.Count == 0)
            {
                isTargetDetected.Value = false;
            }
        }

        #endregion //Public API

        #region Client Impl

        private void InitControlledObservers()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(otherCollider2D => IsNotExcludedObject(otherCollider2D))
                .Where(otherCollider2D => IsMatchingTag(otherCollider2D.tag))
                .Subscribe(otherCollider2D => CaptureTargets(otherCollider2D))
                .AddTo(disposables);

            this.OnTriggerExit2DAsObservable()
                .Where(otherCollider2D => IsNotExcludedObject(otherCollider2D))
                .Where(otherCollider2D => IsMatchingTag(otherCollider2D.tag))
                .Subscribe(otherCollider2D => ClearTargets())
                .AddTo(disposables);

            this.OnCollisionEnter2DAsObservable()
                .Where(otherCollision2D => IsNotExcludedObject(otherCollision2D.collider))
                .Where(otherCollision2D => IsMatchingTag(otherCollision2D.gameObject.tag))
                .Subscribe(otherCollision2D => CaptureTargets(otherCollision2D.collider))
                .AddTo(disposables);

            this.OnCollisionExit2DAsObservable()
                .Where(otherCollision2D => IsNotExcludedObject(otherCollision2D.collider))
                .Where(otherCollision2D => IsMatchingTag(otherCollision2D.gameObject.tag))
                .Subscribe(otherCollider2D => ClearTargets())
                .AddTo(disposables);

            //self-check list of targets for null items, then reset m_isTargetDetected.Value
            Observable.Interval(System.TimeSpan.FromSeconds(1))
                .Where(_ => isTargetDetected.Value)
                .Subscribe(_ => CheckTargetsListForDestruction())
                .AddTo(disposables);
        }

        private void InitUnControlledObservers()
        {
            if (isAdjustingHorizontally)
            {
                currentFlipX = spriteRenderer.flipX;

                this.FixedUpdateAsObservable()
                .Select(_ => spriteRenderer.flipX)
                .Where(hasFlipped => (hasFlipped != currentFlipX))
                .Subscribe(_ => {
                    OffsetCollider();
                })
                .AddTo(disposables);

                OffsetCollider();
            }
        }

        private void OffsetCollider()
        {
            currentFlipX = spriteRenderer.flipX;
            coll2D.offset = new Vector2(
                (offsetHorizontal * (currentFlipX ? -1 : 1)),
                coll2D.offset.y);
        }

        private void CaptureTargets(Collider2D targetCollider)
        {
            if (!isTargetDetected.Value)
            {
                RefreshTargets(targetCollider);
                isTargetDetected.Value = true;
            }
        }

        private void ClearTargets()
        {
            isTargetDetected.SetValueAndForceNotify(false);
            targets.Clear();
        }

        private void RefreshTargets(Collider2D coll)
        {
            targets.Clear();

            if (isLockedToFirstSingleTarget)
            {
                if (IsMatchingTag(coll.tag) && IsNotExcludedObject(coll))
                {
                    targets.Add(coll);
                }
            }
            else
            {
                Collider2D[] tempTargets = Physics2D.OverlapCircleAll(transform.position, detectionRange);

                //filter targets by tags
                foreach (Collider2D collider2D in tempTargets)
                {
                    if (collider2D.isActiveAndEnabled 
                        && IsMatchingTag(collider2D.tag) && IsNotExcludedObject(collider2D))
                    {
                        targets.Add(collider2D);
                    }
                }
            }
        }

        private bool IsMatchingTag(string tag) => targetTags.Contains(tag);

        private bool IsNotExcludedObject(Collider2D col) => !excludedObjects.Contains(col);

        #endregion //Client Impl

    }


}