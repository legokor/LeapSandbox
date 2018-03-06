using Leap;
using System;
using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Shows the detected hands relative to the controller.
    /// </summary>
    [AddComponentMenu("Leap VR / Quick Hands")]
    public class QuickHands : MonoBehaviour {
        LeapMotion Leap;

        Transform[] Hands = new Transform[0];

        void Start() {
            Leap = LeapMotion.Instance;
        }

        void AddSphere(Transform Parent, int ChildID, Vector Position, float Size) {
            Transform UTransform = Parent.childCount > ChildID ? Parent.GetChild(ChildID) : GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            UTransform.SetParent(Parent);
            UTransform.localPosition = new Vector3(-Position.x, Position.y, Position.z);
            UTransform.localScale = new Vector3(Size, Size, Size);
        }

        void AddCylinder(Transform Parent, int ChildID, Vector Basis, Vector Center, float Size) {
            Vector3 UBasis = new Vector3(-Basis.x, Basis.y, Basis.z), UCenter = new Vector3(-Center.x, Center.y, Center.z), Direction = UBasis - UCenter;
            Transform UTransform = Parent.childCount > ChildID ? Parent.GetChild(ChildID) : GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            UTransform.SetParent(Parent);
            UTransform.localPosition = UCenter;
            UTransform.up = transform.rotation * Direction;
            UTransform.localScale = new Vector3(Size, Direction.magnitude, Size);
        }

        void Update() {
            int NewHands = Leap.GetHandCount(), OldHands = Hands.Length;
            if (NewHands != OldHands) {
                for (int i = NewHands; i < OldHands; ++i)
                    Destroy(Hands[i].gameObject);
                Array.Resize(ref Hands, NewHands);
                for (int i = OldHands; i < NewHands; ++i) {
                    Hands[i] = new GameObject().transform;
                    Hands[i].SetParent(gameObject.transform, false);
                    Hands[i].localScale = new Vector3(.005f, .005f, .005f);
                }
            }
            for (int i = 0; i < NewHands; ++i) {
                int ObjectID = 0;
                foreach (Finger f in Leap.RawFrame.Hands[i].Fingers) {
                    foreach (Bone b in f.bones) {
                        AddSphere(Hands[i], ObjectID++, b.Basis.translation, b.Width);
                        if (b.Type != Bone.BoneType.TYPE_DISTAL)
                            AddCylinder(Hands[i], ObjectID++, b.Basis.translation, b.Center, b.Width * .5f);
                    }
                }
            }
        }
    }
}