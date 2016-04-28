using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Util;

public class HPBox : MonoBehaviour {
    public int curHP;
    private int displayedHP;
    private Sprite[] sprites;
    private bool changing;// was the displayed HP changing last frame? Used to avoid changing sprites when they don't need it
    private Image[] HPDisplay = new Image[4];

    // Initialization not requiring other objects (except those set in editor)
    void Awake() {
        HPDisplay[0] = gameObject.FindChild("1").GetComponent<Image>();
        HPDisplay[1] = gameObject.FindChild("10").GetComponent<Image>();
        HPDisplay[2] = gameObject.FindChild("100").GetComponent<Image>();
        HPDisplay[3] = gameObject.FindChild("1000").GetComponent<Image>();
    }


    // Initialization that depends on specific external things having initialized
    // This requires an input value of the player's HP, so it is called only after the HUD has a reference to the player
    public void Init(int hp) {
        curHP = hp;
        displayedHP = hp;
        sprites = Controller.UI.getSprite(hp);
        changing = false;
        UpdateSprites();
    }
	
	// Update is called once per frame
	void Update () {
        if (curHP != displayedHP)
        {// changing, so update
            changing = true;
            //TODO adjust rate to that of actual game
            if (curHP < displayedHP) { displayedHP = Mathf.Max(displayedHP - 5, curHP); }
            else { displayedHP = Mathf.Min(displayedHP + 5, curHP); }
            UpdateSprites();
        } else
        {
            if (changing)
            {// was changing last frame, so it needs one more update
                changing = false;
                UpdateSprites();
            }
            // not changing, wasn't changing last time, nothing to do
        }
    }

    // update the internal set of stored sprites and the display thereof
    private void UpdateSprites() {
        sprites = Controller.UI.getSprite(displayedHP, curHP - displayedHP);
        for(int i = 0; i < sprites.Length; ++i)
        {
            HPDisplay[i].enabled = true;
            HPDisplay[i].sprite = sprites[i];
        }
        // anything past that is a blank space
        for (int i = sprites.Length; i < 4; ++i)
        {
            HPDisplay[i].enabled = false;
        }
    }

    // update the transparency of this
    public void setTransp(float f) {
        Image img = GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, f);
        foreach (Image i in HPDisplay)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, f);
        }
    }
}
