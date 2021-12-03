using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMEWORK
{
    class Program
    {
        private static byte[] m_PacketData;
        private static uint m_Pos;
        
        private static int totalData;

        private static byte[] m_ReadData;
        private static int dataStartPoint;

        private static int reverseDataStart;
        private static int reverseDataEnd;
        private static int datdLengthStartPoint;
        private static int dataIdStartPoint;
        private static int dataLength;

        // int : 1，folat : 2 ， string : 3
        private static int dataType;

        public static void Main(string[] args)
        {
            m_PacketData = new byte[1024];
            m_Pos = 4;

            m_ReadData = m_PacketData;
            datdLengthStartPoint = 4;

            reverseDataStart = 0;
            reverseDataEnd = 4;

            dataLength = 0;
            dataType = 0;
            dataIdStartPoint = 8;
            dataStartPoint = 12;

            totalData = 0;

            Write(109);
            Write(109.99f);
            Write("Hello!");

            Console.Write($"Output Byte array(length:{m_Pos}): ");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");
            }

            Console.WriteLine(" ");

            // 反轉資料(從頭開始)
            _Read(m_ReadData, reverseDataStart, reverseDataEnd);
            Console.WriteLine("解封結果:");
            Read(m_ReadData, dataStartPoint, (int)m_Pos);

            Console.Read();
        }

        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);

            dataLength = bytes.Length;
            if (dataLength == 0)
                return false;

            totalData += dataLength;
            var lengthToBytes = BitConverter.GetBytes(dataLength);
            dataType = 1;
            var idToBytes = BitConverter.GetBytes(dataType);

            _Write(lengthToBytes);
            _Write(idToBytes);
            _Write(bytes);
            
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);

            dataLength = bytes.Length;
            if (dataLength == 0)
                return false;

            totalData += dataLength;
            var lengthToBytes = BitConverter.GetBytes(dataLength);
            dataType = 2;
            var idToBytes = BitConverter.GetBytes(dataType);

            _Write(lengthToBytes);
            _Write(idToBytes);
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);

            dataLength = bytes.Length;

            if (dataLength == 0)
                return false;

            totalData += dataLength;
            var lengthToBytes = BitConverter.GetBytes(dataLength);
            dataType = 3;
            var idToBytes = BitConverter.GetBytes(dataType);

            _Write(lengthToBytes);
            _Write(idToBytes);
            _Write(bytes);
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                var totalDataBytes = BitConverter.GetBytes(totalData);
                Array.Reverse(totalDataBytes);
                Array.Reverse(byteData);
                totalDataBytes.CopyTo(m_PacketData, 0);
            }

            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
        }

        private static void Read(byte[] byteData, int dataStartPoint, int length)
        {
            var totalDataLength = BitConverter.ToInt32(byteData, 0);
            Console.WriteLine("資料內容總長度: " + totalDataLength);

            while (true)
            {
                var datalength = BitConverter.ToInt32(byteData, datdLengthStartPoint);
                var dataId = BitConverter.ToInt32(byteData, dataIdStartPoint);

                // 如果資料長度為 0 跳出迴圈
                if (datalength == 0)
                    break;

                datdLengthStartPoint += datalength;
                datdLengthStartPoint += 8;
                dataIdStartPoint += datalength;
                dataIdStartPoint += 8;

                if (dataId == 1)
                {
                    var intData = BitConverter.ToInt32(byteData, dataStartPoint);
                    Console.WriteLine("int data is " + intData);
                    dataStartPoint += datalength;
                    dataStartPoint += 8;
                }
                else if (dataId == 2)
                {
                    var floatData = BitConverter.ToSingle(byteData, dataStartPoint);
                    Console.WriteLine("float data is " + floatData);
                    dataStartPoint += datalength;
                    dataStartPoint += 8;
                }
                else if (dataId == 3)
                {
                    var stringData = Encoding.Unicode.GetString(byteData, dataStartPoint, datalength);
                    Console.WriteLine("string data is " + stringData);
                    dataStartPoint += datalength;
                    dataStartPoint += 8;
                }
            }
        }

        private static void _Read(byte[] byteData , int start , int length)
        {
            if (BitConverter.IsLittleEndian)
            {
                int dl = 4;
                Array.Reverse(byteData, start, length);
                start += 4;
                for (int j = 0; j < m_Pos; j++)
                {
                    Array.Reverse(byteData, start, length);
                    start += 4;
                    Array.Reverse(byteData, start, length);
                    var dataLength = BitConverter.ToInt32(byteData, dl);
                    start += 4;
                    dl += (dataLength + 8);
                    Array.Reverse(byteData, start, dataLength);
                    start += dataLength;

                    if (dataLength == 0)
                        break;
                }
            }
           
            byteData.CopyTo(m_ReadData, 0);

            Console.WriteLine("m_ReadData :");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_ReadData[i] + ", ");
            }
            Console.WriteLine(" ");
        }
    }
}
