using Lab09_Entity_Framework.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab09_Entity_Framework
{
    public partial class UpdateFoodForm : Form
    {
        private int? _foodId; // [cite: 4699]
        RestaurantContext context = new RestaurantContext();

         
        public UpdateFoodForm(int? foodId = null) // [cite: 4699]
        {
            InitializeComponent();
            _foodId = foodId;
        }

        private void UpdateFoodForm_Load(object sender, EventArgs e)
        {
            InitCategoryComboBox(); // Tải danh mục [cite: 4701]

            if (_foodId != null) // Chế độ Cập nhật
            {
                this.Text = "Cập nhật Món ăn";
                DisplayFoodInfo();
            }
            else // Chế độ Thêm mới
            {
                this.Text = "Thêm Món ăn";
            }
        }

        // Tải danh mục vào ComboBox
        private void InitCategoryComboBox()
        {
            cboCategory.DataSource = context.Categories.OrderBy(c => c.Name).ToList();
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "ID";
        }

         // Lấy thông tin món ăn (dùng lại ở nhiều nơi) [cite: 4704-4705]
        private Food GetFood(int foodId)
        {
            return context.Foods.Find(foodId);
        }

         // Hiển thị thông tin món ăn lên Form [cite: 4706]
        private void DisplayFoodInfo()
        {
            var food = GetFood(_foodId.Value);
            if (food == null)
            {
                MessageBox.Show("Không tìm thấy món ăn.");
                this.Close();
                return;
            }

            txtID.Text = food.ID.ToString();
            txtName.Text = food.Name;
            txtUnit.Text = food.Unit;
            nudPrice.Value = food.Price;
            txtNotes.Text = food.Notes;
            cboCategory.SelectedValue = food.FoodCategoryID;
        }

         // Kiểm tra dữ liệu nhập [cite: 4707]
        private bool ValidateUserInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên món ăn không được để trống.");
                return false;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục.");
                return false;
            }
            return true;
        }

         // Lấy đối tượng Food từ Form [cite: 4708]
        private Food GetFood()
        {
            var food = new Food
            {
                Name = txtName.Text,
                Unit = txtUnit.Text,
                Price = (int)nudPrice.Value,
                Notes = txtNotes.Text,
                FoodCategoryID = (int)cboCategory.SelectedValue
            };

            if (_foodId != null)
            {
                food.ID = _foodId.Value;
            }
            return food;
        }

         // Xử lý nút Lưu [cite: 4709-4710]
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateUserInput()) return;

            var newFood = GetFood();

            try
            {
                if (_foodId == null) // Thêm mới
                {
                    context.Foods.Add(newFood);
                }
                else // Cập nhật
                {
                    var oldFood = GetFood(_foodId.Value);
                    if (oldFood != null)
                    {
                        oldFood.Name = newFood.Name;
                        oldFood.Unit = newFood.Unit;
                        oldFood.Price = newFood.Price;
                        oldFood.Notes = newFood.Notes;
                        oldFood.FoodCategoryID = newFood.FoodCategoryID;
                    }
                }
                context.SaveChanges();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }
    }
}