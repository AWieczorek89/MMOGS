using System;
using UnityEngine;

public class WorldMapSectionHandler : MonoBehaviour
{
    public GameObject _bilboardText;

    public Camera CameraInstance { get; set; } = null;

    public int PositionX { get; private set; } = 0;
    public int PositionY { get; private set; } = 0;
    public int WmId { get; private set; } = -1;
    public string PlaceName { get; private set; } = "";

    private bool _bilboardTextDisabled = true;

    void LateUpdate()
    {
        if (!_bilboardTextDisabled)
        {
            _bilboardText.transform.LookAt
            (
                _bilboardText.transform.position + this.CameraInstance.transform.rotation * Vector3.forward, 
                this.CameraInstance.transform.rotation * Vector3.up    
            );
        }
    }

    public void SetPlaceInformation(int positionX, int positionY, int wmId, string placeName = "")
    {
        this.PositionX = positionX;
        this.PositionY = positionY;
        this.WmId = wmId;
        this.PlaceName = placeName;

        if (!String.IsNullOrWhiteSpace(placeName))
        {
            TextMesh bilboardTextMesh = _bilboardText.GetComponent<TextMesh>();
            bilboardTextMesh.text = placeName;
            _bilboardTextDisabled = false;
        }
        else
        {
            _bilboardText.SetActive(false);
        }
    }
}
