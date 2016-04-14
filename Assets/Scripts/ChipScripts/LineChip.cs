using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using pairLists;

// class for chips and other attacks that damage the first thing in a line ahead of the player
public class LineChip : AChip {

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {
        print("used by " + (user.isRed ? "red" : "blue"));
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
        print("checking");
        // now check each panel until a hit, starting directly in front of the player
        while (target != end)
        { // do this all in one frame, no yield until end
            if(Controller.gameCore.panels[target].GetComponent<Panel>().isOccupied)
            {
                Controller.gameCore.panels[target].GetComponent<Panel>().occupant.hit(this);
                break; // stop after hit
            } else
            {// if no hit keep looking
                target = isRed ? target + 1 : target - 1;
            }
        }
        yield return 1;
    }

    // Use this for initialization
    void Start() { }
}
