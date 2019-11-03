using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutodestruirMancha : MonoBehaviour
{
    float nextActionTime;
    float intervalo;

    // Start is called before the first frame update
    void Start()
    {
        nextActionTime = Time.time + 6.5f;
        intervalo = 6.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            //Actualizar cada x segundos los almaceces y los soviets que existen
            nextActionTime = Time.time + intervalo;
            Destroy(gameObject);
        }
    }
}
