using Lab09_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity; // Thêm thư viện EF
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab09_Entity_Framework
{
    public partial class AccountForm : Form
    {
        RestaurantContext _context;
        List<Account> _accounts; // Danh sách tài khoản gốc (dùng để lọc)

        public AccountForm()
        {
            InitializeComponent();
            _context = new RestaurantContext();
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {
            // Tải danh sách Role (vai trò) vào ComboBox lọc
            LoadRolesToComboBox();

            // Tải danh sách tài khoản
            LoadAccounts();
        }

        private void LoadRolesToComboBox()
        {
            var roles = _context.Roles.ToList();

            // Thêm một mục "Tất cả"
            roles.Insert(0, new Role() { ID = 0, RoleName = "Tất cả vai trò" });

            cbbRole.DataSource = roles;
            cbbRole.DisplayMember = "RoleName";
            cbbRole.ValueMember = "ID";
        }

        private void LoadAccounts()
        {
            // Lấy tất cả tài khoản
            // .Include("RoleAccounts") tải kèm danh sách vai trò
            _accounts = _context.Accounts.Include("RoleAccounts").ToList();

            // Hiển thị lên DataGridView
            // Chỉ hiển thị các cột cơ bản
            var view = _accounts.Select(acc => new {
                acc.AccountName,
                acc.FullName,
                acc.Email,
                acc.Tell,
                acc.DateCreated
            }).ToList();

            dgvAccounts.DataSource = view;

            ClearTextboxes();
        }

        private void ClearTextboxes()
        {
            txtAccountName.Text = "";
            txtPassword.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtTell.Text = "";
            txtAccountName.Enabled = true; // Cho phép nhập Tên TK
        }

        private void dgvAccounts_SelectionChanged(object sender, EventArgs e)
        {
            // Khi bấm vào 1 dòng trong DataGridView
            if (dgvAccounts.SelectedRows.Count == 0) return;

            string accountName = dgvAccounts.SelectedRows[0].Cells["AccountName"].Value.ToString();

            // Tìm tài khoản trong danh sách đã tải
            var account = _accounts.FirstOrDefault(x => x.AccountName == accountName);
            if (account == null) return;

            // Hiển thị thông tin
            txtAccountName.Text = account.AccountName;
            txtFullName.Text = account.FullName;
            txtEmail.Text = account.Email;
            txtTell.Text = account.Tell;
            txtPassword.Text = ""; // Không hiển thị mật khẩu

            txtAccountName.Enabled = false; // Không cho sửa Tên TK (khóa chính)
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Tên TK, Mật khẩu và Họ tên không được để trống!");
                return;
            }

            // Kiểm tra TK đã tồn tại chưa
            var account = _context.Accounts.Find(txtAccountName.Text);
            if (account != null)
            {
                MessageBox.Show("Tên tài khoản đã tồn tại.");
                return;
            }

            // 1. Tạo tài khoản mới
            var newAccount = new Account()
            {
                AccountName = txtAccountName.Text,
                Password = txtPassword.Text, // Thực tế cần mã hóa
                FullName = txtFullName.Text,
                Email = txtEmail.Text,
                Tell = txtTell.Text,
                DateCreated = DateTime.Now
            };

            // 2. Thêm vào Context
            _context.Accounts.Add(newAccount);

            // 3. Lưu CSDL
            _context.SaveChanges();

            LoadAccounts(); // Tải lại danh sách
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                MessageBox.Show("Bạn phải chọn một tài khoản để cập nhật!");
                return;
            }

            // 1. Tìm tài khoản
            var account = _context.Accounts.Find(txtAccountName.Text);
            if (account == null) return;

            // 2. Cập nhật thông tin
            account.FullName = txtFullName.Text;
            account.Email = txtEmail.Text;
            account.Tell = txtTell.Text;

            // 3. Đánh dấu là đã sửa
            _context.Entry(account).State = EntityState.Modified;

            // 4. Lưu CSDL
            _context.SaveChanges();

            LoadAccounts(); // Tải lại
        }

        private void btnResetPass_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                MessageBox.Show("Bạn phải chọn một tài khoản!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Bạn phải nhập mật khẩu mới vào ô Mật khẩu!");
                return;
            }

            // 1. Tìm tài khoản
            var account = _context.Accounts.Find(txtAccountName.Text);
            if (account == null) return;

            // 2. Cập nhật mật khẩu
            account.Password = txtPassword.Text; // Mật khẩu mới

            // 3. Lưu
            _context.SaveChanges();

            MessageBox.Show("Reset mật khẩu thành công.");
            ClearTextboxes();
        }

        // --- MENU CHUỘT PHẢI ---
        private void tsmDeleteAccount_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count == 0) return;
            string accountName = dgvAccounts.SelectedRows[0].Cells["AccountName"].Value.ToString();

            if (MessageBox.Show("Bạn có chắc muốn XÓA (vô hiệu hóa) tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            // Yêu cầu: Xóa Password, gán về null
            var account = _context.Accounts.Find(accountName);
            if (account == null) return;

            account.Password = null; // Gán mật khẩu về NULL

            // Yêu cầu: Vô hiệu hóa vai trò (Active = 0)
            var roles = _context.RoleAccounts.Where(x => x.AccountName == accountName).ToList();
            roles.ForEach(r => r.Actived = false); // Đặt Actived = 0

            // Lưu CSDL
            _context.SaveChanges();

            MessageBox.Show("Đã xóa mật khẩu và vô hiệu hóa các vai trò của tài khoản.");
            LoadAccounts();
        }

        private void tsmViewRoles_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count == 0) return;
            string accountName = dgvAccounts.SelectedRows[0].Cells["AccountName"].Value.ToString();

            // Mở Form gán vai trò
            RoleAssignmentForm form = new RoleAssignmentForm();
            form.LoadRoles(accountName);
            form.ShowDialog(this);
        }

        // --- LỌC VÀ TÌM KIẾM ---
        private void ApplyFilter()
        {
            if (_accounts == null) return; // Chặn lỗi khi Form đang load
            if (cbbRole.SelectedItem == null) return;

            var selectedRole = cbbRole.SelectedItem as Role;
            if (selectedRole == null) return;

            int roleID = selectedRole.ID;
            string keyword = txtSearch.Text.ToLower();

            // 1. Bắt đầu với danh sách gốc
            var query = _accounts.AsQueryable();

            // 2. Lọc theo Role (nếu có chọn)
            if (roleID > 0) // (0 là "Tất cả")
            {
                query = query.Where(acc =>
                    acc.RoleAccounts.Any(ra => ra.RoleID == roleID && ra.Actived == true));
            }

            // 3. Lọc theo Tên (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(acc =>
                    acc.FullName.ToLower().Contains(keyword) ||
                    acc.AccountName.ToLower().Contains(keyword));
            }

            // 4. Hiển thị kết quả (chỉ các cột cần thiết)
            dgvAccounts.DataSource = query.Select(acc => new {
                acc.AccountName,
                acc.FullName,
                acc.Email,
                acc.Tell,
                acc.DateCreated
            }).ToList();
        }

        private void cbbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
    }
}