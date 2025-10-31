using System.Data.Entity;

namespace Lab09_Entity_Framework.Models
{
    // Lớp ngữ cảnh (DbContext), kế thừa từ DbContext của EF
    public class RestaurantContext : DbContext
    {
        // Tên chuỗi kết nối "RestaurantContext" trong App.config
        public RestaurantContext() : base("name=RestaurantContext")
        {
        }

        // Khai báo các bảng mà EF sẽ quản lý
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
    }
}