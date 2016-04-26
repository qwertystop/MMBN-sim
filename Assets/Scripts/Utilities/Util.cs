namespace Util {
    // defining an empty delegate globally for use as necessary
    public delegate void intVoid(int x);

    public static class Extensions {
        // Extension method on GameObject:
        //  find the GameObject with the given name that is a child of this GameObject
        public static UnityEngine.GameObject FindChild(this UnityEngine.GameObject ob, string name) {
            return ob.transform.Find(name).gameObject;
        }
    }
}
