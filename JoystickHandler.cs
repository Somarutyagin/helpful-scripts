using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class JoystickHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _joystickBackground;
    [SerializeField] private Image _joystickArea;
    [SerializeField] private Image _joystick;

    private Vector2 _joystickBackgroundStartPosition;
    private Camera _camera;

    protected Vector2 _inputVector;
    protected Platform _platform;

    [SerializeField] private Color _activeJoystick;
    [SerializeField] private Color _activeJoystickBackground;
    [SerializeField] private Color _inActiveJoystick;
    [SerializeField] private Color _inActiveJoystickBackground;

    private bool _isActiveJoystick = true;

    public void Initialize()
    {
        ClickEffectSwitch();

        _joystickBackgroundStartPosition = _joystickBackground.rectTransform.anchoredPosition;
        _camera = Camera.main;
#if UNITY_IOS
        _platform = Platform.IOS;
#elif UNITY_WebGL
        _platform = Platform.WebGL;
#elif UNITY_Android
        _platform = Platform.Android;
#else
        _platform = Platform.Windows;
#endif

        if (_platform == Platform.Android || _platform == Platform.IOS)
            _joystickArea.gameObject.SetActive(true);
        else
            _joystickArea.gameObject.SetActive(false);
    }

    private void ClickEffectSwitch()
    {
        if (_isActiveJoystick)
        {
            _joystick.color = _inActiveJoystick;
            _joystickBackground.color = _inActiveJoystickBackground;
            _isActiveJoystick = false;
        }
        else
        {
            _joystick.color = _activeJoystick;
            _joystickBackground.color = _activeJoystickBackground;
            _isActiveJoystick = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBackground.rectTransform, eventData.position, _camera, out Vector2 joystickPosition))
        {
            joystickPosition.x = (joystickPosition.x * 2 / _joystickBackground.rectTransform.sizeDelta.x);
            joystickPosition.y = (joystickPosition.y * 2 / _joystickBackground.rectTransform.sizeDelta.y);

            _inputVector = joystickPosition;

            _inputVector = (_inputVector.magnitude > 1f) ? _inputVector.normalized : _inputVector;

            _joystick.rectTransform.anchoredPosition = new Vector2(_inputVector.x * (_joystickBackground.rectTransform.sizeDelta.x / 2), _inputVector.y * (_joystickBackground.rectTransform.sizeDelta.y / 2));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClickEffectSwitch();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickArea.rectTransform, eventData.position, _camera, out Vector2 joystickBackgroundPosition))
            _joystickBackground.rectTransform.anchoredPosition = joystickBackgroundPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ClickEffectSwitch();

        _joystickBackground.rectTransform.anchoredPosition = _joystickBackgroundStartPosition;

        _inputVector = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
    }
}
