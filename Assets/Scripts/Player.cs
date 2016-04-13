using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public int hp = 1000;
    public Controller.Element element = Controller.Element.Null;
    public int playerNo = 0;
    public int currentPanelIndex = 7;
    public bool isRed { get { return playerNo == 0; } } // left player is red, right is blue
    int nextMove = 0; // 0 = none; 1 = up; 2 = down; 3 = left; 4 = right.
    public int moveTimer = -1;
    public int moveCoolDownTime = 9;
    bool justMoved = false;
    // GameObjects containing the data for busters
    public GameObject busterUncharged;
    public GameObject busterCharged;
    // private references directly to that data
    private LineChip b_nocharge;
    private AChip b_charge;

    // a list of sprites to loop through, at one per frame
    // set it to a different animation to immediately switch, append to the end to queue something up.
    public Sprite[] sprites;
    // the idle animation
    public Sprite[] idleAnim;

    void Start() {
        b_nocharge = Instantiate(busterUncharged).GetComponent<LineChip>();
        //b_charge = Instantiate(busterCharged).GetComponent<AChip>();
        StartCoroutine(animate());
    }

    void Update() {
        Movement();
        Move();
        //TODO refine this - should be a sep. method or multiple like movement is
        if (InputHandler.buttonUp(playerNo, InputHandler.button.B))
        {
            Debug.Log("true for player " + playerNo);
            StartCoroutine(b_nocharge.use(this));
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
        Delegates.intVoid mover = x =>
        {
            GameObject target = Controller.gameCore.panels[currentPanelIndex + x];
            if (target.GetComponent<Panel>().isRed == this.isRed // only move onto proper color
            && !target.GetComponent<Panel>().isOccupied) // can't move onto occupied panel
            {
                MovingNow(true);
                currentPanelIndex = currentPanelIndex + x;
                GetComponentInParent<Transform>().transform.position = target.transform.position;
            }
        };

        switch (nextMove)
        {
            case 1: // up
                if (currentPanelIndex > 6)
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

    // true unless stunned, currently moving, or in the middle of an action
    private bool canAct() {
        return true;//TODO STUB
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
            (chip.damageBase + chip.damagePlus) * chip.damageMultiplier :
            (chip.damageBase + chip.damagePlus) * (chip.damageMultiplier + 1); // +1 to multiplier - stacking doublers is +100%, not x2
        // deduct from HP
        hp -= damage;
    }

    // animates this player across the given list of sprites at a rate of one per frame
    System.Collections.IEnumerator animate() {
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        while(true)
        {
            foreach (Sprite s in sprites)
            {
                spriteRenderer.sprite = s;
                yield return 0;
            }
            // now sprites is empty, reset it to idle animation
            sprites = idleAnim;
        }
    }
}
