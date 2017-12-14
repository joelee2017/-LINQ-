using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 常用LINQ查詢
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建立 AutoCompleteStringCollection 自動填入值(提示)
        /// asc.AddRange項目加入結尾，將CompanyName加入
        /// btnCompanyName.AutoCompleteCustomSource = asc;
        /// 控制項btnCompanyName使用AutoCompleteCustomSource屬性，實作asc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)//在Textbox控制項，出現提示
        {
            NorthwindEntities dc = new NorthwindEntities();//Entity連結db
            AutoCompleteStringCollection asc = new AutoCompleteStringCollection();
            asc.AddRange(dc.Customers.Select(c => c.CompanyName).ToArray());
            btnCompanyName.AutoCompleteCustomSource = asc;
        }

        /// <summary>
        /// NorthwindEntities dc = new NorthwindEntities();因快取機制建議每次都新建一個
        /// query 查詢==textbox中的值，Single只取得一筆記錄 查詢ordersID
        /// 錯誤訊息 if (combobox 資料來源 query.toarray) else 查詢 null 的話顯示尚無訂單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            NorthwindEntities dc = new NorthwindEntities();
            var query = dc.Customers.Where(c => c.CompanyName == btnCompanyName.Text)
                                                .Single().Orders.Select(o => o.OrderID);
            if(query.Count() !=0)
            {
                cbOrders.DataSource = query.ToArray();
            }
            else
            {
                cbOrders.DataSource = null;
                MessageBox.Show("尚無訂單");
            }            

        }

        /// <summary>
        /// dc.Order_Details.ToArray() 條件後先將欲查詢目標資料表ToArray() 轉成集合
        /// int.Parse( cbOrders.Text)因型態不同，使用parse
        /// od.UnitPrice * od.Quantity*(1 -(decimal)od.Discount) 型態不同所以轉型(decimal)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            NorthwindEntities dc = new NorthwindEntities();
            var query = dc.Order_Details.ToArray().Where
                            (od => od.OrderID == int.Parse( cbOrders.Text));
            
            dataGridView1.DataSource = query.Select(od => new
            {

                訂單編號 = od.OrderID,
                商品編號 = od.ProductID,
                商品單價 = od.UnitPrice,
                購買數量 = od.Quantity,
                優惠折扣 = od.Discount,
                商品小計 = (od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)).ToString("c")
            }).ToArray();

            decimal subtotal = query.Sum(od =>
                                    od.UnitPrice * od.Quantity*(1 -(decimal)od.Discount));
            labtotal.Text = string.Format($"小計：{subtotal.ToString("C")}");

        }
    }
}
