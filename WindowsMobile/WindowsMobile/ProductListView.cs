using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Data.Common;
using WindowsMobile.DAO;
using WindowsMobile.model;

namespace WindowsMobile
{
    public partial class ProductListView : Form
    {
        public ProductListView()
        {
            InitializeComponent();
        }

        private void ProductListView_Load(object sender, EventArgs e)
        {
            DbConnection dbc = null;
            try
            {
                DB db = new SqlServer();
                dbc = db.Connect();
                dbc.Open();

                Dao dao = new Dao(db);
                Product product = new Product();

                Product[] productList = dao.Select(dbc, null, new Product(),null, new String[]{"barcode","name","image","expirationDate"}, -1, -1, null);
                foreach (Product prod in productList) {
                    var row = new string[] { prod.Barcode, prod.Name, prod.Image, prod.ExpirationDate.ToString()};
                    for (int i = 0; i < row.Length; i++)
                    {
                        ListViewItem lvi = new ListViewItem(row[i]);
                        lvi.Tag = row[i];
                        productsListView.Items.Add(lvi);
                    }
                }
            }
            catch (DbException Ex)
            {
                MessageBox.Show("SQL Exception .\n" + Ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (dbc != null && dbc.State == ConnectionState.Open)
                {
                    dbc.Close();
                }
            }
        }
    }
}