using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Post.Models
{
    public class Post_Banda
    {
        [Key]
        public int IDdePost { get; set; }

        [Required]
        public int IDdeCuenta { get; set; }

        [Required]
        public int IDdeBanda { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }  // Relación con el modelo Post

        public Cuenta Cuenta { get; set; }

        public Banda Banda { get; set; }
    }

}
