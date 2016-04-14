using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using pairLists;

// abstract class for chips and other attacks
// allows delay before and after acting
public abstract class AChip : MonoBehaviour {
    // chip element
    public Controller.Element element;
    // animation for the player, one per frame
    public List<Sprite> playerAnimation;
    // for each frame, all of the extra sprites to place, matched to the locations in which to place them
    // locations are relative to location of player: +/- 1 horiz, +/-6 vert
    public intsAndSprites[] decorations;

    public int damageMultiplier = 1;
    public int damageBase = 0;
    public int damagePlus = 0;
    public Type chipType = Type.Standard;
    public string chipName = "NULL";
    public Sprite icon;
    public Sprite largeImg;

    // delay before and after acting
    public int windup = 0;
    public int winddown = 0;


    // coroutine to run when using this chip
    public abstract IEnumerator use(Player user);

    // Use this for initialization
    void Start() {

    }

    public enum Type {
        Standard,
        Mega,
        Giga,
        ProgramAdvance,
    };
}
