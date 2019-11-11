using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalVariables : MonoBehaviour
{
    //Calibracion de personas
    public float minX, maxX, minY1, maxY1, minY2, maxY2;

    public sonido son;

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
    public GameObject barrera;
    public GameObject prefabBicho;
    public List<GameObject> listadoComidas;
    public List<GameObject> sovietComunidades;
    public List<GameObject> almacenesDeComida;
    public List<GameObject> fabricasDeComida;
    public List<GameObject> arbustosDeComida;

    [SerializeField]
    List<GameObject> listaDePersonas;

    [SerializeField]
    int poblacionActual;
    [SerializeField]
    public int cantSoviets;
    [SerializeField]
    public int cantFabricas;
    [SerializeField]
    public int cantAlmacenes;
    public int cantToconesSoviets;
    public int cantToconesFabricas;
    public int cantToconesAlmacenes;


    //Cada 33hab
    [SerializeField]
    int cantSovietsIdeales;
    [SerializeField]
    int cantFabricasIdeales;
    [SerializeField]
    int cantAlmacenesIdeales;
    [SerializeField]
    public int relacionSovietPoblacion;
    [SerializeField]
    public int relacionFabricaPoblacion;
    [SerializeField]
    public int relacionAlmacenPoblacion;

    public float porcentajeKinectCubierta;

    Vector3[] coloresLineas = new[] { new Vector3(230, 46, 56), new Vector3(216, 6, 97), new Vector3(150, 15, 170), new Vector3(103, 42, 172), new Vector3(62, 73, 178), new Vector3(43, 153, 240), new Vector3(16, 178, 240), new Vector3(13, 206, 213), new Vector3(14, 163, 133), new Vector3(80, 192, 84), new Vector3(144, 205, 78), new Vector3(202, 230, 67), new Vector3(251, 241, 66), new Vector3(247, 197, 16), new Vector3(243, 151, 8), new Vector3(240, 71, 40), new Vector3(118, 79, 72), new Vector3(158, 156, 157), new Vector3(98, 125, 136), new Vector3(255, 250, 255) };
    int coloresPedidos = 0;

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
    void Start() {
        nextActionTime = 0f;
        intervalo = 5f;

        listaDePersonas = new List<GameObject>();

        relojProximoSegundo = 0f;
        relojSegundo = 1f;

        minutoActual = 0;
        horaActual = 0;
        diaActual = 1;

        ActualizarSovietsComunidades();
        ActualizarAlmacenesDeComida();
        ActualizarFabricasDeComida();
        ActualizarArbustosDeComida();
        ActualizarCantidadesEdificios();
        ActualizarCantidadesIdealesEdificios();
        listadoComidas = new List<GameObject>();

        relacionSovietPoblacion = 1;
        relacionFabricaPoblacion = 2;
        relacionAlmacenPoblacion = 3;

        sp = GetComponent<spawner>();
        //son = GameObject.Find("Sonido").GetComponent<sonido>();


        depositosDeComida = new List<GameObject>();

        Debug.Log("Hay piso?");
        if(piso != null){
            Debug.Log("Hay piso :)");
            minX = piso.transform.GetChild(0).transform.position.x;
            Debug.Log("DESDE GV: " + minX);
            maxX = piso.transform.GetChild(1).transform.position.x;
            maxY1 = piso.transform.GetChild(2).transform.position.y;
            minY1 = piso.transform.GetChild(3).transform.position.y;
            maxY2 = piso.transform.GetChild(4).transform.position.y;
            minY2 = piso.transform.GetChild(5   ).transform.position.y;

        } else{
            Debug.LogError("No hay un objeto piso asignado");
        }

        totems = GameObject.FindGameObjectsWithTag("totem");
        foreach (GameObject totem in totems)
        {
            if(totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PRIVADOCOMERCIAL)
            {
                totemIndividualista = totem;
            }
            if(totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PRIVADOENTRETENIMIENTO)
            {
                totemEntretenimiento = totem;
            }
            if(totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PUBLICOESTATAL)
            {
                totemLider = totem;
            }
            if(totem.GetComponent<totem>().ObtenerTipoTotem() == TIPOTOTEM.PUBLICOMILITAR)
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
            ActualizarSovietsComunidades();
            ActualizarAlmacenesDeComida();
            ActualizarFabricasDeComida();
            ActualizarArbustosDeComida();
            ActualizarCantidadesEdificios();
            ActualizarCantidadesIdealesEdificios();
            PrepararConstruccionesDeEdificios();
            ActualizarDepositosDeComida(); //V4 Nuevo
            ActualizarPuntosRevolucionarios(); //v4 Nuevo
        }
        if (Time.time > relojProximoSegundo)
        {
            relojProximoSegundo = Time.time + relojSegundo;

            minutoActual+=4f;
            if(minutoActual >= 60)
            {
                minutoActual = 0;
                horaActual++;
            }
            if(horaActual >= 10)
            {
                horaActual = 0;
                diaActual++;
            }

            if(promedioDescontento > 50)
            {
                Debug.Log("REVOLUCION");
            }


            if(horaActual == 3 && minutoActual == 0)
            {
                foreach (GameObject s in GameObject.FindGameObjectsWithTag("sujeto"))
                {
                    //s.GetComponent<vida>().setearSovietComunidad(null);
                }
                //son.TocarTimbre();
            }
            if (horaActual == 0 && minutoActual == 0)
            {
                //son.TocarTimbre();
            }
        }
    }


    public List<GameObject> ObtenerSovietsComunidadesOrdenadosPorCercania(Vector3 posComparacion)
    {
        List<GameObject> cacheSovietsComunidades = new List<GameObject>(sovietComunidades);
        List<GameObject> sovietsComunidadesOrdenados = new List<GameObject>();

        for (int i = 0; i < cacheSovietsComunidades.Count; i++)
        {
            GameObject cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject sovietComunidad in cacheSovietsComunidades)
            {
                float cacheDistancia = Vector2.Distance(sovietComunidad.transform.position, posComparacion);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercano = sovietComunidad;
                }
            }
            cacheSovietsComunidades.Remove(cacheMasCercano);
            sovietsComunidadesOrdenados.Add(cacheMasCercano);
        }
        return sovietsComunidadesOrdenados;
    }

    void ActualizarSovietsComunidades()
    {
        sovietComunidades = new List<GameObject>(GameObject.FindGameObjectsWithTag("sovietComunidad"));
    }
    
    void ActualizarAlmacenesDeComida()
    {
        almacenesDeComida = new List<GameObject>(GameObject.FindGameObjectsWithTag("almacenDeComida"));
    }

    void ActualizarFabricasDeComida()
    {
        fabricasDeComida = new List<GameObject>(GameObject.FindGameObjectsWithTag("fabricaDeComida"));
    }

    void ActualizarArbustosDeComida()
    {
        arbustosDeComida = new List<GameObject>(GameObject.FindGameObjectsWithTag("arbustoDeComida"));
    }

    public List<GameObject> ObtenerTodosLosAlmacenes()
    {
        return almacenesDeComida;
    }

    public List<GameObject> ObtenerTodasLasFabricas()
    {
        return fabricasDeComida;
    }

    public List<GameObject> ObtenerTodosLosArbustos()
    {
        return arbustosDeComida;
    }

    void ActualizarCantidadesEdificios()
    {
        cantAlmacenes = almacenesDeComida.Count + cantToconesAlmacenes;
        cantFabricas = fabricasDeComida.Count + cantToconesFabricas;
        cantSoviets = sovietComunidades.Count + cantToconesSoviets;
    }

    public void SumarPoblacion()
    {
        poblacionActual++;
    }
    public void RestarPoblacion()
    {
        poblacionActual--;
    }
    public void ActualizarCantidadesIdealesEdificios()
    {
        cantSovietsIdeales = (poblacionActual / 33) * relacionSovietPoblacion;
        cantAlmacenesIdeales = (poblacionActual / 33) * relacionAlmacenPoblacion;
        cantFabricasIdeales = (poblacionActual / 33) * relacionFabricaPoblacion;
        if(cantSovietsIdeales < 1)
        {
            cantSovietsIdeales = 1;
        }
        if (cantFabricasIdeales < 1)
        {
            cantFabricasIdeales = 1;
        }
        if (cantAlmacenesIdeales < 1)
        {
            cantAlmacenesIdeales = 1;
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

    void PrepararConstruccionesDeEdificios()
    {
        if (cantSoviets < cantSovietsIdeales)
        {
            sp.ConstruirSoviet();
        }

        if(cantFabricas < cantFabricasIdeales)
        {
            sp.ConstruirFabrica();
        }

        if(cantAlmacenes < cantAlmacenesIdeales)
        {
            sp.ConstruirAlmacen();
        }
    }

    public Vector3 ObtenerColorLineaSoviet()
    {
        Vector3 color =  coloresLineas[coloresPedidos];
        coloresPedidos++;
        return color;
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
            Debug.LogError("ERROR: intentando agregar una persona que ya esta en el listado.");
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

    public void ActivarRetirada()
    {
        foreach(GameObject sujeto in GameObject.FindGameObjectsWithTag("sujeto"))
        {
            sujeto.GetComponent<vida>().Retirar();
        }
    }

    public Transform ObtenerPuntoRetiradaMasCercano(Vector3 posComparacion)
    {
        List<Transform> cachePuntos = new List<Transform>(spotsRetirada);
        List<Transform> puntosOrdenados = new List<Transform>();

        for (int i = 0; i < cachePuntos.Count; i++)
        {
            Transform cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (Transform spot in cachePuntos)
            {
                float cacheDistancia = Vector2.Distance(spot.transform.position, posComparacion);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercano = spot;
                }
            }
            cachePuntos.Remove(cacheMasCercano);
            puntosOrdenados.Add(cacheMasCercano);
        }
        return puntosOrdenados[0];
    }

    public GameObject ObtenerPersonaMasCercana(Vector3 posComparacion)
    {
        List<GameObject> cachePersonas = new List<GameObject>(listaDePersonas);
        List<GameObject> personasOrdenadas = new List<GameObject>();

        for (int i = 0; i < cachePersonas.Count; i++)
        {
            GameObject cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject persona in cachePersonas)
            {
                float cacheDistancia = Vector2.Distance(persona.transform.position, posComparacion);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercano = persona;
                }
            }
            cachePersonas.Remove(cacheMasCercano);
            personasOrdenadas.Add(cacheMasCercano);
        }
        return personasOrdenadas[0];
    }

    public bool ObtenerHayPersonas()
    {
        if(listaDePersonas.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
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
                float cacheDistancia = Vector2.Distance(depo.transform.position, posComparacion);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercano = depo;
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
                if(depo != null)
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
            if(cacheDistanciaMinima <= distanciaMax)
            {
                depositosOrdenados.Add(cacheMasCercano);
            }
        }
        return depositosOrdenados;
    }

    void ActualizarPuntosRevolucionarios()
    {
        List<GameObject> sujetos = new List<GameObject>(GameObject.FindGameObjectsWithTag("sujeto"));

        if(sujetos.Count < 1)
        {
            ReiniciarEscena();
        }


        int descontetoAcumulado = 0;
        foreach (GameObject sujeto in sujetos)
        {
            descontetoAcumulado += sujeto.GetComponent<Follower>().ObtenerPorcentajeDescontento();
        }
        promedioDescontento = descontetoAcumulado / sujetos.Count;

        if(promedioDescontento > 50)
        {
            foreach (GameObject sujeto in sujetos)
            {
                forzarRevolucion = true;
                sujeto.GetComponent<Follower>().ActivarProcesoRevolucionario();
            }
        }

        /// <summary> Si estamos en proceso revolucionario y el descontento cae retorcedemos y cancelamso</summary>
        if(forzarRevolucion)
        {
            if(promedioDescontento < 50)
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
