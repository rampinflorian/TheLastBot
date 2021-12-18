using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using ManziBot.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace DistillerieManzibar.Data.Dapper
{
    public class CustomQuery
    {
        private readonly ApplicationDbContext _context;
        private DbConnection _connection;
        public CustomQuery(ApplicationDbContext context)
        {
            _context = context;
            _connection = _context.Database.GetDbConnection();
            _context.Database.OpenConnectionAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query)
        {
            return await _connection.QueryAsync<T>(query);
        }

        public int Execute(string query, object parameters)
        {
            return _connection.Execute(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object parameters)
        {
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}