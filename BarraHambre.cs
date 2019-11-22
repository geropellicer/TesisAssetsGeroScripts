using UnityEngine;

public class BarraHambre : MonoBehaviour {
    
    [SerializeField]
    Follower bicho;

    [SerializeField]
    float minAnchoBarrita = 0;

    [SerializeField]
    // TODO: llevar al valor real del tamanio de la bara llena y setar el anchor como corresponde
    float maxAnchoBarrita = 0.1f;

    float minHambrePosible = 0;
    float maxHambrePosible = 100;


    private void Start() {
        bicho = transform.parent.transform.parent.GetComponent<Follower>();
    }

    private void Update() {
        float newX = Utilidades.Map(bicho.ObtenerHambre(), minHambrePosible, maxHambrePosible, minAnchoBarrita, maxAnchoBarrita);
        transform.localScale = new Vector3(newX, transform.localScale.y, transform.localScale.z);
    }
}