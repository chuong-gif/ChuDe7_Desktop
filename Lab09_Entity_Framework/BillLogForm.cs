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
    public partial class BillLogForm : Form
    {
        RestaurantContext _context;

        public BillLogForm()
        {
            InitializeComponent();
            _context = new RestaurantContext();
        }

        public void LoadBillLog(int tableID, string tableName)
        {
            this.Text = "Nhật ký hóa đơn cho bàn: " + tableName;

            // Lấy nhật ký (chỉ HĐ đã thanh toán)
            var logs = _context.Bills
                .Where(b => b.TableID == tableID && b.Status == true)
                .Select(b => new {
                    MaHD = b.ID,
                    TenHD = b.Name,
                    ThucThu = b.Amount - (b.Discount ?? 0) + (b.Tax ?? 0),
                    NgayThanhToan = b.CheckoutDate,
                    NguoiLap = b.Account
                })
                .OrderByDescending(b => b.NgayThanhToan)
                .ToList();

            dgvBillLog.DataSource = logs;

            // Tính tổng
            int totalBills = logs.Count;
            decimal totalRevenue = logs.Sum(b => (decimal)b.ThucThu);

            lblTotalBills.Text = "Tổng số hóa đơn: " + totalBills;
            lblTotalRevenue.Text = "Tổng doanh thu: " + totalRevenue.ToString("N0") + " đ";
        }
    }
}