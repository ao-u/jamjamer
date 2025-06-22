using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Interactable : MonoBehaviour
{
    GameObject hitbox;
    Vector3 baseScale;
    public Quaternion baseRot;
    AudioSource aud;
    Rigidbody rb;
    void Start()
    {
        if (transform.Find("hitbox") == null)
        {
            Director.LogTemp("FAKE INTERACTABLE NO HITBOX!!!", Color.red, 1f);
            return;
        }
        hitbox = transform.Find("hitbox").gameObject;
        baseScale = transform.localScale;
        baseRot = transform.localRotation;
        //asdas

        if (GetComponent<AudioSource>() == null)
        {
            this.AddComponent<AudioSource>();
        }
        aud = GetComponent<AudioSource>();

        
        if (index.Contains("pickup"))
        {
            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
            }
            rb = gameObject.GetComponent<Rigidbody>();
        }

        /*
        if (name.Contains("ball"))
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Mesh m = new Mesh();
            m.vertices = mesh.vertices;
            m.triangles = mesh.triangles;
            m.uv = mesh.uv;
            m.normals = mesh.normals;
            m.colors = mesh.colors;
            m.tangents = mesh.tangents;

            Vector3[] verts = m.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 v = verts[i];
                {
                    //float scale = .2f;
                    verts[i] *= Random.Range(1f, 1.5f);
                    //verts[i] = new Vector3(v.x + Random.Range(-scale, scale), v.y + Random.Range(-scale, scale), v.z + Random.Range(-scale, scale));
                }
            }
            m.vertices = verts;
            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();
            GetComponent<MeshFilter>().sharedMesh = m;
        }*/
    }
    float hoveringtimer = -1f;
    float interacttimer = -1f;
   
    private void FixedUpdate()
    {
        hoveringtimer -= Time.fixedDeltaTime;
        interacttimer -= Time.fixedDeltaTime;
        pickuptimer -= Time.fixedDeltaTime;

        float lerpvalue = 0.3f;
        Quaternion targetrot;
        Vector3 targetscale;
        if (index == "pickuptech")
        {
            Vector3 dir = (GameObject.Find("player").transform.position - transform.position).normalized;
            baseRot = Quaternion.LookRotation(dir);
            if (pickedup)
            {
                baseRot = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
        }
        if (hoveringtimer > 0.1f)
        {
            
           
            targetscale = baseScale * 1.1f;
            transform.localScale = Vector3.Lerp(transform.localScale, targetscale, lerpvalue);

            float speed = 2f, scale = 1f;
            float offset = (Mathf.Sin(Time.fixedTime * speed) * scale);
            float offset1 = (Mathf.Sin(Time.fixedTime * speed * 1.1f) * scale);

            targetrot = Quaternion.Euler(new Vector3(baseRot.eulerAngles.x + offset1, baseRot.eulerAngles.y, baseRot.eulerAngles.z + offset));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetrot, lerpvalue);
        }
        else
        {

            targetscale = baseScale;
            transform.localScale = Vector3.Lerp(transform.localScale, targetscale, lerpvalue);

            targetrot = baseRot;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetrot, lerpvalue);
        }

        if (rb != null && !pickedup)
        {
            Director.ApplyGravity(rb);
            rb.velocity *= .95f;
        }
        

        if (index == "quota")
        {
            if (Director.quotaprogress >= Director.quota)
            {
                if (GameObject.Find("quotaBlock") != null)
                {
                    if (!openingdoor)
                    {
                        openingdoor = true;
                        StartCoroutine(openDoor());
                    }
                    
                }
                Director.LogTemp("QUOTA_TIER_" + Director.quotatier + " completed in " + Director.globaltimer, Color.white, 2f);
                Menu.besttimes[Director.quotatier] = Mathf.Min(Menu.besttimes[Director.quotatier], Director.globaltimer);


                //Director.LogTemp("QUOTA_MET", Color.white, 3f);
                Director.quotaprogress = 0;
                Director.quotatier++;
                Director.CalculateQuota();

                
            }
        }
    }
    bool openingdoor = false;
    IEnumerator openDoor()
    {
        GameObject g = GameObject.Find("quotaBlock");
        for (int i = 0; i < 22; i++)
        {
            g.transform.position += new Vector3(0f, 1f, 0f);
            yield return new WaitForFixedUpdate();
        }
    }
    void Update()
    {
        if (pickedup && Player.rightclick > 0f)
        {
            //
        }
        if (transform.position.y < -100f)
        {
            Destroy(gameObject);
        }
    }
    public void Shake(float amount)
    {
        float div = 3f;
        transform.localEulerAngles += new Vector3(RandSign() * amount + Random.Range(-amount / div, amount / div), 0f, RandSign() * amount + Random.Range(-amount / div, amount / div));
    }
    public static float RandSign() { return Random.Range(0, 2) * 2 - 1; }
    public void Hovering()
    {
        hoveringtimer = interacttimer > 0f || hoveringtimer > .2f ? 0f : .2f;
        
    }
    public string index = "";
    public void OnInteract()
    {
        if (interacttimer > 0f) return; 
        //Director.LogTemp("Interacted with " + gameObject.name, Color.green);
        Shake(10f);
        interacttimer = .2f;
        InteractIndex(index);
    }
    public bool pickedup = false;
    float pickuptimer = -1f;
    void Pickup()
    {
        if (pickuptimer > 0f) return; 
        transform.parent = Director.layeredcamera.transform;
        transform.gameObject.layer = LayerMask.NameToLayer("UI");
        transform.localPosition = new Vector3(3f, -2f, 6f);
        pickedup = true;
        pickuptimer = 0.2f;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
        Player.helditems.Add(gameObject);
    }
    public void Throw()
    {
        if (pickuptimer > 0f) return;
        Player.helditems.Remove(gameObject);
        pickedup = false;
        pickuptimer = 0.2f;
        transform.parent = Director.maincamera.transform;
        transform.localPosition = new Vector3(0f, -2f, 5f);
        transform.parent = null;
        transform.gameObject.layer = LayerMask.NameToLayer("Default");
        



        if (rb != null)
        {
            rb.velocity += Director.player.GetComponent<Rigidbody>().velocity * .7f;
            rb.velocity += Director.maincamera.transform.forward * 30f;
        }
        
    }

    float value = 0f;
    void InteractIndex(string s)
    {
        string soundeffect = "selectno";
        switch (s)
        {
            case "SENSUP":
                if (Player.sensmult >= 1.8f)
                {
                    return;
                }
                else
                {
                    Player.sensmult += 0.1f;
                    Player.UpdateSensSliderThing();
                    soundeffect = "selectyes";
                }
                
                break;
            case "SENSDOWN":
                if (Player.sensmult <= 0.2f)
                {
                    return;
                }
                else
                {
                    Player.sensmult -= 0.1f;
                    Player.UpdateSensSliderThing();
                    soundeffect = "selectyes";
                }
                    
                break;


            case "lock":
                Director.LogTemp("You have not met the quota.", Color.white, 3f);
                break;
            case "quota":
                Color c = Color.white;
                if (Director.globaltimer > 60f)
                {
                    Director.LogTemp("If you cant meet the quota, feed the machine your own " + Director.C("FLESH", Color.red), c, 3f);
                }
                else
                {
                    Color cc = new Color(.6f, .2f, .2f);
                    Director.LogTemp("Feed the machine " + Director.C("SCRAP", cc) + " to meet your quota.", c, 3f);
                }



                    break;
            case "roomspawner":
                soundeffect = "selectyes";
                if (GameObject.Find("spawnRoom") != null)
                {
                    MapGen.GenRoom(false);
                }
                value = value > 1f ? 0f : 99f;
                transform.parent.localEulerAngles = value > 1f ? new Vector3(0f, 115f, 0f) : new Vector3(0f, 0f, 0f);
                index = "door";
                break;
            case "hallwayspawner":
                soundeffect = "selectyes";
                if (GameObject.Find("spawnHallway") != null)
                {
                    MapGen.GenHallway();
                }
                value = value > 1f ? 0f : 99f;
                transform.parent.localEulerAngles = value > 1f ? new Vector3(0f, 115f, 0f) : new Vector3(0f, 0f, 0f);
                //lock the door behind you LOL?
                transform.parent.parent.Find("doorholder1").Find("door1").gameObject.GetComponent<Interactable>().index = "lockeddoor";
                transform.parent.parent.Find("doorholder1").localEulerAngles = new Vector3(0f, 0f, 0f);
                index = "door";


                break;
            case "lockeddoor":
                soundeffect = "selectno";
                Director.LogTemp("im locked!", Color.red, 1f);
                break;
            case "door":
                soundeffect = "selectyes";
                value = value > 1f ? 0f : 99f;
                transform.parent.localEulerAngles = value > 1f ? new Vector3(0f, 115f, 0f) : new Vector3(0f, 0f, 0f);
                break;
            case "pickupflesh":
                soundeffect = "selectyes";
                Pickup();
                break;
            case "pickuptech":
                soundeffect = "selectyes";
                Pickup();
                break;
            case "vent":
                Director.LogTemp("uhhh im a vent", Color.gray, 1f);
                soundeffect = "wiggle";
                break;
            case "menuplay":
                soundeffect = "selectyes";
                StartCoroutine(GameObject.Find("Main Camera").GetComponent<Menu>().Play());
                break;
            case "menuquit":
                soundeffect = "selectno";
                Menu.Quit();
                break;
            default:
                Debug.Log("wrong interactable index: " + s);
                soundeffect = "selectno";
                break;
        }
        Director.PlaySound(soundeffect, aud);
    }
    private void OnTriggerEnter(Collider c)
    {
        if (c.transform.name == "FEED")
        {
            //Director.LogTemp("consumed", Color.green, 1f);
            Director.PlaySound("hurt", Director.aud);
            GameObject itempref = Resources.Load<GameObject>("prefabs/item1");
            Vector3 spawnloc = GameObject.Find("PRODUCE").transform.position;
            GameObject g = Instantiate(itempref, spawnloc, Quaternion.identity);
            g.transform.position += new Vector3(Random.Range(-.5f, .5f), 0f, Random.Range(-.5f, .5f));
            
            if (index == "pickupflesh")
            {
                Director.fleshtimer += 40f;
            }
            if (index == "pickuptech")
            {
                Director.quotaprogress += 1;
                float r = Random.value;
                if (Director.globaltimer < 60f) r = 0f;
                Color cc = new Color(.6f, .2f, .2f);
                if (r > .8f) {
                    Player.scrapSPEED += 0.1f;
                    Director.LogTemp("+" + Director.C("SPEED", Color.blue) + " from " + Director.C("SCRAP", cc) + "!", Color.white, 2f);
                }
                else if (r > .40f)
                {
                    Player.scrapSTRENGTH+= 0.2f;
                    Director.LogTemp("+" + Director.C("STRENGTH", Color.red) + " from " + Director.C("SCRAP", cc) + "!", Color.white, 2f);
                }
                else if (r > .2f)
                {
                    Player.scrapDASHCD+= 0.1f;
                    Director.LogTemp("+" + Director.C("DASH", Color.green) + " from " + Director.C("SCRAP", cc) + "!", Color.white, 2f);
                }
                else
                {

                }
            }


            Destroy(gameObject);
        }
    }
}
