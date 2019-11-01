using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sonido : MonoBehaviour
{
    float nextActionTime;
    float intervalo;

    [SerializeField]
    float felicidadPromedio;

    public AudioClip timbre;
    public AudioClip ruidoBlanco;
    public AudioClip multitud;
    public AudioClip sonidoPI;

    public AudioSource as1;
    public AudioSource as2;
    public AudioSource as3;

    globalVariables gV;
    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 3f;

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + intervalo;
            felicidadPromedio = ObtenerPromedioFelicidad();
            if(felicidadPromedio <= -100)
            {
                gV.ActivarRetirada();
            }
        }

            float volumenMultitud = Utilidades.Map(gV.GetPoblacionActual(), 2, 100, 0, 1, true);
        as1.volume = volumenMultitud;

        float volumenPi = Utilidades.Map(felicidadPromedio, -50, 100, 1, 0);
        as2.volume = volumenPi;
    }

    float ObtenerPromedioFelicidad()
    {
        float sumaFelicidad = 0;
        int cantidadSujetos = 0;
        GameObject[] sujetos = GameObject.FindGameObjectsWithTag("sujeto");
        foreach (GameObject sujeto in sujetos)
        {
            sumaFelicidad += sujeto.GetComponent<vida>().ObtenerFelicidad();
            cantidadSujetos++;
        }
        return sumaFelicidad / cantidadSujetos;
    }

    public void TocarTimbre()
    {
        as3.Play();
    }
}
