using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using pairLists;

// class for chips and other attacks that damage fixed positions on the battlefield (relative to the user)
// Example: Quake
// does not hurt user or user-allies
public class RelativeAOEChip : AChip {
    // all locations and how much damage to do to them
    // relative to player
    public intsAndInts locationsAndDamages;

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {
        StartCoroutine(decorateRelative(user));
        // prevent acting, animate user, wait through windup
        yield return StartCoroutine(base.use(user));


        bool isRed = user.isRed;
        for (int i = 0; i < locationsAndDamages.locations.Length; ++i)
        {// modify each location to be relative to player
            locationsAndDamages.locations[i] = makeRelative(user, locationsAndDamages.locations[i]);
        }

        // now damage each panel on the list with the given amount of damage
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
}