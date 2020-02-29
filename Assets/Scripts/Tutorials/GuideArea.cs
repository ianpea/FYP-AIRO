using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArea : MonoBehaviour
{
    public GameObject complete;
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
        if(other.gameObject.tag == "Player" && !complete.activeInHierarchy)
        {
            complete.SetActive(true);
            AudioManager.Instance.SFXCompleteGuide();
        }
    }
}
