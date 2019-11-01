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
[UniqueComponent(tag = "ai.destination")]
public class Follower : MonoBehaviour {
    
    /// <summary>La persona que debe seguir este Follower</summary>
    public Transform persona;

    /// <summary>Referencia al AI the A*</summary>
    IAstarAI ai;

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




    void OnEnable () {
        ai = GetComponent<IAstarAI>();
        sR = GetComponent<SpriteRenderer>();
        estado = Estado.IDLE;
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.
        if (ai != null) ai.onSearchPath += Update;
    }

    void OnDisable () {
        if (ai != null) ai.onSearchPath -= Update;
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
        } else if(estado == Estado.SIGUIENDO) {
            if (persona != null && ai != null) ai.destination = persona.position;
            // Obtenemos el estado del persona y si se paro switcheamos aca a trabajando
            if(persona != null){
                if(PersonaEstaParada()){
                    CambiarEstado(Estado.TRABAJANDO);
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
                    ManejarColisionesConSujetoHuerfano();
                } else if(persona.GetComponent<Seguido>().GetNumSeguidores > other.gameObject.GetComponent<Follower>().persona.GetNumSeguidores){
                    ManejarColisionesConSujeto();
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
        colorSprite = nuevoSeguido.GetComponent<Seguido>.colorSprite;
        sR.color = colorSprite;
    }
}

