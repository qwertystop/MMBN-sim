using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;

public class HUD : MonoBehaviour {
    private HPBox hpbox;
    private CustomWindow customwindow;
    public Player player;

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        hpbox = gameObject.FindChild("HPBox").GetComponent<HPBox>();
        customwindow = gameObject.FindChild("CustomWindow").GetComponent<CustomWindow>();
    }

    // Initialization after all Awake() methods have run
    void Start () {
    }

    // Initialization that depends on specific external things having initialized
    // this cannot be run until UIManager.Start() has been run, so that calls this method
    public void Init() {
        hpbox.Init(player.hp);
    }
	
	// Update is called once per frame
	void Update () {
        // every frame, update HP box to player's HP
        hpbox.curHP = player.hp;
	}
}
