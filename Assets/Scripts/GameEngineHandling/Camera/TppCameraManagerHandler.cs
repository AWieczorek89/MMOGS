using UnityEngine;

public class TppCameraManagerHandler : MonoBehaviour
{
    public float _cameraRotationSpeed = 1f;
    public float _cameraDistanceChangeSpeed = 0.1f;
    public Vector2 _cameraDistanceRange = new Vector2(-10f, -2f);
    public GameObject _cameraAnchor;
    public Camera _mainCamera;

    private Transform _anchor = null;

    void Update()
    {
        HandleCameraPosition();
    }

    void LateUpdate ()
    {
        HandleBasePosition();
    }

    public void SetAnchor(Transform anchorTransform)
    {
        _anchor = anchorTransform;
    }

    private void HandleBasePosition()
    {
        if (_anchor == null)
            return;

        this.gameObject.transform.position = new Vector3
        (
            _anchor.position.x,
            _anchor.position.y,
            _anchor.position.z
        );
    }
    
    private void HandleCameraPosition()
    {
        if (Input.GetMouseButton(1)) //right click
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            _cameraAnchor.transform.eulerAngles = new Vector3
            (
                _cameraAnchor.transform.eulerAngles.x,
                _cameraAnchor.transform.eulerAngles.y + (mouseInputX * _cameraRotationSpeed),
                _cameraAnchor.transform.eulerAngles.z
            );
        }

        if (Input.GetMouseButton(2)) //middle click
        {
            float mouseInputY = Input.GetAxis("Mouse Y");
            float newPosX = _mainCamera.transform.localPosition.x + (mouseInputY * _cameraDistanceChangeSpeed);
            
            _mainCamera.transform.localPosition = new Vector3
            (
                Mathf.Clamp(newPosX, _cameraDistanceRange.x, _cameraDistanceRange.y),
                _mainCamera.transform.localPosition.y,
                _mainCamera.transform.localPosition.z
            );
        }
    }
}
