using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class luzDia : MonoBehaviour
{
    Light l;
    globalVariables gV;
    [SerializeField]
    public float horaTraducida;
    [SerializeField]
    double brilloActual;

    [SerializeField]
    float minBrillo = 0f;
    [SerializeField]
    float maxBrillo = 0.7f;
    [SerializeField]
    float minMinutos = 0f;
    [SerializeField]
    float maxMinutos = 480f;


    // Start is called before the first frame update
    void Start()
    {
        l = GetComponent<Light>();
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        TraducirHoraAMinutos();
        TraducirMinutosALuz();
    }

    void TraducirHoraAMinutos()
    {
        horaTraducida = gV.horaActual * 60 + gV.minutoActual;
    }

    void TraducirMinutosALuz()
    {
        if (gV.horaActual < 8)
        {
            brilloActual = Utilidades.Map(horaTraducida, minMinutos, maxMinutos, minBrillo, maxBrillo, true);
            l.intensity = (float)brilloActual;
        } else if(gV.horaActual >= 8)
        {
            brilloActual = Utilidades.Map(horaTraducida, 480, 600, maxBrillo, minBrillo);
            l.intensity = (float)brilloActual;
        }
    }
}
