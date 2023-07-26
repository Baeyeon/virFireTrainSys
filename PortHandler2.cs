using UnityEngine;
using UnityEngine.Events;

public class PortHandler2
{
    public UnityAction<int> OnButton_01Click;       //move，只用向前走
    public UnityAction<int> OnButton_02Click;       //material，按一下切换为水，第二下切换干粉，按三下切换CO2，按四下再次切换为水...
    public UnityAction<int> OnButton_03Click;       //喷射开关，按一下为开，第二下为关。

    //private int btn_02_count;
    public static int btn_02_count;
    public static int btn_03_count;

    public void HandMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

        var key = ParseHeader(msg);
        switch (key)
        {
            case KeyCode.Button_01:
                OnButton_01Click(btn_01_count); 
                break;                          

            case KeyCode.Button_02:
                int i = btn_02_count % 3;         //利用余数判断这是按的第几下
                OnButton_02Click(i);              //i为1时材料为水，2为干粉，3为CO2。
                break;

            case KeyCode.Button_03:             
                int j = btn_03_count % 2;        //奇数为开，偶数为关。
                OnButton_03Click(j);
                break;
        }
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


}