using UnityEngine;

public class SendInteractionToParent : MonoBehaviour, IInteractable
{
    public void OnInteract(GameObject player)
    {
        foreach (var interact in GetComponentsInParent<IInteractable>())
        {
            if (interact is not null && interact != this)
            {
                interact.OnInteract(player);
                break;
            }
        }
    }
}
