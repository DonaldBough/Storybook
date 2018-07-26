using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRaycaster : MonoBehaviour
{

    public float m_RayLength = 500f;              // How far into the scene the ray is cast.
    public LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
    Transform camera;                      //camera transform
    private RaycastHit hitInfo;                                     // Raycast result
    bool carrying;
    GameObject carriedObject;
    Vector3 lastPosition;
    System.DateTime pickedUpTime;

    public float distance;
    public float smooth;
    public float sensitivity;

    public GameObject capsule;

    // Use this for initialization
    void Start()
    {
        //error checking
        if (camera == null)
        {
            camera = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EyeRaycast();
        if (carrying)
        {
            carry(carriedObject);
            checkDrop();
        }
        else
        {
            EyeRaycast ();
        }
    }


    private void EyeRaycast()
    {
        if ((Input.GetKeyDown("space")))
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(camera.position, camera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                Debug.Log("raycast got a hit");
                Pickupable p = hit.collider.GetComponent<Pickupable>();
                if (p != null)
                {
                    Debug.Log("Hit a pickupable item!");
                    carrying = true;
                    carriedObject = p.gameObject;
                    p.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    pickedUpTime = System.DateTime.Now;
                    lastPosition = carriedObject.transform.position;
                }
                else
                {
                    Debug.Log("non pickupable item that was hit: " + hit.collider.gameObject.name);
                }
            }
        }
    }

    void carry(GameObject o)
    {
        o.GetComponent<Rigidbody>().isKinematic = true;
        //o.transform.position = Vector3.Lerp(o.transform.position, camera.position +
        //  camera.forward * distance, Time.deltaTime * smooth);

        o.transform.position = camera.position + camera.forward * distance;

         //o.transform.position = Vector3.Lerp(o.transform.position, capsule.transform.position +
        //capsule.transform.forward * distance, Time.deltaTime * smooth);
    }

    void checkDrop()
    {
        if (Input.GetKeyUp("space"))
        {
            throwObject();
        }
    }

    void throwObject()
    {
        carriedObject.GetComponent<BagController>().SetThrownTime();
        carrying = false;
        carriedObject.GetComponent<Rigidbody>().isKinematic = false;
        carriedObject.transform.parent = null;

        double elapsedTime = (System.DateTime.Now - pickedUpTime).TotalSeconds;
        double speed = (transform.position - lastPosition).magnitude / elapsedTime;
        speed *= 80;
        Debug.Log("Speed: " + speed);
        carriedObject.GetComponent<BagController>().SetThrownSpeed(speed);
        carriedObject.GetComponent<Rigidbody>().AddForce(camera.forward * (float)speed);
        carriedObject = null;
    }

    /**
    public Lab2VRInteractive GetCurrentInteractable()
    {
        return this.m_CurrentInteractible;
    } **/
    public RaycastHit GetHitInfo()
    {
        return this.hitInfo;
    }
}
