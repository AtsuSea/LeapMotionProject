using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;
using System;
using System.Linq;

public class HandControllerArduino : MonoBehaviour
{

    Controller controller;
    HandModel hand_model;
    public Text gestureTypeText;
    public Text recognizeText;
    public Text stopRatioText;
    public Text moveRatioText;
    public Text leftRatioText;
    public Text rightRatioText;
    public Text straightRatioText;


    public PlayerControllerArduino playerController;
    public const float RecoginizeStopTime = 1.0f;
    public const float ReloadTime = 0.2f;
    public const int RemainFingerCount = 3;
    private float remainTime = ReloadTime;
    private List<Vector3> fingerDirectionList = new List<Vector3>();
    private List<Vector3> fingerTipList = new List<Vector3>();
    public bool isMove = false;
    bool isRotateLeft = false;
    bool isRotateRight = false;
    bool isReadyRotate = false;


    void Start()
    {
        controller = new Controller();

        controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
        controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
        controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
        controller.EnableGesture(Gesture.GestureType.TYPEINVALID);
        controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);

    }

    void Update()
    {
        Frame frame = controller.Frame();
        GestureList leapGestures = frame.Gestures();

        if (leapGestures.Count > 0)
        {
            gestureTypeText.text = "";
        }

        for (int i = 0; i < leapGestures.Count; i++)
        {
            Gesture gesture = leapGestures[i];
            gestureTypeText.text += gesture.Type.ToString() + " / ";
        }



        FingerList fingers = frame.Hands[0].Fingers;
        int extendedFingerCount = 0;
        Finger extendFinger = new Finger();
        for (int i = 0; i < fingers.Count; i++)
        {
            Finger finger = fingers[i];
            if (finger.IsExtended)
            {
                extendedFingerCount += 1;
                extendFinger = finger;
            }
        }

        //print("x:" + v.x.ToString());
        //print("y:" + v.y.ToString());
        //print("z:" + v.z.ToString());
        //Vector3 v3 = new Vector3(v.x, v.y, v.z);
        //playerController.Rotate(v3);

        //foreach (var fingeraaa in frame.Hands[0].Fingers)
        //{
        //    print(string.Format("ID : {0} 位置 : {1} 速度 : {2} 向き : {3}",
        //      fingeraaa.Id, fingeraaa.TipPosition, fingeraaa.TipVelocity, fingeraaa.Direction));
        //}

        //Finger finger = frame.Hands[0].Fingers.Frontmost;
        //Quaternion quaternion = Quaternion.Euler(finger.Direction.x, finger.Direction.y, finger.Direction.z);
        //print("x:" + finger.Direction.x.ToString());
        //print("y:" + finger.Direction.y.ToString());
        //print("z:" + finger.Direction.z.ToString());



        remainTime -= Time.deltaTime;
        if (remainTime <= 0.0)
        {
            remainTime = ReloadTime;
            if (extendedFingerCount == 1)
            {
                fingerDirectionList.Add(VectorToVector3(extendFinger.Direction));
                fingerTipList.Add(VectorToVector3(extendFinger.TipPosition));


                if (fingerDirectionList.Count > RemainFingerCount)
                {
                    fingerDirectionList.RemoveAt(0);
                    fingerTipList.RemoveAt(0);

                    if (IsRecognize())
                    {
                        Constants.MoveStyle moveStyle = ClassificationMoveStyle();
                        switch(moveStyle)
                        {
                            case Constants.MoveStyle.GoStraight:
                                isMove = true;
                                break;
                            case Constants.MoveStyle.RotateLeft:
                                isRotateLeft = true;
                                break;
                            case Constants.MoveStyle.RotateRight:
                                isRotateRight = true;
                                break;
                            case Constants.MoveStyle.Stop:
                                ClearParameter();
                                break;
                        }
                    }
                }

            }
            else
            {
                fingerDirectionList = new List<Vector3>();
                fingerTipList = new List<Vector3>();
            }
        }

        if (isMove)
        {
            
            if (!leapGestures.Any(g => g.Type == Gesture.GestureType.TYPE_CIRCLE))
            {
                playerController.GoStraight();
            }

            if (extendedFingerCount != 1)
            {
                isMove = false;
            }
        }
        else
        {
            playerController.Stop();
        }

        if (isRotateLeft && !playerController.isRotate)
        {
            if(leapGestures.Any(g => g.Type == Gesture.GestureType.TYPE_CIRCLE))
            {
                playerController.RotateLeft(false);
            }
            else
            {
                playerController.RotateLeft(true);
            }
            isRotateLeft = false;
            ClearParameter();
        }
        else if (isRotateRight && !playerController.isRotate)
        {
            if (leapGestures.Any(g => g.Type == Gesture.GestureType.TYPE_CIRCLE))
            {
                playerController.RotateRight(false);
            }
            else
            {
                playerController.RotateRight(true);
            }
            isRotateRight = false;
            ClearParameter();
        }

        if(extendedFingerCount != 1)
        {
            playerController.RotateStop();
        }

        //Leap.Vector v = frame.Hands[0].Direction;
        //Constants.Rotates rotate;
        //if (v.x < -0.2)
        //{
        //    rotate = Constants.Rotates.Left;
        //}
        //else if (v.x > 0.2)
        //{
        //    rotate = Constants.Rotates.Right;
        //}
        //else
        //{
        //    rotate = Constants.Rotates.None;
        //}
        //playerController.RotateChange(rotate, v.x);
    }

    public Vector3 VectorToVector3(Vector d) => new Vector3(d.x, d.y, d.z);


    private bool IsRecognize()
    {
        float firstStop = StopRecognitionFuzzy(fingerTipList[2], fingerTipList[1]);
        float secondStop = StopRecognitionFuzzy(fingerTipList[2], fingerTipList[0]);
        float thirdStop = StopRecognitionFuzzy(fingerTipList[1], fingerTipList[0]);

        float StopValue = Math.Min(Math.Min(firstStop, secondStop), thirdStop);

        float firstMove = MoveRecognitionFuzzy(fingerTipList[2], fingerTipList[1]);
        float secondMove = MoveRecognitionFuzzy(fingerTipList[2], fingerTipList[0]);
        float thirdMove = MoveRecognitionFuzzy(fingerTipList[1], fingerTipList[0]);

        float MoveValue = Math.Max(Math.Max(firstMove, secondMove), thirdMove);

        if (StopValue > MoveValue)
        {
            recognizeText.text = "認識(●・▽・●)";
            stopRatioText.text = $"StopValue : {StopValue * 100}%";
            moveRatioText.text = $"MoveValue : {MoveValue * 100}%";

            return true;
        }
        else
        {
            recognizeText.text = "認識No";
            stopRatioText.text = $"StopValue : {StopValue * 100}%";
            moveRatioText.text = $"MoveValue : {MoveValue * 100}%";

            return false;
        }
    }


    private Constants.MoveStyle ClassificationMoveStyle()
    {
        //print($"x = {fingerDirectionList[0].x}  y = {fingerDirectionList[0].y}  z = {fingerDirectionList[0].z}");

        float value = fingerDirectionList[0].x;

        float left = LeftRotationClassifyFuzzy(value);
        float right = RightRotationClassifyFuzzy(value);
        float straight = GoStraitClassifyFuzzy(value);

        leftRatioText.text = $"LeftRotation:{left * 100}%";
        straightRatioText.text = $"StraightS:{straight * 100}%";
        rightRatioText.text = $"RightRotation:{right * 100}%";

        if (left > right)
        {
            if (left > straight)
            {
                return Constants.MoveStyle.RotateLeft;
            }
            return Constants.MoveStyle.GoStraight;
        }

        if (right > straight)
        {
            return Constants.MoveStyle.RotateRight;
        }

        return Constants.MoveStyle.GoStraight;
    }


    float StopRecognitionFuzzy(Vector3 first, Vector3 second)
    {
        float x = (float)Math.Sqrt(Math.Pow(first.x - second.x, 2) + Math.Pow(first.y - second.y, 2) + Math.Pow(first.z - second.z, 2));
        float left = 10f;
        float right = 80f;

        if (x >= right)
        {
            return 0;
        }
        else if (x <= left)
        {
            return 1;
        }
        else
        {
            float denom = right - left;
            return 1 - ((x - left) / denom);
        }
    }


    float MoveRecognitionFuzzy(Vector3 first, Vector3 second)
    {
        float x = (float)Math.Sqrt(Math.Pow(first.x - second.x, 2) + Math.Pow(first.y - second.y, 2) + Math.Pow(first.z - second.z, 2));
        float left = 20f;
        float right = 300f;

        if (x >= right)
        {
            return 1;
        }
        else if (x <= left)
        {
            return 0;
        }
        else
        {
            float denom = right - left;
            return (x - left) / denom;
        }
    }


    float LeftRotationClassifyFuzzy(float x)
    {
        float left = -0.8f;
        float right = 0f;

        if (x >= right)
        {
            return 0;
        }
        else if (x <= left)
        {
            return 1;
        }
        else
        {
            float denom = right - left;
            return -x / denom;
        }
    }


    private float RightRotationClassifyFuzzy(float x)
    {
        float left = 0f;
        float right = 0.8f;

        if (x >= right)
        {
            return 1;
        }
        else if (x <= left)
        {
            return 0;
        }
        else
        {
            float denom = right - left;
            return x / denom;
        }
    }


    private float GoStraitClassifyFuzzy(float x)
    {
        float left = -0.40f;
        float right = 0.40f;

        if (x >= right || x <= left)
        {
            return 0;
        }
        else
        {
            float denom = (right - left) / 2;
            return 1 - (Math.Abs(x) / denom);
        }
    }



    public void ResetGesutureText()
    {
        gestureTypeText.text = "";
    }


    public void ClearParameter()
    {
        fingerDirectionList.Clear();
        fingerTipList.Clear();
        isMove = false;
        isRotateLeft = false;
        isRotateRight = false;
    }
}
