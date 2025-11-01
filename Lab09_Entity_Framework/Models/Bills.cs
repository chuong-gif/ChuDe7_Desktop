using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    [Table("Bills")]
    public class Bills
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        public int TableID { get; set; }
        public int Amount { get; set; }
        public double? Discount { get; set; } // Cho phép NULL
        public double? Tax { get; set; } // Cho phép NULL
        public bool Status { get; set; }
        public DateTime? CheckoutDate { get; set; } // Cho phép NULL

        [StringLength(100)]
        public string Account { get; set; }

        // Khai báo mối quan hệ: Một hóa đơn có nhiều chi tiết
        public virtual ICollection<BillDetails> BillDetails { get; set; }
    }
}