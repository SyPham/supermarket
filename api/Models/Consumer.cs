using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    [Table("Consumers")]
    public class Consumer
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; }
        [MaxLength(50)]
        public string EmployeeId { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
    }
}
