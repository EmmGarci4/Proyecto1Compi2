using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	static class HandlerFiles
	{
		public static void guardarArchivo(String texto,String filePath) {
				//Path
				using (StreamWriter we = new StreamWriter(filePath))
				{
				we.Write(texto);
				we.Close();
				}			
		}

		public static String AbrirArchivo(String filePath) {
			String fileContent = "";
			try {
				using (StreamReader reader = new StreamReader(filePath))
				{
					fileContent = reader.ReadToEnd();
					reader.Close();
				}
			}
			catch (Exception ex) {
				return null;
			}
			return fileContent;
		}

		internal static string getDate()
		{
			return "'"+DateTime.Now.ToString("yyyy-MM-dd")+"'";
		}

		internal static string getTime()
		{
			return "'"+DateTime.Now.ToString("HH:mm:ss")+"'";
		}
	}
}
