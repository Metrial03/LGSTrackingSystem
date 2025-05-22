namespace LGSTrackingSystem
{
    partial class StudentReportPanel
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
            cbExamList = new ComboBox();
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
            panel2.Controls.Add(cbExamList);
            panel2.Controls.Add(label23);
            panel2.Location = new Point(12, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(248, 85);
            panel2.TabIndex = 39;
            // 
            // textBoxExamName
            // 
            textBoxExamName.Location = new Point(110, 16);
            textBoxExamName.Name = "textBoxExamName";
            textBoxExamName.Size = new Size(116, 23);
            textBoxExamName.TabIndex = 39;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.BackColor = Color.FromArgb(245, 247, 255);
            label20.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label20.Location = new Point(19, 17);
            label20.Name = "label20";
            label20.Size = new Size(85, 17);
            label20.TabIndex = 40;
            label20.Text = "Exam Name:";
            // 
            // cbExamList
            // 
            cbExamList.DropDownStyle = ComboBoxStyle.DropDownList;
            cbExamList.FormattingEnabled = true;
            cbExamList.Location = new Point(110, 45);
            cbExamList.Name = "cbExamList";
            cbExamList.Size = new Size(116, 23);
            cbExamList.TabIndex = 36;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.BackColor = Color.FromArgb(245, 247, 255);
            label23.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label23.Location = new Point(59, 47);
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
            btnExportPDF.Location = new Point(12, 103);
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
            btnShowGraph.Location = new Point(141, 103);
            btnShowGraph.Name = "btnShowGraph";
            btnShowGraph.Size = new Size(119, 23);
            btnShowGraph.TabIndex = 42;
            btnShowGraph.Text = "Show Graph";
            btnShowGraph.UseVisualStyleBackColor = false;
            btnShowGraph.Click += btnShowGraph_Click;
            // 
            // StudentReportPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Lavender;
            ClientSize = new Size(270, 137);
            Controls.Add(btnShowGraph);
            Controls.Add(btnExportPDF);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "StudentReportPanel";
            StartPosition = FormStartPosition.Manual;
            Text = "Report Panel";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        private Panel panel2;
        private TextBox textBoxExamName;
        private Label label20;
        private ComboBox cbExamList;
        private Label label23;
        private Button btnExportPDF;
        private Button btnShowGraph;
        #endregion
    }
}