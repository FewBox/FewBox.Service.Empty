using System.Data;
using System.Text;
using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace FewBox.Service.Shipping.Controllers
{
    /// <summary>
    /// MySql
    /// </summary>
    public class MySqlController : MapperController
    {
        public MySqlController(IMapper mapper) : base(mapper)
        {
        }

        [HttpGet("{connectionString}")]
        public PayloadResponseDto<string> Get(string connectionString = "NULL")
        {
            StringBuilder result = new StringBuilder();
            if (connectionString == "NULL")
            {
                connectionString = "Server=192.168.1.38;Database=mysql;Uid=root;Pwd={Password};SslMode=none;Charset=utf8;ConnectionTimeout=60;DefaultCommandTimeout=60;";
            }
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"select Host,User from user;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Append($"HOST: {reader.GetValue(0)}; USER: {reader.GetValue(1)}.");
                    }
                }
            }
            return new PayloadResponseDto<string>
            {
                Payload = result.ToString()
            };
        }
    }
}