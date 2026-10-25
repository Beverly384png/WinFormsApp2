using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WinFormsApp2.Models;

namespace WinFormsApp2.Data
{
    public class MaterialRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=Aromahome;Trusted_Connection=True;TrustServerCertificate=True;";

        public List<MaterialListItem> GetMaterials()
        {
            var result = new List<MaterialListItem>();

            const string sql = @"
                SELECT
                    m.id,
                    mt.name                             AS material_type,
                    m.name                              AS material_name,
                    m.min_quantity,
                    m.stock_quantity,
                    m.price,
                    m.unit                              AS measure_unit,
                    COALESCE(SUM(pm.quantity), 0)       AS required_qty
                FROM materials m
                JOIN material_types mt ON mt.id = m.material_type_id
                LEFT JOIN material_products pm ON pm.material_id = m.id
                GROUP BY m.id, mt.name, m.name, m.min_quantity, m.stock_quantity, m.price, m.unit
                ORDER BY mt.name, m.name;";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new MaterialListItem
                {
                    Id = reader.GetInt32(0),
                    MaterialType = reader.GetString(1),
                    Name = reader.GetString(2),
                    MinQty = reader.GetDecimal(3),
                    StockQty = reader.GetDecimal(4),
                    Price = reader.GetDecimal(5),
                    MeasureUnit = reader.GetString(6),
                    RequiredQty = Math.Round(reader.GetDecimal(7), 2)
                });
            }

            return result;
        }

        public MaterialListItem? GetMaterialById(int id)
        {
            var all = GetMaterials();
            return all.Find(x => x.Id == id);
        }

        public void SaveMaterial(MaterialListItem item, bool isEdit)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            int typeId = 1;
            var cmdType = new SqlCommand("SELECT id FROM material_types WHERE name = @name", connection);
            cmdType.Parameters.AddWithValue("@name", item.MaterialType);
            var resType = cmdType.ExecuteScalar();

            if (resType != null)
            {
                typeId = Convert.ToInt32(resType);
            }
            else
            {
                var insertType = new SqlCommand("INSERT INTO material_types (name, loss_percent) OUTPUT INSERTED.id VALUES (@name, @loss)", connection);
                insertType.Parameters.AddWithValue("@name", item.MaterialType);
                insertType.Parameters.AddWithValue("@loss", 5.0);
                typeId = (int)insertType.ExecuteScalar();
            }

            if (isEdit)
            {
                string sql = @"
                    UPDATE materials 
                    SET material_type_id = @typeId, name = @name, price = @price, 
                        unit = @unit, stock_quantity = @stock, min_quantity = @min 
                    WHERE id = @id";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@price", item.Price);
                cmd.Parameters.AddWithValue("@unit", item.MeasureUnit);
                cmd.Parameters.AddWithValue("@stock", item.StockQty);
                cmd.Parameters.AddWithValue("@min", item.MinQty);
                cmd.Parameters.AddWithValue("@id", item.Id);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string sql = @"
                    INSERT INTO materials (material_type_id, name, price, unit, pack_quantity, stock_quantity, min_quantity) 
                    VALUES (@typeId, @name, @price, @unit, @pack, @stock, @min)";
                using var cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@price", item.Price);
                cmd.Parameters.AddWithValue("@unit", item.MeasureUnit);
                cmd.Parameters.AddWithValue("@pack", 1);
                cmd.Parameters.AddWithValue("@stock", item.StockQty);
                cmd.Parameters.AddWithValue("@min", item.MinQty);
                cmd.ExecuteNonQuery();
            }
        }

        public List<string> GetProductsForMaterial(int materialId)
        {
            var result = new List<string>();
            const string sql = @"
                SELECT p.name, pm.quantity 
                FROM products p
                JOIN material_products pm ON pm.product_id = p.id
                WHERE pm.material_id = @materialId";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@materialId", materialId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add($"{reader.GetString(0)} — {reader.GetDecimal(1)}");
            }
            return result;
        }
    }
}