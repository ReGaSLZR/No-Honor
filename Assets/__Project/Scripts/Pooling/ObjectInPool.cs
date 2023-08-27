using UnityEngine;
using System.Collections;

namespace ReGaSLZR
{

    public class ObjectInPool : MonoBehaviour
    {

        #region Private Fields

        private int index;
        private ObjectPooler pooler;

        #endregion //Private Fields

        #region Public API

        public void SetUp(int index, ObjectPooler pooler)
        {
            this.index = index;
            this.pooler = pooler;
        }

        public void PutBackToPool()
        {
            if (pooler == null)
            {
                Debug.LogWarning("PutBackToPool():" +
                    " Missing ObjectPooler. Did you forget to call SetUp()?");
            }
            else
            {
                pooler.RecycleInPool(index);
            }
            
        }

        public void PutBackToPool(float delay)
        {
            StartCoroutine(C_BackToPool(delay));
        }

        #endregion //Public API

        #region Client Impl

        private IEnumerator C_BackToPool(float delay)
        {
            yield return new WaitForSeconds(delay);
            PutBackToPool();
        }

        #endregion //Client Impl

    }

}