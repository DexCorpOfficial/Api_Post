using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Post.Models
{
    public class Post_Feed
    {
        [Key]
        public int IDdePost { get; set; }

        [Required]
        public int IDdeCuenta { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }  // Relación con el modelo Post

        public Cuenta Cuenta { get; set; }
    }

}
