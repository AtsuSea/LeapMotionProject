using UnityEngine;
using System.Collections;
using Leap;

public class HandControllerNeck : MonoBehaviour
{

    Controller controller;
    HandModel hand_model;

    void Start()
    {
        controller = new Controller();
        // スワイプのジェスチャーを有効にする
        controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
    }

    void Update()
    {

        Frame frame = controller.Frame();
        GestureList leap_gestures = frame.Gestures();

        for (int i = 0; i < leap_gestures.Count; i++)
        {
            Gesture gesture = leap_gestures[i];

            // ジェスチャーがスワイプだった場合
            if (gesture.Type == Gesture.GestureType.TYPESWIPE)
            {
                SwipeGesture Swipe = new SwipeGesture(gesture);
                // スワイプ方向
                Vector SwipeDirection = Swipe.Direction;
                // 0より小さかった場合
                if (SwipeDirection.x < 0)
                {
                    // Downのログを表示
                    Debug.Log("x<0");

                    // 0より大きかった場合
                }
                else if (SwipeDirection.x > 0)
                {
                    // Upのログを表示
                    Debug.Log("x>0");
                }
            }
        }

    }
}
