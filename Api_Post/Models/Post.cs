using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Post.Models
{
    public class Post
    {
        [Key]
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

        [Required]
        public string discriminator { get; set; }

        // Relaciones con las tablas derivadas
        public ICollection<Post_Feed> Post_Feeds { get; set; }
        public ICollection<Post_Evento> PostEvento { get; set; }
        public ICollection<Post_Banda> PostBandas { get; set; }
    }


}
