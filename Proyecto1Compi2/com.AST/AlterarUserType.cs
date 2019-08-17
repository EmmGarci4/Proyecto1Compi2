using com.Analisis;
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
	class AlterarUserType : Sentencia
	{
		TipoAccion accion;
		string nombre;
		Dictionary<string, TipoObjetoDB> agregarattrib;
		List<Acceso> quitarattrib;

		public AlterarUserType(TipoAccion accion, string nombre, Dictionary<string, TipoObjetoDB> agregarattrib, int linea, int columna) : base(linea, columna)
		{
			this.Accion = accion;
			this.Nombre = nombre;
			this.Agregarattrib = agregarattrib;
			this.Quitarattrib = null;
		}

		public AlterarUserType(TipoAccion accion, string nombre, List<Acceso> quitarattrib, int linea, int columna) : base(linea, columna)
		{
			this.Accion = accion;
			this.Nombre = nombre;
			this.Agregarattrib = null;
			this.Quitarattrib = quitarattrib;
		}

		public TipoAccion Accion { get => accion; set => accion = value; }
		public string Nombre { get => nombre; set => nombre = value; }
		public List<Acceso> Quitarattrib { get => quitarattrib; set => quitarattrib = value; }
		internal Dictionary<string, TipoObjetoDB> Agregarattrib { get => agregarattrib; set => agregarattrib = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos tb)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				//VALIDANDO USER TYPE
				if (db.ExisteUserType(nombre))
				{
					UserType ut = db.BuscarUserType(nombre);
					if (accion == TipoAccion.Agregar)
					{
						//VALIDANDO
						foreach (KeyValuePair<string, TipoObjetoDB> valores in agregarattrib)
						{
							if (ut.Atributos.Contains(valores))
							{
								return new ThrowError(TipoThrow.Exception,
									"El atributo '"+valores.Key+"' ya existe en el User Type",
									Linea, Columna);
							}
						}
						//AGREGANDO
						foreach (KeyValuePair<string, TipoObjetoDB> valores in agregarattrib)
						{
							ut.Atributos.Add(valores.Key,valores.Value);
						}
					}
					else
					{
						//eliminar
						//foreach (Acceso att in quitarattrib) {
						//	if (att.getTipo() == TipoAcceso.Campo)
						//	{
						//		ut.Atributos.Remove(att.getValor().ToString());
						//	}
						//	else{
						//		//eliminar campo en objeto adentro

						//	}
						//}

					}
				}
				else
				{
					return new ThrowError(TipoThrow.TypeDontExists,
				"El user Type '" + nombre + "' no existe",
				Linea, Columna);
				}
			}
			else
			{
				return new ThrowError(TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					Linea, Columna);
			}
			return null;
		}
	}
}
