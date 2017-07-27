using UnityEngine;

public class InWorldUIRotatesToCamera : MonoBehaviour {

    private RectTransform uiElement;    
    private Transform cameraRigRotator;

    private void Start()
    {
        cameraRigRotator = GameObject.Find("CameraRigRotator").transform;
        uiElement = GetComponent<RectTransform>();
    }

    private void Update () {
        uiElement.localRotation = Quaternion.Euler(0f, cameraRigRotator.rotation.eulerAngles.z, 0f);
	}
}
