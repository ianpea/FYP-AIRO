using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public bool hitByRay = false;
    Vector3 originalPos;
    float speed = 5.0f;
    Material originalMaterial;
    bool isActivated = false;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        originalMaterial = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitByRay && !isActivated)
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * speed, transform.position.z);
        else
            transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Activate(other));
    }

    private IEnumerator Activate(Collider other)
    {
        isActivated = true;
        WorldSpaceUIManager.Instance.Pressed(gameObject);
        yield return new WaitForSeconds(2.0f);
        if (other.gameObject.tag == "ActivateArea")
        {
            switch (gameObject.name)
            {
                case "Start":
                    VirtualSceneManager.Instance.NextScene();
                    break;
                case "Help":
                    break;
                case "Quit":
                    UnityEditor.EditorApplication.isPlaying = false;
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
