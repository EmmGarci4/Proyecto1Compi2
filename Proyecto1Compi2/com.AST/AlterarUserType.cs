﻿using com.Analisis.Util;
using Proyecto1Compi2.com.db;
using Proyecto1Compi2.com.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1Compi2.com.AST
{
	class AlterarUserType:Sentencia
	{
		TipoAccion accion;
		string objeto;
		Dictionary<string, TipoObjetoDB> agregarattrib;
		List<Acceso> quitarattrib;

		public AlterarUserType(TipoAccion accion, string objeto, Dictionary<string, TipoObjetoDB> agregarattrib,  int linea, int columna) : base(linea, columna)
		{
			this.Accion = accion;
			this.Objeto = objeto;
			this.Agregarattrib = agregarattrib;
			this.Quitarattrib = null;
		}

		public AlterarUserType(TipoAccion accion, string objeto,  List<Acceso> quitarattrib, int linea, int columna) : base(linea, columna)
		{
			this.Accion = accion;
			this.Objeto = objeto;
			this.Agregarattrib = null;
			this.Quitarattrib = quitarattrib;
		}

		public TipoAccion Accion { get => accion; set => accion = value; }
		public string Objeto { get => objeto; set => objeto = value; }
		public List<Acceso> Quitarattrib { get => quitarattrib; set => quitarattrib = value; }
		internal Dictionary<string, TipoObjetoDB> Agregarattrib { get => agregarattrib; set => agregarattrib = value; }

		public override object Ejecutar(Sesion sesion)
		{
			Console.WriteLine("Alterando objeto..." + this.objeto + "->" + this.accion.ToString().ToLower());

			return null;
		}
	}
}
