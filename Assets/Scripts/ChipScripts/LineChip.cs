using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mDimLists;

// class for non-piercing linear hitscan chips
// may have AOE centered around hit. Same damage to all hit locations.
// Example: Cannon, the default MegaBuster, Spreader
// Close-but-not-quite nonexample: Shockwave, ZapRing, DollThunder
public class LineChip : AChip {
    [SerializeField] private int[] aoe = new int[1];// locations relative to target
    [SerializeField] private Animation2D hitBurst;// animation to play on each square of aoe

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {
        decorateRelative(user.currentPanelIndex, user.isRed);
        yield return StartCoroutine(base.use(user));
        // calculate misc values ahead of time to avoid delaying the complicated bit
        // determine direction
        bool isRed = user.isRed;
        // determine row - int is the end condition for off the red end, math to convert to blue end is simple
        int row = user.currentPanelIndex < 6 ? 6 :
                                               user.currentPanelIndex < 12 ? 12 : 18;
        int end = isRed ? row : row - 7;// 6 -> -1, 12 -> 5, 18 -> 11; 1 past the end of the line in the other direction
        int target = isRed ? user.currentPanelIndex + 1 : user.currentPanelIndex - 1;

        // prevent movement in mid-shot
        // TODO update this when status beyond can/can't move is implemented - don't want to be able to cancel one chip with another
        user.moveTimer = windup + winddown;
        // wait through windup
        for (int i = 0; i <= windup; i++)
        {
            yield return 0;
        }
        // now check each panel until a hit, starting directly in front of the player
        while (target != end)
        { // do this all in one frame, no yield until end
            if(Controller.gameCore.panels[target].GetComponent<Panel>().isOccupied)
            {
                // hit and decorate all panels around target
                indexedAnimation2D[] splash = new indexedAnimation2D[aoe.Length];
                for(int i = 0; i < aoe.Length; ++i)
                {// panel number
                    int t = makeRelative(target, aoe[i], isRed);
                    // check that panel exists
                    if (t > -1 && t < 18)
                    {
                        // ready decoration for it
                        splash[i] = new indexedAnimation2D(t, hitBurst);
                        // cache panel
                        Panel p = Controller.gameCore.panels[t].GetComponent<Panel>();
                        if (p.isOccupied)
                        {// hit panel
                            p.occupant.hit(damageBase + damagePlus, damageMultiplier, element);
                        }
                    }
                }
                // decorate all panels - make sure it doesn't flip, because that calculation was already done above
                decorateFixed(true, splash);
                break; // stop after hitscan finds something
            } else
            {// if no hit keep looking
                target = isRed ? target + 1 : target - 1;
            }
        }
        yield return 1;
    }
}
