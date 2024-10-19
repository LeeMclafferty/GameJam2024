using UnityEngine.AI;
using UnityEngine;

public class MouseControler : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    public NavMeshAgent agent;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))

{}          agent.SetDestination(hit.point);
        }
    }
}
