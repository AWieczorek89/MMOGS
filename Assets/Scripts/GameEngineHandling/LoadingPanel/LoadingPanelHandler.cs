using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelHandler : MonoBehaviour
{
    public float _fadingTime = 1f;
    public Text[] _textElements;

    private float _fadingTimeCounter = 0f;
    private bool _fadingTrigger = false;
    
	void Update ()
    {
        HandleFading();
    }

    private void HandleFading()
    {
        if (!_fadingTrigger || _fadingTime <= 0f)
            return;

        _fadingTimeCounter += Time.deltaTime;
        
        float normalizedValue = 1f - (_fadingTimeCounter / _fadingTime);
        if (normalizedValue < 0f)
            normalizedValue = 0f;

        Image img = this.gameObject.GetComponent<Image>();
        
        img.color = new Color
        (
            img.color.r,
            img.color.g,
            img.color.b,
            normalizedValue
        );

        foreach (Text txtElement in _textElements)
        {
            txtElement.color = new Color
            (
                txtElement.color.r,
                txtElement.color.g,
                txtElement.color.b,
                normalizedValue
            );
        }
        
        if (normalizedValue <= 0f)
        {
            _fadingTrigger = false;
            Destroy(this.gameObject);
        }  
    }

    public void Fade()
    {
        _fadingTrigger = true;
    }


}
