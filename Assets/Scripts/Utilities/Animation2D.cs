using UnityEngine;
using System.Collections;

// animation system almost exactly copied from:
// http://gamasutra.com/blogs/DonaldKooiker/20150821/251260/Luckslinger_tech_1__How_to_make_2D_Sprite_Animations_in_Unity.php
public class Animation2D : MonoBehaviour {
    public new string name;
    public float FPS;
    public bool isLooping;
    public bool playOnStartup = false;
    public Sprite[] frames;
    // output renderer accessed via property to make it public but not editor-modifiable
    private SpriteRenderer _or;
    public SpriteRenderer outputRenderer { set { _or = value; } get { return _or; } }
    private float secondsToWait;

    private int currentFrame;
    private bool stopped = false;
    // property for external read-only access while allowing internal modification
    public bool isStopped { get { return stopped; } }

    public void Awake() {
        outputRenderer = this.GetComponent<SpriteRenderer>();
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
        if (reset)
        {
            currentFrame = 0;
        }

        stopped = false;
        outputRenderer.enabled = true;

        if (frames.Length > 1)
        {
            Animate();
        } else if (frames.Length > 0)
        {
            outputRenderer.sprite = frames[0];
        }
    }

    private void Animate() {
        CancelInvoke("Animate");
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

            outputRenderer.sprite = frames[currentFrame];
            ++currentFrame;
            if (secondsToWait > 0)
            {
                Invoke("Animate", secondsToWait);
            }
        } else
        {
            outputRenderer.enabled = false;
        }
    }

    public void Stop() {
        CancelInvoke("Animate");
        stopped = true;
        outputRenderer.enabled = false;
    }

}
