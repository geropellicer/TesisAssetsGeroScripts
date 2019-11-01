using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmacenDeComida : edificio
{
    [SerializeField]
    Transform[] spotsComer = new Transform[5];
    ParticleSystem ps;
    public ParticleSystem.EmissionModule psem;

    int iteradorRosquillas = 24;
    GameObject[] rosquillas = new GameObject[25];

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        trabajoNecesarioBase = 20;
        trabajadoresAsignados = new List<GameObject>();
        cantMaxTrabajadores = 4;
        fabricasCercanas = new List<GameObject>();
        distanciaMaxFabricasCercanas = 55;

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        sP = GameObject.Find("GlobalManager").GetComponent<spawner>();

        ps = GetComponent<ParticleSystem>();

        psem = ps.emission;
        psem.enabled = false;

        for(int i=0; i<25; i++)
        {
            rosquillas[i] = transform.Find("Rosquillas").GetChild(i).gameObject;
        }
    }

    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        sP = GameObject.Find("GlobalManager").GetComponent<spawner>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Cada 5 segundos actualizar almacen más vacio
            nextActionTime = Time.time + intervalo;
            ActualizarFabricasCercanas(distanciaMaxFabricasCercanas);
        }
    }

    public float ObtenerTrabajoNecesario()
    {
        return (trabajoNecesarioBase + Random.Range(-100, 100));
    }


    public void DepositarComidaDistribuida(vida sujeto)
    {
        comidaAlmacenada++;
        sujeto.RestarComidaFabricada();
        SumarRosquilla();
    }

    public void DepositarComidaDistribuida(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            DepositarComidaDistribuida(sujeto);
        }
    }

    public void SacarComidaDistribuida(vida sujeto)
    {
        if (comidaAlmacenada > 0)
        {
            comidaAlmacenada--;
            sujeto.SumarComidaFabricada();
        }
    }

    public void SacarComidaDistribuida(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            SacarComidaDistribuida(sujeto);
        }
    }

    public int ObtenerComidaAlmacenada()
    {
        return comidaAlmacenada;
    }

    public int ObtenerCantidadDeTrabajadores()
    {
        return trabajadoresAsignados.Count;
    }

    public bool agregarTrabajador(GameObject trabajador)
    {
        if (trabajadoresAsignados.Count < cantMaxTrabajadores)
        {
            if (!trabajadoresAsignados.Contains(trabajador))
            {
                trabajadoresAsignados.Add(trabajador);
            }
            else
            {
                Debug.LogWarning("ERROR: intentando agregar trabajador a fabrica a la que ya está asignado");
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameObject ObtenerFabricaCercanaConMasComida()
    {
        List<GameObject> cacheFabricasCercanas = new List<GameObject>(fabricasCercanas);

        GameObject cacheConMasComida = null;
        float cacheCantidadComidaMaxima = 0;

        foreach (GameObject fabrica in cacheFabricasCercanas)
        {
            float cacheComida = fabrica.GetComponent<FabricaDeComida>().ObtenerComidaProcesada();
            if (cacheComida > cacheCantidadComidaMaxima)
            {
                cacheCantidadComidaMaxima = cacheComida;
                cacheConMasComida = fabrica;
            }
        }
        if (cacheConMasComida != null)
        {
            return cacheConMasComida;
        }
        else
        {
            return cacheFabricasCercanas[Mathf.RoundToInt(Random.Range(0, cacheFabricasCercanas.Count))];   
        }
    }

    void ActualizarFabricasCercanas(float distanciaMaxima)
    {
        List<GameObject> todasLasFabricas = new List<GameObject>(gV.ObtenerTodasLasFabricas());
        fabricasCercanas.Clear();

        foreach (GameObject fabrica in todasLasFabricas)
        {
            if (Vector2.Distance(gameObject.transform.position, fabrica.transform.position) <= distanciaMaxima)
            {
                fabricasCercanas.Add(fabrica);
            }
        }
    }

    public void GetComida(GameObject sujeto)
    {
        if (comidaAlmacenada > 0)
        {
            sujeto.GetComponent<vida>().comidaActual = sP.SpawnearComida(transform.position);
            comidaAlmacenada--;
        }
    }

    public GameObject ObtenerSpotComer()
    {
        return spotsComer[Mathf.FloorToInt(Random.Range(0, 2))].gameObject;
    }

    public void RobarComida()
    {
        if(comidaAlmacenada > 20)
        {
            comidaAlmacenada -= 20;
        }
        else
        {
            comidaAlmacenada = 0;
        }
    }

    public void SumarRosquilla()
    {
        if(iteradorRosquillas < 24)
        {
            iteradorRosquillas++;
            rosquillas[iteradorRosquillas].SetActive(true);
        }

    }

    public void RestarRosquillas()
    {
        if(iteradorRosquillas > 0)
        {
            iteradorRosquillas--;
            rosquillas[iteradorRosquillas].SetActive(false);
        }
    }
}
