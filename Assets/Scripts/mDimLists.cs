using UnityEngine;
using System;
using System.Collections.Generic;

// this namespace exists because Unity won't put multidimensional collections in the editor without workarounds like this one
// experimentation shows that to get the Unity editor to display them, the object cannot be multiply parameterized
// so no lists of pairs of whatever - need a distinct type for each combination in use
namespace mDimLists {
    // this one needs to exist to get around the multidimensional list prohibition
    [Serializable]
    public abstract class indexed<T> {
        public int index;
        public T value;
    }

    // all below need to exist because Unity does not support serializing generics in the editor other than List<T>
    [Serializable]
    public class indexedInt : indexed<int> { }

    [Serializable]
    public class indexedAnimation2D : indexed<Animation2D> { }
}