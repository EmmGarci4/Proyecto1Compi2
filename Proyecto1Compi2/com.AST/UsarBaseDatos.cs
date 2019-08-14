using com.Analisis;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class UsarBaseDatos:Sentencia
	{
		string nombre;

		public UsarBaseDatos(string nombre, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
		}

		public string Nombre { get => nombre; set => nombre = value; }

		public override object Ejecutar()
		{
			if (Analizador.ExisteDB(nombre))
			{
				//indicar al usuario que db esta en uso
				Analizador.DBActual = Analizador.BuscarDB(nombre);
			}
			else {
				return new ThrowError(TipoThrow.BDDontExists,"La base de datos '"+Nombre+"' no existe",Linea,Columna);
			}
			return null;
		}
	}
}
