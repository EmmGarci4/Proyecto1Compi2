using System;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	internal class Limit:Sentencia
	{
		Expresion limitante;
		ResultadoConsulta resultado;

		public Limit(Expresion limitante,int linea,int columna):base(linea,columna)
		{
			this.limitante = limitante;
		}

		internal Expresion Limitante { get => limitante; set => limitante = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (resultado != null)
			{
				object valor = limitante.GetValor(tb, sesion);
				TipoOperacion tipo = limitante.GetTipo(tb, sesion);
				if (tipo == TipoOperacion.Numero)
				{
					if (int.TryParse(valor.ToString(), out int num))
					{
						if (num>resultado.Count) {
							resultado.RemoveRange(num, resultado.Count - num);
						}
						return resultado;
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,
										"No se puede ejecutar la clausula LIMIT con un decimal",
										Linea, Columna);
					}
				}
				else {
					return new ThrowError(Util.TipoThrow.Exception,
										"No se puede ejecutar la clausula LIMIT con un dato no numérico",
										Linea, Columna);
				}
			}
			else {
				//error
				return new ThrowError(Util.TipoThrow.Exception,
					"No se puede ejecutar la clausula LIMIT",
					Linea, Columna);
			}
		}

		internal void PasarResultado(ResultadoConsulta res)
		{
			this.resultado = res;
		}
	}
}