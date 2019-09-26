using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class Objeto
	{
		Dictionary<string, object> atributos;
		UserType plantilla;
		bool isNull;

		public Objeto(Dictionary<string, object> atributos,UserType plantilla)
		{
			this.atributos = atributos;
			this.plantilla = plantilla;
			this.isNull = false;
		}

		public Objeto( UserType plantilla)
		{
			this.atributos = new Dictionary<string, object>();
			this.plantilla = plantilla;
			this.isNull = false;
		}

		public Objeto(UserType plantilla,bool isnull)
		{
			this.atributos = null;
			this.plantilla = plantilla;
			this.isNull = isnull;
		}

		internal Dictionary<string, object> Atributos { get => atributos; set => atributos = value; }
		internal UserType Plantilla { get => plantilla; set => plantilla = value; }
		public bool IsNull { get => isNull; set => isNull = value; }

		public override string ToString()
		{
			StringBuilder cadena = new StringBuilder();
			
			int i = 0;
			if (!IsNull) {
				if (atributos != null)
				{
					cadena.Append("<");
					foreach (KeyValuePair<string, object> val in atributos)
					{
						cadena.Append("\"" + val.Key + "\"=");
						TipoObjetoDB tipo = this.plantilla.Atributos[val.Key];
						if (tipo != null)
						{
							if (tipo.Tipo.Equals(TipoDatoDB.STRING))
							{
								cadena.Append("\"" + val.Value + "\"");
							}
							else if (tipo.Tipo.Equals(TipoDatoDB.DATE) || tipo.Tipo.Equals(TipoDatoDB.TIME))
							{
								if (val.Value.Equals("null"))
								{
									cadena.Append(val.Value);
								}
								else
								{
									cadena.Append("\'" + val.Value + "\'");
								}
							}
							else
							{
								cadena.Append(val.Value.ToString());
							}
						}

						if (i < atributos.Count - 1)
						{
							cadena.Append(",");
						}
						i++;
					}
					cadena.Append(">");
				}
			}
			else {
				cadena.Append("null");
			}

			
			return cadena.ToString();
		}

		internal bool IsObjetoTipo(string nombre)
		{
			return plantilla.Nombre == nombre;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				// Choose large primes to avoid hashing collisions
				const int HashingBase = (int)2166136261;
				const int HashingMultiplier = 16777619;

				int hash = HashingBase;
				hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, atributos) ? atributos.GetHashCode() : 0);
				hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, plantilla) ? plantilla.GetHashCode() : 0);
				return hash;
			}
		}

		internal string GetLinealizado()
		{
			StringBuilder cadena = new StringBuilder();
			if (!IsNull) {
				if (atributos != null)
				{
					cadena.Append("{\n");
					int i = 0;
					foreach (KeyValuePair<string, object> val in atributos)
					{
						cadena.Append("\"" + val.Key + "\":");
						TipoObjetoDB tipo = this.plantilla.Atributos[val.Key];
						if (tipo != null)
						{
							if (tipo.Tipo.Equals(TipoDatoDB.STRING))
							{
								cadena.Append("\"" + val.Value + "\"");
							}
							else if (tipo.Tipo.Equals(TipoDatoDB.DATE) || tipo.Tipo.Equals(TipoDatoDB.TIME))
							{
								if (val.Value.Equals("null"))
								{
									cadena.Append(val.Value);
								}
								else
								{
									cadena.Append("\'" + val.Value + "\'");
								}
							}
							else if (tipo.Tipo == TipoDatoDB.OBJETO)
							{
								cadena.Append(((Objeto)val.Value).GetLinealizado());
							}
							else
							{
								cadena.Append(val.Value.ToString());
							}
						}
						if (i < atributos.Count - 1)
						{
							cadena.Append(",\n");
						}
						i++;
					}

					cadena.Append("\n}");
				}
			}
			else {
				cadena.Append("null");
			}
			return cadena.ToString();
		}
	}
}
