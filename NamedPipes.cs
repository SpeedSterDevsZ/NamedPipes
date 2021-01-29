using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace PomoAPI
{
	internal class Pipe
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WaitNamedPipe(string name, int timeout);

		public static bool NamedPipeExist(string pipeName)
		{
			bool result;
			try
			{
				int timeout = 0;
				if (!Pipe.WaitNamedPipe(Path.GetFullPath(string.Format("\\\\.\\pipe\\{0}", pipeName)), timeout))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 0)
					{
						return false;
					}
					if (lastWin32Error == 2)
					{
						return false;
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static void PipeClient(string pipe, string input)
		{
			if (Pipe.NamedPipeExist(pipe))
			{
				try
				{
					using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", pipe, PipeDirection.Out))
					{
						namedPipeClientStream.Connect();
						using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream))
						{
							streamWriter.Write(input);
							streamWriter.Dispose();
						}
						namedPipeClientStream.Dispose();
					}
					return;
				}
				catch (IOException)
				{
					return;
				}
				catch (Exception ex)
				{
					return;
				}
			}
		}
	}
}
