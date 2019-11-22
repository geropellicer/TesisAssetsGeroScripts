using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/// <summary>
/// Maneja el comportamiento del sujeto alrededor de una persona y en estado Idle cuando no hay persona.
/// </summary>

/// <summary>Lista de posibles estados que puede adquirir un follower.
/// IDLE: No hay persona, boludea 
/// SIGUIENDO: Se dedica a ir tras una persona en movimiento.
/// CONVIRTIENDOSE: Frame/s de transicion de cuando estaba siguiendo a una persona y otra persona con mas seguidores lo capta.
/// TRABAJANDO: Cuando una persona a la que esta siguiendo se para, se pone a trabajar. </summary>
public enum Estado
{
    IDLE,
    SIGUIENDO,
    CONVIRTIENDOSE,
    TRABAJANDO
}

public class Follower : MonoBehaviour
{

    /// <summary>La persona que debe seguir este Follower</summary>
    public Transform persona;

    /// <summary>El estado actual del follower</summary>
    [SerializeField]
    private Estado estado;

    /// <summary>Referencia al sprite renderer para cambiar el color del bicho según a que persona sigue.</summary>    
    private SpriteRenderer sR;

    /// <summary>Guardamos el color que vamos a aplicarle al sR del mismo bicho (PERTENENCIA A UNA PERSONA).</summary>    
    [SerializeField]
    private Color colorSpriteBicho;
    /// <summary>Guardamos el color que vamos a aplicarle a la zona del bicho(PERTENENCIA A UNA PERSONA).</summary>    
    [SerializeField]
    private Color colorSpriteZona;

    /// <summary>Guardamos el color que vamos a aplicarle al sR del anillo emocion (SEGUN LA EMOCION QUE SIENTA EL BICHO).</summary>    
    [SerializeField]
    private Color colorSpriteEmocion;

    /// <summary> El fondo del bicho que se pinta del color de la emocion del bicho </summary>
    private GameObject zona;

    /// <summary> El sprite renderer del fondo del bicho </summary>


    /// <summary> El HUE del color del bicho basado en la emocion que siente. </summary>
    /// <summary> PUBLICOESTATAL:   350, </summary>
    /// <summary> PRIVADOCOMERCIAL:   141, </summary>
    /// <summary> PUBLICOMILITAR:   63, </summary>
    /// <summary> PRIVADOENTRETENIMIENTO:   250, </summary>
    /// <summary> REBELION:   350, </summary>
    /// <summary> IRSE:   0, </summary>
    /// <summary> NADA:   0, </summary>
    float hueColorEmocion;

    /// <summary> El SATURATION del color del bicho basado en la emocion que siente. </summary>
    /// <summary> Aumento de acuerdo a un mapping entre ciertos limites y la intensidad de la emocion. </summary>
    float saturationColorEmocion;

    /// <summary> El BRIGHTNESS del color del bicho basado en la emocino que siente. </summary>
    /// <summary> 50 para las 4 emociones de antenas y para la de rebelion. </summary>
    /// <summary> 100 para nada o neutral. </summary>
    float brightnessColorEmocion;

    /// <summary>Devolvemos si la persona que seguimos esta parada o no.</summary>
    bool PersonaEstaParada()
    {
        return persona.GetComponent<Seguido>().EstaParado();
    }

    [SerializeField]
    /// <summary> Velocidad de desplazamiento mínima para cuando camina</summary>
    private float velMin;

    [SerializeField]
    /// <summary> Velocidad de desplazamiento mínima para cuando camina</summary>
    private float velMax;

    /// <summary> Referencia al globalManager para sacar las gV y otras utilidades </summary>
    GameObject globalManager;

    /// <summary> Referencia a las variables globales</summary>
    private globalVariables gV;

    /// <summary> Referencia al AIPath de A* para setear velocidades</summary>
    private AIPath aiP;

    /// <summary> Referencia al animator para cambair el estad de los graficos</summary>
    private Animator an;

    /// <summary> Referencia al Gero Destination Setter script para setear el destino del agente AI de A*</summary>
    private GeroDestinationSetter gds;

    /// <summary> Lo usamos para alamacenar un punto en el espacio al que va a minar</summary>
    Vector3 posLugarDeTrabajo;

    /// <summary> Cuanto lleva trabajado en este ciclo particular de trabajo
    /// (se increementa cadsa frame que esta trabajando en el lugar correcto)</summary>
    [SerializeField]
    int tiempoActualTrabajar = 0;

    [SerializeField]
    /// <summary> El total que tiene que trabajar en este ciclo para conseguir el producto</summary>
    int tiempoTotalTrabajar;

    /// <summary> Los tres subestados posibles para el estado TRABAJANDO</summary>
    /// <summary> Buscando Trabajo: buscamos un punto cercano en el espacio</summary>
    /// <summary> Caminando al trabajo: Vamos (seteado por AIPath, acá no hacemos mucho)</summary>
    /// <summary> Trabajando: minando literalmente hasta que obtiene el producto.</summary>
    enum TRABAJANDO
    {
        buscandoTrabajo,
        caminandoAlTrabajo,
        trabajando,
    }
    /// <summary> Almacenamos el subestado actual en caso de que estemos en el estado TRABAJANDO</summary>
    [SerializeField]
    private TRABAJANDO subEstadoActualTrabajando;

    /// <summary> Contador para el tiempo que llevamos quietos en IDLE, se suma cada frame que esté quieto</summary>
    [SerializeField]
    int tiempoActualRumiar = 0;
    /// <summary> Valor al que tenemos que llegar con el tiempoActualRumiar para cosniderar e ciclo terminado</summary>
    [SerializeField]
    int tiempoTotalRumiar;
    /// <summary> Los tres subestados posibles para el estado IDLE</summary>
    /// <summary> Buscando Lugar: buscamos un punto cercano en el espacio</summary>
    /// <summary> Caminando: Vamos (seteado por AIPath, acá no hacemos mucho)</summary>
    /// <summary> Rumiando: esperando sin hacer nada literalmente hasta que termine el tiempo rumiar.</summary>
    enum IDLE
    {
        buscandoLugar,
        caminando,
        rumiando
    }
    /// <summary> Almacenamos el subestado actual en caso de que estemos en el estado IDLE</summary>
    [SerializeField]
    private IDLE subEstadoActualIdle;

    [SerializeField]
    /// <summary> Guardamos la cantidad de incidencia que tiene una antena particular. </summary>
    /// <summary> Cada vez que una onda de un totem particular le pega a un bicho, incrementa el efecto de esa antena.</summary>
    /// <summary> TODO: adicionalmente también deberían aumentar por efecto contagio de otros bichos de la misma persona. </summary>

    //private int efectoPublicoEstatal, efectoPublicoMilitar, efectoPrivadoComercial, efectoPrivadoEntretenimiento;
    /// <summary> Cambio los efectos a booleanos mas simples. El booleano de la emocion que este activado permite que el
    /// nivel de emocion de esa emocion aumente </summary>
    bool publicoEstatal, publicoMilitar, privadoComercial, privadoEntretenimiento;

    /// <summary> Si la distancia de un efecto a otro supera la del umbral, empieza a sumar puntos esa emocion. </summary>
    int umbralEfectoEmocion = 20;

    [SerializeField]
    /// <summary> Acá almacenamos con cuanta intensidad se siente una emoción. Solo deberíamos tener en mayor a 0 </summary>
    /// <summary> la emoción que actualmente está activa. La diferencia con el efecto es que el efecto es según </summary>
    /// <summary> cuanto afecta cada antena solamente, no se reinicia sino que se acumula a lo largo del tiempo. </summary>
    /// <summary> El nivel de emocion en cambio empieza de 1 apenas se transiciona a la emocion y va aumentando según factores.</summary>
    private int nivelEmocionActual;

    /// <summary> Si el nivel de una emocion es mayor a este umbral, puede empezar a contagiar al resto si entra en contacto.</summary>
    private int umbralContagioEmocion = 100;

    /// <summary> Las 6 distintas emociones (y la 7ma NADA) que pueden sentir (una por vez por ahora) </summary>
    /// <summary> Si en el futuro queremos que sean combinables, deberíamos tener booleanos separados. </summary>
    public enum EMOCION
    {
        NADA,
        AMORALLIDER,
        HACERGUERRA,
        BOLUDEAR,
        INDIVIDUALISTA,
        REBELARSE,
        ESCAPARSE
    }
    /// <summary> La emoción que siente actualmente. Adicionalmente tenemos la intensidad en las variables "nivelEmocion". </summary>
    [SerializeField]
    private EMOCION emocionActual;
    /// <summary> Devolvemos la emocion actual a agentes externos. </summary> 
    public EMOCION ObtenerEmocionActual()
    {
        return emocionActual;
    }

    float nextActionTime;
    float intervalo;
    [SerializeField]
    GameObject prefabManchaPiso;

    /// <summary> Contamos en tiempo real cuanto hambre tiene. Sube por cada ciclo y baja al alimentarse. </summary>
    [SerializeField]
    int hambre;

    /// <summary> Devolvemos para la barra de hambre cuanto hambre tiene. Sube por cada ciclo y baja al alimentarse. </summary>
    [SerializeField]
    public int ObtenerHambre()
    {
        return hambre;
    }

    /// <summary> Cuando el hambre supere este umbral va a intentar alimentarse cuando tenga la oportunidad </summary>
    [SerializeField]
    int umbralHambreAlimentarse = 33;

    /// <summary> Cuando el hambre supere este umbral va a morir inmediatamente </summary>
    [SerializeField]
    int umbralHambreMuerte = 100;

    /// <summary> Cuando este modo esta prendido, en los estados IDLE y TRABAJANDO va a intentar alimentarse antes. </summary>
    [SerializeField]
    bool modoAlimentacion = false;

    /// <summary> Contiene los tres posibles subestados para cuando está activado el modoAlimentacion</summary>
    public enum COMIENDO
    {
        /// <summary> Cuando le pedimos a la persona su lista de comidas almacenadas y seleccionamos una. </summary>
        /// <summary> Tambien volvemos a caer acá si en el camino yendo alguien se come la comida que teniamos </summary>
        SELECCIONANDOCOMIDA,
        /// <summary> Yendo hacia la persona a buscar la comida. </summary>
        CAMINANDOACOMIDA,
        /// <summary> Si hemos llegado y la comida existe, nos alimentamos. </summary>
        COMIENDO
    }

    /// <summary> Almacenamos en que estado de alimentacion estamos. </summary>
    [SerializeField]
    private COMIENDO subEstadoActualComiendo;

    [SerializeField]
    /// <summary> Si no tenemos persona, nos guardamos las comidas aca. Si tenemos, debería quedar vacío. </summary>
    List<GameObject> comidasPropias;

    /// <summary> Guardamos el prefab con el que instanciamos la comida en cada iteracion de trabajo.</summary>
    [SerializeField]
    private GameObject prefabComidaNueva;

    [SerializeField]
    /// <summary> Guardamos estáticamente una lista con los depositos de comida cercanos a nosotros o a nuesta persona
    ///  según el caso. Para actualizarlo hay que llamar a la funcion ActualizarDepositosDeComidaCercanos()</summary>
    List<GameObject> depositosDeComidaCercanos;

    /// <summary> El deposito de los cercanos y posibles que habian que elegimos. Debemos reiniciarlo null al irnos de trabajando.</summary>
    [SerializeField]
    GameObject depositosDeComidaSeleccionado;

    /// <summary> Tiempo que llevamos esperando actualmete </summary>
    [SerializeField]
    int esperarDepositoActual = 0;
    /// <summary> Tiempo que tenemos que esperar para volver a consultar si hay depositos cerca </summary>
    [SerializeField]
    int esperarDepositoTotal = 120;
    /// <summary> Si estamos esperando o no </summary>
    [SerializeField]
    bool esperandoDeposito;

    /// <summary> Si estamos en modo de forzado de boludeo o no. Caso afirmativo puede salirse del trabajo y forzar un IDLE </summary>
    [SerializeField]
    bool forzarBoludear = false;
    /// <summary> Lo que llevamos esperado en este ciclo desde que se activó el forzar boludear </summary>
    [SerializeField]
    int esperarBoludearActual = 0;
    /// <summary> Lo que tenemos que esperar para completar un ciclo de espera y sacar el forzarBolduear </summary>
    [SerializeField]
    int esperarBoludearTotal = 120;

    /// <summary> Si estamos en modo de forzado individualista o no. Caso afirmativo puede ignorar el modo alimentacion y solo trabar </summary>
    //[SerializeField]
    bool forzarIndividualista = false;
    /// <summary> Lo que llevamos esperado en este ciclo desde que se activó el forzar boludear </summary>
    [SerializeField]
    //int esperarIndividualistaActual = 0;
    /// <summary> Lo que tenemos que esperar para completar un ciclo de espera y sacar el forzarBolduear </summary>
    //[SerializeField]
    //int esperarIndividualistaTotal = 120;

    /// <summary> Referencia al sprite renderer del anillo de emociones que debemos pintar del color correspondiente </summary>
    SpriteRenderer srAnilloEmociones;

    /// <summary> Este numero representa que lugar ocupamos en la lista de seguidores de la persona a la que seguimos. </summary>
    /// <summary> Lo usamos por ejemplo en forzarMilitar para otorgarle un lugar en la formacion segun su lugar en la lista </summary>
    [SerializeField]
    int indexSeguidorEnPersona;

    /// <summary> Si estamos en modo de forzado de Militar o no. Caso afirmativo formar militarmente, etc. </summary>
    [SerializeField]
    bool forzarMilitar = false;
    /// <summary> Lo que llevamos esperado en este ciclo desde que se activó el forzar militar </summary>
    //[SerializeField]
    //int esperarMilitarActual = 0;
    /// <summary> Lo que tenemos que esperar para completar un ciclo de espera y sacar el forzarMilitar </summary>
    //[SerializeField]
    //int esperarMilitarTotal = 240;

    /// <summary> Esta variable se actualiza cada cierto tiempo desde el update. Es verdadera </summary>
    /// <summary> Cuando hay sujetos de otras personas en un determinado rango y está activado forzarMilitar. </summary>
    [SerializeField]
    bool militar_haySujetosEnRango;

    /// <summary> Si estamos en modo de forzado de Militar o no. Caso afirmativo formar militarmente, etc. </summary>
    [SerializeField]
    bool forzarNacionalismo = false;
    /// <summary> Lo que llevamos esperado en este ciclo desde que se activó el forzar militar </summary>
    //[SerializeField]
    //int esperarNacionalismoActual = 0;
    /// <summary> Lo que tenemos que esperar para completar un ciclo de espera y sacar el forzarMilitar </summary>
    //[SerializeField]
    //int esperarNacionalismoTotal = 240;


    /// <summary> Variable en la que guardamos el grado de descontento en una escala del 1 al 100 </summary>
    /// <summary> Aumenta con el hambre y siendo esclavo de una persona, disminuye con los discursos </summary>
    [SerializeField]
    int porcentajeDescontento;
    /// <summary> Devolvemos para el exterior el grado de descontento en una escala del 1 al 100. </summary>
    public int ObtenerPorcentajeDescontento()
    {
        return porcentajeDescontento;
    }

    /// <summary> Si esta activado, se fuerza este modo por encima de todos los demas </summary>
    [SerializeField]
    bool forzarRevolucion;

    /// <summary> Posibles estados del proceso revolucionario, en la funcion DecidirQueHaceRevolucion() se explican </summary>
    public enum REVOLUCION
    {
        NADA,
        BUSCANDOANTENA,
        YENDOAANTENA,
        APAGANDOANTENA,
        BUSCANDOPRENDERFUEGO,
        YENDOAPRENDERFUEGO,
        PRENDIENDOFUEGO,
        BUSCANDOPORDONDESALIR,
        SALIENDO
    }

    /// <summary> Almacena en qué estado estamos actualmente dentro de un proceso revolucionario</summary>
    [SerializeField]
    REVOLUCION estadoRevolucionActual;
    /// <summary> Cuando recien activamos el estado revolucionario por primera vez debemos sortear si va a ir
    /// a una antena o a prender un fuego, por eso necesitamos pasar por un momento de inicializacion </summary>
    [SerializeField]
    bool estadoRevolucionInicializado;
    /// <summary> Si es incendiario o saboteador de antenas. Se sortea en el primer frame de DecidirQueHHacerRevolucion </summary>
    [SerializeField]
    bool esIncendiario;
    /// <summary> Si es incendiario o saboteador de antenas. Se sortea en el primer frame de DecidirQueHHacerRevolucion </summary>
    [SerializeField]
    bool esSaboteador;
    /// <summary> Guardamos el lugar al que tiene que ir (puede ser un punto a prender fuego) </summary>
    [SerializeField]
    Vector3 posPrenderFuego;
    /// <summary> El totem que hemos elegido para ir a apagar en caso de revlucion </summary>
    [SerializeField]
    GameObject totemSeleccionado;
    /// <summary> Devolvemos al exterior el totem que hemos elegido para ir a apagar en caso de revlucion </summary>
    [SerializeField]
    public GameObject TotemSeleccionado()
    {
        return totemSeleccionado;
    }
    /// <summary> Almacenamos el prefab del incendio para instnaicarlo si es incendiario en proceso revolucionario </summary>
    [SerializeField]
    GameObject prefabIncendio;
    /// <summary> pregreso que llevamos desde que empezmaos el incendio </summary>
    [SerializeField]
    int progresoIncendioActual = 0;
    /// <summary> Total al que tenemos que llegar para poder instanciar un incendio </summary>
    [SerializeField]
    int progresoIncendioTotal = 240;


    /// <summary> Cuando queremos apagar una antena prendemos esto para que al ser detectado por la antena,
    /// esta empiece a restar en su cuenta atrás de apagado (si el totem selccionado coincide, para evitar apagar
    /// a la pasada otra antena que no es el target) </summary>
    [SerializeField]
    bool intencionDeApagarAntena;
    /// <summary> Devuelve al exterior la intencion de apagar o no la antena </summary>
    public bool IntencionDeApagarAntena()
    {
        return intencionDeApagarAntena;
    }

    /// <summary> Cuando queremos prender una antena prendemos esto para que al ser detectado por la antena,
    /// esta empiece a sumar en su cuenta atrás de prendido (si el totem selccionado coincide, para evitar predner
    /// a la pasada otra antena que no es el target) </summary>
    [SerializeField]
    bool intencionDePrenderAntena;
    /// <summary> Devuelve al exterior la intencion de apagar o no la antena </summary>
    public bool IntencionDePrenderAntena()
    {
        return intencionDePrenderAntena;
    }


    /// <summary> El vector3 que va acontener el punto al cual debemos dirigirnos para evacuar la obra </summary>
    Vector3 posSalidaElegida;

    /// <summary> Si tenemos alguna emocion, este booleano devuelve si la antena correspondiente
    /// a la emocion está transmitiendo o no </summary>
    bool antenaDeEmocionEstaTransmitiendo;


    /// <summary> Si hemos ya seleccionado que comida de la persona nos vamos a comer, es decir, si esta inicializado el proceso de alimentacion
    /// para ir a los otros subestados de ManejarAlimentacion()</summary>
    [SerializeField]
    bool comidaSeleccionada;
    

    /// <summary> La comida que hemos pre elegido de la persona para ir a comer. </summary>
    [SerializeField]
    GameObject comidaPreseleccionada;
    
    /// <summary> 
    /// </summary>

    /// <summary> 
    /// </summary>


    // FIN VARIABLES
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////





    // MANEJOS GENERALES
    // /////////////////////////////////////////////////////////////////////////
    //  /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////////////


    void OnEnable()
    {
        sR = GetComponent<SpriteRenderer>();
        aiP = GetComponent<AIPath>();
        gds = GetComponent<GeroDestinationSetter>();
        an = GetComponent<Animator>();

        srAnilloEmociones = transform.Find("AnilloEmocion").GetComponent<SpriteRenderer>();
        srAnilloEmociones.color = new Color(1, 1, 1, 0);

        globalManager = GameObject.Find("GlobalManager");
        gV = globalManager.GetComponent<globalVariables>();

        estado = Estado.IDLE;
        subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
        subEstadoActualIdle = IDLE.buscandoLugar;

        velMin = Random.Range(2, 4);
        velMax = Random.Range(6, 10);

        tiempoTotalRumiar = Random.Range(200, 450);
        tiempoTotalTrabajar = Random.Range(200, 450);

        // Necesario para que al iniciaizar sean blancos
        brightnessColorEmocion = 100;

        nextActionTime = Time.time + Random.Range(0f, 2f);
        intervalo = Random.Range(3f, 4f);

        comidasPropias = new List<GameObject>();

        float localScale = Random.Range(0.85f, 1.15f);
        transform.localScale = new Vector3(localScale, localScale, localScale);

        depositosDeComidaCercanos = new List<GameObject>();

        colorSpriteBicho = new Color(1, 1, 1, .25f);

        persona = null;

        CambiarModoAlimentacion(false);

        gV.SumarPoblacion(gameObject);
    }

    /// <summary>Todos los frames evaluamos que hacer dependiendo el estado y los eventos</summary>
    void Update()
    {
        DecidirSentimientos();
        if (!forzarRevolucion)
        {
            // Si no setimos nada decdimos que hacer normalmente
            // Si setnimos algo con mucha intensisdad y esa antena está prendida, también
            // En cabmio si sentimos alg con mucha intensidad y se apaga la antena, vamos a intentar prenderla
            if (emocionActual == EMOCION.NADA || (emocionActual != EMOCION.NADA && nivelEmocionActual > 90 && antenaDeEmocionEstaTransmitiendo))
            {
                DecidirQueHacer();
            }
            else if (nivelEmocionActual > 90 && !antenaDeEmocionEstaTransmitiendo)
            {
                AdministrarPrenderAntenas();
            }
        }
        else if (forzarRevolucion)
        {
            DecidirQueHacerRevolucion();
        }
        AsignarColorEmocion();

        if (forzarBoludear)
        {
            if (esperarBoludearActual >= esperarBoludearTotal)
            {
                forzarBoludear = false;
                esperarBoludearActual = 0;
            }
            else
            {
                esperarBoludearActual++;
            }
        }

        /*if(forzarIndividualista){
            if(esperarIndividualistaActual >= esperarIndividualistaTotal)
            {
                forzarIndividualista = false;
                esperarIndividualistaActual = 0;
            } else{
                esperarIndividualistaActual++;
            }
        }*/

        /*if(forzarMilitar){
            if(esperarMilitarActual >= esperarMilitarTotal)
            {
                SalirForzarMilitar();
            } else{
                esperarMilitarActual++;
            }
        }*/

        /*if(forzarNacionalismo){
           /* if(esperarNacionalismoActual >= esperarNacionalismoTotal)
            {
                forzarNacionalismo = false;
                esperarNacionalismoActual = 0;
            } else{
                esperarNacionalismoActual++;
            }
            */

        // Por ahora vamos a probar en que el nacioanlismo se mantenga permanente
        /*}*/

        if (Time.time > nextActionTime)
        {

            AcualizarAntenaDeEmocionEstaTransmitiendo();

            // En principio los efectos los pasamos a booleanos
            // y el unico fator que controlamos es la intenisidad de la emocion
            // que solo baja exponiendose a cualquier otra antena que no sea la de la emocion actual
            /*if(efectoPrivadoComercial > 0)
            {
                efectoPrivadoComercial--;
            }
            if(efectoPrivadoEntretenimiento > 0)
            {
                efectoPrivadoEntretenimiento--;
            }
            if(efectoPublicoEstatal > 0)
            {
                efectoPublicoEstatal--;
            }
            if(efectoPublicoMilitar > 0)
            {
                efectoPublicoMilitar--;
            }*/

            nextActionTime = Time.time + intervalo;

            if (persona != null)
            {
                zona = Instantiate(prefabManchaPiso, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                zona.GetComponent<SpriteRenderer>().color = colorSpriteZona;
            }

            if (emocionActual != EMOCION.NADA)
            {
                srAnilloEmociones.color = colorSpriteEmocion;
            }
            if (emocionActual == EMOCION.NADA)
            {
                srAnilloEmociones.color = new Color(1, 1, 1, 0);
            }

            if (forzarMilitar)
            {
                if (persona != null)
                {
                    SetPosicionFormacionMilitar(true);
                }
                else
                {
                    SetPosicionFormacionMilitar(false);
                }

                ActualizarHaySujetosEnRango();
            }


            if (emocionActual == EMOCION.HACERGUERRA && !forzarMilitar)
            {
                ForzarMilitar();
            }

            if (emocionActual == EMOCION.AMORALLIDER && !forzarNacionalismo)
            {
                ForzarNacionalismo();
            }


            if (nivelEmocionActual > 90)
            {
                porcentajeDescontento--;
            }


            ManejarHambre();
        }
    }

    /// <summary>Todos los frames evaluamos como se siente dependiendo de como le afecten las antenas</summary>
    /// <summary>Para todos los casos evaluamos: 1) que el efecto que tiene un discurso determinado sea mayor a </summary>
    /// <summary>todods los demás y 2) que la distancia al segundo efecto sea mayor a un umbral de efecto.</summary>
    /// <summary>Esto nos permite A) solo hacer pasos a emociones cuyo discursos nos influyen mas que todos los otros y</summary>
    /// <summary>B) solo cambiar de emocion cuando hay mucha diferencia de incidencia de discursos. Por lo tanto discursos</summary>
    /// <summary>muy balanceados afectando a un sujeto van a tender a evitar el cambio de emociones.</summary>
    void DecidirSentimientos()
    {
        // PUBLICO ESTATAL
        if (publicoEstatal && nivelEmocionActual < 100)
        {
            nivelEmocionActual++;
            if (emocionActual != EMOCION.AMORALLIDER)
            {
                CambiarEmocion(EMOCION.AMORALLIDER, 0);
            }
        }

        // PUBLICO MILITAR
        if (publicoMilitar && nivelEmocionActual < 100)
        {
            nivelEmocionActual++;
            if (emocionActual != EMOCION.HACERGUERRA)
            {
                CambiarEmocion(EMOCION.HACERGUERRA, 0);
            }
        }

        // PRIVADO ENTRETENIMIENTO
        if (privadoComercial && nivelEmocionActual < 100)
        {
            nivelEmocionActual++;
            if (emocionActual != EMOCION.INDIVIDUALISTA)
            {
                CambiarEmocion(EMOCION.INDIVIDUALISTA, 0);
            }
        }

        // PRIVADO COMERCIAL
        if (privadoEntretenimiento && nivelEmocionActual < 100)
        {
            nivelEmocionActual++;
            if (emocionActual != EMOCION.BOLUDEAR)
            {
                CambiarEmocion(EMOCION.BOLUDEAR, 0);
            }
        }

    }


    void DecidirQueHacer()
    {
        if (estado == Estado.IDLE)
        {
            // Si no no estamos alimentamos proseguimos con el iddle normalmente
            // Pero si nos deberiamos estar alimentando pero no hay comida, tambien
            // Para evitar que se paralice por tener hambre y no tener comida
            if (!modoAlimentacion ||
            (persona == null && modoAlimentacion && comidasPropias.Count < 1 ||
            persona != null && modoAlimentacion && persona.GetComponent<Seguido>().comidas.Count < 1)
            && !forzarMilitar)
            {
                // Lo unico que lo puede sacar de este estado seria que lo toque un usuario
                // Esto podria ser desde un OnCollision aca o en el usuario
                DecidirSubEstadoIdle();
            }
            else if (modoAlimentacion && !forzarIndividualista)
            {
                if (persona == null)
                {
                    if (comidasPropias.Count > 0)
                    {
                        ManejarAlimentacion();
                    }
                }
                else if (persona != null)
                {
                    if (persona.GetComponent<Seguido>().comidas.Count > 0)
                    {
                        ManejarAlimentacion();
                    }
                }

            }
            else if (forzarMilitar)
            {
                ManejarForzarMilitar();
            }
        }
        else if (estado == Estado.TRABAJANDO)
        {
            // Obtenemos el estado del persona y si se movio switcheamos aca a siguiendo
            if (persona != null)
            {
                if (!PersonaEstaParada())
                {
                    if (!forzarBoludear)
                    {
                        CambiarEstado(Estado.SIGUIENDO);
                    }
                    else if (forzarBoludear)
                    {
                        Girar();
                    }
                }
            }
            // Si no no estamos alimentamos proseguimos con el trabajando normalmente
            // Pero si nos deberiamos estar alimentando pero no hay comida, tambien
            // Para evitar que se paralice por tener hambre y no tener comida
            if (!modoAlimentacion || (modoAlimentacion && comidasPropias.Count < 1) && !forzarMilitar && !forzarBoludear)
            {
                DecidirSubEstadoTrabajando();
            }
            else if ((modoAlimentacion && comidasPropias.Count > 0 && persona == null ||
                    modoAlimentacion && persona != null && persona.GetComponent<Seguido>().comidas.Count > 0) &&
                    !forzarBoludear)
            {
                ManejarAlimentacion();
            }
            else if (forzarMilitar)
            {
                ManejarForzarMilitar();
            }
            else if (forzarBoludear)
            {
                Girar();
            }
        }
        else if (estado == Estado.SIGUIENDO)
        {
            // Obtenemos el estado del persona y si se paro switcheamos aca a trabajand
            if (persona != null)
            {

                if (PersonaEstaParada() && aiP.reachedDestination)
                {
                    if (!forzarBoludear && !forzarIndividualista)
                    {
                        CambiarEstado(Estado.TRABAJANDO);
                        an.SetTrigger("idle");
                    }
                    else if (forzarBoludear)
                    {
                        CambiarEstado(Estado.IDLE);
                        an.SetTrigger("idle");
                    }
                }
                else if (!PersonaEstaParada() && !aiP.reachedDestination)
                {
                    if (!aiP.pathPending)
                    {
                        an.SetTrigger("caminando");
                    }
                }
                if (forzarIndividualista)
                {
                    Debug.Log("Siguiendo en forzar individualista");
                    DesacoplarDePersona();
                }
            }
            // Si no hay persona porque se fue volvemos a IDLE (opcionalmente podriamos matarlo)
            if (persona == null)
            {
                CambiarEstado(Estado.IDLE);
            }
        }
        else if (estado == Estado.CONVIRTIENDOSE)
        {

        }
        else
        {
            Debug.Log("ERROR: no hay estado seleccionado en follower.");
        }
    }

    /// <summary>Cuando entramos en un Trigger debemos manejar los cambios. En principio las reglas debieran ser:
    /// Si es una persona (a traves del Seguido), lo manejamos desde aca porque el seguido no tiene OnTriggerEnter
    /// Si es con otro sujeto: de la misma persona, ignoramos. De otra persona, maneja quien tiene persona con mas seguidores.</summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        /*if(other.gameObject.tag == "persona"){
            if(persona == null || GameObject.ReferenceEquals(persona, other.gameObject)){
                ManejarColisionesConPersona(other);
            }
        }*/
        // Arreglo: si nuestra persona es null y si nuestra persona no es la misma que con la que estamos chocando
        // (antes estaba o si es la misma) 
        if (other.gameObject.tag == "persona")
        {
            if (persona == null)
            {
                // Si mi persona es null soy huerfano. Solo llamo a manejar la colision si enre los seguidores de
                // la persona con la que me estoy chocando no hay al menos 10 nacionalistas
                if (other.gameObject.GetComponent<Seguido>().ContarFollowersNacionalistas() < 10)
                {
                    ManejarColisionesHuerfanoConPersona(other);
                }
            }
        }
        if (other.gameObject.tag == "sujeto")
        {
            // Si somos huerfanos, no hacemos nada cuando nos encontramos con otro sujeto. Solo si nos encontramos con una person (arriba).
            // Pero no cuando nos encontramos con otro sujeto porque en caso de que el otro sea huerfano no pasa ninguna interaccion
            // y en caso de que el otro no lo sea se manejara en el otro (abajo)
            if (persona != null)
            {
                // Si el otro sujeto no tiene persona, es huerfano, manejamos aca
                if (other.gameObject.GetComponent<Follower>().persona == null)
                {
                    if (!forzarNacionalismo)
                    {
                        ManejarColisionesConSujetoHuerfano(other);
                    }
                }
                else if (persona.GetComponent<Seguido>().GetNumSeguidores() > other.gameObject.GetComponent<Follower>().persona.GetComponent<Seguido>().GetNumSeguidores())
                {
                    // Si el otro sujeto tiene persona y tiene menos seguidores que nosotros, manejamos aca
                    if (!forzarNacionalismo)
                    {
                        ManejarColisionesConSujeto(other);
                    }
                }
                if (GameObject.Equals(persona, other.gameObject.GetComponent<Follower>().persona))
                {
                    // Si las dos personas son la misma, estamos en presencia de un seguidor de nuestro propio grupo.
                    // Solo actuamos en caso de que haya que contagiar: si sentimos mas emocion que el umbral y si el otro no tiene nuestra emocion
                    // TODO: deberiamos agregar los niveles de emocion de rebelarse y huir
                    if (nivelEmocionActual > umbralContagioEmocion)
                    {
                        if (emocionActual != other.gameObject.GetComponent<Follower>().ObtenerEmocionActual())
                        {
                            ContagiarEmocion(other.gameObject, emocionActual);
                        }
                    }
                }
            }
        }
    }

    void ContagiarEmocion(GameObject sujeto, EMOCION emocionAContagiar)
    {
        sujeto.GetComponent<Follower>().RecibirContagio(emocionAContagiar, nivelEmocionActual);
        Debug.Log("CONTAGIANDO a " + sujeto.name + " con la emocion " + emocionAContagiar);
    }

    public void RecibirContagio(EMOCION emocionContagiada, int nivelEmocion)
    {
        CambiarEmocion(emocionContagiada, nivelEmocion - 10);
        Debug.Log("Recibiendo contagio de " + emocionContagiada + " con nivel de contagio " + nivelEmocion);
    }

    /// <summary>Cuando tocamos un trigger y es una persona y nosotros no tenemos persona 
    /// (somos huerfanos o tenemos una persona distinta).</summary>
    void ManejarColisionesHuerfanoConPersona(Collider2D other)
    {
        if (!forzarIndividualista)
        {
            other.gameObject.GetComponent<Seguido>().SolicitarEmpezarASeguir(gameObject, false);
        }
    }

    /// <summary>Cuando tocamos un trigger y es un sujeto huerfano y nosotros tenemos persona, lo captamos.</summary>
    void ManejarColisionesConSujetoHuerfano(Collider2D other)
    {
        // Notar que no le pasamos este sujeto como parametro, sino el que colisiono con nosotros
        // Notar que se lo mandamos a esta persona
        if (!forzarIndividualista)
        {
            persona.GetComponent<Seguido>().SolicitarEmpezarASeguir(other.gameObject, false);
        }
    }

    /// <summary>Cuando tocamos un trigger y es un sujeto cuya persona tiene menos seguidores que este</summary>
    void ManejarColisionesConSujeto(Collider2D other)
    {
        // Notar que no le pasamos este sujeto como parametro, sino el que colisiono con nosotros
        // Notar que se lo mandamos a esta persona
        // Notar que le pasamos true porque tiene persona
        persona.GetComponent<Seguido>().SolicitarEmpezarASeguir(other.gameObject, true);
    }

    // Esta funcion se ejecuta desde la persona como devolucion a cuando le mandamos DejarDeSeguir();
    // También se ejecuta desde la persona cuando empezamos a segur a otra persona con mas seguidores, en la transición
    // Entre seguir a una y otra persona.
    public void VaciarSeguido(GameObject exSeguido)
    {
        persona = null;
        CambiarEstado(Estado.CONVIRTIENDOSE);
        colorSpriteBicho = new Color(1, 1, 1, 1);
        colorSpriteZona = new Color(1, 1, 1, 0);
        sR.color = colorSpriteBicho;
    }

    //Esta funcion la llamamos desde el seguido, nos la devuelve cuando le damos a EmpezarASeguir();
    public void ConfirmarNuevoSeguido(GameObject nuevoSeguido)
    {
        persona = nuevoSeguido.transform;
        CambiarEstado(Estado.SIGUIENDO);
        colorSpriteBicho = nuevoSeguido.GetComponent<Seguido>().GetColorSprite();
        colorSpriteZona = new Color(colorSpriteBicho.r, colorSpriteBicho.g, colorSpriteBicho.b, 0.25f);
        sR.color = colorSpriteBicho;


        // Le transferimos todas nuestras comidas a la persona
        List<GameObject> comidasATransferir = new List<GameObject>(comidasPropias);
        comidasPropias.Clear();
        persona.GetComponent<Seguido>().AgregarComidasNuevoSeguidor(comidasATransferir);
        foreach (var item in comidasATransferir)
        {
            item.GetComponent<ComidaNueva>().ConfigurarSoloPersona(persona.gameObject);
        }
    }



    // INDIVIDUALISTA
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    void DesacoplarDePersona()
    {
        // En vez de vacir el seguido directamente, en cuyo caso la persona quedaría sin avisar, le mandamos 
        // a la persona un dejar de seguir, que nos devuelve un vaciar seguido, asi ambas partes quedan actualizadas
        //VaciarSeguido(persona.gameObject);
        persona.GetComponent<Seguido>().DejarDeSeguir(gameObject);
    }

    void ForzarIndividualista()
    {
        if (!forzarIndividualista)
        {
            /*esperarIndividualistaActual = 0;
            esperarIndividualistaTotal = Random.Range(120, 450);*/
            forzarIndividualista = true;
        }
    }



    // MILITAR
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    void ForzarMilitar()
    {
        if (!forzarMilitar)
        {
            /*esperarMilitarActual = 0;
            esperarMilitarTotal = Random.Range(260, 620);
            forzarMilitar = true;*/
            // TODO: aca segría un buen lugar para disparar la animación militar
        }
    }

    void SalirForzarMilitar()
    {
        forzarMilitar = false;
        tiempoActualTrabajar = 0;
        tiempoActualRumiar = 0;
        subEstadoActualIdle = IDLE.buscandoLugar;
        subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
    }

    void ManejarForzarMilitar()
    {
        if (persona != null)
        {
            if (militar_haySujetosEnRango)
            {
                //Seteamos el destination setter en el punto en el que se detecto a alguien
                Vector3 pos = persona.GetComponent<Seguido>().Detector().ObtenerUltimaPosDetectada();
                gds.SetDestination(pos);
                gds.SetDistanciaObjetivo(20);
            }
            else
            {
                // Seteamos el destination setter en el lugar de la formacion y esperamos tranquilamente
                SetPosicionFormacionMilitar(true);
                gds.SetDistanciaObjetivo(3);
            }
        }
    }

    void ActualizarHaySujetosEnRango()
    {
        if (persona != null)
        {
            persona.GetComponent<Seguido>().ObtenerHaySujetosAjenosEnRango();
        }
    }


    void SetPosicionFormacionMilitar(bool tienePersona)
    {
        // if(tienePersona && !militar_militar_haySujetosEnRango)
        if (tienePersona)
        {
            float multiplicadorUnidad = 15f;


            Vector3 posPersona = persona.transform.position;
            float offsetX = 0;
            float offsetY = -1f * multiplicadorUnidad;


            // Definimos la posicion en X
            if (indexSeguidorEnPersona % 4 == 0)
            {
                offsetX = -1.5f * multiplicadorUnidad;
            }
            else if (indexSeguidorEnPersona % 4 == 1)
            {
                offsetX = -.5f * multiplicadorUnidad;
            }
            else if (indexSeguidorEnPersona % 4 == 2)
            {
                offsetX = .5f * multiplicadorUnidad;
            }
            else if (indexSeguidorEnPersona % 4 == 3)
            {
                offsetX = 1.5f * multiplicadorUnidad;
            }

            // Definimos la posición en Y partiendo del offset base
            offsetY -= Mathf.CeilToInt(indexSeguidorEnPersona / 4) * multiplicadorUnidad;

            // Conformamos el vector definitvo
            Vector3 posEnFormacion = new Vector3(posPersona.x + offsetX, posPersona.y + offsetY, posPersona.z);

            // Lo asignamos para que lo siga
            gds.SetDestination(posEnFormacion);
            gds.SetDistanciaObjetivo(1f);
            Debug.Log("Se asigno una posición en formacion militar a " + name + ": " + posEnFormacion);
        }
    }


    // NACIONALISMO
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    void ForzarNacionalismo()
    {
        if (!forzarNacionalismo)
        {
            /*esperarNacionalismoActual = 0;
            esperarNacionalismoTotal = Random.Range(260, 620);*/
            forzarNacionalismo = true;
            // TODO: aca segría un buen lugar para disparar la animación
        }
    }


    // Trabajando
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    void DecidirSubEstadoTrabajando()
    {
        if (subEstadoActualTrabajando == TRABAJANDO.buscandoTrabajo)
        {
            if (persona != null)
            {
                // Si tenemos persona, sebemos buscar un deposito dentro de un margen de la persona
                if (!esperandoDeposito)
                {
                    ActualizarDepositosDeComidaCercanos(persona.transform.position, 40, true);
                }
                if (depositosDeComidaCercanos.Count > 0)
                {
                    int randomIndex = Random.Range(0, depositosDeComidaCercanos.Count - 1);
                    depositosDeComidaSeleccionado = depositosDeComidaCercanos[randomIndex];
                    posLugarDeTrabajo = depositosDeComidaSeleccionado.transform.position;
                }
                else
                {
                    esperandoDeposito = true;
                    if (esperarDepositoActual > esperarDepositoTotal)
                    {
                        esperarDepositoActual = 0;
                        esperandoDeposito = false;
                    }
                    else
                    {
                        esperarDepositoActual++;
                    }
                }
            }
            else
            {
                // Si no tenemos persona vamos al deposito más cercano sin importar la distancia
                Debug.Log("probando sin persona");
                ActualizarDepositosDeComidaCercanos(transform.position, 0, false);
                if (depositosDeComidaCercanos.Count > 0)
                {
                    int index = 0;
                    depositosDeComidaSeleccionado = depositosDeComidaCercanos[index];
                    if (depositosDeComidaSeleccionado != null)
                    {
                        //Hacemos este chequeo extra porque porla demora en actualizar los depositos puede ser que intente ir a una ya destruido
                        posLugarDeTrabajo = depositosDeComidaSeleccionado.transform.position;
                    }
                }
                else
                {
                    esperandoDeposito = true;
                    if (esperarDepositoActual > esperarDepositoTotal)
                    {
                        esperarDepositoActual = 0;
                        esperandoDeposito = false;
                    }
                    else
                    {
                        esperarDepositoActual++;
                    }
                }
            }

            if (posLugarDeTrabajo != Vector3.zero && posLugarDeTrabajo != null)
            {
                gds.SetDestination(posLugarDeTrabajo);
                gds.SetDistanciaObjetivo(20);
                subEstadoActualTrabajando = TRABAJANDO.caminandoAlTrabajo;
                aiP.canSearch = true;
                float vel = SetVelocidadRandom();
                ActualizarVelAn(vel);
                aiP.maxSpeed = vel;
            }
        }
        else if (subEstadoActualTrabajando == TRABAJANDO.caminandoAlTrabajo)
        {
            if (posLugarDeTrabajo == Vector3.zero || posLugarDeTrabajo == null)
            {
                Debug.LogWarning("Error: Se intenta ir a un lugar de trabajo no inicializado");
                subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                return;
            }

            if (!aiP.pathPending)
            {
                an.SetTrigger("caminandoPico");
            }

            if (emocionActual == EMOCION.BOLUDEAR)
            {
                if (Utilidades.RandomWeightedBool(nivelEmocionActual, 1000))
                {
                    ForzarBoludear();
                }
            }

            if (aiP.reachedDestination)
            {
                subEstadoActualTrabajando = TRABAJANDO.trabajando;
                an.SetTrigger("usandoPico");
                ActualizarVelAn(0);
                SetTiempoTrabajar();
            }

            // Si no ha llegado a detino el AIPath lo va a llevar, acá no debemos hacer nada

        }
        else if (subEstadoActualTrabajando == TRABAJANDO.trabajando)
        {
            // Aca que espere un rato
            if (tiempoActualTrabajar < tiempoTotalTrabajar)
            {
                tiempoActualTrabajar++;
                transform.LookAt(posLugarDeTrabajo);

                if (emocionActual == EMOCION.INDIVIDUALISTA)
                {
                    if (Utilidades.RandomWeightedBool(nivelEmocionActual, 1000))
                    {
                        ForzarIndividualista();
                    }
                }

            }
            else
            {
                tiempoActualTrabajar = 0;
                // Aca entramos cuando ya termino el trabajo, debemos crear una comida.
                an.SetTrigger("idle");
                // Creamos la comida, la configuramos y la almacenamos en nuestro array
                GameObject comidaCreada = null;
                if (depositosDeComidaSeleccionado != null)
                {
                    comidaCreada = depositosDeComidaSeleccionado.GetComponent<depositoDeComida>().Cosechar();
                }
                if (persona != null && comidaCreada != null)
                {
                    comidaCreada.GetComponent<ComidaNueva>().Configurar(persona.gameObject, gameObject, null);
                    persona.GetComponent<Seguido>().AgregarComidaDeSeguidor(comidaCreada);
                }
                else if (persona == null && comidaCreada != null)
                {
                    //Si las comidas propias son menos que los spots que tenemos menos 1 (es decir hay un lugar)
                    if (comidasPropias.Count < 5)
                    {
                        Transform spot = transform.Find("puntosComida").GetChild(comidasPropias.Count);
                        comidaCreada.GetComponent<ComidaNueva>().Configurar(null, gameObject, spot.gameObject);
                        comidasPropias.Add(comidaCreada);
                    }
                    else
                    {
                        Destroy(comidaCreada);
                    }
                }

                // Si no hay persona y hay no tenemos comida llena, podemos ir a IDLE o quedarnos
                if (persona == null && comidasPropias.Count < 6)
                {
                    int pos = Random.Range(0, 100);

                    if (forzarIndividualista)
                    {
                        pos = 100;
                    }

                    if (pos > 50)
                    {
                        subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                    }
                    else
                    {
                        CambiarEstado(Estado.IDLE);
                    }
                }
                // Si no hay persona y tenemos mas de 6 vamos si o si a IDLE
                if (persona == null && comidasPropias.Count >= 6)
                {
                    CambiarEstado(Estado.IDLE);
                }
                // Si hay persona seguimos trabajando hasta el infinito
                if (persona != null)
                {
                    subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                }
            }
        }
        else
        {
            Debug.LogError("ERROR: no hay ningun subestado trabajando asignado");
        }

    }

    /// <summary> Llama a la funcion de las globalVariable spara guardar localmente cuales son los dpeositos cercnaos,
    /// si los hubiera. En el caso de que haya persona debemos pasarla como puntoRef, en caso de ser huerfanos pasamos 
    /// la pos actual del bicho como puntoRef y la distancia no importa. </summary>
    void ActualizarDepositosDeComidaCercanos(Vector3 puntoRef, float distancia, bool restringirPorDistancia)
    {
        if (restringirPorDistancia)
        {
            // En este caso la hemos llamado sigueindo a una persona para no alejarnos mucho de la persona
            depositosDeComidaCercanos = gV.ObtenerDepositosDeComidaOrdenadosPorCercania(puntoRef, distancia);
        }
        else
        {
            // En este caso la hemos llamado siendo huéfanos, no restringimos por distancia, obtenemos toda la lista.
            depositosDeComidaCercanos = gV.ObtenerDepositosDeComidaOrdenadosPorCercania(puntoRef);
        }
    }






    // BOLUDEAR
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    void ForzarBoludear()
    {
        if (!forzarBoludear)
        {
            /*esperarBoludearActual = 0;
            esperarBoludearTotal = Random.Range(60, 240);*/
            forzarBoludear = true;
            CambiarEstado(Estado.IDLE);
            // TODO: aca segría un buen lugar para disparar la animación de pensando / boludeando
        }
    }

    void Girar()
    {
        Debug.Log("Girando");
        transform.Rotate(0, 0, 1);
    }



    // IDLE
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    void DecidirSubEstadoIdle()
    {
        if (subEstadoActualIdle == IDLE.buscandoLugar)
        {
            float vel = SetVelocidadRandom();
            aiP.maxSpeed = vel;
            gds.SetDestination(Utilidades.PuntoRandom(gV.piso, transform.position, 50));
            gds.SetDistanciaObjetivo(15);
            subEstadoActualIdle = IDLE.caminando;
            an.SetTrigger("caminando");
            ActualizarVelAn(vel);

        }
        else if (subEstadoActualIdle == IDLE.caminando)
        {
            if (aiP.reachedDestination)
            {
                subEstadoActualIdle = IDLE.rumiando;
                ActualizarVelAn(0);
                SetTiempoRumiar();
            }
        }
        else if (subEstadoActualIdle == IDLE.rumiando)
        {
            //Chequear el contador random y si se cumple, vaciar el destino idle y pasarlo a buscando objetivo
            if (tiempoActualRumiar >= tiempoTotalRumiar)
            {
                tiempoActualRumiar = 0;
                int pos = Random.Range(0, 100);

                if (forzarIndividualista)
                {
                    pos = 1;
                }

                if (pos > 50)
                {
                    subEstadoActualIdle = IDLE.buscandoLugar;
                }
                else
                {
                    if (!forzarBoludear)
                    {
                        CambiarEstado(Estado.TRABAJANDO);
                    }
                    else
                    {
                        Debug.Log("Yendo a IDLE forzado por boludeo");
                        subEstadoActualIdle = IDLE.buscandoLugar;
                    }
                }
            }
            else
            {
                tiempoActualRumiar++;

                if (emocionActual == EMOCION.INDIVIDUALISTA)
                {
                    if (Utilidades.RandomWeightedBool(nivelEmocionActual, 1000))
                    {
                        ForzarIndividualista();
                    }
                }
            }
        }
        else
        {
            Debug.LogError("ERROR: no hay un subestado iddle seleccionado.");
        }
    }




    // Utilidades
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    void SetTiempoRumiar()
    {
        tiempoActualRumiar = 0;
        tiempoTotalRumiar = Random.Range(200, 450);
    }
    void SetTiempoTrabajar()
    {
        tiempoActualTrabajar = 0;
        tiempoTotalTrabajar = Random.Range(120, 500);
    }

    void ActualizarVelAn(float vel)
    {
        if (vel == 0)
        {
            an.SetFloat("velocidad", 0);
        }
        else
        {
            an.SetFloat("velocidad", .33f + Utilidades.Map(vel, 0f, 10f, 0f, 1f, true));
        }
    }

    float SetVelocidadRandom()
    {
        return Random.Range(velMin, velMax);
    }

    /// <summary>Siempre que pasamos de un estado al otro no deberiamos asignar la variable directamente, si no pasar ppor aca
    /// para hacer todsos los chequeos en un solo lugar y si fuera necesario implementar hooks</summary>
    void CambiarEstado(Estado nuevoEstado)
    {
        if (estado != nuevoEstado)
        {
            if (nuevoEstado == Estado.TRABAJANDO)
            {
                subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                an.SetTrigger("idle");
                aiP.canSearch = false;
                //gds.ClearDestination();
            }

            if (nuevoEstado == Estado.IDLE)
            {
                subEstadoActualIdle = IDLE.buscandoLugar;
            }

            if (nuevoEstado == Estado.SIGUIENDO)
            {
                an.SetTrigger("caminando");
                aiP.canSearch = true;
                gds.SetDestination(persona, 15);
                gds.SetDistanciaObjetivo(30 + 10 * persona.GetComponent<Seguido>().GetNumSeguidores() / 10);
            }

            if (estado == Estado.TRABAJANDO)
            {
                depositosDeComidaSeleccionado = null;
                esperarDepositoActual = 0;
                esperandoDeposito = false;
                posLugarDeTrabajo = Vector3.zero;
                depositosDeComidaSeleccionado = null;
            }

            //Debug.Log("Se efectuo un cambio de estado: de " + estado + " a " + nuevoEstado);
            estado = nuevoEstado;
        }
    }

    /// <summary> Cuando la persona actualiza su lista d eseguidores (agrega o quita a alguien) llama a esta funcion</summary>
    /// <summary> en todos sus seguidores para tenerlos actualizados de que lugar ocupan en sus seguidores.</summary>
    public void ActualizarIndexEnSeguido(int index)
    {
        indexSeguidorEnPersona = index;
    }

    /// <summary> Para cambiar cualquier emocion no se debe asignar directamente, sino que debriamos pasar por aca </summary>
    /// <summary> por tema checkeos y hooks. El segundo parametro siempre debe ser 0 en un cambio normal, solo se usa </summary>
    /// <summary> en caso de que sea un cambio por contagio, en el que se pasa cuan emocionado está el contagiante - 10 </summary> 
    void CambiarEmocion(EMOCION nuevaEmocion, int nivelNuevaEmocion)
    {
        if (emocionActual != nuevaEmocion)
        {
            if (nuevaEmocion == EMOCION.AMORALLIDER)
            {
                hueColorEmocion = 350;
                brightnessColorEmocion = 75;
                publicoEstatal = true;
                publicoMilitar = false;
                privadoComercial = false;
                privadoEntretenimiento = false;
            }
            if (nuevaEmocion == EMOCION.HACERGUERRA)
            {
                hueColorEmocion = 63;
                brightnessColorEmocion = 75;
                publicoEstatal = true;
                publicoMilitar = false;
                privadoComercial = false;
                privadoEntretenimiento = false;
            }
            if (nuevaEmocion == EMOCION.BOLUDEAR)
            {
                hueColorEmocion = 250;
                brightnessColorEmocion = 75;
                publicoEstatal = false;
                publicoMilitar = false;
                privadoComercial = false;
                privadoEntretenimiento = true;
            }
            if (nuevaEmocion == EMOCION.INDIVIDUALISTA)
            {
                hueColorEmocion = 141;
                brightnessColorEmocion = 75;
                publicoEstatal = false;
                publicoMilitar = false;
                privadoComercial = true;
                privadoEntretenimiento = false;
            }
            if (nuevaEmocion == EMOCION.NADA)
            {
                hueColorEmocion = 0;
                brightnessColorEmocion = 100;
            }
            nivelEmocionActual = nivelNuevaEmocion;
            emocionActual = nuevaEmocion;
        }
        else
        {
            Debug.LogWarning("Atencion: llamando a cambiar emocion entre dos emociones iguales: Actual: " + emocionActual + " Nueva: " + nuevaEmocion);
        }
    }

    void AsignarColorEmocion()
    {
        float h = Utilidades.Map(hueColorEmocion, 0, 360, 0, 1, true);
        // Para la saturacion: entramos con el nivel de emocion conmo variable a mapear. A mayor intensidad,
        // mayor saturacion del color. Mapeamos entre un umbral minimo (100 como prueba) y un umbral maximo donde ya se muestra
        // el maximo posible del color
        float s = 0;
        if (emocionActual != EMOCION.NADA)
        {
            s = Utilidades.Map(nivelEmocionActual, 100, 3500, 0, 1, true);
        }
        else
        {
            s = 0;
        }

        float v = Utilidades.Map(brightnessColorEmocion, 0, 100, 0, 1, true);
        colorSpriteEmocion = Color.HSVToRGB(h, s, v);
        // El alfa se lo asignamos como la saturacion, es decir, que empiece de 0 y vaya aumentando con la intensidad de la emoción.
        colorSpriteEmocion.a = s;
    }

    void Morir(string tipoMuerte)
    {
        if (tipoMuerte == "hambre")
        {
            if (persona != null)
            {
                persona.GetComponent<Seguido>().AvisarMuerteSeguidor(gameObject);
            }
            Destroy(gameObject);
        }
        else if (tipoMuerte == "SalidoDelMapa")
        {
            if (persona != null)
            {
                persona.GetComponent<Seguido>().AvisarMuerteSeguidor(gameObject);
            }
            Destroy(gameObject);
        }

        gV.RestarPoblacion(gameObject);
    }

    void AcualizarAntenaDeEmocionEstaTransmitiendo()
    {
        if (emocionActual == EMOCION.AMORALLIDER)
        {
            if (gV.totemLider.GetComponent<totem>().ObtenerEstaTransmitiendo())
            {
                antenaDeEmocionEstaTransmitiendo = true;
            }
            else
            {
                antenaDeEmocionEstaTransmitiendo = false;
            }
        }
        if (emocionActual == EMOCION.BOLUDEAR)
        {
            if (gV.totemEntretenimiento.GetComponent<totem>().ObtenerEstaTransmitiendo())
            {
                antenaDeEmocionEstaTransmitiendo = true;
            }
            else
            {
                antenaDeEmocionEstaTransmitiendo = false;
            }
        }
        if (emocionActual == EMOCION.HACERGUERRA)
        {
            if (gV.totemMilitar.GetComponent<totem>().ObtenerEstaTransmitiendo())
            {
                antenaDeEmocionEstaTransmitiendo = true;
            }
            else
            {
                antenaDeEmocionEstaTransmitiendo = false;
            }
        }
        if (emocionActual == EMOCION.INDIVIDUALISTA)
        {
            if (gV.totemIndividualista.GetComponent<totem>().ObtenerEstaTransmitiendo())
            {
                antenaDeEmocionEstaTransmitiendo = true;
            }
            else
            {
                antenaDeEmocionEstaTransmitiendo = false;
            }
        }
    }



    // Accedidas desde ONDAS
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    public void AfectarPorAntena(TIPOTOTEM totem)
    {

        //Funciona asi: la antena baja la intensidad de cualquier emocion que no sea la de la propia antena
        // con la emocion de la propia antena no hace nada, ya que ese valor (el nivelEMocionActual) se incrementa solo por tiempo
        // pero si va reduciendo el nivelEmocionActual si es otra antena distinta. Si ese nivel penetra 1 para abajo,
        // se produce un cambio de Emocion en el followe
        switch (totem)
        {
            case TIPOTOTEM.PUBLICOESTATAL:
                if (!publicoEstatal)
                {
                    nivelEmocionActual--;
                    if (nivelEmocionActual < 1)
                    {
                        publicoEstatal = true;
                    }
                }
                break;
            case TIPOTOTEM.PUBLICOMILITAR:
                if (!publicoMilitar)
                {
                    nivelEmocionActual--;
                    if (nivelEmocionActual < 1)
                    {
                        publicoMilitar = true;
                    }
                }
                break;
            case TIPOTOTEM.PRIVADOENTRETENIMIENTO:
                if (!privadoEntretenimiento)
                {
                    nivelEmocionActual--;
                    if (nivelEmocionActual < 1)
                    {
                        privadoEntretenimiento = true;
                    }
                }
                break;
            case TIPOTOTEM.PRIVADOCOMERCIAL:
                if (!privadoComercial)
                {
                    nivelEmocionActual--;
                    if (nivelEmocionActual < 1)
                    {
                        privadoComercial = true;
                    }
                }
                break;
            default:
                Debug.Log("ERROR: Se envio un 'AfectarAntena()' sin tipo de totem");
                break;
        }
    }


    // HAMBRE Y RECURSOS
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary> En vez de asignar la variable de modo de alimentacion directamente siempre pasamos por aca
    /// para aprovechar controles y hooks. </summary>
    void CambiarModoAlimentacion(bool e)
    {
        modoAlimentacion = e;
    }

    /// <summary> Entramos aca cada 3 o 4 segundos segun el ciclo asignado al azar.
    /// En principio en cualquier ciclo aumentaos el hambre en un punto y si ademas
    /// tenemos mas hambre que el umbral ponemos modo alimentacion on a traves de IntentarAlimentarse.
    /// También lo matamos si el hambre supera el umbral de muerte. </summary>
    void ManejarHambre()
    {
        
        hambre += 2;

        if (hambre > umbralHambreAlimentarse)
        {
            // Solo nos intentamos alimentar si no tenemos persona y tenemos comida propia, o si nuestra persona,  en caso de tenerla, tiene comida
            if((persona == null && comidasPropias.Count > 0) || (persona != null && persona.GetComponent<Seguido>().comidas.Count > 0)){
                IntentarAlimentarse();
            }

            if (porcentajeDescontento < 100)
            {
                porcentajeDescontento += 2;
            }
        }

        if (hambre > 50)
        {
            if (porcentajeDescontento < 100)
            {
                porcentajeDescontento += 4;
            }
        }

        if (hambre > umbralHambreMuerte)
        {
            Morir("hambre");
        }
    }

    /// <summary> Prende el modo alimentacion para tener prioridad para comer en los estados de 
    ///  IDLE y TRABAJANDO, no asi en siguiendo que mantiene su comportamiento. </summary>
    void IntentarAlimentarse()
    {
        CambiarModoAlimentacion(true);
    }

    /// <summary> Se hace efectivo desde la comida NO DESDE ACA. Bajamos 
    ///  hambre y por ende dejamos de estar en modo alimentacion. </summary>
    public void Alimentarse()
    {
        hambre = 0;
        CambiarModoAlimentacion(false);
    }

    /// <summary> Esta funcion maneja los estados de las distintas etapas del proceso de comer. 
    /// Es llamada cuando estamos TRABAJANDO o en IDLE y esta activado el modoAlimentacion (porque hay hambre) </summary>
    void ManejarAlimentacion()
    {
        // Si hay persona nos embrollamos en todo el proceso correpondiente para comer una comida.
        // TODO: si somos huerfanos habria que reemplazar el IDLE por el trabajando, o alternar un ciclo de cada uno
        // sino se van a morir de hambre los huerfanos
        if (persona != null)
        {
            if(!comidaSeleccionada)
            {
                // Seleccionar comida de la persona
                comidaSeleccionada = true;
                subEstadoActualComiendo = COMIENDO.SELECCIONANDOCOMIDA;
            }

            if (subEstadoActualComiendo == COMIENDO.SELECCIONANDOCOMIDA)
            {
                for(int i = 0; i < persona.GetComponent<Seguido>().comidas.Count; i++)
                {
                    comidaPreseleccionada =  persona.GetComponent<Seguido>().ObtenerComidaDisponible();
                    if(comidaPreseleccionada != null)
                    {
                        // Si encontramos una comida que no esta preseleciconada, salimos del loop y vamos al siguiente modo
                        gds.SetDestination(comidaPreseleccionada.transform);
                        subEstadoActualComiendo = COMIENDO.CAMINANDOACOMIDA;
                        break;
                    }
                }

                // Si llegamos hasta aca es porqe en todo el loop ninguna comida esta disponible. Desactivamos el modo alimentaicon
                CambiarModoAlimentacion(false);
                return;
            }
            if (subEstadoActualComiendo == COMIENDO.CAMINANDOACOMIDA)
            {
                if(!aiP.pathPending && !parado)
                {
                    SetAnimacion(ANIMACION.CAMINANDO);
                }

                if(aiP.reachedDestination)
                {
                    // TODO: podriamos poner la animacion comiendo
                    SetAnimacion(ANIMACION.IDLE);
                    subEstadoActualComiendo = COMIENDO.COMIENDO;
                }
            }
            if (subEstadoActualComiendo == COMIENDO.COMIENDO)
            {
                // comer comida
                comidaPreseleccionada.GetComponent<ComidaNueva>().Comer(gameObject);
                comidaSeleccionada = false;
                comidaPreseleccionada = null;
            }
        }
        // Si somos huerfanos simplemente comemos una de las comidas que tengamos almacenadas.
        else if (persona == null)
        {
            if (comidasPropias.Count > 0)
            {
                SeleccionarComidaPropia();
                if (emocionActual == EMOCION.INDIVIDUALISTA)
                {
                    if (comidasPropias.Count > 0)
                    {
                        SeleccionarComidaPropia();
                    }
                    // Si es individualista comemos dos veces por gloton. 
                    // TODO: habria que implementarlo en el ciclo de comer de la persona.
                }
            }
        }
    }

    void SeleccionarComidaPropia()
    {
        GameObject comidaComer = comidasPropias[comidasPropias.Count - 1];
        comidaComer.GetComponent<ComidaNueva>().Comer(gameObject);
        comidasPropias.Remove(comidaComer);
    }





    // REVOLUCION
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////


    /// <summary> Esta funcion maneja los estados de las distintas etapas del proceso de comer. 
    /// Es llamada cuando estamos TRABAJANDO o en IDLE y esta activado el modoAlimentacion (porque hay hambre) </summary>
    public void ActivarProcesoRevolucionario()
    {
        forzarRevolucion = true;
    }

    public void DesactivarProcesoRevolucionario()
    {
        forzarRevolucion = false;
        estadoRevolucionInicializado = false;
    }

    /// <summary>
    /// Tenemos que pasar por 8 estados (en realidad 5 porque los primeros 3 son en paralelo)
    /// El flow seria asi: basado en azar, puede que le toque 
    /// A) Encontrar una antena -> ir -> apagar la antena -> buscar por donde salir -> salir
    /// B) Encontrar un punto en el piso (o un deposito) -> ir -> prender fuego -> buscar por donde salir -> salir
    /// </summary>
    void DecidirQueHacerRevolucion()
    {
        if (!estadoRevolucionInicializado)
        {
            if (Utilidades.RandomWeightedBool(1, 1))
            {
                esIncendiario = true;
                estadoRevolucionActual = REVOLUCION.BUSCANDOPRENDERFUEGO;
            }
            else
            {
                esSaboteador = true;
                estadoRevolucionActual = REVOLUCION.BUSCANDOANTENA;
            }
            estadoRevolucionInicializado = true;
        }


        if (esIncendiario)
        {
            if (estadoRevolucionActual == REVOLUCION.BUSCANDOPRENDERFUEGO)
            {
                if (depositosDeComidaCercanos.Count > 0)
                {
                    int randomIndex = Random.Range(0, depositosDeComidaCercanos.Count - 1);
                    depositosDeComidaSeleccionado = depositosDeComidaCercanos[randomIndex];
                    posPrenderFuego = depositosDeComidaSeleccionado.transform.position;
                }
                else
                {
                    posPrenderFuego = Utilidades.PuntoRandom(gV.piso);
                }

                if (posPrenderFuego != Vector3.zero && posLugarDeTrabajo != null)
                {
                    gds.SetDestination(posPrenderFuego);
                    gds.SetDistanciaObjetivo(15);
                    aiP.canSearch = true;
                    an.SetTrigger("caminandoCartel");
                    float vel = SetVelocidadRandom();
                    ActualizarVelAn(vel);
                    aiP.maxSpeed = vel;
                    estadoRevolucionActual = REVOLUCION.YENDOAPRENDERFUEGO;
                }
            }
            if (estadoRevolucionActual == REVOLUCION.YENDOAPRENDERFUEGO)
            {
                if (aiP.reachedDestination)
                {
                    estadoRevolucionActual = REVOLUCION.PRENDIENDOFUEGO;
                }
            }
            if (estadoRevolucionActual == REVOLUCION.PRENDIENDOFUEGO)
            {
                if (progresoIncendioActual > progresoIncendioTotal)
                {
                    Instantiate(prefabIncendio, transform.position, Quaternion.identity);
                    estadoRevolucionActual = REVOLUCION.BUSCANDOPORDONDESALIR;
                }
                else
                {
                    progresoIncendioActual++;
                }
            }
        }


        if (esSaboteador)
        {
            if (estadoRevolucionActual == REVOLUCION.BUSCANDOANTENA)
            {
                var totemsEnEscena = new GameObject[] { gV.totemEntretenimiento, gV.totemIndividualista, gV.totemLider, gV.totemMilitar };
                if (totemsEnEscena.Length > 0)
                {
                    int randomIndex = Random.Range(0, depositosDeComidaCercanos.Count - 1);
                    totemSeleccionado = totemsEnEscena[randomIndex];
                }
                else
                {
                    esIncendiario = true;
                    esSaboteador = false;
                    estadoRevolucionActual = REVOLUCION.BUSCANDOPRENDERFUEGO;
                }

                if (totemSeleccionado != null)
                {
                    gds.SetDestination(totemSeleccionado.transform);
                    gds.SetDistanciaObjetivo(25);
                    aiP.canSearch = true;
                    an.SetTrigger("caminandoCartel");
                    float vel = SetVelocidadRandom();
                    ActualizarVelAn(vel);
                    aiP.maxSpeed = vel;
                    estadoRevolucionActual = REVOLUCION.YENDOAANTENA;
                    intencionDeApagarAntena = true;
                }
            }
            if (estadoRevolucionActual == REVOLUCION.YENDOAANTENA)
            {
                if (aiP.reachedDestination)
                {
                    estadoRevolucionActual = REVOLUCION.APAGANDOANTENA;
                }
            }
            if (estadoRevolucionActual == REVOLUCION.APAGANDOANTENA)
            {
                // Aca en lugar de un contador debemos quedarnos hasta que efectivamente se apague la antena
                // Esto puede ocurrir por accion directa del sujeto o antes de que este llegue, de parte de otros
                // o incluso casualmente de un usuario
                if (!totemSeleccionado.GetComponent<totem>().ObtenerEstaTransmitiendo())
                {
                    estadoRevolucionActual = REVOLUCION.BUSCANDOPORDONDESALIR;
                    intencionDeApagarAntena = false;
                }
            }
        }


        if (estadoRevolucionActual == REVOLUCION.BUSCANDOPORDONDESALIR)
        {
            int randomIndex = Random.Range(0, 4);
            GameObject salidaElegida = gV.markersLimites[randomIndex];
            if (randomIndex == 0)
            {
                // caso de que elegimos el costado izquierdo
                float offsetX = -100;
                float offsetY = Random.Range(-150f, 150f);

                posSalidaElegida = new Vector3(salidaElegida.transform.position.x + offsetX, salidaElegida.transform.position.y + offsetY, salidaElegida.transform.position.z);
            }
            else if (randomIndex == 1)
            {
                // caso de que elegimos el costado derecho
                float offsetX = 100;
                float offsetY = Random.Range(-150f, 150f);

                posSalidaElegida = new Vector3(salidaElegida.transform.position.x + offsetX, salidaElegida.transform.position.y + offsetY, salidaElegida.transform.position.z);
            }
            if (randomIndex == 2)
            {
                // caso de que elegimos el costado de arriba
                float offsetX = Random.Range(-100f, 100f);
                float offsetY = 100f;

                posSalidaElegida = new Vector3(salidaElegida.transform.position.x + offsetX, salidaElegida.transform.position.y + offsetY, salidaElegida.transform.position.z);
            }
            if (randomIndex == 0)
            {
                // caso de que elegimos el costado de abajo
                float offsetX = Random.Range(-100f, 100f);
                float offsetY = -100f;

                posSalidaElegida = new Vector3(salidaElegida.transform.position.x + offsetX, salidaElegida.transform.position.y + offsetY, salidaElegida.transform.position.z);
            }

            if (posSalidaElegida != null && posSalidaElegida != Vector3.zero)
            {
                gds.SetDestination(posSalidaElegida);
                gds.SetDistanciaObjetivo(10);
                aiP.maxSpeed = velMax;
                estadoRevolucionActual = REVOLUCION.SALIENDO;
            }
            else
            {
                Debug.Log("ERROR: pasamos por todas las opciones en BUSCANDOPORDONDESALIR y el punto por donde salir sigue sin inicializarse");
            }
        }
        if (estadoRevolucionActual == REVOLUCION.SALIENDO)
        {
            if (aiP.reachedDestination)
            {
                Morir("SalidoDelMapa");
            }
        }

    }

    void AdministrarPrenderAntenas()
    {
        gds.SetDestination(totemSeleccionado.transform);
        gds.SetDistanciaObjetivo(23);

        if (aiP.reachedDestination)
        {
            intencionDeApagarAntena = true;
        }

        // De este estado deberia salir solo porque desde el update cuando invoque a totemEmocionActualEstaPrendido()
        // y vea que está prendido debería dejar de ingresar aca
    }
}

