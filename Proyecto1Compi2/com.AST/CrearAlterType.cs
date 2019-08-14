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
	class CrearUserType:Sentencia
	{
		string objeto;
		Dictionary<string, TipoObjetoDB> atributos;

		public CrearUserType(string objeto, Dictionary<string, TipoObjetoDB> atributos,  int linea, int columna) : base(linea, columna)
		{
			this.Objeto = objeto;
			this.atributos = atributos;
		}

		public string Objeto { get => objeto; set => objeto = value; }
		internal Dictionary<string, TipoObjetoDB> Atributos { get => atributos; set => atributos = value; }

		public override object Ejecutar(Usuario usuario)
		{
			Console.WriteLine("Creando objeto..." + this.objeto + "->");

			return null;
		}
	}
}
