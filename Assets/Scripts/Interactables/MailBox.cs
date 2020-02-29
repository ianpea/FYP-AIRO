using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Invoke("CompleteLevel", 1);
            AudioManager.Instance.SFXCompleteGuide();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Slow down player.
        if(other.gameObject.tag == "Player")
        {
            Rigidbody rb = PlayerView.Instance.Model.rb;
            rb.AddForce(-rb.velocity.x *3.0f, 0, -rb.velocity.z*3.0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameplayView.Instance.Controller.CancelCompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        GameplayView.Instance.Model.isLevelComplete = true;
        GameplayView.Instance.Controller.CompleteLevel();
    }
}
