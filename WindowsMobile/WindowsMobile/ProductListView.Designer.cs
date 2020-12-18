namespace WindowsMobile
{
    partial class ProductListView
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.productsListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // productsListView
            // 
            this.productsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productsListView.Location = new System.Drawing.Point(0, 0);
            this.productsListView.Name = "productsListView";
            this.productsListView.Size = new System.Drawing.Size(176, 180);
            this.productsListView.TabIndex = 0;
            // 
            // ProductListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(176, 180);
            this.Controls.Add(this.productsListView);
            this.Menu = this.mainMenu1;
            this.Name = "ProductListView";
            this.Text = "List of products";
            this.Load += new System.EventHandler(this.ProductListView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView productsListView;
    }
}