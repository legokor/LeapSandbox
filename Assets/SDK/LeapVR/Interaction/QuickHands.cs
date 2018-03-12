using Leap;
using System;
using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Shows the detected hands relative to the controller.
    /// </summary>
    [AddComponentMenu("Leap VR / Interaction / Quick Hands")]
    public class QuickHands : MonoBehaviour {
        LeapMotion Leap;

        Transform[] Hands = new Transform[0];

        void Start() {
            Leap = LeapMotion.Instance;
        }

        Transform AddSphere(Transform Parent, int ChildID, Vector Position, float Size) {
            Transform UTransform = Parent.childCount > ChildID ? Parent.GetChild(ChildID) : GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            UTransform.SetParent(Parent);
            UTransform.localPosition = new Vector3(-Position.x, Position.y, Position.z);
            UTransform.localScale = new Vector3(Size, Size, Size);
            return UTransform;
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
                    Hands[i].localScale = new Vector3(.001f, .001f, .001f);
                }
            }
            for (int i = 0; i < NewHands; ++i) {
                Hand h = Leap.RawFrame.Hands[i];
                Transform Palm = AddSphere(Hands[i], 0, h.PalmPosition, h.PalmWidth * .2f);
                GrabbingPalm Grabber;
                if (Grabber = Palm.gameObject.GetComponent<GrabbingPalm>())
                    Grabber.HandID = i;
                else
                    Palm.gameObject.AddComponent<GrabbingPalm>().HandID = i;
                int ObjectID = 1;
                Vector3 PalmForward = Vector3.zero;
                foreach (Finger f in h.Fingers) {
                    foreach (Bone b in f.bones) {
                        AddSphere(Hands[i], ObjectID++, b.Basis.translation, b.Width);
                        if (b.Type != Bone.BoneType.TYPE_DISTAL)
                            AddCylinder(Hands[i], ObjectID++, b.Basis.translation, b.Center, b.Width * .5f);
                        if (b.Type == Bone.BoneType.TYPE_METACARPAL)
                            PalmForward += new Vector3(b.Direction.x, b.Direction.y, b.Direction.z);
                    }
                }
                Palm.rotation = Quaternion.LookRotation(PalmForward, new Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z));
            }
        }
    }
}