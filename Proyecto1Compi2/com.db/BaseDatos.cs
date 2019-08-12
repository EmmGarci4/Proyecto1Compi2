using com.Analisis;
using com.Analisis.Util;
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

		public BaseDatos(String nombre) {
			this.nombre = nombre;
			this.tablas = new ListaTablas();
			this.objetos = new ListaUserTypes();
			this.procedimientos = new ListaProcedimientos();
		}

		public bool ExisteTabla(string nombre)
		{
			return tablas.Existe(nombre);
		}
		public bool ExisteProcedimiento(string nombre)
		{
			return procedimientos.Existe(nombre);
		}

		public bool ExisteUserType(string nombre)
		{
			return objetos.Existe(nombre);
		}

		public void AgregarTabla(Tabla tb) {
			this.tablas.Add(tb);
		}

		public void AgregarUserType(UserType obj)
		{
			this.objetos.Add(obj);
		}

		public void AgregarProcedimiento(Procedimiento obj)
		{
			this.procedimientos.Add(obj);
		}

		public void Insertar(string nombre, List<object> cls,int linea,int columna)
		{
			Tabla tabla = this.tablas.Buscar(nombre);
			if (tabla != null)
			{
				tabla.Insertar(cls);
			}
			else {
				//INSERTANDO ERROR EN TABLA ERRORS
				Analizador.Errors.Insertar(new List<object>
						{
							"Sintáctico",
							"La tabla '"+nombre+"' no existe",
							linea,
							columna,
							HandlerFiles.getDate(), //fecha
							HandlerFiles.getTime()//hora
						});
			}
		}

		public void Insertar(string nombre, List<string> columnas, List<object> cls,int linea,int columna)
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
				//INSERTANDO ERROR EN TABLA ERRORS
				Analizador.Errors.Insertar(new List<object>
						{
							"Sintáctico",
							"La tabla '"+nombre+"' no existe",
							linea,
							columna,
							HandlerFiles.getDate(), //fecha
							HandlerFiles.getTime()//hora
						});

			}
		}

		public void MostrarBaseDatos()
		{
			Console.WriteLine("********************************Base de Datos:" + this.nombre + "********************************");
			this.tablas.Mostrar();
			this.Objetos.Mostrar();
			this.procedimientos.Mostrar();
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
