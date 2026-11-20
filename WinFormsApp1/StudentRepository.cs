using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public class StudentRepository
    {
        public List<Student> GetAll()
        {
            var list = new List<Student>();
            const string sql = @"
                SELECT StudentId, FirstName, LastName, Age, Email
                FROM Students
                ORDER BY StudentId DESC";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Student
                        {
                            Id = Convert.ToInt32(r["StudentId"]),
                            FirstName = r["FirstName"].ToString() ?? string.Empty,
                            LastName = r["LastName"].ToString() ?? string.Empty,
                            Age = Convert.ToInt32(r["Age"]),
                            Email = r["Email"] == DBNull.Value ? string.Empty : (r["Email"].ToString() ?? string.Empty)
                        });
                    }
                }
            }

            return list;
        }

        public int Add(Student student)
        {
            const string sql = @"
                INSERT INTO Students (FirstName, LastName, Age, Email)
                VALUES (@firstName, @lastName, @age, @email)";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                cmd.Parameters.Add(new SqlParameter("@age", student.Age));
                cmd.Parameters.Add(new SqlParameter("@email", student.Email));

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}