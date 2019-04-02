using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;

public class controler : MonoBehaviour
{
    SerialPort stream;
    public GameObject target; // is the gameobject to 
    private float amountToMove;
    public float speed;

    float acc_normalizer_factor = 0.00025f;
    float gyro_normalizer_factor = 1.0f / 32768.0f;   // 32768 is max value captured during test on imu
    float curr_angle_x = 0;
    float curr_angle_y = 0;
    float curr_angle_z = 0;
    float curr_offset_x = 0;
    float curr_offset_y = 0;
    float curr_offset_z = 0;

    // Increase the speed/influence rotation
    public float factor = 7;
    public bool enableRotation;
    public bool enableTranslation;

    // SELECT YOUR COM PORT AND BAUDRATE
    string port = "COM5";
    int baudrate = 9600;
    int readTimeout = 25;

   void Start()
    {
       // open port. Be shure in unity edit > project settings > player is NET2.0 and not NET2.0Subset
        stream = new SerialPort("\\\\.\\" + port, baudrate);
        try
        {
            stream.ReadTimeout = readTimeout;
        }
        catch (System.IO.IOException ioe)
        {
            Debug.Log("IOException: " + ioe.Message);
        }
 
        stream.Open();
    }
 
    void Update()
    {
        amountToMove = speed * Time.deltaTime;//move amount
        string dataString = "null received";
        if (stream.IsOpen)
        {
            try
            {
                //MoveObject(stream.ReadByte());
                dataString = stream.ReadLine();
                Debug.Log("RCV_ : " + dataString);
            }
            catch (System.IO.IOException ioe)
            {
                Debug.Log("IOException: " + ioe.Message);
            }
        }
        else
            dataString = "NOT OPEN";
        Debug.Log("RCV_ : " + dataString);
        if (!dataString.Equals("NOT OPEN"))
        {
           // recived string is  like  "accx;accy;accz;gyrox;gyroy;gyroz"
            char splitChar = ';';
            string[] dataRaw = dataString.Split(splitChar);

            // normalized accelerometer values
            float ax = Int32.Parse(dataRaw[0]) * acc_normalizer_factor;
            float ay = Int32.Parse(dataRaw[1]) * acc_normalizer_factor;
            float az = Int32.Parse(dataRaw[2]) * acc_normalizer_factor;

            // normalized gyrocope values
            float gx = Int32.Parse(dataRaw[3]) * gyro_normalizer_factor;
            float gy = Int32.Parse(dataRaw[4]) * gyro_normalizer_factor;
            float gz = Int32.Parse(dataRaw[5]) * gyro_normalizer_factor;

            //button1,2,3 press check
            if (Int32.Parse(dataRaw[6]) != 0 || Int32.Parse(dataRaw[7]) != 0 || Int32.Parse(dataRaw[8]) != 0) {
                int bIn = Int32.Parse(dataRaw[6]) + Int32.Parse(dataRaw[7]) + Int32.Parse(dataRaw[8]);
                //button press method
            }

            //move joystick
            MoveObject(Int32.Parse(dataRaw[9]), Int32.Parse(dataRaw[10]));


            // prevent 
            if (Mathf.Abs(ax) - 1 < 0) ax = 0;
            if (Mathf.Abs(ay) - 1 < 0) ay = 0;
            if (Mathf.Abs(az) - 1 < 0) az = 0;

            curr_offset_x += ax;
            curr_offset_y += ay;
            curr_offset_z += 0; // The IMU module have value of z axis of 16600 caused by gravity

            // prevent little noise effect
            if (Mathf.Abs(gx) < 0.025f) gx = 0f;
            if (Mathf.Abs(gy) < 0.025f) gy = 0f;
            if (Mathf.Abs(gz) < 0.025f) gz = 0f;

            curr_angle_x += gx;
            curr_angle_y += gy;
            curr_angle_z += gz;

            if(enableTranslation) target.transform.position = new Vector3(curr_offset_x, curr_offset_z, curr_offset_y);
            if(enableRotation) target.transform.rotation = Quaternion.Euler(curr_angle_x* factor, -curr_angle_z* factor, curr_angle_y* factor);
        }
    }

    void MoveObject(int JoyX, int JoyY)
    {
        Debug.Log("stick in:X axis" + JoyX +"Y axis" + JoyY);
        int XDelta = Math.Abs(500 - JoyX);
        int YDelta = Math.Abs(500 - JoyY);

        speed = 2+((XDelta>YDelta?XDelta:YDelta)>>6);//select bigger and set speed from that.

        if (JoyX > 550 || JoyX < 450)
        {
            if(JoyX > 550)
                transform.Translate(Vector3.right * amountToMove, Space.World);
            if (JoyX < 450)
                transform.Translate(Vector3.left * amountToMove, Space.World);
        }
        if (JoyY > 550 || JoyY < 450)
        {
            if (JoyY > 550)
                transform.Translate(Vector3.forward * amountToMove, Space.World);
            if (JoyY < 450)
                transform.Translate(Vector3.back * amountToMove, Space.World);
        }
    }


}
