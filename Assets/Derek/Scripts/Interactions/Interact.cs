using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private float _maxInteractDistance;
    [SerializeField] private LayerMask _interactLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxInteractDistance, _interactLayer) && hit.collider.gameObject.TryGetComponent(out IInteractable interact))
            {
                interact.OnInteract(gameObject);
            }
        }
    }
}
