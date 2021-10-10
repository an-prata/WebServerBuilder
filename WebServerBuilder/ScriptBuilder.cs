using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebServerBuilder
{
	class ScriptBuilder
	{
		private FileStream fileStream;
        private bool _disposed;
        public readonly string scriptPath;

		public ScriptBuilder(string path, FileMode fileMode = FileMode.OpenOrCreate)
		{
			try { fileStream = new FileStream(path, fileMode); }
			catch (ArgumentException) { fileStream = new FileStream("temp", fileMode); }
			scriptPath = path;
		}

		public void SaveFile(string lines)
		{
			byte[] bytes = new byte[lines.Length];
			for (int i = 0; i < lines.Length; i++) bytes[i] = Convert.ToByte(lines[i]);
			fileStream.Dispose();
			fileStream = new FileStream(scriptPath, FileMode.Create);
			try { fileStream.Write(bytes, 0, bytes.Length); }
			catch (NullReferenceException) { return; }
		}

		public string[] GetLines() 
		{
			byte[] bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, (int)fileStream.Length);
			return Encoding.Default.GetString(bytes).Split('\n');
		}

		public string[] MakeGet(string filePath, string url)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();

			string[] s = {
				$"app.get('\"{filePath}\"" + "', (req, res) => {",
				$"\tres.sendFile(path.join(__dirname + '\"{url}\"'));",
				$"\tconsole.log('Got request for {url} ({filePath}) ... ');",
				"});"
			};

			return s;
		}

		public static string MakeListen(int port)
		{
			return $"app.listen({Convert.ToString(port)}, console.log('Server listening on port {Convert.ToString(port)}'));";
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				fileStream.Dispose();
			}

			_disposed = true;
		}

		public void Dispose()
        {
			Dispose(true);
			GC.SuppressFinalize(this);
        }
	}
}
