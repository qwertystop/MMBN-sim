using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
    
    public static Controller gameCore;
    public GameObject[] panels = new GameObject[18];

	// Use this for initialization
	void Start () {
        gameCore = this;
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
