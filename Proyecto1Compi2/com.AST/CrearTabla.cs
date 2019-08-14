using com.Analisis;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearTabla : Sentencia
	{
		String nombre;
		List<object> objetos;
		bool ifExist;

		public CrearTabla(String tabla,List<object> objetos,bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.nombre = tabla;
			this.objetos = objetos;
			this.ifExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<object> Objetos { get => objetos; set => objetos = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar(Sesion sesion)
		{
			//BUSCANDO LLAVES COMPUESTAS
			List<string> llavePrimariaCompuesta = null;
			object respuesta = BuscarLlavesCompuestas();
			if (respuesta!=null) {
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return (ThrowError)respuesta;
				}
				else {
					llavePrimariaCompuesta = (List<string>)respuesta;
				}
			}
			//BUSCANDO COLUMNAS
			 respuesta= BuscarColumnas();
			if (respuesta.GetType() == typeof(ThrowError))
			{
				return (ThrowError)respuesta;
			}
			List<Columna> columnas = (List<Columna>)respuesta;

			Tabla tabla = new Tabla(Nombre);
			//VALIDANDO COLUMNAS
			foreach (Columna cl in columnas) {
				//hasta este punto, ya se validó que las columnas no estén repetidas
				//VALIDANDO COUNTER
				if (cl.Tipo.Tipo == Util.TipoDatoDB.COUNTER)
				{
					if (cl.IsPrimary)
					{
						if (llavePrimariaCompuesta == null)
						{
							//agregando a tabla
							tabla.AgregarColumna(cl);
						}
						else
						{
							//validando que no se pueda agregar una pk y una pk compuesta al mismo tiempo
							return new ThrowError(Util.TipoThrow.Exception,
							"No se puede agregar una llave primaria compuesta y una llave primaria unitaria al mismo tiempo",
							Linea, Columna);
						}
					}
					else
					{
						//validando que una columna counter debe ser llave primaria
						return new ThrowError(Util.TipoThrow.Exception,
							"Una columna de tipo counter debe ser una llave primaria",
							Linea, Columna);
					}
				}
				else {
					//VALIDANDO TIPO
					if (EsListaDeLista(cl)) {

					}
				}
			}
			return null;
		}

		private object BuscarColumnas()
		{
			List<Columna> cols = new List<Columna>();
			foreach (object ob in this.objetos) {
				if (ob.GetType() == typeof(Columna)) {
					if (!cols.Contains((Columna)ob))
					{
						cols.Add((Columna)ob);
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
							"No se pueden agregar dos columnas con el mismo nombre a una tabla",
							Linea, Columna);
					}
				}
			}
			return cols;
		}

		private object BuscarLlavesCompuestas()
		{
			//validar que no exista más de una llave compuesta
			List<string> llavePrimariaCompuesta = null;
			foreach (object ob in this.objetos) {
				if (ob.GetType()==typeof(List<string>)) {
					if (llavePrimariaCompuesta == null)
					{
						llavePrimariaCompuesta = (List<string>)ob;
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
							"No puede existir más de una llave primaria compuesta",
							Linea, Columna);
					}
				}
			}
			//validar que las llaves no se repitan
			if (llavePrimariaCompuesta!=null) {
				List<string> llaves = new List<string>();
				foreach (string llave in llavePrimariaCompuesta) {
					if (!llaves.Contains(llave))
					{
						llaves.Add(llave);
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
							"No se puede incluir dos veces una columna como llave primaria en una tabla",
							Linea, Columna);
					}
				}
				return llaves;
			}
			return llavePrimariaCompuesta;
		}

		private bool EsListaDeLista(Columna cl)
		{
			if (cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_BOOLEAN ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_DATE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_DOUBLE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_INT ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_OBJETO ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_STRING ||
								cl.Tipo.Tipo == Util.TipoDatoDB.LISTA_TIME ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_BOOLEAN ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_DATE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_DOUBLE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_INT ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_OBJETO ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_STRING ||
								cl.Tipo.Tipo == Util.TipoDatoDB.SET_TIME ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_BOOLEAN ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_DATE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_DOUBLE ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_INT ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_OBJETO ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_STRING ||
								cl.Tipo.Tipo == Util.TipoDatoDB.MAP_TIME)
			{
			}
			return false;
		}
	}
}
