using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject NPCObj;
    public GameObject NPCBodies;
    public GameObject Player;
    public GameObject StartRoom;

    public GameObject musicSource;
    public GameObject ambientSource;

    public List<AudioClip> music                = new List<AudioClip>();
    public List<AudioClip> ambientSound         = new List<AudioClip>();


    public List<AudioClip> characterSounds      = new List<AudioClip>();
    public List<GameObject> gazeObjects         = new List<GameObject>();
    public List<GameObject> locations           = new List<GameObject>();

    public List<GameObject> NPCList             = new List<GameObject>();


    // Use a list for these instead. 
    private GameObject EarthHealer;
    private GameObject PeopleHealer;
    private GameObject ChildGuide;

    // Use this for initialization
    void Start () {
        // Create Characters

        EarthHealer = addNPC("EarthHealer", "EarthHealer", 0, new Vector3(4.1f, 0.45f, -97f), 1);
        addAction("Good Choice",         EarthHealer,  false, true,  false, 1, null,            true,  characterSounds[0], false, null);
        addAction("Bottle Bucket",       EarthHealer,  false, false, false, 3, gazeObjects[0],  true,  characterSounds[1], false, null);
        addAction("Go To Soil Bucket",   EarthHealer,  false, false, false, 0, null,            false, null,               true,  locations[1]);
        addAction("Look in Soil Bucket", EarthHealer,  false, false, false, 1, null,            true,  characterSounds[2], false, null);
        addAction("Soil Bucket Speech",  EarthHealer,  false, false, false, 1, null,            true,  characterSounds[3], false, null);

        PeopleHealer = addNPC("PeopleHealer", "PeopleHealer", 0, new Vector3(-6.5f, 0.45f, -45.9f), 1);
        addAction("Surprised",           PeopleHealer, false, true,  false,  1, null, true, characterSounds[4], false, null);
        addAction("Screen Bucket",       PeopleHealer, false, false, false,  3, gazeObjects[1], true, characterSounds[5], false, null);

        ChildGuide = addNPC("ChildGuide", "ChildGuide", 0, new Vector3(2.7f, 0.45f, -108f), 1);
        ChildGuide.transform.localScale = new Vector3(90.6f, 90.6f, 90.6f);

        addAction("Follow Me!",          ChildGuide,    false, true, false,  1, null, false, null, false, null);
        addAction("Go To EarthHealer",   ChildGuide,   false, false, false, 0, null, false, null, true, locations[1]);

        addAction("Follow Me!", ChildGuide, false, true, false, 1, null, false, null, false, null);
        addAction("Go To EarthHealer", ChildGuide, false, false, false, 0, null, false, null, true, locations[0]);
        addAction("Follow Me!", ChildGuide, false, true, false, 1, null, false, null, false, null);
        addAction("Go To PeopleHealer", ChildGuide, false, false, false, 0, null, false, null, true, locations[2]);


    }

    public void gazeReceived(GameObject target)
    {
        if (target.name == "EarthHealer WaterBottle Bucket") {
            EarthHealer.GetComponent<NPCScript>().gazeReceived(target);
        }
    }

    public void roomTransition()
    {
        // Wait till Whoosh is done, then change sounds.
        musicSource.GetComponent<AudioSource>().clip = music[2];
        musicSource.GetComponent<AudioSource>().Play();
        ambientSource.GetComponent<AudioSource>().clip = ambientSound[2];
        ambientSource.GetComponent<AudioSource>().Play();
        StartRoom.SetActive(false);

    }

    GameObject addNPC(string characterName, string characterRole, int NPCBody, Vector3 characterLocation, float movementSpeed)
    {
        //        var newNPC = Instantiate(NPCBodies.transform.GetChild(NPCBody).gameObject, characterLocation, Quaternion.Euler(-180, -90, 0));
        var newNPC = Instantiate(NPCBodies.transform.GetChild(NPCBody).gameObject, characterLocation, NPCBodies.transform.GetChild(NPCBody).rotation);
        // These aren't used currently, but help us know who's who in the Editor.
        newNPC.name = characterName;
        newNPC.GetComponent<NPCScript>().characterName = characterName;
        newNPC.GetComponent<NPCScript>().characterRole = characterRole;

        newNPC.GetComponent<NPCScript>().movementSpeed = movementSpeed; //NOT CURRENTLY IMPLEMENTED



        return newNPC;

    }

    void addAction(string actionName, GameObject targetNPC, bool isTriggered, bool isReady, bool isClicked, int triggerType, GameObject gazeObject, bool actionSpeak, AudioClip actionSound, bool actionLead, GameObject actionLocation)
    {
        var newAction = new characterAction();
        newAction.actionName = actionName;
        newAction.isTriggered = isTriggered;
        newAction.isReady = isReady;
        newAction.isClicked = isClicked;

        newAction.gazeObject = gazeObject;

        /*
         *  0 = Instant (as soon as "isReady" is true, trigger next Update()
         *  1 = Click       // Trigger when clicked
         *  2 = Proximity   // Trigger by proximity
         *  3 = Gaze        // Trigger by gaze
         *  4 = Follow      // Trigger by following
         */
        newAction.triggerType = triggerType;

        // If true, NPC will activate sound during this action.
        newAction.actionSpeak = actionSpeak;

        // Check GameController in the Hierarchy to see what sounds are loaded in and find their index number.
        newAction.actionSound = actionSound;

        targetNPC.GetComponent<NPCScript>().characterActions.Add(newAction);

        newAction.actionLead = actionLead;
        newAction.actionLocation = actionLocation;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
