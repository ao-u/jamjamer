using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.GraphicsBuffer;

public class Director : MonoBehaviour
{
    public static GameObject player, canvas, maincamera, layeredcamera;
    GameObject map, fleshtimerUI;

    public Material mattt;

    public static float gravity = 30f;
    public static void ApplyGravity(Rigidbody rb)
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.fixedDeltaTime, rb.velocity.z);
    }
    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        player = GameObject.Find("player");
        maincamera = gameObject;
        layeredcamera = GameObject.Find("LayeredCamera");
        fleshtimerUI = GameObject.Find("timerUI");
        mattt = Resources.Load<Material>("shader/t");
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mattt != null)
        {
            Graphics.Blit(source, destination, mattt);
        }
        else
        {
            Debug.Log("matt isnt there");
            Graphics.Blit(source, destination);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.J)) { Time.timeScale = 3f; } else { Time.timeScale = 1f; }
    }
    public static float globaltimer = 0f;
    public static float fleshtimer = 60f;
    private void FixedUpdate()
    {
        LogUpdate();

        globaltimer += Time.fixedDeltaTime;

        fleshtimer -= Time.fixedDeltaTime;

        fleshtimerUI.GetComponent<Slider>().value = fleshtimer / 80f;
        fleshtimerUI.transform.Find("fa").Find("Fill").GetComponent<Image>().color = 
            Color.Lerp(Color.white, new Color(0f, 255f, 200f), (fleshtimer - 80f) / 100f);
        

        Director.LogConst("Time : " + globaltimer.ToString("#.00"), "Time", Color.white);
    }
    private void OnDrawGizmos()
    {
      
    }
    public class Log 
    {
        public string name;
        public GameObject logitself;
        public float timer;
    }
    public static List<Log> logstemp = new List<Log>();
    public static void LogTemp(string message, Color color)
    {
        Log l = new Log();
        l.logitself = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), Vector3.zero, Quaternion.identity, canvas.transform);  
        l.logitself.GetComponent<TextMeshProUGUI>().text = message;
        l.logitself.GetComponent<TextMeshProUGUI>().color = color;
        l.logitself.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        l.logitself.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        l.timer = 1f;
        logstemp.Add(l);
    }
    public static List<Log> logsconst = new List<Log>();
    public static void LogConst(string message, string name, Color color)
    {
        foreach(Log ll in logsconst)
        {
            if (ll.name == name)
            {
                ll.timer = 1f;
                ll.logitself.GetComponent<TextMeshProUGUI>().text = message;
                return;
            }
        }
        Log l = new Log();
        l.logitself = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), Vector3.zero, Quaternion.identity, canvas.transform);
        l.logitself.GetComponent<TextMeshProUGUI>().text = message;
        l.logitself.GetComponent<TextMeshProUGUI>().color = color;
        l.logitself.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        l.logitself.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        l.timer = 1f;
        l.name = name;
        logsconst.Add(l);
    }
    private void LogUpdate()
    {
        for (int i = 0; i < logstemp.Count; i++)
        {
            RectTransform r = logstemp[i].logitself.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(150f, -40f * (logstemp.Count - i + 1));

            logstemp[i].timer -= Time.fixedDeltaTime;
            Color c = logstemp[i].logitself.GetComponent<TextMeshProUGUI>().color;
            logstemp[i].logitself.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, logstemp[i].timer);
            if (logstemp[i].timer < 0f || (i < logstemp.Count - 15))
            {
                Destroy(logstemp[i].logitself);
                logstemp.RemoveAt(i);
                i--;
            }

        }
        for (int i = 0; i < logsconst.Count; i++) 
        {
            RectTransform r = logsconst[i].logitself.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(-500f, -40f * (logsconst.Count - i + 1));

            logsconst[i].timer -= Time.fixedDeltaTime;

            Color c = logsconst[i].logitself.GetComponent<TextMeshProUGUI>().color;
            logsconst[i].logitself.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, logsconst[i].timer);
            if (logsconst[i].timer < 0f || (i < logsconst.Count - 15))
            {
                Destroy(logsconst[i].logitself);
                logsconst.RemoveAt(i);
                i--;
            }
           
        }

    }

    public static T GetRandomResourceByName<T>(string name, string path) where T : UnityEngine.Object
    {
        List<T> possresources = new List<T>();
        foreach (T r in Resources.LoadAll<T>(path))
        {
            if (r.name.StartsWith(name))
            {
                possresources.Add(r);
            }
        }

        if (possresources.Count > 0)
        {
            return possresources[Random.Range(0, possresources.Count)];
        }
        else
        {
            Director.LogTemp("resource not found | name | " + name + " | path | " + path, Color.red);
            return null;
        }
    }
    public static void PlaySound(string name, AudioSource source)
    {
        AudioClip c = GetRandomResourceByName<AudioClip>(name, "audio/");
        if (c != null)
        {
            source.clip = c;
            source.pitch = Random.Range(.7f, 1.3f);
            source.volume = .05f;
            source.Play();
        }
        else
        {
            Director.LogTemp("sound not found | " + name, Color.red);
        }
    }


    public static string C(string message, Color color)
    {
        return $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
    }
}
