using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Util;

public class Controller : MonoBehaviour {
    // static object to allow accessing the parts of this from anywhere that can't be static due to Unity restrictions
    public static Controller gameCore;
    public static UIManager UI;// Static reference to UI manager, set in UIManager's Awake()
    public static Player[] players;// left, then right
    private static bool _paused = true;// game starts with custom screens up
    public static bool paused { get { return _paused; } }// externally readonly

    private static int cust = 0;// fill level of custom gauge
    public static int custSpeed = 2;// to allow halving from SloGauge
    public static float custFill { get { return cust / 1200f; } }// percentage of fill of custom gauge

    // Must be non-static to be set in editor
    public GameObject[] panels = new GameObject[18];//starting from top-left, counting horizontal before vertical

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        // make this available statically
        gameCore = this;
        // set framerate to 60fps cap
        // technically the GBA ran at 59.8621 Hz, but A: can you really tell? and B: Unity only takes integer framerates anyway
        Application.targetFrameRate = 60;
    }

    // Initialization after all Awake() methods have run
    void Start () {
        // players
        players = new Player[2];
        players[0] = gameObject.FindChild("LeftPlayer").GetComponent<Player>();
        players[1] = gameObject.FindChild("RightPlayer").GetComponent<Player>();
        // panels
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<Panel>().index = i;
        }
        // UI
        UI.Init(players[0], players[1]);
        // and start the first turn
        startTurn();
    }
	
	// Update is called once per frame
	void Update () {
        // if the game is paused and both players are ready to resume
        if (paused && UI.ready)
        {// resume it
            _paused = false;
        } else if (!paused)
        {// while not paused, custom gauge fills up over time
            if (cust < 200) { cust += custSpeed; }
            else if (cust >= 200) {
                cust = 200;// cap the gauge
                // if either player is pressing L or R and the gauge is full
                if (InputHandler.buttonDown(0, InputHandler.button.L) || InputHandler.buttonDown(0, InputHandler.button.R) ||
                    InputHandler.buttonDown(1, InputHandler.button.L) || InputHandler.buttonDown(1, InputHandler.button.R))
                {// then start the turn
                    startTurn();
                }
            }
        }
	}

    // starts the turn: pauses the game, delegates the rest to the UI
    private void startTurn() {
        _paused = true;
        cust = 0;
        UI.continueStartTurn();
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
