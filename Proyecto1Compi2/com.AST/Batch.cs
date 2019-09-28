using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class Batch : Sentencia
	{
		List<Sentencia> sentencias;

		public Batch(int linea, int columna) : base(linea, columna)
		{
			this.sentencias = new List<Sentencia>();
		}

		internal List<Sentencia> Sentencias { get => sentencias; set => sentencias = value; }

		public override object Ejecutar(TablaSimbolos tb, Sesion sesion)
		{
			if (sesion.DBActual != null)
			{
				foreach (Sentencia sent in sentencias)
				{
					if (sent.GetType() == typeof(Seleccionar))
					{
						return new ThrowError(Util.TipoThrow.BatchException, "Hay sentencias que no pertenecen al bloque Batch",
										Linea, Columna);
					}
				}
				//GUARDANDO ESTADO
				List<Error> erroresCql = new List<Error>();
				erroresCql.AddRange(Analizador.ErroresCQL);
				Analizador.GenerarArchivos("auxbatch.chison");
				foreach (Sentencia sent in sentencias)
				{
					object respuesta = sent.Ejecutar(tb, sesion);
					if (respuesta != null)
					{
						if (respuesta.GetType() == typeof(ThrowError))
						{
							leerAuxiliar();
							Analizador.ErroresCQL.Add(new Error((ThrowError)respuesta));
							return new ThrowError(Util.TipoThrow.BatchException, "Hay sentencias con errores en el bloque Batch",
										Linea, Columna);
						}
					}
				}
			}
			else
			{
				Analizador.ErroresCQL.Add(new Error(TipoError.Semantico,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna));
				return new ThrowError(Util.TipoThrow.BatchException, "Hay sentencias con errores en el bloque Batch",
										Linea, Columna);
			}
			return null;
		}

		private void leerAuxiliar()
		{
			Analizador.BasesDatos.Clear();
			//Usuariosdb = GetListaUsuarios();
			Analizador.Usuarios.Clear();
			//funciones.Clear();
			//erroresCQL.Clear();
			//GeneradorDB.ErroresChison.Clear();
			//codigoAnalizado = "";
			//errorCatch = GetErrorCatch();
			//errors = GetTablaErrors();

			String chi = HandlerFiles.AbrirArchivo("auxbatch.chison");
			if (chi != null)
			{
				if (GeneradorDB.AnalizarChison(chi))
				{
					Console.WriteLine("ARCHIVO CARGADO CON EXITO");
				}
				else
				{
					Console.WriteLine("ARCHIVO CARGADO CON ERRORES");
					//Analizador.Errors.MostrarCabecera();
					//Analizador.Errors.MostrarDatos();
				}
			}
		}
	}
}
