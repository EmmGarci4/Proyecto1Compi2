using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.db
{
	class BaseDatos
	{
		String nombre;
		String path;
		List<Tabla> tablas;
		Lista_Objetos objetos;
		Lista_Procedimientos procedimientos;


		public string Nombre { get => nombre; set => nombre = value; }
		public string Path { get => path; set => path = value; }
		internal List<Tabla> Tablas { get => tablas; set => tablas = value; }
		internal Lista_Objetos Objetos { get => objetos; set => objetos = value; }
		internal Lista_Procedimientos Procedimientos { get => procedimientos; set => procedimientos = value; }

		public BaseDatos(String nombre,String path) {
			this.nombre = nombre;
			this.path = path;
			this.tablas = new List<Tabla>();
			this.objetos = new Lista_Objetos("Holis.txt");
			this.procedimientos = new Lista_Procedimientos("Adios.txt");
		}

		public void AgregarTabla(Tabla tb) {
			this.tablas.Add(tb);
		}

		public Tabla BuscarTabla(String nombre) {
			foreach (Tabla tb in this.tablas) {
				if (tb.Nombre.Equals(nombre)) {
					return tb;
				}
			}
			return null;
		}

		public void EliminarTabla() {
			foreach (Tabla tb in this.tablas)
			{
				if (tb.Nombre.Equals(nombre))
				{
					this.tablas.Remove(tb);
				}
			}
		}

		public bool ExisteTabla(String nombre)
		{
			foreach (Tabla tb in this.tablas)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		public void MostrarBaseDatos()
		{
			Console.WriteLine("Base de Datos:"+this.nombre+"********************************");
			foreach (Tabla tb in this.tablas) {
				Console.WriteLine("Tabla: "+tb.Nombre);
				tb.MostrarColumnas();
			}
		}

		public void Insertar(string nombre, List<Celda> cls)
		{
			Tabla tabla = BuscarTabla(nombre);
			if (tabla != null)
			{
				tabla.AgregarFila(cls);
			}
			else {
				Console.WriteLine("ERROR:NO EXISTE LA TABLA");
			}
		}

		internal void Insertar(string nombre, List<string> columnas, List<Celda> cls)
		{
			Tabla tabla = BuscarTabla(nombre);
			if (tabla != null)
			{
				if (tabla.ExistenColumnas(columnas)) {
					tabla.AgregarFila(cls, columnas);
				}
			}
			else
			{
				Console.WriteLine("ERROR:NO EXISTE LA TABLA");
			}
		}

		internal void GenerarArchivo()
		{
			StringBuilder archivoDB = new StringBuilder();
			//procedimientos
			archivoDB.AppendLine("<Prodedure>");
			archivoDB.AppendLine("<Path>"+this.procedimientos.Path+ "</Path>");
			archivoDB.AppendLine("</Prodedure>");
			//objetos
			archivoDB.AppendLine("<Object>");
			archivoDB.AppendLine("<Path>" + this.objetos.Path + "</Path>");
			archivoDB.AppendLine("</Object>");

			//tablas
			foreach (Tabla tb in this.tablas) {
				archivoDB.Append(tb.GetXml());
			}
			Console.WriteLine("****************************");
			Console.WriteLine(archivoDB.ToString());
			//GENERANDO ARCHIVOS DE CADA TABLA
			foreach (Tabla tb in this.tablas)
			{
				tb.GenerarArchivo();
			}
			//GENERANDO ARCHIVO DE OBJETOS
			this.objetos.GenerarArchivo();
			//GENERANDO ARCHIVO DE PROCEDIMIENTOS
			this.procedimientos.GenerarArchivo();
		}
	}
}
