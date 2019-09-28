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

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (this.nombre=="errors") {
				return new ThrowError(Util.TipoThrow.TableAlreadyExists,
					"La tabla 'errors' ya existe",
					Linea, Columna);
			}
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
					bool hayCounter = false;
					//VALIDANDO COLUMNAS
					foreach (Columna cl in columnas)
					{
						//hasta este punto, ya se validó que las columnas no estén repetidas
						//VALIDANDO COUNTER
						if (cl.Tipo.Tipo == Util.TipoDatoDB.COUNTER)
						{
							//AGREGANDO COLUMNA A TABLA
							hayCounter=true;
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
								hayCounter = false;
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
								if (!Datos.IsPrimitivo(cl.Tipo.Tipo)) {
									return new ThrowError(Util.TipoThrow.Exception,
										"No se puede seleccionar una columna tipo '"+cl.Tipo.ToString()+"' como llave primaria",
										Linea, Columna);
								}
							}
							//VALIDANDO TIPO
							if (!EsListaDeLista(cl))
							{
								//más validaciones?
								if (cl.Tipo.Tipo==TipoDatoDB.OBJETO) {
									object res = Operacion.ExisteObjeto(cl.Tipo, sesion, Linea, Columna);
									if (res!=null) {
										if (res.GetType()==typeof(ThrowError)) {
											return res;
										}
									}
								}
								if (cl.Tipo.Tipo==TipoDatoDB.LISTA_OBJETO|| cl.Tipo.Tipo == TipoDatoDB.SET_OBJETO|| cl.Tipo.Tipo == TipoDatoDB.MAP_OBJETO) {
									TipoObjetoDB tipoAdentro = Datos.GetTipoObjetoDBPorCadena(cl.Tipo.Nombre);
									if (tipoAdentro.Tipo == TipoDatoDB.OBJETO)
									{
										object res = Operacion.ExisteObjeto(tipoAdentro, sesion, Linea, Columna);
										if (res != null)
										{
											if (res.GetType() == typeof(ThrowError))
											{
												return res;
											}
										}
									}
								}
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
						hayCounter = false;
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
				if (Datos.IsLista(GetTipoDatoConNombre(cl.Tipo.Nombre)))
				{
					return true;
				}
			}
			return false;
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
				case "list<int>":
				case "list<double>":
				case "list<boolean>":
				case "list<date>":
				case "list<time>":
					return TipoDatoDB.LISTA_PRIMITIVO;
				//sets
				case "set<string>":
				case "set<int>":
				case "set<double>":
				case "set<boolean>":
				case "set<date>":
				case "set<time>":
					return TipoDatoDB.SET_PRIMITIVO;
				//maps
				case "map<string>":
				case "map<int>":
				case "map<double>":
				case "map<boolean>":
				case "map<date>":
				case "map<time>":
					return TipoDatoDB.MAP_PRIMITIVO;
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
