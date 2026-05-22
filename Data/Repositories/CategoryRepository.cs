// CategoryRepository - репозиторий для работы с таблицей categories.
// Create - создает новую ценовую категорию с базовой ценой и мультипликатором.
// GetById - возвращает категорию по идентификатору или null.
// GetAll - возвращает список всех категорий, отсортированных по убыванию базовой цены.
// Update - обновляет название, базовую цену и мультипликатор категории.
// Delete - физическое удаление категории из базы данных.

using System.Data;
using Dapper;
using EventTicket.Models;

namespace EventTicket.Data.Repositories
{
    public class CategoryRepository
    {
        private readonly IDbConnection _connection;

        public CategoryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Category Create(Category category)
        {
            var sql = @"INSERT INTO categories (name, base_price, multiplier) 
                        VALUES (@Name, @BasePrice, @Multiplier) RETURNING id";
            category.Id = _connection.QuerySingle<int>(sql, category);
            return category;
        }

        public Category? GetById(int id)
        {
            return _connection.QuerySingleOrDefault<Category>(
                "SELECT id, name, base_price AS BasePrice, multiplier AS Multiplier FROM categories WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Category> GetAll()
        {
            return _connection.Query<Category>(
                "SELECT id, name, base_price AS BasePrice, multiplier AS Multiplier FROM categories ORDER BY base_price DESC");
        }

        public void Update(Category category)
        {
            _connection.Execute(@"
                UPDATE categories 
                SET name = @Name, base_price = @BasePrice, multiplier = @Multiplier 
                WHERE id = @Id", category);
        }

        public void Delete(int id)
        {
            _connection.Execute("DELETE FROM categories WHERE id = @Id", new { Id = id });
        }
    }
}