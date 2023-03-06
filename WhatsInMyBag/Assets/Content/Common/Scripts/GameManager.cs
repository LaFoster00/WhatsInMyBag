using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool lockCursor = true;
    
    [HideInInspector] public PlayerInput playerInput; 
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance && Instance != this)
        {
            DestroyImmediate(this);
            return;
        }
        transform.parent = null;
        
        DontDestroyOnLoad(gameObject);
        
        Instance = this;

        if (lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
