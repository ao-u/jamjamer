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
    public static void Play()
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
    public static void Quit()
    {

    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;

        float leftclick = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) ? 1f : -1f;
        RaycastHit rayhit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);
        if (Physics.Raycast(ray, out rayhit, 999f, 1))
        {
            if (rayhit.transform.name.Contains("item")
             || rayhit.transform.name.Contains("hitbox"))
            {

                Interactable i = rayhit.transform.GetComponentInParent<Interactable>();
                if (leftclick > 0f)
                {
                    i.OnInteract();
                }
                else
                {
                    i.Hovering();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        float rate = .4f;
        transform.eulerAngles += new Vector3(0f, rate, 0f);
    }

}
