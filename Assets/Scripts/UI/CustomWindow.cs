using UnityEngine;
using System.Collections.Generic;
using Util;
using UnityEngine.UI;

// The CustomWindow: code for chip-selection and for rendering the chips drawn/selected
public class CustomWindow : MonoBehaviour {
    // assorted collections of Chips
    public AChip[] folder = new AChip[30];// all chips for the player
    private List<AChip> unused = new List<AChip>(30);// the chips not yet used
    private List<AChip> used = new List<AChip>(30);// the chips that have been used - need to be kept for NavRcycl, FoldrBak, etc.
    private List<AChip> hand = new List<AChip>(10);// current hand
    private List<int> selected = new List<int>(5);// indices in hand of selected chips

    // parameters of chip selection
    private int handSize = 5;// current Custom
    private int cursorLoc = 0;// current cursor location. 0-9: chip in hand with matching index. 10: ADD button. 11: OK button.
    private int activeIndex = 0;// the index in hand of the chip the cursor is on, or the last one it was on if currently on a button

    // parameters of Image and Text renderers for graphical output
    // hand
    private Image[] handRenderers = new Image[10];
    private Text[] handCodes = new Text[10];
    private Color starCodeColor = new Color(0xCE / 255f, 0x71 / 255f, 0x00 / 255f);
    private Color letterCodeColor = new Color(0xF6 / 255f, 0xEE / 255f, 0x28 / 255f);
    // selected
    private Image[] selectedRenderers = new Image[5];
    // active
    private Text activeName;
    private Image activeCard;
    private Image activeCode;
    private Image activeElement;
    private Image[] activeDamage = new Image[3];
    // buttons and cursor
    private Image add;
    private Image ok;
    private Image cursor;
    private readonly Vector2 cursorSizeOnButton = new Vector2(26, 17);
    private readonly Vector2 cursorSizeOnChip = new Vector2(18, 18);

    // reference to Player
    private Player player;

    /* TODO:
    Folder setup by user (outside editor)
    Regular chips
    Tag chips
    graying chips in hand of wrong code
    */ 

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        // at start, all chips are unused.
        unused.AddRange(folder);
    }

    // Initialization after all Awake() methods have run
    void Start () {
        // connect to graphic output objects
        // organizational empty object for hand
        GameObject handGroup = gameObject.FindChild("Hand");
        for (int i = 0; i < 10; ++i)
        {// find/reference all the Image and Text components in hand, in order
            handRenderers[i] = handGroup.FindChild(i.ToString()).GetComponent<Image>();
            handCodes[i] = handRenderers[i].GetComponentInChildren<Text>();
        }
        // organizational empty object for selected
        GameObject selectedGroup = gameObject.FindChild("Selected");
        for (int i = 0; i < 5; ++i)
        {// find/reference all the Image renderers for selected, in order
            selectedRenderers[i] = selectedGroup.FindChild(i.ToString()).GetComponent<Image>();
        }
        // organizational empty object for active
        GameObject activeGroup = gameObject.FindChild("ActiveChip");
        activeName = activeGroup.FindChild("Name").GetComponent<Text>();
        activeCard = activeGroup.FindChild("Card").GetComponent<Image>();
        activeCode = activeGroup.FindChild("Code").GetComponent<Image>();
        activeElement = activeGroup.FindChild("Element").GetComponent<Image>();
        for (int i = 0; i < 3; ++i)
        {
            activeDamage[i] = activeGroup.FindChild(i.ToString()).GetComponent<Image>();
        }

        // buttons and cursor
        ok = gameObject.FindChild("OK").GetComponent<Image>();
        add = gameObject.FindChild("Add").GetComponent<Image>();
        cursor = gameObject.FindChild("Cursor").GetComponent<Image>();

        // and draw the first hand
        hand.AddRange(Draw(handSize));
    }

    public void Init(Player p) {
        player = p;
    }

    // Update is called once per frame
    void Update () {
        moveCursor();
        if (cursorLoc < handSize)
        {// active index matches cursor location while cursor is over a chip
            activeIndex = cursorLoc;
        }
	}

    // Runs after Update once per frame - ensures that changes here don't get overwritten by UI system
    void LateUpdate() {
        updateHandRenderers();
        updateSelectedRenderers();
        updateActiveRenderers();
        updateCursorRenderer();
    }

    // moves cursor as appropriate for button input and current position
    // (includes wraparound)
    private void moveCursor() {
        int oldLoc = cursorLoc;
        if (InputHandler.buttonDown(player.playerNo, InputHandler.button.UP)) {
            if (10 == cursorLoc) { cursorLoc = 11; }// on ADD, move to OK
            else if (11 == cursorLoc || cursorLoc < 5) { cursorLoc = 10; }// on OK or top row, move to ADD
            else { cursorLoc -= 5; }// on bottom row, move to top row
        } else if (InputHandler.buttonDown(player.playerNo, InputHandler.button.DOWN)) {
            if (10 == cursorLoc) { cursorLoc = (player.playerNo ^ 1) * 4; }// on ADD, move to near end of top row (0 or 4)
            else if (11 == cursorLoc) { cursorLoc = 10; }// on OK, move to ADD
            else if (5 > cursorLoc) { cursorLoc += 5; }// on top row, move to bottom row
            else { cursorLoc -= 5; }// on bottom row, move to top row
        } else if(InputHandler.buttonDown(player.playerNo, InputHandler.button.LEFT)) {
            if (10 == cursorLoc || 11 == cursorLoc) { cursorLoc = 4; }// on OK or ADD, move to right end of top row
            else if (0 == cursorLoc % 5) { cursorLoc = (0 == player.playerNo) ? 4 + cursorLoc : 10; }// p0 moves to opp. end of row, p1 moves to ADD
            else { cursorLoc -= 1; }// somewhere in row other than left end, move left
        } else if (InputHandler.buttonDown(player.playerNo, InputHandler.button.RIGHT)) {
            if (10 == cursorLoc || 11 == cursorLoc) { cursorLoc = 0; }// on OK or ADD, move to left end of top row
            else if (4 == cursorLoc % 5) { cursorLoc = (0 == player.playerNo) ? 10 : cursorLoc - 4; }// p0 moves to ADD, p1 moves to opp. end of row
            else { cursorLoc += 1; }// somewhere in row other than right end, move right
        }
    }
    
    private void updateHandRenderers() {
        for(int i = 0; i < handSize; ++i)
        {// for each chip in hand
            if (selected.Contains(i))
            {// if selected blank it
                handRenderers[i].enabled = false;
            } else
            {// else draw the small icon
                handRenderers[i].enabled = true;
                handRenderers[i].sprite = hand[i].icon;
            }
            // either way draw the code
            char code = hand[i].code;
            if (code == '*' || code == '\u2731')
            {// star is diff. color, and uses Unicode 2731 (HEAVY ASTERISK) for visibility at small size
                // allow data to be regular asterisk for the sake of convenience
                handCodes[i].text = "\u2731";
                handCodes[i].color = starCodeColor;
            } else
            {
                handCodes[i].text = code.ToString();
                handCodes[i].color = letterCodeColor;
            }
        }
        for (int i = handSize; i < 10; ++i)
        {// for each empty space in hand, blank it
            handRenderers[i].enabled = false;
        }
    }

    private void updateSelectedRenderers() {
        for (int i = 0; i < selected.Count; ++i)
        {
            selectedRenderers[i].enabled = true;
            selectedRenderers[i].sprite = hand[selected[i]].icon;
        }
        for (int i = selected.Count; i < 5; ++i)
        {
            selectedRenderers[i].enabled = false;
        }
    }

    private void updateActiveRenderers() {
        AChip active = hand[activeIndex];
        activeName.text = active.chipName;
        activeCard.sprite = active.largeImg;
        activeCode.sprite = Controller.UI.getSprite(active.code);
        activeElement.sprite = Controller.UI.getSprite(active.element);
        Sprite[] dmg = Controller.UI.getSprite(active.damageBase);
        for (int i = 0; i < dmg.Length; ++i)
        {
            activeDamage[i].sprite = dmg[i];
            activeDamage[i].enabled = true;
        }// anything past that is a blank space
        for (int i = dmg.Length; i < 3; ++i)
        {
            activeDamage[i].enabled = false;
        }
    }

    private void updateCursorRenderer() {// update drawn cursor locaction if it changed
        if (cursorLoc > 9)
        {// on either Add or OK, resize to button size and move to that button
            cursor.rectTransform.sizeDelta = cursorSizeOnButton;
            cursor.rectTransform.position = (cursorLoc == 10) ? add.rectTransform.position : ok.rectTransform.position;
        } else
        {// on a chip, resize to chip size and move to that chip
            cursor.rectTransform.sizeDelta = cursorSizeOnChip;
            cursor.rectTransform.position = handRenderers[cursorLoc].rectTransform.position;
        }
    }

    // draw n new chips from the front of unused into new array, after shuffling unused
    // chips are removed from unused
    AChip[] Draw(int n) {
        unused.Shuffle();
        AChip[] c = new AChip[n];
        for (int i = 0; i < n; ++i)
        {
            c[i] = unused[i];
            unused.RemoveAt(i);
        }
        return c;
    }
}
