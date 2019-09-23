using MMOC.BackgroundManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleFader : MonoBehaviour
{
    public Text[] _fadingTextObjects = null;

    public float _fadingAnimationTime = 5f;
    public float _blackScreenTime = 1f;
    
    private float _timer = 0f;
    private bool _enabled = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (MainGameHandler.CheckIfSceneActive(MainGameHandler.SceneType.Startup, scene))
        {
            _timer = 0f;
            _enabled = true;
        }
    }
    
	void Update()
    {
        if (_enabled)
        {
            _timer += Time.deltaTime;
            float tValue = 0;
            float dividend = 0;
            float divider = 0;

            if 
            (
                _timer > _blackScreenTime && 
                _timer <= (_blackScreenTime + (_fadingAnimationTime / 2))
            )
            {
                dividend = _timer - _blackScreenTime;
                divider = _fadingAnimationTime / 2;

                if (divider == 0) tValue = 0;
                else tValue = dividend / divider;

                SetAlphaOfElements(tValue);
            }
            else
            if
            (
                _timer > (_blackScreenTime + (_fadingAnimationTime / 2)) &&
                _timer <= (_blackScreenTime + _fadingAnimationTime)
            )
            {
                dividend = _timer - (_blackScreenTime + (_fadingAnimationTime / 2));
                divider = _fadingAnimationTime / 2;

                if (divider == 0) tValue = 0;
                else tValue = 1 - (dividend / divider);

                SetAlphaOfElements(tValue);
            }
            else
            if (_timer > (_blackScreenTime + _fadingAnimationTime))
            {
                SetAlphaOfElements(0);
            }

            if (_timer > ((_blackScreenTime * 2) + _fadingAnimationTime))
            {
                enabled = false;
                MainGameHandler.ChangeScene(MainGameHandler.SceneType.Login);
            }
        }
	}

    private void SetAlphaOfElements(float tValue)
    {
        if (_fadingTextObjects == null)
            return;

        tValue = Mathf.Clamp(tValue, 0f, 1f);
        Color color;
        
        for (int i = 0; i < _fadingTextObjects.Length; i++)
        {
            color = new Color
            (
                _fadingTextObjects[i].color.r,
                _fadingTextObjects[i].color.g,
                _fadingTextObjects[i].color.b,
                tValue
            );

            _fadingTextObjects[i].color = color;
        }
    }
}
