using UnityEngine;

public class BarraHambrePersona : MonoBehaviour {
    
    [SerializeField]
    Seguido persona;

    [SerializeField]
    float minAnchoBarrita = 0;

    [SerializeField]
    // TODO: llevar al valor real del tamanio de la bara llena y setar el anchor como corresponde
    float maxAnchoBarrita = 0.1f;

    float minHambrePosible = 0;
    float maxHambrePosible = 100;


    private void Start() {
        persona = transform.parent.transform.parent.GetComponent<Seguido>();
    }

    private void Update() {
        float newX = Utilidades.Map(persona.ObtenerHambre(), minHambrePosible, maxHambrePosible, minAnchoBarrita, maxAnchoBarrita);
        transform.localScale = new Vector3(newX, transform.localScale.y, transform.localScale.z);
    }
}