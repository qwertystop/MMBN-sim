using UnityEngine;
using System.Collections;
using System;
using Util;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private Sprite[] numbersTan = new Sprite[10]; // 0-9
    [SerializeField]
    private Sprite[] numbersGrn = new Sprite[10]; // 0-9
    [SerializeField]
    private Sprite[] numbersRed = new Sprite[10]; // 0-9
    [SerializeField]
    private Sprite[] codes = new Sprite[27]; // *, then A-Z
    [SerializeField]
    private Sprite[] elements = new Sprite[5];// null, fire, aqua, wood, elec, same as order in Controller.Element enum.
    private HUD[] huds = new HUD[2];// left, then right

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        Controller.UI = this;
    }

    // Initialization after all Awake() methods have run
    void Start() {
    }

    // Initialization that depends on specific external things having initialized
    // requires player references passed in from Controller
    public void Init(Player p0, Player p1) {
        huds[0] = gameObject.FindChild("LeftHUD").GetComponent<HUD>();
        huds[0].Init(p0);
        huds[1] = gameObject.FindChild("RightHUD").GetComponent<HUD>();
        huds[1].Init(p1);
    }

    // Update is called once per frame
    void Update() {

    }

    // Overloaded methods to convert data of various types into the icon to display
    
    // For ints, three sets of sprites exist for different colors.
    // if which > 0, green. If which == 0, tan. If which < 0, red. Defaults to tan.
    // increasing index is increasing significance of bit
    public Sprite[] getSprite(int target, int which = 0) {
        int count = (int)Mathf.Log10(target) + 1;// number of digits
        Sprite[] s = new Sprite[count];// array length equal to number of digits
        int ind;// index in array of all digits at which correct digit is located
        for (int i = 0; i < count; target /= 10, ++i)// each iteration, increment i and remove a digit from target
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
