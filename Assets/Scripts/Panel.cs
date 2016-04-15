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

    // Use this for initialization
    void Start () {
        decorations.Add(Instantiate(blankRenderer));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Displays the given sprite at this panel for one frame
    // Uses object-pooling to reduce instantiation
    // if boolean is true, reverse left-right of sprite
    public void Decorate(Sprite sprite, bool flip) {
        int i = 0;
        while (i < decorations.Count)
        {// look until the end of the list or until finding an empty spot
            if (decorations[i].GetComponent<SpriteRenderer>().sprite == null)
            {// if this is an empty spot
                StartCoroutine(Decorator(sprite, i)); // put the decoration in it
                return;// stop looking
            }// else keep looking
            i += 1;
        }
        // getting here means no empty spot was found and i is one past the last index
        decorations.Add(Instantiate(blankRenderer));
        StartCoroutine(Decorator(sprite, i));
    }

    // Helper for Decorate, handles delayed disappearance
    // int is index in decorations to hold sprite
    private IEnumerator Decorator(Sprite sprite, int i) {
        decorations[i].transform.position = transform.position;
        decorations[i].GetComponent<SpriteRenderer>().sprite = sprite;
        yield return 0; // the next frame
        decorations[i].GetComponent<SpriteRenderer>().sprite = null;
    }
    
}
