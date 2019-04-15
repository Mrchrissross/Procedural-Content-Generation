using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [System.Serializable]
    public class Text
    {
        //variables
        public TMP_Text _text;
        [VectorLabels("Time", "Count"), Tooltip("How long in seconds it takes for the object to first appear on the screen.")]
        public Vector2 inTimer = new Vector2(2.0f, 0.0f);
        [VectorLabels("Time", "Count"), Tooltip("How long in seconds until the object vanishs from the screen.")]
        public Vector2 outTimer = new Vector2(10.0f, 0.0f);

        [HideInInspector]
        public bool animated;

        [Header("Animation Settings")]
        public bool scale = true;
        public bool fade = false;
        [Space]
        public float animationInSpeed = 2.0f;
        [HideInInspector]
        public float animationOutSpeed = 1.0f;
        [Space]
        public bool reset;
    }

    [SerializeField]
    Text[] texts;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Text text in texts)
        {
            text.inTimer.y = text.inTimer.x;
            text.outTimer.y = text.outTimer.x;

            if (text.scale)
                text._text.transform.localScale = Vector3.zero;
            else if (text.fade)
                text._text.alpha = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Text text in texts)
        {
            if(text.reset)
            {
                text.inTimer.y = text.inTimer.x;
                text.outTimer.y = text.outTimer.x;
                text.animated = false;
                text.reset = false;
            }

            if (!text.animated)
            {
                text.outTimer.y -= Time.deltaTime;

                if (text.inTimer.y > 0)
                {
                    text.inTimer.y -= Time.deltaTime;
                    continue;
                }

                if (text.outTimer.y > 0)
                {
                    AnimateIn(text);
                    continue;
                }

                AnimateOut(text);
            }
        }
    }

    void AnimateIn(Text text)
    {
        if (text.scale && text._text.transform.localScale.x < 1)
        {
            Vector3 scale = text._text.transform.localScale;
            scale += (Time.deltaTime * text.animationInSpeed) * Vector3.one;
            text._text.transform.localScale = scale;
        }
        else if(text.fade && text._text.alpha < 1)
            text._text.alpha += (Time.deltaTime * text.animationInSpeed);
    }

    void AnimateOut(Text text)
    {
        if (text.scale && text._text.transform.localScale.x > 0)
        {
            Vector3 scale = text._text.transform.localScale;
            scale -= (Time.deltaTime * text.animationOutSpeed) * Vector3.one;
            text._text.transform.localScale = scale;
        }
        else if (text.fade && text._text.alpha > 0)
            text._text.alpha -= (Time.deltaTime * text.animationOutSpeed);
        else
            text.animated = true;
    }
}
