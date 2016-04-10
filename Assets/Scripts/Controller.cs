using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
    
    public static Controller gameCore;
    public GameObject[] panels = new GameObject[18];

	// Use this for initialization
	void Start () {
        gameCore = this;
        // set framerate to 60fps cap
        // technically the GBA ran at 59.8621 Hz, but A: can you really tell? and B: Unity only takes integer framerates
        Application.targetFrameRate = 60;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<Panel>().index = i;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    public enum Element {
        Null,
        Fire,
        Aqua,
        Wood,
        Elec
    };
}
