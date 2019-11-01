using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public static class Utilidades
{
    public static float Map(float x, float in_min, float in_max, float out_min, float out_max, bool clamp = false)
    {
        if (clamp) x = Math.Max(in_min, Math.Min(x, in_max));
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    /// <summary>
    /// Devuelve un punto random en un plano garantizando que sea un nodo caminable del grafico de pathfinding
    /// </summary>
    /// <param name="piso"></param>
    /// <returns></returns>
    public static Vector3 PuntoRandom(GameObject piso)
    {
        int contador = 0;
        while (true)
        {
            float minX, maxX, minY, maxY;
            float newX;
            float newY;
            float sizeX, sizeY;
            sizeX = piso.GetComponent<MeshRenderer>().bounds.size.x;
            sizeY = piso.GetComponent<MeshRenderer>().bounds.size.y;
            minX = piso.transform.localPosition.x - (sizeX / 2);
            maxX = piso.transform.localPosition.x + (sizeX / 2);
            minY = piso.transform.localPosition.y - (sizeY / 2);
            maxY = piso.transform.localPosition.y + (sizeY / 2);
            newX = UnityEngine.Random.Range(minX, maxX);
            newY = UnityEngine.Random.Range(minY, maxY);

            //resoluciones
            Vector3 destinoIdle = new Vector3(newX, newY, -0.1f);

            GraphNode node = AstarPath.active.GetNearest(destinoIdle).node;
            if (node.Walkable)
            {
                return destinoIdle;
            }
            else
            {
                contador++;
                if(contador >= 50)
                {
                    Debug.LogError("No se encuentra un punto random caminable despues de " + contador + " intentos.");
                    return Vector3.zero;
                }
            }
        } 
    }

    public static Vector3 PuntoRandom(GameObject piso, float z)
    {
        int contador = 0;
        while (true)
        {
            float minX, maxX, minY, maxY;
            float newX;
            float newY;
            float sizeX, sizeY;
            sizeX = piso.GetComponent<MeshRenderer>().bounds.size.x;
            sizeY = piso.GetComponent<MeshRenderer>().bounds.size.y;
            minX = piso.transform.localPosition.x - (sizeX / 2);
            maxX = piso.transform.localPosition.x + (sizeX / 2);
            minY = piso.transform.localPosition.y - (sizeY / 2);
            maxY = piso.transform.localPosition.y + (sizeY / 2);
            newX = UnityEngine.Random.Range(minX, maxX);
            newY = UnityEngine.Random.Range(minY, maxY);

            //resoluciones
            Vector3 destinoIdle = new Vector3(newX, newY, z);

            GraphNode node = AstarPath.active.GetNearest(destinoIdle).node;
            if (node.Walkable)
            {
                return destinoIdle;
            }
            else
            {
                contador++;
                if (contador >= 50)
                {
                    Debug.LogError("No se encuentra un punto random caminable despues de " + contador + " intentos.");
                    return Vector3.zero;
                }
            }
        }
    }

    /// <summary>
    /// Genera un punto random en un planto provisto en el primer GameObject, garantizando que esté a menos distancia que la provista del
    /// punto de comparación provista. Garantizando que sea caminable en el patfhinding.
    /// </summary>
    /// <param name="piso"></param>
    /// <param name="posReferencia"></param>
    /// <param name="distanciaMax"></param>
    /// <returns></returns>
    public static Vector3 PuntoRandom(GameObject piso, Vector3 posReferencia, float distanciaMax)
    {
        while (true)
        {
            Vector3 puntoRandomGenerado = PuntoRandom(piso);
            if (Vector2.Distance(posReferencia, puntoRandomGenerado) < distanciaMax && Vector2.Distance(posReferencia, puntoRandomGenerado) > 8)
            {
                return puntoRandomGenerado;
            }
        }
    }

    /// <summary>
    /// Devuelve un vector3 con un punto dentro del Gameobject piso, que quede a menos distancia de la distanciaMax de todos los puntos
    /// de referencia provistos en el array, y que quede a mas de 20 unidades de distancia de cada punto de referencia
    /// </summary>
    /// <param name="piso"></param>
    /// <param name="posReferencia"></param>
    /// <param name="distanciaMax"></param>
    /// <returns></returns>
    public static Vector3 PuntoRandom(GameObject piso, Vector3[] posReferencia, float distanciaMax)
    {
        while (true)
        {
            Vector3 puntoRandomGenerado = PuntoRandom(piso);
            foreach (Vector3 pos in posReferencia)
            {
                if (Vector2.Distance(pos, puntoRandomGenerado) > distanciaMax || Vector2.Distance(pos, puntoRandomGenerado) > 20)
                {
                    break;
                }
                return puntoRandomGenerado;
            }
        }
    }

    public static void LookAt2D(Transform objeto, Transform target, float offset)
    {
        Vector3 targetPos;
        Vector3 thisPos;
        float angle;

    targetPos = target.position;
        thisPos = objeto.transform.position;
        targetPos.x = targetPos.x - thisPos.x;
        targetPos.y = targetPos.y - thisPos.y;
        angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        objeto.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + offset));
    }
}
