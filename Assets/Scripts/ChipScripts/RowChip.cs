using UnityEngine;
using System.Collections;


// class for chips that damage a rectangular area in front of the user evenly (one damage value)
// technically could be implemented with RelativeAOEChip, but this is easier to set in the editor
public class RowChip : AChip {

    public int length = 0; //The depth of the attack
    public int width = 0; //The width of the attack

    // coroutine to run when using this chip
    public override IEnumerator use(Player user)
    {
        decorateRelative(user);
        yield return StartCoroutine(base.use(user));
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
            // now check each panel until a hit, starting directly in front of the player
            while (target != end)
            { // do this all in one frame, no yield until end
                if (Controller.gameCore.panels[target].GetComponent<Panel>().isOccupied)
                {
                    Controller.gameCore.panels[target].GetComponent<Panel>().occupant.hit(this);
                }
            }
            target -= (increment * length);
            target = (target + 6) % 18;
            end = (end + 6) % 18 ;
        }
        yield return 1;
    }
}
