using UnityEngine;
using System;

class Player : MonoBehaviour {

    public int playerNo = 0;
    public int currentPanelIndex = 7;
    public bool redToMove = true;// ie can only move to red squares, the default
    int nextMove = 0; // 0 = none; 1 = up; 2 = down; 3 = left; 4 = right.
    public int moveTimer = -1;
    public int moveCoolDownTime = 9;
    bool justMoved = false;

    void Start() {
      //  transform.position = Controller.gameCore.panels[currentPanelIndex].transform.position;
    }

    void Update() {
        Movement();
        Move();
    }

    // determine direction of movement if moving is possible
    private void Movement() {
        moveTimer--;

        if (moveTimer < 0)
        {
            if (isMovable() && justMoved == false)
            {
                nextMove = Utilities.InputHandler.whichMove(playerNo);
            }

            justMoved = false;
        }
    }

    // actually move
    private void Move() {
        // all the stuff to do to move is exactly the same except for the edge-of-area check and the int
        Utilities.intVoid mover = x =>
        {
            GameObject target = Controller.gameCore.panels[currentPanelIndex + x];
            if (target.GetComponent<Panel>().isRed == redToMove)
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
    private bool isMovable() {
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

    // animates this player across the given list of sprites at a rate of one per frame
    System.Collections.IEnumerator animate(System.Collections.Generic.IList<Sprite> sprites) {
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        foreach (Sprite s in sprites) {
            spriteRenderer.sprite = s;
            yield return 0;
        }
    }
}
