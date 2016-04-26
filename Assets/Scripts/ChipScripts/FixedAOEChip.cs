using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mDimLists;

// class for chips and other attacks that damage fixed positions on the battlefield (user location irrelevant)
// TODO flip according to player's team - nothing ignores the player completely 
// Example: Lance
// does not hurt user or user-allies
public class FixedAOEChip : AChip {
    // all locations and how much damage to do to them
    public indexedInt[] locationsAndDamages;

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {
        decorateFixed(user);
        yield return StartCoroutine(base.use(user));

        // now damage each panel on the list with the given amount of damage
        for (int i = 0; i < locationsAndDamages.Length; ++i)
        {
            int adjustedIndex = mirrorIfBlue(user, locationsAndDamages[i].index);

            if (Controller.gameCore.panels[adjustedIndex].GetComponent<Panel>().isOccupied)
            {// someone in the panel
                Player p = Controller.gameCore.panels[adjustedIndex].GetComponent<Panel>().occupant;
                // don't hurt the user
                // TODO when summons are implemented make sure this doesn't hit allies
                if (user != p)
                {
                    p.hit(locationsAndDamages[i].value+damagePlus, damageMultiplier, element);
                }
            }
        }
    }
}