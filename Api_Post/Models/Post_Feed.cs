using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Post_Feed : Post
    {
        [Required]
        public int IDdeCuenta { get; set; }
        public Cuenta Cuenta { get; set; }

        // Relación con Post (IDdePost) no es necesario definirlo como propiedad 'Post'
        public int IDdePost { get; set; } // Esta es la clave foránea a la tabla Post
        public Post Post { get; set; } // Relación explícita con Post
    }
}
