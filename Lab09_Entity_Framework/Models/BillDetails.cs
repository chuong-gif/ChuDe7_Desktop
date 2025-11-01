using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    // Đổi tên class thành BillDetails (CSDL của bạn là BilLDetails)
    [Table("BilLDetails")]
    public class BillDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int InvoiceID { get; set; }
        public int FoodID { get; set; }
        public int Quantity { get; set; }

        // Khai báo khóa ngoại
        [ForeignKey("InvoiceID")]
        public virtual Bills Bill { get; set; }

        [ForeignKey("FoodID")]
        public virtual Food Food { get; set; }
    }
}