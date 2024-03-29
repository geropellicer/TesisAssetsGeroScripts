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

    /// <summary> Listado privado de comidas que va expropiando. De afuera se accede por los metodos de abajo </summary>
    [SerializeField]
    public List<GameObject> comidas;

    /// <summary> Devuelve una comida no nula al azar </summary>
    public GameObject ObtenerComidaDisponible()
    {
        int random = Random.Range(0,comidas.Count - 1);
        if(comidas[random] == null){
            Debug.Log("Atencion: se iba a devolver una comida NULA del array de comidas de la persona " + gameObject.name);
            Debug.Log("Index de la comida nula: " + random);
        } else {
            // Si no es nulla es una buena comida. La marcamos como preseleccionada y la sacamos de la lista.
            if(comidas[random].GetComponent<ComidaNueva>().preseleccionada == false)
            {
                comidas[random].GetComponent<ComidaNueva>().preseleccionada = true;
            } else {
                // Si la comida ya estaba preseleccionada devolvemos null y deberiamos intentar de nuevo desde el bicho.
                return null;
            }
        }
        return comidas[random];
    }

    public void AgregarComidasNuevoSeguidor(List<GameObject> comidasNuevas)
    {
        comidas.AddRange(comidasNuevas);
    }

    public void AgregarComidaDeSeguidor(GameObject comidaNueva)
    {
        comidas.Add(comidaNueva);
    }

    [SerializeField]
    GameObject prefabManchaPiso;
    float nextActionTime;
    float intervalo;
    float nextActionTimeComer;
    float intervaloComer;


    /// <summary> Almacenamos la ultima medicionn de si alrededor nuestro, en un rango determinado en el 
    /// detecroSujtosCercanos hay sujetos de otra persona o no.</summary>
    [SerializeField]
    bool haySujetosAjenosEnRango;
    /// <summary> Esto lo leemos desde los sujetos que siguen a esta persona. </summary>
    public bool ObtenerHaySujetosAjenosEnRango()
    {
        return haySujetosAjenosEnRango;
    }
    /// <summary> Esto lo seteamos desde e detectorSujetosCercanos que invocamos. </summary>
    public void SetHaySujetosAjenosEnRango(bool hay)
    {
        haySujetosAjenosEnRango = hay;
    }
    [SerializeField]
    GameObject detector;

    public detectorSujetosCercanos Detector()
    {
        return detector.GetComponent<detectorSujetosCercanos>();
    }

    float hambre;


    void Awake()
    {
        seguidores = new List<GameObject>();
        sR = GetComponent<SpriteRenderer>();
        colorSprite = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), 1f);
        sR.color = colorSprite;
        //For good measure, set the previous locations
        for(int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }

        comidas = new List<GameObject>();

        nextActionTime = Time.time + Random.Range(0f,1f);
        intervalo = Random.Range(1f, 2f);

        nextActionTimeComer = Time.time + Random.Range(2f,4f);
        intervaloComer= Random.Range(6, 12);

        detector = transform.GetChild(0).gameObject;
    }

    private void Update() {
        DecidirSiEstaParado();

        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + intervalo;
            GameObject zona = Instantiate(prefabManchaPiso, transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0,360))));
            zona.transform.localScale = new Vector3(12.5f,12.5f,12.5f);
            zona.GetComponent<SpriteRenderer>().color = colorSprite;
        }
        if (Time.time > nextActionTimeComer)
        {
            nextActionTimeComer = Time.time + intervaloComer;
            if(comidas.Count > 0)
            {
                GameObject comidaComer = comidas[0];
                //Destroy(comidaComer);
                // En vez de destruirla hacemos que se coma 
                comidaComer.GetComponent<ComidaNueva>().ComerPorPersona(gameObject);
                comidas.Remove(comidaComer);
            }

            // Instanciamos un objeto invisible con un collider, lo configuramos para que responda a este objeto
            // y que nos devuelva al morir si collisiono con algun sujeto cuya persona no somos nosotros o no
            detector.SetActive(true);
        }

        hambre++;
    }

    /// <summary> Cuando un follower toca a otro de una persona distinta, si su persona es mayor, solicita empezar a seguirlo.
    /// Aca se aplica la dif entre seguidores de personas: a mayor dif, mayor posibilidad de aceptar el seguidor, a menor dif mayor probabilidad
    /// de rechazarlo. </summary>
    public void SolicitarEmpezarASeguir(GameObject posibleNuevoSeguidor, bool tienePersona) {
        if(tienePersona){
            int peso1 = numSeguidores;
            int peso2 = posibleNuevoSeguidor.GetComponent<Follower>().persona.GetComponent<Seguido>().GetNumSeguidores();
            if(Utilidades.RandomWeightedBool(peso1, peso2)){
                AceptarEmpezarASeguir(posibleNuevoSeguidor);
            }
        } else {
            // Si es huerfano aceptamos de una
            AceptarEmpezarASeguir(posibleNuevoSeguidor);
        }
    }

    /// <summary> En caso de que la solicitud resulte exitosa pasamos por aca para agregarlo a la persona y para pegarle al bicho y agregar
    /// una referencia a la persona. </summary>
    public void AceptarEmpezarASeguir(GameObject nuevoSeguidor) {
        nuevoSeguidor.GetComponent<Follower>().VaciarSeguido(gameObject);
        seguidores.Add(nuevoSeguidor);
        numSeguidores = seguidores.Count;
        nuevoSeguidor.GetComponent<Follower>().ConfirmarNuevoSeguido(gameObject);
        ActualizarIndicesSeguidores();
    }

    /// <summary> Cuando otra persona nos roba un seguidor pasamos por aca (llamado desde el bicho)</summary>
    public void DejarDeSeguir(GameObject seguidorPerdido) {
        if(seguidores.Contains(seguidorPerdido))
        {
            seguidores.Remove(seguidorPerdido);
            numSeguidores = seguidores.Count;
            seguidorPerdido.GetComponent<Follower>().VaciarSeguido(gameObject);
            ActualizarIndicesSeguidores();
        } else
        {
            Debug.LogWarning("ERROR: en el dejar de seguir de " + seguidorPerdido.name + " se intento desacoplar de la persona " + name + " pero no estaba en su lista de seguidores");
        }
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

    /// <summary> Cuando un bicho se muere de hambre, pasamos por acá, llamado desde el bicho que nos seguía y está muriendo. </summary>
    public void AvisarMuerteSeguidor(GameObject seguidorMuerto)
    {
        if(seguidores.Contains(seguidorMuerto))
        {
            seguidores.Remove(seguidorMuerto);
            numSeguidores--;
            ActualizarIndicesSeguidores();
        } else
        {
            Debug.LogWarning("ERROR: en la muerte de " + seguidorMuerto.name + " se intento desacoplar de la persona " + name + " pero no estaba en su lista de seguidores");
        }
    }

    void ActualizarIndicesSeguidores()
    {
        int i = 0;
        foreach (GameObject seguidor in seguidores)
        {
            if(seguidor.GetComponent<Follower>().ObtenerEmocionActual() == Follower.EMOCION.HACERGUERRA)
            {
                seguidor.GetComponent<Follower>().ActualizarIndexEnSeguido(i);
                i++;
            }
        }
    }

    public int ContarFollowersNacionalistas()
    {
        List<GameObject> followerNacionalistas = new List<GameObject>();
        foreach (GameObject follower in seguidores)
        {
            if(follower.GetComponent<Follower>().ObtenerEmocionActual() == Follower.EMOCION.AMORALLIDER)
            {
                followerNacionalistas.Add(follower);
            }
        }
        return followerNacionalistas.Count;
    }

    public float ObtenerHambre()
    {
        return hambre;
    }


    public void Alimentarse()
    {
        hambre = 0;
    }
    
}