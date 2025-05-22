namespace LGSTrackingSystem
{
    partial class AdminReportPanel  
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
            panel2 = new Panel();
            textBoxExamName = new TextBox();
            label20 = new Label();
            label1 = new Label();
            cbStudentList = new ComboBox();
            cbExamList = new ComboBox();
            textBoxTestStudentName = new TextBox();
            label7 = new Label();
            label23 = new Label();
            btnExportPDF = new Button();
            btnShowGraph = new Button();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(245, 247, 255);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(textBoxExamName);
            panel2.Controls.Add(label20);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(cbStudentList);
            panel2.Controls.Add(cbExamList);
            panel2.Controls.Add(textBoxTestStudentName);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label23);
            panel2.Location = new Point(12, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(248, 138);
            panel2.TabIndex = 39;
            // 
            // textBoxExamName
            // 
            textBoxExamName.Location = new Point(114, 73);
            textBoxExamName.Name = "textBoxExamName";
            textBoxExamName.Size = new Size(116, 23);
            textBoxExamName.TabIndex = 39;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.BackColor = Color.FromArgb(245, 247, 255);
            label20.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label20.Location = new Point(23, 74);
            label20.Name = "label20";
            label20.Size = new Size(85, 17);
            label20.TabIndex = 40;
            label20.Text = "Exam Name:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(245, 247, 255);
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label1.Location = new Point(48, 42);
            label1.Name = "label1";
            label1.Size = new Size(60, 17);
            label1.TabIndex = 38;
            label1.Text = "Student:";
            // 
            // cbStudentList
            // 
            cbStudentList.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStudentList.FormattingEnabled = true;
            cbStudentList.Location = new Point(114, 39);
            cbStudentList.Name = "cbStudentList";
            cbStudentList.Size = new Size(116, 23);
            cbStudentList.TabIndex = 37;
            // 
            // cbExamList
            // 
            cbExamList.DropDownStyle = ComboBoxStyle.DropDownList;
            cbExamList.FormattingEnabled = true;
            cbExamList.Location = new Point(114, 102);
            cbExamList.Name = "cbExamList";
            cbExamList.Size = new Size(116, 23);
            cbExamList.TabIndex = 36;
            // 
            // textBoxTestStudentName
            // 
            textBoxTestStudentName.Location = new Point(114, 10);
            textBoxTestStudentName.Name = "textBoxTestStudentName";
            textBoxTestStudentName.Size = new Size(116, 23);
            textBoxTestStudentName.TabIndex = 0;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(245, 247, 255);
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label7.Location = new Point(8, 11);
            label7.Name = "label7";
            label7.Size = new Size(100, 17);
            label7.TabIndex = 0;
            label7.Text = "Student Name:";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.BackColor = Color.FromArgb(245, 247, 255);
            label23.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label23.Location = new Point(63, 104);
            label23.Name = "label23";
            label23.Size = new Size(45, 17);
            label23.TabIndex = 35;
            label23.Text = "Exam:";
            // 
            // btnExportPDF
            // 
            btnExportPDF.BackColor = Color.FromArgb(161, 164, 175);
            btnExportPDF.FlatStyle = FlatStyle.Popup;
            btnExportPDF.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.Location = new Point(12, 156);
            btnExportPDF.Name = "btnExportPDF";
            btnExportPDF.Size = new Size(119, 23);
            btnExportPDF.TabIndex = 41;
            btnExportPDF.Text = "Export PDF";
            btnExportPDF.UseVisualStyleBackColor = false;
            btnExportPDF.Click += btnExportPDF_Click;
            // 
            // btnShowGraph
            // 
            btnShowGraph.BackColor = Color.FromArgb(161, 164, 175);
            btnShowGraph.FlatStyle = FlatStyle.Popup;
            btnShowGraph.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnShowGraph.ForeColor = Color.White;
            btnShowGraph.Location = new Point(141, 156);
            btnShowGraph.Name = "btnShowGraph";
            btnShowGraph.Size = new Size(119, 23);
            btnShowGraph.TabIndex = 42;
            btnShowGraph.Text = "Show Graph";
            btnShowGraph.UseVisualStyleBackColor = false;
            btnShowGraph.Click += btnShowGraph_Click;
            // 
            // AdminReportPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Lavender;
            ClientSize = new Size(270, 189);
            Controls.Add(btnShowGraph);
            Controls.Add(btnExportPDF);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "AdminReportPanel";
            StartPosition = FormStartPosition.Manual;
            Text = "Report Panel";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel2;
        private TextBox textBoxExamName;
        private Label label20;
        private Label label1;
        private ComboBox cbStudentList;
        private ComboBox cbExamList;
        private TextBox textBoxTestStudentName;
        private Label label7;
        private Label label23;
        private Button btnExportPDF;
        private Button btnShowGraph;
    }
}