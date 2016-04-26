using UnityEngine;
using System.Collections;

public class HPBox : MonoBehaviour {
    private int curHP = 1000;
    private int displayedHP = 1000;
    private Sprite[] HPdisplay = new Sprite[4];
    // When HP is set, display gradually scrolls to the new value
    public int hp { set {
            curHP = value;
            if (curHP != displayedHP)
            {
                //TODO
            } else
            {
                //TODO
            }
        } }


	// initialize based on player's HP
    public void init(int hp) {
        //TODO
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
