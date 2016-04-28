using UnityEngine;
using System.Collections.Generic;
using Util;
using UnityEngine.UI;
using System;

// The CustomWindow: code for chip-selection and for rendering the chips drawn/selected
public class CustomWindow : MonoBehaviour {
    // assorted collections of Chips
    [SerializeField]
    private AChip[] folder;// all chips for the player
    private List<AChip> unused = new List<AChip>(30);// the chips not yet used
    private List<AChip> hand = new List<AChip>(10);// current hand
    private List<int> selected = new List<int>(5);// indices in hand of selected chips

    // parameters of chip selection
    private int handSize = 5;// current Custom
    private int cursorLoc = 0;// current cursor location. 0-9: chip in hand with matching index. 10: ADD button. 11: OK button.
    private int activeIndex = 0;// the index in hand of the chip the cursor is on, or the last one it was on if currently on a button
    // to select a chip, it must have either the same code or same name as all currently selected chips.
    // * is valid with any one other code (not multiple codes: Cannon A and Cannon B selected means only more Cannons can be selected)
    private char selectedCode = '*';// Letter code of all selected chips. '\0' for no common code.
    private string selectedName = string.Empty;// Name of all selected chips. Empty for no name yet (chips not selected). null for no common name.

    // parameters of Image and Text renderers for graphical output
    // hand
    private Image[] handRenderers = new Image[10];
    private Text[] handCodes = new Text[10];
    private Color starCodeColor = new Color(0xCE / 255f, 0x71 / 255f, 0x00 / 255f);// a sort of dull orange
    private Color letterCodeColor = new Color(0xF6 / 255f, 0xEE / 255f, 0x28 / 255f);// the correct shade of yellow
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

    // reference to Player and HUD
    private Player player;
    private HUD hud;

    /* TODO:
    actual chip selection/passing to player
    Folder setup by user (outside editor)
    Regular chips
    Tag chips
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
    }

    public void Init(Player p, HUD h) {
        player = p;
        hud = h;
    }

    // Update is called once per frame
    void Update () {
        if (Controller.paused && !hud.ready)
        {
            moveCursor();
            select();
            deselect();
        }
	}

    // Runs after Update once per frame - ensures that changes here don't get overwritten by UI system
    void LateUpdate() {
        if (Controller.paused && !hud.ready)
        {
            updateHandRenderers();
            updateSelectedRenderers();
            updateActiveRenderers();
            updateCursorRenderer();
        }
    }

    // last step of starting turn - attempts to check gamestate and throws exception if called at wrong time
    public void finalStartTurn() {
        if (Controller.paused)
        {// reset cursor location
            cursorLoc = 0;
            // draw hand back up to capacity
            hand.AddRange(Draw(handSize - hand.Count));
            // reset selection conditions
            updateSelectConditions();
        } else { throw new Exception("Called out of order"); }
    }

    // confirm selection on either OK or ADD: Loads or discards chips as appropriate, then passes confirmation up to HUD
    private void confirmTurn() {
        if (10 == cursorLoc)
        {// ADD
            // hand size increases for future turns by number discarded
            handSize = Mathf.Min(10, selected.Count + handSize);
            foreach (int i in selected)
            {// discard them
                // not added to used
                hand.RemoveAt(i);
            }
            // clear leftovers from prev. turn
            player.chipsPicked.Clear();
        }
        else
        {// OK
            // send selected chips to player if there are any, overwriting whatever might be left over
            if (selected.Count != 0)
            {// clear old loaded chips
                player.chipsPicked.Clear();
                // keep count of number moved to allow index adjustment
                // can't use for or for-each loop because hand has to be modified
                while (selected.Count != 0)
                {
                    int i = selected[0];
                    // send chip to player
                    hand[i] = Instantiate(hand[i].gameObject).GetComponent<AChip>();
                    player.chipsPicked.Enqueue(hand[i]);
                    // remove from hand and selected
                    hand.RemoveAt(i);
                    selected.RemoveAt(0);
                    // adjust indices to account for removal
                    for (int n = 0; n < selected.Count; ++n) {
                        if (selected[n] > i) { selected[n] -= 1; }
                    }
                }
            }// but don't overwrite something with nothing
        }
        hud.closeCustom();
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
        if (cursorLoc < 9)
        {// active index matches cursor location while cursor is over the hand
            activeIndex = cursorLoc;
        }
    }
    
    // handles A presses in custom screen
    private void select() {
        if (InputHandler.buttonDown(player.playerNo, InputHandler.button.A)) {
            if (cursorLoc > 9)
            {// if on OK or ADD goto confirmTurn()
                confirmTurn();
            } else
            {// if on a chip
                if (cursorLoc < hand.Count && canSelect(hand[cursorLoc]))
                {// see if it's valid
                    selected.Add(cursorLoc);
                    // and update the select conditions
                    updateSelectConditions();
                }
            }
        }
    }

    // handles B button presses in custom screen
    private void deselect() {
        if (InputHandler.buttonDown(player.playerNo, InputHandler.button.B))
        {
            if (0 != selected.Count)
            {// if there are chips selected deselect the last one
                selected.RemoveAt(selected.Count - 1);
                // and update the selection conditions
                updateSelectConditions();
            }
        }
    }

    // updates selectedCode and selectedName
    private void updateSelectConditions() {
        // accumulate common properties (code, name) of chips
        // first reset them
        selectedCode = '*';
        selectedName = string.Empty;
        foreach (int i in selected)
        {// accumulate code 
            if ('*' == selectedCode)
            {// update code to the next selected chip
                selectedCode = hand[i].code;
            } else if (hand[i].code != selectedCode && hand[i].code != '*')
            {// codes not equal, new code not star, no valid code (condition must be name)
                selectedCode = '\0';
            }// else same code, no change to code
            // accumulate name
            if (string.Empty == selectedName)
            {// no name yet, get name of current chip
                selectedName = hand[i].chipName;
            } else if (hand[i].chipName != selectedName)
            {// names not the same, no valid name
                selectedName = null;
            }// else name is the same, leave unchanged
        }
    }

    // Can a given chip be selected?
    private bool canSelect(AChip c) {
        return// check quantity
        selected.Count < 5 && 
        // check code
        (c.code == selectedCode || '*' == selectedCode || ('*' == c.code && selectedCode != '\0') ||
        // code did not pass, check name
        c.chipName == selectedName);
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
                // color depending on whether it can be selected
                handRenderers[i].color = canSelect(hand[i]) ? Color.white : Color.gray;
            }
            // either way draw the code
            char code = hand[i].code;
            if (code == '*')
            {// star is diff. color, and uses Unicode 2731 (HEAVY ASTERISK) for visibility at small size
                // actual data should be regular asterisk for the sake of convenience
                handCodes[i].text = "\u2731";
                handCodes[i].color = starCodeColor;
            } else
            {
                handCodes[i].text = code.ToString();
                handCodes[i].color = letterCodeColor;
            }
        }
        for (int i = handSize; i < 10; ++i)
        {// beyond end of hand, all blank
            handRenderers[i].enabled = false;
            handCodes[i].enabled = false;
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
        if (activeIndex < hand.Count)
        {// if active chip is an actual chip, all the displays show it
            AChip active = hand[activeIndex];
            activeName.text = active.chipName;
            activeName.enabled = true;
            activeCard.sprite = active.largeImg;
            activeCard.enabled = true;
            activeCode.sprite = Controller.UI.getSprite(active.code);
            activeCode.enabled = true;
            activeElement.sprite = Controller.UI.getSprite(active.element);
            activeElement.enabled = true;
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
        } else
        {// if not, all the displays are blanked
            activeName.enabled = false;
            activeCard.enabled = false;
            activeCode.enabled = false;
            activeElement.enabled = false;
            for (int i = 0; i < activeDamage.Length; ++i)
            {
                activeDamage[i].enabled = false;
            }
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
        AChip[] c = new AChip[n];
        if (0 == n) { return c; }
        unused.Shuffle();
        for (int i = 0; i < n; ++i)
        {
            c[i] = unused[i];
            unused.RemoveAt(i);
        }
        return c;
    }
}
