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

		public ResultadoConsulta(List<string> titulos, List<TipoObjetoDB> tipos)
		{
			this.titulos = titulos;
			this.tipos = tipos;
		}

		public List<string> Titulos { get => titulos; set => titulos = value; }
		public List<TipoObjetoDB> Tipos { get => tipos; set => tipos = value; }

		public override string ToString()
		{
			StringBuilder resultado = new StringBuilder();
			resultado.Append("<table class=\"table table - striped\">\n");
			//CABECERA
			if (this.titulos != null)
			{
				resultado.Append("<thead class=\"thead - dark\">\n");
				resultado.Append("\t<tr>\n");
				foreach (string titulo in titulos)
				{
					resultado.Append("\t\t<th scope=\"col\">");
					resultado.Append(titulo);
					resultado.Append("</th>\n");
				}
				resultado.Append("</tr>\n");
				resultado.Append("</thead>\n");
				//CONTENIDO
				resultado.Append("<tbody>\n");
				foreach (FilaDatos fila in this)
				{
					resultado.Append("\t<tr>\n");
					foreach (ParDatos par in fila.Datos)
					{
						resultado.Append("\t\t<td>");
						if (par.Valor.GetType() == typeof(CollectionListCql))
						{
							resultado.Append(((CollectionListCql)par.Valor).GetLinealizado());
							resultado.Append("<br/>");
						}
						else if (par.Valor.GetType() == typeof(CollectionMapCql))
						{
							resultado.Append(((CollectionMapCql)par.Valor).GetLinealizado());
							resultado.Append("<br/>");
						}
						else if (par.Valor.GetType() == typeof(Objeto))
						{
							resultado.Append(((Objeto)par.Valor).GetLinealizado());
							resultado.Append("<br/>");
						}
						else
						{
							resultado.Append(par.Valor);
							resultado.Append("<br/>");
						}
						resultado.Append("</td>\n");
					}
					resultado.Append("\t</tr>\n");
				}
				resultado.Append("</tbody>\n");
			}
			resultado.Append("</table>");
			return resultado.ToString();
		}

		internal ResultadoConsulta[] Dividir(int columna,TipoObjetoDB tipo)
		{
			List<ResultadoConsulta> res = new List<ResultadoConsulta>();
			ResultadoConsulta auxiliar = this;
			object pivote = null;
			int index = 0;
			while (index<auxiliar.Count) {
				pivote = this.ElementAt(index).Datos.ElementAt(columna).Valor;
				ResultadoConsulta bloque = new ResultadoConsulta(this.titulos,this.tipos);
				foreach (FilaDatos fila in this)
				{
					switch (tipo.Tipo)
					{
						case TipoDatoDB.INT:
						case TipoDatoDB.DOUBLE:
						case TipoDatoDB.BOOLEAN:
						case TipoDatoDB.STRING:
							{
								object val = fila.Datos.ElementAt(columna).Valor;
								if (pivote.Equals(val))
								{
									bloque.Add(fila);
								}
								break;
							}
						case TipoDatoDB.TIME:
						case TipoDatoDB.DATE:
							{
								MyDateTime val = (MyDateTime)fila.Datos.ElementAt(columna).Valor;
								if (((MyDateTime)pivote).CompareTo(val) == 0)
								{
									bloque.Add(fila);
								}
								break;
							}
					}
				}
				res.Add(bloque);
				index = index+bloque.Count;
			}
			if (res.Count==this.Count) {
				ResultadoConsulta[] res2 = { this };
				return res2;
			}
			return res.ToArray();
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
