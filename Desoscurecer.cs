using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Desoscurecer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Image im;
    [SerializeField]
    float alfa = 1f;

    [SerializeField]
    bool oscurecer = false;

    void Start()
    {
        im = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        im.color = new Color(im.color.r, im.color.g, im.color.b, alfa);

        if (!oscurecer)
        {
            if (alfa > 0)
            {
                alfa -= .005f;
            }
        } else
        {
            if (alfa < 1)
            {
                alfa += .005f;
            }
        }
    }

    public void Oscurecer()
    {
        oscurecer = true;
    }
}
