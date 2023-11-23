namespace ModelContertApp
{
    partial class Main
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
            this.buttonSelectInputFile = new System.Windows.Forms.Button();
            this.buttonSaveFileLocation = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSaveFileLocation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.checkBoxFlipYZ = new System.Windows.Forms.CheckBox();
            this.checkBoxFlipTriangles = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxModelUnits = new System.Windows.Forms.ComboBox();
            this.buttonUpdateConverter = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.siteTxtBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonSelectInputFile
            // 
            this.buttonSelectInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectInputFile.Location = new System.Drawing.Point(1193, 19);
            this.buttonSelectInputFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSelectInputFile.Name = "buttonSelectInputFile";
            this.buttonSelectInputFile.Size = new System.Drawing.Size(112, 35);
            this.buttonSelectInputFile.TabIndex = 0;
            this.buttonSelectInputFile.Text = "...";
            this.buttonSelectInputFile.UseVisualStyleBackColor = true;
            this.buttonSelectInputFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // buttonSaveFileLocation
            // 
            this.buttonSaveFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveFileLocation.Location = new System.Drawing.Point(1193, 62);
            this.buttonSaveFileLocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSaveFileLocation.Name = "buttonSaveFileLocation";
            this.buttonSaveFileLocation.Size = new System.Drawing.Size(112, 35);
            this.buttonSaveFileLocation.TabIndex = 1;
            this.buttonSaveFileLocation.Text = "...";
            this.buttonSaveFileLocation.UseVisualStyleBackColor = true;
            this.buttonSaveFileLocation.Click += new System.EventHandler(this.buttonSaveFile_Click);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileName.Location = new System.Drawing.Point(123, 21);
            this.textBoxFileName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ReadOnly = true;
            this.textBoxFileName.Size = new System.Drawing.Size(1060, 26);
            this.textBoxFileName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Input File: ";
            // 
            // textBoxSaveFileLocation
            // 
            this.textBoxSaveFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSaveFileLocation.Location = new System.Drawing.Point(123, 66);
            this.textBoxSaveFileLocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxSaveFileLocation.Name = "textBoxSaveFileLocation";
            this.textBoxSaveFileLocation.Size = new System.Drawing.Size(1060, 26);
            this.textBoxSaveFileLocation.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 71);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output File: ";
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConvert.Location = new System.Drawing.Point(230, 227);
            this.buttonConvert.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(1076, 42);
            this.buttonConvert.TabIndex = 8;
            this.buttonConvert.Text = "Convert";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // checkBoxFlipYZ
            // 
            this.checkBoxFlipYZ.AutoSize = true;
            this.checkBoxFlipYZ.Location = new System.Drawing.Point(123, 122);
            this.checkBoxFlipYZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxFlipYZ.Name = "checkBoxFlipYZ";
            this.checkBoxFlipYZ.Size = new System.Drawing.Size(81, 24);
            this.checkBoxFlipYZ.TabIndex = 9;
            this.checkBoxFlipYZ.Text = "FlipYZ";
            this.checkBoxFlipYZ.UseVisualStyleBackColor = true;
            // 
            // checkBoxFlipTriangles
            // 
            this.checkBoxFlipTriangles.AutoSize = true;
            this.checkBoxFlipTriangles.Location = new System.Drawing.Point(230, 122);
            this.checkBoxFlipTriangles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxFlipTriangles.Name = "checkBoxFlipTriangles";
            this.checkBoxFlipTriangles.Size = new System.Drawing.Size(124, 24);
            this.checkBoxFlipTriangles.TabIndex = 10;
            this.checkBoxFlipTriangles.Text = "FlipTriangles";
            this.checkBoxFlipTriangles.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(382, 125);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Model Units: ";
            // 
            // comboBoxModelUnits
            // 
            this.comboBoxModelUnits.FormattingEnabled = true;
            this.comboBoxModelUnits.Location = new System.Drawing.Point(495, 120);
            this.comboBoxModelUnits.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBoxModelUnits.Name = "comboBoxModelUnits";
            this.comboBoxModelUnits.Size = new System.Drawing.Size(162, 28);
            this.comboBoxModelUnits.TabIndex = 12;
            // 
            // buttonUpdateConverter
            // 
            this.buttonUpdateConverter.Location = new System.Drawing.Point(18, 179);
            this.buttonUpdateConverter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonUpdateConverter.Name = "buttonUpdateConverter";
            this.buttonUpdateConverter.Size = new System.Drawing.Size(202, 42);
            this.buttonUpdateConverter.TabIndex = 13;
            this.buttonUpdateConverter.Text = "Update Converter";
            this.buttonUpdateConverter.UseVisualStyleBackColor = true;
            this.buttonUpdateConverter.Click += new System.EventHandler(this.buttonUpdateConverter_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(835, 121);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(112, 26);
            this.nameTextBox.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(699, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 20);
            this.label4.TabIndex = 15;
            this.label4.Text = "Building Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1006, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 20);
            this.label5.TabIndex = 17;
            this.label5.Text = "Site Name";
            // 
            // siteTxtBox
            // 
            this.siteTxtBox.Location = new System.Drawing.Point(1131, 122);
            this.siteTxtBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.siteTxtBox.Name = "siteTxtBox";
            this.siteTxtBox.Size = new System.Drawing.Size(112, 26);
            this.siteTxtBox.TabIndex = 16;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1323, 288);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.siteTxtBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.buttonUpdateConverter);
            this.Controls.Add(this.comboBoxModelUnits);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxFlipTriangles);
            this.Controls.Add(this.checkBoxFlipYZ);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSaveFileLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.buttonSaveFileLocation);
            this.Controls.Add(this.buttonSelectInputFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Model Converter";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSelectInputFile;
        private System.Windows.Forms.Button buttonSaveFileLocation;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSaveFileLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.CheckBox checkBoxFlipYZ;
        private System.Windows.Forms.CheckBox checkBoxFlipTriangles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxModelUnits;
        private System.Windows.Forms.Button buttonUpdateConverter;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox siteTxtBox;
    }
}

