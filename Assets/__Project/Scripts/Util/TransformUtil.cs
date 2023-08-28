using UnityEngine;

namespace ReGaSLZR
{

    public static class TransformUtil
    {

        public static void DestroyAllChildren(this Transform parent)
        {
            while (parent.childCount > 0)
            {
                Object.DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }

    }

}