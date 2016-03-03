using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
    
    public GameObject[] panels = new GameObject[18];
    public static GameObject[,] panelsInRows = new GameObject[6, 3];

	// Use this for initialization
	void Start () {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<Panel>().index = i;
            panelsInRows[i / 6, i % 6] = panels[i];
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
