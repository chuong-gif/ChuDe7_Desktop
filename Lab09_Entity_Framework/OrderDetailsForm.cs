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
    public partial class OrderDetailsForm : Form
    {
        RestaurantContext _context;

        public OrderDetailsForm()
        {
            InitializeComponent();
            _context = new RestaurantContext(); // Khởi tạo Context
        }

        public void LoadBillDetails(int billID)
        {
            this.Text = "Chi tiết hóa đơn số: " + billID;

            // --- DÙNG ENTITY FRAMEWORK VÀ LINQ ---
            // Truy vấn, Join 2 bảng BillDetails và Food
            var query = _context.BillDetails
                .Where(bd => bd.InvoiceID == billID) // Lọc theo ID Hóa đơn
                .Select(bd => new { // Chọn các cột cần hiển thị
                    TenMonAn = bd.Food.Name,
                    SoLuong = bd.Quantity,
                    DonGia = bd.Food.Price,
                    ThanhTien = bd.Quantity * bd.Food.Price
                })
                .ToList();

            dgvBillDetails.DataSource = query;
        }
    }
}