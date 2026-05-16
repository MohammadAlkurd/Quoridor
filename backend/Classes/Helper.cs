
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using overrides;

namespace Assistant
{
    static class Helper
    {
        public static Dictionary<int, char> ConvertionTable = new Dictionary<int, char>()
    {

        {0,'a'},{1,'b'},{2,'c'},{3,'d'},{4,'e'},{5,'f'},{6,'g'},{7,'h'},{8,'i'},{9,'j'}
        ,{10,'k'},{11,'l'},{12,'m'},{13,'n'},{14,'o'},{15,'p'},{16,'q'},{17,'r'},{18,'s'}
        ,{19,'t'},{20,'u'},{21,'v'},{22,'w'},{23,'x'},{24,'y'},{25,'z'},{26,'A'},{27,'B'},{28,'C'}
    };
        public static Dictionary<char, int> ConvertionTableReverse = new Dictionary<char, int>()
    {
         //subtract 1 from all the numbers below for example a,1 becomes a,0 etc
    {'a', 0}, {'b', 1}, {'c', 2}, {'d', 3}, {'e', 4}, {'f', 5}, {'g', 6}, {'h', 7}, {'i', 8}, {'j', 9},
    {'k', 10}, {'l', 11}, {'m', 12}, {'n', 13}, {'o', 14}, {'p', 15}, {'q', 16}, {'r', 17}, {'s', 18},
    {'t', 19}, {'u', 20}, {'v', 21}, {'w', 22}, {'x', 23}, {'y', 24}, {'z', 25}, {'A', 26}, {'B', 27},{'C', 28}


    };
        public static string ConvertStringToIP(string roomCode)
        {

            // Convert the room code to a character array.
            char[] roomCodeChars = roomCode.ToCharArray();

            // Convert the first three characters of the room code to numbers.
            string[] ipComponents = new string[4];
            int b = 0;
            string port = "";
            for (int i = 0; i < roomCode.Length; i++)
            {
                if (int.TryParse(roomCodeChars[i].ToString(), out int result))
                {
                    ipComponents[b - 1] += result.ToString();
                }
                else
                {
                    if (b >= 4) { b = i; break; }
                    ipComponents[b] = ConvertionTableReverse[roomCodeChars[i]].ToString();
                    b++;
                }

            }
            for (int i = b; i < roomCode.Length; i++)
            {
                port += ConvertionTableReverse[roomCodeChars[i]].ToString();
            }

            // Return the IP address as a string.
            return string.Join(".", ipComponents) + ":" + port;

        }
        public static string ConvertIPTOString(string ip)
        {
            string[] ipComponents = ip.Split(':').First().Split('.');

            // Convert the first three components of the IP address to characters.
            string[] roomCode = new string[4];
            for (int i = 0; i < 4; i++)
            {
                if (int.Parse(ipComponents[i]) < 28)
                    roomCode[i] = ConvertionTable[int.Parse(ipComponents[i])].ToString();
                else
                {
                    roomCode[i] = ConvertionTable[int.Parse(ipComponents[i]) / 10].ToString() + (int.Parse(ipComponents[i]) % 10).ToString();
                }
            }

            // Add the port number to the room code.
            char[] portnumbers = int.Parse(ip.Split(':').Last()).ToString().ToCharArray();
            string gaming = "";
            for (int i = 0; i < portnumbers.Length; i++)
            {
                if (i < portnumbers.Length - 1)
                {
                    int tmp = int.Parse(portnumbers[i].ToString() + portnumbers[i + 1].ToString());
                    if (tmp < 28)
                    {
                        gaming += ConvertionTable[tmp].ToString();
                        i++;
                        continue;
                    }
                    else
                    {
                        gaming += ConvertionTable[int.Parse(portnumbers[i].ToString())].ToString();
                    }
                }
                else
                {
                    gaming += ConvertionTable[int.Parse(portnumbers[i].ToString())].ToString();
                }
            }
            // Return the room code as a string.
            return string.Join("", roomCode) + gaming;
        }

        public static byte[] strTObyt(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        public static bool IsValidReq(byte[] Req)
        {
            return true;
        }
        public static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = s.Available == 0;
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}