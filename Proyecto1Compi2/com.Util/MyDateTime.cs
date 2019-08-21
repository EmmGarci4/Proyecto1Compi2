using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.Util
{
	class MyDateTime : IComparable
	{
		TipoDatoDB tipo;
		DateTime dato;

		public MyDateTime(TipoDatoDB tipo, DateTime dato)
		{
			this.tipo = tipo;
			this.dato = dato;
		}

		public override string ToString()
		{
			if (tipo == TipoDatoDB.TIME)
			{
				return dato.ToString("HH:mm:ss");
			}
			else {
				return dato.ToString("yyyy-MM-dd");
			}
		}

		public int CompareTo(object dato)
		{
			MyDateTime other;
			if (dato.GetType() == typeof(MyDateTime))
			{
				other = (MyDateTime)dato;
			}
			else {
				return 0;
			}

			if (this.tipo == TipoDatoDB.TIME)
			{
				if (this.dato.Hour > other.Dato.Hour)
				{
					return 1;
				}
				else if (this.dato.Hour < other.dato.Hour)
				{
					return -1;
				}
				else {
					if (this.dato.Minute > other.Dato.Minute)
					{
						return 1;
					}
					else if (this.dato.Minute < other.dato.Minute)
					{
						return -1;
					}
					else
					{
						if (this.dato.Second > other.Dato.Second)
						{
							return 1;
						}
						else if (this.dato.Second < other.dato.Second)
						{
							return -1;
						}
						else
						{
							return 0;
						}
					}
				}
			}
			else
			{
				if (this.dato.Year > other.Dato.Year)
				{
					return 1;
				}
				else if (this.dato.Year < other.dato.Year)
				{
					return -1;
				}
				else
				{
					if (this.dato.Month > other.Dato.Month)
					{
						return 1;
					}
					else if (this.dato.Month < other.dato.Month)
					{
						return -1;
					}
					else
					{
						if (this.dato.Day > other.Dato.Day)
						{
							return 1;
						}
						else if (this.dato.Day < other.dato.Day)
						{
							return -1;
						}
						else
						{
							return 0;
						}
					}
				}
			}
		}

		public TipoDatoDB Tipo { get => tipo; set => tipo = value; }
		public DateTime Dato { get => dato; set => dato = value; }
	}
}
