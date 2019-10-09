using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    //先ほど作成したクラス
    public SerialHandler serialHandler;


    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    void Update()
    {
        //文字列を送信
        // serialHandler.Write("hogehoge");
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "\t" }, System.StringSplitOptions.None);
        if (data.Length < 2) return;

        try
        {
            print(message);
            print("ooooooooooooooo");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Stop()
    {
        serialHandler.Write("0");
    }

    public void GoFoward()
    {
        serialHandler.Write("1");
    }

    public void RotateRightAndGo()
    {
        serialHandler.Write("4");
    }

    public void RotateLeftAndGo()
    {
        serialHandler.Write("5");
    }

    public void RotateRightAndStop()
    {
        serialHandler.Write("4");
    }

    public void RotateLeftAndStop()
    {
        serialHandler.Write("5");
    }
}

