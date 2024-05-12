using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.Domain.Entities.Product
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [StringLength(100)]
        public string Username { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }
        [Required]  
        [Range(1, 5)]  
        public int Rating { get; set; }
    }

}

