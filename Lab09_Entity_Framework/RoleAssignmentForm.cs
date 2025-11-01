using Lab09_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab09_Entity_Framework
{
    public partial class RoleAssignmentForm : Form
    {
        RestaurantContext _context;
        string _accountName; // Biến lưu tên tài khoản đang xem

        public RoleAssignmentForm()
        {
            InitializeComponent();
            _context = new RestaurantContext(); // Khởi tạo Context
        }

        // Hàm này được gọi từ AccountForm
        public void LoadRoles(string accountName)
        {
            _accountName = accountName;
            this.Text = "Phân quyền cho: " + accountName;
        }

        private void RoleAssignmentForm_Load(object sender, EventArgs e)
        {
            // 1. Tải tất cả các vai trò từ CSDL
            var allRoles = _context.Roles.ToList();
            clbRoles.DataSource = allRoles;
            clbRoles.DisplayMember = "RoleName";
            clbRoles.ValueMember = "ID";

            // 2. Lấy các vai trò của tài khoản này
            var accountRoles = _context.RoleAccounts
                .Where(ra => ra.AccountName == _accountName && ra.Actived == true)
                .Select(ra => ra.RoleID) // Chỉ lấy danh sách ID
                .ToList();

            // 3. Tích chọn các vai trò mà tài khoản đó đang có
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                var role = clbRoles.Items[i] as Role;
                if (role != null && accountRoles.Contains(role.ID))
                {
                    clbRoles.SetItemChecked(i, true);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Xóa tất cả vai trò cũ của tài khoản này
                var oldRoles = _context.RoleAccounts
                    .Where(ra => ra.AccountName == _accountName);

                _context.RoleAccounts.RemoveRange(oldRoles);

                // 2. Thêm lại các vai trò được chọn
                foreach (var item in clbRoles.CheckedItems)
                {
                    var role = item as Role;
                    var newRoleAccount = new RoleAccount()
                    {
                        AccountName = _accountName,
                        RoleID = role.ID,
                        Actived = true
                    };
                    _context.RoleAccounts.Add(newRoleAccount);
                }

                // 3. Lưu tất cả thay đổi (xóa và thêm) xuống CSDL
                _context.SaveChanges();

                MessageBox.Show("Cập nhật vai trò thành công!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vai trò: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}