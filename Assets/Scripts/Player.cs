using UnityEngine;
using System.Collections;
using System;

class Player : MonoBehaviour {

    public int playerNo = 0;
    public int currentPanelIndex = 7;
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
        switch (nextMove)
        {
            case 1: // up
                if (currentPanelIndex > 6)
                {
                    MovingNow(true);
                    currentPanelIndex = currentPanelIndex - 6;
                    GetComponentInParent<Transform>().transform.position = Controller.gameCore.panels[currentPanelIndex].transform.position;
                }
                break;
            case 2: // down
                if (currentPanelIndex < 12)
                {
                    MovingNow(true);
                    currentPanelIndex = currentPanelIndex + 6;
                    GetComponentInParent<Transform>().transform.position = Controller.gameCore.panels[currentPanelIndex].transform.position;
                }
                break;
            case 3: // left
                if ((currentPanelIndex % 6) > 0)
                {
                    MovingNow(true);
                    currentPanelIndex = currentPanelIndex - 1;
                    GetComponentInParent<Transform>().transform.position = Controller.gameCore.panels[currentPanelIndex].transform.position;
                }
                break;
            case 4: // right
                if ((currentPanelIndex % 6) < 5)
                {
                    MovingNow(true);
                    currentPanelIndex = currentPanelIndex + 1;
                    GetComponentInParent<Transform>().transform.position = Controller.gameCore.panels[currentPanelIndex].transform.position;
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
}
