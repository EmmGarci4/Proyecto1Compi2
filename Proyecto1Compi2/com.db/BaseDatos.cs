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
		List<Tabla> tablas;
		Lista_Objetos objetos;
		Lista_Procedimientos procedimientos;


		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Tabla> Tablas { get => tablas; set => tablas = value; }
		internal Lista_Objetos Objetos { get => objetos; set => objetos = value; }
		internal Lista_Procedimientos Procedimientos { get => procedimientos; set => procedimientos = value; }

		public BaseDatos(String nombre, List<object> objetosdb) {
			this.nombre = nombre;
			this.tablas = new List<Tabla>();
			this.objetos = new Lista_Objetos();
			this.procedimientos = new Lista_Procedimientos();
			foreach (object obj in objetosdb) {
				if (obj is Tabla) {
					if (!ExisteTabla(((Tabla)obj).Nombre)) {
						this.tablas.Add((Tabla)obj);
					}
					else {
						Console.WriteLine("ERROR LA TABLA YA EXISTE");
					}
				}else if (obj is UserType)
				{
					if (!ExisteUserType(((UserType)obj).Nombre))
					{
						this.objetos.Add((UserType)obj);
					}
					else
					{
						Console.WriteLine("ERROR EL USERTYPE YA EXISTE");
					}
				}
				else if (obj is Procedimiento)
				{
					if (!ExisteProcedimiento(((Procedimiento)obj).Nombre))
					{
						this.procedimientos.Add((Procedimiento)obj);
					}
					else
					{
						Console.WriteLine("ERROR EL PROCEDMIENTO YA EXISTE");
					}
					
				}
			}
		}

		private bool ExisteProcedimiento(string nombre)
		{
			foreach (Procedimiento tb in this.procedimientos)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		private bool ExisteUserType(string nombre)
		{
			foreach (UserType tb in this.objetos)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
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
				tb.MostrarCabecera();
				tb.MostrarDatos();
			}
			foreach (UserType user in objetos) {
				user.Mostrar();
			}
			foreach (Procedimiento pr in procedimientos) {
				pr.Mostrar();
			}
		}

		public void Insertar(string nombre, List<object> cls)
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

		internal void Insertar(string nombre, List<string> columnas, List<object> cls)
		{
			Tabla tabla = BuscarTabla(nombre);
			if (tabla != null)
			{
				if (tabla.ExistenColumnas(columnas)) {
					//tabla.AgregarFila(cls, columnas);
				}
			}
			else
			{
				Console.WriteLine("ERROR:NO EXISTE LA TABLA");
			}
		}

	}
}
