using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab09_Entity_Framework.Models
{
    // Ánh xạ tới bảng 'Food'
    [Table("Food")]
    public class Food
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Unit { get; set; }

        public int FoodCategoryID { get; set; }

        public int Price { get; set; }

        [StringLength(3000)]
        public string Notes { get; set; }

        // Navigation property: Mối quan hệ khóa ngoại
        [ForeignKey("FoodCategoryID")]
        public virtual Category Category { get; set; }
    }
}