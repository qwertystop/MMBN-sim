using UnityEngine;
using System;
using System.Collections.Generic;

// this namespace exists because Unity won't put multidimensional collections in the editor without workarounds like this one
// experimentation shows that to get the Unity editor to display them, the object cannot be multiply parameterized
// so no lists of pairs of whatever - need a distinct type for each combination in use
namespace pairLists {
    [Serializable]
    public class intsAndSprites {
        public int[] ints;
        public Sprite[] sprites;
    }

    [Serializable]
    public class intsAndInts {
        public int[] locations;
        public int[] damages;
    }
}