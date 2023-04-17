using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomControl : MonoBehaviour
{
    public float minZoom = 1f;
    public float maxZoom = 10f;
    float sensitivity = 2f;
    Vector3 mousePositionOnScreen;
    Vector3 mousePositionOnScreen1;
    Vector3 mouseOnWorld;

    // Start is called before the first frame update
    void Start()
    {
        mousePositionOnScreen = new Vector3();
        mousePositionOnScreen1 = new Vector3();
        mouseOnWorld = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            mousePositionOnScreen = mousePositionOnScreen1;
            mousePositionOnScreen1 = Input.mousePosition;
            if (Vector3.Distance(mousePositionOnScreen, mousePositionOnScreen1) == 0)
            {
                float fov = Camera.main.orthographicSize;
                fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
                fov = Mathf.Clamp(fov, minZoom, maxZoom);
                Camera.main.orthographicSize = fov;
                Vector3 mouseOnWorld1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 posDiff = mouseOnWorld - mouseOnWorld1;
                Vector3 camPos = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(camPos.x + posDiff.x, camPos.y + posDiff.y, camPos.z);
            }
            else
            {
                mouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}