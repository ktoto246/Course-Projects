using System.Resources;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridViewCrops;
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
        private System.Windows.Forms.TextBox textBoxНазвание;
        private System.Windows.Forms.ComboBox comboBoxТип;
        private System.Windows.Forms.TextBox textBoxСрок;
        private System.Windows.Forms.TextBox textBoxСтоимость;
        private System.Windows.Forms.TextBox textBoxУрожайность;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label labelНазвание;
        private System.Windows.Forms.Label labelТип;
        private System.Windows.Forms.Label labelСрок;
        private System.Windows.Forms.Label labelСтоимость;
        private System.Windows.Forms.Label labelУрожайность;
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
            this.dataGridViewCrops = new System.Windows.Forms.DataGridView();
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
            this.textBoxУрожайность = new System.Windows.Forms.TextBox();
            this.textBoxСтоимость = new System.Windows.Forms.TextBox();
            this.textBoxСрок = new System.Windows.Forms.TextBox();
            this.comboBoxТип = new System.Windows.Forms.ComboBox();
            this.textBoxНазвание = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.labelУрожайность = new System.Windows.Forms.Label();
            this.labelСтоимость = new System.Windows.Forms.Label();
            this.labelСрок = new System.Windows.Forms.Label();
            this.labelТип = new System.Windows.Forms.Label();
            this.labelНазвание = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCrops)).BeginInit();
            this.panelControls.SuspendLayout();
            this.panelFormHeader.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewCrops
            // 
            this.dataGridViewCrops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCrops.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewCrops.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCrops.Location = new System.Drawing.Point(12, 280);
            this.dataGridViewCrops.Name = "dataGridViewCrops";
            this.dataGridViewCrops.Size = new System.Drawing.Size(860, 300);
            this.dataGridViewCrops.TabIndex = 0;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.panelControls.Controls.Add(this.buttonRefresh);
            this.panelControls.Controls.Add(this.buttonSearch);
            this.panelControls.Controls.Add(this.button2);
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
            this.buttonRefresh.Location = new System.Drawing.Point(1390, 10);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 30);
            this.buttonRefresh.TabIndex = 5;
            this.buttonRefresh.Text = "Обновить";
            this.buttonRefresh.UseVisualStyleBackColor = false;
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
            this.labelSearch.Size = new System.Drawing.Size(130, 15);
            this.labelSearch.TabIndex = 0;
            this.labelSearch.Text = "Поиск по культуре";
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
            this.labelFormTitle.Size = new System.Drawing.Size(189, 24);
            this.labelFormTitle.TabIndex = 0;
            this.labelFormTitle.Text = "Сельхоз культуры";
            // 
            // panelInput
            // 
            this.panelInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelInput.Controls.Add(this.button1);
            this.panelInput.Controls.Add(this.buttonClose);
            this.panelInput.Controls.Add(this.textBoxУрожайность);
            this.panelInput.Controls.Add(this.textBoxСтоимость);
            this.panelInput.Controls.Add(this.textBoxСрок);
            this.panelInput.Controls.Add(this.comboBoxТип);
            this.panelInput.Controls.Add(this.textBoxНазвание);
            this.panelInput.Controls.Add(this.textBoxID);
            this.panelInput.Controls.Add(this.labelУрожайность);
            this.panelInput.Controls.Add(this.labelСтоимость);
            this.panelInput.Controls.Add(this.labelСрок);
            this.panelInput.Controls.Add(this.labelТип);
            this.panelInput.Controls.Add(this.labelНазвание);
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
            this.buttonClose.Location = new System.Drawing.Point(1390, 110);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(120, 30);
            this.buttonClose.TabIndex = 13;
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.UseVisualStyleBackColor = false;
            // 
            // textBoxУрожайность
            // 
            this.textBoxУрожайность.Location = new System.Drawing.Point(600, 80);
            this.textBoxУрожайность.Name = "textBoxУрожайность";
            this.textBoxУрожайность.Size = new System.Drawing.Size(200, 22);
            this.textBoxУрожайность.TabIndex = 12;
            // 
            // textBoxСтоимость
            // 
            this.textBoxСтоимость.Location = new System.Drawing.Point(600, 40);
            this.textBoxСтоимость.Name = "textBoxСтоимость";
            this.textBoxСтоимость.Size = new System.Drawing.Size(200, 22);
            this.textBoxСтоимость.TabIndex = 11;
            // 
            // textBoxСрок
            // 
            this.textBoxСрок.Location = new System.Drawing.Point(600, 10);
            this.textBoxСрок.Name = "textBoxСрок";
            this.textBoxСрок.Size = new System.Drawing.Size(200, 22);
            this.textBoxСрок.TabIndex = 10;
            // 
            // comboBoxТип
            // 
            this.comboBoxТип.FormattingEnabled = true;
            this.comboBoxТип.Items.AddRange(new object[] {
            "зерновые",
            "овощи",
            "фрукты",
            "технические"});
            this.comboBoxТип.Location = new System.Drawing.Point(150, 50);
            this.comboBoxТип.Name = "comboBoxТип";
            this.comboBoxТип.Size = new System.Drawing.Size(200, 24);
            this.comboBoxТип.TabIndex = 9;
            // 
            // textBoxНазвание
            // 
            this.textBoxНазвание.Location = new System.Drawing.Point(150, 20);
            this.textBoxНазвание.Name = "textBoxНазвание";
            this.textBoxНазвание.Size = new System.Drawing.Size(200, 22);
            this.textBoxНазвание.TabIndex = 8;
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(150, 110);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(100, 22);
            this.textBoxID.TabIndex = 7;
            this.textBoxID.Visible = false;
            // 
            // labelУрожайность
            // 
            this.labelУрожайность.AutoSize = true;
            this.labelУрожайность.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelУрожайность.Location = new System.Drawing.Point(470, 85);
            this.labelУрожайность.Name = "labelУрожайность";
            this.labelУрожайность.Size = new System.Drawing.Size(101, 15);
            this.labelУрожайность.TabIndex = 6;
            this.labelУрожайность.Text = "Урожайность:";
            // 
            // labelСтоимость
            // 
            this.labelСтоимость.AutoSize = true;
            this.labelСтоимость.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelСтоимость.Location = new System.Drawing.Point(470, 45);
            this.labelСтоимость.Name = "labelСтоимость";
            this.labelСтоимость.Size = new System.Drawing.Size(85, 15);
            this.labelСтоимость.TabIndex = 5;
            this.labelСтоимость.Text = "Стоимость:";
            // 
            // labelСрок
            // 
            this.labelСрок.AutoSize = true;
            this.labelСрок.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelСрок.Location = new System.Drawing.Point(470, 15);
            this.labelСрок.Name = "labelСрок";
            this.labelСрок.Size = new System.Drawing.Size(125, 15);
            this.labelСрок.TabIndex = 4;
            this.labelСрок.Text = "Срок созревания:";
            // 
            // labelТип
            // 
            this.labelТип.AutoSize = true;
            this.labelТип.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelТип.Location = new System.Drawing.Point(20, 55);
            this.labelТип.Name = "labelТип";
            this.labelТип.Size = new System.Drawing.Size(35, 15);
            this.labelТип.TabIndex = 3;
            this.labelТип.Text = "Тип:";
            // 
            // labelНазвание
            // 
            this.labelНазвание.AutoSize = true;
            this.labelНазвание.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelНазвание.Location = new System.Drawing.Point(20, 25);
            this.labelНазвание.Name = "labelНазвание";
            this.labelНазвание.Size = new System.Drawing.Size(76, 15);
            this.labelНазвание.TabIndex = 2;
            this.labelНазвание.Text = "Название:";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelID.Location = new System.Drawing.Point(20, 115);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(25, 15);
            this.labelID.TabIndex = 1;
            this.labelID.Text = "ID:";
            this.labelID.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(20)))), ((int)(((byte)(60)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(737, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 30);
            this.button1.TabIndex = 18;
            this.button1.Text = "Закрыть";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(737, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 30);
            this.button2.TabIndex = 17;
            this.button2.Text = "Обновить";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 591);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelFormHeader);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.dataGridViewCrops);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Культуры - КФХ Горбов Н.И.";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCrops)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelFormHeader.ResumeLayout(false);
            this.panelFormHeader.PerformLayout();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}