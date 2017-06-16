using System;
using System.Collections;
using System.Linq;
using System.Text;


namespace ControlVentas.modelo.dao
{
    /*Data access object*/
    abstract class AbstractDao
    {
        abstract public int crear(Object o);
        abstract public int editar(Object o);
        abstract public int cambiarEstado(string id, bool vigente);
        abstract public int eliminar(string id);
        abstract public ArrayList ver(int[] arreglo, string buscar);
        abstract public object buscar(string id);
    }
}
