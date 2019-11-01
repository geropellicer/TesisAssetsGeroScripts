using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seguido : MonoBehaviour {

    /// <summary>Umbral para determinar si se esta parado o moviendose</summary>
    private float noMovementThreshold = 0.001f;

    /// <summary>Cuantos frames tiene que estar dentro del umbral para que se considere quieto</summary>
    private const int noMovementFrames = 3;

    /// <summary>Buffer de puntos en los que estuvo</summary>
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    
    /// <summary>Alamacenamos si está quieto o no</summary>
    private bool  parado = true;
    
    /// <summary>Devolvemos a agentes externos si está quieto o no</summary>
    public bool EstaParado() {
        return parado;
    }

    /// <summary>La cantidad de seguidores sacado del seguidores.length</summary>
    [SerializeField]
    private int numSeguidores = 0;

    /// <summary>Lista de los sujetos que lo siguen en este momento.</summary>
    private List<GameObject> seguidores;

    /// <summary>Devolvemos a externos la lista de los sujetos que lo siguen en este momento.</summary>    
    public int GetNumSeguidores() {
        return numSeguidores;
    }

    /// <summary>Referencia al sprite renderer y al color para cambiarle el color cuando lo spwneamos.
    /// Este color debemos luego trasmitirselo a sus seguidores.</summary>    
    private SpriteRenderer sR;

    /// <summary>Referencia al sprite renderer y al color para cambiarle el color cuando lo spwneamos.   
    /// Este color debemos luego trasmitirselo a sus seguidores.</summary>  
    private Color colorSprite;

    /// <summary>Devolvemos el color para pasarle a agentes externos, sobre todo los seguidores.</summary>  
    public Color GetColorSprite(){
        return colorSprite;
    }


    void Awake()
    {
        seguidores = new List<GameObject>();
        sR = GetComponent<SpriteRenderer>();
        colorSprite = new Color(Mathf.Random(0,1), Mathf.Random(0,1), Mathf.Random(0,1), 1);
        sR.color = colorSprite;
        //For good measure, set the previous locations
        for(int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    private void Update() {
        DecidirSiEstaParado();
    }

    /// <summary> Cuando un follower toca a otro de una persona distinta, si su persona es mayor, solicita empezar a seguirlo.
    /// Aca se aplica la dif entre seguidores de personas: a mayor dif, mayor posibilidad de aceptar el seguidor, a menor dif mayor probabilidad
    /// de rechazarlo. </summary>
    public void SolicitarEmpezarASeguir(GameObject posibleNuevoSeguidor, bool tienePersona) {
        if(tienePersona){
            int peso1 = numSeguidores;
            int peso2 = posibleNuevoSeguidor.GetComponent<Follower>().persona.GetComponent<Seguido>.GetNumSeguidores;
            bool[] posibilidades = new bool[true, false];
            int[] pesosPosibilidades = new int[peso1, peso2]; 
            if(GetRandomWeightedIndex(pesosPosibilidades)){
                AceptarEmpezarASeguir(posibleNuevoSeguidor);
            }
        } else {
            // Si es huerfano aceptamos de una
            AceptarEmpezarASeguir();
        }
    }

    /// <summary> En caso de que la solicitud resulte exitosa pasamos por aca para agregarlo a la persona y para pegarle al bicho y agregar
    /// una referencia a la persona. </summary>
    public void AceptarEmpezarASeguir(GameObject nuevoSeguidor) {
        seguidorPerdido.GetComponent<Follower>().VaciarSeguido(gameObject);
        seguidores.Add(nuevoSeguidor);
        numSeguidores = seguidores.Count;
        nuevoSeguidor.GetComponent<Follower>().ConfirmarNuevoSeguido(gameObject);
    }

    /// <summary> Cuando otra persona nos roba un seguidor pasamos por aca (llamado desde el bicho)</summary>
    public void DejarDeSeguir(GameObject seguidorPerdido) {
        seguidores.Remove(seguidorPerdido);
        numSeguidores = seguidores.Count;
        seguidorPerdido.GetComponent<Follower>().VaciarSeguido(gameObject);
    }


    /// <summary> Para decidir el estado de todos los seguidores (trabajando o siguiento) debemos conocer si la persona esta parada.
    /// Aca lo que hacemos es chequear si en los ultimos tres frames estuvo dentro de un mimsmo threshold de distancias. </summary>
    void DecidirSiEstaParado(){
        for(int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i+1];
        }
        previousLocations[previousLocations.Length - 1] = transform.position;
        
        //Check the distances between the points in your previous locations
        //If for the past several updates, there are no movements smaller than the threshold,
        //you can most likely assume that the object is not moving
        for(int i = 0; i < previousLocations.Length - 1; i++)
        {
            if(Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                //The minimum movement has been detected between frames
                parado = false;
                break;
            }
            else
            {
                parado = true;
            }
        }
    }


    /// <summary> Devuelve el indice al azar de uno de los pesos insertados en el array</summary>
    public int GetRandomWeightedIndex(int[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0f;
        for (int i = 0; i < weights; ++i)
        {
            weightSum += weights[i];
        }
    
        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = elementCount - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }
    
            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }
    
        // No other item was selected, so return very last index.
        return index;
    }
}