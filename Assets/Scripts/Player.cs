using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    // Basic system fields
    public int playerNo = 0;
    public int currentPanelIndex = 7;
    public bool isRed { get { return playerNo == 0; } } // left player is red, right is blue
    int nextMove = 0; // 0 = none; 1 = up; 2 = down; 3 = left; 4 = right.
    public int moveTimer = -1;
    public int moveCoolDownTime = 9;
    bool justMoved = false;
    public Status status = Status.FREE;

    // Customizable properties
    public int hp = 1000;
    public Controller.Element element = Controller.Element.Null;

    // Buster
    public GameObject busterUncharged;
    public GameObject busterCharged;
    // private references to the scripts for the buster - GameObject usage is just for editor handles
    private LineChip b_nocharge;
    private AChip b_charge;
    private int chargeCounter = 0;
    private int chargeTime = 60;

    // Chips
    public List<GameObject> chipsSelected;// for editor access - will probably not be necessary once chip selection is implemented
    private Queue<AChip> chips = new Queue<AChip>(5);

    // Animation
    // a list of sprites to loop through, at one per frame
    // set it to a different animation to immediately switch, append to the end to queue something up.
    private List<Sprite> sprites = new List<Sprite>();
    // access for sprites - resets count on setting new animation
    public List<Sprite> curAnimation
    {
        get { return sprites; }
        set { sprites = value; animationCount = 0; }
    }
    private int animationCount;
    // the idle animation
    public List<Sprite> idleAnim;
    // buster charging animations
    public List<Sprite> chargingAnim;
    public List<Sprite> chargedAnim;
    public List<Sprite> chargeReleaseAnim;

    void Start() {
        // instantiate buster
        b_nocharge = Instantiate(busterUncharged).GetComponent<LineChip>();
        b_charge = Instantiate(busterCharged).GetComponent<AChip>();
        // ready animation
        StartCoroutine(animate());
        // panel is occupied
        Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().occupant = this;
        // instantiate starting chips
        // TODO when chip selection is implemented move this bit to there
        foreach(GameObject chip in chipsSelected)
        {
            chips.Enqueue(Instantiate(chip).GetComponent<AChip>());
        }
    }

    void Update() {
        Movement();
        Move();
        Buster();
        Chip();
    }

    // determine direction of movement if moving is possible
    private void Movement() {
        moveTimer--;

        if (moveTimer < 0)
        {
            if (canAct() && justMoved == false)
            {
                nextMove = InputHandler.whichMove(playerNo);
            } else
            {
                justMoved = false;
            }

        }
    }

    // actually move
    private void Move() {
        // all the stuff to do to move is exactly the same except for the edge-of-area check and the int
        Delegates.intVoid mover = x =>
        {
            GameObject target = Controller.gameCore.panels[currentPanelIndex + x];
            if (target.GetComponent<Panel>().isRed == this.isRed // only move onto proper color
            && !target.GetComponent<Panel>().isOccupied) // can't move onto occupied panel
            {
                MovingNow(true);
                // remove this from current panel
                int prevPanelIndex = int.Parse(currentPanelIndex.ToString());
                Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().occupant = null;
                // move to new panel
                currentPanelIndex = currentPanelIndex + x;
                GetComponentInParent<Transform>().transform.position = target.transform.position;
                //target.GetComponent<Panel>().occupant = this;
                Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().occupant = this;
                Controller.gameCore.panels[prevPanelIndex].GetComponent<Panel>().occupant = null;
            }
        };

        switch (nextMove)
        {
            case 1: // up
                if (currentPanelIndex > 5)
                {
                    mover(-6);
                }
                break;
            case 2: // down
                if (currentPanelIndex < 12)
                {
                    mover(6);
                }
                break;
            case 3: // left
                if ((currentPanelIndex % 6) > 0)
                {
                    mover(-1);
                }
                break;
            case 4: // right
                if ((currentPanelIndex % 6) < 5)
                {
                    mover(1);
                }
                break;
        }// default is do nothing
    }

    // Handles Buster use
    private void Buster() {
        if (canAct())
        {
            // on holding button, charge
            if (InputHandler.buttonHeld(playerNo, InputHandler.button.B))
            {
                chargeCounter += 1;
                if (chargeCounter >= chargeTime)
                {// charged
                 // display the charging sprite on this panel
                    Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().Decorate(chargedAnim[chargeCounter % chargedAnim.Count]);
                } else
                {// charging
                 // display the charged sprite on this panel
                    Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().Decorate(chargingAnim[chargeCounter % chargingAnim.Count]);
                }
            }

            // on releasing button, shoot (depending on charge level), reset charge
            // TODO when non-basic busters are added, will need to add decorating to charge shot code
            if (InputHandler.buttonUp(playerNo, InputHandler.button.B))
            {
                if (chargeCounter >= chargeTime)
                {// if charged
                   // sprites = new List<Sprite>(b_charge.playerAnimation);
                    //animationCount = 0;
                    StartCoroutine(b_charge.use(this));
                } else
                {
                //    sprites = new List<Sprite>(b_nocharge.playerAnimation);
                  //  animationCount = 0;
                    StartCoroutine(b_nocharge.use(this));
                }
                chargeCounter = 0;
            }
        }
    }

    // Handles Chip use
    // TODO will need to be modified when chip-charging abilities are added
    private void Chip() {
        if (canAct() && InputHandler.buttonDown(playerNo, InputHandler.button.A))
        {
            if (chips.Count != 0)
            {
                sprites = new List<Sprite>(chips.Peek().playerAnimation);
                animationCount = 0;
                StartCoroutine(chips.Dequeue().use(this));
            }
        }
    }

    // true unless stunned, in the middle of an action, etc
    private bool canAct() {
        return status == Status.FREE;
    }

    // resets movement timer if given is true, resets next move either way
    void MovingNow(bool possible) {
        if (possible)
        {
            moveTimer = moveCoolDownTime;
            justMoved = true;
        }
        nextMove = 0;
    }

    // hit this player with the given Chip
    public void hit(AChip chip) {
        // calculate damage
        int damage = Controller.isSuper(chip.element, element) ?
            (chip.damageBase + chip.damagePlus) * (chip.damageMultiplier + 1) : // +1 to multiplier - stacking doublers is +100%, not x2
            (chip.damageBase + chip.damagePlus) * chip.damageMultiplier;
        // deduct from HP
        hp -= damage;
    }

    // animates this player across the given list of sprites at a rate of one per frame
    System.Collections.IEnumerator animate() {
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        while(true)
        {
            while (animationCount < sprites.Count)
            {
                spriteRenderer.sprite = sprites[animationCount];
                ++animationCount;
                yield return 0;
            }
            // now sprites is empty, reset it to idle animation
            sprites = idleAnim;
            animationCount = 0;
        }
    }

    // status conditions
    // will add more as causes are implemented
    public enum Status {
        FREE,
        ACTING
    }
}
