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
    /// <summary> Velocidad de desplazamiento mínima para cuando camina
    private float velMin;

    [SerializeField]
    /// <summary> Velocidad de desplazamiento mínima para cuando camina
    private float velMax;

    GameObject globalManager;   
    private globalVariables gV; 
    private AIPath aiP;
    private Animator an;
    private GeroDestinationSetter gds;

    Vector3 posLugarDeTrabajo;
    int tiempoActualTrabajar = 0;
    int tiempoTotalTrabajar;
    enum TRABAJANDO {
        buscandoTrabajo,
        caminandoAlTrabajo,
        trabajando,
    }
     [SerializeField]
     private TRABAJANDO subEstadoActualTrabajando;

    int tiempoActualRumiar = 0;
    int tiempoTotalRumiar;
    enum IDLE
    {
        buscandoLugar,
        caminando,
        rumiando
    }
    [SerializeField]
    private IDLE subEstadoActualIdle;


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
    }

    /// <summary>Todos los frames evaluamos que hacer dependiendo el estado y los eventos</summary>
    void Update () {
        DecidirQueHacer();
    }

    /// <summary>Todos los frames evaluamos que hacer dependiendo el estado y los eventos</summary>
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
                    ManejarColisionesConSujeto(other);
                }
            }
        }
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


    // Trabajando
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
}

