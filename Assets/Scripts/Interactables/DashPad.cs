using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    public float dashForce;
    
    public void Dash(Collider other)
    {
        PlayerModel player = PlayerView.Instance.Model;
        other.gameObject.GetComponent<Rigidbody>().AddForce(player.cameraPivot.transform.forward * dashForce, ForceMode.Impulse);
        AudioManager.Instance.SFXPush();
    }

    private void OnTriggerEnter(Collider other)
    {
        Dash(other);
    }
}
