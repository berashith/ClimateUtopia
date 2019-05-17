using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {
    public GameObject GameController;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(doTransition());
    }

    public IEnumerator doTransition()
    {
        // Play "whoosh", wait till crescendo, activate room transition.
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(3f); // waits 3 seconds
        GameController.GetComponent<GameController>().roomTransition();

    }

}
