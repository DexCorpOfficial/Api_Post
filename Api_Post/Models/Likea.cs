using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Likea
    {
        [Required]
        public int IDdePost { get; set; }

        [Required]
        public int IDdeCuenta { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }

        public Cuenta Cuenta { get; set; }
    }
}
