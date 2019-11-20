﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalVariables : MonoBehaviour
{
    //Calibracion de personas
    public float minX, maxX, minY1, maxY1, minY2, maxY2;

    [SerializeField]
    Transform[] spotsRetirada;

    float nextActionTime;
    float intervalo;

    float relojProximoSegundo;
    float relojSegundo;
    public int horaActual;
    public float minutoActual;
    public int diaActual;

    public GameObject piso;
    public GameObject prefabBicho;

    [SerializeField]
    List<GameObject> listaDePersonas;

    [SerializeField]
    int poblacionActual;

    spawner sp;


    /// <summary> Listado de depositos de comida que están vivos en el mapa y tienen al menos 1 comida</summmary>
    [SerializeField]
    List<GameObject> depositosDeComida;

    /// <summary> El promedio del descontento (del 1 al 100) de todos los sujetos de la escena actualizada cada 5s</summary>
    [SerializeField]
    float promedioDescontento = 0;

    [SerializeField]
    bool forzarRevolucion;

    [SerializeField]
    public GameObject[] markersLimites;

    public GameObject[] totems;
    public GameObject totemMilitar, totemLider, totemEntretenimiento, totemIndividualista;

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        listaDePersonas = new List<GameObject>();

        relojProximoSegundo = 0f;
        relojSegundo = 1f;

        minutoActual = 0;
        horaActual = 0;
        diaActual = 1;

        depositosDeComida = new List<GameObject>();

        sp = GetComponent<spawner>();
        //son = GameObject.Find("Sonido").GetComponent<sonido>();


        if (piso != null)
        {
            minX = piso.transform.GetChild(0).transform.position.x;
            maxX = piso.transform.GetChild(1).transform.position.x;
            maxY1 = piso.transform.GetChild(2).transform.position.y;
            minY1 = piso.transform.GetChild(3).transform.position.y;
            maxY2 = piso.transform.GetChild(4).transform.position.y;
            minY2 = piso.transform.GetChild(5).transform.position.y;

        }
        else
        {
            Debug.LogError("No hay un objeto piso asignado");
        }

        totems = GameObject.FindGameObjectsWithTag("totem");
        foreach (GameObject totem in totems)
        {
            if (totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PRIVADOCOMERCIAL)
            {
                totemIndividualista = totem;
            }
            if (totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PRIVADOENTRETENIMIENTO)
            {
                totemEntretenimiento = totem;
            }
            if (totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PUBLICOESTATAL)
            {
                totemLider = totem;
            }
            if (totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PUBLICOMILITAR)
            {
                totemMilitar = totem;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Actualizar cada x segundos los almaceces y los soviets que existen
            nextActionTime = Time.time + intervalo;
            ActualizarDepositosDeComida(); //V4 Nuevo
            ActualizarPuntosRevolucionarios(); //v4 Nuevo
        }
        if (Time.time > relojProximoSegundo)
        {
            relojProximoSegundo = Time.time + relojSegundo;

            minutoActual += 4f;
            if (minutoActual >= 60)
            {
                minutoActual = 0;
                horaActual++;
            }
            if (horaActual >= 10)
            {
                horaActual = 0;
                diaActual++;
            }

            if (promedioDescontento > 50)
            {
                Debug.Log("REVOLUCION");
            }
        }
    }


    /// <summary> Actualizamos la variable depositosDeComida para saber cuantos yacimientos vivos y con 1 o más comida hay</summary>
    void ActualizarDepositosDeComida() 
    {
        List<GameObject> todosLosDepositos = new List<GameObject>(GameObject.FindGameObjectsWithTag("depositodecomida"));
        List<GameObject> devolver = new List<GameObject>();
        foreach (GameObject depo in todosLosDepositos)
        {
            if(depo.GetComponent<depositoDeComida>().ObtenerUnidadesRestantes() > 0) 
            {
                devolver.Add(depo);
            }
        }
        depositosDeComida = devolver;
    }


    public void SumarPoblacion()
    {
        poblacionActual++;
    }

    public void RestarPoblacion()
    {
        poblacionActual--;
    }

    public int GetPoblacionActual()
    {
        return poblacionActual;
    }

    public void AgregarPersona(GameObject per)
    {
        if (!listaDePersonas.Contains(per))
        {
            listaDePersonas.Add(per);
        }
        else
        {
            //Debug.Log("intentando agregar una persona que ya esta en el listado.");
        }
    }

    public void EliminarPersona(GameObject per)
    {
        if (listaDePersonas.Contains(per))
        {
            listaDePersonas.Remove(per);
        }
        else
        {
            Debug.LogError("ERROR: intentando eliminar una persona qu eno existe en el listado.");
        }
    }


    /// <summary> Devuelve un listado con todos los depositos de comida que tienen al menos 1 alimento
    /// ordenados de mas cercano a más lejano respecto al punto de comparación proporcionado </summary>
    public List<GameObject> ObtenerDepositosDeComidaOrdenadosPorCercania(Vector3 posComparacion)
    {
        List<GameObject> cacheDepositos = new List<GameObject>(depositosDeComida);
        List<GameObject> depositosOrdenados = new List<GameObject>();

        for (int i = 0; i < cacheDepositos.Count; i++)
        {
            GameObject cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject depo in cacheDepositos)
            {
                if (depo != null)
                { //Esta linea es necesaria para que cheque si el objeto existe porque hay un desfasaje entre que actualiza los depositos que hay y chequea esto
                    float cacheDistancia = Vector2.Distance(depo.transform.position, posComparacion);
                    if (cacheDistancia < cacheDistanciaMinima)
                    {
                        cacheDistanciaMinima = cacheDistancia;
                        cacheMasCercano = depo;
                    }
                }
            }
            cacheDepositos.Remove(cacheMasCercano);
            depositosOrdenados.Add(cacheMasCercano);
        }
        return depositosOrdenados;
    }

    /// <summary> Devuelve un listado de depositos que están dentro de una distancia maxima de un punto, ordenados
    /// de más cercano a más lejano del punto de comparacion proporcionado. </summary>
    public List<GameObject> ObtenerDepositosDeComidaOrdenadosPorCercania(Vector3 posComparacionPersona, float distanciaMax)
    {
        List<GameObject> cacheDepositos = new List<GameObject>(depositosDeComida);
        List<GameObject> depositosOrdenados = new List<GameObject>();

        for (int i = 0; i < cacheDepositos.Count; i++)
        {
            GameObject cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject depo in cacheDepositos)
            {
                if (depo != null)
                {
                    float cacheDistancia = Vector2.Distance(depo.transform.position, posComparacionPersona);
                    if (cacheDistancia < cacheDistanciaMinima)
                    {
                        cacheDistanciaMinima = cacheDistancia;
                        cacheMasCercano = depo;
                    }
                }
            }
            cacheDepositos.Remove(cacheMasCercano);
            if (cacheDistanciaMinima <= distanciaMax)
            {
                depositosOrdenados.Add(cacheMasCercano);
            }
        }
        return depositosOrdenados;
    }

    void ActualizarPuntosRevolucionarios()
    {
        List<GameObject> sujetos = new List<GameObject>(GameObject.FindGameObjectsWithTag("sujeto"));

        if (sujetos.Count < 1)
        {
            ReiniciarEscena();
        }


        int descontetoAcumulado = 0;
        foreach (GameObject sujeto in sujetos)
        {
            descontetoAcumulado += sujeto.GetComponent<Follower>().ObtenerPorcentajeDescontento();
        }
        promedioDescontento = descontetoAcumulado / sujetos.Count;

        if (promedioDescontento > 50)
        {
            foreach (GameObject sujeto in sujetos)
            {
                forzarRevolucion = true;
                sujeto.GetComponent<Follower>().ActivarProcesoRevolucionario();
            }
        }

        /// <summary> Si estamos en proceso revolucionario y el descontento cae retorcedemos y cancelamso</summary>
        if (forzarRevolucion)
        {
            if (promedioDescontento < 50)
            {
                foreach (GameObject sujeto in sujetos)
                {
                    sujeto.GetComponent<Follower>().DesactivarProcesoRevolucionario();
                }
            }
        }
    }

    void ReiniciarEscena()
    {
        //TODO: oscurecer la escena lentamente y luego recargarla
    }
}
