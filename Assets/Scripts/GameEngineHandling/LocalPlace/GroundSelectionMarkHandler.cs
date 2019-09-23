using UnityEngine;

public class GroundSelectionMarkHandler : MonoBehaviour
{
    public float _existingTime = 2f;
    private float _timeCounter = 0f;

	void Start ()
    {
        _timeCounter = 0f;
	}
	
	void Update ()
    {
        _timeCounter += Time.deltaTime;
        if (_timeCounter >= _existingTime)
            Destroy(this.gameObject);
	}
}
