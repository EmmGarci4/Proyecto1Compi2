using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Proyecto1Compi2.com.AST
{
	internal class OrderBy : Sentencia
	{
		List<PropOrderBy> propiedades;
		ResultadoConsulta resultado;

		public OrderBy(int linea, int columna) : base(linea, columna)
		{
			this.propiedades = new List<PropOrderBy>(); ;
		}

		internal List<PropOrderBy> Propiedades { get => propiedades; set => propiedades = value; }

		public override object Ejecutar(TablaSimbolos tb, Sesion sesion)
		{
			if (resultado != null)
			{
				if (propiedades.Count > 0)
				{
					//repetir
					int indexPropiedades = 0;
					return OrdenarResultado(indexPropiedades, resultado);
					//*****
				}
				return resultado;
			}
			else
			{
				//error
				return new ThrowError(Util.TipoThrow.Exception,
					"No se puede ejecutar la clausula ORDER BY",
					Linea, Columna);
			}
		}

		private object OrdenarResultado(int indexPropiedades, ResultadoConsulta consulta)
		{

			PropOrderBy prop = this.propiedades.ElementAt(indexPropiedades);
			string nombre = prop.Columna;
			int index = consulta.Titulos.IndexOf(nombre);
			if (index >= 0)
			{
				TipoObjetoDB tipo = consulta.Tipos.ElementAt(index);
				if (Datos.IsPrimitivo(tipo.Tipo))
				{
					ResultadoConsulta res = Ordenar(index, tipo, consulta);
					if (!prop.IsAsc) res.Reverse();
					indexPropiedades++;
					if (indexPropiedades < this.propiedades.Count)
					{
						ResultadoConsulta[] consultas = res.Dividir(index, tipo);
						if (consultas.Length == 1)
						{
							return consultas[0];
						}
						else
						{

							int contador = 0;
							foreach (ResultadoConsulta con in consultas)
							{
								object val = OrdenarResultado(indexPropiedades, con);
								if (val != null)
								{
									if (val.GetType() == typeof(ThrowError))
									{
										return val;
									}
									consultas.SetValue((ResultadoConsulta)val, contador);
								}
								contador++;
							}

							//unir todas
							ResultadoConsulta final = new ResultadoConsulta();
							foreach (ResultadoConsulta con in consultas)
							{
								final.AddRange(con);
							}
							return final;
						}
					}
					return res;
				}
				else
				{
					if (tipo.Nombre.Equals("%%"))
					{
						return new ThrowError(TipoThrow.Exception,
						"No se puede aplicar la clausula ORDER BY sobre la columna '" + consulta.Titulos.ElementAt(index) + "'",
						Linea, Columna);
					}
					else
					{
						return new ThrowError(Util.TipoThrow.Exception,
						"No se puede aplicar la clausula ORDER BY sobre una columna tipo '" + tipo.ToString() + "'",
						Linea, Columna);
					}
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.ColumnException,
					"La columna '" + prop.Columna + "' no existe en el resultado de la sentencia SELECT",
					Linea, Columna);
			}
		}

		private ResultadoConsulta Ordenar(int index, TipoObjetoDB tipo, ResultadoConsulta consulta)
		{
			ResultadoConsulta nuevo = new ResultadoConsulta();
			nuevo.Tipos = consulta.Tipos;
			nuevo.Titulos = consulta.Titulos;
			while (consulta.Count > 0)
			{
				int menor = consulta.getMenor(index);
				if (menor >= 0)
				{
					nuevo.Add(consulta.ElementAt(menor));
					consulta.RemoveAt(menor);
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