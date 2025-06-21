using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Misc : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (GameObject.Find("SIGN") != null)
        {
            GameObject g = GameObject.Find("SIGN");
            float rate = 2f;
            g.transform.localEulerAngles += new Vector3(0f, rate, 0f);
        }
    }
}
