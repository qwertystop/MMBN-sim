using UnityEngine;
using System.Collections.Generic;

// class for chips and other attacks
public class Chip : MonoBehaviour {
    // chip element
    public Controller.Element element;
    // animation for the player, one per frame
    public Queue<Sprite> playerAnimation = new Queue<Sprite>();
    // for each frame, all of the extra sprites to place, keyed by the locations in which to place them
    // locations are relative to location of player
    public Queue<List<KeyValuePair<int, Sprite>>> decorationAnimation = new Queue<List<KeyValuePair<int, Sprite>>>();
    // for each frame, damage keyed by panel
    // 0 damage for yellowing
    // locations are relative to location of player
    public Queue<List<KeyValuePair<int, int>>> hits = new Queue<List<KeyValuePair<int, int>>>();

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
