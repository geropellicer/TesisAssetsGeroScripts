using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabricaDeComida : edificio
{
    [SerializeField]
    Transform[] spotsDeposito = new Transform[3];

    ParticleSystem ps;
    public ParticleSystem.EmissionModule psem;


    [SerializeField]
    GameObject almacenMasVacio;

    [SerializeField]
    List<GameObject> almacenesCercanos;

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        trabajoNecesarioBase = 30;

        almacenesCercanos = new List<GameObject>();
        trabajadoresAsignados = new List<GameObject>();

        ActualizarAlmacenMasVacio();
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();

        cantMaxTrabajadores = 8;

        ps = GetComponent<ParticleSystem>();

        psem = ps.emission;
        psem.enabled = false;

    }

    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Cada 5 segundos actualizar almacen más vacio
            nextActionTime = Time.time + intervalo;
            ActualizarAlmacenesCercanos(25);
            ActualizarAlmacenMasVacio();
        }
    }

    public float ObtenerTrabajoNecesario()
    {
        return (trabajoNecesarioBase + Random.Range(-100, 100));
    }


    public int ObtenerComidaCosechada()
    {
        return comidaCosechada;
    }

    public int ObtenerComidaProcesada()
    {
        return comidaProcesada;
    }

    public void TomarComidaCosechada(vida sujeto)
    {
        if (comidaCosechada > 0)
        {
            comidaCosechada--;
            sujeto.SumarComidaCosechada();
        }
    }
    public void TomarComidaCosechada(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            TomarComidaCosechada(sujeto);
        }
    }

    public void DejarComidaCosechada(vida sujeto)
    {
        comidaCosechada++;
        sujeto.RestarComidaCosechada();
    }
    public void DejarComidaCosechada(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            DejarComidaCosechada(sujeto);
        }
    }

    public void TomarComidaProcesada(vida sujeto)
    {
        if (comidaProcesada > 0)
        {
            comidaProcesada--;
            sujeto.SumarComidaFabricada();
        }
    }
    public void TomarComidaProcesada(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            TomarComidaProcesada(sujeto);
        }
    }

    public void DejarComidaProcesada(vida sujeto)
    {
        comidaProcesada++;
        sujeto.RestarComidaFabricada();
    }
    public void DejarComidaProcesada(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            DejarComidaProcesada(sujeto);
        }
    }

    void ActualizarAlmacenMasVacio()
    {
        List<GameObject> cacheAlmacenesCercanos = new List<GameObject>(almacenesCercanos);

        GameObject cacheMasVacio = null;
        float cacheComidaAlmacenadaMinima = Mathf.Infinity;
        foreach (GameObject almacen in cacheAlmacenesCercanos)
        {
            float cacheComidaAlmacenada = almacen.GetComponent<AlmacenDeComida>().ObtenerComidaAlmacenada();
            if (cacheComidaAlmacenada < cacheComidaAlmacenadaMinima)
            {
                cacheComidaAlmacenadaMinima = cacheComidaAlmacenada;
                cacheMasVacio = almacen;
                //cacheAlmacenesCercanos.Remove(cacheMasVacio);
            }
        }
        almacenMasVacio = cacheMasVacio;
    }

    void ActualizarAlmacenesCercanos(float distanciaMaxima)
    {
        List<GameObject> todosLosAlmacenes = new List<GameObject>();
        todosLosAlmacenes.AddRange(gV.ObtenerTodosLosAlmacenes());

        almacenesCercanos.Clear();
        foreach(GameObject almacen in todosLosAlmacenes)
        {
            if(Vector2.Distance(gameObject.transform.position, almacen.transform.position) <= distanciaMaxima)
            {
                almacenesCercanos.Add(almacen);
            }
        }
    }

    public GameObject ObtenerAlmacenMasVacio()
    {
        return almacenMasVacio;
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


    public Vector3 GetPosicionDeposito()
    {
        return spotsDeposito[Mathf.FloorToInt(Random.Range(0, 2))].position;
    }

}
