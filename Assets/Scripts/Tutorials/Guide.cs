using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    PlayerModel player;
    void Start()
    {
        player = PlayerView.Instance.Model;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position * 2 - player.rb.position);
    }
}
