using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos
{
    public class TutoresAnimaisDto
    {
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }
        public int AnimalId { get; set; }
    }
}