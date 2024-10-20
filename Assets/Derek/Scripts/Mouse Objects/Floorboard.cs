using UnityEngine;

[AddComponentMenu("Mouse Objects/Floorboard")]
public class Floorboard : MouseObject
{
    [SerializeField] private float _rotation;

    public override void OnStartWithMouse(GameObject mouse)
    {
        //transform.rotation
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
