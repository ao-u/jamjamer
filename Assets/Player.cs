using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Vector2 rotation = Vector2.zero;
    Vector2 sens = new Vector2(1.5f, 1f);
    public static float sensmult = 1f;
    Transform maincamera;
    Transform ori;
    public static AudioSource aud;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        speed = basespeed;
        maxspeed = basemaxspeed;


        ori = transform.Find("ori");
        maincamera = GameObject.Find("Main Camera").transform;
        shiftUI = GameObject.Find("shiftUI");
        lc1 = GameObject.Find("LayeredCamera");
        lc2 = GameObject.Find("LayeredCamera2");
        canvasholder = GameObject.Find("CameraCanvasHolder");
        rb = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
        rb.freezeRotation = true;
        rb.useGravity = false;


        UpdateSensSliderThing();
        
    }
    public static void UpdateSensSliderThing()
    {
        GameObject g = GameObject.Find("sensslider");
        float multaspos = (-1f + Player.sensmult) * 3.5f;
        g.transform.localPosition = new Vector3(g.transform.localPosition.x, g.transform.localPosition.y, multaspos);
    }
    void Update()
    {
        GetInput();

        /*
        if (Input.GetKey(KeyCode.H))
        {
            Application.targetFrameRate = 30;
        }
        else
        {
            Application.targetFrameRate = 300;
        }
        */
        if (Input.GetKeyDown(KeyCode.R))
        {
            maincamera.GetComponent<Director>().Death();
        }
        sens = new Vector2(1.5f, 1f) * sensmult;
        realdeltatime = Time.deltaTime;
    }
    float realdeltatime;
    float fpstimer = .2f;
    private void FixedUpdate()
    {
        fpstimer -= Time.fixedDeltaTime;
        if (fpstimer < 0f && Director.showdebugstuff) { 
        Director.LogConst("FPS : " + (1.0f / realdeltatime).ToString("#.00"), "FPS", Color.white);
        fpstimer = .2f; }

        if (!Director.dying)
        {
            Movement();

            InteractableController();
        }
        else
        {
            rb.velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * 10f;
        }
        
        
    }
    void UIStuff()
    {
        //WHY CANT I LERP IN UPDATE WHY CANT I LERP IN UPDATE WHY WHYW HY WHYW HYW HY IVE TRIDEE EVERYTHING
        float lerpvalue = .97f;
        lerpvalue /= 1f + Time.deltaTime;
        float lerpvalue2 = .94f;
        lerpvalue2 /= 1f + Time.deltaTime;
        canvasholder.transform.position = Vector3.Lerp(canvasholder.transform.position, maincamera.position, lerpvalue);
        canvasholder.transform.rotation = Quaternion.Slerp(canvasholder.transform.rotation, maincamera.rotation, lerpvalue2);
        lc2.transform.position = maincamera.position;
        lc2.transform.rotation = maincamera.rotation;
    }
    private void LateUpdate()
    {
        Camera();
        //UIStuff();
    }
    GameObject lc1, lc2, canvasholder;
    void Camera()
    {
        Quaternion targetcamerarot;
        rotation.x += mousex * sens.x;
        rotation.y += mousey * sens.y;
        rotation.y = Mathf.Clamp(rotation.y, -89f, 89f);
        Quaternion rot = Quaternion.AngleAxis(rotation.x, Vector3.up) * Quaternion.AngleAxis(rotation.y, Vector3.left);
        ori.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        targetcamerarot = rot;

        float amount = touchingsurface ? 1.5f : 1f;
       // amount *= shift ? 3f : 1f;
        targetcamerarot = Quaternion.Euler(new Vector3(targetcamerarot.eulerAngles.x, targetcamerarot.eulerAngles.y, -horizontal * amount));
        maincamera.position = transform.position + new Vector3(0f, playerheight / 2f - 2f, 0f);

        maincamera.rotation = Quaternion.Euler(
                new Vector3(targetcamerarot.eulerAngles.x,
                            targetcamerarot.eulerAngles.y,
                            Mathf.LerpAngle(maincamera.eulerAngles.z, targetcamerarot.eulerAngles.z, 2f * Time.deltaTime)));
        float offsety = (Mathf.Sin(Time.fixedTime * 2f) / 4000f);
        float offsetx = (Mathf.Sin(Time.fixedTime * 3f) / 6000f);

        Vector3 offsetvector = new Vector3(offsetx, 0f, offsety);
        maincamera.eulerAngles += offsetvector;

        //lc2.transform.eulerAngles += offsetvector;
        
        //lc2.transform.rotation = Quaternion.Slerp(lc1.transform.rotation, Quaternion.identity, .2f);
        //Director.layeredcamera.transform.rotation = Quaternion.Slerp(Director.layeredcamera.transform.rotation, maincamera.rotation, .2f);
    }
    public static float mousex, mousey;
    public static float shift;
    public static float jump;
    public static float vertical, horizontal;
    public static float leftclick;
    public static float rightclick;
    void GetInput()
    {
        mousex = Input.GetAxis("Mouse X");
        mousey = Input.GetAxis("Mouse Y");
        shift -= Time.deltaTime;
        shift = Input.GetKeyDown(KeyCode.LeftShift) ? .1f : shift;
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        jump -= Time.deltaTime;
        jump = (Input.GetKey(KeyCode.Space) || Input.mouseScrollDelta.y < 0f) ? .1f : jump;
        leftclick -= Time.deltaTime;
        leftclick = (Input.GetMouseButtonDown(0) || Input.mouseScrollDelta.y > 0f || Input.GetKeyDown(KeyCode.E)) ? .1f : leftclick;
        rightclick -= Time.deltaTime;
        rightclick = Input.GetMouseButtonDown(1) ? .1f : rightclick;
    }
    float basespeed = 3f;
    float speed;
    float basemaxspeed = 20f;
    float maxspeed;
    
    float jumpforce = 15f;

    public static float scrapSPEED = 1f;
    public static float scrapSTRENGTH = 1f;
    public static float scrapDASHCD = 1f;

    float jumptimer = -1f;
    float shifttimer = -1f;
    float throwtimer = -1f;
    
    public static Vector3 vel;
    public static bool touchingsurface;
    Vector3 point;
    const float playerheight = 7.5f; 
    const float playerradius = 2.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position - new Vector3(0, playerheight / 2 - playerradius, 0f), playerradius);
        Gizmos.DrawSphere(transform.position + new Vector3(0, playerheight / 2 - playerradius, 0f), playerradius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, .1f);
    }
    GameObject shiftUI;
    float shiftmax = .3f;
    void Movement()
    {

        //TIMERS
        jumptimer -= Time.fixedDeltaTime;
        

        //FS
        //if (transform.position.y < -9999f) transform.position = new Vector3(0, 10, 0);

        Vector3 r = rb.velocity;

        


        //get closest point to butt sphere of player
        Vector3 spherepos = transform.position - new Vector3(0, playerheight / 2 - playerradius, 0f);
        Collider[] c = Physics.OverlapSphere(spherepos, playerradius, LayerMask.GetMask("Default"));
        Vector3 closestpoint = new Vector3(123456f, 99999f, 99999f);
        foreach (Collider cc in c)
        {
            if (!cc.isTrigger)
            {
                Vector3 p = cc.ClosestPoint(spherepos);
                if (Vector3.Distance(p, spherepos) < Vector3.Distance(closestpoint, spherepos)) closestpoint = p;
            }
        }
        point = closestpoint;
        touchingsurface = point.x != 123456f;

        shifttimer -=  ( touchingsurface ? Time.fixedDeltaTime * .2f : Time.fixedDeltaTime * .5f ) * scrapDASHCD;
        shiftUI.GetComponent<Slider>().maxValue = shiftmax;
        shiftUI.GetComponent<Slider>().value = shiftmax - shifttimer;


        //jump

        Vector3 realdir = (spherepos - closestpoint).normalized;
        //Director.LogConst("realdir   : " + realdir, "point", Color.white);
        if (jump > 0f && touchingsurface && jumptimer < 0f && realdir.y > .6f)
        {
            jumptimer = .1f;
            rb.velocity = new Vector3(r.x, 0f, r.z);

            rb.velocity += realdir * jumpforce * (realdir.y < .1 ? 3f : 0f);
            rb.velocity += Vector3.up * jumpforce;

        }


        //movement
        Vector3 wishdir = ori.forward * vertical + ori.right * horizontal;
        wishdir = wishdir.normalized;

        
        //make walls unsticky
        if (touchingsurface)
        {
            Vector3 surfnormall = (spherepos - point).normalized;
            if (Vector3.Dot(wishdir, surfnormall) < 0f)
            {
                wishdir = Vector3.ProjectOnPlane(wishdir, surfnormall);
            }
                
        }
        
        Vector3 velnoy = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        /*
        velnoy *= touchingsurface ? .95f : .99f;
        float cs = Vector3.Dot(velnoy, wishdir);
        float addspeed = Mathf.Clamp(maxspeed - cs, 0f, 1.2f);
        velnoy += wishdir * addspeed;
        */
        velnoy *= touchingsurface ? .9f : .95f;
        Vector3 addspeed = wishdir * speed * Mathf.Clamp( maxspeed / velnoy.magnitude , 0f, 5f );
        velnoy += addspeed;

        //shift
        rb.velocity = new Vector3(velnoy.x, rb.velocity.y, velnoy.z);
        Director.ApplyGravity(rb);


        if (shifttimer < 0f && shift > 0f)
        {
            //Director.LogTemp("shifted", Color.green);
            float shiftspeed = 90f;
            shifttimer = shiftmax;
            Vector3 shiftdir = Vector3.Lerp(wishdir, Director.maincamera.transform.forward, .7f).normalized;
            //shiftdir = Director.maincamera.transform.forward;
            rb.velocity += shiftdir * shiftspeed * speedmult;
            Director.PlaySound("swipejump", aud);
        }


        

        if (Director.showdebugstuff)
        {
            Director.LogConst("Velocity : " + velnoy.magnitude.ToString("#.00"), "velnoy", Color.white);
            Director.LogConst("STR:" + scrapSTRENGTH + " SPD:" + scrapSPEED + " DCD:" + scrapDASHCD, "stats", Color.white);
        }
        
        //interactions

        RaycastHit rayhit;
        Ray ray = new Ray(maincamera.transform.position, maincamera.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);
        if (Physics.Raycast(ray, out rayhit, 20f, 1))
        {
            if (rayhit.transform.name.Contains("item")
             || rayhit.transform.name.Contains("hitbox"))
            {

                Interactable i = rayhit.transform.GetComponentInParent<Interactable>();
                //Director.LogTemp("Interacting with " + i.name, Color.green);
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

    public static List<GameObject> helditems = new List<GameObject>();
    public float speedmult;
    void InteractableController()
    {

        throwtimer -= Time.fixedDeltaTime;

        float heavyness = 1f + (helditems.Count * ((.1f) / scrapSTRENGTH));
        speedmult = 1f / heavyness;

        speedmult *= scrapSPEED;

        speed = basespeed * speedmult;
        maxspeed = basemaxspeed * speedmult;

        for (int i = 0; i < helditems.Count; i++) 
        {
            int ia = helditems.Count - i - 1;
            helditems[i].transform.localPosition = new Vector3(2f + ia * 3f, -2f - ia, 6f + ia * 3f);
            helditems[i].GetComponent<Rigidbody>().velocity *=.95f;
            helditems[i].GetComponent<Interactable>().baseRot = Quaternion.Euler(0f, 0f, 0f);
        }

        if (rightclick > 0f && helditems.Count > 0 && throwtimer < 0f)
        {
            throwtimer = .2f;
            GameObject item = helditems[helditems.Count - 1];
            item.GetComponent<Interactable>().Throw();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Director.LogTemp("CB !", Color.red);

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "safeTrigger")
        {
            Director.fleshtimerpaused = .2f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "FEED")
        {
            GameObject.Find("Main Camera").GetComponent<Director>().Death();
        }
    }
}
