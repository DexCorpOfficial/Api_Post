using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Post
    {
        public int ID { get; set; }
        public byte[] Media { get; set; }

        [StringLength(328)]
        public string Descripcion { get; set; }

        [Required]
        public DateTime fecha_pub { get; set; } = DateTime.Now;

        [Required]
        public int NUpvotes { get; set; } = 0;

        [Required]
        public bool Activo { get; set; } = true;
    }

}
