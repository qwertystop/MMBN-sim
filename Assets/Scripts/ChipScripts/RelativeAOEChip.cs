﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mDimLists;

// class for chips and other attacks that damage fixed positions on the battlefield (relative to the user)
// Example: Quake
// does not hurt user or user-allies
public class RelativeAOEChip : AChip {
    // all locations and how much damage to do to them
    // relative to player
    public indexedInt[] locationsAndDamages;

    // coroutine to run when using this chip
    public override IEnumerator use(Player user) {
        decorateRelative(user);
        // prevent acting, animate user, wait through windup
        yield return StartCoroutine(base.use(user));
        
        bool isRed = user.isRed;
        for (int i = 0; i < locationsAndDamages.Length; ++i)
        {// modify each location to be relative to player
            indexedInt tmp = new indexedInt();
            tmp.index = makeRelative(user, locationsAndDamages[i].index);
            tmp.value = locationsAndDamages[i].value;
            locationsAndDamages[i] = tmp;
        }

        // now damage each panel on the list with the given amount of damage
        for (int i = 0; i < locationsAndDamages.Length; ++i)
        {
            if (locationsAndDamages[i].index > -1 && locationsAndDamages[i].index < 18 && // panel is on the battlefield
                Controller.gameCore.panels[locationsAndDamages[i].index].GetComponent<Panel>().isOccupied)
            {// someone in the panel
                Player p = Controller.gameCore.panels[locationsAndDamages[i].index].GetComponent<Panel>().occupant;
                // don't hurt the user
                // TODO when summons are implemented make sure this doesn't hit allies
                if (user != p)
                {// TODO currently does not properly use all damages, just the one main value
                    p.hit(this);
                }
            }
        }
    }
}