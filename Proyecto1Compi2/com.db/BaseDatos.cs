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

		public string Nombre { get => nombre; set => nombre = value; }
		public string Path { get => path; set => path = value; }
		internal List<Tabla> Tablas { get => tablas; set => tablas = value; }

		public BaseDatos(String nombre,String path) {
			this.nombre = nombre;
			this.path = path;
			this.tablas = new List<Tabla>();
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
	}
}
