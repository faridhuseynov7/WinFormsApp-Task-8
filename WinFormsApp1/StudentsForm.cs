using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class StudentsForm : Form
    {
        private readonly StudentRepository _repo = new StudentRepository();

        public StudentsForm()
        {
            InitializeComponent();
            this.Load += StudentsForm_Load;
            btnAdd.Click += btnAdd_Click;
        }

        private void StudentsForm_Load(object? sender, EventArgs e)
        {
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                dgvStudents.DataSource = null;
                dgvStudents.DataSource = _repo.GetAll();
                dgvStudents.ReadOnly = true;
                dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                if (dgvStudents.Columns["Id"] != null)
                    dgvStudents.Columns["Id"].Visible = false;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Verilenler bazasi xetasi: " + ex.Message);
            }
        }

        private bool TryReadStudent(out Student student)
        {
            student = new Student();

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Ad bos ola bilmez.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Soyad bos ola bilmez.");
                return false;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age <= 0)
            {
                MessageBox.Show("Yasi duzgun tam eded kimi daxil edin.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("E-poct bos ola bilmez.");
                return false;
            }

            student.FirstName = firstName;
            student.LastName = lastName;
            student.Age = age;
            student.Email = email;
            return true;
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!TryReadStudent(out Student student))
                return;

            try
            {
                int rows = _repo.Add(student);
                if (rows > 0)
                {
                    MessageBox.Show("Telebe elave olundu.");
                    ClearInputs();
                    LoadStudents();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    MessageBox.Show("Bu e-poct artiq movcuddur");
                }
                else
                {
                    MessageBox.Show("Verilenler bazasi xetasi: " + ex.Message);
                }
            }
        }

        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAge.Clear();
            txtEmail.Clear();
            txtFirstName.Focus();
        }
    }
}