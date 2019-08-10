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
	enum TipoDatoDB
	{
		STRING,
		INT,
		DOUBLE,
		BOOLEAN,
		DATE,
		TIME,
		COUNTER,
		NULO,
		OBJETO
	}
	public enum TipoAccion
	{
		Agregar,
		Quitar
	}
	public enum TipoObjeto
	{
		Tabla,
		BaseDatos,
		Objeto,
		Usuario
	}

}
