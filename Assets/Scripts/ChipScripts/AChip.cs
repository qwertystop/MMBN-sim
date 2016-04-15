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
    // locations may be relative to location of player, depending on chip type: +/- 1 horiz, +/-6 vert
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
    // will be overridden by inheritors but generally start by calling this
    public virtual IEnumerator use(Player user) {
        // prevent overlapping actions
        StartCoroutine(haltUser(user));
        // animate user
        user.curAnimation = playerAnimation;
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

    // Decorates panels with sprites in this.decorations, flipped by the player's side but otherwise not relative
    // TODO flip according to player's side
    public virtual IEnumerator decorateFixed(Player user) {
        foreach (intsAndSprites frame in decorations)
        {// each frame
            for (int i = 0; i < frame.ints.Length; ++i)
            {// draw all sprites for this frame
                if (frame.ints[i] > -1 && frame.ints[i] < 18)
                {// if panel exists, decorate it
                    Controller.gameCore.panels[frame.ints[i]].GetComponent<Panel>().Decorate(frame.sprites[i], !user.isRed);
                }
            }
            // wait a frame
            yield return 0;
        }
    }

    // Decorates panels with sprites in this.decorations, position relative to the player
    public virtual IEnumerator decorateRelative(Player user) {
        foreach (intsAndSprites frame in decorations)
        {// each frame
            for (int i = 0; i < frame.ints.Length; ++i)
            {// draw all sprites for this frame
                int rel = makeRelative(user, frame.ints[i]);
                if (rel > -1 && rel < 18)
                {// if panel exists, decorate it
                    Controller.gameCore.panels[rel].GetComponent<Panel>().Decorate(frame.sprites[i]);
                }
            }
            // wait a frame
            yield return 0;
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
