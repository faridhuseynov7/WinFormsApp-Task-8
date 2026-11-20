using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class LibraryForm : Form
    {
        private int? _selectedBookId = null;
        private int? _selectedLoanId = null;

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public LibraryForm()
        {
            InitializeComponent();

            cmbAuthors.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoanBooks.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMembers.DropDownStyle = ComboBoxStyle.DropDownList;

            this.Load += LibraryForm_Load;
            dgvBooks.CellClick += dgvBooks_CellClick;
            dgvActiveLoans.CellClick += dgvActiveLoans_CellClick;

            btnAddBook.Click += btnAddBook_Click;
            btnUpdateBook.Click += btnUpdateBook_Click;
            btnDeleteBook.Click += btnDeleteBook_Click;

            btnLoan.Click += btnLoan_Click;
            btnReturn.Click += btnReturn_Click;
        }

        private bool TryGetSelectedId(ComboBox combo, out int id)
        {
            id = 0;
            if (combo.SelectedValue == null || combo.SelectedValue is ComboItem)
                return false;

            try
            {
                id = Convert.ToInt32(combo.SelectedValue);
                return id > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool IsBookCurrentlyLoaned(int bookId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Loans
                WHERE BookId = @bookId AND ReturnDate IS NULL";

            using (var con = new SqlConnection(Db.LibraryConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@bookId", bookId);
                con.Open();
                int activeCount = Convert.ToInt32(cmd.ExecuteScalar());
                return activeCount > 0;
            }
        }

        private void LibraryForm_Load(object? sender, EventArgs e)
        {
            LoadAuthors();
            LoadMembers();
            LoadBooks();
            LoadActiveLoans();
        }

        private void LoadAuthors()
        {
            var authors = new List<ComboItem>();
            const string sql = "SELECT AuthorId, FullName FROM Authors ORDER BY FullName";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            authors.Add(new ComboItem
                            {
                                Id = Convert.ToInt32(r["AuthorId"]),
                                Name = r["FullName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }

                cmbAuthors.DataSource = null;
                cmbAuthors.DisplayMember = "Name";
                cmbAuthors.ValueMember = "Id";
                cmbAuthors.DataSource = authors;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void LoadMembers()
        {
            var members = new List<ComboItem>();
            const string sql = "SELECT MemberId, FullName FROM Members ORDER BY FullName";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            members.Add(new ComboItem
                            {
                                Id = Convert.ToInt32(r["MemberId"]),
                                Name = r["FullName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }

                cmbMembers.DataSource = null;
                cmbMembers.DisplayMember = "Name";
                cmbMembers.ValueMember = "Id";
                cmbMembers.DataSource = members;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void LoadBooks()
        {
            const string sql = @"
                SELECT 
                    b.BookId,
                    b.AuthorId,
                    b.Title AS KitabAdi,
                    a.FullName AS Muellif,
                    b.PublishYear AS NesrIli,
                    b.Price AS Qiymet
                FROM Books b
                INNER JOIN Authors a ON b.AuthorId = a.AuthorId
                ORDER BY b.BookId DESC";

            try
            {
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(sql, Db.LibraryConnStr))
                {
                    da.Fill(dt);
                }

                dgvBooks.DataSource = dt;
                dgvBooks.ReadOnly = true;
                dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgvBooks.Columns["BookId"] != null)
                    dgvBooks.Columns["BookId"].Visible = false;
                if (dgvBooks.Columns["AuthorId"] != null)
                    dgvBooks.Columns["AuthorId"].Visible = false;

                var loanBooks = new List<ComboItem>();
                foreach (DataRow row in dt.Rows)
                {
                    loanBooks.Add(new ComboItem
                    {
                        Id = Convert.ToInt32(row["BookId"]),
                        Name = row["KitabAdi"].ToString() ?? string.Empty
                    });
                }

                cmbLoanBooks.DataSource = null;
                cmbLoanBooks.DisplayMember = "Name";
                cmbLoanBooks.ValueMember = "Id";
                cmbLoanBooks.DataSource = loanBooks;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void LoadActiveLoans()
        {
            const string sql = @"
                SELECT 
                    l.LoanId,
                    b.Title AS KitabAdi,
                    m.FullName AS UzvAdi,
                    l.LoanDate AS IcareTarixi
                FROM Loans l
                INNER JOIN Books b ON l.BookId = b.BookId
                INNER JOIN Members m ON l.MemberId = m.MemberId
                WHERE l.ReturnDate IS NULL
                ORDER BY l.LoanDate DESC";

            try
            {
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(sql, Db.LibraryConnStr))
                {
                    da.Fill(dt);
                }

                dgvActiveLoans.DataSource = dt;
                dgvActiveLoans.ReadOnly = true;
                dgvActiveLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgvActiveLoans.Columns["LoanId"] != null)
                    dgvActiveLoans.Columns["LoanId"].Visible = false;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private bool TryReadBookInput(out string title, out int publishYear, out int authorId, out decimal price)
        {
            title = txtTitle.Text.Trim();
            publishYear = 0;
            authorId = 0;
            price = 0;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Kitabin adini daxil edin.");
                return false;
            }

            if (!int.TryParse(txtPublishYear.Text.Trim(), out int yr) || yr <= 0)
            {
                MessageBox.Show("Nesr ilini duzgun tam eded kimi daxil edin.");
                return false;
            }

            if (!TryGetSelectedId(cmbAuthors, out int aId))
            {
                MessageBox.Show("Muellif secin.");
                return false;
            }

            string priceText = txtPrice.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pr) || pr <= 0)
            {
                MessageBox.Show("Qiymet 0-dan boyuk eded olmalidir.");
                return false;
            }

            publishYear = yr;
            authorId = aId;
            price = pr;
            return true;
        }

        private void btnAddBook_Click(object? sender, EventArgs e)
        {
            if (!TryReadBookInput(out string title, out int publishYear, out int authorId, out decimal price))
                return;

            const string sql = @"
                INSERT INTO Books (Title, PublishYear, AuthorId, Price)
                VALUES (@title, @year, @authorId, @price)";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@year", publishYear);
                    cmd.Parameters.AddWithValue("@authorId", authorId);
                    cmd.Parameters.AddWithValue("@price", price);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kitab elave olundu.");
                ClearBookInputs();
                LoadBooks();
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnUpdateBook_Click(object? sender, EventArgs e)
        {
            if (_selectedBookId == null)
            {
                MessageBox.Show("Yenilemek ucun cedvelden kitab secin.");
                return;
            }

            if (!TryReadBookInput(out string title, out int publishYear, out int authorId, out decimal price))
                return;

            const string sql = @"
                UPDATE Books
                SET Title = @title, PublishYear = @year, AuthorId = @authorId, Price = @price
                WHERE BookId = @id";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", _selectedBookId.Value);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@year", publishYear);
                    cmd.Parameters.AddWithValue("@authorId", authorId);
                    cmd.Parameters.AddWithValue("@price", price);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kitab yenilendi.");
                ClearBookInputs();
                LoadBooks();
                LoadActiveLoans();
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnDeleteBook_Click(object? sender, EventArgs e)
        {
            if (_selectedBookId == null)
            {
                MessageBox.Show("Silmek ucun cedvelden kitab secin.");
                return;
            }

            var confirm = MessageBox.Show(
                "Secilmis kitabi silmek isteyirsiniz?",
                "Tesdiq",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            const string sql = "DELETE FROM Books WHERE BookId = @id";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", _selectedBookId.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kitab silindi.");
                ClearBookInputs();
                LoadBooks();
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnLoan_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedId(cmbLoanBooks, out int bookId))
            {
                MessageBox.Show("Icareye vermek ucun kitab secin.");
                return;
            }

            if (!TryGetSelectedId(cmbMembers, out int memberId))
            {
                MessageBox.Show("Uzv secin.");
                return;
            }

            const string sql = @"
                INSERT INTO Loans (BookId, MemberId, LoanDate, ReturnDate)
                VALUES (@bookId, @memberId, GETDATE(), NULL)";

            try
            {
                if (IsBookCurrentlyLoaned(bookId))
                {
                    MessageBox.Show("Bu kitab hal-hazirda icarededir ve qaytarilmayib.");
                    return;
                }

                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    cmd.Parameters.AddWithValue("@memberId", memberId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kitab icareye verildi.");
                LoadActiveLoans();
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnReturn_Click(object? sender, EventArgs e)
        {
            if (_selectedLoanId == null)
            {
                MessageBox.Show("Qaytarmaq ucun qaytarilmamis kitablar siyahisindan setir secin.");
                return;
            }

            const string sql = @"
                UPDATE Loans
                SET ReturnDate = GETDATE()
                WHERE LoanId = @loanId AND ReturnDate IS NULL";

            try
            {
                using (var con = new SqlConnection(Db.LibraryConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@loanId", _selectedLoanId.Value);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Kitab qaytarildi.");
                        _selectedLoanId = null;
                        LoadActiveLoans();
                    }
                    else
                    {
                        MessageBox.Show("Secilmis icare artiq qaytarilib ve ya tapilmadi.");
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void dgvBooks_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvBooks.Rows[e.RowIndex];
            if (row.Cells["BookId"].Value != null && row.Cells["BookId"].Value != DBNull.Value)
                _selectedBookId = Convert.ToInt32(row.Cells["BookId"].Value);

            if (row.Cells["KitabAdi"].Value != null)
                txtTitle.Text = row.Cells["KitabAdi"].Value.ToString();

            if (row.Cells["NesrIli"].Value != null)
                txtPublishYear.Text = row.Cells["NesrIli"].Value.ToString();

            if (row.Cells["AuthorId"].Value != null && row.Cells["AuthorId"].Value != DBNull.Value)
                cmbAuthors.SelectedValue = Convert.ToInt32(row.Cells["AuthorId"].Value);

            if (row.Cells["Qiymet"].Value != null)
                txtPrice.Text = row.Cells["Qiymet"].Value.ToString();
        }

        private void dgvActiveLoans_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvActiveLoans.Rows[e.RowIndex];
            if (row.Cells["LoanId"].Value != null && row.Cells["LoanId"].Value != DBNull.Value)
                _selectedLoanId = Convert.ToInt32(row.Cells["LoanId"].Value);
        }

        private void ClearBookInputs()
        {
            _selectedBookId = null;
            txtTitle.Clear();
            txtPublishYear.Clear();
            txtPrice.Clear();
        }

        private void ShowDbError(SqlException ex)
        {
            if (ex.Number == 547)
            {
                MessageBox.Show("Bu kitab icare cedvelinde istifade olundugu ucun siline bilmez (547 xetasi).");
            }
            else
            {
                MessageBox.Show("Verilenler bazasi xetasi: " + ex.Message);
            }
        }
    }
}