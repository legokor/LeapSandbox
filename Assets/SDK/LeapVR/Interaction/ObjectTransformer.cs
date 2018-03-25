using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Move and rotate an object with hand gestures.
    /// </summary>
    [AddComponentMenu("Leap VR / Interaction / Object Transformer")]
    public class ObjectTransformer : MonoBehaviour {
        public Transform Target;

        [Tooltip("Sensitivity for movement (single hand grab).")]
        public float MoveSensitivity = .1f;
        [Tooltip("Sensitivity for rotation (dual hand grab).")]
        public float RotationSensitivity = .5f;

        /// <summary>
        /// Starting position of a movement, to cancel the movement if the gesture is a rotation.
        /// </summary>
        Vector3 MoveStart;

        /// <summary>
        /// Previous hand position for movement.
        /// </summary>
        Vector3 PrevPosition;

        /// <summary>
        /// Previous hand difference vector for rotation.
        /// </summary>
        Vector3 PrevRotation;

        void Update() {
            if (LeapMouse.Instance.Action()) {
                int LeftID = LeapMotion.Instance.FirstLeftHand(), RightID = LeapMotion.Instance.FirstRightHand();
                if (LeftID != -1 && RightID != -1 && LeapMotion.Instance.ExtendedFingers(LeftID) == 0 && LeapMotion.Instance.ExtendedFingers(RightID) == 0) { // Rotate
                    Vector3 Left = LeapMotion.Instance.PalmPosition(LeftID), Right = LeapMotion.Instance.PalmPosition(RightID), NewRotation = Right - Left;
                    if (PrevRotation != Vector3.up) {
                        Transform Cam = Camera.main.transform;
                        Vector3 Delta = (NewRotation - PrevRotation) * RotationSensitivity;
                        Target.rotation = Quaternion.Euler(Cam.right * Delta.x) * Quaternion.Euler(Cam.forward * Delta.y) *
                            Quaternion.Euler(Cam.up * Delta.z) * Target.rotation;
                    }
                    PrevRotation = NewRotation;
                    Target.position = MoveStart;
                } else { // Move
                    Vector3 PalmPos = LeapMotion.Instance.PalmPosition();
                    PalmPos = new Vector3(PalmPos.x, PalmPos.y, -PalmPos.z);
                    if (PrevPosition != Vector3.up)
                        Target.position += Camera.main.transform.rotation * ((PalmPos - PrevPosition) * MoveSensitivity);
                    PrevPosition = PalmPos;
                }
            } else {
                PrevPosition = Vector3.up;
                PrevRotation = Vector3.up;
                MoveStart = Target.position;
            }
        }
    }
}