using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Proyecto1Compi2.com.AST
{
	internal class OrderBy:Sentencia
	{
		List<PropOrderBy> propiedades;
		ResultadoConsulta resultado;

		public OrderBy(int linea,int columna):base(linea,columna)
		{
			this.propiedades = new List<PropOrderBy>(); ;
		}

		internal List<PropOrderBy> Propiedades { get => propiedades; set => propiedades = value; }

		public override object Ejecutar(TablaSimbolos tb,Sesion sesion)
		{
			if (resultado != null)
			{
				if (propiedades.Count>0) {
					//repetir
					PropOrderBy prop = this.propiedades.ElementAt(0);
					int index = resultado.Titulos.IndexOf(prop.Columna);
					if (index >= 0)
					{
						TipoObjetoDB tipo = resultado.Tipos.ElementAt(index);
						if (Datos.IsPrimitivo(tipo.Tipo))
						{
							return Ordenar(index,tipo);
						}
						else {
							if (tipo.Nombre.Equals("%%"))
							{
								return new ThrowError(TipoThrow.Exception,
								"No se puede aplicar la clausula ORDER BY sobre la columna '" + resultado.Titulos.ElementAt(index) + "'",
								Linea, Columna);
							}
							else {
								return new ThrowError(Util.TipoThrow.Exception,
								"No se puede aplicar la clausula ORDER BY sobre una columna tipo '" + tipo.ToString() + "'",
								Linea, Columna);
							}
						}
					}
					else {
						return new ThrowError(Util.TipoThrow.ColumnException,
							"La columna '"+prop.Columna+"' no existe en el resultado de la sentencia SELECT",
							Linea, Columna);
					}
				//*****
				}
			}
			else
			{
				//error
				return new ThrowError(Util.TipoThrow.Exception,
					"No se puede ejecutar la clausula ORDER BY",
					Linea, Columna);
			}
			return null;
		}

		private ResultadoConsulta Ordenar(int index, TipoObjetoDB tipo)
		{
			ResultadoConsulta nuevo = new ResultadoConsulta();
			nuevo.Tipos = resultado.Tipos;
			nuevo.Titulos = resultado.Titulos;
			while (resultado.Count>0) {
				int menor = resultado.getMenor(index);
				if (menor>=0) {
					nuevo.Add(resultado.ElementAt(menor));
					resultado.RemoveAt(menor);
				}
			}
			return nuevo;
		}

		internal void PasarResultado(ResultadoConsulta resultado)
		{
			this.resultado = resultado;
		}
	}

	internal class PropOrderBy
	{
		string columna;
		bool isAsc;

		public PropOrderBy(string acceso, bool isAsc)
		{
			this.columna = acceso;
			this.IsAsc = isAsc;
		}

		public bool IsAsc { get => isAsc; set => isAsc = value; }
		internal string Columna { get => columna; set => columna = value; }
	}
}