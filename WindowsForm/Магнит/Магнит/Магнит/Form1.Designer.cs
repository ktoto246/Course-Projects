using Магнит.Properties;

namespace Магнит
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel navigationPanel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Button btnEmployees;
        private System.Windows.Forms.Button btnPositions;
        private System.Windows.Forms.Button btnDepartments;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.PictureBox companyImage;
        private System.Windows.Forms.Label welcomeLabel;
        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.navigationPanel = new System.Windows.Forms.Panel();
            this.btnDepartments = new System.Windows.Forms.Button();
            this.btnPositions = new System.Windows.Forms.Button();
            this.btnEmployees = new System.Windows.Forms.Button();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.companyImage = new System.Windows.Forms.PictureBox();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.navigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.companyImage)).BeginInit();
            this.SuspendLayout();
            // 
            // navigationPanel
            // 
            this.navigationPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.navigationPanel.Controls.Add(this.btnDepartments);
            this.navigationPanel.Controls.Add(this.btnPositions);
            this.navigationPanel.Controls.Add(this.btnEmployees);
            this.navigationPanel.Controls.Add(this.logoPictureBox);
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigationPanel.Location = new System.Drawing.Point(0, 0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(214, 573);
            this.navigationPanel.TabIndex = 0;
            // 
            // btnDepartments
            // 
            this.btnDepartments.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnDepartments.FlatAppearance.BorderSize = 0;
            this.btnDepartments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDepartments.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDepartments.ForeColor = System.Drawing.Color.White;
            this.btnDepartments.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDepartments.Location = new System.Drawing.Point(0, 243);
            this.btnDepartments.Name = "btnDepartments";
            this.btnDepartments.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnDepartments.Size = new System.Drawing.Size(214, 52);
            this.btnDepartments.TabIndex = 3;
            this.btnDepartments.Text = "   Отделы";
            this.btnDepartments.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDepartments.UseVisualStyleBackColor = false;
            this.btnDepartments.Click += new System.EventHandler(this.btnDepartments_Click);
            // 
            // btnPositions
            // 
            this.btnPositions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnPositions.FlatAppearance.BorderSize = 0;
            this.btnPositions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPositions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnPositions.ForeColor = System.Drawing.Color.White;
            this.btnPositions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPositions.Location = new System.Drawing.Point(0, 191);
            this.btnPositions.Name = "btnPositions";
            this.btnPositions.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnPositions.Size = new System.Drawing.Size(214, 52);
            this.btnPositions.TabIndex = 2;
            this.btnPositions.Text = "   Должности";
            this.btnPositions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPositions.UseVisualStyleBackColor = false;
            this.btnPositions.Click += new System.EventHandler(this.btnPositions_Click);
            // 
            // btnEmployees
            // 
            this.btnEmployees.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnEmployees.FlatAppearance.BorderSize = 0;
            this.btnEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmployees.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEmployees.ForeColor = System.Drawing.Color.White;
            this.btnEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployees.Location = new System.Drawing.Point(0, 139);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnEmployees.Size = new System.Drawing.Size(214, 52);
            this.btnEmployees.TabIndex = 1;
            this.btnEmployees.Text = "   Сотрудники";
            this.btnEmployees.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEmployees.UseVisualStyleBackColor = false;
            this.btnEmployees.Click += new System.EventHandler(this.btnEmployees_Click);
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.BackColor = System.Drawing.Color.White;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(0, 0);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(214, 104);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 0;
            this.logoPictureBox.TabStop = false;
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.Controls.Add(this.companyImage);
            this.contentPanel.Controls.Add(this.welcomeLabel);
            this.contentPanel.Controls.Add(this.titleLabel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(214, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(629, 573);
            this.contentPanel.TabIndex = 1;
            // 
            // companyImage
            // 
            this.companyImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.companyImage.Image = ((System.Drawing.Image)(resources.GetObject("companyImage.Image")));
            this.companyImage.Location = new System.Drawing.Point(6, 124);
            this.companyImage.Name = "companyImage";
            this.companyImage.Size = new System.Drawing.Size(611, 437);
            this.companyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.companyImage.TabIndex = 2;
            this.companyImage.TabStop = false;
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.welcomeLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.welcomeLabel.Location = new System.Drawing.Point(45, 78);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(543, 43);
            this.welcomeLabel.TabIndex = 1;
            this.welcomeLabel.Text = "Добро пожаловать в систему управления персоналом сети магазинов \"Магнит\"";
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.titleLabel.Location = new System.Drawing.Point(45, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(543, 69);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "МАГНИТ";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 573);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.navigationPanel);
            this.MinimumSize = new System.Drawing.Size(859, 612);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Магнит - Система управления персоналом";
            this.navigationPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.companyImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}

