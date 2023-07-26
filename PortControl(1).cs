using System;
using UnityEngine;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class PortControl : MonoBehaviour
{
    [SerializeField]
    private string portName = "COM5";//串口名

    [SerializeField]
    private int baudRate = 115200;//波特率

    [SerializeField]
    private Parity parity = Parity.None;//效验位

    [SerializeField]
    private int dataBits = 8;//数据位

    [SerializeField]
    private StopBits stopBits = StopBits.One;//停止位

    [SerializeField]
    private int readTimeOut = 4000;//发送超时

    [SerializeField]
    private int threadSleep = 10;//线程休眠


    public PortHandler handler = new PortHandler();

    private const int DATA_LENGTH = 5;//数据长度
    private SerialPort sp = null;
    private Thread dataReceiveThread;
    private List<byte> listReceive = new List<byte>();
    private static bool isOpen = false;

    private void Start()
    {
        if (isOpen)
        {
            ClosePort();
        }

        if (!isOpen)
        {
            isOpen = true;

            OpenPort();
            dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
            dataReceiveThread.Start();
        }
    }
    
    public void OpenPort()
    {
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);//创建串口
        sp.ReadTimeout = readTimeOut;
        try
        {
            sp.Open();
            Debug.Log("串口打开成功！");
        }
        catch (Exception ex)
        {
            Debug.LogError("串口打开异常:" + ex.Message);
        }
    }
    
    private void OnApplicationQuit()
    {
        ClosePort();
    }

    public void ClosePort()
    {
        try
        {
            sp.Close();
            dataReceiveThread.Abort();
            isOpen = false;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    
    private void DataReceiveFunction()
    {

#if TEST
        Debug.Log("使用测试01");
        return;
#endif

        if (sp.IsOpen)
        {
            Debug.Log("准备接收数据。。。。。。");
        }

        int length = 0;
        byte[] buffer = new byte[DATA_LENGTH];

        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    length = sp.Read(buffer, 0, buffer.Length);//接收字节
                    if (length == 0)
                    {
                        Debug.Log("接收到的数据为 0");
                        continue;
                    }
                    else
                    {
                        string result = ByteArrayToHex(buffer);
                        Debug.Log("接收到的数据 ： " + result);
                        handler.HandMessage(result);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            Thread.Sleep(threadSleep);
        }
    }

    /// <summary>
    /// 字节数组转 16 进制
    /// </summary>
    private string ByteArrayToHex(byte[] data)
    {
        StringBuilder sb = new StringBuilder(data.Length * 2);
        foreach (byte b in data)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString().ToUpper();
    }

    ///// <summary>
    ///// 向串口写入数据
    ///// </summary>
    //public void WriteData(string data)
    //{
    //    if (sp.IsOpen)
    //    {
    //        sp.Write(data);
    //    }
    //    else
    //    {
    //        Debug.Log("写入数据失败，串口未打开");
    //    }
    //}

    //public void ChangeToolsCommand()
    //{
    //    handler.HandMessage("5A03000000");
    //}


#if TEST
    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.Q))
        {

            handler.HandMessage("5A01000000");
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            handler.HandMessage("5A02000000");
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            handler.HandMessage("5A03000000");
        }
    }
#endif
}







/******************** 测试 **********************/
//private string message = string.Empty;
//private void OnGUI()
//{
//    message = GUILayout.TextField(message);
//    if (GUILayout.Button("Send Input"))
//    {
//        WriteData(message);
//    }

//    string test = "112233";//测试字符串
//    if (GUILayout.Button("Send Test"))
//    {
//        WriteData(test);
//    }
//}






//public static string ByteArrayToHex(byte[] bytes)
//{
//    string returnStr = "";
//    if (bytes != null)
//    {
//        for (int i = 0; i < bytes.Length; i++)
//        {
//            returnStr += bytes[i].ToString("X2");
//        }
//    }
//    return returnStr;
//}

//public static String byteArrToHex(byte[] btArr)
//{
//    char[] strArr = new char[btArr.Length * 2];
//    int i = 0;
//    foreach (byte bt in btArr)
//    {
//        strArr[i++] = HexCharArr[bt >>> 4 & 0xf];
//        strArr[i++] = HexCharArr[bt & 0xf];
//    }
//    return new String(strArr);
//}
