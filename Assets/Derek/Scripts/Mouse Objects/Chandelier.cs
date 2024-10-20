using UnityEngine;

[AddComponentMenu("Mouse Objects/Chandelier")]
public class Chandelier : MouseObject
{
    [SerializeField] private AnimationCurve _shakeX, _shakeY, _shakeZ;
    [SerializeField] private float _shakeSpeed;
    [SerializeField] private float _shakeFrequency;

    public override void OnStartWithMouse(GameObject mouse)
    {
        mouse.transform.position = transform.position;
    }

    public override void OnStartWithoutMouse()
    {

    }

    public override void OnUpdateWithMouse(GameObject mouse)
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.x = _shakeX.Evaluate(Mathf.Repeat(Time.time * _shakeSpeed, 1)) * _shakeFrequency;
        newRotation.y = _shakeY.Evaluate(Mathf.Repeat(Time.time * _shakeSpeed, 1)) * _shakeFrequency;
        newRotation.z = _shakeZ.Evaluate(Mathf.Repeat(Time.time * _shakeSpeed, 1)) * _shakeFrequency;

        transform.rotation = Quaternion.Euler(newRotation);
    }

    public override void OnUpdateWithoutMouse()
    {

    }

    public override void OnEndWithMouse(GameObject mouse)
    {

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
