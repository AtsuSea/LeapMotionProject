using UnityEngine;
using static Constants;

public class PlayerControllerArduino : MonoBehaviour
{

    public ArduinoManager arduinoManager;

    public bool rotateAndMove = true;
    public bool isMove = false;
    public bool isRotate = false;
    public bool isRotateLeftAndGo = false;
    public bool isRotateRightAndGo = false;
    public bool isRotateLeftAndStop = false;
    public bool isRotateRightAndStop = false;

    bool debug = true;




    void Start()
    {
    }

    void Update()
    {
        if (isRotateLeftAndGo)
        {
            arduinoManager.RotateLeftAndGo();
            if(debug)
            print("3");
        }
        else if (isRotateRightAndGo)
        {
            arduinoManager.RotateRightAndGo();
            if (debug)
            print("2");
        }
        else if (isRotateLeftAndStop)
        {
            arduinoManager.RotateLeftAndStop();
            if (debug)
            print("3");
        }
        else if (isRotateRightAndStop)
        {
            arduinoManager.RotateRightAndStop();
            if (debug)
            print("2");
        }
        else if (isMove)
        {
            arduinoManager.GoFoward();
            if (debug)
            print("1");
        }
        else
        {
            arduinoManager.Stop();
            if (debug)
            print("0");
        }
    }

    public void GoStraight()
    {
        if (!isMove)
        {
            isMove = true;
        }
    }

    public void Stop()
    {
        if (isMove)
        {
            isMove = false;
        }
    }

    public void RotateLeft(bool move = true)
    {
        if (move)
        {
            isRotateLeftAndGo = true;
        }
        else
        {
            isRotateLeftAndStop = true;
        }
    }


    public void RotateRight(bool move = true)
    {
        if (move)
        {
            isRotateRightAndGo = true;
        }
        else
        {
            isRotateRightAndStop = true;
        }
    }

    public void RotateStop()
    {
        isRotate = false;
        isRotateLeftAndGo = false;
        isRotateRightAndGo = false;
        isRotateLeftAndStop = false;
        isRotateRightAndStop = false;
    }
}
