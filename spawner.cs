using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[ExecuteInEditMode]
public class spawner : MonoBehaviour
{
    public int cantidadASpawnear;
    public GameObject manager;
    globalVariables gV;
    public GameObject prefabComida;

    public GameObject prefabSoviet;
    public GameObject prefabFabrica;
    public GameObject prefabAlmacen;
    public GameObject prefabTocon;
    public GameObject prefabArbusto;

    public float velocidadDelTiempo;

    [SerializeField]
    int sovietActualConstruirFabrica = 1;
    [SerializeField]
    int sovietActualConstruirAlmacen;

    public enum tipo
    {
        arbusto,
        almacen,
        fabrica,
        soviet,
        sujeto,
        comida,
        tocon
    }



    // Start is called before the first frame update
    void Start()
    {
        gV = manager.GetComponent<globalVariables>();
        sovietActualConstruirFabrica = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnearBicho()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject.Instantiate(gV.prefabBicho, Utilidades.PuntoRandom(gV.piso), Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }
    public void SpawnearComidaRandom()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject ultimaComida = GameObject.Instantiate(prefabComida, Utilidades.PuntoRandom(gV.piso), Quaternion.identity);
                ultimaComida.transform.position = new Vector3(ultimaComida.transform.position.x, .33f, ultimaComida.transform.position.z);
            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }
    
    public GameObject SpawnearComida(Vector3 position)
    {
        return (GameObject)GameObject.Instantiate(prefabComida, position, Quaternion.identity);
    }
    

    public void SpawnearArbustos()
    {
        if (cantidadASpawnear > 0)
        {
            for (int i = 0; i < cantidadASpawnear; i++)
            {
                GameObject ultimoArbusto = GameObject.Instantiate(prefabArbusto, Utilidades.PuntoRandom(gV.piso, -1), Quaternion.Euler(new Vector3(0,0,Random.Range(-90,90))));
                //Actualizamos el graph
                var guo = new GraphUpdateObject(ultimoArbusto.GetComponent<Collider2D>().bounds);
                guo.updatePhysics = true;
                AstarPath.active.UpdateGraphs(guo);

            }
        }
        else
        {
            Debug.LogWarning("no hay ingresada una cantidad a spawnear mayor a 0");
        }
    }

    public void SetVelocidadDelTiempo()
    {
        Time.timeScale = velocidadDelTiempo;
    }


    public void ConstruirSoviet()
    {
        int intentos = 0;
        Vector3 pos = Utilidades.PuntoRandom(gV.piso);
        do
        {
            pos = Utilidades.PuntoRandom(gV.piso);
            intentos++;
            if(intentos > 50)
            {
                Debug.Log("Se supero la cantidad de intentos y nio hay lugar");
                break;
            }
        }

        while (!ChequearPosSoviet(pos, 30));

        GameObject ultimoTocon = GameObject.Instantiate(prefabTocon, new Vector3(pos.x, pos.y, 1), Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10))));
        ultimoTocon.GetComponent<toconConstruccion>().tipoTocon = toconConstruccion.tocon.soviet;
        gV.cantToconesSoviets++;
    }

    public void ConstruirFabrica()
    {
        GameObject sovietActual;
        if(gV.sovietComunidades.Count > sovietActualConstruirFabrica - 1)
        {
            if (gV.cantFabricas / gV.cantSoviets >= gV.relacionFabricaPoblacion)
            {
                sovietActualConstruirFabrica++;
            }
            if (gV.sovietComunidades.Count < sovietActualConstruirFabrica)
            {
                return;
            }
            else
            {
                sovietActual = gV.sovietComunidades[sovietActualConstruirFabrica - 1];
            }
            Vector3 posReferencia = sovietActual.transform.position;
            Vector3 posNueva = Utilidades.PuntoRandom(gV.piso, posReferencia, 35);

            int intentos = 0;
            while (!ChequearPosicion(posNueva, 2))
            {
                posNueva = Utilidades.PuntoRandom(gV.piso, posReferencia, 35);
                intentos++;
                if (intentos > 50)
                {
                    Debug.Log("Se supero la cantidad de intentos y nio hay lugar");
                    break;
                }
            }

            GameObject ultimoTocon = GameObject.Instantiate(prefabTocon, posNueva, Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10))));
            ultimoTocon.transform.position = new Vector3(ultimoTocon.transform.position.x, ultimoTocon.transform.position.y, 1);
            ultimoTocon.GetComponent<toconConstruccion>().tipoTocon = toconConstruccion.tocon.fabrica;
            gV.cantToconesFabricas++;
        }
    }

    public void ConstruirAlmacen()
    {
        if (gV.sovietComunidades.Count > sovietActualConstruirAlmacen)
        {
            if (gV.cantAlmacenes / gV.cantSoviets >= gV.relacionAlmacenPoblacion)
            {
                sovietActualConstruirAlmacen++;
            }
            GameObject sovietActual = gV.sovietComunidades[sovietActualConstruirAlmacen];
            Vector3 posReferencia = sovietActual.transform.position;
            Vector3 posNueva = Utilidades.PuntoRandom(gV.piso, posReferencia, 45);

            int intentos = 0;
            while (!ChequearPosicion(posNueva, 3))
            {
                posNueva = Utilidades.PuntoRandom(gV.piso, posReferencia, 45);
                intentos++;
                if (intentos > 50)
                {
                    Debug.Log("Se supero la cantidad de intentos y nio hay lugar");
                    break;
                }
            }

            GameObject ultimoTocon = GameObject.Instantiate(prefabTocon, posNueva, Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10))));
            ultimoTocon.transform.position = new Vector3(ultimoTocon.transform.position.x, ultimoTocon.transform.position.y, 1);
            ultimoTocon.GetComponent<toconConstruccion>().tipoTocon = toconConstruccion.tocon.almacen;
            gV.cantToconesAlmacenes++;
        }                           
    }

    bool ChequearPosicion(Vector3 pos, float dist)
    {
        
        //Collider[] obstaculos = Physics.OverlapBox(pos, new Vector3(dist / 2, dist / 2, dist / 2));
        List<Collider2D> cols = new List<Collider2D>();
        Physics2D.OverlapCircle(new Vector2(pos.x, pos.y), dist/2, new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D c in cols)
        {
            if (c.tag != "piso" /*&& !c.gameObject.Equals(gameObject)*/ && c.tag != "sujeto")
            {
                return false;
            }
        }
        return true;
    }

    bool ChequearPosSoviet(Vector3 pos, float dist)
    {
         //Collider[] obstaculos = Physics.OverlapBox(pos, new Vector3(dist / 2, dist / 2, dist / 2));
         List<Collider2D> cols = new List<Collider2D>();
         Physics2D.OverlapCircle(new Vector2(pos.x, pos.y), dist/2, new ContactFilter2D().NoFilter(), cols);
         int i = 0;
         foreach (Collider2D c in cols)
         {
             if (c.tag != "piso" && c.tag != "arbustoDeComida" && c.tag != "sujeto" && !c.gameObject.Equals(gameObject))
             {
                 return false;
             }
             if (c.tag == "arbustoDeComida")
             {
                 i++;
             }
         }
         if (i > 0)
         {
             return true;
         }
         else
         {
             return false;
         }
    }


    public void BorrarDelTipo(tipo tipoABorrar)
    {
        switch (tipoABorrar)
        {
            case tipo.almacen:
                foreach(GameObject a in GameObject.FindGameObjectsWithTag("almacenDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.arbusto:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("arbustoDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.comida:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("comida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.fabrica:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("fabricaDeComida"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.soviet:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("sovietComunidad"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.sujeto:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("sujeto"))
                {
                    DestroyImmediate(a);
                }
                break;
            case tipo.tocon:
                foreach (GameObject a in GameObject.FindGameObjectsWithTag("tocon"))
                {
                    DestroyImmediate(a);
                }
                break;
            default:
                break;
        }
    }

}
