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
		List<Tabla> tablas;
		List<UserType> objetos;
		List<Procedimiento> procedimientos;

		public BaseDatos(String nombre)
		{
			this.nombre = nombre;
			this.tablas = new List<Tabla>();
			this.objetos = new List<UserType>();
			this.procedimientos = new List<Procedimiento>();
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Tabla> Tablas { get => tablas; set => tablas = value; }
		internal List<UserType> UserTypes { get => objetos; set => objetos = value; }
		internal List<Procedimiento> Procedimientos { get => procedimientos; set => procedimientos = value; }

		//*****************************TABLAS****************************************************
		public void AgregarTabla(Tabla tb)
		{
			this.tablas.Add(tb);
		}

		public bool ExisteTabla(string nombre)
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

		internal Tabla BuscarTabla(string nombreTabla)
		{
			foreach (Tabla tb in this.tablas)
			{
				if (tb.Nombre.Equals(nombreTabla))
				{
					return tb;
				}
			}
			return null;
		}

		internal void EliminarTabla(string nombre)
		{
			this.tablas.Remove(BuscarTabla(nombre));
		}

		//*****************************USER TYPES************************************************
		public void AgregarUserType(UserType obj)
		{
			this.objetos.Add(obj);
		}

		public bool ExisteUserType(string nombre)
		{
			foreach (UserType tb in this.UserTypes)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		internal UserType BuscarUserType(string nombre)
		{
			foreach (UserType tb in this.objetos)
			{
				if (tb.Nombre.Equals(nombre))
				{
					return tb;
				}
			}
			return null;
		}

		internal void EliminarUserType(string nombre)
		{
			this.objetos.Remove(BuscarUserType(nombre));
		}

		//*****************************PROCEDIMIENTOS********************************************
		public void AgregarProcedimiento(Procedimiento obj)
		{
			this.procedimientos.Add(obj);
		}

		public bool ExisteProcedimiento(string nombre)
		{
			foreach (Procedimiento tb in this.procedimientos)
			{
				if (tb.GetLlave().Equals(nombre))
				{
					return true;
				}
			}
			return false;
		}

		public Procedimiento BuscarProcedimiento(string nombre)
		{
			foreach (Procedimiento tb in this.procedimientos)
			{
				if (tb.GetLlave().Equals(nombre))
				{
					return tb;
				}
			}
			return null;
		}

		internal void EliminarProcedimiento(string nombre)
		{
			this.procedimientos.Remove(BuscarProcedimiento(nombre));
		}

		//*****************************MOSTRAR***************************************************

		public void MostrarBaseDatos()
		{
			Console.WriteLine("********************************Base de Datos:" + this.nombre + "********************************");
			MostrarTablas();
			MostrarUserTypes();
			MostrarProcedimientos();
		}

		private void MostrarProcedimientos()
		{
			foreach (Procedimiento pr in this.procedimientos)
			{
				Console.WriteLine("Procedimiento" + pr.Nombre);
				//pr.Mostrar();
			}
		}

		private void MostrarUserTypes()
		{
			foreach (UserType user in this.UserTypes)
			{
				Console.WriteLine("UserType: " + user.Nombre);
				//user.Mostrar();
			}
		}

		private void MostrarTablas()
		{
			foreach (Tabla tb in this.tablas)
			{
				Console.WriteLine("Tabla: " + tb.Nombre);
				//tb.MostrarCabecera();
				//tb.MostrarDatos();
			}
		}

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			cadena.Append("\n<\n");
			cadena.Append("\"NAME\"=\"" + Nombre + "\",\n");
			cadena.Append("\"DATA\"=[");
			if (UserTypes.Count > 0)
			{
				IEnumerator<UserType> enumerator = this.UserTypes.GetEnumerator();
				bool hasNext = enumerator.MoveNext();
				while (hasNext)
				{
					UserType i = enumerator.Current;
					cadena.Append(i.ToString());
					hasNext = enumerator.MoveNext();
					if (hasNext)
					{
						cadena.Append(",");
					}
				}
				enumerator.Dispose();

				if (procedimientos.Count > 0 || Tablas.Count > 0)
				{
					cadena.Append(",");
				}
			}
			if (Procedimientos.Count > 0)
			{
				IEnumerator<Procedimiento> enumerator = this.procedimientos.GetEnumerator();
				bool hasNext = enumerator.MoveNext();
				while (hasNext)
				{
					Procedimiento i = enumerator.Current;
					cadena.Append(i.ToString());
					hasNext = enumerator.MoveNext();
					if (hasNext)
					{
						cadena.Append(",");
					}
				}
				enumerator.Dispose();
				if (tablas.Count > 0)
				{
					cadena.Append(",");
				}
			}
			if (Tablas.Count > 0)
			{
				IEnumerator<Tabla> enumerator = this.tablas.GetEnumerator();
				bool hasNext = enumerator.MoveNext();
				while (hasNext)
				{
					Tabla i = enumerator.Current;
					cadena.Append(i.ToString());
					hasNext = enumerator.MoveNext();
					if (hasNext)
					{
						cadena.Append(",");
					}
				}
				enumerator.Dispose();
			}
			cadena.Append("]\n");
			cadena.Append(">");
			return cadena.ToString();
		}
	}
}
