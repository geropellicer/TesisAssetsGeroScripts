using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2DOverlap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Collider2D> cols = new List<Collider2D>();
        Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), 15, new ContactFilter2D().NoFilter(), cols);
        if(cols.Count > 0)
        {
            Debug.Log("Tocando algo");
        }
        else{
            Debug.Log("Tocando nada");  
        }
    }
}
