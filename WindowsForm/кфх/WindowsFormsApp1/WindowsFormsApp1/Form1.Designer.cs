using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelSubtitle;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button buttonEmployees;
        private System.Windows.Forms.Button buttonCrops;
        private System.Windows.Forms.Button buttonPlots;
        private System.Windows.Forms.Button buttonFinance;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.PictureBox pictureBoxFarm;
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.labelSubtitle = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.buttonFinance = new System.Windows.Forms.Button();
            this.buttonPlots = new System.Windows.Forms.Button();
            this.buttonCrops = new System.Windows.Forms.Button();
            this.buttonEmployees = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.pictureBoxFarm = new System.Windows.Forms.PictureBox();
            this.panelHeader.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFarm)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.panelHeader.Controls.Add(this.labelSubtitle);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(884, 100);
            this.panelHeader.TabIndex = 0;
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.AutoSize = true;
            this.labelSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSubtitle.ForeColor = System.Drawing.Color.White;
            this.labelSubtitle.Location = new System.Drawing.Point(280, 55);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(265, 20);
            this.labelSubtitle.TabIndex = 1;
            this.labelSubtitle.Text = "Глава: Горбов Николай Иванович";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(180, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(580, 29);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "КРЕСТЬЯНСКОЕ ФЕРМЕРСКОЕ ХОЗЯЙСТВО";
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.panelMenu.Controls.Add(this.buttonFinance);
            this.panelMenu.Controls.Add(this.buttonPlots);
            this.panelMenu.Controls.Add(this.buttonCrops);
            this.panelMenu.Controls.Add(this.buttonEmployees);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 100);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(250, 483);
            this.panelMenu.TabIndex = 1;
            // 
            // buttonFinance
            // 
            this.buttonFinance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonFinance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFinance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonFinance.ForeColor = System.Drawing.Color.White;
            this.buttonFinance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFinance.Location = new System.Drawing.Point(25, 270);
            this.buttonFinance.Name = "buttonFinance";
            this.buttonFinance.Size = new System.Drawing.Size(200, 60);
            this.buttonFinance.TabIndex = 3;
            this.buttonFinance.Text = "Финансы";
            this.buttonFinance.UseVisualStyleBackColor = false;
            this.buttonFinance.Click += new System.EventHandler(this.buttonFinance_Click);
            // 
            // buttonPlots
            // 
            this.buttonPlots.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonPlots.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlots.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonPlots.ForeColor = System.Drawing.Color.White;
            this.buttonPlots.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPlots.Location = new System.Drawing.Point(25, 190);
            this.buttonPlots.Name = "buttonPlots";
            this.buttonPlots.Size = new System.Drawing.Size(200, 60);
            this.buttonPlots.TabIndex = 2;
            this.buttonPlots.Text = "Участки";
            this.buttonPlots.UseVisualStyleBackColor = false;
            this.buttonPlots.Click += new System.EventHandler(this.buttonPlots_Click);
            // 
            // buttonCrops
            // 
            this.buttonCrops.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonCrops.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCrops.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCrops.ForeColor = System.Drawing.Color.White;
            this.buttonCrops.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCrops.Location = new System.Drawing.Point(25, 110);
            this.buttonCrops.Name = "buttonCrops";
            this.buttonCrops.Size = new System.Drawing.Size(200, 60);
            this.buttonCrops.TabIndex = 1;
            this.buttonCrops.Text = "Культуры";
            this.buttonCrops.UseVisualStyleBackColor = false;
            this.buttonCrops.Click += new System.EventHandler(this.buttonCrops_Click);
            // 
            // buttonEmployees
            // 
            this.buttonEmployees.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.buttonEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEmployees.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonEmployees.ForeColor = System.Drawing.Color.White;
            this.buttonEmployees.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonEmployees.Location = new System.Drawing.Point(25, 30);
            this.buttonEmployees.Name = "buttonEmployees";
            this.buttonEmployees.Size = new System.Drawing.Size(200, 60);
            this.buttonEmployees.TabIndex = 0;
            this.buttonEmployees.Text = "Сотрудники";
            this.buttonEmployees.UseVisualStyleBackColor = false;
            this.buttonEmployees.Click += new System.EventHandler(this.buttonEmployees_Click);
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Controls.Add(this.pictureBoxFarm);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(250, 100);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(634, 483);
            this.panelContent.TabIndex = 2;
            // 
            // pictureBoxFarm
            // 
            this.pictureBoxFarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxFarm.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxFarm.Name = "pictureBoxFarm";
            this.pictureBoxFarm.Size = new System.Drawing.Size(634, 483);
            this.pictureBoxFarm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFarm.TabIndex = 0;
            this.pictureBoxFarm.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 583);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "КФХ Горбов Н.И.";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMenu.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFarm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}

