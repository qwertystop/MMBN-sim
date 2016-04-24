using UnityEngine;
using System.Collections;

public class RowChip : AChip {

    public int length = 0; //The depth of the attack
    public int width = 0; //The width of the attack

    // coroutine to run when using this chip
    public override IEnumerator use(Player user)
    {
        StartCoroutine(decorateRelative(user));
        yield return StartCoroutine(base.use(user));
        // calculate misc values ahead of time to avoid delaying the complicated bit
        // determine direction
        bool isRed = user.isRed;
        // determine row - int is the end condition for off the red end, math to convert to blue end is simple
        int row = user.currentPanelIndex < 6 ? 6 :
                                               user.currentPanelIndex < 12 ? 12 : 18;
        int target = isRed ? user.currentPanelIndex + 1 : user.currentPanelIndex - 1;
        int end = isRed ? target + (length % 4) : target - (length % 4);//calculate the end based on the inputted depth
       
        int increment = isRed ? 1 : -1;

        for (int i = 0; i < width; i++)
        {
            bool didBreak = false;
            // now check each panel until a hit, starting directly in front of the player
            while (target != end)
            { // do this all in one frame, no yield until end
                if (Controller.gameCore.panels[target].GetComponent<Panel>().isOccupied)
                {
                    Controller.gameCore.panels[target].GetComponent<Panel>().occupant.hit(this);
                    didBreak = true;
                    break; // stop after hit
                }
                else
                {// if no hit keep looking
                    target = isRed ? target + 1 : target - 1;
                }
            }
            if (didBreak) {
                break;
            }
            target -= (increment * length);
            target = (target + 6) % 18;
            end = (end + 6) % 18 ;
        }
        yield return 1;
    }
}
