using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownTrigger : MonoBehaviour
{
    public Rigidbody rb;

    bool isPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        rb = other.gameObject.GetComponent<Rigidbody>();
        if(rb == PlayerView.Instance.Model.rb)
        {
            isPlayer = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPlayer) return;
        if (PlayerView.Instance.Model.playerState != PLAYER_STATE.Grounded)
        {
            PlayerView.Instance.Model.rb.AddForce(-rb.velocity.x, 0, -rb.velocity.z);
        }
    }
}
