using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;

public class vida : MonoBehaviour
{
    GameObject globalManager;
    globalVariables gV;
    RVOController rvo;
    AIPath aip;
    Animator an;
    GeroDestinationSetter gds;

    public float distanciaAceptableLlegar;

    [SerializeField]
    int segundosDeVidaTotales;
    [SerializeField]
    int segundosDeVida;
    float velocidadDeDesplazamientoMinima;
    float velocidadDeDesplazamientoMaxima;
    float velocidadDeDesplazamientoPromedio;
    [SerializeField]
    float velocidadDeDesplazamientoActual;
    float velocidadDeRotacion;

    //Reproduccion
    [SerializeField]
    float porcentajeMinimoVidaReproduccion;
    [SerializeField]
    float porcentajeMaximoVidaReproduccion;
    [SerializeField]
    float ganasDeReproducirse;
    int cantidadDeHijos;
    List<GameObject> hijos;
    GameObject padre;
    GameObject madre;
    [SerializeField]
    GameObject parejaEncontrada;
    [SerializeField]
    Vector3 puntoAExplorarPareja;
    [SerializeField]
    Vector3 puntoDeEncuentroConPareja;
    float totalTiempoRitual;
    float tiempoActualRitual;
    List<GameObject> ultimosHijosNacidos;
    Vector3 nuevoPuntoRitualDeReproduccion;

    //SECCION HAMBRE
    //El bajo es el que van a consultar si casualmente se cruza con comida
    //El alto es el que aumenta el mal humor y lo hace parar sus actividades para alimentarse
    [SerializeField]
    float umbralDeHambreBajo;
    [SerializeField]
    float umbralDeHambreAlto;
    [SerializeField]
    float hambreActual;
    [SerializeField]
    public GameObject comidaActual;
    float tiempoActualEsperandoSinComida;
    float esperarSinComida;


    //Trabajo
    [SerializeField]
    GameObject sovietComunidad;
    [SerializeField]
    Vector3 posLugarDeTrabajo;
    [SerializeField]
    GameObject lugarDeTrabajo;
    float tiempoTareaActual;
    float capacidadDeTrabajo;
    bool tieneTarea;
    [SerializeField]
    int comidaCosechada;
    [SerializeField]
    int comidaFabricada;
    [SerializeField]
    int comidaQuePuedeLlevar;
    Vector3 posFabrica1;
    Vector3 posFabrica2;
    bool tieneFabricaCercanaAsignada;
    [SerializeField]
    GameObject fabricaCercanaAsignada;

    //Resistiendo
    Transform personaEnemiga;

    //Construccion
    [SerializeField]
    GameObject spotConstruccion;


    enum estado
    {
        idle,
        comiendo,
        reproduciendose,
        trabajando,
        construyendo,
        resistiendo
    }
    [SerializeField]
    estado estadoActual;
    List<estado> historialDeEstados;

    enum idle
    {
        buscandoLugar,
        caminando,
        rumiando
    }
    [SerializeField]
    idle subEstadoActualIdle;

    enum comiendo
    {
        buscandoComida,
        caminando,
        caminandoSinComida,
        esperandoSinComida,
        comiendo
    }
    [SerializeField]
    comiendo subEstadoActualComiendo;

    
    enum reproduciendose
    {
        buscandoPareja,
        caminando,
        caminandoHaciaPareja,
        reproduciendose
    }
    [SerializeField]
    reproduciendose subEstadoActualReproduciendose;

    enum trabajando
    {
        buscandoTrabajo,
        caminandoAlSoviet,
        caminandoAlTrabajo,
        trabajando,
    }
    [SerializeField]
    trabajando subEstadoActualTrabajando;

    public enum trabajo
    {
        cosechador,
        fabricante,
        distribuidor
    }
    [SerializeField]
    trabajo trabajoActual;

    enum cosechando
    {
        cosechando,
        dejandoComidaCosechada
    }
    [SerializeField]
    cosechando subEstadoCosechando;

    enum fabricando
    {
        fabricando,
        dejandoComidaFabricada
    }
    [SerializeField]
    fabricando subEstadoFabricando;

    enum distribuyendo
    {
        dejandoComidaProcesada,
        buscandoComidaProcesada
    }
    [SerializeField]
    distribuyendo subEstadoDistribuyendo;

    enum construyendo
    {
        caminandoAConstruccion,
        construyendo
    }
    [SerializeField]
    construyendo subEstadoActualConstruyendo;

    enum resistiendo
    {
        decidiendo,
        yendoAPersona,
        atacandoAPersona,
        buscandoLugarIdle,
        yendoALugarIdle,
        rumiando
    }
    [SerializeField]
    resistiendo subEstadoActualResistiendo;


    float nextActionTime;
    float segundo;

    //Van de 0 a 100
    [SerializeField]
    float felicidadActual;
    [SerializeField]
    float saludActual;

    [SerializeField]
    Vector3 destinoIdle;
    float tiempoRumiar;
    float tiempoActualRumiar;

    GameObject destinoComida;
    float cachePorcionComida;

    [SerializeField]
    bool retirada = false;
    [SerializeField]
    bool retiradaPreparada = false;
    [SerializeField]
    Transform puntoRetirada;

    // Start is called before the first frame update
    void Start()
    {
        tag = "sujeto";

        globalManager = GameObject.Find("GlobalManager");
        gV = globalManager.GetComponent<globalVariables>();

        rvo = GetComponent<RVOController>();
        aip = GetComponent<AIPath>();
        gds = GetComponent<GeroDestinationSetter>();

        segundosDeVidaTotales = Random.Range(segundosDeVidaTotales - 50, segundosDeVidaTotales + 50);
        segundosDeVida = 0;

        nextActionTime = 0f;
        segundo = 1f;

        felicidadActual = Random.Range(25, 75);
        saludActual = Random.Range(25, 75);

        velocidadDeDesplazamientoMinima = Random.Range(0.33f, 2);
        velocidadDeDesplazamientoMaxima = Random.Range(5, 10);
        velocidadDeDesplazamientoPromedio = (velocidadDeDesplazamientoMinima + velocidadDeDesplazamientoMaxima) / 2;
        velocidadDeDesplazamientoActual = 0;
        velocidadDeRotacion = Random.Range(3, 7);

        hambreActual = Random.Range(25, 75);
        umbralDeHambreBajo = Random.Range(66, 80);
        umbralDeHambreAlto = Random.Range(80, 100);
        tiempoActualEsperandoSinComida = 0;
        esperarSinComida = 0;


        porcentajeMinimoVidaReproduccion = Random.Range(segundosDeVidaTotales * 15 / 100, segundosDeVidaTotales * 33 / 100);
        porcentajeMaximoVidaReproduccion = Random.Range(segundosDeVidaTotales * 45 / 100, segundosDeVidaTotales * 66 / 100);
        totalTiempoRitual = 266;
        hijos = new List<GameObject>();
        ultimosHijosNacidos = new List<GameObject>();

        estadoActual = estado.idle;
        subEstadoActualIdle = idle.buscandoLugar;
        subEstadoActualComiendo = comiendo.buscandoComida;
        subEstadoActualReproduciendose = reproduciendose.buscandoPareja;
        subEstadoActualTrabajando = trabajando.buscandoTrabajo;
        subEstadoActualConstruyendo = construyendo.caminandoAConstruccion;
        subEstadoActualResistiendo = resistiendo.decidiendo;

        subEstadoCosechando = cosechando.cosechando;
        subEstadoFabricando = fabricando.fabricando;
        subEstadoDistribuyendo = distribuyendo.buscandoComidaProcesada;

        historialDeEstados = new List<estado>();

        float newScale = Random.Range(.5f, 1f);
        transform.localScale = new Vector3(newScale, newScale, newScale);

        capacidadDeTrabajo = Random.Range(1, 3);
        comidaQuePuedeLlevar = 5;

        lugarDeTrabajo = null;

        gV.SumarPoblacion();

        an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!retirada)
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime = Time.time + segundo;

                if (segundosDeVida >= segundosDeVidaTotales)
                {
                    Morir();
                }
                //Hacemos que avance la vida
                segundosDeVida++;
                hambreActual++;

                //Que el hambre  afecte la felicidad
                if (hambreActual > umbralDeHambreBajo)
                {
                    felicidadActual--;
                }
                //Aumentamos las ganas de reproducirse si esta en la edad correspondiente
                if (segundosDeVida > porcentajeMinimoVidaReproduccion && segundosDeVida < porcentajeMaximoVidaReproduccion)
                {
                    ganasDeReproducirse += Random.Range(0, 10);
                }
                else if (segundosDeVida > porcentajeMaximoVidaReproduccion && ganasDeReproducirse > 0)
                {
                    ganasDeReproducirse -= Random.Range(0.5f, 2);
                    if (ganasDeReproducirse < 0)
                    {
                        ganasDeReproducirse = 0;
                    }
                }
                if (estadoActual != estado.trabajando && estadoActual != estado.construyendo)
                {
                    //Si el hambre supera el humral alto, vamos a comer antes que nada
                    if (hambreActual >= umbralDeHambreAlto)
                    {
                        CambiarDeEstado(estado.comiendo);
                        return;
                    }
                    //Si hay ganas de reproducirse y no hay hambre urgente
                    if (ganasDeReproducirse > 50 && hambreActual < umbralDeHambreAlto)
                    {
                        CambiarDeEstado(estado.reproduciendose);
                        return;
                    }
                }


                //La segunda prioridad es que trabaje enn el horario de las primeras 5 horas si no está construyendo nada
                if (estadoActual != estado.construyendo && estadoActual != estado.resistiendo && gV.horaActual >= 3 && gV.sovietComunidades.Count > 0)
                {
                    velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                    ActualizarVelAn();
                    CambiarDeEstado(estado.trabajando);
                    return;
                }

                //Si son mas de las 5 sale de trabajar
                if (estadoActual == estado.trabajando && gV.horaActual < 3)
                {
                    subEstadoActualTrabajando = trabajando.buscandoTrabajo;
                    velocidadDeDesplazamientoActual = velocidadDeDesplazamientoPromedio;
                    ActualizarVelAn();
                    CambiarDeEstado(estado.idle);
                    return;
                }
            }

            switch (estadoActual)
            {
                case estado.idle:
                    DecidirSubestadoIdle();
                    break;
                case estado.comiendo:
                    DecidirSubestadoComiendo();
                    break;
                case estado.reproduciendose:
                    DecidirSubestadoReproduciendose();
                    break;
                case estado.resistiendo:
                    DecidirSubestadoResistiendo();
                    break;
                case estado.trabajando:
                    DecidirSubestadoTrabajando();
                    break;
                case estado.construyendo:
                    DecidirSubestadoConstruyendo();
                    break;
                default:
                    Debug.LogWarning("no hay estado seleccionado");
                    break;
            }

            //
            //TODO: Desactivar
            //transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
        }
        else
        {
            if (!retiradaPreparada)
            {
                GetComponent<GeroDestinationSetter>().enabled = false;
                puntoRetirada = gV.ObtenerPuntoRetiradaMasCercano(transform.position);
                velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                ActualizarVelAn();
                an.SetTrigger("caminandoLanza");
                retiradaPreparada = true;
            }
            else
            {
                Utilidades.LookAt2D(transform, puntoRetirada, +90);
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(puntoRetirada.transform.position.x, puntoRetirada.transform.position.y), .3f);
            }
        }
    }


    /*
     * ********************************************************************************************* *
     * ********************************************************************************************* *
     * **********************  FUNCIONES GENERALES **************************************************************** *
     * ********************************************************************************************* *
     * ********************************************************************************************* *
     */

     //TODO: agregar animacion muerte y un timer para que se reproduzca la animacion y recién después se elimine el gamobject

    void Morir()
    {
        gV.RestarPoblacion();
        if(estadoActual == estado.construyendo)
        {
            EliminarDeConstruccion();
        }
        if (sovietComunidad != null)
        {
            sovietComunidad.GetComponent<Soviet>().EliminarDeComunidad(GetComponent<vida>(), lugarDeTrabajo, trabajoActual);
        }
        Destroy(gameObject);
    }

    void Comer(GameObject comida)
    {
        if (hambreActual > umbralDeHambreBajo)
        {
            hambreActual -= comida.GetComponent<comida>().GetPorcion();
            saludActual += comida.GetComponent<comida>().GetValorNutricional();
            felicidadActual += comida.GetComponent<comida>().GetGusto();
            Destroy(comida);
        }
    }

    void CambiarDeEstado(estado nuevoEstado)
    {
        if(estadoActual == estado.trabajando && estadoActual != estado.trabajando && gV.horaActual >= 5)
        {
            Debug.Log("Cambiando de estado desde trabajando a otro después de las 5");
        }
        if(estadoActual == estado.resistiendo && nuevoEstado != estado.resistiendo)
        {
            /*Debug.Log("Saliendo de resistiendo");
            ReiniciarResistiendo();*/
        }

        // Cambiamos de estado solo si el estado destino es distinto al estado en el que estamos
        if (nuevoEstado != estadoActual)
        {
            if(estadoActual == estado.idle)
            {
                subEstadoActualIdle = idle.buscandoLugar;
            }
            if (estadoActual == estado.construyendo)
            {
                EliminarDeConstruccion();
            }
            if (estadoActual == estado.trabajando)
            {
                if (sovietComunidad != null && lugarDeTrabajo != null)
                {
                    sovietComunidad.GetComponent<Soviet>().EliminarDeEmpleos(GetComponent<vida>(), lugarDeTrabajo, trabajoActual);
                    subEstadoActualTrabajando = trabajando.buscandoTrabajo;
                }
                tieneTarea = false;
            }
            if(estadoActual == estado.comiendo)
            {
                subEstadoActualComiendo = comiendo.buscandoComida;
            }
            if(estadoActual == estado.resistiendo)
            {
                aip.endReachedDistance = 4.34f;
            }
            historialDeEstados.Add(estadoActual);
            estadoActual = nuevoEstado;
        }
        
    }

    public float ObtenerFelicidad()
    {
        return felicidadActual;
    }

    public float ObtenerGanasDeReproducirse()
    {
        return ganasDeReproducirse;
    }

    public bool ObtenerEstaDisponible()
    {
        if (estadoActual == estado.reproduciendose && parejaEncontrada == null)
        {
            return true;
        }
        return false;
    }

    float SetVelocidadRandom()
    {
        return Random.Range(velocidadDeDesplazamientoMinima, velocidadDeDesplazamientoMaxima);
    }

    void ActualizarVelAn()
    {
        an.SetFloat("velocidad", .33f + Utilidades.Map(velocidadDeDesplazamientoActual, 0f, 10f, 0f, 1f, true));
    }

    void ActualizarVelAn(float vel)
    {        
        if(vel == 0)
        {
            an.SetFloat("velocidad", 0);
        }
        else
        {
            an.SetFloat("velocidad", .33f + Utilidades.Map(vel, 0f, 10f, 0f, 1f, true));
        }
    }

    public void Atacar()
    {
        felicidadActual -= 20;
    }

    /*
     * ********************************************************************************************* *
     * ********************************************************************************************* *
     * **********************  IDLE **************************************************************** *
     * ********************************************************************************************* *
     * ********************************************************************************************* *
     */

    void DecidirSubestadoIdle()
    {
        if(subEstadoActualIdle == idle.buscandoLugar)
        {
            BuscarDestinoIdle();
            velocidadDeDesplazamientoActual = SetVelocidadRandom();
            aip.maxSpeed = velocidadDeDesplazamientoActual;
            gds.SetDestination(destinoIdle);
            subEstadoActualIdle = idle.caminando;

            an.SetTrigger("caminando");
            ActualizarVelAn();

        } else if(subEstadoActualIdle == idle.caminando)
        {
            if (aip.reachedDestination)
            {
                subEstadoActualIdle = idle.rumiando;
                ActualizarVelAn(0);
                SetTiempoRumiar();
            }
        } else if(subEstadoActualIdle == idle.rumiando)
        {
            //Chequear el contador random y si se cumple, vaciar el destino idle y pasarlo a buscando objetivo
            if (tiempoActualRumiar >= tiempoRumiar)
            {
                destinoIdle = Vector3.zero;
                subEstadoActualIdle = idle.buscandoLugar;
                tiempoActualRumiar = 0;
            }
            else
            {
                tiempoActualRumiar++;
            }
        }
        else
        {
            Debug.LogError("ERROR: no hay un subestado iddle seleccionado.");
        }      
    }

    void BuscarDestinoIdle()
    {
        destinoIdle = Utilidades.PuntoRandom(gV.piso, transform.position, 25);
    }

    void SetTiempoRumiar()
    {
        tiempoRumiar = Random.Range(120, 500);
    }


    /*
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    * **********************  COMIENDO **************************************************************** *
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    */

    void DecidirSubestadoComiendo()
    {
        if (subEstadoActualComiendo == comiendo.buscandoComida)
        {
            BuscarDestinoComida();
            if (destinoComida != null)
            {
                velocidadDeDesplazamientoActual = SetVelocidadRandom();
                aip.maxSpeed = velocidadDeDesplazamientoActual;
                gds.SetDestination(destinoComida.transform.position);
                subEstadoActualComiendo = comiendo.caminando;

                ActualizarVelAn();
                an.SetTrigger("caminandoCartelComida");
            }
            //ACa entramos cunaod no hya comida. Podriamos ir enfureciendolo
            else
            {
                velocidadDeDesplazamientoActual = SetVelocidadRandom();
                aip.maxSpeed = velocidadDeDesplazamientoActual;
                destinoIdle = Utilidades.PuntoRandom(gV.piso, transform.position, 30);
                gds.SetDestination(destinoIdle);

                ActualizarVelAn();
                an.SetTrigger("caminandoCartelComida");
                subEstadoActualComiendo = comiendo.caminandoSinComida;

            }
        } else if (subEstadoActualComiendo == comiendo.caminandoSinComida) {
            //TODO: AJUSTAR INFELICIDAD
            felicidadActual--;
            if (aip.reachedDestination)
            {
                subEstadoActualComiendo = comiendo.esperandoSinComida;

                ActualizarVelAn(0);
                an.SetTrigger("usandoCartelComida");

                tiempoActualEsperandoSinComida = 0;
                esperarSinComida = Random.Range(60, 250);
            }
        } else if (subEstadoActualComiendo == comiendo.esperandoSinComida) {
            //TODO: AJUSTAR INFELICIDAD
            felicidadActual--;
            if (tiempoActualEsperandoSinComida < esperarSinComida)
            {
                tiempoActualEsperandoSinComida++;
            }
            else
            {
                subEstadoActualComiendo = comiendo.buscandoComida;
            }
        } else if (subEstadoActualComiendo == comiendo.caminando)
        {
            if (aip.reachedDestination)
            {
                subEstadoActualComiendo = comiendo.comiendo;
                //ca.CambiarAnimacion(customAnimator.animacion.comiendo);
                an.SetTrigger("comiendo");
                ActualizarVelAn(0);
            }
        } else if (subEstadoActualComiendo == comiendo.comiendo)
        {
            if (comidaActual == null)
            {
                destinoComida.transform.parent.GetComponent<AlmacenDeComida>().GetComida(gameObject);
                destinoComida.transform.parent.GetComponent<AlmacenDeComida>().RestarRosquillas();
                if (comidaActual == null)
                {
                    Debug.Log("No hay mas comida");
                    return;
                }
            }
            //Vamos comiendo la porcion
            if (cachePorcionComida < comidaActual.GetComponent<comida>().GetPorcion())
            {
                hambreActual--;
                cachePorcionComida++;
                saludActual += comidaActual.GetComponent<comida>().GetValorNutricional();
                felicidadActual += comidaActual.GetComponent<comida>().GetGusto();
            }
            else
            {
                Destroy(comidaActual);
                destinoComida = null;
                comidaActual = null;
                if (hambreActual < umbralDeHambreBajo)
                {
                    //Si ya no tenemos hambre, vamos a hacer otra cosa
                    //ca.CambiarAnimacion(customAnimator.animacion.caminando);
                    an.SetTrigger("idle");
                    CambiarDeEstado(estado.idle);
                    transform.localScale += new Vector3(.1f, .1f, .1f);
                    return;
                }
                else
                {
                    //Si aun tenemos, buscamos de nuevo
                    an.SetTrigger("usandoCartelComida");
                    subEstadoActualComiendo = comiendo.buscandoComida;
                }
            }

        }
        else
        {
            Debug.LogError("ERROR: No hay un subestado de comiendo seleccionado");
        }
    }


    void BuscarDestinoComida()
    {
        //gV.ObtenerComidas();
        //destinoComida = OrdenarComidasMasCercanas()[0];
        if (OrdenarAlmacenesMasCercanosConComida().Count > 0)
        {
            destinoComida = OrdenarAlmacenesMasCercanosConComida()[0].GetComponent<AlmacenDeComida>().ObtenerSpotComer();
        }
        else
        {
            destinoComida = null;
            return;
        }
        cachePorcionComida = 0;
    }

    List<GameObject> OrdenarAlmacenesMasCercanosConComida()
    {
        List<GameObject> cacheTodosLosAlmacenes = new List<GameObject>(gV.almacenesDeComida);
        List<GameObject> almacenesOrdenados = new List<GameObject>();

        for (int i = 0; i < cacheTodosLosAlmacenes.Count; i++)
        {
            GameObject cacheMasCercano = null;
            float cacheDistanciaMinima = Mathf.Infinity;
            foreach (GameObject almacen in cacheTodosLosAlmacenes)
            {
                float cacheDistancia = Vector2.Distance(almacen.transform.position, transform.position);
                if (cacheDistancia < cacheDistanciaMinima)
                {
                    cacheDistanciaMinima = cacheDistancia;
                    cacheMasCercano = almacen;
                }
            }
            cacheTodosLosAlmacenes.Remove(cacheMasCercano);
            if (cacheMasCercano.GetComponent<AlmacenDeComida>().ObtenerComidaAlmacenada() > 0)
            {
                almacenesOrdenados.Add(cacheMasCercano);
            }
        }

        return almacenesOrdenados;
    }

    /*
* ********************************************************************************************* *
* ********************************************************************************************* *
* **********************  REPRODUCIENDOSE **************************************************************** *
* ********************************************************************************************* *
* ********************************************************************************************* *
*/
    /*
     * PRIMERO SE TIRA UN RAYCAST 360: Se avanza en cualquier sentido al azar que se encuentra un sujeto. 
     * si no hay sujetos encontrados, se selecciona un punto random. 
     * Si encuentra a alguien y está a mas de x(5) unidades, avanza hacia allá hasta llegar a ese lugar.
     * Si no encuentra a alguien y esta a menos de 5, pregunta por su nivel de reproduccion. Si matchean, 
     * se seleccionan mutuamente como pareja.
     * Van hasta estar a una distnacia minima y ahi se reproducen
     * */


    void DecidirSubestadoReproduciendose()
    {
        if(subEstadoActualReproduciendose == reproduciendose.buscandoPareja)
        {
            //Desde adentro de esta funcion deberiamos cambiar a caminando o a caminando hacia pareja
            DecidirDireccionBuscarPareja();
        } else if(subEstadoActualReproduciendose == reproduciendose.caminando)
        {
            if (aip.reachedDestination)
            {
                subEstadoActualReproduciendose = reproduciendose.buscandoPareja;
            }
        } else if(subEstadoActualReproduciendose == reproduciendose.caminandoHaciaPareja)
        {
            if (aip.reachedDestination)
            {
                subEstadoActualReproduciendose = reproduciendose.reproduciendose;
                an.SetTrigger("cogiendo");
            }
        } else if(subEstadoActualReproduciendose == reproduciendose.reproduciendose)
        {
            StartCoroutine(HacerRitualDeReproduccion());
        }
        else
        {
            Debug.LogError("ERRROR: no hay ningun subestado reproduciendose seleccionado");
        }

    }

    void DecidirDireccionBuscarPareja()
    {
        //Se subdivide en mirar si hay alguien a menos de x(5) distancia (caso afirmativo explora posible matcheo) o sino, buscar
        //alguien en la redonda

        //Esfera 1
        //Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, gV.piso.GetComponent<MeshRenderer>().bounds.size.z / 3);
        List<Collider2D> cols = new List<Collider2D>();
        Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), gV.piso.GetComponent<MeshRenderer>().bounds.size.x / 4, new ContactFilter2D().NoFilter(), cols);
        int i = 0;
        while (i < cols.Count)
        {
            if (cols[i].gameObject.tag == "sujeto" && !cols[i].gameObject.Equals(gameObject))
            {
                if (cols[i].GetComponent<vida>().ObtenerGanasDeReproducirse() > 50 &&
                    cols[i].GetComponent<vida>().ObtenerEstaDisponible())
                {
                    parejaEncontrada = cols[i].gameObject;
                    cols[i].GetComponent<vida>().Enamorar(gameObject, DecidirPuntoDeEncuentroConPareja());
                    puntoDeEncuentroConPareja = DecidirPuntoDeEncuentroConPareja();

                    //Antes de salir le mandamos toda la data al RVO para que se mueva
                    subEstadoActualReproduciendose = reproduciendose.caminandoHaciaPareja;
                    velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                    aip.maxSpeed = velocidadDeDesplazamientoActual;
                    gds.SetDestination(puntoDeEncuentroConPareja);
                    //Animacion
                    ActualizarVelAn();
                    an.SetTrigger("caminandoEnamorado");

                    return;
                }
            }
            i++;
        }

        //Esfera 2
        //Collider[] hitColliders2 = Physics.OverlapSphere(gameObject.transform.position, gV.piso.GetComponent<MeshRenderer>().bounds.size.z / 2);
        List<Collider2D> cols2 = new List<Collider2D>();
        Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), gV.piso.GetComponent<MeshRenderer>().bounds.size.x / 4, new ContactFilter2D().NoFilter(), cols);

        int j = 0;
        //Se llega a esta linea solo si no encontró pareja en la esfera pequeña
        while (j < cols2.Count)
        {
            if (cols2[j].gameObject.tag == "sujeto")
            {
                //Para evitar que se detecte a sí mismo o a sus componentes hijos
                if (cols2[j].gameObject.Equals(gameObject) == false && cols2[j].gameObject.Equals(gameObject.transform.GetChild(0).gameObject) == false)
                {
                    puntoAExplorarPareja = cols2[j].gameObject.transform.position;
                    subEstadoActualReproduciendose = reproduciendose.caminando;

                    //Antes de salir le mandamos toda la data al RVO para que se mueva
                    velocidadDeDesplazamientoActual = SetVelocidadRandom();
                    aip.maxSpeed = velocidadDeDesplazamientoActual;
                    gds.SetDestination(puntoAExplorarPareja);
                    //Animacion
                    ActualizarVelAn();
                    an.SetTrigger("caminandoEnamorado");

                    return;
                }
            }
            j++;
        }

        //Se llega a esta linea solo si no encontro a nadie en niguna instancia
        puntoAExplorarPareja = Utilidades.PuntoRandom(gV.piso, transform.position, 40);
        subEstadoActualReproduciendose = reproduciendose.caminando;

        //Antes de salir le mandamos toda la data al RVO para que se mueva
        velocidadDeDesplazamientoActual = SetVelocidadRandom();
        aip.maxSpeed = velocidadDeDesplazamientoActual;
        gds.SetDestination(puntoAExplorarPareja);
        //Animacion
        ActualizarVelAn();
        an.SetTrigger("caminandoEnamorado");
    }

    public void Enamorar(GameObject romeo, Vector3 puntoDeEncuentro)
    {
        parejaEncontrada = romeo;
        CambiarDeEstado(estado.reproduciendose);
        subEstadoActualReproduciendose = reproduciendose.caminandoHaciaPareja;
        puntoDeEncuentroConPareja = puntoDeEncuentro;

        //Antes de salir le mandamos toda la data al RVO para que se mueva
        velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
        aip.maxSpeed = velocidadDeDesplazamientoActual;
        gds.SetDestination(puntoDeEncuentroConPareja);
        //Animacion
        ActualizarVelAn();
        an.SetTrigger("caminandoEnamorado");
    }

    Vector3 DecidirPuntoDeEncuentroConPareja()
    {
       /* if (parejaEncontrada == null)
        {
            //Debug.LogWarning("error. Se quiere consensuar un punto en comun con una pareja no existente");
            return Vector3.zero;
        }

        float x1, y1, x2, y2, z1, z2, newX, newY, newZ;
        x1 = gameObject.transform.position.x;
        x2 = parejaEncontrada.transform.position.x;
        y1 = gameObject.transform.position.y;
        y2 = parejaEncontrada.transform.position.y;
        z1 = gameObject.transform.position.z;
        z2 = parejaEncontrada.transform.position.z;

        newX = (x1 + x2) / 2;
        newY = (y1 + y2) / 2;
        newZ = -0.1f;*/

        Vector3 resultado = Utilidades.PuntoRandom(gV.piso, transform.position, 40);

        //Hasta acá todo normal pero tenemos que cerciorarnos que ese punto es caminable.
        /* GraphNode node = AstarPath.active.GetNearest(resultado, NNConstraint.Default).node;

         return (Vector3)node.position;*/
        return resultado;
    }

    IEnumerator HacerRitualDeReproduccion()
    {
        for (int i = 0; i < totalTiempoRitual; i++)
        {
            tiempoActualRitual++;

            //Si por alguna razon abandona la pareja
            if (parejaEncontrada == null || parejaEncontrada.GetComponent<vida>().estadoActual != estado.reproduciendose)
            {
                TerminarReproduccion(false);
                yield break;
            }
            else
            {
                yield return null;
            }
        }

        int cantHijosCamadaActual = Mathf.RoundToInt(Random.Range(1, 6));
        for (int i = 0; i < cantHijosCamadaActual; i++)
        {
            StartCoroutine(NacerHijo());
        }
        TerminarReproduccion(cantHijosCamadaActual);
    }

    IEnumerator NacerHijo()
    {
        ultimosHijosNacidos.Add(Instantiate(gV.prefabBicho, transform.position, transform.rotation));
        ultimosHijosNacidos[ultimosHijosNacidos.Count - 1].GetComponent<vida>().ConfigurarComoHijo(gameObject);
        yield return new WaitForSeconds(.5f);
    }

    void TerminarReproduccion(int cantHijos)
    {
        ganasDeReproducirse = 0;
        felicidadActual += 50;
        cantidadDeHijos += cantHijos;
        hijos.AddRange(ultimosHijosNacidos);
        parejaEncontrada = null;
        puntoAExplorarPareja = Vector3.zero;
        puntoDeEncuentroConPareja = Vector3.zero;
        ultimosHijosNacidos.Clear();
        tiempoActualRitual = 0;
        CambiarDeEstado(estado.idle);
    }
    void TerminarReproduccion(bool caso)
    {
        if (caso == false)
        {
            parejaEncontrada = null;
            puntoAExplorarPareja = Vector3.zero;
            puntoDeEncuentroConPareja = Vector3.zero;
            ultimosHijosNacidos.Clear();
            tiempoActualRitual = 0;
            CambiarDeEstado(estado.idle);
        }
        else
        {
            Debug.LogError("ERROR: Se salio de la reproduccion por una excepecion pero no se encuentra la excepcion");
        }
    }

    public void ConfigurarComoHijo(GameObject padre)
    {
        this.padre = padre;
        madre = padre.GetComponent<vida>().ObtenerPareja();
    }

    public GameObject ObtenerPareja()
    {
        return parejaEncontrada;
    }



    /*
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    * **********************  TRABAJANDO  ********************************************************* *
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    */
    /* Para buscar trabajo: va hasta el centro mas cercano, se incorpora a esa comunidad y se fija que hay para hacer
     * 
     * 
     * */

    public bool TieneTrabajo()
    {
        if(lugarDeTrabajo == null)
        {
            return false;
        }
        return true;
    }

    public void setearSovietComunidad(GameObject g)
    {
        sovietComunidad = g;
    }

    void DecidirSubestadoTrabajando()
    {
        if (subEstadoActualTrabajando == trabajando.buscandoTrabajo)
        {
            if (sovietComunidad == null)
            {
                BuscarSovietComunidad();
                return;
            }

            if (lugarDeTrabajo != null)
            {
                lugarDeTrabajo.GetComponent<edificio>().EliminarTrabajador(gameObject);
                lugarDeTrabajo = null;
                posLugarDeTrabajo = Vector3.zero;
            }

            subEstadoActualTrabajando = trabajando.caminandoAlSoviet;
            velocidadDeDesplazamientoActual = velocidadDeDesplazamientoPromedio;
            aip.maxSpeed = velocidadDeDesplazamientoActual;
            gds.SetDestination(sovietComunidad.GetComponent<Soviet>().puerta.position);

            an.SetTrigger("caminando");
        }
        else if (subEstadoActualTrabajando == trabajando.caminandoAlSoviet)
        {
            if (aip.reachedDestination)
            {
                sovietComunidad.GetComponent<Soviet>().BuscarTrabajo(gameObject);

                //Si lo mandamos a resistir, cambia de estado y hay que sacarlo de la funcion
                //Es el caso del SetTrabajo(0) que devuelve el soviet
                if (estadoActual != estado.trabajando)
                {
                    return;
                }

                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
                velocidadDeDesplazamientoActual = Random.Range(velocidadDeDesplazamientoPromedio, velocidadDeDesplazamientoMaxima);
                aip.maxSpeed = velocidadDeDesplazamientoActual;
                posLugarDeTrabajo = lugarDeTrabajo.GetComponent<edificio>().GetSpotTrabajo();
                gds.SetDestination(posLugarDeTrabajo);

                if(trabajoActual == trabajo.fabricante)
                {
                    an.SetTrigger("caminandoMartillo");
                    ActualizarVelAn();
                } else if(trabajoActual == trabajo.distribuidor)
                {
                    an.SetTrigger("transportandoVacio");
                    ActualizarVelAn();
                } else if(trabajoActual == trabajo.cosechador)
                {
                    an.SetTrigger("caminandoPico");
                    ActualizarVelAn();
                }
            }
        }
        else if (subEstadoActualTrabajando == trabajando.caminandoAlTrabajo)
        {
            if (posLugarDeTrabajo == Vector3.zero || posLugarDeTrabajo == null)
            {
                Debug.LogWarning("Error: Se intenta ir a un lugar de trabajo no inicializado");
                subEstadoActualTrabajando = trabajando.buscandoTrabajo;
                return;
            }

            if (aip.reachedDestination)
            {
                subEstadoActualTrabajando = trabajando.trabajando;

                if (trabajoActual == trabajo.fabricante)
                {
                    an.SetTrigger("usandoMartillo");
                }
                else if (trabajoActual == trabajo.distribuidor)
                {
                    an.SetTrigger("usandoTransporteDescargandoCocido");
                }
                else if (trabajoActual == trabajo.cosechador)
                {
                    an.SetTrigger("usandoPico");
                }
            }

        }
        else if (subEstadoActualTrabajando == trabajando.trabajando)
        {
            if (!tieneTarea && (
                (trabajoActual == trabajo.cosechador && subEstadoCosechando == cosechando.cosechando) ||
                (trabajoActual == trabajo.fabricante && subEstadoFabricando == fabricando.fabricando) ||
                (trabajoActual == trabajo.distribuidor && subEstadoDistribuyendo == distribuyendo.dejandoComidaProcesada)
                ))
            {
                if (trabajoActual == trabajo.cosechador)
                {
                    tiempoTareaActual = lugarDeTrabajo.GetComponent<ArbustoDeComida>().ObtenerTrabajoNecesario();
                    subEstadoCosechando = cosechando.cosechando;
                }
                else if (trabajoActual == trabajo.fabricante)
                {
                    tiempoTareaActual = lugarDeTrabajo.GetComponent<FabricaDeComida>().ObtenerTrabajoNecesario();
                    subEstadoFabricando = fabricando.fabricando;
                }
                else if (trabajoActual == trabajo.distribuidor)
                {
                    tiempoTareaActual = lugarDeTrabajo.GetComponent<AlmacenDeComida>().ObtenerTrabajoNecesario();
                    subEstadoDistribuyendo = distribuyendo.dejandoComidaProcesada;
                }
                else
                {
                    Debug.LogError("ERROR: LLamando a buscar el tiempo de tarea sin tener un oficio");
                }
                tieneTarea = true;
            }
            else
            {
                if (trabajoActual == trabajo.cosechador)
                {
                    if (subEstadoCosechando == cosechando.cosechando)
                    {
                        Utilidades.LookAt2D(transform, lugarDeTrabajo.transform, -90);
                        if (tiempoTareaActual <= 0)
                        {
                            TerminarTareaActual();
                        }
                        else
                        {
                            tiempoTareaActual -= capacidadDeTrabajo;
                        }

                    }
                    else if (subEstadoCosechando == cosechando.dejandoComidaCosechada)
                    {

                        StartCoroutine(DejarComidaCosechada());

                    }
                }
                else if (trabajoActual == trabajo.fabricante)
                {
                    if (subEstadoFabricando == fabricando.fabricando)
                    {
                        Utilidades.LookAt2D(transform, lugarDeTrabajo.transform, -90);
                        if (tiempoTareaActual <= 0)
                        {
                            TerminarTareaActual();
                        }
                        else
                        {
                            tiempoTareaActual -= capacidadDeTrabajo;
                        }

                    }
                    else if (subEstadoFabricando == fabricando.dejandoComidaFabricada)
                    {
                        StartCoroutine(DejarComidaFabricada());
                    }
                }
                else if (trabajoActual == trabajo.distribuidor)
                {
                    if (subEstadoDistribuyendo == distribuyendo.dejandoComidaProcesada)
                    {
                        Utilidades.LookAt2D(transform, lugarDeTrabajo.transform, -90);
                        if (tiempoTareaActual <= 0)
                        {
                            TerminarTareaActual();
                        }
                        else
                        {
                            tiempoTareaActual -= capacidadDeTrabajo;
                        }

                    }
                    else if (subEstadoDistribuyendo == distribuyendo.buscandoComidaProcesada)
                    {
                        if (!tieneFabricaCercanaAsignada)
                        {
                            fabricaCercanaAsignada = lugarDeTrabajo.GetComponent<AlmacenDeComida>().ObtenerFabricaCercanaConMasComida();
                            tieneFabricaCercanaAsignada = true;
                        }

                        StartCoroutine(BuscarComidaProcesada());
                    }
                }
                else
                {
                    Debug.LogError("ERROR: intentando hacer una tarea sin trabajo");
                }
            }
        }
        else
        {
            Debug.LogError("ERROR: no hay ningun subestado trabajando asignado");
        }
    } 

    void TerminarTareaActual()
    {
        if (trabajoActual == trabajo.cosechador)
        {
            tieneTarea = false;
            lugarDeTrabajo.GetComponent<ArbustoDeComida>().CosecharComida(GetComponent<vida>());
            if (comidaCosechada >= comidaQuePuedeLlevar)
            {
                aip.maxSpeed = velocidadDeDesplazamientoPromedio;
                gds.SetDestination(lugarDeTrabajo.GetComponent<ArbustoDeComida>().fabricaCercana.GetComponent<FabricaDeComida>().GetPosicionDeposito());
                ActualizarVelAn();
                an.SetTrigger("transportandoCrudo");

                subEstadoCosechando = cosechando.dejandoComidaCosechada;
            }
        }
        else if (trabajoActual == trabajo.fabricante)
        {
            tieneTarea = false;
            if (comidaFabricada < comidaQuePuedeLlevar)
            {
                lugarDeTrabajo.GetComponent<FabricaDeComida>().TomarComidaCosechada(GetComponent<vida>());
                FabricarComida();
            }
            if (comidaFabricada >= comidaQuePuedeLlevar)
            {
                gds.SetDestination(lugarDeTrabajo.GetComponent<FabricaDeComida>().GetPosicionDeposito());
                aip.maxSpeed = velocidadDeDesplazamientoPromedio;
                ActualizarVelAn();
                an.SetTrigger("transportandoCocido");
                subEstadoFabricando = fabricando.dejandoComidaFabricada;
            }

        }
        else if (trabajoActual == trabajo.distribuidor)
        {
            tieneTarea = false;
            lugarDeTrabajo.GetComponent<AlmacenDeComida>().DepositarComidaDistribuida(GetComponent<vida>());
            if (comidaFabricada <= 0)
            {

                aip.maxSpeed = velocidadDeDesplazamientoPromedio;
                gds.SetDestination(fabricaCercanaAsignada.GetComponent<FabricaDeComida>().GetPosicionDeposito());
                ActualizarVelAn();
                an.SetTrigger("transportandoVacio");
                subEstadoDistribuyendo = distribuyendo.buscandoComidaProcesada;
            }
        }
    }

    IEnumerator DejarComidaCosechada()
    {
        //Caso cosechador: llevar la comida a la fabrica
        if (!aip.reachedDestination)
        {
            yield return null;
        }
        else
        {
            for (int i = 0; i < comidaCosechada; i++)
            {
                if(i == 0)
                {
                    //an.SetTrigger("usandoTransporte");
                }
                lugarDeTrabajo.GetComponent<ArbustoDeComida>().fabricaCercana.GetComponent<FabricaDeComida>().DejarComidaCosechada(GetComponent<vida>());
                yield return null;
            }
            if (comidaCosechada <= 0)
            {
                comidaCosechada = 0;
                subEstadoCosechando = cosechando.cosechando;
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;

                gds.SetDestination(posLugarDeTrabajo);
                velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                ActualizarVelAn();

                an.SetTrigger("caminandoPico");
            }
        }
    }

    IEnumerator DejarComidaFabricada()
    {
        //Caso fabricante: depositar la comida procesada y agarrar nueva sin procesar
        if (!aip.reachedDestination)
        {
            yield return null;
        }
        else
        {
            for (int i = 0; i < comidaFabricada; i++)
            {
                lugarDeTrabajo.GetComponent<FabricaDeComida>().DejarComidaProcesada(GetComponent<vida>());
                yield return null;
            }
            if (comidaFabricada <= 0)
            {
                comidaFabricada = 0;
                subEstadoFabricando = fabricando.fabricando;
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;

                gds.SetDestination(posLugarDeTrabajo);
                velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                ActualizarVelAn();

                an.SetTrigger("caminandoMartillo");
            }
        }
        //TODO: recargar comida cosechada sin procesar
    }

    IEnumerator BuscarComidaProcesada()
    {
        //Caso distribuidor: ir a la fabrica cercasna con mas comida a buscar mas comida para despues dejar.
        //TODO: introducir demora
        if (!aip.reachedDestination)
        {
            yield return null;
        }
        else
        {
            for (int i = 0; i < comidaQuePuedeLlevar; i++)
            {
                if (comidaFabricada < comidaQuePuedeLlevar)
                {
                    fabricaCercanaAsignada.GetComponent<FabricaDeComida>().TomarComidaProcesada(GetComponent<vida>());
                }
                yield return null;
            }
            if (comidaFabricada >= comidaQuePuedeLlevar)
            {
                comidaFabricada = comidaQuePuedeLlevar;
                subEstadoDistribuyendo = distribuyendo.dejandoComidaProcesada;
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;

                gds.SetDestination(posLugarDeTrabajo);
                velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                ActualizarVelAn();

                an.SetTrigger("transportandoCocido");
            }
        }
    }


    public void SetTrabajo(int caso)
    {
        switch (caso)
        {
            case 0:
                //Lo mandamos al iddle o a la muralla
                CambiarDeEstado(estado.resistiendo);
                break;
            case 1:
                trabajoActual = trabajo.cosechador;
                BuscarArbustoDeComidaCercano();
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
                break;
            case 2:
                trabajoActual = trabajo.fabricante;
                BuscarFabricaDeComidaCercana();
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
                break;
            case 3:
                trabajoActual = trabajo.distribuidor;
                BuscarAlmacenDeComidaCercano();
                subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
                break;
            default:
                Debug.LogWarning("ERROR: no se devolvió un valor saludable de la funcion buscar trabajao del soviet");
                break;
        }
    }

    void BuscarSovietComunidad()
    {
        if (gV.ObtenerSovietsComunidadesOrdenadosPorCercania(transform.position).Count > 0)
        {
            sovietComunidad = gV.ObtenerSovietsComunidadesOrdenadosPorCercania(transform.position)[0];
        }
        else
        {
            CambiarDeEstado(estado.idle);
        }
    }

    void BuscarArbustoDeComidaCercano()
    {
        foreach (GameObject arbusto in sovietComunidad.GetComponent<Soviet>().ObtenerArbustosCercanosOrdenadosPorCantidadDeTrabajadores())
        {
            if (arbusto.GetComponent<ArbustoDeComida>().agregarTrabajador(gameObject))
            {
                lugarDeTrabajo = arbusto;
                //TODO: adecuar bien que lugar fisico ocupa en el trabajo
                posLugarDeTrabajo = lugarDeTrabajo.transform.position;
                //TODO: ver cuando carajo sacarlo de donde esta assignado. Puede ser al cambiar de estado.
                return;
            }
        }
        //Si se llego hasta este punto de la funcion es porque en ningun arbusto cercano hay lugar
        //En ese caso lo pasamos a ser cosechador
        trabajoActual = trabajo.cosechador;
        subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
    }

    void BuscarFabricaDeComidaCercana()
    {
        foreach (GameObject fabrica in sovietComunidad.GetComponent<Soviet>().ObtenerFabricasCercanasOrdenadasPorCantidadDeTrabajadores())
        {
            if (fabrica.GetComponent<FabricaDeComida>().agregarTrabajador(gameObject))
            {
                lugarDeTrabajo = fabrica;
                //TODO: adecuar bien que lugar fisico ocupa en el trabajo
                posLugarDeTrabajo = lugarDeTrabajo.transform.position;
                //TODO: ver cuando carajo sacarlo de donde esta assignado. Puede ser al cambiar de estado.
                return;
            }
        }
        //Si se llego hasta este punto de la funcion es porque en ninguna fabrica cercana hay lugar
        //En ese caso lo pasamos a ser distribuidor
        trabajoActual = trabajo.distribuidor;
        subEstadoActualTrabajando = trabajando.caminandoAlTrabajo;
    }

    void BuscarAlmacenDeComidaCercano()
    {
        foreach (GameObject almacen in sovietComunidad.GetComponent<Soviet>().ObtenerAlmacenesCercanosOrdenadosPorCantidadDeTrabajadores())
        {
            if (almacen.GetComponent<AlmacenDeComida>().agregarTrabajador(gameObject))
            {
                lugarDeTrabajo = almacen;
                //TODO: adecuar bien que lugar fisico ocupa en el trabajo
                posLugarDeTrabajo = lugarDeTrabajo.transform.position;
                //TODO: ver cuando carajo sacarlo de donde esta assignado. Puede ser al cambiar de estado.
                return;
            }
        }
        //TODO: cuando ya paso todo hay que mandarlo a la barrera o a otroa comundiad
        CambiarDeEstado(estado.resistiendo);
    }



    public void SumarComidaCosechada()
    {
        comidaCosechada++;
    }
    public void RestarComidaCosechada()
    {
        comidaCosechada--;
    }
    public void SumarComidaFabricada()
    {
        comidaFabricada++;
    }
    public void RestarComidaFabricada()
    {
        comidaFabricada--;
    }

    void FabricarComida()
    {
        if (comidaCosechada > 0)
        {
            comidaCosechada--;
            comidaFabricada++;
        }
    }

    IEnumerator Esperar(float n)
    {
       for(int i = 0; i<n; i++)
        {
            Debug.Log("esperando " + i);
            yield return null;
        }
    }







    /*
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    * **********************  CONSTRUYENDO  ********************************************************* *
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    */
    /* 
     * 
     * 
     * */


    void DecidirSubestadoConstruyendo()
    {
        if(subEstadoActualConstruyendo == construyendo.caminandoAConstruccion)
        {
            if (aip.reachedDestination)
            {
                //ca.CambiarAnimacion(customAnimator.animacion.construyendo);
                an.SetTrigger("usandoMartillo");
                subEstadoActualConstruyendo = construyendo.construyendo;
                if (!spotConstruccion.transform.parent.GetComponent<Animator>().GetBool("animado"))
                {
                    spotConstruccion.transform.parent.GetComponent<Animator>().SetTrigger("tocon");
                    spotConstruccion.transform.parent.GetComponent<Animator>().SetBool("animado", true);
                }
            }
        } else if(subEstadoActualConstruyendo == construyendo.construyendo)
        {
            if(spotConstruccion == null || spotConstruccion.transform.parent.gameObject == null)
            {
                Debug.Log("Construccion nula");
                spotConstruccion = null;
                an.SetTrigger("caminando");
                CambiarDeEstado(estado.idle);
                return;
            }
            if(spotConstruccion.GetComponentInParent<toconConstruccion>().GetVida() > 0){
                spotConstruccion.GetComponentInParent<toconConstruccion>().Construir();
                Utilidades.LookAt2D(transform, spotConstruccion.transform.parent, -90);
            }//LookAt2D(spotConstruccion.transform.parent.gameObject);
            //TODO: mirar desde el spot a la construccion
        }
        else
        {
            Debug.LogError("ERROR: No hay subestado actual construyendo");
        }
    }

    public void AsignarSpotConstruccion(GameObject spot)
    {
        spotConstruccion = spot;
        subEstadoActualConstruyendo = construyendo.caminandoAConstruccion;
        velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
        aip.maxSpeed = velocidadDeDesplazamientoActual;
        gds.SetDestination(spotConstruccion.transform.position);
        CambiarDeEstado(estado.construyendo);
        ActualizarVelAn();
        an.SetTrigger("caminandoMartillo");
    }

    //Esto se llama desdde la muerte para no dejarlo en la lista de trabajadores asignados y que cuando muera otro pueda
    //ocupar su lugar
    public void EliminarDeConstruccion()
    {
        if (spotConstruccion != null)
        {
            spotConstruccion.transform.GetComponentInParent<toconConstruccion>().EliminarPorMuerte(gameObject);
            estadoActual = estado.idle;
            spotConstruccion = null;
            //ca.CambiarAnimacion(customAnimator.animacion.caminando);
            ActualizarVelAn();
            an.SetTrigger("caminando");
        }
        else
        {
            estadoActual = estado.idle;
            ActualizarVelAn();
            an.SetTrigger("caminando");
        }
    }
    
    /*
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    * **********************  RESISTIENDO  ********************************************************* *
    * ********************************************************************************************* *
    * ********************************************************************************************* *
    */
    /* 
     * 
     * 
     * */
     void DecidirSubestadoResistiendo()
    {
        Debug.Log("Resistiendo");
        Debug.Log("Resistiendo felizmente :))");
        if(subEstadoActualResistiendo == resistiendo.decidiendo)
        {
            if (gV.ObtenerHayPersonas())
            {
                personaEnemiga = gV.ObtenerPersonaMasCercana(transform.position).transform;
                velocidadDeDesplazamientoActual = velocidadDeDesplazamientoMaxima;
                ActualizarVelAn();
                an.SetTrigger("caminandoLanza");
                aip.endReachedDistance = 25;
                subEstadoActualResistiendo = resistiendo.yendoAPersona;
            }
        } else if (subEstadoActualResistiendo == resistiendo.yendoAPersona)
        {
            gds.SetDestination(personaEnemiga.position);
            if (aip.reachedDestination)
            {
                subEstadoActualResistiendo = resistiendo.atacandoAPersona;
                an.SetTrigger("usandoLanza");
            }
        } else if (subEstadoActualResistiendo == resistiendo.atacandoAPersona)
        {
            personaEnemiga.GetComponent<persona>().Atacar();
            if (!aip.reachedDestination)
            {
                subEstadoActualResistiendo = resistiendo.yendoAPersona;
                an.SetTrigger("caminandoLanza");
            }
        } else if(subEstadoActualResistiendo == resistiendo.buscandoLugarIdle)
        {
            destinoIdle = Utilidades.PuntoRandom(gV.piso, transform.position, 45);
            velocidadDeDesplazamientoActual = SetVelocidadRandom();
            aip.maxSpeed = velocidadDeDesplazamientoActual;
            gds.SetDestination(destinoIdle);
            subEstadoActualResistiendo = resistiendo.yendoALugarIdle;

            an.SetTrigger("caminandoLanza");
            ActualizarVelAn();

        }
        else if(subEstadoActualResistiendo == resistiendo.yendoALugarIdle)
        {
            if (aip.reachedDestination)
            {
                subEstadoActualResistiendo = resistiendo.rumiando;

                ActualizarVelAn(0);
                an.SetTrigger("idle");
                SetTiempoRumiar();
            }
        } else if (subEstadoActualResistiendo == resistiendo.rumiando)
        {
            //Chequear el contador random y si se cumple, vaciar el destino idle y pasarlo a buscando objetivo
            if (tiempoActualRumiar >= tiempoRumiar)
            {
                destinoIdle = Vector3.zero;
                subEstadoActualIdle = idle.buscandoLugar;
                tiempoActualRumiar = 0;
            }
            else
            {
                tiempoActualRumiar++;
            }
        }
        else
        {
            Debug.LogError("ERROR: NO HAY substado resistiendo selecionado");
        }
    }

    public bool ObtenerEstaResistiendo()
    {
        if(estadoActual == estado.resistiendo)
        {
            return true;
        }
        return false;
    }





    ///*
    ///
    ///*
    ///

    public void Retirar()
    {
        retirada = true;
    }




}