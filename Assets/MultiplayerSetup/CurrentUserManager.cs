using UnityEngine;

public class CurrentUserManager : MonoBehaviour
{
    public static CurrentUserManager Instance { get; private set; }

    public string CurrentUserId { get; private set; }
    public string CurrentUsername { get; private set; }

    private void Awake()
    {  
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void SetCurrentUser(string userId)
    {
        CurrentUserId = userId;
    }

    public string GetCurrentUserId()
    {
        return CurrentUserId;
    }
    public void SetCurrentUsername(string userName)
    {
        CurrentUsername = userName;
    }

    public string GetCurrentUsername() 
    {
        return CurrentUsername;
    }
}
