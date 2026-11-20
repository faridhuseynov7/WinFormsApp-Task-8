using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private int? _selectedGradeId = null;

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            cmbStudents.SelectedIndexChanged += cmbStudents_SelectedIndexChanged;
            dgvGrades.CellClick += dgvGrades_CellClick;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            LoadStudents();
            LoadCourses();
            LoadGrades();
            UpdateAverageLabel();
        }

        private void LoadStudents()
        {
            var students = new List<ComboItem>();
            const string sql = "SELECT StudentId, FirstName + ' ' + LastName AS FullName FROM Students ORDER BY FirstName";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            students.Add(new ComboItem
                            {
                                Id = r.GetInt32(0),
                                Name = r["FullName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }

                cmbStudents.DataSource = null;
                cmbStudents.DisplayMember = "Name";
                cmbStudents.ValueMember = "Id";
                cmbStudents.DataSource = students;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void LoadCourses()
        {
            var courses = new List<ComboItem>();
            const string sql = "SELECT CourseId, CourseName FROM Courses ORDER BY CourseName";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            courses.Add(new ComboItem
                            {
                                Id = r.GetInt32(0),
                                Name = r["CourseName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }

                cmbCourses.DataSource = null;
                cmbCourses.DisplayMember = "Name";
                cmbCourses.ValueMember = "Id";
                cmbCourses.DataSource = courses;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void LoadGrades()
        {
            const string sql = @"
                SELECT 
                    g.GradeId,
                    g.StudentId,
                    g.CourseId,
                    s.FirstName + ' ' + s.LastName AS TelebeAdi,
                    c.CourseName AS FennAdi,
                    g.Score AS Qiymet
                FROM Grades g
                INNER JOIN Students s ON g.StudentId = s.StudentId
                INNER JOIN Courses c ON g.CourseId = c.CourseId
                ORDER BY g.GradeId DESC";

            try
            {
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(sql, Db.ConnStr))
                {
                    da.Fill(dt);
                }

                dgvGrades.DataSource = dt;
                dgvGrades.ReadOnly = true;
                dgvGrades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgvGrades.Columns["GradeId"] != null)
                    dgvGrades.Columns["GradeId"].Visible = false;
                if (dgvGrades.Columns["StudentId"] != null)
                    dgvGrades.Columns["StudentId"].Visible = false;
                if (dgvGrades.Columns["CourseId"] != null)
                    dgvGrades.Columns["CourseId"].Visible = false;
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void UpdateAverageLabel()
        {
            if (cmbStudents.SelectedValue == null || !(cmbStudents.SelectedValue is int studentId))
            {
                lblAverage.Text = "Qiymet yoxdur";
                return;
            }

            const string sql = "SELECT AVG(Score * 1.0) FROM Grades WHERE StudentId = @sid";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        lblAverage.Text = "Qiymet yoxdur";
                    }
                    else
                    {
                        decimal avg = Convert.ToDecimal(result);
                        lblAverage.Text = $"Orta bal: {avg:0.##}";
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private bool TryGetValidatedInput(out int studentId, out int courseId, out int score)
        {
            studentId = 0;
            courseId = 0;
            score = 0;

            if (cmbStudents.SelectedValue == null || !(cmbStudents.SelectedValue is int sId))
            {
                MessageBox.Show("Zehmet olmasa, telebe secin.");
                return false;
            }

            if (cmbCourses.SelectedValue == null || !(cmbCourses.SelectedValue is int cId))
            {
                MessageBox.Show("Zehmet olmasa, fenn secin.");
                return false;
            }

            if (!int.TryParse(txtScore.Text.Trim(), out int sc) || sc < 0 || sc > 100)
            {
                MessageBox.Show("Qiymet 0 ile 100 arasinda tam eded olmalidir.");
                return false;
            }

            studentId = sId;
            courseId = cId;
            score = sc;
            return true;
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!TryGetValidatedInput(out int studentId, out int courseId, out int score))
                return;

            const string sql = @"
                INSERT INTO Grades (StudentId, CourseId, Score)
                VALUES (@sid, @cid, @score)";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.Parameters.AddWithValue("@score", score);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Qiymet elave olundu.");
                        txtScore.Clear();
                        LoadGrades();
                        UpdateAverageLabel();
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedGradeId == null)
            {
                MessageBox.Show("Duzelis etmek ucun cedvelden qiymet setri secin.");
                return;
            }

            if (!TryGetValidatedInput(out int studentId, out int courseId, out int score))
                return;

            const string sql = @"
                UPDATE Grades
                SET StudentId = @sid, CourseId = @cid, Score = @score
                WHERE GradeId = @gid";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@gid", _selectedGradeId.Value);
                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@cid", courseId);
                    cmd.Parameters.AddWithValue("@score", score);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        MessageBox.Show("Secilmis qiymet tapilmadi.");
                    }
                    else
                    {
                        MessageBox.Show("Qiymet yenilendi.");
                        LoadGrades();
                        UpdateAverageLabel();
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedGradeId == null)
            {
                MessageBox.Show("Silmek ucun cedvelden qiymet setri secin.");
                return;
            }

            var confirm = MessageBox.Show(
                "Secilmis qiymeti silmek istediyinize eminsiniz?",
                "Tesdiq",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            const string sql = "DELETE FROM Grades WHERE GradeId = @gid";

            try
            {
                using (var con = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@gid", _selectedGradeId.Value);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        MessageBox.Show("Silinecek qiymet tapilmadi.");
                    }
                    else
                    {
                        MessageBox.Show("Qiymet silindi.");
                        _selectedGradeId = null;
                        txtScore.Clear();
                        LoadGrades();
                        UpdateAverageLabel();
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowDbError(ex);
            }
        }

        private void dgvGrades_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvGrades.Rows[e.RowIndex];
            if (row.Cells["GradeId"].Value != null && row.Cells["GradeId"].Value != DBNull.Value)
                _selectedGradeId = Convert.ToInt32(row.Cells["GradeId"].Value);

            if (row.Cells["StudentId"].Value != null && row.Cells["StudentId"].Value != DBNull.Value)
                cmbStudents.SelectedValue = Convert.ToInt32(row.Cells["StudentId"].Value);

            if (row.Cells["CourseId"].Value != null && row.Cells["CourseId"].Value != DBNull.Value)
                cmbCourses.SelectedValue = Convert.ToInt32(row.Cells["CourseId"].Value);

            if (row.Cells["Qiymet"].Value != null && row.Cells["Qiymet"].Value != DBNull.Value)
                txtScore.Text = row.Cells["Qiymet"].Value.ToString();
        }

        private void cmbStudents_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateAverageLabel();
        }

        private void ShowDbError(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627:
                    MessageBox.Show("Bu melumat artiq movcuddur (tekrarlanan qeyd).");
                    break;
                case 547:
                    MessageBox.Show("Elaqeli melumat ve ya qayda (FOREIGN KEY / CHECK) pozulub.");
                    break;
                case 4060:
                    MessageBox.Show("Verilenler bazasi acila bilmir. Baza adini yoxlayin.");
                    break;
                default:
                    MessageBox.Show("Verilenler bazasi xetasi: " + ex.Message);
                    break;
            }
        }
    }
}