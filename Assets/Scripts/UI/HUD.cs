using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;

public class HUD : MonoBehaviour {
    private HPBox hpbox;
    private CustomWindow customwindow;
    public Player player;

    void Awake() {
        hpbox = gameObject.FindChild("HPBox").GetComponent<HPBox>();
        customwindow = gameObject.FindChild("CustomWindow").GetComponent<CustomWindow>();
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        // every frame, update HP box to player's HP
        hpbox.hp = player.hp;
	}
}
