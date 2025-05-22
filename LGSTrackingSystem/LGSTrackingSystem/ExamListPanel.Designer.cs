namespace LGSTrackingSystem
{
    partial class ExamListPanel
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridViewExams = new DataGridView();
            textBoxExamName = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewExams).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewExams
            // 
            dataGridViewExams.AllowUserToAddRows = false;
            dataGridViewExams.AllowUserToDeleteRows = false;
            dataGridViewExams.AllowUserToResizeColumns = false;
            dataGridViewExams.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.MistyRose;
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = Color.RoyalBlue;
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridViewExams.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewExams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewExams.BackgroundColor = Color.White;
            dataGridViewExams.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewExams.Location = new Point(0, 0);
            dataGridViewExams.Name = "dataGridViewExams";
            dataGridViewExams.ReadOnly = true;
            dataGridViewExams.RowHeadersVisible = false;
            dataGridViewExams.Size = new Size(184, 492);
            dataGridViewExams.TabIndex = 0;
            // 
            // textBoxExamName
            // 
            textBoxExamName.Location = new Point(12, 496);
            textBoxExamName.Name = "textBoxExamName";
            textBoxExamName.Size = new Size(160, 23);
            textBoxExamName.TabIndex = 1;
            textBoxExamName.TextChanged += textBoxExamName_TextChanged;
            // 
            // ExamListPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(184, 524);
            Controls.Add(textBoxExamName);
            Controls.Add(dataGridViewExams);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExamListPanel";
            StartPosition = FormStartPosition.Manual;
            Text = "ExamList";
            ((System.ComponentModel.ISupportInitialize)dataGridViewExams).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridViewExams;
        private TextBox textBoxExamName;
    }
}