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

		public ResultadoConsulta():base()
		{
			titulos = null;
		}

		public ResultadoConsulta(List<string> titulos) : base()
		{
			this.titulos = titulos;
		}

		public List<string> Titulos { get => titulos; set => titulos = value; }

		public override string ToString()
		{
			StringBuilder resultado = new StringBuilder();
			resultado.Append("<table>\n");
			//CABECERA
			resultado.Append("\t<tr>\n");
			foreach (string titulo in titulos) {
				resultado.Append("\t\t<th>");
				resultado.Append(titulo);
				resultado.Append("</th>\n");
			}
			resultado.Append("</tr>\n");
			//CONTENIDO
			foreach (FilaDatos fila in this) {
				resultado.Append("\t<tr>\n");
				foreach (ParDatos par in fila.Datos) {
					resultado.Append("\t\t<td>");
					if (par.Valor.GetType() == typeof(CollectionListCql))
					{
						resultado.Append(((CollectionListCql)par.Valor).GetLinealizado());
					}else if (par.Valor.GetType() == typeof(CollectionMapCql))
					{
						resultado.Append(((CollectionMapCql)par.Valor).GetLinealizado());
					}else if (par.Valor.GetType() == typeof(Objeto))
					{
						resultado.Append(((Objeto)par.Valor).GetLinealizado());
					}
					else {
						resultado.Append(par.Valor);
					}
					resultado.Append("</td>\n");
				}
				resultado.Append("\t</tr>\n");
			}
			
			resultado.Append("</table>");
			return resultado.ToString();
		}
	}
}
