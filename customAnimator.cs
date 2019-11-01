using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customAnimator : MonoBehaviour
{
    int frameActual = 1;
    int framesEsperar = 4;
    SpriteRenderer sr;

    public Sprite sp1;
    public Sprite sp2;
    public Sprite sp3;
    public Sprite sp4;
    public Sprite sp5;
    public Sprite sp6;
    public Sprite sp7;
    public Sprite sp8;
    public Sprite sp9;
    public Sprite sp10;
    public Sprite sp11;
    public Sprite sp12;
    public Sprite sp13;
    public Sprite sp14;
    public Sprite sp15;
    public Sprite sp16;
    public Sprite sp17;
    public Sprite sp18;
    public Sprite sp19;
    public Sprite sp20;

    public enum animacion
    {
        nulo,
        caminando,
        cogiendo,
        comiendo,
        cosechando,
        fabricando,
        construyendo,
        transportando,
        resistiendo
    }
    public animacion animacionActual;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        CambiarAnimacion(animacion.caminando);
    }

    // Update is called once per frame
    void Update()
    {
        if(animacionActual != animacion.nulo)
        {
            if(animacionActual == animacion.caminando)
            {
                if(frameActual == 1)
                {
                    sr.sprite = sp1;
                    frameActual = 2;
                } else if(frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp2;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.cogiendo)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp3;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp4;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar*2)
                {
                    sr.sprite = sp5;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.comiendo)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp6;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp7;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar * 2)
                {
                    sr.sprite = sp8;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.construyendo || animacionActual == animacion.fabricando)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp9;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp10;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar * 2)
                {
                    sr.sprite = sp11;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.transportando)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp12;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp13    ;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar * 2)
                {
                    sr.sprite = sp14;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.resistiendo)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp15;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp16;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar * 2)
                {
                    sr.sprite = sp17;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
            if (animacionActual == animacion.cosechando)
            {
                if (frameActual == 1)
                {
                    sr.sprite = sp18;
                    frameActual = 2;
                }
                else if (frameActual == 2 + framesEsperar)
                {
                    sr.sprite = sp19;
                    frameActual++;
                }
                else if (frameActual == 3 + framesEsperar * 2)
                {
                    sr.sprite = sp20;
                    frameActual = 1 - framesEsperar;
                }
                else
                {
                    frameActual++;
                }
            }
        }
    }

    public void CambiarAnimacion(animacion anDestino)
    {
        if (anDestino != animacionActual)
        {
            frameActual = 1;
            animacionActual = anDestino;
        }
    }
}
