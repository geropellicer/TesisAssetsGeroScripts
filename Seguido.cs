using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seguido : MonoBehaviour {

     private float noMovementThreshold = 0.001f;
    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    
    private bool  parado = true;
    public bool EstaParado() {
        return parado;
    }
    
    private int numSeguidores = 0;
    private List<GameObject> seguidores;


    void Awake()
    {
        //For good measure, set the previous locations
        for(int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }
    }

    public int GetNumSeguidores() {
        return numSeguidores;
    }

    private void Update() {
        DecidirSiEstaParado();
    }

    public void EmpezarASeguir(GameObject nuevoSeguidor) {
        seguidores.Add(nuevoSeguidor);
        numSeguidores = seguidores.Count;
        nuevoSeguidor.GetComponent<Follower>().ConfirmarNuevoSeguido(gameObject);
    }

    public void DejarDeSeguir(GameObject seguidorPerdido) {
        seguidores.Remove(seguidorPerdido);
        numSeguidores = seguidores.Count;
        seguidorPerdido.GetComponent<Follower>().VaciarSeguido(gameObject);
    }

    void DecidirSiEstaParado(){
        for(int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i+1];
        }
        previousLocations[previousLocations.Length - 1] = objectTransfom.position;
        
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
}