using BackgroundManagement.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerPositionMarkHandler : MonoBehaviour
{
    public GameObject _renderSectionObject;
    public float _correctionDistance = 2f;
    public int _correctionTime = 2;

    private IChat _chat;
    private bool _isWorking = false;
    private GameObject _playerCharacter = null;
    private Vector3 _oldPosition = Vector3.zero;
    private Vector3 _newPosition = Vector3.zero;
    private float _timeArrivalTotal = 0f;
    private float _timeArrivalCounter = 0f;

    private float _currentDistance = 0f;
    private int _distanceCorrectionTimeCounter = 0;
    
    private void Start()
    {
        _chat = MainGameHandler.GetChatHandler();
        _isWorking = true;
        HandlePositionCorrectionAsync();
    }

    private void OnDestroy()
    {
        _isWorking = false;
    }

    private void Update ()
    {
        HandleMovement();
    }

    public void SetPlayerCharacter(GameObject playerCharacter)
    {
        _playerCharacter = playerCharacter;
    }

    public void MoveMark(Vector3 pointTo, int timeArrivalMs)
    {
        //Debug.Log($"mark [{pointTo.x}; {pointTo.y}; {pointTo.z}]");
        _oldPosition = this.gameObject.transform.position;
        _newPosition = pointTo;
        _timeArrivalTotal = Convert.ToSingle(timeArrivalMs) / 1000f;
        _timeArrivalCounter = 0f;
    }

    private async void HandlePositionCorrectionAsync()
    {
        try
        {
            while (true)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(1000));

                if (_playerCharacter == null || _timeArrivalTotal > 0f)
                    continue;

                if (this == null || this.gameObject == null || !_isWorking)
                    break;

                _currentDistance = Vector3.Distance(this.gameObject.transform.position, _playerCharacter.transform.position);

                if (_currentDistance >= _correctionDistance)
                {
                    _distanceCorrectionTimeCounter++;
                }
                else
                {
                    _distanceCorrectionTimeCounter = 0;
                }

                if (_distanceCorrectionTimeCounter >= _correctionTime)
                {
                    LocalCharacterHandler charHandler = _playerCharacter.GetComponent<LocalCharacterHandler>();

                    charHandler.UpdateMovement
                    (
                        LocalCharacterHandler.MovementType.Idle,
                        this.gameObject.transform.position,
                        charHandler.CurrentAngle,
                        0,
                        false
                    );

                    _distanceCorrectionTimeCounter = 0;
                }
            }
        }
        catch (Exception exception)
        {
            _chat.UpdateLog($"Position mark handler - position correction handling error: {exception.Message}");
        }
    }
    
    private void HandleMovement()
    {
        if (_timeArrivalTotal <= 0f)
            return;

        _timeArrivalCounter += Time.deltaTime;
        float tParam = _timeArrivalCounter / _timeArrivalTotal;

        if (tParam < 1)
        {
            this.gameObject.transform.position = new Vector3
            (
                Mathf.Lerp(_oldPosition.x, _newPosition.x, tParam),
                Mathf.Lerp(_oldPosition.y, _newPosition.y, tParam),
                Mathf.Lerp(_oldPosition.z, _newPosition.z, tParam)
            );
        }
        else
        {
            this.gameObject.transform.position = _newPosition;
            _timeArrivalTotal = 0f;
        }
    }
}
