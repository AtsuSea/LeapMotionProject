using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    float rotateSpeed = 0.0f;
    public float rotateTime;
    private float elapsedTime = 0f;
    public bool isRotate = false;
    public Transform frontTransform;

    // CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    public bool isMove = false;
    private Quaternion targetRotate;
    private Quaternion startRotate;
    public Control_UI controller;

    public bool rotateAndMove = true;



    void Start()
    {
        // controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isMove)
        {
            //if (controller.isGrounded)
            //{
            //    moveDirection = frontTransform.localPosition;
            //    moveDirection = transform.TransformDirection(moveDirection);
            //    moveDirection *= speed;
            //}

            //moveDirection.y -= gravity * Time.deltaTime;
            //controller.Move(moveDirection * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        //// Y軸(Vector3.up)周りを１フレーム分の角度だけ回転させるQuaternionを作成
        //Quaternion rot = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.up);

        //// 元の回転値と合成して上書き
        //transform.localRotation = rot * transform.localRotation;

        if (isRotate)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > rotateTime)
            {
                transform.localRotation = targetRotate;
                isRotate = false;
                controller.ClearParameter();
                controller.isMove = true;
                if(rotateAndMove)
                {
                    GoStraight();
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(startRotate, targetRotate, elapsedTime / rotateTime);
            }
        }
    }

    public void GoStraight()
    {
        isMove = true;
    }

    public void Stop()
    {
        isMove = false;
    }


    public void RotateChange(Constants.Rotates rotates, float f = 0)
    {
        switch(rotates)
        {
            case Constants.Rotates.None:
                rotateSpeed = 0.0f;
                break;
            case Constants.Rotates.Left:
                rotateSpeed = 180.0f * f;
                break;
            case Constants.Rotates.Right:
                rotateSpeed = 180.0f * f;
                break;
        }
    }

    public void Rotate(float rotateAngle, bool move = true)
    {
        Vector3 v = transform.localRotation.eulerAngles;
        targetRotate = Quaternion.Euler(v.x, v.y + 90 * rotateAngle, v.z);
        startRotate = transform.localRotation;
        elapsedTime = 0f;
        isRotate = true;

        rotateAndMove = move;
    }

    public void RotateStop()
    {
        isRotate = false;
    }

}
