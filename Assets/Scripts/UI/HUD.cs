using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;
using System;

// The UI for one player: Both their HP box and their CustomWindow
public class HUD : MonoBehaviour {
    private HPBox hpbox;
    private CustomWindow customwindow;
    private Vector3 hpOnPos, hpOffPos, customOnPos, customOffPos;
    private Player player;

    // is this player ready
    private bool rd = false;
    public bool ready { get { return rd; } }

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        hpbox = gameObject.FindChild("HPCanvas/HPBox").GetComponent<HPBox>();
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
        customwindow.Init(p, this);

        customOffPos = transform.localPosition;
        customOnPos = new Vector3(customOffPos.x - ((player.playerNo*2-1) * 120), customOffPos.y);
        hpOffPos = hpbox.transform.localPosition;
        hpOnPos = new Vector3(hpOffPos.x + ((player.playerNo*2-1) * 50), hpOffPos.y);

    }

    // Opens custom screen, draws chips, etc.
    // Called on start of turn - attempts to check gamestate and throws exception if wrong time
    public void openCustom() {
        if (Controller.paused)
        {
            // shift onto screen
            transform.localPosition = customOnPos;
            hpbox.transform.localPosition = hpOnPos;
            hpbox.setTransp(0.5f);

            rd = false;
            customwindow.finalStartTurn();
        } else { throw new Exception("Called out of order"); }
    }

    // Closes custom screen, sends chips to player, etc.
    // Called when player has selected OK or ADD - attempts to check gamestate and throws exception if wrong time
    // Signals Controller that player is ready
    public void closeCustom() {
        if (Controller.paused)
        {
            // shift off screen
            transform.localPosition = customOffPos;
            hpbox.transform.localPosition = hpOffPos;
            hpbox.setTransp(1f);
            rd = true;
        } else { throw new Exception("Called out of order"); }
    }
	
	// Update is called once per frame
	void Update () {
        // every frame, update HP box to player's HP
        hpbox.curHP = player.curHP;
	}
}
