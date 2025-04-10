using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Data.Sqlite;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString;

    // Start is called before the first frame update
    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/PlayerData.db";
        CreateTable();//创建表
        InsertData("Jack", 10);//向"Player"表中插入一条玩家角色信息
        QueryData();//查询所有的角色信息
    }

    // 创建表
    void CreateTable()
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Player (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, level INTEGER)";
                cmd.ExecuteNonQuery();
            }
        }
    }

    // 插入数据
    void InsertData(string name, int level)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Player (name, level) VALUES (@name, @level)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@level", level);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // 查询数据
    void QueryData()
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Player";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var level = reader.GetInt32(2);

                        Debug.LogFormat("id: {0}, name: {1}, level: {2}", id, name, level);
                    }
                }
            }
        }
    }
}
