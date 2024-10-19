using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Mouse Objects/Bookshelf")]
public class Bookshelf : MouseObject
{
    [SerializeField] private GameObject[] _bookTypes;
    [SerializeField] private Transform[] _bookPositions;
    [SerializeField] private Range<int> _bookCount;
    [SerializeField] private Range<Vector3> _bookPositionOffset;
    [SerializeField] private Range<Vector3> _bookRotation;
    [SerializeField] private Range<Vector3> _bookScale;

    [SerializeField] private AnimationCurve _mouseBookRotation;
    [SerializeField] private float _mouseBookRotationSpeed;
    [SerializeField] private float _mouseBookRotationFrequency;

    private readonly Stack<GameObject> _currentBooks = new();
    private GameObject _mouseBook;

    public override void OnStartWithMouse(GameObject mouse)
    {
        _mouseBook = _currentBooks.ElementAt(Random.Range(0, _currentBooks.Count));
        mouse.transform.position = _mouseBook.transform.position;
    }

    public override void OnStartWithoutMouse()
    {

    }

    public override void OnUpdateWithMouse(GameObject mouse)
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.z = _mouseBookRotation.Evaluate(Mathf.Repeat(Time.time * _mouseBookRotationSpeed, 1)) * _mouseBookRotationFrequency;

        _mouseBook.transform.rotation = Quaternion.Euler(newRotation);
    }

    public override void OnUpdateWithoutMouse()
    {

    }

    public override void OnEndWithMouse(GameObject mouse)
    {
        _mouseBook = null;
    }

    public override void OnEndWithoutMouse()
    {

    }

    [ContextMenu("Setup")]
    public override void OnSetup()
    {
        if (_currentBooks.Count > 0)
            return;

        var randomPositions = _bookPositions.OrderBy(_ => Random.Range(-1, 2)).Take(Random.Range(_bookCount.Min, _bookCount.Max + 1));

        foreach (var transform in randomPositions)
        {
            GameObject book = Instantiate(_bookTypes[Random.Range(0, _bookTypes.Length)], transform, false);

            book.transform.position += new Vector3(Random.Range(_bookPositionOffset.Min.x, _bookPositionOffset.Max.x), Random.Range(_bookPositionOffset.Min.y, _bookPositionOffset.Max.y), Random.Range(_bookPositionOffset.Min.z, _bookPositionOffset.Max.z));
            book.transform.localRotation = Quaternion.Euler(Random.Range(_bookRotation.Min.x, _bookRotation.Max.x), Random.Range(_bookRotation.Min.y, _bookRotation.Max.y), Random.Range(_bookRotation.Min.z, _bookRotation.Max.z));
            book.transform.localScale = new Vector3(Random.Range(_bookScale.Min.x, _bookScale.Max.x), Random.Range(_bookScale.Min.y, _bookScale.Max.y), Random.Range(_bookScale.Min.z, _bookScale.Max.z));

            _currentBooks.Push(book);
        }
    }

    [ContextMenu("Clear")]
    public override void OnClear()
    {
        while (_currentBooks.Count > 0)
        {
            GameObject book = _currentBooks.Pop();
            Destroy(book);
        }
    }
}
