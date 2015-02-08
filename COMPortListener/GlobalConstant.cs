using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace COMPortListener
{
    class GlobalConstant
    {
        public static string SERVICE_NAME               = "COMPortListener";
        public static string SERVICE_DISPLAYNAME        = "COMPortListener";
        public static string SERVICE_DESCRIPTION        = "Listen OPOS via COM port and generate text file to the specific path.";

        public static int THREAD_SLEEP                  = 5000;

        public static string LOG_FOLDER_PATH            = @"c:\comportservice";

        public static string COM_PORTNAME               = "COM3";
        public static int COM_BAUDRATE                  = 9600;
        public static int COM_DATABITS                  = 8;
        public static StopBits COM_STOPBITS             = StopBits.One;
        public static Parity COM_PARITY                 = Parity.Odd;
    }
}
