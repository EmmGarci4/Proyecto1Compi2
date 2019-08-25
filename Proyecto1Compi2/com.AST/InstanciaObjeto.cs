using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class InstanciaObjeto:Expresion
	{
		string nombreUserType;
		List<Expresion> expresiones;

		public InstanciaObjeto(string nombreUserType, List<Expresion> expresiones,int linea,int columna):base(linea,columna)
		{
			this.nombreUserType = nombreUserType;
			this.expresiones = expresiones;
		}

		public string NombreUserType { get => nombreUserType; set => nombreUserType = value; }
		internal List<Expresion> Expresiones { get => expresiones; set => expresiones = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			return TipoOperacion.InstanciaObjeto;
		}

		public override object GetValor(TablaSimbolos ts)
		{
			return this;
		}

		internal object getInstanciaObjeto(TipoObjetoDB tipoAsignacion,Sesion sesion,TablaSimbolos ts)
		{
			if (tipoAsignacion.Equals(NombreUserType))
			{
				object resultadoValidacion = IsObjetoValido(sesion);
				if (resultadoValidacion.GetType() == typeof(ThrowError))
				{
					return resultadoValidacion;
				}
				if ((bool)resultadoValidacion)
				{
					UserType ut = Analizador.BuscarDB(sesion.DBActual).BuscarUserType(NombreUserType);
					Objeto nuevaInstancia = new Objeto(ut);
					int indice = 0;
					object resExp;
					foreach (KeyValuePair<string, TipoObjetoDB> atributo in ut.Atributos)
					{
						resExp = Expresiones.ElementAt(indice).GetValor(ts);
						if (Datos.IsTipoCompatibleParaAsignar(atributo.Value, resExp))
						{
							resExp = Datos.CasteoImplicito(atributo.Value.Tipo, resExp);
							nuevaInstancia.Atributos.Add(atributo.Key, resExp);
						}
						else
						{
							return new ThrowError(Util.TipoThrow.Exception,
								"Los atributos no corresponden al tipo '" + tipoAsignacion.ToString() + "'",
								Linea, Columna);
						}
						indice++;
					}
					//asignando la nueva instancia despues de todas las validaciones
					return nuevaInstancia;
				}
				else
				{
					return new ThrowError(Util.TipoThrow.Exception,
						"Los atributos no corresponden al tipo '" + tipoAsignacion.ToString() + "'",
						Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.Exception,
				"No se puede asignar un objeto '" + NombreUserType + "' a un tipo '" + tipoAsignacion.ToString() + "'",
				Linea, Columna);
			}
		}

		private object IsObjetoValido(Sesion sesion)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteUserType(NombreUserType))
				{
					//validando cantidad
					return db.BuscarUserType(NombreUserType).Atributos.Count == Expresiones.Count;
				}
				else
				{
					return new ThrowError(Util.TipoThrow.TypeDontExists,
				"El user Type '" + NombreUserType + "' no existe",
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

	}
}
