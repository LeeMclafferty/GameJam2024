using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseObjectManager : MonoBehaviour
{
    private static MouseObjectManager _instance;
    public static MouseObjectManager Instance => _instance;

    [SerializeField] private GameObject _mousePrefab;
    public GameObject MousePrefab => _mousePrefab;

    [SerializeField] private Range<int> _mouseCount;

    private List<MouseObject> _allMouseObjects = new();
    public List<MouseObject> AllMouseObjects => _allMouseObjects;

    private Stack<MouseObject> _currentMouseObjects = new();

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

        var randomMouseObjects = _allMouseObjects.OrderBy(_ => Random.Range(-1, 2)).Take(Random.Range(_mouseCount.Min, _mouseCount.Max + 1));

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
    }
}
