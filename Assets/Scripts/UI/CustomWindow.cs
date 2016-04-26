using UnityEngine;
using System.Collections.Generic;
using Util;
using UnityEngine.UI;

// The CustomWindow: code for chip-selection and for rendering the chips drawn/selected
public class CustomWindow : MonoBehaviour {
    // assorted collections of Chips
    public readonly AChip[] folder = new AChip[30];// all chips for the player - TODO currently must be set in editor, no folder-setup stuff yet.
    private List<AChip> unused = new List<AChip>(30);// the chips not yet used
    private List<AChip> used = new List<AChip>(30);// the chips that have been used - need to be kept for NavRcycl, FoldrBak, etc.
    private AChip[] hand = new AChip[10];// current hand
    private List<int> selected = new List<int>(5);// indices in hand of selected chips

    // parameters of chip selection
    private int handSize = 5;// current Custom
    private int cursorLoc = 0;// current cursor location. 0-9: chip in hand with matching index. 10: OK button. 11: ADD button.
    private int active = 0;// the chip the cursor is currently over, or the last one it was over if currently not over a chip

    // parameters of Image and Text renderers for graphical output
    private Image[] handRenderers = new Image[10];
    private Text[] handCodes = new Text[10];
    private Image[] selectedRenderers = new Image[5];
    private Text activeName;
    private Image activeCard;
    private Image activeCode;
    private Image activeElement;
    private Image[] activeDamage = new Image[3];


    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        // at start, all chips are unused.
        unused.AddRange(folder);
        // place in random order
        // TODO have not implemented Regular or Tag chips
        unused.Shuffle();
    }

    // Initialization after all Awake() methods have run
    void Start () {
        // connect to graphic output objects

        // organizational empty object for hand
        GameObject hand = gameObject.FindChild("Hand");
        for (int i = 0; i < 10; ++i)
        {
            handRenderers[i] = hand.FindChild(i.ToString()).GetComponent<Image>();
        }

        //TODO other graphic output
    }

    // Update is called once per frame
    void Update () {
	
	}
}
