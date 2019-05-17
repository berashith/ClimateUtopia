using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterAction
{
    public string actionName = "Unnamed";
    public bool isReady = false;        // Can't trigger until ready
    public bool isTriggered = false;
    public bool isFinished = false;

    public bool isTravelling = false;   // 

    public bool isClicked = false;
    public AudioClip actionSound;
    public Vector3 navPoint;


    /*
     *  0 = Instant (as soon as "isReady" is true, trigger next Update()
     *  1 = Click       // Trigger when clicked
     *  2 = Proximity   // Trigger by proximity
     *  3 = Gaze        // Trigger by gaze
     *  4 = Follow      // Trigger by following
     */

    public int triggerType;

    public GameObject gazeObject;

    public bool actionSpeak = false;
    public bool actionFollow = false;
    public bool actionLead = false;

    public GameObject actionLocation;
}

public class NPCScript : MonoBehaviour {

    public string characterName;
    public string characterRole;
    public float movementSpeed;
    public AudioSource NPCVoice;


    public bool clicked = false;
    public bool gazed = false;

    public List<characterAction> characterActions = new List<characterAction>();

    // Use this for initialization
    void Awake() {
        NPCVoice = GetComponent<AudioSource>();
	}

    public void gazeReceived(GameObject target)
    {
        if (gazed == false)
        {
            gazed = true;
            Debug.Log("GAZED");
            checkTriggers(target);
            gazed = false;
        }
    }


    public void clickNPC()
    {
        if (clicked == false)
        {
            clicked = true;
            checkTriggers();
            clicked = false;
        }
    }

    // Run through all triggers in list. If one is ready but not triggered, and the trigger condition is met, trigger it.
    void checkTriggers(GameObject target = null)
    {
        // NOTE current system is linear: when one action is triggered, it readies the next action. Ideally, have a list of actions that get 'readied' upon activation / completion.

        int actionIndex = 0; 
        foreach (var actionItem in characterActions)
        {
            // If we're speaking or moving, wait until we're done before we start the next action
            if (NPCVoice.isPlaying || GetComponent<UnityEngine.AI.NavMeshAgent>().hasPath) {
                return;
            }

            // Is it appropriate to trigger the action? Is it ready, untriggered and hasn't already been done?
            if (actionItem.isReady && !actionItem.isTriggered && !actionItem.isFinished)
            {
                bool triggerAction = false;
                // Check Triggers
                
                // Instant Trigger
                if (
                    (actionItem.triggerType == 0)                       ||        // Instant Trigger
                    (actionItem.triggerType == 1 && clicked == true)    ||        // Click Trigger
                    (actionItem.triggerType == 3 && gazed == true && GameObject.ReferenceEquals(target, actionItem.gazeObject)) 
                    ) {
                    triggerAction = true;
                }

                // Do actions

                // Deal with Click Actions
                if (triggerAction)
                {
                    Debug.Log(characterName + " is triggering " + actionItem.actionName);
                    actionItem.isReady = false;
                    clicked = false;
                    actionItem.isTriggered = true;

                    // Go through possible actions
                    // Should we speak? If so, trigger it
                    if (actionItem.actionSpeak)
                    {
                        // This should really be a coroutine. Ideally we should detect when it finished, so we can activate actions at start or end of audio.
                        NPCVoice.clip = actionItem.actionSound;
                        NPCVoice.Play(0);
                    }

                    // Should we move? If so, move
                    if (actionItem.actionLead)
                    {
                        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(actionItem.actionLocation.transform.position);
                    }

                    if (characterActions.Count > actionIndex + 1)
                    {
                        characterActions[actionIndex + 1].isReady = true;
                        Debug.Log(characterActions[actionIndex + 1].actionName + " is Ready");
                        characterActions[actionIndex].isFinished = true;
                    }
                }

            }
            if (characterActions.Count > actionIndex + 1)
            {
                actionIndex++;
            }
        }
    }
}
