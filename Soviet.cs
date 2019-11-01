using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Soviet : MonoBehaviour
{
    float nextActionTime;
    float intervalo;

    public Transform puerta;

    [SerializeField]
    List<GameObject> miembrosComunidad;

    [SerializeField]
    List<GameObject> cosechadores;
    [SerializeField]
    int cosechadoresIdeal = 30; // 40 porciento 
    [SerializeField]
    List<GameObject> fabricadores;
    [SerializeField]
    int fabricadoresIdeal = 30; // 30 porciento
    [SerializeField]
    List<GameObject> distribuidores;
    [SerializeField]
    int distribuidoresIdeal = 38; // 18 porciento
    //Dejamos el 10 porciento libre para ir a la muralla

    [SerializeField]
    List<GameObject> fabricasCercanas;
    [SerializeField]
    List<GameObject> arbustosCercanos;
    [SerializeField]
    List<GameObject> almacenesCercanos;
    [SerializeField]
    float distanciaFabricasCercanas;
    [SerializeField]
    float distanciaAlmacenesCeranos;
    [SerializeField]
    float distanciaArbustosCercanos;

    LineRenderer lr;

    Vector3 colorLinea;

    globalVariables gV;
        
    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = 0f;
        intervalo = 5f;

        miembrosComunidad = new List<GameObject>();
        cosechadores = new List<GameObject>();
        fabricadores = new List<GameObject>();
        distribuidores = new List<GameObject>();

        fabricasCercanas = new List<GameObject>();
        arbustosCercanos = new List<GameObject>();
        almacenesCercanos = new List<GameObject>();

        cosechadoresIdeal = 35;
        fabricadoresIdeal = 20;
        distribuidoresIdeal = 35;

        distanciaFabricasCercanas = 65;
        distanciaArbustosCercanos = 85;
        distanciaAlmacenesCeranos = 125;

        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        lr = GetComponent<LineRenderer>();

    }

    private void Awake()
    {
        gV = GameObject.Find("GlobalManager").GetComponent<globalVariables>();
        lr = GetComponent<LineRenderer>();
        colorLinea = gV.ObtenerColorLineaSoviet();
        float r = Utilidades.Map(colorLinea.x, 0, 255, 0, 1, true);
        float g = Utilidades.Map(colorLinea.y, 0, 255, 0, 1, true);
        float b = Utilidades.Map(colorLinea.z, 0, 255, 0, 1, true);
        lr.startColor = new Color(r,g,b);
        lr.endColor = lr.startColor;

        ActualizarArbustosCercanos(distanciaArbustosCercanos);
        ActualizarFabricasCercanas(distanciaFabricasCercanas);
        ActualizarAlmacenesCercanos(distanciaAlmacenesCeranos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Cada 5 segundos actualizar almacen más vacio
            nextActionTime = Time.time + intervalo;
            ActualizarArbustosCercanos(distanciaArbustosCercanos);
            ActualizarFabricasCercanas(distanciaFabricasCercanas);
            ActualizarAlmacenesCercanos(distanciaAlmacenesCeranos);
        }
        int i = 0;
        foreach(GameObject sujeto in miembrosComunidad)
        {
            if(sujeto == null)
            {
                miembrosComunidad.Remove(sujeto);
                return;
            }
            if(miembrosComunidad.Count != lr.positionCount)
            {
                lr.positionCount = miembrosComunidad.Count;
            }
            lr.SetPosition(i, sujeto.transform.position);
            i++;
        }
    }

    //Esta funcion se llama desde el trabajador cuando llega al soviet
    public void BuscarTrabajo(GameObject trabajador)
    {
        //Si aun no pertenece a la comunidad,lo agregamos
        if (!miembrosComunidad.Contains(trabajador))
        {
            AgregarTrabajadorAComunidad(trabajador);
            trabajador.GetComponent<vida>().setearSovietComunidad(gameObject);
        }
        if (trabajador.GetComponent<vida>().TieneTrabajo() ||
            cosechadores.Contains(trabajador) || distribuidores.Contains(trabajador) || fabricadores.Contains(trabajador)) 
        {
            Debug.Log("se quiso buscar trabajo a uno que ya tiene");
            return;
        }


        //Si no hay al menos un trabajador en algun trabajo completamos eso
        if (cosechadores.Count < 1)
        {
            cosechadores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(1);
            return;
        }
        if(fabricadores.Count < 1)
        {
            fabricadores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(2);
            return;
        }
        if (distribuidores.Count < 1)
        {
            distribuidores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(3);
            return;
        }

        //Si hay al menos un trabajador en cada una, seguimos la jerarquía de la linea de produccion
        if (cosechadores.Count <= miembrosComunidad.Count * cosechadoresIdeal / 100)
        {
            cosechadores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(1);
            return;
        }
        if (fabricadores.Count <= miembrosComunidad.Count * fabricadoresIdeal / 100)
        {
            fabricadores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(2);
            return;
        }
        if (distribuidores.Count <= miembrosComunidad.Count * distribuidoresIdeal / 100)
        {
            distribuidores.Add(trabajador);
            trabajador.GetComponent<vida>().SetTrabajo(3);
            return;
        }

        trabajador.GetComponent<vida>().SetTrabajo(0);
    }

    //Este metodo es llamado desde el trabajador cuando llega a un soviet. Se pasa el mismo trabajador como parametro
    //de la funcion y se le devuelve el edificio para que pueda agendar a que comunidad pertenece
    public GameObject AgregarTrabajadorAComunidad(GameObject trabajador)
    {
        if (!miembrosComunidad.Contains(trabajador))
        {
            miembrosComunidad.Add(trabajador);
            return gameObject;
        }
        else
        {
            Debug.LogWarning("ERROR: intentando agregar trabajador a una comunidad en la que ya estaba");
            return null;
        }
    }

    public List<GameObject> ObtenerFabricasCercanasOrdenadasPorCantidadDeTrabajadores()
    {
        List<GameObject> cacheFabricasCercanas = new List<GameObject>(fabricasCercanas);
        List<GameObject> FabricasCercanasOrdenadasPorCantidadDeTrabajadores = new List<GameObject>();

        for (int i = 0; i < cacheFabricasCercanas.Count; i++)
        {
            GameObject cacheMasVacio = null;
            float cacheNumeroTrabajadoresMinimo = Mathf.Infinity;
            foreach (GameObject fabrica in cacheFabricasCercanas)
            {
                float cacheNumeroTrabajadores = fabrica.GetComponent<FabricaDeComida>().ObtenerCantidadDeTrabajadores();
                if (cacheNumeroTrabajadores < cacheNumeroTrabajadoresMinimo)
                {
                    cacheNumeroTrabajadoresMinimo = cacheNumeroTrabajadores;
                    cacheMasVacio = fabrica;
                }
            }
            cacheFabricasCercanas.Remove(cacheMasVacio);
            FabricasCercanasOrdenadasPorCantidadDeTrabajadores.Add(cacheMasVacio);
        }
        return FabricasCercanasOrdenadasPorCantidadDeTrabajadores;
    }
    public List<GameObject> ObtenerArbustosCercanosOrdenadosPorCantidadDeTrabajadores()
    {
        List<GameObject> cacheArbustosCercanos = new List<GameObject>(arbustosCercanos);
        List<GameObject> ArbustosCercanosOrdenadasPorCantidadDeTrabajadores = new List<GameObject>();

        for (int i = 0; i < cacheArbustosCercanos.Count; i++)
        {
            GameObject cacheMasVacio = null;
            float cacheNumeroTrabajadoresMinimo = Mathf.Infinity;
            foreach (GameObject arbusto in cacheArbustosCercanos)
            {
                float cacheNumeroTrabajadores = arbusto.GetComponent<ArbustoDeComida>().ObtenerCantidadDeTrabajadores();
                if (cacheNumeroTrabajadores < cacheNumeroTrabajadoresMinimo)
                {
                    cacheNumeroTrabajadoresMinimo = cacheNumeroTrabajadores;
                    cacheMasVacio = arbusto;
                }
            }
            cacheArbustosCercanos.Remove(cacheMasVacio);
            ArbustosCercanosOrdenadasPorCantidadDeTrabajadores.Add(cacheMasVacio);
        }
        return ArbustosCercanosOrdenadasPorCantidadDeTrabajadores;
    }
    public List<GameObject> ObtenerAlmacenesCercanosOrdenadosPorCantidadDeTrabajadores()
    {
        List<GameObject> cacheAlmacenesCercanos = new List<GameObject>(almacenesCercanos);
        List<GameObject> AlmacenesCercanosOrdenadasPorCantidadDeTrabajadores = new List<GameObject>();

        for (int i = 0; i < cacheAlmacenesCercanos.Count; i++)
        {
            GameObject cacheMasVacio = null;
            float cacheNumeroTrabajadoresMinimo = Mathf.Infinity;
            foreach (GameObject almacen in cacheAlmacenesCercanos)
            {
                float cacheNumeroTrabajadores = almacen.GetComponent<AlmacenDeComida>().ObtenerCantidadDeTrabajadores();
                if (cacheNumeroTrabajadores < cacheNumeroTrabajadoresMinimo)
                {
                    cacheNumeroTrabajadoresMinimo = cacheNumeroTrabajadores;
                    cacheMasVacio = almacen;
                }
            }
            cacheAlmacenesCercanos.Remove(cacheMasVacio);
            AlmacenesCercanosOrdenadasPorCantidadDeTrabajadores.Add(cacheMasVacio);
        }
        return AlmacenesCercanosOrdenadasPorCantidadDeTrabajadores;
    }

    void ActualizarFabricasCercanas(float distanciaComparacion)
    {
        List<GameObject> cacheFabricas = new List<GameObject>(gV.ObtenerTodasLasFabricas());
        fabricasCercanas.Clear();

        foreach (GameObject fabrica in cacheFabricas)
        {
            float distanciaAFabrica = Vector2.Distance(transform.position, fabrica.transform.position);
            if (distanciaAFabrica <= distanciaComparacion)
            {
                fabricasCercanas.Add(fabrica);
            }
        }
    }

    void ActualizarArbustosCercanos(float distanciaComparacion)
    {
        List<GameObject> cacheArbustos = new List<GameObject>(gV.ObtenerTodosLosArbustos());
        arbustosCercanos.Clear();

        foreach (GameObject arbusto in cacheArbustos)
        {
            float distanciaAArbusto = Vector2.Distance(transform.position, arbusto.transform.position);
            if (distanciaAArbusto <= distanciaComparacion)
            {
                arbustosCercanos.Add(arbusto);
            }
        }
    }

    void ActualizarAlmacenesCercanos(float distanciaComparacion)
    {
        List<GameObject> cacheAlmacenes = new List<GameObject>(gV.ObtenerTodosLosAlmacenes());
        almacenesCercanos.Clear();

        foreach (GameObject almacen in cacheAlmacenes)
        {
            float distanciaAAlmacen = Vector2.Distance(transform.position, almacen.transform.position);
            if (distanciaAAlmacen <= distanciaComparacion)
            {
                almacenesCercanos.Add(almacen);
            }
        }
    }

    public void EliminarDeComunidad(vida sujeto, GameObject lugarDeTrabajo, vida.trabajo t)
    {
        if (lugarDeTrabajo != null)
        {
            EliminarDeEmpleos(sujeto, lugarDeTrabajo, t);
        }
        if (miembrosComunidad.Contains(sujeto.gameObject))
        {
            miembrosComunidad.Remove(sujeto.gameObject);
        }
        else
        {
            Debug.LogWarning("Error: intentando eliminar un sujeto de una comunidad a la que no pertenece");
        }
    }
    public void EliminarDeEmpleos(vida sujeto, GameObject lugarDeTrabajo, vida.trabajo t)
    {
        if (t == vida.trabajo.cosechador)
        {
            lugarDeTrabajo.GetComponent<edificio>().EliminarTrabajador(sujeto.gameObject);
            if (cosechadores.Contains(sujeto.gameObject)){
                cosechadores.Remove(sujeto.gameObject);
            }
        }
        if (t == vida.trabajo.fabricante)
        {
            lugarDeTrabajo.GetComponent<edificio>().EliminarTrabajador(sujeto.gameObject);
            if (fabricadores.Contains(sujeto.gameObject))
            {
                fabricadores.Remove(sujeto.gameObject);
            }
        }
        if (t == vida.trabajo.distribuidor)
        {
            lugarDeTrabajo.GetComponent<edificio>().EliminarTrabajador(sujeto.gameObject);
            if (distribuidores.Contains(sujeto.gameObject))
            {
                distribuidores.Remove(sujeto.gameObject);
            }
        }
    }
}
