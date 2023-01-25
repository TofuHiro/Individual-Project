using UnityEngine;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour
{
    static RawImage icon;
    static bool isFollowingCursor = false;

    void Awake()
    {
        icon = GetComponent<RawImage>();
    }

    void OnEnable()
    {
        PlayerInventory.OnInventoryClose += ResetIcon;
    }

    void OnDisable()
    {
        PlayerInventory.OnInventoryClose -= ResetIcon;
    }

    void Update()
    {
        if (isFollowingCursor) {
            transform.position = Input.mousePosition;
        }
    }

    public static void SetIcon(Texture _newIcon)
    {
        icon.texture = _newIcon;
        icon.enabled = _newIcon != null;
    }

    public static void FollowCursor(bool _state)
    {
        isFollowingCursor = _state;
    }
    
    void ResetIcon()
    {
        SetIcon(null);
        FollowCursor(false);
    }
}
