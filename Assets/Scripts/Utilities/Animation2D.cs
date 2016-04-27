using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// animation system based on (almost exactly copied from):
// http://gamasutra.com/blogs/DonaldKooiker/20150821/251260/Luckslinger_tech_1__How_to_make_2D_Sprite_Animations_in_Unity.php
public class Animation2D : MonoBehaviour {
    public new string name;
    public float FPS;
    public bool isLooping;
    public bool playOnStartup = false;
    public Sprite[] frames;
    // output renderer for objects using SpriteRenderer
    private SpriteRenderer _sprite_ren;
    // output renderer for objects using Image (UI stuff)
    private Image _img_ren;
    // boolean saying which of the two is in use
    private bool is_sprite;
    // output renderer accessed via property to make it public but not editor-modifiable
    // also lets other things set the renderer no matter whether this is on an Image or a SpriteRenderer
    public Component outputRenderer {
        set
        {
            SpriteRenderer s = value as SpriteRenderer;
            Image i = value as Image;
            if (null != s)
            {
                is_sprite = true;
                _sprite_ren = s;
            } else if (null != i)
            {
                is_sprite = false;
                _img_ren = i;
            } else
            {
                throw new System.InvalidCastException("Value is neither SpriteRenderer nor Image");
            }
        }
        get
        {
            if (is_sprite)
            {
                return _sprite_ren;
            } else
            {
                return _img_ren;
            }
        }
    }

    private float secondsToWait;

    private int currentFrame;
    private bool stopped = false;
    // property for external read-only access while allowing internal modification
    public bool isStopped { get { return stopped; } }
    public bool paused = false;

    public void Awake() {
        stopped = false;
        currentFrame = 0;
        if (FPS > 0)
        {
            secondsToWait = 1 / FPS;
        } else {
            secondsToWait = 0f;
        }
        if (playOnStartup)
        {
            Play(true);
        }
    }

    public void Play(bool reset = false) {
        // get the SpriteRenderer or Image component on this object
        // preference to the SpriteRenderer if both are present (they shouldn't be)
        SpriteRenderer sr_check = GetComponent<SpriteRenderer>();
        if (null == sr_check)
        {
            is_sprite = false;
            outputRenderer = GetComponent<Image>();
        } else
        {
            is_sprite = true;
            outputRenderer = sr_check;
        }

        if (reset)
        {
            currentFrame = 0;
        }

        stopped = false;
        if (is_sprite)
        {
            _sprite_ren.enabled = true;
        } else
        {
            _img_ren.enabled = true;
        }

        if (frames.Length > 1)
        {
            paused = false;
            StartCoroutine(Animate());
        } else if (frames.Length > 0)
        {
            if (is_sprite)
            {
                _sprite_ren.sprite = frames[0];
            } else
            {
                _img_ren.sprite = frames[0];
            }
        }
    }

    private IEnumerator Animate() {
        StopCoroutine(Animate());
        int framesToWait = (int)(secondsToWait * 60);
        while(framesToWait > 0)
        {// count down only while this is not paused
            if (!paused) { yield return --framesToWait; }
            else { yield return 0; }
        }

        if (currentFrame >= frames.Length || currentFrame < 0)
        {
            if (!isLooping)
            {
                stopped = true;
            } else {
                currentFrame = 0;
            }
        }

        if (!stopped)
        {

            if (is_sprite)
            {
                _sprite_ren.sprite = frames[currentFrame];
            } else
            {
                _img_ren.sprite = frames[currentFrame];
            }
            ++currentFrame;
            if (secondsToWait > 0)
            {
                StartCoroutine(Animate());
            }
        } else
        {
            if (is_sprite)
            {
                _sprite_ren.enabled = false;
            } else
            {
                _img_ren.enabled = false;
            }
        }
    }

    public void Stop() {
        StopCoroutine(Animate());
        stopped = true;
        if (is_sprite)
        {
            _sprite_ren.enabled = false;
        } else
        {
            _img_ren.enabled = false;
        }
    }
}
