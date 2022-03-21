using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray mouseWorldPos = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(mouseWorldPos, out hit))
        {
            transform.position = hit.point;
        }
    }
}
