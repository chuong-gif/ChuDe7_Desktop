namespace Lab09_Entity_Framework.Models
{
    // Lớp DTO (Data Transfer Object) 
    // Dùng để lấy thông tin từ nhiều bảng (Food và Category)
    public class FoodModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
        public string Notes { get; set; }
    }
}