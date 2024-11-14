using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.DTOs
{
    public class Post_CreateDTO
    {
        public byte[] Media { get; set; }          // Media (común para todos los tipos de post)
        public string Descripcion { get; set; }    // Descripción (común para todos los tipos de post)
        public int IDdeCuenta { get; set; }        // ID de cuenta (común para todos los tipos de post)

        // Propiedades opcionales para cada tipo de publicación
        public int? IDdeBanda { get; set; }        // Para Post_Banda (opcional)
        public int? IDdeEvento { get; set; }       // Para Post_Evento (opcional)
    }
}
