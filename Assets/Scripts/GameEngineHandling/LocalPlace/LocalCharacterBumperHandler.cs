using BackgroundManagement.Measurement;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LocalCharacterBumperHandler : MonoBehaviour
{
    public GameObject _parentObject;
    public float _collisionCheckInterval = 0.25f;
    
    private Rigidbody _parentRigidbody = null;
    private float _collisionCheckTimer = 0f;
    private float _checkpointX = 0f;
    private float _checkpointY = 0f;
    private float _checkpointZ = 0f;
    private bool _isCheckpointReached = false;
    private bool _isCollisionDetected = false;
    
    private void Start()
    {
        _parentRigidbody = _parentObject.GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        _collisionCheckTimer += Time.fixedDeltaTime;
        if (_collisionCheckTimer >= _collisionCheckInterval)
        {
            _collisionCheckTimer = 0f;
            CheckCollisionAsync();
        }
    }
    
    private async void CheckCollisionAsync()
    {
        await Task.Factory.StartNew(() => Thread.Sleep(100));

        if (this == null || this.gameObject == null || _parentObject == null)
            return;

        if (!_isCollisionDetected)
        {
            _checkpointX = _parentObject.transform.position.x;
            _checkpointY = _parentObject.transform.position.y;
            _checkpointZ = _parentObject.transform.position.z;
            _isCheckpointReached = true;
            return;
        }

        if (_isCheckpointReached)
        {
            LocalCharacterHandler handler = _parentObject.GetComponent<LocalCharacterHandler>();
            Vector3 pointTo = new Vector3(_checkpointX, _checkpointY, _checkpointZ);
            double angle = Measure.CalculateMovingAngle(_parentObject.transform.position, pointTo);

            handler.UpdateMovement
            (
                LocalCharacterHandler.MovementType.Moving,
                /*new Vector3(_checkpointX, _checkpointY, _checkpointZ), */
                pointTo,
                angle,
                0,
                true
            );
        }

        _isCollisionDetected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
            return;
        
        if (other.gameObject.tag.Equals("Obstacle") || other.gameObject.tag.Equals("Platform"))
        {
            _isCollisionDetected = true;
        }
    }
}
