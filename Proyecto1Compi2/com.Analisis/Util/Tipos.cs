using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Analisis.Util
{
	public enum TipoError
	{
		Lexico,
		Sintactico,
		Semantico,
		Advertencia
	}
	public enum TipoDato
	{
		TEXT,
		INTEGER,
		DOUBLE,
		BOOL,
		DATE,
		DATETIME
	}
	public enum TipoSentencia
	{
		Expresion,
		Retorno,
		Break,
		Continue
	}

	public enum Visibilidad {
		Private,
		Public,
		Protected,
		Error
	}
}
