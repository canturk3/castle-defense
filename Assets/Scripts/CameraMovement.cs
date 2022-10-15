using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float panSpeed = 30f;
    public float scrollSpeed = 400f;
    public float minX = -40f, maxX = 40f, minZ = -10f, maxZ = 40f, minY = 5f, maxY = 20f;

    public float rotateSpeed = 0.01f;

    [SerializeField]
    [Range(0f, 1f)]
    private float lerpPct = 0.5f;

    void Start()    
    {
        
    }

    void Update()
    {
        Vector3 newPos = transform.position;
        if (Input.GetKey("w"))
        {
            newPos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            newPos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            newPos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            newPos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        newPos.y -= scroll * scrollSpeed * Time.deltaTime;

        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.x = Mathf.Clamp(newPos.x,minX,maxX);
        newPos.z = Mathf.Clamp(newPos.z,minZ,maxZ);

        // TODO Rotate with right click may require changing the movement also
       /* if (Input.GetMouseButton(1))
        {
            Quaternion newRotation = new Quaternion(transform.rotation.x + Input.GetAxis("Mouse Y"),
                transform.rotation.y + Input.GetAxis("Mouse X"), 0, transform.rotation.w);

            transform.rotation = Quaternion.Lerp(transform.rotation,newRotation,rotateSpeed);
            
        }*/
        transform.position = Vector3.Lerp(transform.position,newPos,lerpPct);
    }
}
