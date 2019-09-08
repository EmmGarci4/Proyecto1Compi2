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
	class Seleccionar:Sentencia
	{
		List<Expresion> listaAccesos;
		string tabla;
		Where condicion;
		OrderBy order;
		Limit limit;

		public Seleccionar(List<Expresion> listaAccesos, string tabla, int linea,int columna):base(linea,columna)
		{
			this.listaAccesos = listaAccesos;
			this.tabla = tabla;
			this.condicion = null;
			this.order = null;
			this.limit = null;
		}

		public string Tabla { get => tabla; set => tabla = value; }
		internal List<Expresion> ListaAccesos { get => listaAccesos; set => listaAccesos = value; }
		internal Where PropiedadWhere { get => condicion; set => condicion = value; }
		internal OrderBy PropiedadOrderBy { get => order; set => order = value; }
		internal Limit PropiedadLimit { get => limit; set => limit = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			
			//VALIDANDO TABLA
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteTabla(tabla))
				{
					Tabla miTabla = db.BuscarTabla(tabla);
					ResultadoConsulta resultado = new ResultadoConsulta();
					//AGREGANDO FILA A LA TABLA DE SIMBOLOS
					int i = 0;
					for (i=0;i<miTabla.ContadorFilas;i++) {
						TablaSimbolos local = new TablaSimbolos(tb);
						foreach (Columna cl in miTabla.Columnas)
						{
							object dato = cl.Datos.ElementAt(i);
							Simbolo s;
							if (cl.Tipo.Tipo == TipoDatoDB.COUNTER)
							{
								s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);

							}
							else
							{
								s = new Simbolo(cl.Nombre, dato, cl.Tipo, Linea, Columna);
							}

							local.AgregarSimbolo(s);

						}
						//SELECCIONANDO LOS DATOS
						if (listaAccesos != null)
						{
							//HAY COLUMNAS
							FilaDatos fila = new FilaDatos();
							//TITULOS
							if (resultado.Titulos == null)
							{
								int cc;
								resultado.Titulos = new List<string>();
								for (cc=0;cc<listaAccesos.Count;cc++) {
									resultado.Titulos.Add("Resultado "+(cc+1));
								}
							}
							//VALORES
							int indiceColumna;
							for (indiceColumna = 0; indiceColumna < listaAccesos.Count; indiceColumna++)
							{
								object val = listaAccesos.ElementAt(indiceColumna).GetValor(local, sesion);
								fila.Datos.Add(new ParDatos("", val));
							}
							resultado.Add(fila);
						}
						else
						{
							//COMODIN
							Simbolo val;
								FilaDatos fila = new FilaDatos();
							//TITULOS
								if (resultado.Titulos == null)
								{
								resultado.Titulos = new List<string>();
									foreach (Columna cl in miTabla.Columnas)
									{
										resultado.Titulos.Add(cl.Nombre);
									}
								}
								//llenando nombre de columnas
								foreach (Columna cl in miTabla.Columnas)
								{
									val = local.GetSimbolo(cl.Nombre);
									fila.Datos.Add(new ParDatos(cl.Nombre, val.Valor));
								}
								resultado.Add(fila);
						}

					}
					Console.WriteLine(resultado.ToString());
				}
				else
				{
						return new ThrowError(Util.TipoThrow.TableAlreadyExists,
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
