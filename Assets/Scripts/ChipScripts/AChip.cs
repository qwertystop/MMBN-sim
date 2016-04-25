using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mDimLists;

// abstract class for chips and other attacks
// allows delay before and after acting
public abstract class AChip : MonoBehaviour {
    // chip element
    public Controller.Element element;
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

    // Decorates panels with animations in this.decorations, flipped by the player's side but otherwise not relative
    public virtual void decorateFixed(Player user) {
        foreach (indexed<Animation2D> anim in decorations)
        {// for each animation
            // if the panel it's for exists, decorate that panel with it
            if (anim.index > -1 && anim.index < 18)
            {
                Controller.gameCore.panels[anim.index].GetComponent<Panel>().Decorate(anim.value, !user.isRed);
            }
        }
    }

    // Decorates panels with sprites in this.decorations, position relative to the player
    public virtual void decorateRelative(Player user) {
        foreach (indexed<Animation2D> anim in decorations)
        {// make each one relative
            int rel = makeRelative(user, anim.index);
            if (rel > -1 && rel < 18)
            {// then, if panel exists, decorate it
                Controller.gameCore.panels[rel].GetComponent<Panel>().Decorate(anim.value, !user.isRed);
            }
        }
    }

    // given an int for a panel index and a player, treat the index as a modifier on the player's location
    // and return the modified location
    protected int makeRelative(Player user, int index) {
        // vertical and horizontal displacement
        int vert = (index / 6) * 6;
        int horiz = index % 6; // % is actually remainder, not mod, even if it's called mod - negatives are kept properly

        return user.isRed ? vert + horiz + user.currentPanelIndex : vert - horiz + user.currentPanelIndex;
    }

    public enum Type {
        Standard,
        Mega,
        Giga,
        ProgramAdvance,
    };
}
