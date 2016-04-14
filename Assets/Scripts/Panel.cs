using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {
    public int index;
    public bool isRed = true;
    public Player occupant = null;
    public bool isOccupied { get { return occupant != null; } }
    public Sprite normalRed;
    public Sprite crackedRed;
    public Sprite normalBlue;
    public Sprite crackedBlue;
    public Sprite missing;


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
}
