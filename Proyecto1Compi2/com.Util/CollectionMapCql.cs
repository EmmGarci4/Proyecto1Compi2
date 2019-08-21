using Proyecto1Compi2.com.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class CollectionMapCql : Dictionary<object, object>
	{
		TipoObjetoDB tipoLlave;
		TipoObjetoDB tipoValor;

		public CollectionMapCql(TipoObjetoDB tipoLlave, TipoObjetoDB tipoValor):base()
		{
			this.tipoLlave = tipoLlave;
			this.tipoValor = tipoValor;
		}

		public TipoObjetoDB TipoLlave { get => tipoLlave; set => tipoLlave = value; }
		public TipoObjetoDB TipoValor { get => tipoValor; set => tipoValor = value; }

		internal object AddItem(object clave, object valorr, int linea, int columna)
		{
			try {
				this.Add(clave, valorr);
				//CONSULTA PARA ORDENAR
				return null;
			} catch (ArgumentException) {
				return new ThrowError(Util.TipoThrow.Exception,
										"La clave '" + clave + "' ya existe",
										linea, columna);
			}
			
			//ordenar
		}


		internal object GetItem(object nuevo,int linea,int columna)
		{
			try
			{
				return this[nuevo];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		internal object SetItem(object clave, object valorr, int linea, int columna)
		{
			foreach (KeyValuePair<object,object> valores in this) {
				if (valores.Key.Equals(clave)) {
					this[valores.Key] = valorr;
					return null;
				}
			}
			return new ThrowError(TipoThrow.Exception,
				"La clave '"+valorr+"' no existe", linea, columna);
		}

		internal object EliminarItem(object nuevo, int linea, int columna)
		{
			if (this.ContainsKey(nuevo))
			{
				this.Remove(nuevo);
			}
			else {
				return new ThrowError(TipoThrow.Exception,
								"La clave '" + nuevo.ToString() + "' no existe", linea, columna);
			}
			return null;
		}
	}
}
