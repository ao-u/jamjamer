using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            SceneManager.LoadScene("game");
            Director.logsconst.Clear();
            Director.logstemp.Clear();
            Director.quota = 1;
            Director.quotaprogress = 0;
            Director.quotatier = 1;
            Director.fleshtimer = 60f;
            MapGen.count = 0;
            MapGen.localroomcount = 0;
            MapGen.allrooms.Clear();
        }
    }
}
