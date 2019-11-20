using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prePersona : MonoBehaviour
{
    //Si esta quieto, no est asiendo trackeado
    /// <Summary> El tiempo que esperamos a ver si lo enganchamos con alguien o instanciamos una persona nueva</summary>
    [SerializeField]
    int preVidaTotal;

    /// <Summary> El tiempo de vida transcurrido </summary>
    int preVidaActual;

    OSC o;
    int kin;
    int id;

    globalVariables gV;

    [SerializeField]
    GameObject prefabPersona;

    private void Start()
    {
        gV = GameObject.Find("GlobalManger").GetComponent<globalVariables>();
    }

    private void Update()
    {
        preVidaActual++;
        if (preVidaActual >= preVidaTotal)
        {
            GameObject persona = Instantiate(prefabPersona, transform.position, Quaternion.identity);
            persona.GetComponent<persona>().Configurar(o, id, kin);
            Destroy(gameObject);
        }
    }

    public void Configurar(OSC _o, int _kin, int _id)
    {
        o = _o;
        id = _id;
        kin = _kin;

        if (kin == 1)
        {
            o.SetAddressHandler("/KIN1/Updated/", RecibirDatosPersonaKin1);
        }
        if (kin == 2)
        {
            o.SetAddressHandler("/KIN2/Updated/", RecibirDatosPersonaKin2);
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "persona")
        {
            if (!other.GetComponent<persona>().EstaFuncionando())
            {
                other.GetComponent<persona>().PersonaReencontrada(o, id, kin);
                Destroy(gameObject);
            }
        }
    }

    public void RecibirDatosPersonaKin1(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            float x = m.GetFloat(1);
            float y = m.GetFloat(2);
            transform.localPosition = new Vector3(Utilidades.Map(x, 0, 1, gV.minX, gV.maxX), Utilidades.Map(y, 0, 1, gV.minY1, gV.maxY1), -0.1f);
        }
    }


    public void RecibirDatosPersonaKin2(OscMessage m)
    {
        if (m.GetFloat(0) == id)
        {
            float x = m.GetFloat(1);
            float y = m.GetFloat(2);
            transform.localPosition = new Vector3(Utilidades.Map(x, 0, 1, gV.minX, gV.maxX), Utilidades.Map(y, 0, 1, gV.minY2, gV.maxY2), -0.1f);
        }
    }
}