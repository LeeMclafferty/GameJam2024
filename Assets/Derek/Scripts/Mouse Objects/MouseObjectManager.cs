using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MouseObjectManager : MonoBehaviour
{
    private static MouseObjectManager _instance;
    public static MouseObjectManager Instance => _instance;

    [SerializeField] private GameObject _mousePrefab;
    public GameObject MousePrefab => _mousePrefab;

    [SerializeField] private Range<int> _mouseCount;

    private readonly List<MouseObject> _allMouseObjects = new();
    public List<MouseObject> AllMouseObjects => _allMouseObjects;

    private readonly Stack<MouseObject> _currentMouseObjects = new();

    [SerializeField] private UnityEvent<string> _onCollectMouse;
    [SerializeField] private string _format;

    private int _currentMouseCount;
    public int CurrentMouseCount => _currentMouseCount;
    public void ModifyCurrentMouseCount(int amount)
    {
        _currentMouseCount += amount;

        _onCollectMouse.Invoke(string.Format(_format, _maxMouseCount - _currentMouseCount, _maxMouseCount));
    }

    private int _maxMouseCount;
    public int MaxMouseCount => _maxMouseCount;

    private void Awake()
    {
        _instance = this;
    }

    [ContextMenu("Setup")]
    public void SetupAllMouseObjects()
    {
        foreach (var mouseObject in AllMouseObjects)
        {
            mouseObject.OnSetup();
        }
    }

    [ContextMenu("Clear")]
    public void ClearAllMouseObjects()
    {
        foreach (var mouseObject in AllMouseObjects)
        {
            mouseObject.OnClear();
        }
    }

    [ContextMenu("Add Mice")]
    public void AddAllMice()
    {
        if (_currentMouseObjects.Count > 0)
            return;

        int mouseCount = Random.Range(_mouseCount.Min, _mouseCount.Max + 1);
        var randomMouseObjects = _allMouseObjects.OrderBy(_ => Random.Range(-1, 2)).Take(mouseCount);

        _maxMouseCount = mouseCount;

        foreach (var mouseObject in randomMouseObjects)
        {
            Debug.Log($"Mouse is hiding in {mouseObject.name}");

            mouseObject.AddMouse();
            _currentMouseObjects.Push(mouseObject);
        }
    }

    [ContextMenu("Remove Mice")]
    public void RemoveAllMice()
    {
        while (_currentMouseObjects.Count > 0)
        {
            MouseObject mouseObject = _currentMouseObjects.Pop();
            mouseObject.RemoveMouse();
        }

        _maxMouseCount = 0;
    }
}
