using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class CollisionLogger : MonoBehaviour
{
    public GameObject referenceObject;
    private string connectionString;

    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/test.db";
        CreateDatabaseTable();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 distance = transform.position - referenceObject.transform.position;
        Vector3 rotation = transform.rotation.eulerAngles - referenceObject.transform.rotation.eulerAngles;

        string originalObjectName = gameObject.name;
        string collidedObjectName = collision.gameObject.name;

        InsertCollisionData(originalObjectName, collidedObjectName, distance, rotation);

        Destroy(GetComponent<Collider>());
    }

    void CreateDatabaseTable()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sql = "CREATE TABLE IF NOT EXISTS collisionData (id INTEGER PRIMARY KEY AUTOINCREMENT, originalObjectName TEXT, collidedObjectName TEXT, distanceX REAL, distanceY REAL, distanceZ REAL, rotationX REAL, rotationY REAL, rotationZ REAL)";
                dbCmd.CommandText = sql;
                dbCmd.ExecuteNonQuery();
            }
        }
    }

    void InsertCollisionData(string originalObjectName, string collidedObjectName, Vector3 distance, Vector3 rotation)
    {
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sql = "INSERT INTO collisionData (originalObjectName, collidedObjectName, distanceX, distanceY, distanceZ, rotationX, rotationY, rotationZ) VALUES (@originalObjectName, @collidedObjectName, @distanceX, @distanceY, @distanceZ, @rotationX, @rotationY, @rotationZ)";

                dbCmd.Parameters.Add(new SqliteParameter("@originalObjectName", originalObjectName));
                dbCmd.Parameters.Add(new SqliteParameter("@collidedObjectName", collidedObjectName));
                dbCmd.Parameters.Add(new SqliteParameter("@distanceX", distance.x));
                dbCmd.Parameters.Add(new SqliteParameter("@distanceY", distance.y));
                dbCmd.Parameters.Add(new SqliteParameter("@distanceZ", distance.z));
                dbCmd.Parameters.Add(new SqliteParameter("@rotationX", rotation.x));
                dbCmd.Parameters.Add(new SqliteParameter("@rotationY", rotation.y));
                dbCmd.Parameters.Add(new SqliteParameter("@rotationZ", rotation.z));

                dbCmd.CommandText = sql;
                dbCmd.ExecuteNonQuery();
            }
        }
    }
}
