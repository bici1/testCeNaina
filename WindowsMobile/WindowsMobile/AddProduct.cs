using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WindowsMobile.model;
using WindowsMobile.DAO;
using System.Data.Common;
using WindowsMobile.webServices;

namespace WindowsMobile
{
    public partial class AddProduct : Form
    {
        public AddProduct()
        {
            InitializeComponent();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            DbConnection dbc = null;
            try
            {
                DB db = new SqlServer();
                dbc = db.Connect();
                dbc.Open();

                Dao dao = new Dao(db);
                Product product = new Product();
                
                product.Barcode = barCodeTextBox.Text;
                product.checkBarCode();
                Product.checkExpiration_date(ExpirationDatePicker.Text);
                product.ExpirationDate = DateTime.Parse(ExpirationDatePicker.Text);
                
                /*Select API*/
                OpenFoodFactsClientAPI offa = new OpenFoodFactsClientAPI();
                Product productTemp = offa.GetProductByCode(product.Barcode);

                if (productTemp != null)
                {
                    Product[] prodList = dao.Select(dbc, null, new Product(), null, null, -1, -1, "where barcode = ?", product.Barcode);
                    if (prodList.Length > 0)
                    {
                        Product temp = prodList[0];

                        List<string> notAdd = new List<string>();
                        notAdd.Add("id");
                        productTemp.ExpirationDate = product.ExpirationDate;

                        //Update the product which has the same id than temp.Id
                        dao.Update(dbc, null, productTemp, null, null, notAdd, "where id=?", temp.Id);
                    }
                    else
                    {
                        //Add the other elements from l'API 
                        dao.Insert(dbc, null, productTemp, null, null, false);
                    }

                    //this.Close();
                    ProductGrid pl = new ProductGrid();
                    pl.ShowDialog();
                }
                else {
                    throw new Exception("Error, product not found!");
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

        private void AddProduct_Load(object sender, EventArgs e)
        {

        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            ProductGrid pl = new ProductGrid();
            pl.ShowDialog();
        }
    }
}