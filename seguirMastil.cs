using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seguirMastil : MonoBehaviour
{
    float nextActionTime;
    float intervalo;
    
    [SerializeField]
    float felicidadPromedio;

    [SerializeField]
    Transform seguir = null;

    [SerializeField]
    Transform mastil = null;

    globalVariables gV;

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        gV = GetComponent<globalVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Cada 5 segundos actualizar almacen más vacio
            nextActionTime = Time.time + intervalo;
            mastil.localScale = new Vector3(mastil.localScale.x, mastil.localScale.y, MastilZ());
            transform.position = seguir.position;
            felicidadPromedio = ObtenerPromedioFelicidad();
        }
    }

    float MastilZ()
    {
        return Utilidades.Map(ObtenerPromedioFelicidad(), 0, 100, 50, 200, true);
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
}
