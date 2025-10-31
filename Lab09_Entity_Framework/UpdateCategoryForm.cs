using Lab09_Entity_Framework.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab09_Entity_Framework
{
    public partial class UpdateCategoryForm : Form
    {
         // Biến lưu ID, nếu null là Thêm mới, nếu có giá trị là Cập nhật [cite: 4668]
        private int? _categoryId; // [cite: 4668]
        RestaurantContext context = new RestaurantContext();

         // Constructor cho Thêm mới [cite: 4668]
         
        public UpdateCategoryForm(int? categoryId = null) // [cite: 4668]
        {
            InitializeComponent();
            _categoryId = categoryId;
        }

        // Tải thông tin danh mục nếu là Cập nhật
        private void UpdateCategoryForm_Load(object sender, EventArgs e)
        {
             // Load ComboBox Type [cite: 4673]
            cboType.Items.Add(new { Value = 0, Text = "Thức uống" });
            cboType.Items.Add(new { Value = 1, Text = "Đồ ăn" });
            cboType.DisplayMember = "Text";
            cboType.ValueMember = "Value";

              if (_categoryId != null) // Chế độ Cập nhật [cite: 4671]
            {
                this.Text = "Cập nhật Danh mục";
                DisplayCategoryInfo(); // [cite: 4672]
            }
            else // Chế độ Thêm mới
            {
                this.Text = "Thêm Danh mục";
                cboType.SelectedIndex = 0; // Mặc định là thức uống
            }
        }

         // Hiển thị thông tin lên form [cite: 4673]
        private void DisplayCategoryInfo()
        {
            var category = context.Categories.Find(_categoryId);
            if (category == null)
            {
                MessageBox.Show("Không tìm thấy danh mục.");
                this.Close();
                return;
            }

            txtID.Text = category.ID.ToString();
            txtName.Text = category.Name;
            cboType.SelectedValue = category.Type;
        }

         // Hàm kiểm tra dữ liệu nhập [cite: 4679-4680]
        private bool ValidateUserInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên danh mục không được để trống.");
                return false;
            }

            if (cboType.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại danh mục.");
                return false;
            }
            return true;
        }

         // Lấy đối tượng Category từ Form [cite: 4680]
        private Category GetCategory()
        {
            var category = new Category
            {
                Name = txtName.Text,
                Type = (int)cboType.SelectedValue
            };

            if (_categoryId != null) // Nếu là cập nhật, gán ID
            {
                category.ID = _categoryId.Value;
            }
            return category;
        }

         // Xử lý nút Lưu [cite: 4681]
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateUserInput()) return;

            var newCategory = GetCategory();

            try
            {
                  if (_categoryId == null) // Thêm mới [cite: 4679]
                {
                    context.Categories.Add(newCategory);
                }
                 else // Cập nhật [cite: 4678]
                {
                    var oldCategory = context.Categories.Find(_categoryId);
                    if (oldCategory != null)
                    {
                        oldCategory.Name = newCategory.Name;
                        oldCategory.Type = newCategory.Type;
                    }
                }
                context.SaveChanges(); // Lưu thay đổi vào CSDL

                this.DialogResult = DialogResult.OK; // Đóng form và báo thành công
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }
    }
}