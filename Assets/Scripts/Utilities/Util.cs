using System.Collections.Generic;
using UnityEngine;

namespace Util {
    // defining an empty delegate globally for use as necessary
    public delegate void intVoid(int x);

    public static class Extensions {
        // Extension method on GameObject:
        //  find the GameObject with the given name that is a child of this GameObject
        public static GameObject FindChild(this UnityEngine.GameObject ob, string name) {
            return ob.transform.Find(name).gameObject;
        }

        // Fisher-Yates Shuffle for IList<T>
        public static void Shuffle<T>(this IList<T> list) {
            for (int i = list.Count - 1; i > 0; --i)
            {
                int r = Random.Range(0, i);
                T val = list[r];
                list[r] = list[i];
                list[i] = val;
            }
        }
    }
}
