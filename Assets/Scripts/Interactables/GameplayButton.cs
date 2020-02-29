using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayButton : MonoBehaviour
{
    public bool hitByRay = false;
    float originalY;
    float speed = 35.0f;
    Material originalMaterial;
    bool isActivated = false;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
        originalMaterial = gameObject.GetComponent<Renderer>().material;
    }   

    // Update is called once per frame
    void Update()
    {
        if (hitByRay && !isActivated)
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * speed, transform.position.z);
        else
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, originalY, transform.position.z), Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Activate(other));
    }

    private IEnumerator Activate(Collider other)
    {
        isActivated = true;
        WorldSpaceUIManager.Instance.Pressed(gameObject);
        yield return new WaitForSeconds(1.0f);
        if (other.gameObject.tag == "ActivateArea")
        {
            switch (gameObject.name)
            {
                case "MainMenuButton":
                    VirtualSceneManager.Instance.ReturnToMenu();
                    break;
                case "PauseButton":
                    if (Time.timeScale == 1.0f)
                        Time.timeScale = 0.0f;
                    else
                        Time.timeScale = 1.0f; 
                    break;

            }
        }
        isActivated = false;
        WorldSpaceUIManager.Instance.HardReset();
    }

    public void Reset()
    {
        hitByRay = false;
    }

    public void HardReset()
    {
        ResetColor();
    }

    private void ResetColor()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = originalMaterial;
        }
    }
}
