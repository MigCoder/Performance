namespace RingClip
{
    partial class OpenClSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.label1 = new System.Windows.Forms.Label();
            this.platformComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.computeUnitLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.vendorNameLabel = new System.Windows.Forms.Label();
            this.exitButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.messageLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.deviceTypeLabel = new System.Windows.Forms.Label();
            this.size_Small = new System.Windows.Forms.RadioButton();
            this.size_Middle = new System.Windows.Forms.RadioButton();
            this.size_Big = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_FillW = new System.Windows.Forms.TextBox();
            this.txt_FillH = new System.Windows.Forms.TextBox();
            this.txt_Count = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ck_useGdi = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Platform:";
            // 
            // platformComboBox
            // 
            this.platformComboBox.DisplayMember = "Name";
            this.platformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platformComboBox.FormattingEnabled = true;
            this.platformComboBox.Location = new System.Drawing.Point(13, 28);
            this.platformComboBox.Name = "platformComboBox";
            this.platformComboBox.Size = new System.Drawing.Size(218, 20);
            this.platformComboBox.TabIndex = 0;
            this.platformComboBox.ValueMember = "Name";
            this.platformComboBox.SelectedIndexChanged += new System.EventHandler(this.platformComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Device:";
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DisplayMember = "Name";
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(13, 69);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(218, 20);
            this.deviceComboBox.TabIndex = 1;
            this.deviceComboBox.ValueMember = "Name";
            this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(237, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "Compute Unit:";
            // 
            // computeUnitLabel
            // 
            this.computeUnitLabel.AutoSize = true;
            this.computeUnitLabel.Location = new System.Drawing.Point(317, 70);
            this.computeUnitLabel.Name = "computeUnitLabel";
            this.computeUnitLabel.Size = new System.Drawing.Size(59, 12);
            this.computeUnitLabel.TabIndex = 6;
            this.computeUnitLabel.Text = "(unknown)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(237, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "Vendor:";
            // 
            // vendorNameLabel
            // 
            this.vendorNameLabel.Location = new System.Drawing.Point(287, 30);
            this.vendorNameLabel.Name = "vendorNameLabel";
            this.vendorNameLabel.Size = new System.Drawing.Size(120, 36);
            this.vendorNameLabel.TabIndex = 8;
            this.vendorNameLabel.Text = "(unknown)";
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(332, 570);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 21);
            this.exitButton.TabIndex = 9;
            this.exitButton.Text = "E&xit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(251, 570);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // messageLabel
            // 
            this.messageLabel.ForeColor = System.Drawing.Color.Red;
            this.messageLabel.Location = new System.Drawing.Point(10, 394);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(397, 36);
            this.messageLabel.TabIndex = 12;
            this.messageLabel.Text = "Ready";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(237, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "Device Type:";
            // 
            // deviceTypeLabel
            // 
            this.deviceTypeLabel.AutoSize = true;
            this.deviceTypeLabel.Location = new System.Drawing.Point(314, 96);
            this.deviceTypeLabel.Name = "deviceTypeLabel";
            this.deviceTypeLabel.Size = new System.Drawing.Size(59, 12);
            this.deviceTypeLabel.TabIndex = 14;
            this.deviceTypeLabel.Text = "(unknown)";
            // 
            // size_Small
            // 
            this.size_Small.AutoSize = true;
            this.size_Small.Location = new System.Drawing.Point(13, 159);
            this.size_Small.Name = "size_Small";
            this.size_Small.Size = new System.Drawing.Size(53, 16);
            this.size_Small.TabIndex = 2;
            this.size_Small.Tag = "600";
            this.size_Small.Text = "Small";
            this.size_Small.UseVisualStyleBackColor = true;
            // 
            // size_Middle
            // 
            this.size_Middle.AutoSize = true;
            this.size_Middle.Location = new System.Drawing.Point(13, 193);
            this.size_Middle.Name = "size_Middle";
            this.size_Middle.Size = new System.Drawing.Size(59, 16);
            this.size_Middle.TabIndex = 3;
            this.size_Middle.Text = "Middle";
            this.size_Middle.UseVisualStyleBackColor = true;
            // 
            // size_Big
            // 
            this.size_Big.AutoSize = true;
            this.size_Big.Checked = true;
            this.size_Big.Location = new System.Drawing.Point(13, 228);
            this.size_Big.Name = "size_Big";
            this.size_Big.Size = new System.Drawing.Size(41, 16);
            this.size_Big.TabIndex = 4;
            this.size_Big.TabStop = true;
            this.size_Big.Text = "Big";
            this.size_Big.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(12, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "WindowSize(will affection performance)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(12, 266);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "Config:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(12, 297);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "Width:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(9, 328);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 21;
            this.label9.Text = "Height:";
            // 
            // txt_FillW
            // 
            this.txt_FillW.Location = new System.Drawing.Point(57, 295);
            this.txt_FillW.Name = "txt_FillW";
            this.txt_FillW.Size = new System.Drawing.Size(126, 21);
            this.txt_FillW.TabIndex = 5;
            this.txt_FillW.Text = "300";
            // 
            // txt_FillH
            // 
            this.txt_FillH.Location = new System.Drawing.Point(57, 326);
            this.txt_FillH.Name = "txt_FillH";
            this.txt_FillH.Size = new System.Drawing.Size(126, 21);
            this.txt_FillH.TabIndex = 6;
            this.txt_FillH.Text = "300";
            // 
            // txt_Count
            // 
            this.txt_Count.Location = new System.Drawing.Point(57, 360);
            this.txt_Count.Name = "txt_Count";
            this.txt_Count.Size = new System.Drawing.Size(126, 21);
            this.txt_Count.TabIndex = 7;
            this.txt_Count.Text = "1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(10, 363);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "Count:";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.Green;
            this.label11.Location = new System.Drawing.Point(9, 441);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(397, 99);
            this.label11.TabIndex = 25;
            this.label11.Text = "You can open this window by input hot key Esc.\r\n\r\nAnd brush draw performance will" +
    " show at title bar.\r\n\r\nIf you want more performance may you should try relase pr" +
    "ogram version.";
            // 
            // ck_useGdi
            // 
            this.ck_useGdi.AutoSize = true;
            this.ck_useGdi.Location = new System.Drawing.Point(251, 159);
            this.ck_useGdi.Name = "ck_useGdi";
            this.ck_useGdi.Size = new System.Drawing.Size(60, 16);
            this.ck_useGdi.TabIndex = 26;
            this.ck_useGdi.Text = "UseGdi";
            this.ck_useGdi.UseVisualStyleBackColor = true;
            // 
            // OpenClSetting
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(423, 602);
            this.Controls.Add(this.ck_useGdi);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txt_Count);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txt_FillH);
            this.Controls.Add(this.txt_FillW);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.size_Big);
            this.Controls.Add(this.size_Middle);
            this.Controls.Add(this.size_Small);
            this.Controls.Add(this.deviceTypeLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.vendorNameLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.computeUnitLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.deviceComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.platformComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenClSetting";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OpenCL Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpenClSetting_FormClosed);
            this.Load += new System.EventHandler(this.OpenClSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox platformComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label computeUnitLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label vendorNameLabel;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label deviceTypeLabel;
        private System.Windows.Forms.RadioButton size_Small;
        private System.Windows.Forms.RadioButton size_Middle;
        private System.Windows.Forms.RadioButton size_Big;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_FillW;
        private System.Windows.Forms.TextBox txt_FillH;
        private System.Windows.Forms.TextBox txt_Count;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox ck_useGdi;
    }
}