using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;

public class HUD : MonoBehaviour {
    private GameObject HPBox;
    private GameObject CustomWindow;
    public Player player;

    void Awake() {
        HPBox = gameObject.FindChild("HPBox");
        CustomWindow = gameObject.FindChild("CustomWindow");
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
