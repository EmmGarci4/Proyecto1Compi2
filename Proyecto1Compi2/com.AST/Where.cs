using System.Collections.Generic;
using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;

namespace Proyecto1Compi2.com.AST
{
	internal class Where:Expresion
	{
		Expresion condicion;
		Expresion izquierda; //para clausula in
		Expresion expresiones;
		List<Expresion> listaExpresiones;

		//WHERE CON CONDICION
		public Where(Expresion condicion,int linea,int columna):base(linea,columna)
		{
			this.condicion = condicion;
			this.izquierda = null;
			this.expresiones = null;
			this.listaExpresiones = null;
		}


		//WHERE CON EXPRESION IZQUIERDA Y DERECHA
		public Where(Expresion izquierda, Expresion expresiones, int linea, int columna) : base(linea, columna)
		{
			this.condicion = null;
			this.izquierda = izquierda;
			this.expresiones = expresiones;
			this.listaExpresiones = null;
		}

		//WHERE CON LISTA DE EXPRESIONES
		public Where(Expresion izquierda, List<Expresion> expresiones, int linea, int columna) : base(linea, columna)
		{
			this.condicion = null;
			this.izquierda = izquierda;
			this.expresiones = null;
			this.listaExpresiones = expresiones;
		}

		internal Expresion Condicion { get => condicion; set => condicion = value; }
		internal Expresion Izquierda { get => izquierda; set => izquierda = value; }
		internal Expresion Expresiones { get => expresiones; set => expresiones = value; }
		internal List<Expresion> ListaExpresiones { get => listaExpresiones; set => listaExpresiones = value; }

		public override TipoOperacion GetTipo(TablaSimbolos ts,Sesion sesion)
		{
			return TipoOperacion.Boolean;
		}

		public override object GetValor(TablaSimbolos ts,Sesion sesion)
		{
			//WHERE CON CONDICION
			if (this.izquierda == null && this.expresiones == null && this.listaExpresiones == null)
			{
				object val= this.condicion.GetValor(ts, sesion);
				if (val!=null) {
					if (val.GetType()==typeof(ThrowError)) {
						return val;
					}
					if (val.GetType()!=typeof(bool)) {
						return new ThrowError(TipoThrow.Exception, 
							"La propiedad where debe evaluar un valor booleano", 
							Linea, Columna);
					}
					return val;
				}
			}
			else
			//WHERE CON IN Y EXPRESION
			if (this.condicion == null && this.listaExpresiones == null)
			{
				//LISTA
				object supuestaLista = this.expresiones.GetValor(ts, sesion);
				if (supuestaLista != null)
				{
					if (supuestaLista.GetType() == typeof(ThrowError))
					{
						return supuestaLista;
					}
				}
				TipoObjetoDB tipoLista = Datos.GetTipoObjetoDB(supuestaLista);
				//VALOR
				object valorAEvaluar = this.izquierda.GetValor(ts, sesion);
				if (valorAEvaluar != null)
				{
					if (valorAEvaluar.GetType() == typeof(ThrowError))
					{
						return valorAEvaluar;
					}
				}
				TipoObjetoDB tipovalorAEvalizar = Datos.GetTipoObjetoDB(valorAEvaluar);
				if (Datos.IsLista(tipoLista.Tipo))
				{
					if (tipoLista.Tipo != TipoDatoDB.MAP_OBJETO || tipoLista.Tipo != TipoDatoDB.MAP_PRIMITIVO)
					{
						//es un set o una lista 
						if (Datos.IsPrimitivo(tipovalorAEvalizar.Tipo))
						{
							CollectionListCql lista = (CollectionListCql)supuestaLista;
							if (Datos.IsPrimitivo(tipovalorAEvalizar.Tipo))
							{
								if (Datos.IsTipoCompatible(Datos.GetTipoObjetoDBPorCadena(lista.TipoDato.Nombre), valorAEvaluar))
								{
									return lista.Contains(valorAEvaluar);
								}
								else
								{
									return new ThrowError(TipoThrow.Exception,
										"El valor a evaluar no concuerda con el tipo '" + lista.TipoDato.ToString() + "' de la lista",
										Linea, Columna);
								}
							}
							else
							{
								return new ThrowError(TipoThrow.Exception,
									"Solo se puede buscar en listas de valores primitivos",
									Linea, Columna);
							}
						}
						else
						{
							return new ThrowError(TipoThrow.Exception,
						"Solo se pueden buscar valores primitivos",
						Linea, Columna);
						}
					}
					else
					{
						return new ThrowError(TipoThrow.Exception,
									"Solo se puede buscar en una lista o un set",
									Linea, Columna);
					}
				}
				else
				{
					return new ThrowError(TipoThrow.Exception,
						"El valor a evaluar en IN no es una lista",
						Linea, Columna);
				}
			}
			else
			//WHERE CON IN EN LISTA DE DATOS
			if (this.condicion==null && this.expresiones==null)		
			{
				//VALOR
				object valorAEvaluar = this.izquierda.GetValor(ts, sesion);
				if (valorAEvaluar != null)
				{
					if (valorAEvaluar.GetType() == typeof(ThrowError))
					{
						return valorAEvaluar;
					}
				}
				if (!Datos.IsPrimitivo(Datos.GetTipoObjetoDB(valorAEvaluar).Tipo)) {
					return new ThrowError(TipoThrow.Exception,
						"Solo se puede buscar datos primitivos",
						Linea, Columna);
				}
				//LISTA
				List<object> list = new List<object>();
				foreach (Expresion exp in this.listaExpresiones) {
					object val = exp.GetValor(ts, sesion);
					if (val!=null) {
						if (val.GetType()==typeof(ThrowError)) {
							return val;
						}
						list.Add(val);
					}
				}

				foreach (object vals in list) {
					if (vals.Equals(valorAEvaluar)) {
						return true;
					}
				}
				return false;
			}

			return null;
		}
	}
}