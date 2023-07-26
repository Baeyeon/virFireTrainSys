using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class ChatManager : MonoBehaviour
{
    /**
     * socketClient
     */
    //public string ipaddress = "192.168.0.112";
    // public string ipaddress = "10.27.242.118";
    public string ipaddress = "192.168.13.89";
    public int port = 6000;
   // public UIInput textInput;
   // public UILabel chatLabel;

    private Socket clientSocket;
    private Thread t;
    private byte[] data = new byte[1024];//数据容器
    //private string message ;//消息容器
    private string message;
    // Use this for initialization
    static KeyCode key;

    /**
     * portHandler
     */
    //move，只用向前走
    //material，按一下切换为水，第二下切换干粉，按三下切换CO2，按四下再次切换为水...
    //喷射开关，按一下为开，第二下为关。

    //private int btn_02_count;
    public static int btn_02_count;
    public static int btn_03_count;

    void Update()
    {
        //if (message != null && message != "")
        //{
        //   chatLabel.text += "\n" + message;

        //    message = "消息来了！！！！";//清空消息
        // }

        //----------------------------------------------------------------------------------
        //string Msg = message;               //zenmeban..
        //Debug.Log("Msg=" + Msg);            //这个msg一直都是空的。
        //key = ParseHeader(Msg);     //key也是空的。

        if (key.ToString() != null && key.ToString() != "")
        {
            Debug.Log("收到key:");
            Debug.Log("key=" + key.ToString());

            switch (key)
            {
                case KeyCode.Button_01:
                    //case KeyCode.W:
                    Move._instance.moveExtinguisher();
                    Debug.Log("前进了几步！！");
                    break;

                case KeyCode.Button_02:
                    //case KeyCode.A:
                    int i = btn_02_count % 3;         //利用余数判断这是按的第几下             
                    Water._instance.ChangeMaterial(i);//i为1时材料为水，2为干粉，0为CO2
                    Debug.Log("材料已切换！！");
                    break;
                case KeyCode.Button_03:
                    //case KeyCode.S:
                    int j = btn_03_count % 2;        //奇数为开，偶数为关。
                    Water._instance.Switch(j);
                    //Debug.Log("开关！！");
                    break;
            }
        }
    }

    public void HandMessage(string msg)
    {
        Debug.Log("HandMessage已接受消息");  //这步是对的。
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }
        key = ParseHeader(msg);
        /*
        switch (key)
        {
            case KeyCode.Button_01:
                //case KeyCode.W:
                Move._instance.moveExtinguisher();
                //Debug.Log("前进了几步！！");
                break;

            case KeyCode.Button_02:
                //case KeyCode.A:
                int i = btn_02_count % 3;         //利用余数判断这是按的第几下             
                Water._instance.ChangeMaterial(i);//i为1时材料为水，2为干粉，0为CO2。
                //Debug.Log("材料已切换！！");
                break;
            case KeyCode.Button_03:
                //case KeyCode.S:
                int j = btn_03_count % 2;        //奇数为开，偶数为关。
                Water._instance.Switch(j);
                //Debug.Log("开关！！");
                break;
        }
        */
    }   //再改一下函数名就好了。

    private KeyCode ParseHeader(string msg)        //从客户端发来的字节流的服务类型（比如向前走、喷水），并返回指定的按钮。
    {
        if (msg.StartsWith("5A03") || msg.StartsWith("03"))
        {
            btn_03_count++;
            return KeyCode.Button_03;
        }

        if (msg.StartsWith("5A02") || msg.StartsWith("02"))  //需要调试一下传来的具体数据。
        {
            btn_02_count++;
            return KeyCode.Button_02;
        }

        if (msg.StartsWith("5A01") || msg.StartsWith("01"))
        {
            //btn_01_count++;
            return KeyCode.Button_01;
        }

        return KeyCode.None;
    }

    private enum KeyCode
    {
        None,
        Button_01,  //向前走
        Button_02,  //切换材料
        Button_03   //开关

    }
    /*
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            HandMessage(msg1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            HandMessage(msg2);
        }
        if (Input.GetKey(KeyCode.S))
        {
            HandMessage(msg3);
        }
    }
    */

    /*
     * socketClient
     */

    void Start () {
	    ConnectToServer();
	}
	
	// Update is called once per frame
	

    //与服务器端建立联系
    void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        //跟服务器端建立连接
        Debug.Log("asdf");
        clientSocket.Connect( new IPEndPoint(IPAddress.Parse(ipaddress),port) );
        Debug.Log("bbbb");

        Debug.Log("启动新线程");

        //创建一个新的线程 用来接收消息
        t = new Thread(ReceiveMessage);
        
        t.Start();
    }

    //接收服务器端发来的数据
    /// <summary>
    /// 这个线程方法 用来循环接收消息
    /// </summary>
    void ReceiveMessage()
    {
        Debug.Log("开始接收消息");
        while (true)
        {
            if (clientSocket.Connected == false)
                break;

            int length = clientSocket.Receive(data);
           // Debug.Log(length);
            Debug.Log("获得data长度：");
            Debug.Log(length);
            //message = Encoding.UTF8.GetString(data, 0, length);
            message = Encoding.UTF8.GetString(data, 0, length);
            
            Debug.Log("获得的信息：");
            Debug.Log(message);

            //将数据传给portHandler
            ChatManager cm = new ChatManager();
            cm.HandMessage(message);   
            //chatLabel.text += "\n" + message;
        }
    }

    //向服务器端发送数据
    void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
    }

    //按向服务器端发送数据键
    public void OnSendButtonClick()
    {
        //   string value = textInput.value;
        string value = "客户端发起连接请求";
        SendMessage(value);
        //textInput.value = "";
    }

    //关闭连接
    void OnDestroy()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        
        clientSocket.Close();//关闭连接
    }
}
