using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class CrearCursor:Sentencia
	{
		string nombre;
		Seleccionar select;
		Expresion exp;

		public CrearCursor(string nombre, Seleccionar select,int linea,int columna):base(linea,columna)
		{
			this.nombre = nombre;
			this.select = select;
			this.exp = null;
		}

		public CrearCursor(string nombre, Expresion exp, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.select = null;
			this.exp = exp;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		internal Seleccionar Select { get => select; set => select = value; }

		public override object Ejecutar(TablaSimbolos ts,Sesion sesion)
		{
			if (!ts.ExisteSimboloEnAmbito(nombre))
			{
				if (select != null)
				{
					ts.AgregarSimbolo(new Simbolo(nombre, new Cursor(nombre, select), new TipoObjetoDB(TipoDatoDB.CURSOR, "cursor"), Linea, Columna));
				}
				else {
					object valor = exp.GetValor(ts, sesion);
					if (valor!=null) {
						if (valor.GetType()==typeof(ThrowError)) {
							return valor;
						}

						if (valor.GetType() == typeof(Cursor))
						{
							ts.AgregarSimbolo(new Simbolo(nombre, valor, new TipoObjetoDB(TipoDatoDB.CURSOR, "cursor"), Linea, Columna));
						}
						else {
							return new ThrowError(TipoThrow.Exception,
						"No se puede asignar a una variable tipo cursor otro tipo de valor",
						Linea, Columna);
						}

					}
				}

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
