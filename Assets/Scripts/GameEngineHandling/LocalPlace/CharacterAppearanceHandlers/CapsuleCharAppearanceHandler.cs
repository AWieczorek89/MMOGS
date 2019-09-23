using BackgroundManagement.Interfaces;
using UnityEngine;

public class CapsuleCharAppearanceHandler : MonoBehaviour, ICharacterAppearanceHandler
{
    public GameObject _hair01;
    public GameObject _hair02;

    public int GetMaxHairstyleId()
    {
        return 2;
    }

    public void SetHairstyle(int hairstyleId)
    {
        _hair01.SetActive(hairstyleId == 1);
        _hair02.SetActive(hairstyleId == 2);
    }
}
