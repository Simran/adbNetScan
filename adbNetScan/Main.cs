using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace adbNetScan
{
	class MainClass
	{

        static void Main(string[] args)
        {
			Functions.InitColors();
            startScan ();
        }
		
		static void startScan()
		{
			string ipBase = Regex.Match (defGateway(), "(\\d+.\\d+.\\d+.)").Groups[0].Value;
			Functions.log ("Starting adbNetScan v1!", 4);
			Functions.log ("Scanning @ Port: 5555...", 1);
            for (int i = 1; i < 255; i++)
            {
                string ip = ipBase + i.ToString();
				new Thread(delegate()
				{
					checkAdb(ip);
				}).Start ();
            }
		}
		
		
		static void checkAdb(string IP)
		{
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				s.Blocking = true;
				s.Connect(IP, 5555);
		        send (s, string.Empty);
				if (recv(s) == false)
				{
					Functions.log (string.Format ("{0} is \"ADB over network\" enabled!", IP), 2);
				}
			}
			catch { }
			s.Close ();
		}
				
		static string defGateway()
		{
			string ip = null;
			foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            if (f.OperationalStatus == OperationalStatus.Up)
			{
            foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
				{
					ip = d.Address.ToString();
				}
			}
			Functions.log (string.Format ("Network Gateway: {0}", ip), 5);
			return ip;
		}
		
		static void send(Socket s, string data)
		{
			s.Send(Encoding.Default.GetBytes(data));
		}
		
		static bool recv(Socket s)
		{
			bool res = false;
			try
			{
				byte[] buffer = new byte[8192];
				s.ReceiveTimeout = 500;
				int rec = s.Receive(buffer);
            	Array.Resize(ref buffer, rec);	
				res = true; 
			}
			catch { }
			return res;
		}
	}
}