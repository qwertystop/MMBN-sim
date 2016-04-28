using UnityEngine;
using System;
using System.Collections.Generic;

// the player character
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

    // Customizable properties of player character
    public int maxHP = 1000;
    public int curHP;
    public Controller.Element element = Controller.Element.Null;

    // Buster
    public GameObject busterUncharged;
    public GameObject busterCharged;
    // private references to the scripts for the buster - GameObject usage is just for editor handles
    private LineChip b_nocharge;
    private AChip b_charge;
    private int chargeCounter = 0;
    private int chargeTime = 60;

    // Chips currently loaded
    public Queue<AChip> chipsPicked = new Queue<AChip>(5);
    // all actual chip-selection code is done in the CustomWindow class, with the results passed to here
    
    // Animation
    private Animation2D curAnimation;
    // the idle animation
    private Animation2D idleAnim;
    // buster charging animations
    private Animation2D chargingAnim;
    private Animation2D chargedAnim;

    /* TODO
     Modify Chip() for chip-charging abilities
    */


    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        // HP
        curHP = maxHP;
        // instantiate buster
        b_nocharge = Instantiate(busterUncharged).GetComponent<LineChip>();
        b_charge = Instantiate(busterCharged).GetComponent<AChip>();
        // animation
        animationSetup();
        StartCoroutine(animateReset());
    }

    // Initialization after all Awake() methods have run
    void Start() {
        // panel is occupied
        Controller.gameCore.panels[currentPanelIndex].GetComponent<Panel>().occupant = this;
    }

    // fills fields with appropriate Animation2D objects for later reference
    private void animationSetup() {
        // ready animations
        Animation2D[] allAnimations = GetComponentsInChildren<Animation2D>();
        foreach (Animation2D a in allAnimations)
        {
            if (a.gameObject == this.gameObject)
            {// animations on this
                if (a.name.Equals("Idle"))
                {
                    idleAnim = a;
                    curAnimation = a;
                }
            } else
            {// animations on children of this
                if (a.name.Equals("BusterCharging"))
                {
                    chargingAnim = a;
                } else if (a.name.Equals("BusterCharged"))
                {
                    chargedAnim = a;
                }
            }
        }
    }

    void Update() {
        curHP = Math.Max(Math.Min(curHP, maxHP), 0);
        if (!Controller.paused)
        {
            curAnimation.paused = false;
            Movement();
            Move();
            Buster();
            Chip();
        } else
        {
            curAnimation.paused = true;
        }
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
        Util.intVoid mover = x =>
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
            // on button down, delay charge counter
            if (InputHandler.buttonDown(playerNo, InputHandler.button.B))
            {
                chargeCounter = -6;// delayed to ensure button is actually held rather than tapped before animating
            }

            // on holding button, charge
            if (InputHandler.buttonHeld(playerNo, InputHandler.button.B))
            {
                // if just started charging, start charge animation
                if (chargeCounter == 1)
                {
                    chargingAnim.Play(true);
                }

                // increment charge counter
                chargeCounter += 1;
                
                // if just became charged, stop charging animation and start charged animation
                if (chargeCounter == chargeTime)
                {
                    chargingAnim.Stop();
                    chargedAnim.Play();
                }
            }

            // on releasing button, shoot (depending on charge level), reset charge
            // TODO when non-basic busters are added, will need to add decorating to charge shot code
            if (InputHandler.buttonUp(playerNo, InputHandler.button.B))
            {
                if (chargeCounter >= chargeTime)
                {// if charged
                    chargedAnim.Stop();
                    StartCoroutine(b_charge.use(this));
                } else
                {// not charged
                    chargingAnim.Stop();
                    StartCoroutine(b_nocharge.use(this));
                }
                chargeCounter = 0;
            }
        }
    }

    // Handles Chip use
    private void Chip() {
        if (canAct() && InputHandler.buttonDown(playerNo, InputHandler.button.A))
        {
            if (chipsPicked.Count != 0)
            {
                StartCoroutine(chipsPicked.Dequeue().use(this));
            }
        }
    }

    // true unless stunned, in the middle of an action, etc
    private bool canAct() {
        return status == Status.FREE;
    }

    // resets movement timer if given is true, resets next move either way
    private void MovingNow(bool possible) {
        if (possible)
        {
            moveTimer = moveCoolDownTime;
            justMoved = true;
        }
        nextMove = 0;
    }

    // hit this player with the given Chip
    public void hit(int damage, int multiplier, Controller.Element atkElement) {
        // calculate damage
        int totalDamage = Controller.isSuper(atkElement, element) ?
            damage * (multiplier + 1) :// +1 to multiplier - stacking doublers is +100%, not x2
            damage * multiplier;

        // deduct from HP, but don't go below 0
        // healing is negative damage - don't go above max either
        curHP -= totalDamage;
        curHP = curHP < 0 ? 0 : (curHP > maxHP ? maxHP : curHP);
    }

    // This player plays the given animation
    public void Animate(Animation2D a) {
        curAnimation.Stop();
        curAnimation = a;
        a.outputRenderer = GetComponent<SpriteRenderer>();
        curAnimation.Play(true);
    }

    // resets animation to idle whenever no other animation is playing
    // main purpose is to ensure that the animation is always set to something, so stopping it doesn't null-pointer
    private System.Collections.IEnumerator animateReset() {
        while(true)
        {
            if (curAnimation == null || curAnimation.isStopped)
            {// whenever it stops or isn't set, reset to idle
                curAnimation = idleAnim;
                curAnimation.Play(true);
            }
            yield return 0;
        }
    }

    // status conditions
    // will add more as causes are implemented
    public enum Status {
        FREE,
        ACTING
    }
}
