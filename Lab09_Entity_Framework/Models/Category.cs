using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab09_Entity_Framework.Models
{
    // Ánh xạ tới bảng 'Category' trong CSDL
    [Table("Category")]
    public class Category
    {
        [Key] // Khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int ID { get; set; }

        [Required] // Không được null
        [StringLength(1000)]
        public string Name { get; set; }

        public int Type { get; set; }

        // Khóa ngoại: Một Category có nhiều Food
        public virtual ICollection<Food> Foods { get; set; }
    }
}