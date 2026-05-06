using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Panel controlPanel;
        private Button btnAdd;
        private Button btnDelete;
        private TextBox txtSearch;
        private Label lblSearch;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlPanel = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblEnterprise = new System.Windows.Forms.Label();
            this.cmbEnterprise = new System.Windows.Forms.ComboBox();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.lblSupplyDate = new System.Windows.Forms.Label();
            this.dtpSupplyDate = new System.Windows.Forms.DateTimePicker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblInvoice = new System.Windows.Forms.Label();
            this.txtInvoice = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlPanel.Controls.Add(this.btnRefresh);
            this.controlPanel.Controls.Add(this.btnSave);
            this.controlPanel.Controls.Add(this.btnAdd);
            this.controlPanel.Controls.Add(this.btnDelete);
            this.controlPanel.Controls.Add(this.lblSearch);
            this.controlPanel.Controls.Add(this.txtSearch);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(1000, 50);
            this.controlPanel.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(20, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(126, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 35);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Удалить";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.Location = new System.Drawing.Point(470, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(50, 20);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "Поиск:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(520, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 5;
            // 
            // lblEnterprise
            // 
            this.lblEnterprise.Location = new System.Drawing.Point(26, 416);
            this.lblEnterprise.Name = "lblEnterprise";
            this.lblEnterprise.Size = new System.Drawing.Size(100, 20);
            this.lblEnterprise.TabIndex = 19;
            this.lblEnterprise.Text = "Предприятие:*";
            // 
            // cmbEnterprise
            // 
            this.cmbEnterprise.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEnterprise.Location = new System.Drawing.Point(126, 413);
            this.cmbEnterprise.Name = "cmbEnterprise";
            this.cmbEnterprise.Size = new System.Drawing.Size(250, 21);
            this.cmbEnterprise.TabIndex = 20;
            // 
            // lblEmployee
            // 
            this.lblEmployee.Location = new System.Drawing.Point(396, 416);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(120, 20);
            this.lblEmployee.TabIndex = 21;
            this.lblEmployee.Text = "Ответственный:*";
            // 
            // cmbEmployee
            // 
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.Location = new System.Drawing.Point(516, 413);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(250, 21);
            this.cmbEmployee.TabIndex = 22;
            // 
            // lblProduct
            // 
            this.lblProduct.Location = new System.Drawing.Point(543, 495);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(70, 20);
            this.lblProduct.TabIndex = 23;
            this.lblProduct.Text = "Продукт:*";
            // 
            // cmbProduct
            // 
            this.cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProduct.Location = new System.Drawing.Point(613, 492);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(200, 21);
            this.cmbProduct.TabIndex = 24;
            // 
            // lblQuantity
            // 
            this.lblQuantity.Location = new System.Drawing.Point(26, 456);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(90, 20);
            this.lblQuantity.TabIndex = 25;
            this.lblQuantity.Text = "Количество:*";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(116, 453);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(100, 20);
            this.txtQuantity.TabIndex = 26;
            // 
            // lblPrice
            // 
            this.lblPrice.Location = new System.Drawing.Point(236, 456);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(50, 20);
            this.lblPrice.TabIndex = 27;
            this.lblPrice.Text = "Цена:*";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(286, 453);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(100, 20);
            this.txtPrice.TabIndex = 28;
            // 
            // lblSupplyDate
            // 
            this.lblSupplyDate.Location = new System.Drawing.Point(586, 456);
            this.lblSupplyDate.Name = "lblSupplyDate";
            this.lblSupplyDate.Size = new System.Drawing.Size(110, 20);
            this.lblSupplyDate.TabIndex = 31;
            this.lblSupplyDate.Text = "Дата поставки:*";
            // 
            // dtpSupplyDate
            // 
            this.dtpSupplyDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSupplyDate.Location = new System.Drawing.Point(696, 453);
            this.dtpSupplyDate.Name = "dtpSupplyDate";
            this.dtpSupplyDate.Size = new System.Drawing.Size(150, 20);
            this.dtpSupplyDate.TabIndex = 32;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(26, 496);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 20);
            this.lblStatus.TabIndex = 33;
            this.lblStatus.Text = "Статус:*";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Location = new System.Drawing.Point(86, 493);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 21);
            this.cmbStatus.TabIndex = 34;
            // 
            // lblInvoice
            // 
            this.lblInvoice.Location = new System.Drawing.Point(256, 496);
            this.lblInvoice.Name = "lblInvoice";
            this.lblInvoice.Size = new System.Drawing.Size(130, 20);
            this.lblInvoice.TabIndex = 35;
            this.lblInvoice.Text = "Номер накладной:";
            // 
            // txtInvoice
            // 
            this.txtInvoice.Location = new System.Drawing.Point(386, 493);
            this.txtInvoice.Name = "txtInvoice";
            this.txtInvoice.Size = new System.Drawing.Size(150, 20);
            this.txtInvoice.TabIndex = 36;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 48);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1000, 359);
            this.dataGridView1.TabIndex = 37;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(229, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 35);
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click_1);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(335, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Поиск";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtTotal.Location = new System.Drawing.Point(466, 453);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(100, 20);
            this.txtTotal.TabIndex = 30;
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point(406, 456);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(60, 20);
            this.lblTotal.TabIndex = 29;
            this.lblTotal.Text = "Сумма:*";
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 520);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblEnterprise);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.cmbEnterprise);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblEmployee);
            this.Controls.Add(this.txtInvoice);
            this.Controls.Add(this.cmbEmployee);
            this.Controls.Add(this.lblInvoice);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.cmbProduct);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.dtpSupplyDate);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.lblSupplyDate);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtPrice);
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Управление поставками";
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblEnterprise;
        private ComboBox cmbEnterprise;
        private Label lblEmployee;
        private ComboBox cmbEmployee;
        private Label lblProduct;
        private ComboBox cmbProduct;
        private Label lblQuantity;
        private TextBox txtQuantity;
        private Label lblPrice;
        private TextBox txtPrice;
        private Label lblSupplyDate;
        private DateTimePicker dtpSupplyDate;
        private Label lblStatus;
        private ComboBox cmbStatus;
        private Label lblInvoice;
        private TextBox txtInvoice;
        private DataGridView dataGridView1;
        private Button btnRefresh;
        private Button btnSave;
        private TextBox txtTotal;
        private Label lblTotal;
    }
}