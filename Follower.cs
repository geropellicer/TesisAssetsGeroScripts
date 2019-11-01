using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Sets the destination of an AI to the position of a specified object.
/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
/// This component will then make the AI move towards the <see cref="target"/> set on this component.
///
/// See: <see cref="Pathfinding.IAstarAI.destination"/>
///
/// [Open online documentation to see images]
/// </summary>
public enum Estado {
    IDLE,
    SIGUIENDO,
    CONVIRTIENDOSE,
    TRABAJANDO
}
[UniqueComponent(tag = "ai.destination")]
public class Follower : MonoBehaviour {
    /// <summary>The object that the AI should move to</summary>
    public Transform target;
    IAstarAI ai;
    private Estado estado;

    void OnEnable () {
        ai = GetComponent<IAstarAI>();
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.
        if (ai != null) ai.onSearchPath += Update;
    }

    void OnDisable () {
        if (ai != null) ai.onSearchPath -= Update;
    }

    /// <summary>Updates the AI's destination every frame</summary>
    void Update () {
        DecidirQueHacer();
    }

    void DecidirQueHacer() {
        if(estado == Estado.IDLE) {
            // Lo unico que lo puede sacar de este estado seria que lo toque un usuario
            // Esto podria ser desde un OnCollision aca o en el usuario
        }  else if(estado == Estado.TRABAJANDO) {
            // Obtenemos el estado del target y si se movio switcheamos aca a siguiendo
            if(target != null){
                if(!TargetEstaParado()){
                    CambiarEstado(Estado.SIGUIENDO);
                }
            } 
            // Si no hay target porque se fue volvemos a IDLE (opcionalmente podriamos matarlo)
            if(target == null) {
                CambiarEstado(Estado.IDLE);
            }
        } else if(estado == Estado.SIGUIENDO) {
            if (target != null && ai != null) ai.destination = target.position;
            // Obtenemos el estado del target y si se paro switcheamos aca a trabajando
            if(target != null){
                if(TargetEstaParado()){
                    CambiarEstado(Estado.TRABAJANDO);
                }
            }
            // Si no hay target porque se fue volvemos a IDLE (opcionalmente podriamos matarlo)
            if(target == null) {
                CambiarEstado(Estado.IDLE);
            }
        }   else if(estado == Estado.CONVIRTIENDOSE) {

        } else {
            Debug.Log("ERROR: no hay estado seleccionado en follower.");
        }  
    }

    void CambiarEstado(Estado nuevoEstado) {
        if(estado != nuevoEstado) {
            estado = nuevoEstado;
        }
    }

    bool TargetEstaParado(){
        return target.GetComponent<Seguido>().EstaParado();
    }

    //TODO: para que esto funcione correctamente el collider de las personas deberia aumentar dependiendo la cantidad de seguidores
    private void OnCollisionEnter2D(Collision2D other) {
        if (estado == Estado.IDLE) {
            if(other.gameObject.tag == "persona") {
            }
        } else if (estado == Estado.SIGUIENDO) {
            // Si mi target tiene menos seguidores que el extranjero, hacemos el switch
            if(other.gameObject.GetComponent<Seguido>().GetNumSeguidores() < target.GetComponent<Seguido>().GetNumSeguidores()) {
                target.gameObject.GetComponent<Seguido>().DejarDeSeguir(gameObject);
                other.gameObject.GetComponent<Seguido>().EmpezarASeguir(gameObject);
            }
        }        
    }

    // Esta funcion se ejecuta desde la persona como devolucion a cuando le mandamos DejarDeSeguir();
    public void VaciarSeguido(GameObject exSeguido) {
        target = null;
        CambiarEstado(Estado.CONVIRTIENDOSE);
    }

    //Esta funcion la llamamos desde el seguido, nos la devuelve cuando le damos a EmpezarASeguir();
    public void ConfirmarNuevoSeguido(GameObject nuevoSeguido){
        target = nuevoSeguido.transform;
        CambiarEstado(Estado.SIGUIENDO);
    }
}

