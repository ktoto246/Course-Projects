using System.Resources;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridViewEmployees;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.Panel panelFormHeader;
        private System.Windows.Forms.Label labelFormTitle;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.TextBox textBoxФамилия;
        private System.Windows.Forms.TextBox textBoxИмя;
        private System.Windows.Forms.TextBox textBoxОтчество;
        private System.Windows.Forms.TextBox textBoxДолжность;
        private System.Windows.Forms.TextBox textBoxОклад;
        private System.Windows.Forms.TextBox textBoxТелефон;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.DateTimePicker dateTimePickerПрием;
        private System.Windows.Forms.Label labelФамилия;
        private System.Windows.Forms.Label labelИмя;
        private System.Windows.Forms.Label labelОтчество;
        private System.Windows.Forms.Label labelДолжность;
        private System.Windows.Forms.Label labelОклад;
        private System.Windows.Forms.Label labelТелефон;
        private System.Windows.Forms.Label labelДатаПриема;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Button buttonClose;
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
            this.dataGridViewEmployees = new System.Windows.Forms.DataGridView();
            this.panelControls = new System.Windows.Forms.Panel();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.panelFormHeader = new System.Windows.Forms.Panel();
            this.labelFormTitle = new System.Windows.Forms.Label();
            this.panelInput = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.dateTimePickerПрием = new System.Windows.Forms.DateTimePicker();
            this.textBoxТелефон = new System.Windows.Forms.TextBox();
            this.textBoxОклад = new System.Windows.Forms.TextBox();
            this.textBoxДолжность = new System.Windows.Forms.TextBox();
            this.textBoxОтчество = new System.Windows.Forms.TextBox();
            this.textBoxИмя = new System.Windows.Forms.TextBox();
            this.textBoxФамилия = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.labelДатаПриема = new System.Windows.Forms.Label();
            this.labelТелефон = new System.Windows.Forms.Label();
            this.labelОклад = new System.Windows.Forms.Label();
            this.labelДолжность = new System.Windows.Forms.Label();
            this.labelОтчество = new System.Windows.Forms.Label();
            this.labelИмя = new System.Windows.Forms.Label();
            this.labelФамилия = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmployees)).BeginInit();
            this.panelControls.SuspendLayout();
            this.panelFormHeader.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewEmployees
            // 
            this.dataGridViewEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEmployees.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEmployees.Location = new System.Drawing.Point(12, 280);
            this.dataGridViewEmployees.Name = "dataGridViewEmployees";
            this.dataGridViewEmployees.Size = new System.Drawing.Size(860, 300);
            this.dataGridViewEmployees.TabIndex = 0;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.panelControls.Controls.Add(this.buttonRefresh);
            this.panelControls.Controls.Add(this.buttonSearch);
            this.panelControls.Controls.Add(this.buttonDelete);
            this.panelControls.Controls.Add(this.buttonAdd);
            this.panelControls.Controls.Add(this.textBoxSearch);
            this.panelControls.Controls.Add(this.labelSearch);
            this.panelControls.Location = new System.Drawing.Point(12, 220);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(860, 50);
            this.panelControls.TabIndex = 1;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRefresh.ForeColor = System.Drawing.Color.White;
            this.buttonRefresh.Location = new System.Drawing.Point(730, 10);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 30);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "Обновить";
            this.buttonRefresh.UseVisualStyleBackColor = false;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSearch.ForeColor = System.Drawing.Color.White;
            this.buttonSearch.Location = new System.Drawing.Point(350, 10);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(100, 30);
            this.buttonSearch.TabIndex = 4;
            this.buttonSearch.Text = "Поиск";
            this.buttonSearch.UseVisualStyleBackColor = false;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(20)))), ((int)(((byte)(60)))));
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonDelete.ForeColor = System.Drawing.Color.White;
            this.buttonDelete.Location = new System.Drawing.Point(190, 10);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(120, 30);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "Удалить";
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAdd.ForeColor = System.Drawing.Color.White;
            this.buttonAdd.Location = new System.Drawing.Point(30, 10);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(120, 30);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "Добавить";
            this.buttonAdd.UseVisualStyleBackColor = false;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(470, 14);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(200, 22);
            this.textBoxSearch.TabIndex = 1;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSearch.Location = new System.Drawing.Point(470, -5);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(103, 15);
            this.labelSearch.TabIndex = 0;
            this.labelSearch.Text = "Поиск по ФИО";
            // 
            // panelFormHeader
            // 
            this.panelFormHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFormHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.panelFormHeader.Controls.Add(this.labelFormTitle);
            this.panelFormHeader.Location = new System.Drawing.Point(0, 0);
            this.panelFormHeader.Name = "panelFormHeader";
            this.panelFormHeader.Size = new System.Drawing.Size(884, 50);
            this.panelFormHeader.TabIndex = 2;
            // 
            // labelFormTitle
            // 
            this.labelFormTitle.AutoSize = true;
            this.labelFormTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelFormTitle.ForeColor = System.Drawing.Color.White;
            this.labelFormTitle.Location = new System.Drawing.Point(20, 15);
            this.labelFormTitle.Name = "labelFormTitle";
            this.labelFormTitle.Size = new System.Drawing.Size(254, 24);
            this.labelFormTitle.TabIndex = 0;
            this.labelFormTitle.Text = "Управление персоналом";
            // 
            // panelInput
            // 
            this.panelInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelInput.Controls.Add(this.buttonClose);
            this.panelInput.Controls.Add(this.dateTimePickerПрием);
            this.panelInput.Controls.Add(this.textBoxТелефон);
            this.panelInput.Controls.Add(this.textBoxОклад);
            this.panelInput.Controls.Add(this.textBoxДолжность);
            this.panelInput.Controls.Add(this.textBoxОтчество);
            this.panelInput.Controls.Add(this.textBoxИмя);
            this.panelInput.Controls.Add(this.textBoxФамилия);
            this.panelInput.Controls.Add(this.textBoxID);
            this.panelInput.Controls.Add(this.labelДатаПриема);
            this.panelInput.Controls.Add(this.labelТелефон);
            this.panelInput.Controls.Add(this.labelОклад);
            this.panelInput.Controls.Add(this.labelДолжность);
            this.panelInput.Controls.Add(this.labelОтчество);
            this.panelInput.Controls.Add(this.labelИмя);
            this.panelInput.Controls.Add(this.labelФамилия);
            this.panelInput.Controls.Add(this.labelID);
            this.panelInput.Location = new System.Drawing.Point(12, 60);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(860, 150);
            this.panelInput.TabIndex = 3;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(20)))), ((int)(((byte)(60)))));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Location = new System.Drawing.Point(730, 110);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(120, 30);
            this.buttonClose.TabIndex = 16;
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // dateTimePickerПрием
            // 
            this.dateTimePickerПрием.Location = new System.Drawing.Point(600, 40);
            this.dateTimePickerПрием.Name = "dateTimePickerПрием";
            this.dateTimePickerПрием.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerПрием.TabIndex = 15;
            // 
            // textBoxТелефон
            // 
            this.textBoxТелефон.Location = new System.Drawing.Point(600, 80);
            this.textBoxТелефон.Name = "textBoxТелефон";
            this.textBoxТелефон.Size = new System.Drawing.Size(200, 22);
            this.textBoxТелефон.TabIndex = 14;
            // 
            // textBoxОклад
            // 
            this.textBoxОклад.Location = new System.Drawing.Point(600, 10);
            this.textBoxОклад.Name = "textBoxОклад";
            this.textBoxОклад.Size = new System.Drawing.Size(200, 22);
            this.textBoxОклад.TabIndex = 13;
            // 
            // textBoxДолжность
            // 
            this.textBoxДолжность.Location = new System.Drawing.Point(150, 110);
            this.textBoxДолжность.Name = "textBoxДолжность";
            this.textBoxДолжность.Size = new System.Drawing.Size(200, 22);
            this.textBoxДолжность.TabIndex = 12;
            // 
            // textBoxОтчество
            // 
            this.textBoxОтчество.Location = new System.Drawing.Point(150, 80);
            this.textBoxОтчество.Name = "textBoxОтчество";
            this.textBoxОтчество.Size = new System.Drawing.Size(200, 22);
            this.textBoxОтчество.TabIndex = 11;
            // 
            // textBoxИмя
            // 
            this.textBoxИмя.Location = new System.Drawing.Point(150, 50);
            this.textBoxИмя.Name = "textBoxИмя";
            this.textBoxИмя.Size = new System.Drawing.Size(200, 22);
            this.textBoxИмя.TabIndex = 10;
            // 
            // textBoxФамилия
            // 
            this.textBoxФамилия.Location = new System.Drawing.Point(150, 20);
            this.textBoxФамилия.Name = "textBoxФамилия";
            this.textBoxФамилия.Size = new System.Drawing.Size(200, 22);
            this.textBoxФамилия.TabIndex = 9;
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(150, 110);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(100, 22);
            this.textBoxID.TabIndex = 8;
            this.textBoxID.Visible = false;
            // 
            // labelДатаПриема
            // 
            this.labelДатаПриема.AutoSize = true;
            this.labelДатаПриема.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelДатаПриема.Location = new System.Drawing.Point(470, 45);
            this.labelДатаПриема.Name = "labelДатаПриема";
            this.labelДатаПриема.Size = new System.Drawing.Size(99, 15);
            this.labelДатаПриема.TabIndex = 7;
            this.labelДатаПриема.Text = "Дата приема:";
            // 
            // labelТелефон
            // 
            this.labelТелефон.AutoSize = true;
            this.labelТелефон.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelТелефон.Location = new System.Drawing.Point(470, 85);
            this.labelТелефон.Name = "labelТелефон";
            this.labelТелефон.Size = new System.Drawing.Size(71, 15);
            this.labelТелефон.TabIndex = 6;
            this.labelТелефон.Text = "Телефон:";
            // 
            // labelОклад
            // 
            this.labelОклад.AutoSize = true;
            this.labelОклад.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelОклад.Location = new System.Drawing.Point(470, 15);
            this.labelОклад.Name = "labelОклад";
            this.labelОклад.Size = new System.Drawing.Size(52, 15);
            this.labelОклад.TabIndex = 5;
            this.labelОклад.Text = "Оклад:";
            // 
            // labelДолжность
            // 
            this.labelДолжность.AutoSize = true;
            this.labelДолжность.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelДолжность.Location = new System.Drawing.Point(20, 115);
            this.labelДолжность.Name = "labelДолжность";
            this.labelДолжность.Size = new System.Drawing.Size(86, 15);
            this.labelДолжность.TabIndex = 4;
            this.labelДолжность.Text = "Должность:";
            // 
            // labelОтчество
            // 
            this.labelОтчество.AutoSize = true;
            this.labelОтчество.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelОтчество.Location = new System.Drawing.Point(20, 85);
            this.labelОтчество.Name = "labelОтчество";
            this.labelОтчество.Size = new System.Drawing.Size(75, 15);
            this.labelОтчество.TabIndex = 3;
            this.labelОтчество.Text = "Отчество:";
            // 
            // labelИмя
            // 
            this.labelИмя.AutoSize = true;
            this.labelИмя.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelИмя.Location = new System.Drawing.Point(20, 55);
            this.labelИмя.Name = "labelИмя";
            this.labelИмя.Size = new System.Drawing.Size(39, 15);
            this.labelИмя.TabIndex = 2;
            this.labelИмя.Text = "Имя:";
            // 
            // labelФамилия
            // 
            this.labelФамилия.AutoSize = true;
            this.labelФамилия.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelФамилия.Location = new System.Drawing.Point(20, 25);
            this.labelФамилия.Name = "labelФамилия";
            this.labelФамилия.Size = new System.Drawing.Size(73, 15);
            this.labelФамилия.TabIndex = 1;
            this.labelФамилия.Text = "Фамилия:";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelID.Location = new System.Drawing.Point(20, 115);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(25, 15);
            this.labelID.TabIndex = 0;
            this.labelID.Text = "ID:";
            this.labelID.Visible = false;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 591);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelFormHeader);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.dataGridViewEmployees);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Сотрудники - КФХ Горбов Н.И.";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmployees)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelFormHeader.ResumeLayout(false);
            this.panelFormHeader.PerformLayout();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}