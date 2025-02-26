using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Dominio.Entidades
{
    public class Veiculo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get;set;} = default!;

        [Required]
        [StringLength(150)]
        public string Nome {get;set;} = default!;

        [StringLength(100)]
        public string Marca {get;set;} = default!;
        
        public int Ano {get;set;} = default!;
    }
}