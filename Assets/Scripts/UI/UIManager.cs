using UnityEngine;
using UnityEngine.UI;
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
    private Image custGauge;
    private Sprite custGaugeBlank;
    private Image custFill;
    private Transform custFillTransform;
    
    // game ready to resume?
    public bool ready { get { return huds[0].ready && huds[1].ready; } }

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        Controller.UI = this;
    }

    // Initialization after all Awake() methods have run
    void Start() {
        custGauge = gameObject.FindChild("CustomGauge").GetComponent<Image>();
        custGauge.GetComponent<Animation2D>().outputRenderer = custGauge;
        custGaugeBlank = custGauge.sprite;
        custFill = gameObject.FindChild("CustomGauge/Fill").GetComponent<Image>();
        custFillTransform = custFill.transform;
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
    // called after main Update to ensure validity
    void LateUpdate() {
        if (Controller.paused)
        {
            custGauge.enabled = false;
        } else
        {
            custGauge.enabled = true;
            if (Controller.custFill >= 1)// to allow for float imprecision
            {
                custFill.enabled = false;
            } else
            {
                custFill.enabled = true;
                custFillTransform.localScale = new Vector3(Controller.custFill, 1);
            }
        }
    }

    public void gaugeAnim(bool on) {
        if (on)
        {
            custGauge.GetComponent<Animation2D>().Play();
        } else
        {
            custGauge.GetComponent<Animation2D>().Stop();
            custGauge.sprite = custGaugeBlank;
        }
    }

    // Only to be called by Controller.startTurn(). Unfortunately, must be public to allow this.
    // Opens both Custom Screens by delegation to HUD
    public void continueStartTurn() {
        if (Controller.paused)
        {
            foreach (HUD h in huds) { h.openCustom(); }
        } else { throw new Exception("Called out of order"); }
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
