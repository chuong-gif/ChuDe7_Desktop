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
    public partial class TableForm : Form
    {
        RestaurantContext _context;

        // Lớp con để lưu Trạng thái (0, 1, 2)
        private class TableStatus
        {
            public int Value { get; set; }
            public string Display { get; set; }
        }

        public TableForm()
        {
            InitializeComponent();
            _context = new RestaurantContext();
        }

        private void TableForm_Load(object sender, EventArgs e)
        {
            LoadStatusComboBox();
            LoadTables();
        }

        private void LoadStatusComboBox()
        {
            // Tạo danh sách các trạng thái
            List<TableStatus> statusList = new List<TableStatus>();
            statusList.Add(new TableStatus() { Value = 0, Display = "Trống" });
            statusList.Add(new TableStatus() { Value = 1, Display = "Có khách" });
            statusList.Add(new TableStatus() { Value = 2, Display = "Đã đặt" });

            cbbStatus.DataSource = statusList;
            cbbStatus.DisplayMember = "Display";
            cbbStatus.ValueMember = "Value";
        }

        private void LoadTables()
        {
            // Tải danh sách Bàn từ CSDL
            var tables = _context.Tables.ToList();

            // Chuyển đổi trạng thái từ số (0,1,2) sang chữ
            var tableView = tables.Select(t => new {
                t.ID,
                t.Name,
                StatusText = (t.Status == 0 ? "Trống" : (t.Status == 1 ? "Có khách" : "Đã đặt")),
                t.Capacity,
                t.Status // Giữ lại cột Status gốc (ẩn)
            }).ToList();

            dgvTables.DataSource = tableView;

            // Đổi tên cột
            dgvTables.Columns["ID"].HeaderText = "Mã Bàn";
            dgvTables.Columns["Name"].HeaderText = "Tên Bàn";
            dgvTables.Columns["StatusText"].HeaderText = "Trạng thái";
            dgvTables.Columns["Capacity"].HeaderText = "Sức chứa";
            dgvTables.Columns["Status"].Visible = false; // Ẩn cột Status (số)

            ClearTextboxes();
        }

        private void ClearTextboxes()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtCapacity.Text = "";
            cbbStatus.SelectedIndex = 0;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void dgvTables_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTables.SelectedRows.Count == 0) return;

            // Lấy ID từ dòng được chọn
            int tableId = (int)dgvTables.SelectedRows[0].Cells["ID"].Value;

            // Tìm đối tượng Table trong CSDL
            var table = _context.Tables.Find(tableId);
            if (table == null) return;

            // Hiển thị thông tin
            txtID.Text = table.ID.ToString();
            txtName.Text = table.Name;
            txtCapacity.Text = table.Capacity?.ToString(); // Dùng ? vì Capacity cho phép NULL
            cbbStatus.SelectedValue = table.Status;

            btnAdd.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        // --- CÁC NÚT THÊM, SỬA, XÓA ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên bàn không được để trống!");
                return;
            }

            // 1. Tạo đối tượng Table mới
            var table = new Table()
            {
                Name = txtName.Text,
                Status = (int)cbbStatus.SelectedValue,
                Capacity = string.IsNullOrWhiteSpace(txtCapacity.Text) ? (int?)null : Convert.ToInt32(txtCapacity.Text)
            };

            // 2. Thêm vào Context
            _context.Tables.Add(table);

            // 3. Lưu CSDL
            _context.SaveChanges();

            LoadTables(); // Tải lại
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text)) return;

            int tableId = int.Parse(txtID.Text);

            // 1. Tìm bàn
            var table = _context.Tables.Find(tableId);
            if (table == null) return;

            // 2. Cập nhật
            table.Name = txtName.Text;
            table.Status = (int)cbbStatus.SelectedValue;
            table.Capacity = string.IsNullOrWhiteSpace(txtCapacity.Text) ? (int?)null : Convert.ToInt32(txtCapacity.Text);

            // 3. Lưu
            _context.SaveChanges();

            LoadTables(); // Tải lại
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text)) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa bàn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            int tableId = int.Parse(txtID.Text);

            // 1. Tìm bàn
            var table = _context.Tables.Find(tableId);
            if (table == null) return;

            try
            {
                // 2. Xóa
                _context.Tables.Remove(table);
                // 3. Lưu
                _context.SaveChanges();
                LoadTables(); // Tải lại
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu xóa bàn đang có hóa đơn (lỗi khóa ngoại)
                MessageBox.Show("Lỗi: Không thể xóa bàn này vì đã có hóa đơn liên quan. " + ex.Message, "Lỗi Database");
            }
        }

        // --- MENU CHUỘT PHẢI ---
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (dgvTables.SelectedRows.Count == 0)
            {
                // Không chọn, tắt hết
                tsmViewCurrentBill.Enabled = false;
                tsmViewBillHistory.Enabled = false;
                tsmViewBillLog.Enabled = false;
                tsmDeleteTable.Enabled = false;
            }
            else
            {
                // Có chọn, bật các menu lịch sử
                tsmViewBillHistory.Enabled = true;
                tsmViewBillLog.Enabled = true;
                tsmDeleteTable.Enabled = true;

                // Kiểm tra trạng thái "Có khách" (Status = 1)
                int status = (int)dgvTables.SelectedRows[0].Cells["Status"].Value;
                if (status == 1)
                {
                    tsmViewCurrentBill.Enabled = true;
                }
                else
                {
                    tsmViewCurrentBill.Enabled = false;
                }
            }
        }

        private void tsmViewCurrentBill_Click(object sender, EventArgs e)
        {
            int tableID = (int)dgvTables.SelectedRows[0].Cells["ID"].Value;

            // Tìm hóa đơn CHƯA THANH TOÁN (Status=0) của bàn này
            var currentBill = _context.Bills
                .FirstOrDefault(b => b.TableID == tableID && b.Status == false);

            if (currentBill != null)
            {
                // Mở Form BillDetails (từ Bài 3) và hiển thị
                OrderDetailsForm detailsForm = new OrderDetailsForm();
                detailsForm.LoadBillDetails(currentBill.ID);
                detailsForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Không tìm thấy hóa đơn nào đang mở cho bàn này.");
            }
        }

        private void tsmViewBillHistory_Click(object sender, EventArgs e)
        {
            int tableID = (int)dgvTables.SelectedRows[0].Cells["ID"].Value;
            string tableName = dgvTables.SelectedRows[0].Cells["Name"].Value.ToString();

            BillHistoryForm historyForm = new BillHistoryForm();
            historyForm.LoadBillHistory(tableID, tableName);
            historyForm.ShowDialog(this);
        }

        private void tsmViewBillLog_Click(object sender, EventArgs e)
        {
            int tableID = (int)dgvTables.SelectedRows[0].Cells["ID"].Value;
            string tableName = dgvTables.SelectedRows[0].Cells["Name"].Value.ToString();

            BillLogForm logForm = new BillLogForm();
            logForm.LoadBillLog(tableID, tableName);
            logForm.ShowDialog(this);
        }

        private void tsmDeleteTable_Click(object sender, EventArgs e)
        {
            // Gọi sự kiện Click của nút Xóa
            btnDelete.PerformClick();
        }
    }
}