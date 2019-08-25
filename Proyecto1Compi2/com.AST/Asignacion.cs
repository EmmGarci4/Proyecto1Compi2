using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Analisis;
using com.Analisis.Util;
using Proyecto1Compi2.com.Analisis;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	class Asignacion:Sentencia
	{
		Acceso izquierda;
		Expresion derecha;

		public Asignacion(Acceso izquierda, Expresion derecha,int linea,int columna):base(linea,columna)
		{
			this.izquierda = izquierda;
			this.derecha = derecha;
		}

		internal Acceso Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Derecha { get => derecha; set => derecha = value; }

		public override object Ejecutar(Sesion sesion, TablaSimbolos ts)
		{
			//OBTENIENDO RESPUESTA DE EXPRESION
			object respuesta = derecha.GetValor(ts);
			TipoOperacion tipoRespuesta = derecha.GetTipo(ts);
			if (respuesta != null)
			{
				if (respuesta.GetType() == typeof(ThrowError))
				{
					return respuesta;
				}
			}
			//si es una instancia
			//se genera aquí pues no tengo acceso a la sesion desde la operacion
			if (tipoRespuesta == TipoOperacion.NuevaInstancia)
			{
				TipoObjetoDB tipoInstancia = (TipoObjetoDB)respuesta;

				if (Datos.IsLista(tipoInstancia.Tipo))
				{
					object instanciaLista = GetInstanciaLista(tipoInstancia);
					if (instanciaLista!=null) {
						//izquierda.Asignar(instanciaLista, tipoInstancia, ts, sesion);
					}
				}
				else
				{
					if (tipoInstancia.Tipo == TipoDatoDB.OBJETO)
					{
						object instanciaObjeto = GetInstanciaObjeto(tipoInstancia, sesion,Linea,Columna);
						if (instanciaObjeto!=null) {
							//izquierda.Asignar(instanciaObjeto, tipoInstancia, ts, sesion);
						}
					}
					else
					{
						//ERROR NO SE PUEDE INSTANCIAR UN TIPO PRIMITIVO
						return new ThrowError(Util.TipoThrow.Exception,
						"No se puede instanciar un tipo primitivo",
						Linea, Columna);

					}
				}
			}
			else {
				////NO ES UNA INSTANCIA
				return izquierda.Asignar(respuesta, Datos.GetTipoObjetoDB(respuesta), ts, sesion);
			}

			return null;
		}

		public static object GetInstanciaObjeto(TipoObjetoDB tipoInstancia, Sesion sesion,int linea,int columna)
		{
			//VALIDANDO BASEDATOS
			if (sesion.DBActual != null)
			{
				BaseDatos db = Analizador.BuscarDB(sesion.DBActual);
				if (db.ExisteUserType(tipoInstancia.ToString()))
				{
					UserType ut = db.BuscarUserType(tipoInstancia.ToString());
					Objeto nuevaInstancia = new Objeto(ut);
					foreach (KeyValuePair<string,TipoObjetoDB> atributo in ut.Atributos) {
						nuevaInstancia.Atributos.Add(atributo.Key, Declaracion.GetValorPredeterminado(atributo.Value.Tipo));
					}
					return nuevaInstancia;
				}
			}
			else
			{
				return new ThrowError(Util.TipoThrow.UseBDException,
					"No se puede ejecutar la sentencia porque no hay una base de datos seleccionada",
					linea, columna);
			}
			return null;
		}

		public static object GetInstanciaLista(TipoObjetoDB tipoInstancia)
		{

			switch (tipoInstancia.Tipo) {
				case TipoDatoDB.LISTA_OBJETO:
					break;
				case TipoDatoDB.LISTA_PRIMITIVO:
					TipoObjetoDB tipoDatoLista = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					CollectionListCql nueva = new CollectionListCql(tipoDatoLista,true);
					return nueva;
				case TipoDatoDB.SET_OBJETO:
					break;
				case TipoDatoDB.SET_PRIMITIVO:
					tipoDatoLista = Datos.GetTipoObjetoDBPorCadena(tipoInstancia.Nombre);
					nueva = new CollectionListCql(tipoDatoLista, false);
					return nueva;
				case TipoDatoDB.MAP_OBJETO:
					break;
				case TipoDatoDB.MAP_PRIMITIVO:
					//UFFFF JODER
					break;
			}
			return null;
		}
	}
}
