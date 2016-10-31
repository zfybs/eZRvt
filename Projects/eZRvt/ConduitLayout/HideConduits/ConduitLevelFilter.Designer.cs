namespace eZRvt. ConduitLayout
{
    partial class ConduitLevelFilter
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
            this.components = new System.ComponentModel.Container();
            this.checkBoxcheckAllConnector = new System.Windows.Forms.CheckBox();
            this.textBoxTopElevation = new System.Windows.Forms.TextBox();
            this.textBoxBottomElevation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBoxTopOffset = new System.Windows.Forms.TextBox();
            this.textBoxBottomOffset = new System.Windows.Forms.TextBox();
            this.comboBoxTopLevel = new System.Windows.Forms.ComboBox();
            this.comboBoxBottomLevel = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonShow = new System.Windows.Forms.RadioButton();
            this.radioButtonHide = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonChooseTopLevel = new System.Windows.Forms.Button();
            this.buttonChoosebottomLevel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxcheckAllConnector
            // 
            this.checkBoxcheckAllConnector.AutoSize = true;
            this.checkBoxcheckAllConnector.Location = new System.Drawing.Point(179, 99);
            this.checkBoxcheckAllConnector.Name = "checkBoxcheckAllConnector";
            this.checkBoxcheckAllConnector.Size = new System.Drawing.Size(72, 16);
            this.checkBoxcheckAllConnector.TabIndex = 0;
            this.checkBoxcheckAllConnector.Text = "检测全部";
            this.toolTip1.SetToolTip(this.checkBoxcheckAllConnector, "如果勾选，表示当线管中所有的连接件都位于指定的标高范围内时，则将其显示或隐藏；如果不勾选，表示线管中只要有一个连接件位于标高范围内，则将其显示或隐藏。");
            this.checkBoxcheckAllConnector.UseVisualStyleBackColor = true;
            // 
            // textBoxTopElevation
            // 
            this.textBoxTopElevation.Location = new System.Drawing.Point(257, 28);
            this.textBoxTopElevation.Name = "textBoxTopElevation";
            this.textBoxTopElevation.Size = new System.Drawing.Size(67, 21);
            this.textBoxTopElevation.TabIndex = 1;
            // 
            // textBoxBottomElevation
            // 
            this.textBoxBottomElevation.Location = new System.Drawing.Point(257, 56);
            this.textBoxBottomElevation.Name = "textBoxBottomElevation";
            this.textBoxBottomElevation.Size = new System.Drawing.Size(67, 21);
            this.textBoxBottomElevation.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "顶部视图标高 (m)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "底部视图标高 (m)";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(273, 149);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "确定";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(192, 149);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxTopOffset
            // 
            this.textBoxTopOffset.Location = new System.Drawing.Point(197, 28);
            this.textBoxTopOffset.Name = "textBoxTopOffset";
            this.textBoxTopOffset.Size = new System.Drawing.Size(54, 21);
            this.textBoxTopOffset.TabIndex = 1;
            // 
            // textBoxBottomOffset
            // 
            this.textBoxBottomOffset.Location = new System.Drawing.Point(197, 56);
            this.textBoxBottomOffset.Name = "textBoxBottomOffset";
            this.textBoxBottomOffset.Size = new System.Drawing.Size(54, 21);
            this.textBoxBottomOffset.TabIndex = 1;
            // 
            // comboBoxTopLevel
            // 
            this.comboBoxTopLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTopLevel.FormattingEnabled = true;
            this.comboBoxTopLevel.Location = new System.Drawing.Point(120, 28);
            this.comboBoxTopLevel.Name = "comboBoxTopLevel";
            this.comboBoxTopLevel.Size = new System.Drawing.Size(71, 20);
            this.comboBoxTopLevel.TabIndex = 5;
            this.comboBoxTopLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxTopLevel_SelectedIndexChanged);
            // 
            // comboBoxBottomLevel
            // 
            this.comboBoxBottomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBottomLevel.FormattingEnabled = true;
            this.comboBoxBottomLevel.Location = new System.Drawing.Point(120, 56);
            this.comboBoxBottomLevel.Name = "comboBoxBottomLevel";
            this.comboBoxBottomLevel.Size = new System.Drawing.Size(71, 20);
            this.comboBoxBottomLevel.TabIndex = 5;
            this.comboBoxBottomLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxBottomLevel_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "标高对象";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(204, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "偏移(m)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(266, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "标高 (m)";
            // 
            // radioButtonShow
            // 
            this.radioButtonShow.AutoSize = true;
            this.radioButtonShow.Checked = true;
            this.radioButtonShow.Location = new System.Drawing.Point(19, 20);
            this.radioButtonShow.Name = "radioButtonShow";
            this.radioButtonShow.Size = new System.Drawing.Size(47, 16);
            this.radioButtonShow.TabIndex = 6;
            this.radioButtonShow.TabStop = true;
            this.radioButtonShow.Text = "显示";
            this.radioButtonShow.UseVisualStyleBackColor = true;
            // 
            // radioButtonHide
            // 
            this.radioButtonHide.AutoSize = true;
            this.radioButtonHide.Location = new System.Drawing.Point(19, 42);
            this.radioButtonHide.Name = "radioButtonHide";
            this.radioButtonHide.Size = new System.Drawing.Size(47, 16);
            this.radioButtonHide.TabIndex = 6;
            this.radioButtonHide.TabStop = true;
            this.radioButtonHide.Text = "隐藏";
            this.radioButtonHide.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonShow);
            this.groupBox1.Controls.Add(this.radioButtonHide);
            this.groupBox1.Location = new System.Drawing.Point(14, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 71);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "区间内线管操作";
            // 
            // buttonChooseTopLevel
            // 
            this.buttonChooseTopLevel.Font = new System.Drawing.Font("Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonChooseTopLevel.Location = new System.Drawing.Point(327, 28);
            this.buttonChooseTopLevel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.buttonChooseTopLevel.Name = "buttonChooseTopLevel";
            this.buttonChooseTopLevel.Size = new System.Drawing.Size(25, 23);
            this.buttonChooseTopLevel.TabIndex = 8;
            this.buttonChooseTopLevel.Text = "...";
            this.buttonChooseTopLevel.UseVisualStyleBackColor = true;
            this.buttonChooseTopLevel.Click += new System.EventHandler(this.buttonChooseTopLevel_Click);
            // 
            // buttonChoosebottomLevel
            // 
            this.buttonChoosebottomLevel.Font = new System.Drawing.Font("Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.buttonChoosebottomLevel.Location = new System.Drawing.Point(327, 54);
            this.buttonChoosebottomLevel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.buttonChoosebottomLevel.Name = "buttonChoosebottomLevel";
            this.buttonChoosebottomLevel.Size = new System.Drawing.Size(25, 23);
            this.buttonChoosebottomLevel.TabIndex = 8;
            this.buttonChoosebottomLevel.Text = "...";
            this.buttonChoosebottomLevel.UseVisualStyleBackColor = true;
            this.buttonChoosebottomLevel.Click += new System.EventHandler(this.buttonChoosebottomLevel_Click);
            // 
            // ConduitLevelFilter
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 181);
            this.Controls.Add(this.buttonChoosebottomLevel);
            this.Controls.Add(this.buttonChooseTopLevel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxBottomLevel);
            this.Controls.Add(this.comboBoxTopLevel);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBottomOffset);
            this.Controls.Add(this.textBoxTopOffset);
            this.Controls.Add(this.textBoxBottomElevation);
            this.Controls.Add(this.textBoxTopElevation);
            this.Controls.Add(this.checkBoxcheckAllConnector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConduitLevelFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "线管过滤器";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxcheckAllConnector;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBoxTopElevation;
        private System.Windows.Forms.TextBox textBoxBottomElevation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxTopOffset;
        private System.Windows.Forms.TextBox textBoxBottomOffset;
        private System.Windows.Forms.ComboBox comboBoxTopLevel;
        private System.Windows.Forms.ComboBox comboBoxBottomLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButtonShow;
        private System.Windows.Forms.RadioButton radioButtonHide;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonChooseTopLevel;
        private System.Windows.Forms.Button buttonChoosebottomLevel;
    }
}