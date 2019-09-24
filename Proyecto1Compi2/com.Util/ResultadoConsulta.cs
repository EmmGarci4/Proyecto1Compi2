using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class ResultadoConsulta:List<FilaDatos>
	{
		List<string> titulos;
		List<TipoObjetoDB> tipos;

		public ResultadoConsulta() : base()
		{
			titulos = new List<string>();
			tipos = new List<TipoObjetoDB>();
		}


		public List<string> Titulos { get => titulos; set => titulos = value; }
		public List<TipoObjetoDB> Tipos { get => tipos; set => tipos = value; }

		public override string ToString()
		{
			StringBuilder resultado = new StringBuilder();
			resultado.Append("<table>\n");
			//CABECERA
			if (this.titulos != null)
			{
				resultado.Append("\t<tr>\n");
				foreach (string titulo in titulos)
				{
					resultado.Append("\t\t<th>");
					resultado.Append(titulo);
					resultado.Append("</th>\n");
				}
				resultado.Append("</tr>\n");
				//CONTENIDO
				foreach (FilaDatos fila in this)
				{
					resultado.Append("\t<tr>\n");
					foreach (ParDatos par in fila.Datos)
					{
						resultado.Append("\t\t<td>");
						if (par.Valor.GetType() == typeof(CollectionListCql))
						{
							resultado.Append(((CollectionListCql)par.Valor).GetLinealizado());
						}
						else if (par.Valor.GetType() == typeof(CollectionMapCql))
						{
							resultado.Append(((CollectionMapCql)par.Valor).GetLinealizado());
						}
						else if (par.Valor.GetType() == typeof(Objeto))
						{
							resultado.Append(((Objeto)par.Valor).GetLinealizado());
						}
						else
						{
							resultado.Append(par.Valor);
						}
						resultado.Append("</td>\n");
					}
					resultado.Append("\t</tr>\n");
				}
			}
			resultado.Append("</table>");
			return resultado.ToString();
		}

		internal int getMenor(int index)
		{
			if (this.Count>0) {
				switch (tipos.ElementAt(index).Tipo)
				{
					case TipoDatoDB.INT:
						{
							int menor = (int)this.ElementAt(0).Datos.ElementAt(index).Valor;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								int val = (int)fila.Datos.ElementAt(index).Valor;
								if (val < menor)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
					case TipoDatoDB.DOUBLE:
						{
							double menor = (double)this.ElementAt(0).Datos.ElementAt(index).Valor;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								double val = (double)fila.Datos.ElementAt(index).Valor;
								if (val < menor)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
					case TipoDatoDB.STRING:
						{
							string menor = (string)this.ElementAt(0).Datos.ElementAt(index).Valor;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								string val = (string)fila.Datos.ElementAt(index).Valor;
								if (val.CompareTo(menor)==-1)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
					case TipoDatoDB.TIME:
						{
							MyDateTime menor = (MyDateTime)this.ElementAt(0).Datos.ElementAt(index).Valor;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								MyDateTime val = (MyDateTime)fila.Datos.ElementAt(index).Valor;
								if (val.CompareTo(menor) == -1)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
					case TipoDatoDB.DATE:
						{
							MyDateTime menor = (MyDateTime)this.ElementAt(0).Datos.ElementAt(index).Valor;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								MyDateTime val = (MyDateTime)fila.Datos.ElementAt(index).Valor;
								if (val.CompareTo(menor) == -1)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
					case TipoDatoDB.BOOLEAN:
						{
							int menor = (bool)this.ElementAt(0).Datos.ElementAt(index).Valor?1:0;
							int indexMenor = 0;
							int contador = 0;
							foreach (FilaDatos fila in this)
							{
								int val = (bool)fila.Datos.ElementAt(index).Valor ? 1 : 0;
								if (val < menor)
								{
									menor = val;
									indexMenor = contador;
								}
								contador++;
							}
							return indexMenor;
						}
				}
			}
			return -1;
		}
	}
}
