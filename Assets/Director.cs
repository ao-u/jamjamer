using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.GraphicsBuffer;

public class Director : MonoBehaviour
{
    public static GameObject player, canvas, maincamera, layeredcamera;
    static GameObject fleshtimerUI;
    public Material mattt;
    public static AudioSource aud;
    public static float gravity = 30f;

    public static bool showdebugstuff = false;
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
        aud = player.GetComponent<AudioSource>();
        quotatier = 1;
        CalculateQuota();

        StartCoroutine(FadeIn());
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
    public static IEnumerator DeathCo()
    {
        
        for (int j = 0; j < 15; j++)
        {
            string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789###@@@@$$$$###@@@@###";
            string r = "";
            for (int i = 0; i < 10; i++)
            {
                char c = s[Random.Range(0, s.Length)];
                r += c + "-";
            }

            Director.LogTemp("SYSTEM_FAILURE: " + r, Color.white, 1f);



            Color cc = j % 2 == 0 ? Color.white : Color.black;
            fleshtimerUI.transform.Find("Background").GetComponent<Image>().color = cc;

            fleshtimer = j % 2 == 0 ? Random.Range(4f, 24f) : Random.Range(40f, 70f);
            fleshtimerUI.transform.Find("fa").Find("Fill").GetComponent<Image>().color = cc;


            yield return new WaitForSeconds(.2f);



        }
        GameObject fader = GameObject.Find("transition");
        if (fader != null)
        {
            Image img = fader.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 0);
            while (img.color.a < 1f)
            {
                img.color = new Color(1, 1, 1, img.color.a + Time.fixedDeltaTime * 1f);

                
                yield return new WaitForFixedUpdate();
            }
        }
        SceneManager.LoadScene("menu");
    }
    void Update()
    {
        //if (Input.GetKey(KeyCode.J)) { Time.timeScale = 3f; } else { Time.timeScale = 1f; }
        if (Input.GetKeyDown(KeyCode.U)) { showdebugstuff = !showdebugstuff; }
    }
    public static float globaltimer = 0f;
    public static float fleshtimer = 60f;
    public static float fleshtimerpaused = -1f;

    public static int quotaprogress = 0;
    public static int quota = 1;
    public static int quotatier = 1;
    private void FixedUpdate()
    {
        LogUpdate();

        StupidUpdate();
    }
    void StupidUpdate()
    {
        globaltimer += Time.fixedDeltaTime;

        fleshtimerpaused -= Time.fixedDeltaTime;
        if (fleshtimerpaused > 0f)
        {
            float tt = Mathf.Sin(globaltimer * 3f) * 0.5f + 0.5f;
            Color c = new Color(tt, tt, tt, .7f);
            fleshtimerUI.transform.Find("Background").GetComponent<Image>().color = c;
        }
        else
        {
            fleshtimerUI.transform.Find("Background").GetComponent<Image>().color = Color.white;
            fleshtimer -= Time.fixedDeltaTime;
        }

        if (fleshtimer < 30f)
        {
            float tt = Mathf.Sin(globaltimer * 3f) * 0.5f + 0.5f;
            Color c = new Color(1f, tt, tt, .7f);
            fleshtimerUI.transform.Find("Background").GetComponent<Image>().color = c;
        }

        if (GameObject.Find("quotaText") != null)
        {
            GameObject g = GameObject.Find("quotaText");
            g.GetComponent<TextMeshPro>().text = quotaprogress + "/" + quota;
        }

        if (fleshtimer < 0f)
        {
            Death();
        }


        fleshtimerUI.GetComponent<Slider>().value = fleshtimer / 80f;
        fleshtimerUI.transform.Find("fa").Find("Fill").GetComponent<Image>().color =
            Color.Lerp(Color.white, Color.black, (fleshtimer - 80f) / 100f);

        if (showdebugstuff)
        Director.LogConst("Time : " + globaltimer.ToString("#.00"), "Time", Color.white);
    }
    public static bool dying = false;
    public void Death()
    {
        if (dying) return;
        dying = true;
        //Director.LogTemp("You died!", Color.white, 2f);
        StartCoroutine(DeathCo());
    }
    public static void CalculateQuota()
    {
        quota = (int)Mathf.Pow(quotatier, 2f);
    }

    public class Log 
    {
        public string name;
        public GameObject logitself;
        public float timer;
    }
    public static List<Log> logstemp = new List<Log>();
    public static void LogTemp(string message, Color color, float timer)
    {
        Log l = new Log();
        l.logitself = Instantiate(Resources.Load<GameObject>("prefabs/LogText"), Vector3.zero, Quaternion.identity, canvas.transform);  
        l.logitself.GetComponent<TextMeshProUGUI>().text = message;
        l.logitself.GetComponent<TextMeshProUGUI>().color = color;
        l.logitself.GetComponent<RectTransform>().anchorMin = new Vector2(.5f, .5f);
        l.logitself.GetComponent<RectTransform>().anchorMax = new Vector2(.5f, .5f);
        l.logitself.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999f, -9999f);
        l.timer = timer;
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
            r.anchoredPosition = new Vector2(0f, (-200f - 60f * (logstemp.Count - i + 1)) * (Screen.height / 1080f));

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
            r.anchoredPosition = new Vector2(-500f * (Screen.width / 1920f), 
                -40f * (logsconst.Count - i + 1) * (Screen.height / 1080f));

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
            Director.LogTemp("resource not found | name | " + name + " | path | " + path, Color.red, 1f);
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
            Director.LogTemp("sound not found | " + name, Color.red, 1f);
        }
    }


    public static string C(string message, Color color)
    {
        return $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
    }
}
