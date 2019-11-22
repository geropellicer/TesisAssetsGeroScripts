using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    List<GameObject> listaDeSujetos;

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

    public GameObject totemMilitar, totemLider, totemEntretenimiento, totemIndividualista;

    public GameObject oscurecer1, oscurecer2;

    public OscAntenasController oscAntenasController;

    /// <summary> Cantidad minima de bichos que tenemos cuando no hay personas en la proyeccion </summary>
    [SerializeField]
    int cantidadMinimaBichosReposo;

    /// <summary> Cantidad maxima de bichos que tenemos cuando no hay personas en la proyeccion</summary>
    [SerializeField]
    int cantidadMaximaBichosReposo;


    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        listaDePersonas = new List<GameObject>();
        listaDeSujetos = new List<GameObject>();

        relojProximoSegundo = 0f;
        relojSegundo = 1f;

        minutoActual = 0;
        horaActual = 0;
        diaActual = 1;

        depositosDeComida = new List<GameObject>();

        sp = GetComponent<spawner>();
        //son = GameObject.Find("Sonido").GetComponent<sonido>();

        oscAntenasController = GameObject.Find("OSC Manger").GetComponent<OscAntenasController>();


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
            ControlarPoblacion();
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
            if (depo.GetComponent<depositoDeComida>().ObtenerUnidadesRestantes() > 0)
            {
                devolver.Add(depo);
            }
        }
        depositosDeComida = devolver;
    }


    public void SumarPoblacion(GameObject _sujeto)
    {
        Debug.Log("Sumando");
        if (!listaDeSujetos.Contains(_sujeto))
        {
            Debug.Log("Sumando adentro");
            listaDeSujetos.Add(_sujeto);
            poblacionActual++;
        }
        else
        {
            Debug.Log("intentando agregar un sujeto que ya esta en el listado.");
        }
    }

    public void RestarPoblacion(GameObject sujeto)
    {
        Debug.Log("Restando");
        if (listaDeSujetos.Contains(sujeto))
        {
            Debug.Log("Restando adentro");
            listaDeSujetos.Remove(sujeto);
            poblacionActual--;
        }
        else
        {
            Debug.Log("ERROR: intentando eliminar un sujeto que no existe en el listado.");
        }
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
            Debug.Log("intentando agregar una persona que ya esta en el listado.");
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
            Debug.Log("ERROR: intentando eliminar una persona qu eno existe en el listado.");
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
        if (poblacionActual < 1)
        {
            if(Time.time > 10)
            {
                // Solo intentamos reiniciar la escena si han pasado 10 segundos al menosm para evitar que intente reiniciar al principio
                ReiniciarEscena();
            }
        }

        if (poblacionActual > 0)
        {
            int descontetoAcumulado = 0;
            foreach (GameObject sujeto in listaDeSujetos)
            {
                descontetoAcumulado += sujeto.GetComponent<Follower>().ObtenerPorcentajeDescontento();
            }
            promedioDescontento = descontetoAcumulado / listaDeSujetos.Count;

            if (!forzarRevolucion)
            {
                if (promedioDescontento > 50)
                {
                    foreach (GameObject sujeto in listaDeSujetos)
                    {
                        if(sujeto.GetComponent<Follower>().GetForzarRevolucion() == false)
                        {
                            sujeto.GetComponent<Follower>().ActivarProcesoRevolucionario();
                        }
                    }

                    forzarRevolucion = true;
                    // TODO: pintar suelo de rojo
                }
            }

            /// <summary> Si estamos en proceso revolucionario y el descontento cae retorcedemos y cancelamso</summary>
            if (forzarRevolucion)
            {
                if (promedioDescontento < 50)
                {
                    foreach (GameObject sujeto in listaDeSujetos)
                    {
                        if(sujeto.GetComponent<Follower>().GetForzarRevolucion() == true)
                        {
                            sujeto.GetComponent<Follower>().DesactivarProcesoRevolucionario();
                        }
                    }

                    forzarRevolucion = false;
                }
            }
        }
    }


    void ControlarPoblacion()
    {
        // Si hay personas ejercemos un control mas sutil.
        if (listaDePersonas.Count > 0)
        {
            if (promedioDescontento >= 40)
            {
                // No hacemos nada porque es posible que se active el modo revolucionario en cualquier momento
            }
            else if (promedioDescontento < 40)
            {
                // EN principio en este caso solo agregamos un bicho cada 5 segundos, para que el flow dependa mas de lo que pasa en la escena
                // Adicionalmente podriamos manejar algo similar o igual a cuando no hay personas.
                if (poblacionActual < cantidadMinimaBichosReposo)
                {
                    GameObject.Instantiate(prefabBicho, Utilidades.PuntoRandom(piso), Quaternion.identity);
                }
            }
        }

        // Si no hay personas vamos inyectando personas para que siempre se mantenga entre el minimo y el maximo
        if (listaDePersonas.Count == 0)
        {
            if (poblacionActual < cantidadMinimaBichosReposo)
            {
                // Spawneamos la cantidad necesaria para llegar a un promedio entre el minimo y el maximo
                // Por ejemplo si el minimo es 5 y el maximo es 10 y tenemos 2:
                // Espawenamos ((5-2) + (10-2)) / 2 = 5.5;
                int cantidadASpawnear = Mathf.FloorToInt(((cantidadMinimaBichosReposo - poblacionActual) + (cantidadMaximaBichosReposo - poblacionActual)) / 2);
                for (int i = 0; i < cantidadASpawnear; i++)
                {
                    GameObject.Instantiate(prefabBicho, Utilidades.PuntoRandom(piso), Quaternion.identity);
                }
            }
        }
    }


    public void OscurecidoTerminado()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReiniciarEscena()
    {
        oscurecer1.GetComponent<Desoscurecer>().Oscurecer(gameObject);
        oscurecer2.GetComponent<Desoscurecer>().Oscurecer(gameObject);
    }
}
