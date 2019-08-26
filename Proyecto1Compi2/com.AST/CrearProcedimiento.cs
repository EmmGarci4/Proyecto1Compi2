using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearProcedimiento : Sentencia
	{
		string nombre;
		List<Parametro> parametros;
		List<Parametro> retornos;
		List<Sentencia> sentencias;
		List<object> valoresParametros;
		string codigo;

		public CrearProcedimiento(string nombre, List<Parametro> parametros, List<Parametro> retornos, List<Sentencia> sentencias,String codigo, int linea, int columna) : base(linea, columna)
		{
			this.nombre = nombre;
			this.parametros = parametros;
			this.retornos = retornos;
			this.sentencias = sentencias;
			this.codigo = codigo;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<Parametro> Parametros { get => parametros; set => parametros = value; }
		public List<Parametro> Retornos { get => retornos; set => retornos = value; }
		public string Codigo { get => codigo; set => codigo = value; }
		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos tb)
		{
			//VALIDANDO BASEDATOS
			if (Analizador.Sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(Analizador.Sesion.DBActual);
				string llave = GetLlave();
				if (!db.ExisteProcedimiento(llave))
				{
					
					db.AgregarProcedimiento(new Procedimiento(nombre,parametros,retornos,sentencias,codigo,Linea,Columna));
				}
				else
				{
						return new ThrowError(Util.TipoThrow.TypeAlreadyExists,
					"El procedimiento '" + llave + "' ya existe",
					Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}
			return null;
		}

		public string GetLlave()
		{
			StringBuilder llave = new StringBuilder();
			llave.Append(Nombre + "(");
			int contador = 0;
			foreach (Parametro par in this.parametros)
			{
				llave.Append(par.Tipo.ToString());
				if (contador < parametros.Count - 1)
				{
					llave.Append(",");
				}
				contador++;
			}
			llave.Append(")");
			return llave.ToString();
		}
	}
}
