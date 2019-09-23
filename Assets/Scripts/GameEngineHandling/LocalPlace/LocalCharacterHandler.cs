using BackgroundManagement.DataHandlers.CommandBuilding;
using BackgroundManagement.DataHandlers.CommandHandling;
using BackgroundManagement.Interfaces;
using BackgroundManagement.Measurement.Units;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using System;
using UnityEngine;

public class LocalCharacterHandler : MonoBehaviour
{
    public enum MovementType
    {
        Idle,
        Moving
    }
    
    public GameObject _nameBilboardText;
    public GameObject _appearanceBaseObject;
    public GameObject _bumperObject;
    public Rigidbody _rigidbody;

    public Camera CameraInstance { get; set; } = null;
    public bool IsPlayerMainCharacter { get; private set; } = false;
    public float MovingVelocity { get; set; } = 2f;
    public double CurrentAngle { get; set; } = 0f;
    
    private LocalCharacterDetails _details = null;
    private ICharacterAppearanceHandler _appearanceHandler = null;
    private bool _bilboardTextDisabled = false;

    //movement variables
    private float _characterMovementSpeed = 1f;
    private MovementType _movementType = MovementType.Idle;
    private Vector3 _movingStartingPoint = new Vector3(0f, 0f, 0f);
    private Vector3 _movingDestinationPoint = new Vector3(0f, 0f, 0f);
    private int _timeArrivalMs = 0;
    private float _totalDistance = 0;
    
    private void Start()
    {
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleBilboardText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("BoundingBox"))
        {
            UpdateMovement(MovementType.Idle, _movingDestinationPoint, this.CurrentAngle, 0, false);
        }
    }

    private void HandleBilboardText()
    {
        if (!_bilboardTextDisabled)
        {
            _nameBilboardText.transform.LookAt
            (
                _nameBilboardText.transform.position + this.CameraInstance.transform.rotation * Vector3.forward,
                this.CameraInstance.transform.rotation * Vector3.up
            );
        }
    }

    private void SetBilboardTextInformation()
    {
        TextMesh bilboardTextMesh = _nameBilboardText.GetComponent<TextMesh>();
        bilboardTextMesh.text = _details.Name;

        if (this.IsPlayerMainCharacter)
        {
            _nameBilboardText.SetActive(false);
            _bilboardTextDisabled = true;
        }
        else
        {
            //_bumperObject.SetActive(false);
            _bumperObject.GetComponent<LocalCharacterBumperHandler>().enabled = false;
        }
    }

    public void Set (bool isPlayerMainCharacter, LocalCharacterDetails details, ICharacterAppearanceHandler appearanceHandler)
    {
        this.IsPlayerMainCharacter = isPlayerMainCharacter;
        _details = details;
        _appearanceHandler = appearanceHandler;
        SetBilboardTextInformation();
        _appearanceHandler.SetHairstyle(_details.HairstyleId);
    }
    
    public LocalCharacterDetails GetDetails()
    {
        return (LocalCharacterDetails)_details.Clone();
    }

    private void HandleMovement()
    {
        if (_movementType != MovementType.Moving)
            return;

        float _deltaTime = Time.fixedDeltaTime;

        _totalDistance += _deltaTime * _characterMovementSpeed;
        if (_totalDistance >= (Vector3.Distance(_movingStartingPoint, _movingDestinationPoint)))
        {
            this.gameObject.transform.position = new Vector3
            (
                _movingDestinationPoint.x,
                this.gameObject.transform.position.y, //no vertical movement
                _movingDestinationPoint.z
            );
            
            _rigidbody.velocity = Vector3.zero;
            _movementType = MovementType.Idle;
        }
        
        Vector3 directionVector = Vector3.Normalize(_movingDestinationPoint - _movingStartingPoint);
        directionVector = new Vector3 (directionVector.x, 0f, directionVector.z); //no vertical movement
        _rigidbody.MovePosition(transform.position + (directionVector * _characterMovementSpeed * _deltaTime));
    }

    /// <summary>
    /// Updates character movement in server coords orientation (Point3 - XYZ) and converts them to game engine orientation (Vector3 - XZY)
    /// </summary>
    public void UpdateMovement(MovementType type, /*Point3<double> startingPoint,*/ Point3<double> destinationPoint, double angle, int timeArrivalMs, bool sendRequestToServer)
    {
        UpdateMovement
        (
            type,
            //new Vector3
            //(
            //    Convert.ToSingle(startingPoint.X),
            //    Convert.ToSingle(startingPoint.Z),
            //    Convert.ToSingle(startingPoint.Y)
            //),
            new Vector3
            (
                Convert.ToSingle(destinationPoint.X),
                Convert.ToSingle(destinationPoint.Z),
                Convert.ToSingle(destinationPoint.Y)
            ),
            angle,
            timeArrivalMs,
            sendRequestToServer
        );
    }

    /// <summary>
    /// Updates character movement in game engine coords orientation (Vector3 - XZY)
    /// </summary>
    public void UpdateMovement(MovementType type, /*Vector3 startingPoint,*/ Vector3 destinationPoint, double angle, int timeArrivalMs, bool sendRequestToServer)
    {
        try
        {
            _movementType = type;
            Vector3 startingPoint = this.gameObject.transform.position;

            if (timeArrivalMs > 0 && type == MovementType.Moving)
            { _characterMovementSpeed = Vector3.Distance(startingPoint, destinationPoint) / (Convert.ToSingle(timeArrivalMs) / 1000f); }
            else { _characterMovementSpeed = 1f; }
            
            _timeArrivalMs = timeArrivalMs;
            _totalDistance = 0f;

            _movingStartingPoint = startingPoint;
            _movingDestinationPoint = destinationPoint;

            SetCharAppAngle(angle);

            if (type == MovementType.Idle)
            {
                this.gameObject.transform.position = _movingDestinationPoint;
                
                _rigidbody.velocity = Vector3.zero;
            }

            if (sendRequestToServer)
            {
                CommandHandler.Send(new MoveCharRequestCmdBuilder(destinationPoint.x, destinationPoint.z, destinationPoint.y, timeArrivalMs));
                //Debug.Log($"cmd sent, pos [{destinationPoint.x}, {destinationPoint.z}, {destinationPoint.y}] ms [{timeArrivalMs}]");
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"UpdateMovement(): {exception.Message} | {exception.StackTrace}");
        }
    }

    public void SetCharAppAngle(double angle)
    {
        _bumperObject.transform.eulerAngles = new Vector3(-90f, Convert.ToSingle(angle) - 90f, 0f);

        foreach (Transform childTransform in _appearanceBaseObject.transform)
        {
            childTransform.eulerAngles = new Vector3(0f, Convert.ToSingle(angle), 0f);
        }

        this.CurrentAngle = angle;
    }
}
