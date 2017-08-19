using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Map;

namespace GrimoireTD.UI
{
    public class CameraController : SingletonMonobehaviour<CameraController>
    {
        [SerializeField]
        private int cameraHorizontalPanZone;
        [SerializeField]
        private int cameraVerticalPanZone;
        [SerializeField]
        private float cameraHorizontalSpeed;
        [SerializeField]
        private float cameraVerticalSpeed;
        [SerializeField]
        private float cameraMoveLerpFactor;
        private float cameraLeftClamp = 0f;
        private float cameraBottomClamp = 0f;
        private float cameraRightClamp;
        private float cameraTopClamp;

        [SerializeField]
        private float cameraRotationSpeed;
        [SerializeField]
        private float cameraRotationSpeedLerpFactor;
        private float cameraTargetRotation = 0f;

        [SerializeField]
        private float cameraDollyMultiplier;
        [SerializeField]
        private float cameraDollyLerpFactor;
        [SerializeField]
        private float cameraDollyClampMin;
        [SerializeField]
        private float cameraDollyClampMax;
        [SerializeField]
        private float cameraDollyWheelSensitivity;
        [SerializeField]
        private float cameraDollyKeySensitivity;
        private float cameraDollyInput;
        private float cameraTargetDolly = -5f;

        [SerializeField]
        private float cameraTiltSpeed;
        [SerializeField]
        private float cameraTiltLerpFactor;
        [SerializeField]
        private float cameraTiltClampMax;
        [SerializeField]
        private float cameraTiltClampMin;
        [SerializeField]
        private float cameraTiltTarget;

        private Camera mainCamera;
        private Transform cameraTilter;
        private Transform cameraRotator;
        private Vector3 cameraRigTargetPosition;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Camera Controller Start");

            //camera components
            mainCamera = Camera.main;
            cameraTilter = mainCamera.transform.parent.transform;
            cameraRotator = cameraTilter.parent.transform;
            cameraRigTargetPosition = cameraRotator.position;

            //camera clamps
            IMapData map = MapGenerator.Instance.Map;
            cameraRightClamp = map.Width * MapRenderer.HEX_OFFSET * 2;
            cameraTopClamp = map.Height * 0.75f;
        }

        private void Update()
        {
            CameraPanning();
            CameraRotation();
            CameraDollying();
            CameraTilting();
        }

        private void CameraPanning()
        {
            if (Input.mousePosition.x < cameraHorizontalPanZone || Input.GetAxis("CameraPanHorizontal") < 0)
            {
                MoveCameraTarget(-cameraHorizontalSpeed * Time.deltaTime, 0f);
            }
            else if (Input.mousePosition.x > Screen.width - cameraHorizontalPanZone || Input.GetAxis("CameraPanHorizontal") > 0)
            {
                MoveCameraTarget(cameraHorizontalSpeed * Time.deltaTime, 0f);
            }

            if (Input.mousePosition.y < cameraVerticalPanZone || Input.GetAxis("CameraPanVertical") < 0)
            {
                MoveCameraTarget(0f, -cameraVerticalSpeed * Time.deltaTime);
            }
            else if (Input.mousePosition.y > Screen.height - cameraVerticalPanZone || Input.GetAxis("CameraPanVertical") > 0)
            {
                MoveCameraTarget(0f, cameraVerticalSpeed * Time.deltaTime);
            }

            cameraRotator.position = Vector3.Lerp(cameraRotator.position, cameraRigTargetPosition, cameraMoveLerpFactor);
        }

        private void MoveCameraTarget(float hor, float ver)
        {
            cameraRigTargetPosition.x = Mathf.Clamp(cameraRigTargetPosition.x + Mathf.Cos(Mathf.Deg2Rad * cameraRotator.eulerAngles.z) * hor - Mathf.Sin(Mathf.Deg2Rad * cameraRotator.eulerAngles.z) * ver, cameraLeftClamp, cameraRightClamp);
            cameraRigTargetPosition.y = Mathf.Clamp(cameraRigTargetPosition.y + Mathf.Sin(Mathf.Deg2Rad * cameraRotator.eulerAngles.z) * hor + Mathf.Cos(Mathf.Deg2Rad * cameraRotator.eulerAngles.z) * ver, cameraBottomClamp, cameraTopClamp);
        }


        private void CameraRotation()
        {
            cameraTargetRotation += (Input.GetAxisRaw("CameraRotate") * cameraRotationSpeed * Time.deltaTime) % 360f;

            cameraRotator.rotation = Quaternion.Lerp(cameraRotator.rotation, Quaternion.Euler(0f, 0f, cameraTargetRotation), cameraRotationSpeedLerpFactor);
        }

        private void CameraDollying()
        {
            cameraDollyInput = Input.GetAxis("CameraDolly") * cameraDollyWheelSensitivity + Input.GetAxis("CameraDollyKeys") * cameraDollyKeySensitivity;

            cameraTargetDolly = Mathf.Clamp(cameraTargetDolly - cameraDollyInput * cameraDollyMultiplier * cameraTargetDolly * Time.deltaTime, cameraDollyClampMin, cameraDollyClampMax);

            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, Mathf.Lerp(mainCamera.transform.localPosition.z, cameraTargetDolly, cameraDollyLerpFactor));
        }

        private void CameraTilting()
        {
            cameraTiltTarget = Mathf.Clamp(cameraTiltTarget + Input.GetAxis("CameraTilt") * cameraTiltSpeed * Time.deltaTime, cameraTiltClampMin, cameraTiltClampMax);

            cameraTilter.localRotation = Quaternion.Lerp(cameraTilter.localRotation, Quaternion.Euler(cameraTiltTarget, 0f, 0f), cameraTiltLerpFactor);
        }
    }
}