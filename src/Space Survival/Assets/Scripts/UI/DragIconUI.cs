using UnityEngine;
using UnityEngine.UI;

public class DragIconUI : MonoBehaviour
{
    static RawImage icon;
    static bool isFollowingCursor = false;

    void Awake()
    {
        icon = GetComponent<RawImage>();
    }

    void OnEnable()
    {
        PlayerInventory.OnItemChange += ResetIcon;
    }

    void OnDisable()
    {
        PlayerInventory.OnItemChange -= ResetIcon;
        icon = null;
        isFollowingCursor = false;
    }

    void OnDestroy()
    {
        PlayerInventory.OnItemChange -= ResetIcon;
        icon = null;
        isFollowingCursor = false;
    }

    void Update()
    {
        //Moves icon to the position of the mouse in the inventory
        if (isFollowingCursor) {
            transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Set the icon to display an item that follows the mouse
    /// </summary>
    /// <param name="_newIcon">The icon to display</param>
    public static void SetIcon(Texture _newIcon)
    {
        icon.texture = _newIcon;
        icon.enabled = _newIcon != null;
    }

    /// <summary>
    /// Set the icon to follow the mouse cursor
    /// </summary>
    /// <param name="_state">If true, the icon will follow the mouse</param>
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
