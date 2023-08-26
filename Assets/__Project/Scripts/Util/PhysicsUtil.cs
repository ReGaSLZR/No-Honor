using UnityEngine;

namespace ReGaSLZR
{

    public class PhysicsUtil
    {

        /// <summary>
        /// Hasten the falling of a physics-influenced object by multiplying the gravity.
        /// reference: "Better Jumping in Unity >> https://www.youtube.com/watch?v=7KiK0Aqtmzc
        /// </summary>
        /// <param name="multiplier">Has to be greater than 1 (the default gravity).</param>
        /// <returns></returns>
        public static Vector2 GetFallVectorWithMultiplier(float multiplier)
        {
            return (Vector2.up * Physics2D.gravity.y * (multiplier - 1f) * Time.fixedDeltaTime);
        }

    }


}