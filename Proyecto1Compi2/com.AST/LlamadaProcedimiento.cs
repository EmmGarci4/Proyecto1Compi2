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
	class LlamadaProcedimiento : Sentencia
	{
		string nombre;
		List<Expresion> parametros;

		public LlamadaProcedimiento(string nombre, List<Expresion> parametros, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal List<Expresion> Parametros { get => parametros; set => parametros = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				string llave = getLlave(ts,sesion);
				if (db.ExisteProcedimiento(llave))
				{
					Procedimiento funcion = db.BuscarProcedimiento(llave);
					//**********************************************************************************
					List<object> valores = new List<object>();
					//VALIDAR PARAMETROS 
					if (funcion.Parametros.Count == parametros.Count)
					{
						int contador = 0;
						foreach (Parametro vals in funcion.Parametros)
						{
							if (Datos.IsTipoCompatibleParaAsignar(vals.Tipo, parametros.ElementAt(contador).GetValor(ts,sesion)))
							{
								object nuevoDato = Datos.CasteoImplicito(vals.Tipo.Tipo, parametros.ElementAt(contador).GetValor(ts,sesion));
								valores.Add(nuevoDato);
							}
							else
							{
								return new ThrowError(Util.TipoThrow.Exception,
									"No se puede asignar el valor a la variable '" + vals.Nombre + "'",
									Linea, Columna);
							}
						}
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
						"La cantidad de parámetros es incorrecta",
						Linea, Columna);
					}

					funcion.pasarParametros(valores);
					object res = funcion.Ejecutar(ts,sesion);
					if (res != null)
					{
						return res;
					}
					funcion.LimpiarParametros();
					return res;
					//********************************************************************************
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TypeAlreadyExists,
				"El procedimiento '" + llave + "' no existe",
				Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}	
		}

		internal string getLlave(TablaSimbolos ts,Sesion sesion)
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(nombre + "(");
			int contador = 0;
			foreach (Expresion ex in parametros)
			{
				TipoOperacion t = ex.GetTipo(ts,sesion);
				if (t == TipoOperacion.Numero)
				{
					if (ex.GetValor(ts,sesion).ToString().Contains("."))
					{
						llave.Append("double");
					}
					else
					{
						llave.Append("int");
					}
				}
				else
				{
					llave.Append(t.ToString().ToLower());
				}
				if (contador < this.parametros.Count - 1)
				{
					llave.Append(",");
				}
				contador++;
			}
			llave.Append(")");
			return llave.ToString();
		}
	}
}
