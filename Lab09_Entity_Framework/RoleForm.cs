using Lab09_Entity_Framework.Models; // Quan trọng: Thêm namespace Models
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity; // Quan trọng: Thêm thư viện EF
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab09_Entity_Framework
{
    public partial class RoleForm : Form
    {
        // Khai báo Context (bộ não của EF)
        RestaurantContext _context;

        public RoleForm()
        {
            InitializeComponent();
        }

        private void RoleForm_Load(object sender, EventArgs e)
        {
            _context = new RestaurantContext();
            LoadRoles();
        }

        private void LoadRoles()
        {
            // Lấy tất cả Role từ CSDL
            var roles = _context.Roles.ToList();

            // Hiển thị lên DataGridView
            dgvRoles.DataSource = roles;

            // Ẩn các cột không cần thiết
            dgvRoles.Columns["RoleAccounts"].Visible = false;
        }

        private void dgvRoles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0)
            {
                ResetForm();
                return;
            }

            // Lấy ID của dòng được chọn
            var roleId = (int)dgvRoles.SelectedRows[0].Cells["ID"].Value;

            // Hiển thị thông tin lên các ô textbox
            ShowRoleInfo(roleId);

            // Hiển thị danh sách tài khoản thuộc vai trò
            ShowAccountsInRole(roleId);
        }

        private void ShowRoleInfo(int roleId)
        {
            // Tìm vai trò trong CSDL
            var role = _context.Roles.Find(roleId);

            if (role == null) return;

            // Gán giá trị
            txtRoleID.Text = role.ID.ToString();
            txtRoleName.Text = role.RoleName;
            txtNotes.Text = role.Notes;
        }

        private void ShowAccountsInRole(int roleId)
        {
            // Yêu cầu của Bài tập: "Xem danh sách người dùng thuộc role được chọn"

            // Dùng LINQ để truy vấn
            var accounts = _context.RoleAccounts
                .Where(ra => ra.RoleID == roleId && ra.Actived == true) // Lọc theo RoleID
                .Select(ra => new {  // Chỉ chọn các cột cần thiết
                    ra.Account.AccountName,
                    ra.Account.FullName,
                    ra.Account.Email,
                    ra.Account.Tell
                })
                .ToList();

            // Hiển thị lên bảng dgvAccounts
            dgvAccounts.DataSource = accounts;
        }

        private void ResetForm()
        {
            txtRoleID.Text = "";
            txtRoleName.Text = "";
            txtNotes.Text = "";
            dgvAccounts.DataSource = null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Tên vai trò không được để trống.");
                return;
            }

            // 1. Tạo đối tượng Role mới
            var newRole = new Role()
            {
                RoleName = txtRoleName.Text,
                Notes = txtNotes.Text
            };

            // 2. Thêm vào Context
            _context.Roles.Add(newRole);

            // 3. Lưu thay đổi xuống CSDL
            _context.SaveChanges();

            // 4. Tải lại danh sách
            LoadRoles();
            ResetForm();

            MessageBox.Show("Thêm vai trò mới thành công.");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleID.Text))
            {
                MessageBox.Show("Bạn phải chọn một vai trò để cập nhật.");
                return;
            }

            // 1. Tìm vai trò
            int roleId = int.Parse(txtRoleID.Text);
            var role = _context.Roles.Find(roleId);

            if (role == null) return;

            // 2. Cập nhật thông tin
            role.RoleName = txtRoleName.Text;
            role.Notes = txtNotes.Text;

            // 3. Lưu thay đổi
            _context.SaveChanges();

            // 4. Tải lại danh sách
            LoadRoles();
            ResetForm();

            MessageBox.Show("Cập nhật vai trò thành công.");
        }
    }
}