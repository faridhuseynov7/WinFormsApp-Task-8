namespace WinFormsApp1
{
    partial class LibraryForm
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
            txtTitle = new TextBox();
            txtPublishYear = new TextBox();
            txtPrice = new TextBox();
            cmbAuthors = new ComboBox();
            btnAddBook = new Button();
            btnUpdateBook = new Button();
            btnDeleteBook = new Button();
            dgvBooks = new DataGridView();
            cmbLoanBooks = new ComboBox();
            cmbMembers = new ComboBox();
            btnLoan = new Button();
            btnReturn = new Button();
            dgvActiveLoans = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvActiveLoans).BeginInit();
            SuspendLayout();
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(124, 21);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(125, 27);
            txtTitle.TabIndex = 0;
            // 
            // txtPublishYear
            // 
            txtPublishYear.Location = new Point(124, 80);
            txtPublishYear.Name = "txtPublishYear";
            txtPublishYear.Size = new Size(125, 27);
            txtPublishYear.TabIndex = 1;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(124, 188);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(125, 27);
            txtPrice.TabIndex = 2;
            // 
            // cmbAuthors
            // 
            cmbAuthors.FormattingEnabled = true;
            cmbAuthors.Location = new Point(124, 136);
            cmbAuthors.Name = "cmbAuthors";
            cmbAuthors.Size = new Size(151, 28);
            cmbAuthors.TabIndex = 3;
            // 
            // btnAddBook
            // 
            btnAddBook.Location = new Point(314, 21);
            btnAddBook.Name = "btnAddBook";
            btnAddBook.Size = new Size(94, 29);
            btnAddBook.TabIndex = 4;
            btnAddBook.Text = "Elave et";
            btnAddBook.UseVisualStyleBackColor = true;
            // 
            // btnUpdateBook
            // 
            btnUpdateBook.Location = new Point(314, 78);
            btnUpdateBook.Name = "btnUpdateBook";
            btnUpdateBook.Size = new Size(94, 29);
            btnUpdateBook.TabIndex = 5;
            btnUpdateBook.Text = "Yenile";
            btnUpdateBook.UseVisualStyleBackColor = true;
            // 
            // btnDeleteBook
            // 
            btnDeleteBook.Location = new Point(314, 135);
            btnDeleteBook.Name = "btnDeleteBook";
            btnDeleteBook.Size = new Size(94, 29);
            btnDeleteBook.TabIndex = 6;
            btnDeleteBook.Text = "Sil";
            btnDeleteBook.UseVisualStyleBackColor = true;
            // 
            // dgvBooks
            // 
            dgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBooks.Location = new Point(12, 241);
            dgvBooks.Name = "dgvBooks";
            dgvBooks.RowHeadersWidth = 51;
            dgvBooks.Size = new Size(396, 188);
            dgvBooks.TabIndex = 7;
            // 
            // cmbLoanBooks
            // 
            cmbLoanBooks.FormattingEnabled = true;
            cmbLoanBooks.Location = new Point(543, 22);
            cmbLoanBooks.Name = "cmbLoanBooks";
            cmbLoanBooks.Size = new Size(151, 28);
            cmbLoanBooks.TabIndex = 8;
            // 
            // cmbMembers
            // 
            cmbMembers.FormattingEnabled = true;
            cmbMembers.Location = new Point(543, 78);
            cmbMembers.Name = "cmbMembers";
            cmbMembers.Size = new Size(151, 28);
            cmbMembers.TabIndex = 9;
            // 
            // btnLoan
            // 
            btnLoan.Location = new Point(580, 135);
            btnLoan.Name = "btnLoan";
            btnLoan.Size = new Size(94, 29);
            btnLoan.TabIndex = 10;
            btnLoan.Text = "Icare ver";
            btnLoan.UseVisualStyleBackColor = true;
            // 
            // btnReturn
            // 
            btnReturn.Location = new Point(580, 186);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(94, 29);
            btnReturn.TabIndex = 11;
            btnReturn.Text = "Qaytar";
            btnReturn.UseVisualStyleBackColor = true;
            // 
            // dgvActiveLoans
            // 
            dgvActiveLoans.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvActiveLoans.Location = new Point(465, 241);
            dgvActiveLoans.Name = "dgvActiveLoans";
            dgvActiveLoans.RowHeadersWidth = 51;
            dgvActiveLoans.Size = new Size(300, 188);
            dgvActiveLoans.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 25);
            label1.Name = "label1";
            label1.Size = new Size(69, 20);
            label1.TabIndex = 13;
            label1.Text = "Kitab adi";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 87);
            label2.Name = "label2";
            label2.Size = new Size(55, 20);
            label2.TabIndex = 14;
            label2.Text = "Nesr ili";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 139);
            label3.Name = "label3";
            label3.Size = new Size(80, 20);
            label3.TabIndex = 15;
            label3.Text = "Muellif sec";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(25, 191);
            label4.Name = "label4";
            label4.Size = new Size(57, 20);
            label4.TabIndex = 16;
            label4.Text = "Qiymet";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(438, 30);
            label5.Name = "label5";
            label5.Size = new Size(78, 20);
            label5.TabIndex = 17;
            label5.Text = "Icare kitab";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(438, 86);
            label6.Name = "label6";
            label6.Size = new Size(79, 20);
            label6.TabIndex = 18;
            label6.Text = "Uzv secimi";
            // 
            // LibraryForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dgvActiveLoans);
            Controls.Add(btnReturn);
            Controls.Add(btnLoan);
            Controls.Add(cmbMembers);
            Controls.Add(cmbLoanBooks);
            Controls.Add(dgvBooks);
            Controls.Add(btnDeleteBook);
            Controls.Add(btnUpdateBook);
            Controls.Add(btnAddBook);
            Controls.Add(cmbAuthors);
            Controls.Add(txtPrice);
            Controls.Add(txtPublishYear);
            Controls.Add(txtTitle);
            Name = "LibraryForm";
            Text = "LibraryForm";
            ((System.ComponentModel.ISupportInitialize)dgvBooks).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvActiveLoans).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtTitle;
        private TextBox txtPublishYear;
        private TextBox txtPrice;
        private ComboBox cmbAuthors;
        private Button btnAddBook;
        private Button btnUpdateBook;
        private Button btnDeleteBook;
        private DataGridView dgvBooks;
        private ComboBox cmbLoanBooks;
        private ComboBox cmbMembers;
        private Button btnLoan;
        private Button btnReturn;
        private DataGridView dgvActiveLoans;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}