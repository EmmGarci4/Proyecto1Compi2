using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Switch:Sentencia
	{
		Expresion expresion;
		List<Case> listaCase;
		List<Sentencia> default_;

		public Switch(Expresion expresion, List<Case> listaCase, List<Sentencia> default_,int linea,int columna):base(linea,columna)
		{
			this.expresion = expresion;
			this.listaCase = listaCase;
			this.default_ = default_;
		}

		internal Expresion Expresion { get => expresion; set => expresion = value; }
		internal List<Case> ListaCase { get => listaCase; set => listaCase = value; }
		internal List<Sentencia> Default_ { get => default_; set => default_ = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			object exVal = expresion.GetValor(ts);
			if (exVal.GetType() == typeof(ThrowError))
			{
				return exVal;
			}
			TipoOperacion tipoexp = this.expresion.GetTipo(ts);
			if (Datos.IsPrimitivo(tipoexp))
			{
				//VALIDANDO TIPO DE CASES
				foreach (Case cs in listaCase) {
					object res = cs.Exp.GetValor(ts);
					if (res.GetType()==typeof(ThrowError)) {
						return res;
					}
					if (tipoexp != cs.Exp.GetTipo(ts)) {
						return new ThrowError(Util.TipoThrow.Exception,
								"La expresión debe ser del mismo tipo que la evaluada en el switch",
								Linea, Columna);
					}
				}
				bool evaluado = false;
				bool ejecutar = true;
				if (exVal.GetType()==typeof(ThrowError)) {
					return exVal;
				}
				foreach (Case cs in ListaCase) {
					if (exVal.Equals(cs.Exp.GetValor(ts))||(evaluado&&ejecutar)) {
						evaluado = true;
						object res = cs.Ejecutar(sesion, ts);
						if (res!=null) {
							if (res.GetType() == typeof(ThrowError))
							{
								return res;
							}
							else if (res.GetType() == typeof(Break))
							{
								ejecutar = false;
								break;
							}
						}
					}
				}

				if (default_!=null && !evaluado) {
					foreach (Sentencia sentencia in default_)
					{
						object respuesta = sentencia.Ejecutar(sesion, ts);
						if (respuesta != null)
						{
							if (respuesta.GetType() == typeof(ThrowError))
							{
								return respuesta;
							}
							else if (respuesta.GetType() == typeof(Break))
							{
								evaluado = true;
								break;
							}
							{
								//EVALUAR SI ES RETURN, O CONTINUE
							}
						}
					}

				}
			}
			else {
				return new ThrowError(Util.TipoThrow.Exception,
								"La expresión a evaluar debe ser un entero, decimal, booleano o string",
								Linea, Columna);
			}
			return null;
		}
	}
}
