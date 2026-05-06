using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridViewPlots;
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
        private System.Windows.Forms.ComboBox comboBoxОтветственный;
        private System.Windows.Forms.TextBox textBoxПлощадь;
        private System.Windows.Forms.ComboBox comboBoxКультура;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.DateTimePicker dateTimePickerПосев;
        private System.Windows.Forms.DateTimePicker dateTimePickerСбор;
        private System.Windows.Forms.Label labelНазвание;
        private System.Windows.Forms.Label labelОтветственный;
        private System.Windows.Forms.Label labelПлощадь;
        private System.Windows.Forms.Label labelКультура;
        private System.Windows.Forms.Label labelДатаПосева;
        private System.Windows.Forms.Label labelДатаСбора;
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
            this.dataGridViewPlots = new System.Windows.Forms.DataGridView();
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
            this.dateTimePickerСбор = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerПосев = new System.Windows.Forms.DateTimePicker();
            this.comboBoxКультура = new System.Windows.Forms.ComboBox();
            this.textBoxПлощадь = new System.Windows.Forms.TextBox();
            this.comboBoxОтветственный = new System.Windows.Forms.ComboBox();
            this.textBoxНазвание = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.labelДатаСбора = new System.Windows.Forms.Label();
            this.labelДатаПосева = new System.Windows.Forms.Label();
            this.labelКультура = new System.Windows.Forms.Label();
            this.labelПлощадь = new System.Windows.Forms.Label();
            this.labelОтветственный = new System.Windows.Forms.Label();
            this.labelНазвание = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlots)).BeginInit();
            this.panelControls.SuspendLayout();
            this.panelFormHeader.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewPlots
            // 
            this.dataGridViewPlots.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewPlots.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewPlots.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPlots.Location = new System.Drawing.Point(12, 280);
            this.dataGridViewPlots.Name = "dataGridViewPlots";
            this.dataGridViewPlots.Size = new System.Drawing.Size(860, 300);
            this.dataGridViewPlots.TabIndex = 0;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.panelControls.Controls.Add(this.button2);
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
            this.labelSearch.Size = new System.Drawing.Size(120, 15);
            this.labelSearch.TabIndex = 0;
            this.labelSearch.Text = "Поиск по участку";
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
            this.labelFormTitle.Size = new System.Drawing.Size(207, 24);
            this.labelFormTitle.TabIndex = 0;
            this.labelFormTitle.Text = "Управление полями";
            // 
            // panelInput
            // 
            this.panelInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelInput.Controls.Add(this.button1);
            this.panelInput.Controls.Add(this.buttonClose);
            this.panelInput.Controls.Add(this.dateTimePickerСбор);
            this.panelInput.Controls.Add(this.dateTimePickerПосев);
            this.panelInput.Controls.Add(this.comboBoxКультура);
            this.panelInput.Controls.Add(this.textBoxПлощадь);
            this.panelInput.Controls.Add(this.comboBoxОтветственный);
            this.panelInput.Controls.Add(this.textBoxНазвание);
            this.panelInput.Controls.Add(this.textBoxID);
            this.panelInput.Controls.Add(this.labelДатаСбора);
            this.panelInput.Controls.Add(this.labelДатаПосева);
            this.panelInput.Controls.Add(this.labelКультура);
            this.panelInput.Controls.Add(this.labelПлощадь);
            this.panelInput.Controls.Add(this.labelОтветственный);
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
            this.buttonClose.TabIndex = 14;
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.UseVisualStyleBackColor = false;
            // 
            // dateTimePickerСбор
            // 
            this.dateTimePickerСбор.Location = new System.Drawing.Point(600, 80);
            this.dateTimePickerСбор.Name = "dateTimePickerСбор";
            this.dateTimePickerСбор.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerСбор.TabIndex = 13;
            // 
            // dateTimePickerПосев
            // 
            this.dateTimePickerПосев.Location = new System.Drawing.Point(600, 40);
            this.dateTimePickerПосев.Name = "dateTimePickerПосев";
            this.dateTimePickerПосев.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerПосев.TabIndex = 12;
            // 
            // comboBoxКультура
            // 
            this.comboBoxКультура.FormattingEnabled = true;
            this.comboBoxКультура.Location = new System.Drawing.Point(600, 10);
            this.comboBoxКультура.Name = "comboBoxКультура";
            this.comboBoxКультура.Size = new System.Drawing.Size(200, 24);
            this.comboBoxКультура.TabIndex = 11;
            // 
            // textBoxПлощадь
            // 
            this.textBoxПлощадь.Location = new System.Drawing.Point(150, 80);
            this.textBoxПлощадь.Name = "textBoxПлощадь";
            this.textBoxПлощадь.Size = new System.Drawing.Size(200, 22);
            this.textBoxПлощадь.TabIndex = 10;
            // 
            // comboBoxОтветственный
            // 
            this.comboBoxОтветственный.FormattingEnabled = true;
            this.comboBoxОтветственный.Location = new System.Drawing.Point(150, 50);
            this.comboBoxОтветственный.Name = "comboBoxОтветственный";
            this.comboBoxОтветственный.Size = new System.Drawing.Size(200, 24);
            this.comboBoxОтветственный.TabIndex = 9;
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
            // labelДатаСбора
            // 
            this.labelДатаСбора.AutoSize = true;
            this.labelДатаСбора.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelДатаСбора.Location = new System.Drawing.Point(470, 85);
            this.labelДатаСбора.Name = "labelДатаСбора";
            this.labelДатаСбора.Size = new System.Drawing.Size(88, 15);
            this.labelДатаСбора.TabIndex = 6;
            this.labelДатаСбора.Text = "Дата сбора:";
            // 
            // labelДатаПосева
            // 
            this.labelДатаПосева.AutoSize = true;
            this.labelДатаПосева.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelДатаПосева.Location = new System.Drawing.Point(470, 45);
            this.labelДатаПосева.Name = "labelДатаПосева";
            this.labelДатаПосева.Size = new System.Drawing.Size(96, 15);
            this.labelДатаПосева.TabIndex = 5;
            this.labelДатаПосева.Text = "Дата посева:";
            // 
            // labelКультура
            // 
            this.labelКультура.AutoSize = true;
            this.labelКультура.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelКультура.Location = new System.Drawing.Point(470, 15);
            this.labelКультура.Name = "labelКультура";
            this.labelКультура.Size = new System.Drawing.Size(72, 15);
            this.labelКультура.TabIndex = 4;
            this.labelКультура.Text = "Культура:";
            // 
            // labelПлощадь
            // 
            this.labelПлощадь.AutoSize = true;
            this.labelПлощадь.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelПлощадь.Location = new System.Drawing.Point(20, 85);
            this.labelПлощадь.Name = "labelПлощадь";
            this.labelПлощадь.Size = new System.Drawing.Size(71, 15);
            this.labelПлощадь.TabIndex = 3;
            this.labelПлощадь.Text = "Площадь:";
            // 
            // labelОтветственный
            // 
            this.labelОтветственный.AutoSize = true;
            this.labelОтветственный.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelОтветственный.Location = new System.Drawing.Point(20, 55);
            this.labelОтветственный.Name = "labelОтветственный";
            this.labelОтветственный.Size = new System.Drawing.Size(118, 15);
            this.labelОтветственный.TabIndex = 2;
            this.labelОтветственный.Text = "Ответственный:";
            // 
            // labelНазвание
            // 
            this.labelНазвание.AutoSize = true;
            this.labelНазвание.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelНазвание.Location = new System.Drawing.Point(20, 25);
            this.labelНазвание.Name = "labelНазвание";
            this.labelНазвание.Size = new System.Drawing.Size(76, 15);
            this.labelНазвание.TabIndex = 1;
            this.labelНазвание.Text = "Название:";
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
            this.button2.TabIndex = 18;
            this.button2.Text = "Обновить";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            this.button1.TabIndex = 19;
            this.button1.Text = "Закрыть";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 591);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelFormHeader);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.dataGridViewPlots);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Участки - КФХ Горбов Н.И.";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlots)).EndInit();
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