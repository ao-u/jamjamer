using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public static Dictionary<int, float> besttimes = new Dictionary<int, float>
    {
        { 1, 9999f },
        { 2, 9999f },
        { 3, 9999f },
        { 4, 9999f },
        { 5, 9999f },
        { 6, 9999f },
        { 7, 9999f },
        { 8, 9999f },
        { 9, 9999f },
        { 10, 9999f },
        { 11, 9999f },
    };
    public static bool hasplayedonetime = false; 
    GameObject timesobject;
    void Start()
    {
        timesobject = GameObject.Find("times");
        Cursor.lockState = CursorLockMode.None;

        string s = "-BEST_TIMES-\n";
        for (int i = 0; i < besttimes.Count; i++)
        {
            if (besttimes[i + 1] >= 9990f)
            {
                s += "";
            }
            else
            {
                s += "Q" + (i + 1) + ": " + besttimes[i + 1].ToString("#.00") + "s\n";
            }
                
        }
        if (!hasplayedonetime)
        {
            s = "";
        }
        timesobject.GetComponent<TextMeshPro>().text = s;

        StartCoroutine(FadeIn());
    }
    public IEnumerator Play()
    {
        GameObject fader = GameObject.Find("transition");
        if (fader != null)
        {
            
            Image img = fader.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 0);
            while (img.color.a < 1f)
            {
                img.color = new Color(1, 1, 1, img.color.a + Time.fixedDeltaTime * 2f);
                yield return new WaitForFixedUpdate();
            }
        }
        SceneManager.LoadScene("game");
        Director.logsconst.Clear();
        Director.logstemp.Clear();
        Director.quota = 1;
        Director.quotaprogress = 0;
        Director.quotatier = 1;
        Director.fleshtimer = 80f;
        Director.dying = false;
        Director.globaltimer = 0f;
        Player.helditems.Clear();
        Player.scrapDASHCD = 1f;
        Player.scrapSPEED= 1f;
        Player.scrapSTRENGTH= 1f;
        MapGen.count = 0;
        MapGen.localroomcount = 0;
        MapGen.allrooms.Clear();

        
        //Cursor.lockState = CursorLockMode.Locked;
        //GameObject.Find("Main Camera").GetComponent<Director>().StartCoroutine(Director.FadeIn());

    }
    public static IEnumerator FadeIn()
    {
        GameObject fader = GameObject.Find("transition");
        if (fader != null)
        {
            Image img = fader.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 1);
            while (img.color.a > 0f)
            {
                img.color = new Color(1, 1, 1, img.color.a - Time.fixedDeltaTime * 2f);
                yield return new WaitForFixedUpdate();
            }
        }
    }
    public static void Quit()
    {
        Application.Quit();
    }


    void Update()
    {
        

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
