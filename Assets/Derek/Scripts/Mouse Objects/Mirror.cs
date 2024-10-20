using UnityEngine;

[AddComponentMenu("Mouse Objects/Mirror")]
public class Mirror : MouseObject
{
    [SerializeField] private LayerMask _mirrorVisible;

    public override void OnStartWithMouse(GameObject mouse)
    {
        mouse.layer = _mirrorVisible;
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
        mouse.layer = 0;
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
