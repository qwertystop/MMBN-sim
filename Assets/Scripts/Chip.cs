using UnityEngine;
using System.Collections.Generic;
using pairLists;

// class for chips and other attacks
public class Chip : MonoBehaviour {
    // chip element
    public Controller.Element element;
    // animation for the player, one per frame
    public List<Sprite> playerAnimation = new List<Sprite>(1);
    // for each frame, all of the extra sprites to place, matched to the locations in which to place them
    // locations are relative to location of player: +/- 1 horiz, +/-6 vert
    public intsAndSprites[] decorations;
    // for each frame, damage keyed by panel
    // 0 damage for yellowing
    // locations are relative to location of player
    public intsAndInts[] damages;

    public int damageMultiplier = 1;
    public int damagePlus = 0;
    public Type chipType;
    public string chipName;

    public Sprite icon;
    public Sprite largeImg;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public enum Type {
        Standard,
        Mega,
        Giga,
        ProgramAdvance,
    };
}
