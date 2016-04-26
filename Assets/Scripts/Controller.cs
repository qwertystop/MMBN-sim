using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller : MonoBehaviour {
    // static object to allow accessing the parts of this from anywhere
    public static Controller gameCore;
    public GameObject[] players;// left, then right
    public GameObject[] panels = new GameObject[18];//starting from top-left, counting horizontal before vertical
    public static UIManager UI;// set in UIManager's Awake()

    // all Awake methods are called on scene load for extant objects, before any Start methods (which are called before the first frame).
    void Awake() {
        gameCore = this;
        // set framerate to 60fps cap
        // technically the GBA ran at 59.8621 Hz, but A: can you really tell? and B: Unity only takes integer framerates
        Application.targetFrameRate = 60;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<Panel>().index = i;
        }
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    // is the first element super-effective against the second?
    public static bool isSuper(Element e1, Element e2) {
        return (e1 == Element.Aqua && e2 == Element.Heat) ||
            (e1 == Element.Heat && e2 == Element.Wood) ||
            (e1 == Element.Wood && e2 == Element.Elec) ||
            (e1 == Element.Elec && e2 == Element.Aqua);
    }

    public enum Element {
        Null = 0,
        Heat = 1,
        Aqua = 2,
        Elec = 3,
        Wood = 4
    };
}
