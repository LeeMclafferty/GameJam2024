using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Mouse Objects/Floorboard")]
public class Floorboard : MouseObject
{
    [SerializeField] private GameObject _floorboard;

    [SerializeField] private Vector2Int _size;
    [SerializeField] private Vector2 _spacing;
    [SerializeField] private Vector2 _alternatingOffset;

    [SerializeField] private float _mouseBoardRotation;

    private readonly Stack<GameObject> _currentFloorboards = new();
    private GameObject _mouseFloorboard;

    public override void OnStartWithMouse(GameObject mouse)
    {

    }

    public override void OnStartWithoutMouse()
    {

    }

    public override void OnUpdateWithMouse(GameObject mouse)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = _mouseBoardRotation;

        Vector3 rotation2 = transform.rotation.eulerAngles;
        rotation2.z = -_mouseBoardRotation;

        _mouseFloorboard.transform.rotation = Quaternion.Lerp(Quaternion.Euler(rotation), Quaternion.Euler(rotation2), Mathf.Sin(Time.time));
    }

    public override void OnUpdateWithoutMouse()
    {

    }

    public override void OnEndWithMouse(GameObject mouse)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = 0;

        _mouseFloorboard.transform.rotation = Quaternion.Euler(rotation);
    }

    public override void OnEndWithoutMouse()
    {

    }

    public override void OnSetup()
    {
        if (_currentFloorboards.Count > 0)
            return;

        int randomIndex = Random.Range(0, _size.x * _size.y);

        for (int x = 0; x < _size.x; ++x)
        {
            for (int y = 0; y < _size.y; ++y)
            {
                GameObject floorboard = Instantiate(_floorboard, transform, false);
                _currentFloorboards.Push(floorboard);

                floorboard.transform.position += new Vector3(x * _spacing.x, 0, y * _spacing.y);

                if (y % 2 == 0)
                    floorboard.transform.position += (Vector3)_alternatingOffset;

                if (y * _size.x + x == randomIndex)
                {
                    _mouseFloorboard = floorboard;
                }
            }
        }
    }

    public override void OnClear()
    {
        while (_currentFloorboards.Count > 0)
        {
            GameObject floorboard = _currentFloorboards.Pop();
            Destroy(floorboard);
        }
    }
}
