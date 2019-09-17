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
		//static string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
		static string relativePath = "";

		public static void guardarArchivo(String texto,String filePath) {
			if (!Directory.Exists(relativePath+"data")) {
				Directory.CreateDirectory(relativePath + "data");
			}
			Console.WriteLine(relativePath);
			//Path
			using (StreamWriter we = new StreamWriter(relativePath + "data/"+filePath))
				{
				we.Write(texto);
				we.Close();
				}			
		}

		public static String AbrirArchivo(String filePath) {
			
			String fileContent = "";
			try {
				using (StreamReader reader = new StreamReader(relativePath + "data/"+filePath))
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

	}
}
