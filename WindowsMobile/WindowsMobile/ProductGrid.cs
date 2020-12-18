using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WindowsMobile.webServices;
using WindowsMobile.model;
using System.Data.SqlServerCe;
using System.Data.Common;
using WindowsMobile.DAO;

namespace WindowsMobile
{
    public partial class ProductGrid : Form
    {
        public ProductGrid()
        {
            InitializeComponent();
        }

        private void ProductGrid_Load_1(object sender, EventArgs e)
        {
            SqlCeConnection SqlCnx = null;
            try
            {
                SqlCnx = new SqlCeConnection();
                SqlCnx.ConnectionString = @"Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\MyDatabase#1.sdf;Persist Security Info=False";
                SqlCnx.Open();

                SqlServer ss = new SqlServer();
                string sql = "select barcode,image,name,expirationDate from  product";
                SqlCeCommand command = new SqlCeCommand(sql, SqlCnx);
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(command);
                adapter.SelectCommand = command;
                DataSet ds = new DataSet();
                adapter.Fill(ds, "Product");

                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = ds.Tables["Product"];

                productDataGrid.DataSource = bindingSource;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Erreur inattendue.\n" + Ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (SqlCnx.State == ConnectionState.Open)
                {
                    SqlCnx.Close();
                }
            }
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            //Delete
            DataGridCell dgc = productDataGrid.CurrentCell;
            String value = productDataGrid[dgc.RowNumber, 0].ToString();

            DbConnection dbc = null;
            try
            {
                if (value != null && value.CompareTo("") != 0)
                {
                    DB db = new SqlServer();
                    dbc = db.Connect();
                    dbc.Open();

                    Dao dao = new Dao(db);
                    Product product = new Product();
                    product.Barcode = value;

                    dao.Delete(dbc, null, product, null, "where barcode = ?", product.Barcode);

                    ProductGrid pg = new ProductGrid();
                    pg.ShowDialog();
                }
            }
            catch (DbException Ex)
            {
                MessageBox.Show("SqlException.\n" + Ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
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

        private void menuItem3_Click(object sender, EventArgs e)
        {
            DataGridCell dgc = productDataGrid.CurrentCell;
            string barecode = productDataGrid[dgc.RowNumber, 0].ToString();
            string image = productDataGrid[dgc.RowNumber, 1].ToString();
            string name = productDataGrid[dgc.RowNumber, 2].ToString();
            string expirationDate = productDataGrid[dgc.RowNumber, 3].ToString();

            string affichage = "Bar code : " + barecode + "\n\n";
            affichage += "Product picture : " + image + "\n\n";
            affichage += "Name : " + name + "\n\n";
            affichage += "Expiration date : " + expirationDate;

            MessageBox.Show(affichage);
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            AddProduct ap = new AddProduct();
            ap.ShowDialog();
        }
    }
}