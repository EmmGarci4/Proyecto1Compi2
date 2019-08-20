using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;

namespace Proyecto1Compi2.com.AST
{
	class If:Sentencia
	{
		private Expresion condicion;
		private List<ElseIf> elseIfs;
		private List<Sentencia> cuerpoVerdadero;
		private List<Sentencia> cuerpoFalso;

		public If(Expresion condicion, List<ElseIf> elseIfs, List<Sentencia> cuerpoVerdadero, List<Sentencia> cuerpoFalso,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.elseIfs = elseIfs;
			this.cuerpoVerdadero = cuerpoVerdadero;
			this.cuerpoFalso = cuerpoFalso;
		}

		internal Expresion Condicion { get => condicion; set => condicion = value; }
		internal List<ElseIf> ElseIfs { get => elseIfs; set => elseIfs = value; }
		internal List<Sentencia> CuerpoVerdadero { get => cuerpoVerdadero; set => cuerpoVerdadero = value; }
		internal List<Sentencia> CuerpoFalso { get => cuerpoFalso; set => cuerpoFalso = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			TablaSimbolos tsLocal = new TablaSimbolos(ts);
			object respuesta = condicion.GetValor(tsLocal);
			if (respuesta.GetType()==typeof(ThrowError)) {
				return respuesta;
			}
			if ((bool)respuesta)
			{
				foreach (Sentencia sentencia in cuerpoVerdadero)
				{
					respuesta = sentencia.Ejecutar(sesion, tsLocal);
					if (respuesta != null)
					{
						if (respuesta.GetType() == typeof(ThrowError))
						{
							return respuesta;
						}
						else
						{
							//EVALUAR SI ES RETURN, BREAK O CONTINUE
						}
					}
				}
			}
			else {
				bool evaluado = false;
				if (elseIfs!=null) {
					foreach (ElseIf elseif in elseIfs) {
						respuesta = elseif.Condicion.GetValor(tsLocal);
						if (respuesta.GetType() == typeof(ThrowError))
						{
							return respuesta;
						}else 
						if ((bool)respuesta) {
							evaluado = true;
							if (respuesta!=null) {
								respuesta = elseif.Ejecutar(sesion, tsLocal);
								if (respuesta != null)
								{
									if (respuesta.GetType() == typeof(ThrowError))
									{
										return respuesta;
									}
									else
									{
										//EVALUAR SI ES RETURN, BREAK O CONTINUE
									}
								}
							}
							break;
						}
					}
				}

				if (CuerpoFalso!=null && !evaluado) {
					foreach (Sentencia sentencia in CuerpoFalso)
					{
						respuesta = sentencia.Ejecutar(sesion, tsLocal);
						if (respuesta != null)
						{
							if (respuesta.GetType() == typeof(ThrowError))
							{
								return respuesta;
							}
							else
							{
								//EVALUAR SI ES RETURN, BREAK O CONTINUE
							}
						}
					}
				}
			}

			return null;
		}
	}
}
