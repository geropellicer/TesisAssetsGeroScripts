using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    bool interfazEsquinas;
    [SerializeField]
    GameObject interfazEsquinas1 = null;
    [SerializeField]
    GameObject interfazEsquinas2 = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GetComponent<Canvas>().targetDisplay == 0)
            {
                GetComponent<Canvas>().targetDisplay = 1;
            }
            else if (GetComponent<Canvas>().targetDisplay == 1)
            {
                GetComponent<Canvas>().targetDisplay = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            interfazEsquinas1.SetActive(interfazEsquinas);
            interfazEsquinas2.SetActive(interfazEsquinas);
            interfazEsquinas = !interfazEsquinas;
        }
    }
}
