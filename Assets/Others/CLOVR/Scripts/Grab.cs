using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CLOVR_Plugin;

public class Grab : MonoBehaviour {

    public GameObject cameraPivot;
    private bool hold;
    private GameObject holdObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hold)
            Throw();
        else
            Pick();
    }

    private void Pick()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var hit = CLOVRRaycast.Cast(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
            if (hit.collider != null)
            {
                if (CanGrab(hit.collider.gameObject))
                {
                    if (Vector3.Distance(hit.collider.transform.position, cameraPivot.transform.position) < 4f)
                    {
                        hold = true;
                        holdObject = hit.transform.gameObject;
                        holdObject.GetComponent<Rigidbody>().isKinematic = true;
                        holdObject.transform.SetParent(cameraPivot.transform);
                    }
                }
            }
        }
    }

    bool CanGrab(GameObject obj)
    {
        return obj.GetComponent<Rigidbody>() != null;
    }

    void Throw()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            hold = !hold;
            holdObject.GetComponent<Rigidbody>().isKinematic = false;
            holdObject.GetComponent<Rigidbody>().AddForce(cameraPivot.transform.forward * 1000f);
            holdObject.transform.SetParent(null);
            holdObject = null;
        }
    }
}
