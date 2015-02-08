using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COMPortListener
{
    class LogWriter
    {
        static readonly Object bufferLock = new Object();
        static String buffer;
        static String comstatus;

        private System.IO.Ports.SerialPort serialPort;
        private int cnt;
             
        public LogWriter() {
            this.cnt = 0;
            LogWriter.Buffer = "";
        }
        public void ThreadRun()
        {
            while (true)
            {
                // ---------------------------------------------------
                // ---------------------------------------------------
                // check COM port

                LogWriter.comstatus = tryToOpenSerialPort();

                try
                {
                    if (cnt++ % 3 == 0) this.serialPort.Write("m");
                }
                catch (Exception) { }
                // ---------------------------------------------------
                // ---------------------------------------------------
                Thread.Sleep(GlobalConstant.THREAD_SLEEP);
            }
        }

        // ---------------------------------------------------
        // ---------------------------------------------------
        // COM Port

        private string tryToOpenSerialPort() {

            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                {
                    this.serialPort             = new System.IO.Ports.SerialPort();
                    this.serialPort.PortName    = GlobalConstant.COM_PORTNAME;
                    this.serialPort.BaudRate    = GlobalConstant.COM_BAUDRATE;
                    this.serialPort.DataBits    = GlobalConstant.COM_DATABITS;
                    this.serialPort.StopBits    = GlobalConstant.COM_STOPBITS;
                    this.serialPort.Parity      = GlobalConstant.COM_PARITY;
                    this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                    this.serialPort.Open();
                }
                
            }
            catch (System.UnauthorizedAccessException e)
            {
                return "COM used";
            }
            catch (Exception e)
            {
                return e.Message; 
            }
            return "ready";
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort sp = (System.IO.Ports.SerialPort)sender;
            string incomingData = sp.ReadExisting();
            LogWriter.Buffer += incomingData;

            // ---------------------------------------------------
            // ---------------------------------------------------
            // write buffer to log

            if (LogWriter.Buffer != "" || LogWriter.comstatus != "ready")
            {

                // create log folder
                if (!System.IO.Directory.Exists(GlobalConstant.LOG_FOLDER_PATH))
                {
                    System.IO.Directory.CreateDirectory(GlobalConstant.LOG_FOLDER_PATH);
                }

                string logFileName = "log_" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                                CultureInfo.InvariantCulture);

                FileStream fs = new FileStream(GlobalConstant.LOG_FOLDER_PATH + "\\" + logFileName,
                                    FileMode.OpenOrCreate, FileAccess.Write);

                StreamWriter swriter = new StreamWriter(fs);
                swriter.BaseStream.Seek(0, SeekOrigin.End);
                swriter.WriteLine(timestamp + " " + comstatus + " : " + LogWriter.Buffer);
                swriter.Flush();
                swriter.Close();

                LogWriter.Buffer = "";
            }

            // ---------------------------------------------------
            // ---------------------------------------------------
        }

        // ---------------------------------------------------
        // ---------------------------------------------------
        // Thread-safe data

        public static String Buffer
        {
            get { return buffer; }
            set
            {
                lock (bufferLock)
                {
                    buffer = value;
                }
            }
        }

        // ---------------------------------------------------
        // ---------------------------------------------------
    }
}
