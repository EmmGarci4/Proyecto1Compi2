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
	class Borrar:Sentencia
	{
		string tabla;
		object algo; //para eliminar de la tabla
					 //puede ser un nombre o un acceso a collection
		Where condicion;

		public Borrar(string tabla,int linea,int columna):base(linea,columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = null;
		}

		public Borrar(string tabla, object algo, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = null;
		}
		public Borrar(string tabla,Where where, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = null;
			this.condicion = where;
		}

		public Borrar(string tabla, object algo, Where where,int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.algo = algo;
			this.condicion = where;
		}


		public string Tabla { get => tabla; set => tabla = value; }
		public object Algo { get => algo; set => algo = value; }
		internal Where Condicion { get => condicion; set => condicion = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			//VALIDANDO BASE_DATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(tabla))
				{
					Tabla miTabla = db.BuscarTabla(tabla);
					//**************************************************************************
					if (algo == null)
					{
						if (condicion == null)
						{
							miTabla.Truncar();
						}
						else
						{
							//tabla
							
							int i = 0;
							for (i = 0; i < miTabla.ContadorFilas; i++)
							{
								//AGREGANDO FILA A LA TABLA DE SIMBOLOS
								TablaSimbolos local = new TablaSimbolos(tb);
								foreach (Columna cl in miTabla.Columnas)
								{
									object dato = cl.Datos.ElementAt(i);
									Simbolo s;
									if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
									{
										s = new Simbolo(cl.Nombre, dato, new TipoObjetoDB(TipoDatoDB.INT, "int"), Linea, Columna);

									}
									else
									{
										s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);
									}

									local.AgregarSimbolo(s);

								}
								//**************************************************************************
								object condicionWhere = condicion.GetValor(local, sesion);
								if (condicionWhere != null)
								{
									if (condicionWhere.GetType() == typeof(ThrowError))
									{
										return condicionWhere;
									}
									if ((bool)condicionWhere)
									{
										//eliminar valor
										miTabla.EliminarDatos(i);
									}
								}
								//**************************************************************************
							}
						}
					}
					else
					{
						//collection
					}
					//**************************************************************************
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TableDontExists,
					"La tabla '" + tabla + "' no existe",
					Linea, Columna);
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
	}
}
