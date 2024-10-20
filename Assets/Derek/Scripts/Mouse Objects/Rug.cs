using UnityEngine;

[AddComponentMenu("Mouse Objects/Rug")]
public class Rug : MouseObject
{
    [SerializeField] private float _mouseHeight;
    [SerializeField] private float _mouseSpeed;
    [SerializeField] private float _mouseFollowDistance;
    [SerializeField] private float _mouseRadius;

    private MeshRenderer _renderer;
    private Vector2 _mousePosition;
    private Vector2 _mouseTargetPosition;

    protected override void Awake()
    {
        base.Awake();

        _renderer = GetComponent<MeshRenderer>();
    }

    public override void OnStartWithMouse(GameObject mouse)
    {

    }

    public override void OnStartWithoutMouse()
    {

    }

    public override void OnUpdateWithMouse(GameObject mouse)
    {
        _mousePosition = Vector2.Lerp(_mousePosition, _mouseTargetPosition, Time.deltaTime * _mouseSpeed);

        float minMax = -Mathf.Pow(_mouseRadius, -1) + 1;
        if (Vector2.Distance(_mousePosition, _mouseTargetPosition) < _mouseFollowDistance)
        {
            _mouseTargetPosition = new(Random.Range(-minMax, minMax), Random.Range(-minMax, minMax));
        }

        _renderer.material.SetFloat("_Mouse_Height", _mouseHeight);
        _renderer.material.SetVector("_Mouse_Position", _mousePosition);
        _renderer.material.SetFloat("_Mouse_Radius", _mouseRadius);
    }

    public override void OnUpdateWithoutMouse()
    {
        _renderer.material.SetFloat("_Mouse_Height", 0);
    }

    public override void OnEndWithMouse(GameObject mouse)
    {
        mouse.transform.position = transform.position;
    }

    public override void OnEndWithoutMouse()
    {

    }

    public override void OnSetup()
    {

    }

    public override void OnClear()
    {

    }
}
