using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private bool useGamepad;

    [SerializeField] private InputManager.GeneralInput upInput;
    [SerializeField] private InputManager.GeneralInput downInput;
    [SerializeField] private InputManager.GeneralInput rightInput;
    [SerializeField] private InputManager.GeneralInput leftInput;
    [SerializeField] private InputManager.GeneralInput sprintInput;
    [SerializeField] private InputManager.GeneralInput torchInput;

    private ControllerType _controllerType = ControllerType.Keyboard;
    public ControllerType controllerType
    {
        get => _controllerType;
        set
        {
            _controllerType = value;
            upInput.controllerType = value;
            downInput.controllerType = value;
            rightInput.controllerType = value;
            leftInput.controllerType = value;
            sprintInput.controllerType = value;
            torchInput.controllerType = value;
        }
    }

    public bool isSprintUp => sprintInput.IsPressedUp();
    public bool isTorchUp => torchInput.IsPressedUp();
    public bool isSprintDown => sprintInput.IsPressedDown();
    public bool isTorchDown => torchInput.IsPressedDown();
    public bool isSprintPressed => sprintInput.IsPressed();
    public bool isTorchPressed => torchInput.IsPressed();

    [HideInInspector] public float x, y;
    [HideInInspector] public int rawX, rawY;

    private void Start()
    {
        this.controllerType = useGamepad ? ControllerType.Gamepad1 : ControllerType.Keyboard;
    }

    private void Update()
    {
        if(controllerType == ControllerType.Keyboard && !useGamepad)
        {
            if (rightInput.IsPressed())
            {
                x = leftInput.IsPressed() ? 0f : 1f;
            }
            else
            {
                x = leftInput.IsPressed() ? -1f : 0f;
            }

            if (upInput.IsPressed())
            {
                y = downInput.IsPressed() ? 0f : 1f;
            }
            else
            {
                y = downInput.IsPressed() ? -1f : 0f;
            }
        }
        else
        {
            Vector2 tmbStickPosition = InputManager.GetGamepadStickPosition(controllerType, GamepadStick.left).normalized;
            x = tmbStickPosition.x;
            y = tmbStickPosition.y;
        }

        rawX = Mathf.Abs(x) >= 0.1f ? (x > 0f ? 1 : - 1) : 0;
        rawY = Mathf.Abs(y) >= 0.1f ? (y > 0f ? 1 : - 1) : 0;
    }
}
