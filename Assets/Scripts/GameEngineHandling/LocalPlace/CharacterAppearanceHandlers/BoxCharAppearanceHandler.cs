using BackgroundManagement.Interfaces;
using UnityEngine;

public class BoxCharAppearanceHandler : MonoBehaviour, ICharacterAppearanceHandler
{
    public int GetMaxHairstyleId()
    {
        return 0;
    }

    public void SetHairstyle(int hairstyleId)
    {
        IChat chat = MainGameHandler.GetChatHandler();
        chat.UpdateLog($"cannot set hairstyle ID [{hairstyleId}] for box appearance!");
    }
}
