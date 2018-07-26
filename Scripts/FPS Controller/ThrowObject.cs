using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
	Transform mainCamera;
	bool carrying;
	GameObject carriedObject;
	Vector3 lastPosition;
	System.DateTime pickedUpTime;

	public float distance;
	public float smooth;
	public float sensitivity;
    /**
    void Start()
    {
        //mainCamera = GameObject.FindWithTag ("MainCamera");
        if (mainCamera == null)
        {
            mainCamera = UnityEngine.Camera.main.transform;
        }
    }

    void Update()
    {
		if (carrying) {
			carry (carriedObject);
			checkDrop ();
		} else {
			//pickup ();
		}
    }
    **/

	void carry(GameObject o) {
		o.transform.position = Vector3.Lerp (o.transform.position, mainCamera.transform.position + 
			mainCamera.transform.forward * distance, Time.deltaTime * smooth);
	}

	void pickup() {
		if (Input.GetMouseButtonDown (0)) {
			int x = Screen.width / 2;
			int y = Screen.height / 2;

            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));
            Debug.Log("new ray");
            //Ray ray = new Ray(mainCamera.position, mainCamera.forward);

            RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
                Debug.Log("raycast got a hit");
				Pickupable p = hit.collider.GetComponent<Pickupable> ();
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

	void checkDrop() {
		if (Input.GetMouseButtonUp (0)) {
			throwObject ();
		}
	}

	void throwObject() {
		this.GetComponent<BagController> ().SetThrownTime ();
		carrying = false;
		carriedObject.GetComponent<Rigidbody> ().isKinematic = false;
		transform.parent = null;

		double elapsedTime = (System.DateTime.Now - pickedUpTime).TotalSeconds;
		double speed = (transform.position - lastPosition).magnitude / elapsedTime;
		speed *= 100;
		Debug.Log ("Speed: " + speed);
		this.GetComponent<BagController> ().SetThrownSpeed (speed);
		carriedObject.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * (float) speed);
		carriedObject = null;
	}
}