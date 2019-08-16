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
	class Insertar : Sentencia
	{
		string tabla;
		List<Expresion> valores;
		List<string> columnas;

		public Insertar(string tabla, List<string> columnas, List<Expresion> valores, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.Valores = valores;
			this.Columnas = columnas;
		}

		public Insertar(string tabla, List<Expresion> valores, int linea, int columna) : base(linea, columna)
		{
			this.tabla = tabla;
			this.Valores = valores;
			this.Columnas = null;
		}

		public string NombreTabla { get => tabla; set => tabla = value; }
		public List<Expresion> Valores { get => valores; set => valores = value; }
		public List<string> Columnas { get => columnas; set => columnas = value; }

		public override object Ejecutar(Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALLIDANDO TABLA
				if (db.ExisteTabla(tabla))
				{
					Tabla tab = db.BuscarTabla(tabla);
					//**************************************************************************
					if (this.columnas == null)
					{
						//INSERSION NORMAL
						if (this.valores.Count == tab.Columnas.Count)
						{
							//VALIDANDO
							int contador = 0;
							foreach (Columna cl in tab.Columnas) {
								if (!Datos.IsTipoCompatible(cl.Tipo,this.valores.ElementAt(contador).GetValor(new TablaSimbolos(0,"Global"))) ) {
									return new ThrowError(Util.TipoThrow.ValuesException,
									"El valor No."+(contador+1)+" no concuerda con el tipo de dato '"+cl.Nombre+"'("+cl.Tipo.ToString()+")",
									Linea, Columna);
								}
								contador++;
							}
							//INSERTANDO
							
						}
						else {
							return new ThrowError(Util.TipoThrow.ValuesException,
								"La cantidad de valores no concuerda con la cantidad de columnas",
								Linea, Columna);
						}
					}
					else {
						//INSERSION ESPECIAL

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
