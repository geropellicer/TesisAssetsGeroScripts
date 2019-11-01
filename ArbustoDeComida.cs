using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbustoDeComida : edificio
{
    [SerializeField]
    public GameObject fabricaCercana;
    ParticleSystem ps;
    public ParticleSystem.EmissionModule psem;

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;


        trabajadoresAsignados = new List<GameObject>();
        comidaAlmacenada = Random.Range(50, 250);
        trabajoNecesarioBase = Random.Range(30,80);

        cantMaxTrabajadores = 6;

        ActualizarFabricaCercana();

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        ps = GetComponent<ParticleSystem>();

        psem = ps.emission;
        psem.enabled = false;

        //Ponemos una velocidad y un offset random para el viento (animacion) de los arbustos
        GetComponent<Animator>().SetFloat("velocidad", Random.Range(.6f, 1.2f));
        GetComponent<Animator>().SetFloat("offset", Random.Range(0, 1f));
        GetComponent<Animator>().SetTrigger("viento");
    }

    public float ObtenerTrabajoNecesario()
    {
        return (trabajoNecesarioBase + Random.Range(-100, 100));
    }


    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Cada 5 segundos actualizar almacen más vacio
            nextActionTime = Time.time + intervalo;
            ActualizarFabricaCercana();
        }
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
                Debug.LogWarning("ERROR: intentando asignar trabajador a arbusto al que ya esta asignado");
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void ActualizarFabricaCercana()
    {
        List<GameObject> cacheFabricas = new List<GameObject>(gV.ObtenerTodasLasFabricas());
        List<GameObject> fabricasOrdenadas = new List<GameObject>();

        for (int i = 0; i < cacheFabricas.Count; i++)
        {
            GameObject cacheMasCercana = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject fabrica in cacheFabricas)
            {
                float cacheDistancia = Vector2.Distance(fabrica.transform.position, transform.position);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercana = fabrica;
                }
            }
            cacheFabricas.Remove(cacheMasCercana);
            fabricasOrdenadas.Add(cacheMasCercana);
        }
        if (fabricasOrdenadas.Count > 0)
        {
            fabricaCercana = fabricasOrdenadas[0];
        }
        else
        {
            fabricaCercana = null;
        }
    }

    public int ObtenerCantidadDeTrabajadores()
    {
        return trabajadoresAsignados.Count;
    }

    public void CosecharComida(vida sujeto)
    {
        if (comidaAlmacenada > 0)
        {
            comidaAlmacenada--;
            sujeto.SumarComidaCosechada();
        }
    }
    public void CosecharComida(int n, vida sujeto)
    {
        for (int i = 0; i < n; i++)
        {
            CosecharComida(sujeto);
        }
    }
}
