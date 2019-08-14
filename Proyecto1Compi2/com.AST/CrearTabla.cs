using com.Analisis;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class CrearTabla : Sentencia
	{
		String nombre;
		List<object> objetos;
		bool ifExist;

		public CrearTabla(String tabla,List<object> objetos,bool ifexist, int linea, int columna) : base(linea, columna)
		{
			this.nombre = tabla;
			this.objetos = objetos;
			this.ifExist = ifexist;
		}

		public string Nombre { get => nombre; set => nombre = value; }
		public List<object> Objetos { get => objetos; set => objetos = value; }
		public bool IfExist { get => ifExist; set => ifExist = value; }

		public override object Ejecutar(Sesion sesion)
		{
			List<string> llavePrimariaCompuesta = null;
			Tabla tabla = new Tabla(Nombre);
			foreach (object ob in this.objetos) {
				if (ob.GetType() == typeof(Columna))
				{
					Columna cl = (Columna)ob;
					//***************************************************************************
					//VALIDANDO COLUMNA
					if (tabla.ExisteColumna(cl.Nombre))
					{


					}
					else {

					}

					//***************************************************************************
				}
				else {
					if (llavePrimariaCompuesta != null)
					{
						//es llave primaria
						llavePrimariaCompuesta = (List<string>)ob;
					}
					else {
						return new ThrowError(Util.TipoThrow.Exception,"No se puede agregar más de una llave primaria compuesta a una tabla",Linea,Columna);
					}
				}
			}

			//***************************************************************************
			//VALIDANDO TABLA
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (!db.ExisteTabla(Nombre))
				{

				}
				else
				{
					if (!IfExist)
					{
						return new ThrowError(Util.TipoThrow.TableAlreadyExists, "La tabla '" + Nombre + "' ya existe", Linea, Columna);
					}
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException, "No se puede ejecutar la sentencia para crear tabla", Linea, Columna);
			}

			//***************************************************************************

			return null;
		}
	}
}
