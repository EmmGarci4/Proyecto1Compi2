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
	public enum TipoAccion
	{
		Agregar,
		Quitar
	}
	public enum TipoAcceso
	{
		Variable,
		Campo,
		LlamadaFuncion,
		AccesoArreglo
	}
	public enum TipoObjeto
	{
		Tabla,
		Procedimiento,
		Objeto,
		Error
	}

}
