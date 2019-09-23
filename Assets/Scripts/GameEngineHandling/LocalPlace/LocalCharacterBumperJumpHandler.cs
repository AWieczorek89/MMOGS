using BackgroundManagement.Measurement.Units;
using System;
using UnityEngine;

public class LocalCharacterBumperJumpHandler : MonoBehaviour
{
    public GameObject _parentObject;
    public float _jumpForce = 300f;

    private Rigidbody _parentRigidbody = null;
    private Point2<int> _lastJumpingPosition = new Point2<int>(-1, -1);
    private float _lastJumpingPosCheckTimer = 0f;

    void Start ()
    {
        _parentRigidbody = _parentObject.GetComponent<Rigidbody>();
    }
	
	void Update ()
    {
        CheckLastJumpingPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
            return;
        
        if (other.gameObject.tag.Equals("Ground"))
        {
            Jump();
        }
    }

    private void CheckLastJumpingPosition()
    {
        _lastJumpingPosCheckTimer += Time.deltaTime;
        if (_lastJumpingPosCheckTimer > 0.5f)
            return;

        _lastJumpingPosCheckTimer = 0f;
        int posX, posZ;
        GetCurrentPos(out posX, out posZ);

        if (_lastJumpingPosition.X == -1 && _lastJumpingPosition.Y == -1)
            return;

        if (posX != _lastJumpingPosition.X || posZ != _lastJumpingPosition.Y)
            _lastJumpingPosition = new Point2<int>(-1, -1);
    }

    private void Jump()
    {
        int posX, posZ;
        GetCurrentPos(out posX, out posZ);

        if (posX == _lastJumpingPosition.X && posZ == _lastJumpingPosition.Y)
            return;

        _lastJumpingPosition = new Point2<int>(posX, posZ);
        _parentRigidbody.AddForce(new Vector3(0f, _jumpForce, 0f));
    }

    private void GetCurrentPos(out int posX, out int posZ)
    {
        posX = Convert.ToInt32(_parentObject.transform.position.x);
        posZ = Convert.ToInt32(_parentObject.transform.position.z);
    }
}
