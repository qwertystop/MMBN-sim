using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;

// The UI for one player: Both their HP box and their CustomWindow
public class HUD : MonoBehaviour {
    private HPBox hpbox;
    private CustomWindow customwindow;
    private Player player;

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        hpbox = gameObject.FindChild("HPBox").GetComponent<HPBox>();
        customwindow = gameObject.FindChild("CustomWindow").GetComponent<CustomWindow>();
    }

    // Initialization after all Awake() methods have run
    void Start () {
    }

    // Initialization that depends on specific external things having initialized
    // this needs a Player reference from the UIManager (which knows which player goes with which HUD).
    public void Init(Player p) {
        player = p;
        hpbox.Init(player.maxHP);
        customwindow.Init(p);
    }
	
	// Update is called once per frame
	void Update () {
        // every frame, update HP box to player's HP
        hpbox.curHP = player.curHP;
	}
}
