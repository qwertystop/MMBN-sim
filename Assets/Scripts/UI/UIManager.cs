using UnityEngine;
using System.Collections;
using System;
using Util;

public class UIManager : MonoBehaviour {
    public Sprite[] numbersTan = new Sprite[10]; // 0-9
    public Sprite[] numbersGrn = new Sprite[10]; // 0-9
    public Sprite[] numbersRed = new Sprite[10]; // 0-9
    public Sprite[] codes = new Sprite[27]; // *, then A-Z
    public Sprite[] elements = new Sprite[5];// null, fire, aqua, wood, elec, same as order in Controller.Element enum.
    private HUD[] huds = new HUD[2];// left, then right


    // pre-Start initialization
    void Awake() {
        Controller.UI = this;
        huds[0] = gameObject.FindChild("LeftHUD").GetComponent<HUD>();
        huds[0].player = Controller.players[0];
        huds[1] = gameObject.FindChild("RightHUD").GetComponent<HUD>();
        huds[1].player = Controller.players[1];
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    // Overloaded methods to convert data of various types into the icon to display
    
    // For ints, three sets of sprites exist for different colors.
    // if which > 0, green. If which == 0, tan. If which < 0, red. Defaults to tan.
    public Sprite[] getSprite(int target, int which = 0) {
        int count = (int)Mathf.Log10(target);// number of digits minus 1
        Sprite[] s = new Sprite[count+1];// array length equal to number of digits, count starts at highest index in array
        int ind;// index in array of all digits at which correct digit is placed
        for (int i = 0; i < count; target /= 10, --i)// each iteration, decrement i and remove a digit from target
        {
            ind = target % 10;
            // cases ordered by expected frequency of occurrence
            if (which == 0) { s[i] = numbersTan[ind]; }
            else if (which < 0) { s[i] = numbersRed[ind]; }
            else { s[i] = numbersGrn[ind]; }
        }
        return s;
    }

    public Sprite getSprite(char target) {
        if (target == '*') { return codes[0]; }
        else if (target >= 'A' && target <= 'Z') { return codes[target - 'A' + 1]; }
        else { throw new ArgumentOutOfRangeException(target + " is not * or A-Z. Letters must be capital."); }
    }

    public Sprite getSprite(Controller.Element target) {
        return elements[(int)target];
    }
}
