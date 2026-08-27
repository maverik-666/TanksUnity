namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extensions
    {
        public static float RoundToStep(this float value, float step)
        {
            return Mathf.Round(value / step) * step;
        }

        public static Vector3 RoundToStep(this Vector3 value, float step)
        {
            return new Vector3(
                value.x.RoundToStep(step),
                value.y.RoundToStep(step),
                value.z.RoundToStep(step)
            );
        } 
        
        public static void Shuffle<T>(this IList<T> ts) {
            int count = ts.Count;
            int last = count - 1;
            for (var i = 0; i < last; ++i) {
                int r = Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }
    }
}