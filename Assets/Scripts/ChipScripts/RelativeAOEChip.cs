using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using pairLists;

// class for chips and other attacks that damage fixed positions on the battlefield (relative to the user)
// Example: Sword, Quake
// does not hurt user or user-allies
public class RelativeAOEChip : AChip {
    // all locations and how much damage to do to them
    // relative to player
    public intsAndInts locationsAndDamages;

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {

        // calculate misc values ahead of time to avoid delaying the complicated bit
        bool isRed = user.isRed;
        for (int i = 0; i < locationsAndDamages.locations.Length; ++i)
        {// modify each location to be relative to player
            int loc = locationsAndDamages.locations[i];
            // vertical and horizontal displacement
            int vert = (loc / 6) * 6;
            int horiz = loc % 6; // % is actually remainder, not mod, even if it's called mod - negatives are kept properly
            int final;
            if (isRed)
            {
                final = vert + horiz + user.currentPanelIndex;
            } else
            {
                final = vert - horiz + user.currentPanelIndex;
            }
            locationsAndDamages.locations[i] = final;
        }

        // prevent movement in mid-shot
        // TODO update this when status beyond can/can't move is implemented - don't want to be able to cancel one chip with another
        user.moveTimer = windup + winddown;
        // wait through windup
        for (int i = 0; i <= windup; i++)
        {
            yield return 0;
        }

        // now damage each panel on the list with the given amount of damage
        //TODO prevent checking off-board locations
        for (int i = 0; i < locationsAndDamages.locations.Length; ++i)
        {
            if (locationsAndDamages.locations[i] > -1 && locationsAndDamages.locations[i] < 18 && // panel is on the battlefield
                Controller.gameCore.panels[locationsAndDamages.locations[i]].GetComponent<Panel>().isOccupied)
            {// someone in the panel
                Player p = Controller.gameCore.panels[locationsAndDamages.locations[i]].GetComponent<Panel>().occupant;
                // don't hurt the user
                // TODO when summons are implemented make sure this doesn't hit allies
                if (user != p)
                {
                    p.hit(this);
                }
            }
        }
    }

    // Use this for initialization
    void Start() { }
}