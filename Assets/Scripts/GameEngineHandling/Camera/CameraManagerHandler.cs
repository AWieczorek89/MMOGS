using UnityEngine;

public class CameraManagerHandler : MonoBehaviour
{
    public GameObject _anchor;
    
    void LateUpdate()
    {
        MoveAnchoredCamera();
    }

    private void MoveAnchoredCamera()
    {
        if (_anchor == null)
            return;
        
        this.gameObject.transform.position = new Vector3
        (
            _anchor.transform.position.x,
            _anchor.transform.position.y,
            _anchor.transform.position.z
        );
    }
}
