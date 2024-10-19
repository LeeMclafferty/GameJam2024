using UnityEngine;

[AddComponentMenu("Mouse Objects/Chandelier")]
public class Chandelier : MouseObject
{
    [SerializeField] private AnimationCurve _shakeX, _shakeY, _shakeZ;
    [SerializeField] private float _shakeSpeed;
    [SerializeField] private float _shakeFrequency;

    public override void OnStartWithMouse(GameObject mouse)
    {

    }

    public override void OnStartWithoutMouse()
    {

    }

    public override void OnUpdateWithMouse(GameObject mouse)
    {

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
