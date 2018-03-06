using UnityEngine;

public class CameraMovement : MonoBehaviour {
    /// <summary>
    /// Horizontal mouse sensitivity.
    /// </summary>
    public float SensitivityX = 100;

    /// <summary>
    /// Vertical mouse sensitivity.
    /// </summary>
    public float SensitivityY = 100;

    /// <summary>
    /// Vertically center looking on this key.
    /// </summary>
    public KeyCode CalibrationKey = KeyCode.Space;

    Vector3 StartOffset;

    void Awake() {
        StartOffset = transform.localPosition;
    }

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update() {
        Vector2 MouseDelta = Input.mousePosition;
        MouseDelta = new Vector2(Input.GetAxis("Mouse X") * SensitivityX, Input.GetAxis("Mouse Y") * SensitivityY) * Time.deltaTime;
        transform.parent.localEulerAngles = new Vector3(0, Input.GetKeyDown(CalibrationKey) ? transform.parent.localEulerAngles.y :
            transform.parent.localEulerAngles.y + MouseDelta.x, 0);
        float PanDown = transform.localEulerAngles.x - MouseDelta.y, PanSin = Mathf.Sin(PanDown * Mathf.Deg2Rad);
        transform.localPosition = new Vector3(0, StartOffset.y + PanSin * StartOffset.z, (PanSin + 1) * StartOffset.z);
        transform.localEulerAngles = new Vector3(PanDown, 0, 0);
    }
}