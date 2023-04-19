using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    [Tooltip("The position to teleport the player to upon using the hatch")]
    [SerializeField] Transform teleportPosition;

    PlayerController player;
   
    public InteractionType GetInteractionType()
    {
        return InteractionType.Door;
    }

    void Start()
    {
        player = PlayerController.Instance;
    }

    public void Interact()
    {
        player.SetPlayerPosition(teleportPosition.position);
    }
}
