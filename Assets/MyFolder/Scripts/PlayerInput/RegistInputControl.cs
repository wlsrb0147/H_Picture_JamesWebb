using UnityEngine;
using UnityEngine.InputSystem;
using Debug = DebugEx;

public abstract class RegistInputControl : MonoBehaviour, InputControl
{
    protected GameObject Cam;
    private void Start()
    {
        RegisterControl();
        Cam = Camera.main.gameObject;
    }

    private void RegisterControl()
    {
        InputManager.Instance.SetInputControl(this); 
    }

    public virtual void SetCurrentInput(int page, int index)
    {
    }

    public virtual void ExecuteInput(Key key, bool performed)
    {
    }

    public virtual void ChangeIndex()
    {
    }
}
