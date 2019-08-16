using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;

namespace Proyecto1Compi2.com.AST
{
	class AccesoArreglo:Expresion
	{
		Expresion valor;
		string nombre;

		public AccesoArreglo(Expresion valor, string nombre,int linea,int columna):base(linea,columna)
		{
			this.valor = valor;
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Expresion Valor { get => valor; set => valor = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}

		public override object GetValor(TablaSimbolos ts)
		{
			throw new NotImplementedException();
		}
	}
}
