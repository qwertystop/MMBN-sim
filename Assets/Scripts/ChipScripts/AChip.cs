using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mDimLists;

// abstract class for chips and other attacks
// allows delay before and after acting
public abstract class AChip : MonoBehaviour {
    // chip element
    public Controller.Element element;
    // chip code
    [SerializeField]// editor access
    private char _code;// all possible codes
    public char code { get { return _code; } }// easy accessor
    // animation for the player, one per frame
    public Animation2D playerAnimation;
    // All of the animations to place at other panels, paired with the panel indices at which to place them
    // indices may be relative to location of player, depending on chip type: +/- 1 horiz, +/-6 vert
    public indexedAnimation2D[] decorations;

    public int damageMultiplier = 1;
    public int damageBase = 0;// if the chip does different damage with different hitboxes, this is the one displayed
    public int damagePlus = 0;
    public Type chipType = Type.Standard;
    public string chipName = "NULL";
    public Sprite icon;
    public Sprite largeImg;

    // delay before and after acting
    public int windup = 0;
    public int winddown = 0;


    // coroutine to run when using this chip
    // will be overridden by inheritors but generally start by calling this
    public virtual IEnumerator use(Player user) {
        // prevent overlapping actions
        StartCoroutine(haltUser(user));
        // animate user
        user.Animate(playerAnimation);
        // wait through windup
        for (int i = 0; i <= windup; i++)
        {
            yield return 0;
        }
    }

    // Prevent player from doing two things at once
    private IEnumerator haltUser(Player user) {
        user.status = Player.Status.ACTING;
        for (int i = 0; i < windup+winddown; ++i)
        {
            yield return 0;
        }
        user.status = Player.Status.FREE;
    }

    // Decorates panels with animations in given list (defaults to using this.decorations), flipped by the player's side but otherwise not relative
    public void decorateFixed(bool userIsRed, indexedAnimation2D[] dec = null) {
        if (null == dec) { dec = decorations; }
        foreach (indexed<Animation2D> anim in dec)
        {// for each animation
            // if the panel it's for exists, decorate that panel with it
            if (anim.index > -1 && anim.index < 18)
            {
                Controller.gameCore.panels[anim.index].GetComponent<Panel>().Decorate(anim.value, !userIsRed);
            }
        }
    }

    // Decorates panels with animations in given list (defaults to using this.decorations), position relative to given index (and whether user is red)
    public void decorateRelative(int start, bool userIsRed, indexedAnimation2D[] dec = null) {
        if (null == dec) { dec = decorations; }
        foreach (indexed<Animation2D> anim in dec)
        {// make each one relative
            int rel = makeRelative(start, anim.index, userIsRed);
            if (rel > -1 && rel < 18)
            {// then, if panel exists, decorate it
                Controller.gameCore.panels[rel].GetComponent<Panel>().Decorate(anim.value, !userIsRed);
            }
        }
    }

    // given an int for a panel index and a player, mirror the index to the opposite side if the player is the blue (right) one
    // but otherwise do not shift: Position remains relative to battlefield.
    protected int mirrorIfBlue(Player user, int index) {
        // vertical and horizontal displacement
        int vert = (index / 6) * 6;
        int horiz = index % 6; // % is actually remainder, not mod, even if it's called mod - negatives are kept properly

        return user.isRed ? vert + horiz : vert - horiz;
    }

    // given a starting panel, the displacement number, and whether the user is red
    // return the displaced location, inverting horizontal component if the user is red
    protected int makeRelative(int start, int modifier, bool userIsRed) {
        // vertical and horizontal displacement
        int vert = (modifier / 6) * 6;
        int horiz = modifier % 6; // % is actually remainder, not mod, even if it's called mod - negatives are kept properly

        return userIsRed ? vert + horiz + start : vert - horiz + start;
    }

    public enum Type {
        Standard,
        Mega,
        Giga,
        ProgramAdvance,
    };
}
