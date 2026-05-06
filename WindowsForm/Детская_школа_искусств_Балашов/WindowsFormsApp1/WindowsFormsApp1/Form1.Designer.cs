using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button buttonEmployees;
        private System.Windows.Forms.Button buttonDepartments;
        private System.Windows.Forms.Button buttonStudents;
        private System.Windows.Forms.Button buttonSchedule;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label labelSchoolName;
        private System.Windows.Forms.Panel panelHeader;
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
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonEmployees = new System.Windows.Forms.Button();
            this.buttonDepartments = new System.Windows.Forms.Button();
            this.buttonStudents = new System.Windows.Forms.Button();
            this.buttonSchedule = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.labelSchoolName = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(686, 173);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelTitle.Location = new System.Drawing.Point(214, 17);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(272, 24);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "Управление базой данных";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonEmployees
            // 
            this.buttonEmployees.BackColor = System.Drawing.Color.SteelBlue;
            this.buttonEmployees.FlatAppearance.BorderSize = 0;
            this.buttonEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEmployees.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonEmployees.ForeColor = System.Drawing.Color.White;
            this.buttonEmployees.Location = new System.Drawing.Point(43, 69);
            this.buttonEmployees.Name = "buttonEmployees";
            this.buttonEmployees.Size = new System.Drawing.Size(257, 43);
            this.buttonEmployees.TabIndex = 2;
            this.buttonEmployees.Text = "Сотрудники";
            this.buttonEmployees.UseVisualStyleBackColor = false;
            this.buttonEmployees.Click += new System.EventHandler(this.buttonEmployees_Click);
            // 
            // buttonDepartments
            // 
            this.buttonDepartments.BackColor = System.Drawing.Color.SteelBlue;
            this.buttonDepartments.FlatAppearance.BorderSize = 0;
            this.buttonDepartments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDepartments.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonDepartments.ForeColor = System.Drawing.Color.White;
            this.buttonDepartments.Location = new System.Drawing.Point(386, 69);
            this.buttonDepartments.Name = "buttonDepartments";
            this.buttonDepartments.Size = new System.Drawing.Size(257, 43);
            this.buttonDepartments.TabIndex = 3;
            this.buttonDepartments.Text = "Отделения";
            this.buttonDepartments.UseVisualStyleBackColor = false;
            this.buttonDepartments.Click += new System.EventHandler(this.buttonDepartments_Click);
            // 
            // buttonStudents
            // 
            this.buttonStudents.BackColor = System.Drawing.Color.SteelBlue;
            this.buttonStudents.FlatAppearance.BorderSize = 0;
            this.buttonStudents.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStudents.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonStudents.ForeColor = System.Drawing.Color.White;
            this.buttonStudents.Location = new System.Drawing.Point(43, 139);
            this.buttonStudents.Name = "buttonStudents";
            this.buttonStudents.Size = new System.Drawing.Size(257, 43);
            this.buttonStudents.TabIndex = 4;
            this.buttonStudents.Text = "Учащиеся";
            this.buttonStudents.UseVisualStyleBackColor = false;
            this.buttonStudents.Click += new System.EventHandler(this.buttonStudents_Click);
            // 
            // buttonSchedule
            // 
            this.buttonSchedule.BackColor = System.Drawing.Color.SteelBlue;
            this.buttonSchedule.FlatAppearance.BorderSize = 0;
            this.buttonSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonSchedule.ForeColor = System.Drawing.Color.White;
            this.buttonSchedule.Location = new System.Drawing.Point(386, 139);
            this.buttonSchedule.Name = "buttonSchedule";
            this.buttonSchedule.Size = new System.Drawing.Size(257, 43);
            this.buttonSchedule.TabIndex = 5;
            this.buttonSchedule.Text = "Расписание";
            this.buttonSchedule.UseVisualStyleBackColor = false;
            this.buttonSchedule.Click += new System.EventHandler(this.buttonSchedule_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonEmployees);
            this.panelButtons.Controls.Add(this.buttonSchedule);
            this.panelButtons.Controls.Add(this.buttonDepartments);
            this.panelButtons.Controls.Add(this.buttonStudents);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(0, 216);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(686, 217);
            this.panelButtons.TabIndex = 6;
            // 
            // labelSchoolName
            // 
            this.labelSchoolName.AutoSize = true;
            this.labelSchoolName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.labelSchoolName.ForeColor = System.Drawing.Color.DarkRed;
            this.labelSchoolName.Location = new System.Drawing.Point(129, 52);
            this.labelSchoolName.Name = "labelSchoolName";
            this.labelSchoolName.Size = new System.Drawing.Size(472, 26);
            this.labelSchoolName.TabIndex = 7;
            this.labelSchoolName.Text = "Детская школа искусств №1 г. Балашова";
            this.labelSchoolName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.labelSchoolName);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 173);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(686, 43);
            this.panelHeader.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(686, 433);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.pictureBoxLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Детская школа искусств №1 - Главная";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}

