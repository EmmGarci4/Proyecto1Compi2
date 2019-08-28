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

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			List<ThrowError> errores = new List<ThrowError>();
			object exVal = expresion.GetValor(ts,sesion);
			if (exVal.GetType() == typeof(ThrowError))
			{
				return exVal;
			}
			TipoOperacion tipoexp = this.expresion.GetTipo(ts,sesion);
			if (Datos.IsPrimitivo(tipoexp))
			{
				//VALIDANDO TIPO DE CASES
				foreach (Case cs in listaCase) {
					object res = cs.Exp.GetValor(ts,sesion);
					if (res.GetType()==typeof(ThrowError)) {
						return res;
					}
					if (tipoexp != cs.Exp.GetTipo(ts,sesion)) {
						return new ThrowError(Util.TipoThrow.Exception,
								"La expresión debe ser del mismo tipo que la evaluada en el switch",
								Linea, Columna);
					}
				}
				if (exVal.GetType()==typeof(ThrowError)) {
					return exVal;
				}
				bool evaluado = false;
				bool ejecutar = true;

				foreach (Case cs in ListaCase) {
					if (exVal.Equals(cs.Exp.GetValor(ts,sesion))||(evaluado&&ejecutar)) {
						evaluado = true;
						//EJECUTANDO SENTENCIAS ******************************************************************
						object res = cs.Ejecutar(ts,sesion);
						if (res!=null) {
							if (res.GetType() == typeof(ThrowError))
							{
								errores.Add((ThrowError)res);
							}
							else if (res.GetType() == typeof(List<ThrowError>))
							{
								errores.AddRange((List<ThrowError>)res);
							}
							else if (res.GetType() == typeof(Break))
							{
								ejecutar = false;
								break;
							}
							else {
								//continue - return
								if (errores.Count > 0) return errores;
								return res;
							}
						}
					}
				}

				if (default_!=null && !evaluado) {
					TablaSimbolos local = new TablaSimbolos(ts);
					foreach (Sentencia sentencia in default_)
					{
						//EJECUTANDO SENTENCIAS ******************************************************************
						object res = sentencia.Ejecutar(local,sesion);
						if (res != null)
						{
							if (res.GetType() == typeof(ThrowError))
							{
								errores.Add((ThrowError)res);
							}
							else if (res.GetType() == typeof(List<ThrowError>))
							{
								errores.AddRange((List<ThrowError>)res);
							}
							else if (res.GetType() == typeof(Break))
							{
								ejecutar = false;
								break;
							}
							else
							{
								//continue - return
								if (errores.Count > 0) return errores;
								return res;
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
			if (errores.Count > 0) return errores;
			return null;
		}
	}
}
