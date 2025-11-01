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
    public partial class BillsForm : Form
    {
        RestaurantContext _context;

        public BillsForm()
        {
            InitializeComponent();
            _context = new RestaurantContext();

            // Đặt ngày mặc định (CSDL của bạn có dữ liệu từ 2025)
            dtpFromDate.Value = new DateTime(2025, 10, 1);
            dtpToDate.Value = DateTime.Now;
        }

        private void btnViewBills_Click(object sender, EventArgs e)
        {
            // Lấy ngày tháng từ DateTimePicker
            DateTime fromDate = dtpFromDate.Value.Date; // Lấy 00:00:00
            DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // Lấy 23:59:59

            // --- DÙNG ENTITY FRAMEWORK ĐỂ TRUY VẤN ---
            // Dùng LINQ để truy vấn
            var query = _context.Bills
                // Lọc theo ngày và chỉ lấy HĐ đã thanh toán (Status = 1)
                .Where(b => b.CheckoutDate >= fromDate && b.CheckoutDate <= toDate && b.Status == true)
                .Select(b => new { // Chọn các cột cần hiển thị
                    b.ID,
                    b.Name,
                    b.TableID,
                    b.Amount,
                    b.Discount,
                    b.Tax,
                    // Tính toán cột Thực Thu (phải xử lý giá trị NULL)
                    ThucThu = b.Amount - (b.Discount ?? 0) + (b.Tax ?? 0),
                    b.CheckoutDate,
                    b.Account
                });

            var bills = query.ToList();
            dgvBills.DataSource = bills;

            // Đổi tên cột cho đẹp
            dgvBills.Columns["ID"].HeaderText = "Mã HĐ";
            dgvBills.Columns["Name"].HeaderText = "Tên HĐ";
            dgvBills.Columns["TableID"].HeaderText = "Số bàn";
            dgvBills.Columns["Amount"].HeaderText = "Tổng tiền";
            dgvBills.Columns["Discount"].HeaderText = "Giảm giá";
            dgvBills.Columns["Tax"].HeaderText = "Thuế";
            dgvBills.Columns["ThucThu"].HeaderText = "Thực thu";
            dgvBills.Columns["CheckoutDate"].HeaderText = "Ngày TT";
            dgvBills.Columns["Account"].HeaderText = "Người lập";

            // --- TÍNH TỔNG DÙNG LINQ ---
            // (?? 0) để xử lý nếu giá trị là NULL
            var totalAmount = bills.Sum(b => (decimal)b.Amount);
            var totalDiscount = bills.Sum(b => (decimal)(b.Discount ?? 0));
            var totalRevenue = bills.Sum(b => (decimal)b.ThucThu);

            lblTotalAmount.Text = totalAmount.ToString("N0") + " đ";
            lblTotalDiscount.Text = totalDiscount.ToString("N0") + " đ";
            lblTotalRevenue.Text = totalRevenue.ToString("N0") + " đ";
        }

        private void dgvBills_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Đảm bảo người dùng nhấp vào một dòng hợp lệ
            if (e.RowIndex >= 0)
            {
                // Lấy ID của hóa đơn
                int billID = (int)dgvBills.Rows[e.RowIndex].Cells["ID"].Value;

                // Tạo Form chi tiết
                OrderDetailsForm detailsForm = new OrderDetailsForm();

                // Gọi hàm load dữ liệu
                detailsForm.LoadBillDetails(billID);

                // Hiển thị Form
                detailsForm.ShowDialog(this);
            }
        }
    }
}