using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
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

		public CrearTabla(String tabla, List<object> objetos, bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.nombre = tabla;
			this.objetos = objetos;
			this.ifExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<object> Objetos { get => objetos; set => objetos = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			//VALIDANDO TABLA
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (!db.ExisteTabla(Nombre))
				{

					//BUSCANDO LLAVES COMPUESTAS
					List<string> llavePrimariaCompuesta = null;
					object respuesta = BuscarLlavesCompuestas();
					if (respuesta != null)
					{
						if (respuesta.GetType() == typeof(ThrowError))
						{
							return (ThrowError)respuesta;
						}
						else
						{
							llavePrimariaCompuesta = (List<string>)respuesta;
						}
					}
					//BUSCANDO COLUMNAS
					respuesta = BuscarColumnas();
					if (respuesta.GetType() == typeof(ThrowError))
					{
						return (ThrowError)respuesta;
					}
					List<Columna> columnas = (List<Columna>)respuesta;

					Tabla tabla = new Tabla(Nombre);
					//VALIDANDO COLUMNAS
					foreach (Columna cl in columnas)
					{
						//hasta este punto, ya se validó que las columnas no estén repetidas
						//VALIDANDO COUNTER
						if (cl.Tipo.Tipo == Util.TipoDatoDB.COUNTER)
						{
							if (cl.IsPrimary)
							{
								if (llavePrimariaCompuesta == null)
								{
									//AGREGANDO COLUMNA A TABLA
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
						else
						{
							//VALIDANDO COUNTER EN LLAVES PRIMARIAS COMPUESTAS
							if (cl.IsPrimary) {
								bool hayCounter = false;
								foreach (Columna col in tabla.Columnas)
								{
									if (col.Tipo.Tipo == TipoDatoDB.COUNTER)
									{
										hayCounter = true;
										break;
									}
								}
								if (hayCounter) {
									if (cl.Tipo.Tipo!=TipoDatoDB.COUNTER) {
										if (cl.IsPrimary && cl.Tipo.Tipo != TipoDatoDB.COUNTER)
										{
											return new ThrowError(Util.TipoThrow.Exception,
										"Si existe una columna de tipo Counter, todas las llaves primarias deben ser de este tipo",
										Linea, Columna);
										}
									}
								}
							}
							//VALIDANDO TIPO
							if (!EsListaDeLista(cl))
							{
								//más validaciones?
								//AGREGANDO COLUMNA A TABLA
								tabla.AgregarColumna(cl);
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"No se permiten listas de listas como tipo de dato en una columna",
									Linea, Columna);
							}
						}
					}

					//VALIDANDO LLAVES PRIMARIAS
					if (llavePrimariaCompuesta!=null) {
						foreach (string llave in llavePrimariaCompuesta)
						{
							if (tabla.ExisteColumna(llave))
							{
								tabla.BuscarColumna(llave).IsPrimary = true;
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
								"No existe la columna '" + llave + "' para asignarla como llave primaria",
								Linea, Columna);
							}
						}
						//VALIDANDO COUNTER EN LLAVES PRIMARIAS COMPUESTAS
						bool hayCounter = false;
						foreach (Columna cl in tabla.Columnas) {
							if (cl.Tipo.Tipo==TipoDatoDB.COUNTER) {
								hayCounter = true;
								break;
							}
						}
						if (hayCounter) {
							foreach (Columna cl in tabla.Columnas)
							{
								if (cl.IsPrimary && cl.Tipo.Tipo!=TipoDatoDB.COUNTER) {
									return new ThrowError(Util.TipoThrow.Exception,
								"Si existe una columna de tipo Counter, todas las llaves primarias deben ser de este tipo",
								Linea, Columna);
								}
							}
						}
					}
					//AGREGANDO TABLA A BASE DE DATOS
					db.AgregarTabla(tabla);
				}
				else
				{
					if (!IfExist)
					{
						return new ThrowError(Util.TipoThrow.TableAlreadyExists,
							"La tabla '" + Nombre + "' ya existe",
							Linea, Columna);
					}
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}

			return null;
		}

		private object BuscarColumnas()
		{
			List<Columna> cols = new List<Columna>();
			foreach (object ob in this.objetos)
			{
				if (ob.GetType() == typeof(Columna))
				{
					if (!cols.Contains((Columna)ob))
					{
						cols.Add((Columna)ob);
					}
					else
					{
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
			foreach (object ob in this.objetos)
			{
				if (ob.GetType() == typeof(List<string>))
				{
					if (llavePrimariaCompuesta == null)
					{
						llavePrimariaCompuesta = (List<string>)ob;
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"No puede existir más de una llave primaria compuesta",
							Linea, Columna);
					}
				}
			}
			//validar que las llaves no se repitan
			if (llavePrimariaCompuesta != null)
			{
				List<string> llaves = new List<string>();
				foreach (string llave in llavePrimariaCompuesta)
				{
					if (!llaves.Contains(llave))
					{
						llaves.Add(llave);
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
							"No se puede incluir dos veces una columna como llave primaria en una tabla",
							Linea, Columna);
					}
				}
				return llaves;
			}
			return llavePrimariaCompuesta;
		}

		public static bool EsListaDeLista(Columna cl)
		{
			if (cl.Tipo.Tipo == TipoDatoDB.LISTA_OBJETO || cl.Tipo.Tipo == TipoDatoDB.SET_OBJETO || cl.Tipo.Tipo == TipoDatoDB.MAP_OBJETO)
			{
				if (IsLista(GetTipoDatoConNombre(cl.Tipo.Nombre)))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsLista(TipoDatoDB td)
		{
			switch (td)
			{
				case TipoDatoDB.LISTA_OBJETO:
				case TipoDatoDB.SET_OBJETO:
				case TipoDatoDB.MAP_OBJETO:
				case TipoDatoDB.MAP_BOOLEAN:
				case TipoDatoDB.LISTA_BOOLEAN:
				case TipoDatoDB.SET_BOOLEAN:
				case TipoDatoDB.LISTA_DATE:
				case TipoDatoDB.SET_DATE:
				case TipoDatoDB.MAP_DATE:
				case TipoDatoDB.LISTA_DOUBLE:
				case TipoDatoDB.SET_DOUBLE:
				case TipoDatoDB.MAP_DOUBLE:
				case TipoDatoDB.LISTA_INT:
				case TipoDatoDB.SET_INT:
				case TipoDatoDB.MAP_INT:
				case TipoDatoDB.LISTA_STRING:
				case TipoDatoDB.SET_STRING:
				case TipoDatoDB.MAP_STRING:
				case TipoDatoDB.SET_TIME:
				case TipoDatoDB.LISTA_TIME:
				case TipoDatoDB.MAP_TIME:
					return true;
				case TipoDatoDB.OBJETO:
				case TipoDatoDB.BOOLEAN:
				case TipoDatoDB.DOUBLE:
				case TipoDatoDB.INT:
				case TipoDatoDB.STRING:
				case TipoDatoDB.COUNTER:
				case TipoDatoDB.TIME:
				case TipoDatoDB.DATE:
				case TipoDatoDB.NULO:
					return false;
				default:
					return false;
			}
		}

		private static TipoDatoDB GetTipoDatoConNombre(string nombre)
		{
			switch (nombre.ToLower())
			{
				case "string":
					return TipoDatoDB.STRING;
				case "int":
					return TipoDatoDB.INT;
				case "double":
					return TipoDatoDB.DOUBLE;
				case "boolean":
					return TipoDatoDB.BOOLEAN;
				case "date":
					return TipoDatoDB.DATE;
				case "time":
					return TipoDatoDB.TIME;
				case "counter":
					return TipoDatoDB.COUNTER;
				//listas
				case "list<string>":
					return TipoDatoDB.LISTA_STRING;
				case "list<int>":
					return TipoDatoDB.LISTA_INT;
				case "list<double>":
					return TipoDatoDB.LISTA_DOUBLE;
				case "list<boolean>":
					return TipoDatoDB.LISTA_BOOLEAN;
				case "list<date>":
					return TipoDatoDB.LISTA_DATE;
				case "list<time>":
					return TipoDatoDB.LISTA_TIME;
				//sets
				case "set<string>":
					return TipoDatoDB.SET_STRING;
				case "set<int>":
					return TipoDatoDB.SET_INT;
				case "set<double>":
					return TipoDatoDB.SET_DOUBLE;
				case "set<boolean>":
					return TipoDatoDB.SET_BOOLEAN;
				case "set<date>":
					return TipoDatoDB.SET_DATE;
				case "set<time>":
					return TipoDatoDB.SET_TIME;
				//maps
				case "map<string>":
					return TipoDatoDB.MAP_STRING;
				case "map<int>":
					return TipoDatoDB.MAP_INT;
				case "map<double>":
					return TipoDatoDB.MAP_DOUBLE;
				case "map<boolean>":
					return TipoDatoDB.MAP_BOOLEAN;
				case "map<date>":
					return TipoDatoDB.MAP_DATE;
				case "map<time>":
					return TipoDatoDB.MAP_TIME;
				default:
					if (nombre.ToLower().StartsWith("list"))
					{
						return TipoDatoDB.LISTA_OBJETO;
					}
					else if (nombre.ToLower().StartsWith("set"))
					{
						return TipoDatoDB.SET_OBJETO;
					}
					else if (nombre.ToLower().StartsWith("map"))
					{
						return TipoDatoDB.MAP_OBJETO;
					}

					return TipoDatoDB.OBJETO;
			}
		}
	}
}
