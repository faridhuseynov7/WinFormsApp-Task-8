using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class CoursesForm : Form
    {
        private readonly CourseRepository _repo = new CourseRepository();
        private int? _selectedId = null;

        public CoursesForm()
        {
            InitializeComponent();
            this.Load += CoursesForm_Load;
            dgvCourses.CellClick += dgvCourses_CellClick;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
        }

        private void CoursesForm_Load(object? sender, EventArgs e)
        {
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                var items = _repo.GetAll();
                dgvCourses.DataSource = null;
                dgvCourses.DataSource = items;
                dgvCourses.ReadOnly = true;
                dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgvCourses.Columns["Id"] != null)
                    dgvCourses.Columns["Id"].Visible = false;

                lblCount.Text = "Fenn sayi: " + _repo.Count();
            }
            catch (SqlException ex)
            {
                ShowSqlError(ex);
            }
        }

        private bool TryReadInput(out string title, out int credits)
        {
            title = txtTitle.Text.Trim();
            credits = 0;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Fenn adini daxil edin.");
                return false;
            }

            if (!int.TryParse(txtCredits.Text.Trim(), out int cr) || cr < 1 || cr > 10)
            {
                MessageBox.Show("Kredit yalniz 1 ile 10 arasinda tam eded olmalidir.");
                return false;
            }

            credits = cr;
            return true;
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!TryReadInput(out string title, out int credits))
                return;

            try
            {
                _repo.Add(new Course
                {
                    Title = title,
                    Credits = credits
                });

                MessageBox.Show("Fenn elave olundu.");
                ClearInputs();
                LoadCourses();
            }
            catch (SqlException ex)
            {
                ShowSqlError(ex);
            }
        }

        private void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Duzelis etmek ucun cedvelden fenn secin.");
                return;
            }

            if (!TryReadInput(out string title, out int credits))
                return;

            try
            {
                int rows = _repo.Update(new Course
                {
                    Id = _selectedId.Value,
                    Title = title,
                    Credits = credits
                });

                MessageBox.Show(rows == 0 ? "Secilmis fenn tapilmadi." : "Fenn yenilendi.");
                ClearInputs();
                LoadCourses();
            }
            catch (SqlException ex)
            {
                ShowSqlError(ex);
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Silmek ucun cedvelden fenn secin.");
                return;
            }

            var confirm = MessageBox.Show(
                "Secilmis fenni silmek isteyirsiniz?",
                "Tesdiq",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                int rows = _repo.Delete(_selectedId.Value);
                MessageBox.Show(rows == 0 ? "Silinecek fenn tapilmadi." : "Fenn silindi.");
                ClearInputs();
                LoadCourses();
            }
            catch (SqlException ex)
            {
                ShowSqlError(ex);
            }
        }

        private void dgvCourses_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvCourses.Rows[e.RowIndex].DataBoundItem is Course c)
            {
                _selectedId = c.Id;
                txtTitle.Text = c.Title;
                txtCredits.Text = c.Credits.ToString();
            }
        }

        private void ClearInputs()
        {
            _selectedId = null;
            txtTitle.Clear();
            txtCredits.Clear();
        }

        private void ShowSqlError(SqlException ex)
        {
            if (ex.Number == 547)
            {
                MessageBox.Show(
                    "Bu fenne aid qiymetler movcud oldugu ucun fenni silmek mumkun deyil (547 xetasi).",
                    "Silme mehdudiyyeti",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Verilenler bazasi xetasi: " + ex.Message);
            }
        }
    }
}