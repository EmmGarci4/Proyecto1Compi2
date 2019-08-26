using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class CrearCursor:Sentencia
	{
		string nombre;
		Seleccionar select;

		public CrearCursor(string nombre, Seleccionar select,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
			this.select = select;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Seleccionar Select { get => select; set => select = value; }

		public override object Ejecutar(TablaSimbolos ts)
		{
			if (!ts.ExisteSimbolo(nombre))
			{
				ts.AgregarSimbolo(new Simbolo(nombre,new Cursor(nombre,select),new TipoObjetoDB(TipoDatoDB.CURSOR,"cursor"), Linea, Columna));
			}
			else {
				return new ThrowError(TipoThrow.Exception,
						"La variable '" + nombre + "' ya existe",
						Linea, Columna);
			}
			return null;
		}
	}
}
