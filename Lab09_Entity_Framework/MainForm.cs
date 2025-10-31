using Lab09_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace Lab09_Entity_Framework
{
     // Định nghĩa kiểu enum để quản lý các nút gốc trên TreeView [cite: 4645]
    public enum CategoryType
    {
        All = 0,
        Food = 1,
        Drink = 2
    }

    public partial class MainForm : Form
    {
         // Khởi tạo DbContext [cite: 4641]
        RestaurantContext context = new RestaurantContext();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowCategories(); // Tải danh mục lên TreeView khi form load [cite: 4639]
        }

        // Tải lại danh mục
        private void btnReloadCategory_Click(object sender, EventArgs e)
        {
            ShowCategories(); // [cite: 4640]
        }

        // Tải lại món ăn
        private void btnReloadFood_Click(object sender, EventArgs e)
        {
            // Lấy nút đang chọn trên TreeView và tải lại món ăn
            if (tvCategory.SelectedNode != null)
            {
                ShowFoodsForNode(tvCategory.SelectedNode); // [cite: 4691-4693]
            }
        }

        // Hiển thị danh mục lên TreeView
        private void ShowCategories()
        {
            tvCategory.Nodes.Clear(); // Xóa cây

            // Tạo các nút gốc
            var nodeAll = tvCategory.Nodes.Add("Tất cả");
            nodeAll.Tag = CategoryType.All; // Gán tag là enum [cite: 4650]

            var nodeFood = tvCategory.Nodes.Add("Đồ ăn");
            nodeFood.Tag = CategoryType.Food;

            var nodeDrink = tvCategory.Nodes.Add("Thức uống");
            nodeDrink.Tag = CategoryType.Drink;

            // Lấy danh mục từ CSDL
            var categories = context.Categories.ToList();

            foreach (var category in categories)
            {
                  if (category.Type == 1) // Type 1 là Đồ ăn [cite: 4648]
                {
                    var childNode = nodeFood.Nodes.Add(category.Name);
                    childNode.Tag = category; // Gán tag là đối tượng Category
                }
                 else // Type 0 là Thức uống [cite: 4647]
                {
                    var childNode = nodeDrink.Nodes.Add(category.Name);
                    childNode.Tag = category;
                }
            }
            tvCategory.ExpandAll(); // Mở rộng tất cả các nút
        }

         // Lấy danh sách món ăn theo mã danh mục [cite: 4651-4653]
        private List<FoodModel> GetFoodByCategory(int? categoryId)
        {
            var query = context.Foods
                .Include("Category") // Gộp bảng Category
                .AsQueryable();

            if (categoryId != null)
            {
                query = query.Where(f => f.FoodCategoryID == categoryId);
            }

            // Dùng Select để tạo đối tượng DTO (FoodModel)
            return query.OrderBy(f => f.Name)
                .Select(f => new FoodModel
                {
                    ID = f.ID,
                    Name = f.Name,
                    Unit = f.Unit,
                    Price = f.Price,
                    CategoryName = f.Category.Name,
                    Notes = f.Notes
                }).ToList();
        }

         // Lấy danh sách món ăn theo KIỂU (Đồ ăn/Thức uống) [cite: 4654-4655]
        private List<FoodModel> GetFoodByCategoryType(CategoryType cateType)
        {
            int type = (cateType == CategoryType.Food) ? 1 : 0;

            return context.Foods
                .Include("Category")
                .Where(f => f.Category.Type == type) // Lọc theo Type
                .OrderBy(f => f.Name)
                .Select(f => new FoodModel
                {
                    ID = f.ID,
                    Name = f.Name,
                    Unit = f.Unit,
                    Price = f.Price,
                    CategoryName = f.Category.Name,
                    Notes = f.Notes
                }).ToList();
        }

        // Hiển thị món ăn lên ListView
        private void ShowFoods(List<FoodModel> foods)
        {
            lvFood.Items.Clear();
            foreach (var food in foods)
            {
                var item = lvFood.Items.Add(food.ID.ToString());
                item.SubItems.Add(food.Name);
                item.SubItems.Add(food.Unit);
                item.SubItems.Add(food.Price.ToString("N0"));
                item.SubItems.Add(food.CategoryName);
                item.SubItems.Add(food.Notes);
            }
        }

         // Xử lý khi chọn một nút trên TreeView [cite: 4658-4662]
        private void tvCategory_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowFoodsForNode(e.Node);
        }

         // Tải món ăn dựa trên nút được chọn [cite: 4655-4657]
        private void ShowFoodsForNode(TreeNode node)
        {
            if (node == null || node.Tag == null) return;

            List<FoodModel> foods;

              if (node.Level == 0) // Nút gốc [cite: 4650]
            {
                var cateType = (CategoryType)node.Tag;
                if (cateType == CategoryType.All)
                    foods = GetFoodByCategory(null); // Tải tất cả [cite: 4645]
                else
                    foods = GetFoodByCategoryType(cateType); // Tải theo loại [cite: 4646-4647]
            }
            else // Nút con
            {
                var category = node.Tag as Category;
                foods = GetFoodByCategory(category?.ID); // Tải theo danh mục [cite: 4648]
            }

            ShowFoods(foods);
        }

         // Thêm danh mục mới [cite: 4682]
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            var dialog = new UpdateCategoryForm(); // Mở form với categoryId = null [cite: 4686]
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowCategories(); // Tải lại cây danh mục
            }
        }

         // Sửa danh mục (Double Click) [cite: 4684, 4687-4689]
        private void tvCategory_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0) return; // Không sửa nút gốc

            var category = e.Node.Tag as Category;
            if (category == null) return;

            var dialog = new UpdateCategoryForm(category.ID); // Mở form với categoryId [cite: 4688]
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowCategories(); // Tải lại cây
            }
        }

         // Thêm món ăn mới [cite: 4711-4712]
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            var dialog = new UpdateFoodForm(); // Mở form với foodId = null
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowFoodsForNode(tvCategory.SelectedNode); // Tải lại danh sách món ăn
            }
        }

         // Sửa món ăn (Double Click) [cite: 4711-4712]
        private void lvFood_DoubleClick(object sender, EventArgs e)
        {
            if (lvFood.SelectedItems.Count == 0) return;

            int foodId = int.Parse(lvFood.SelectedItems[0].Text);
            var dialog = new UpdateFoodForm(foodId); // Mở form với foodId
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowFoodsForNode(tvCategory.SelectedNode); // Tải lại
            }
        }

         // Xóa món ăn [cite: 4694]
        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            if (lvFood.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa món ăn này?", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            int foodId = int.Parse(lvFood.SelectedItems[0].Text);

            // Tìm món ăn trong CSDL
            var food = context.Foods.Find(foodId);
            if (food == null) return;

            // Xóa món ăn
            context.Foods.Remove(food);
            context.SaveChanges(); // Lưu thay đổi vào CSDL

            // Tải lại danh sách
            ShowFoodsForNode(tvCategory.SelectedNode);
        }
    }
}