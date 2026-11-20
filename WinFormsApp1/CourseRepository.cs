using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public class CourseRepository
    {
        public List<Course> GetAll()
        {
            var list = new List<Course>();
            const string sql = "SELECT Id, Title, Credits FROM Courses ORDER BY Id DESC";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Course
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Title = r["Title"].ToString() ?? string.Empty,
                            Credits = Convert.ToInt32(r["Credits"])
                        });
                    }
                }
            }

            return list;
        }

        public int Add(Course course)
        {
            const string sql = "INSERT INTO Courses (Title, Credits) VALUES (@title, @credits)";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@title", course.Title);
                cmd.Parameters.AddWithValue("@credits", course.Credits);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Update(Course course)
        {
            const string sql = "UPDATE Courses SET Title = @title, Credits = @credits WHERE Id = @id";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", course.Id);
                cmd.Parameters.AddWithValue("@title", course.Title);
                cmd.Parameters.AddWithValue("@credits", course.Credits);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Delete(int id)
        {
            const string sql = "DELETE FROM Courses WHERE Id = @id";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Count()
        {
            const string sql = "SELECT COUNT(*) FROM Courses";

            using (var con = new SqlConnection(Db.ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }
    }
}