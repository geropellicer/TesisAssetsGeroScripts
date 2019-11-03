using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Maneja el comportamiento del sujeto alrededor de una persona y en estado Idle cuando no hay persona.
/// </summary>

/// <summary>Lista de posibles estados que puede adquirir un follower.
/// IDLE: No hay persona, boludea 
/// SIGUIENDO: Se dedica a ir tras una persona en movimiento.
/// CONVIRTIENDOSE: Frame/s de transicion de cuando estaba siguiendo a una persona y otra persona con mas seguidores lo capta.
/// TRABAJANDO: Cuando una persona a la que esta siguiendo se para, se pone a trabajar. </summary>
public enum Estado {
    IDLE,
    SIGUIENDO,
    CONVIRTIENDOSE,
    TRABAJANDO
}

public class Follower : MonoBehaviour {
    
    /// <summary>La persona que debe seguir este Follower</summary>
    public Transform persona;

    /// <summary>El estado actual del follower</summary>
    [SerializeField]
    private Estado estado;

    /// <summary>Referencia al sprite renderer y al color para cambiarle el color cuando sigue a una persona.</summary>    
    private SpriteRenderer sR;

    /// <summary>Guardamos el color que vamos a aplicarle al sR en una variable.</summary>    
    private Color colorSprite;

    /// <summary>Devolvemos si la persona que seguimos esta parada o no.</summary>
    bool PersonaEstaParada(){
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
    int tiempoActualTrabajar = 0;

    /// <summary> El total que tiene que trabajar en este ciclo para conseguir el producto</summary>
    int tiempoTotalTrabajar;

    /// <summary> Los tres subestados posibles para el estado TRABAJANDO</summary>
    /// <summary> Buscando Trabajo: buscamos un punto cercano en el espacio</summary>
    /// <summary> Caminando al trabajo: Vamos (seteado por AIPath, acá no hacemos mucho)</summary>
    /// <summary> Trabajando: minando literalmente hasta que obtiene el producto.</summary>
    enum TRABAJANDO {
        buscandoTrabajo,
        caminandoAlTrabajo,
        trabajando,
    }
    /// <summary> Almacenamos el subestado actual en caso de que estemos en el estado TRABAJANDO</summary>
     [SerializeField]
     private TRABAJANDO subEstadoActualTrabajando;

    /// <summary> Contador para el tiempo que llevamos quietos en IDLE, se suma cada frame que esté quieto</summary>
    int tiempoActualRumiar = 0;
    /// <summary> Valor al que tenemos que llegar con el tiempoActualRumiar para cosniderar e ciclo terminado</summary>
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
    private int efectoPublicoEstatal, efectoPublicoMilitar, efectoPrivadoComercial, efectoPrivadoEntretenimiento;

    /// <summary> Si la distancia de un efecto a otro supera la del umbral, empieza a sumar puntos esa emocion. </summary>
    int umbralEfectoEmocion = 100;

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
    public enum EMOCION {
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

    void OnEnable () {
        sR = GetComponent<SpriteRenderer>();
        aiP = GetComponent<AIPath>();
        gds = GetComponent<GeroDestinationSetter>();
        an = GetComponent<Animator>();

        globalManager = GameObject.Find("GlobalManager");
        gV = globalManager.GetComponent<globalVariables>();
        
        estado = Estado.IDLE;
        subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
        subEstadoActualIdle = IDLE.buscandoLugar;

        velMin = Random.Range(0.33f, 2);
        velMax = Random.Range(5, 10);

        tiempoTotalRumiar = Random.Range(200,450);
        tiempoTotalTrabajar = Random.Range(200,450);

        emocionActual = EMOCION.NADA;
    }

    /// <summary>Todos los frames evaluamos que hacer dependiendo el estado y los eventos</summary>
    void Update () {
        DecidirSentimientos();
        DecidirQueHacer();
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
        if(efectoPublicoEstatal > efectoPrivadoComercial &&
            efectoPublicoEstatal > efectoPrivadoEntretenimiento &&
            efectoPublicoEstatal > efectoPublicoMilitar)
        {
            if(efectoPublicoEstatal - efectoPrivadoComercial > umbralEfectoEmocion &&
               efectoPublicoEstatal - efectoPrivadoEntretenimiento > umbralEfectoEmocion && 
               efectoPublicoEstatal - efectoPublicoMilitar > umbralEfectoEmocion )
            {
                if(emocionActual != EMOCION.AMORALLIDER)
                {
                    CambiarEmocion(EMOCION.AMORALLIDER, 0);
                }
                nivelEmocionActual++;
            }
        }

        // PUBLICO MILITAR
        if(efectoPublicoMilitar > efectoPrivadoComercial &&
            efectoPublicoMilitar > efectoPrivadoEntretenimiento &&
            efectoPublicoMilitar > efectoPublicoEstatal)
        {
            if(efectoPublicoMilitar - efectoPrivadoComercial > umbralEfectoEmocion &&
               efectoPublicoMilitar - efectoPrivadoEntretenimiento > umbralEfectoEmocion && 
               efectoPublicoMilitar - efectoPublicoEstatal > umbralEfectoEmocion )
            {
                if(emocionActual != EMOCION.HACERGUERRA)
                {
                    CambiarEmocion(EMOCION.HACERGUERRA, 0);
                }
                nivelEmocionActual++;
            }
        }

        // PRIVADO ENTRETENIMIENTO
        if(efectoPrivadoEntretenimiento > efectoPrivadoComercial &&
            efectoPrivadoEntretenimiento > efectoPublicoMilitar &&
            efectoPrivadoEntretenimiento > efectoPublicoEstatal)
        {
            if(efectoPrivadoEntretenimiento - efectoPrivadoComercial > umbralEfectoEmocion &&
               efectoPrivadoEntretenimiento - efectoPublicoMilitar > umbralEfectoEmocion && 
               efectoPrivadoEntretenimiento - efectoPublicoEstatal > umbralEfectoEmocion )
            {
                if(emocionActual != EMOCION.BOLUDEAR)
                {
                    CambiarEmocion(EMOCION.BOLUDEAR, 0);
                }
                nivelEmocionActual++;
            }
        }

        // PRIVADO COMERCIAL
        if(efectoPrivadoComercial > efectoPrivadoEntretenimiento &&
            efectoPrivadoComercial > efectoPublicoMilitar &&
            efectoPrivadoComercial > efectoPublicoEstatal)
        {
            if(efectoPrivadoComercial - efectoPublicoEstatal > umbralEfectoEmocion &&
               efectoPrivadoComercial - efectoPrivadoEntretenimiento > umbralEfectoEmocion && 
               efectoPrivadoComercial - efectoPublicoMilitar > umbralEfectoEmocion )
            {
                if(emocionActual != EMOCION.INDIVIDUALISTA)
                {
                    CambiarEmocion(EMOCION.INDIVIDUALISTA, 0);
                }
                nivelEmocionActual++;
            }
        }

    }

    
    void DecidirQueHacer() {
        if(estado == Estado.IDLE) {
            // Lo unico que lo puede sacar de este estado seria que lo toque un usuario
            // Esto podria ser desde un OnCollision aca o en el usuario
            DecidirSubEstadoIdle();
        }  else if(estado == Estado.TRABAJANDO) {
            // Obtenemos el estado del persona y si se movio switcheamos aca a siguiendo
            if(persona != null){
                if(!PersonaEstaParada()){
                    CambiarEstado(Estado.SIGUIENDO);
                }
            } 
            // Si no hay persona porque se fue volvemos a IDLE (opcionalmente podriamos matarlo)
            if(persona == null) {
                CambiarEstado(Estado.IDLE);
            }
            DecidirSubEstadoTrabajando();
        } else if(estado == Estado.SIGUIENDO) {
            // Obtenemos el estado del persona y si se paro switcheamos aca a trabajando
            if(persona != null){
                if(PersonaEstaParada() && GetComponent<AIPath>().reachedDestination){
                    CambiarEstado(Estado.TRABAJANDO);
                    an.SetTrigger("idle");
                }
            }
            // Si no hay persona porque se fue volvemos a IDLE (opcionalmente podriamos matarlo)
            if(persona == null) {
                CambiarEstado(Estado.IDLE);
            }
        }   else if(estado == Estado.CONVIRTIENDOSE) {

        } else {
            Debug.Log("ERROR: no hay estado seleccionado en follower.");
        }  
    }

    /// <summary>Cuando entramos en un Trigger debemos manejar los cambios. En principio las reglas debieran ser:
    /// Si es una persona (a traves del Seguido), lo manejamos desde aca porque el seguido no tiene OnTriggerEnter
    /// Si es con otro sujeto: de la misma persona, ignoramos. De otra persona, maneja quien tiene persona con mas seguidores.</summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "persona"){
            if(persona == null || GameObject.ReferenceEquals(persona, other.gameObject)){
                ManejarColisionesConPersona(other);
            }
        }
        if(other.gameObject.tag == "sujeto"){
            // Si somos huerfanos, no hacemos nada cuando nos encontramos con otro sujeto. Solo si nos encontramos con una person (arriba).
            // Pero no cuando nos encontramos con otro sujeto porque en caso de que el otro sea huerfano no pasa ninguna interaccion
            // y en caso de que el otro no lo sea se manejara en el otro (abajo)
            if(persona != null) {
                // Si el otro sujeto no tiene persona, es huerfano, manejamos aca
                if(other.gameObject.GetComponent<Follower>().persona == null){
                    ManejarColisionesConSujetoHuerfano(other);
                } else if(persona.GetComponent<Seguido>().GetNumSeguidores() > other.gameObject.GetComponent<Follower>().persona.GetComponent<Seguido>().GetNumSeguidores()){
                    // Si el otro sujeto tiene persona y tiene menos seguidores que nosotros, manejamos aca
                    ManejarColisionesConSujeto(other);
                }
                if(GameObject.Equals(persona,  other.gameObject.GetComponent<Follower>().persona))
                {
                    // Si las dos personas son la misma, estamos en presencia de un seguidor de nuestro propio grupo.
                    // Solo actuamos en caso de que haya que contagiar: si sentimos mas emocion que el umbral y si el otro no tiene nuestra emocion
                    // TODO: deberiamos agregar los niveles de emocion de rebelarse y huir
                    if(nivelEmocionActual > umbralContagioEmocion)
                    {
                        if(emocionActual != other.gameObject.GetComponent<Follower>().ObtenerEmocionActual())
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
    void ManejarColisionesConPersona(Collider2D other){
        other.gameObject.GetComponent<Seguido>().SolicitarEmpezarASeguir(gameObject, false);    
    }

    /// <summary>Cuando tocamos un trigger y es un sujeto huerfano y nosotros tenemos persona, lo captamos.</summary>
    void ManejarColisionesConSujetoHuerfano(Collider2D other){
        // Notar que no le pasamos este sujeto como parametro, sino el que colisiono con nosotros
        // Notar que se lo mandamos a esta persona
        persona.GetComponent<Seguido>().SolicitarEmpezarASeguir(other.gameObject, false);            
    }

    /// <summary>Cuando tocamos un trigger y es un sujeto cuya persona tiene menos seguidores que este</summary>
    void ManejarColisionesConSujeto(Collider2D other){
        // Notar que no le pasamos este sujeto como parametro, sino el que colisiono con nosotros
        // Notar que se lo mandamos a esta persona
        // Notar que le pasamos true porque tiene persona
        persona.GetComponent<Seguido>().SolicitarEmpezarASeguir(other.gameObject, true);                    
    }

    // Esta funcion se ejecuta desde la persona como devolucion a cuando le mandamos DejarDeSeguir();
    public void VaciarSeguido(GameObject exSeguido) {
        persona = null;
        CambiarEstado(Estado.CONVIRTIENDOSE);
        colorSprite = new Color(1,1,1,1);
        sR.color = colorSprite;
    }

    //Esta funcion la llamamos desde el seguido, nos la devuelve cuando le damos a EmpezarASeguir();
    public void ConfirmarNuevoSeguido(GameObject nuevoSeguido){
        persona = nuevoSeguido.transform;
        CambiarEstado(Estado.SIGUIENDO);
        colorSprite = nuevoSeguido.GetComponent<Seguido>().GetColorSprite();
        sR.color = colorSprite;
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
            posLugarDeTrabajo = Utilidades.PuntoRandom(gV.piso, persona.position, 25);
            gds.SetDestination(posLugarDeTrabajo);

            if(posLugarDeTrabajo != Vector3.zero && posLugarDeTrabajo != null){
                subEstadoActualTrabajando = TRABAJANDO.caminandoAlTrabajo;
                aiP.canSearch = true;
                an.SetTrigger("caminando");
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

            if (aiP.reachedDestination)
            {
                subEstadoActualTrabajando = TRABAJANDO.trabajando;
                an.SetTrigger("usandoMartillo");
                ActualizarVelAn(0);
                SetTiempoTrabajar();
            }

            // Si no ha llegado a detino el AIPath lo va a llevar, acá no debemos hacer nada

        }
        else if (subEstadoActualTrabajando == TRABAJANDO.trabajando)
        {
            // Aca que espere un rato
            if(tiempoActualTrabajar > tiempoTotalTrabajar){
                subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                an.SetTrigger("idle"); 
            } else {
                tiempoActualTrabajar ++;
            }
        }
        else
        {
            Debug.LogError("ERROR: no hay ningun subestado trabajando asignado");
        }
    
    }


    // IDLE
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    void DecidirSubEstadoIdle()
    {
        if(subEstadoActualIdle == IDLE.buscandoLugar)
        {
            float vel = SetVelocidadRandom();
            aiP.maxSpeed = vel;
            gds.SetDestination(Utilidades.PuntoRandom(gV.piso, transform.position, 25));
            subEstadoActualIdle = IDLE.caminando;
            an.SetTrigger("caminando");
            ActualizarVelAn(vel);

        } else if(subEstadoActualIdle == IDLE.caminando)
        {
            if (aiP.reachedDestination)
            {
                subEstadoActualIdle = IDLE.rumiando;
                ActualizarVelAn(0);
                SetTiempoRumiar();
            }
        } else if(subEstadoActualIdle == IDLE.rumiando)
        {
            //Chequear el contador random y si se cumple, vaciar el destino idle y pasarlo a buscando objetivo
            if (tiempoActualRumiar >= tiempoTotalRumiar)
            {
                subEstadoActualIdle = IDLE.buscandoLugar;
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
        if(vel == 0)
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
    void CambiarEstado(Estado nuevoEstado) {
        if(estado != nuevoEstado) {
            if(nuevoEstado == Estado.TRABAJANDO){
                subEstadoActualTrabajando = TRABAJANDO.buscandoTrabajo;
                aiP.canSearch = false;
                //gds.ClearDestination();
            }

            if(nuevoEstado == Estado.IDLE){
                subEstadoActualIdle = IDLE.buscandoLugar;
            }

            if(nuevoEstado == Estado.SIGUIENDO){
                an.SetTrigger("caminando");
                aiP.canSearch = true;
                gds.SetDestination(persona);
            }
            
            Debug.Log("Se efectuo un cambio de estado: de " + estado + " a " + nuevoEstado);
            estado = nuevoEstado;
        }
    }

    /// <summary> Para cambiar cualquier emocion no se debe asignar directamente, sino que debriamos pasar por aca </summary>
    /// <summary> por tema checkeos y hooks. El segundo parametro siempre debe ser 0 en un cambio normal, solo se usa </summary>
    /// <summary> en caso de que sea un cambio por contagio, en el que se pasa cuan emocionado está el contagiante - 10 </summary> 
    void CambiarEmocion(EMOCION nuevaEmocion, int nivelNuevaEmocion) {
        if(emocionActual != nuevaEmocion)
        {
                nivelEmocionActual = nivelNuevaEmocion;
                emocionActual = nuevaEmocion;
        } else {
            Debug.LogWarning("Atencion: llamando a cambiar emocion entre dos emociones iguales: Actual: " + emocionActual + " Nueva: " + nuevaEmocion);
        }
    }


    // Accedidas desde ONDAS
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    public void AfectarPorAntena(TIPOTOTEM totem) 
    {
        switch (totem)
        {
            case TIPOTOTEM.PUBLICOESTATAL:
                efectoPublicoEstatal++;
                break;
            case TIPOTOTEM.PUBLICOMILITAR:
                efectoPublicoMilitar++;
                break;
            case TIPOTOTEM.PRIVADOENTRETENIMIENTO:
                efectoPrivadoEntretenimiento++;
                break;
            case TIPOTOTEM.PRIVADOCOMERCIAL:
                efectoPrivadoComercial++;
                break;
            default:
                Debug.Log("ERROR: Se envio un 'AfectarAntena()' sin tipo de totem");
                break;
        }
    }
}

