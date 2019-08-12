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
		ListaTablas tablas;
		ListaUserTypes objetos;
		ListaProcedimientos procedimientos;


		public string Nombre { get => nombre; set => nombre = value; }
		internal ListaTablas Tablas { get => tablas; set => tablas = value; }
		internal ListaUserTypes Objetos { get => objetos; set => objetos = value; }
		internal ListaProcedimientos Procedimientos { get => procedimientos; set => procedimientos = value; }

		public BaseDatos(String nombre, List<object> objetosdb) {
			this.nombre = nombre;
			this.tablas = new ListaTablas();
			this.objetos = new ListaUserTypes();
			this.procedimientos = new ListaProcedimientos();
			foreach (object obj in objetosdb) {
				if (obj is Tabla) {
					if (!this.tablas.Existe(((Tabla)obj).Nombre)) {
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

		public void MostrarBaseDatos()
		{
			Console.WriteLine("********************************Base de Datos:" + this.nombre+"********************************");
			this.tablas.Mostrar();
			this.Objetos.Mostrar();
			this.procedimientos.Mostrar();
		}

		public void Insertar(string nombre, List<object> cls)
		{
			Tabla tabla = this.tablas.Buscar(nombre);
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
			Tabla tabla = this.tablas.Buscar(nombre);
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

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"NAME\"=\""+Nombre+"\",\n");
			cadena.Append("\"DATA\"=[");
			cadena.Append(Objetos.ToString());
			if (Procedimientos.Count>0) {
				cadena.Append(",");
				cadena.Append(procedimientos.ToString());
			}
			if (Tablas.Count > 0)
			{
				cadena.Append(",");
				cadena.Append(Tablas.ToString());
			}
			cadena.Append("]\n");
			cadena.Append(">");
			return cadena.ToString();
		}
	}
}
