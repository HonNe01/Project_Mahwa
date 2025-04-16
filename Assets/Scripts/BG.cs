using UnityEngine;

public class BG : MonoBehaviour
{
    Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
    }
}
