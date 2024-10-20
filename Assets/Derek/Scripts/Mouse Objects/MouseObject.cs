using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public abstract class MouseObject : MonoBehaviour, IInteractable, ISetup
{
    [SerializeField] private UnityEvent<GameObject> _onInteract;

    private bool _hasMouseCurrently;
    private GameObject _mouse;

    private Animator _anim;

    public void AddMouse()
    {
        if (_hasMouseCurrently || _mouse)
            return;

        _hasMouseCurrently = true;
        _mouse = Instantiate(MouseObjectManager.Instance.MousePrefab);
        _mouse.SetActive(false);

        OnEndWithoutMouse();
        OnStartWithMouse(_mouse);
    }

    public void RemoveMouse()
    {
        if (!_hasMouseCurrently || !_mouse)
            return;

        OnEndWithMouse(_mouse);
        OnStartWithoutMouse();

        _hasMouseCurrently = false;
        _mouse = null;
    }
    
    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        MouseObjectManager.Instance.AllMouseObjects.Add(this);
    }

    private void Update()
    {
        if (_hasMouseCurrently)
        {
            OnUpdateWithMouse(_mouse);
        }
        else
        {
            OnUpdateWithoutMouse();
        }
    }

    public abstract void OnStartWithMouse(GameObject mouse);
    public abstract void OnUpdateWithMouse(GameObject mouse);
    public abstract void OnEndWithMouse(GameObject mouse);

    public abstract void OnStartWithoutMouse();
    public abstract void OnUpdateWithoutMouse();
    public abstract void OnEndWithoutMouse();

    public void OnInteract(GameObject player)
    {
        Debug.Log($"{name} was interacted with by {player.name}");

        _anim.SetTrigger("Interact");
        _onInteract.Invoke(player);

        if (_hasMouseCurrently)
        {
            _mouse.SetActive(true);
            RemoveMouse();
        }
    }

    public abstract void OnSetup();
    public abstract void OnClear();
}
