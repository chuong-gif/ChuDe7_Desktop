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
    public partial class BillHistoryForm : Form
    {
        RestaurantContext _context;
        private int _tableID;

        public BillHistoryForm()
        {
            InitializeComponent();
            _context = new RestaurantContext();
        }

        public void LoadBillHistory(int tableID, string tableName)
        {
            _tableID = tableID;
            this.Text = "Lịch sử hóa đơn cho bàn: " + tableName;
            LoadBillDates(tableID);
        }

        private void LoadBillDates(int tableID)
        {
            // Lấy danh sách các ngày (duy nhất) có hóa đơn (đã thanh toán)
            var dates = _context.Bills
                .Where(b => b.TableID == tableID && b.Status == true && b.CheckoutDate != null)
                .Select(b => DbFunctions.TruncateTime(b.CheckoutDate)) // Chỉ lấy ngày, bỏ giờ
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            // Gán nguồn dữ liệu cho ListBox
            lsbBillDates.DataSource = dates;
            lsbBillDates.FormatString = "dd/MM/yyyy"; // Định dạng ngày

            lsbBillDates.ClearSelected();
            dgvBillDetails.DataSource = null;
        }

        private void lsbBillDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi người dùng chọn 1 ngày
            if (lsbBillDates.SelectedValue == null) return;

            DateTime selectedDate = (DateTime)lsbBillDates.SelectedValue;

            // Lấy chi tiết hóa đơn của bàn đó VÀO ngày đó
            var details = _context.Bills
                .Where(b => b.TableID == _tableID &&
                            b.Status == true &&
                            DbFunctions.TruncateTime(b.CheckoutDate) == selectedDate)
                .SelectMany(b => b.BillDetails) // Join với BillDetails
                .Select(bd => new { // Chọn các cột cần thiết
                    bd.Bill.ID,
                    bd.Food.Name,
                    bd.Quantity,
                    bd.Food.Price,
                    Total = bd.Quantity * bd.Food.Price
                })
                .ToList();

            dgvBillDetails.DataSource = details;
        }
    }
}