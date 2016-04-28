using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour {
    // system fields - location, type, contents
    public int index;
    public bool isRed = true;
    public Player occupant = null;
    public bool isOccupied { get { return occupant != null; } }

    // sprites for this panel
    public Sprite normalRed;
    public Sprite crackedRed;
    public Sprite normalBlue;
    public Sprite crackedBlue;
    public Sprite missing;

    // object pool for decorations on panels
    private static List<GameObject> decorations = new List<GameObject>();
    // should be set in editor to a GameObject with an empty SpriteRenderer and nothing else
    public GameObject blankRenderer;
    // should be set to an empty GameObject, for in-editor organizational purposes
    public GameObject decorationParent;

    // Initialization not requiring other objects (except those set in editor)
    void Awake () {
        // each panel starts off creating one blank decoration, which ought to be plenty
        addDecoration();
	}

    // Displays the given animation at this panel
    // Uses object-pooling to reduce instantiation
    // if boolean is true, reverse left-right of sprite
    public void Decorate(Animation2D anim, bool flip) {
        for (int i = 0; i < decorations.Count; ++i)
        {// look until the end of the list or until finding an empty spot
            if (decorations[i].GetComponent<SpriteRenderer>().enabled == false)
            {// if this is an empty spot
                Decorator(anim, i, flip); // put the decoration in it
                return;// and stop looking
            }// else keep looking
        }
        // getting here means no empty spot was found, so add a new decoration
        // very unlikely to be used, but it's better to be safe
        addDecoration();
        Decorator(anim, decorations.Count - 1, flip);
    }

    // adds a new decoration to the end of decorations
    private void addDecoration() {
        GameObject dec = Instantiate(blankRenderer);
        dec.transform.parent = decorationParent.transform;
        dec.GetComponent<SpriteRenderer>().enabled = false;
        decorations.Add(dec);
    }

    // Helper for Decorate
    // starts the given animation at this panel using the decorator at the given index
    private void Decorator(Animation2D anim, int i, bool flip) {
        GameObject dec = decorations[i];
        dec.transform.position = this.transform.position;
        
        anim.outputRenderer = dec.GetComponent<SpriteRenderer>();
        if (flip)
        {
            dec.transform.localScale = new Vector3(-5, 5, 1);
        } else
        {
            dec.transform.localScale = new Vector3(5, 5, 1);
        }
        anim.Play(true);
        StartCoroutine(pauseCheck(anim));
    }

    // While the animation is still running, pauses and unpauses it as necessary
    private IEnumerator pauseCheck(Animation2D anim) {
        while (!anim.isStopped)
        {
            anim.paused = Controller.paused;
            yield return 0;
        }
    }
    
}
