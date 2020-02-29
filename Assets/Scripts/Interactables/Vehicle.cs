using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource SFXVehicle;
    public AudioSource SFXBrake;
    public int score;
    public float speed;
    public float stopTime = 0.0f;
    public bool isBraking = false;
    public const float STOP_TIME = 2.0f;
    public Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
        if(!SFXVehicle.isPlaying)
            SFXVehicle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //CheckFront();

        if (!isBraking)
        {
            if (!SFXVehicle.isPlaying)
                SFXVehicle.Play();
            stopTime = 0.0f;
            rb.velocity = -Vector3.forward * speed;
        }
        else
        {
            stopTime += Time.deltaTime;
            rb.velocity = Vector3.zero;
            if(stopTime >= STOP_TIME)
                isBraking = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Vehicle") && collision.contacts[0].normal == Vector3.forward)
        {
            isBraking = true;
            SFXBrake.Play();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Vehicle") && collision.contacts[0].normal == Vector3.forward)
        {
            isBraking = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ScoreManager.Instance.Score(this);
            AudioManager.Instance.SFXWhoosh();
            return;
        }
        if (other.gameObject.tag == "DespawnArea")
        {
            VehicleManager.Instance.DespawnVehicle(gameObject);
            return;
        }
    }

    public void CheckFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position, rb.velocity, out hit, 50.0f))
        {
            if (hit.distance <= 50.0f && (hit.collider.gameObject.tag == "Vehicle" || hit.collider.gameObject.tag == "Player"))
                isBraking = true;
        }
    }
}
