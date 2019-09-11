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
	class FuncionAgregacion : Sentencia
	{
		string nombre;
		Seleccionar select;

		public FuncionAgregacion(string nombre, Seleccionar select, int linea, int columna) : base(linea, columna)
		{
			this.Nombre = nombre;
			this.select = select;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Seleccionar Select { get => select; set => select = value; }

		public override object Ejecutar(TablaSimbolos tb, Sesion sesion)
		{
			object posibleError = select.Ejecutar(tb, sesion);
			if (posibleError != null)
			{
				if (posibleError.GetType() == typeof(ThrowError))
				{
					return posibleError;
				}

				ResultadoConsulta datos = (ResultadoConsulta)posibleError;
				if (datos.Titulos.Count == 1)
				{
					if (datos.Count > 0)
					{
						FilaDatos primeraFila = datos.ElementAt(0);
						object datoPivote = primeraFila.Datos.ElementAt(0).Valor;
						TipoObjetoDB tipo = Datos.GetTipoObjetoDB(datoPivote);
						if (tipo.Tipo == TipoDatoDB.INT || tipo.Tipo == TipoDatoDB.DOUBLE)
						{
							List<double> listadatos = ListarDatos(datos);
							switch (nombre)
							{
								case "count":
									return listadatos.Count;
								case "min":
									return GetMinimo(listadatos);
								case "max":
									return GetMayor(listadatos);
								case "sum":
									return GetSuma(listadatos);
								case "avg":
									return GetPromedio(listadatos);
								default:
									return new ThrowError(TipoThrow.Exception,
										"Las funciones de agregación '" + this.nombre + "' no existe",
										Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
							"Las funciones de agregación solo pueden operar sobre columnas de tipo númerico(int/double)",
							Linea, Columna);
						}
					}
					else
					{
						return 0;
					}
				}
				else
				{
					//error
					return new ThrowError(TipoThrow.Exception,
						"Las funciones de agregación se realizan sobre consultas con una columna",
						Linea, Columna);
				}
			}
			return null;
		}

		private double GetMinimo(List<double> listadatos)
		{
			double menor = listadatos.ElementAt(0);
			foreach (double valor in listadatos)
			{
				if (valor < menor)
				{
					menor = valor;
				}
			}
			return menor;
		}

		private double GetMayor(List<double> listadatos)
		{
			double mayor = listadatos.ElementAt(0);
			foreach (double valor in listadatos)
			{
				if (valor > mayor)
				{
					mayor = valor;
				}
			}
			return mayor;
		}

		private double GetSuma(List<double> listadatos)
		{
			double suma = 0;
			foreach (double valor in listadatos)
			{
				suma += valor;
			}
			return suma;
		}

		private double GetPromedio(List<double> listadatos)
		{
			double suma = 0;
			foreach (double valor in listadatos)
			{
				suma += valor;
			}
			return suma / listadatos.Count;
		}

		private List<double> ListarDatos(ResultadoConsulta datos)
		{
			List<double> lista = new List<double>();
			foreach (FilaDatos fila in datos)
			{
				if (double.TryParse(fila.Datos.ElementAt(0).Valor.ToString(), out double result) ){
					lista.Add(result);
				}
			}
			return lista;
		}
	}
}
