using System;
using UpkModel.Database;
using UpkModel.Database.Schedule;

namespace UpkServices
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    public class DbStoredConfigs : IConfigStore
    {
        public DbStoredConfigs(UpkDatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        UpkDatabaseContext DatabaseContext { get; }

        public string GetDataAsString(string name)
        {
            return DatabaseContext.Configs.Find(name)?.Value;
        }
        public void SetDataAsString(string name, string value)
        {
            var config = DatabaseContext.Configs.Find(name);
            if (config == null) {
                config = new Config() { Key = name, Value = value };
                DatabaseContext.Configs.Add(config);
            } else {
                config.Value = value;
                DatabaseContext.Update(config);
            }
        }

        public int GetDataAsInt32(string name)
        {
            int result = 0;
            Int32.TryParse(DatabaseContext.Configs.Find(name)?.Value, out result);
            return result;
        }

        public void SetDataAsInt32(string name, int value)
        {
            var config = DatabaseContext.Configs.Find(name);
            if (config == null) {
                config = new Config() { Key = name, Value = value.ToString() };
                DatabaseContext.Configs.Add(config);
            } else {
                config.Value = value.ToString();
                DatabaseContext.Update(config);
            }
        }

        public double GetDataAsDouble(string name)
        {
            double result = 0;
            double.TryParse(DatabaseContext.Configs.Find(name)?.Value, out result);
            return result;
        }

        public void SetDataAsDouble(string name, double value)
        {
            var config = DatabaseContext.Configs.Find(name);
            if (config == null) {
                config = new Config() { Key = name, Value = value.ToString() };
                DatabaseContext.Configs.Add(config);
            } else {
                config.Value = value.ToString();
                DatabaseContext.Update(config);
            }
        }

        public string this[string paramName]
        {
            get => GetDataAsString(paramName);
            set => SetDataAsString(paramName, value);
        }

        public void SaveChanges()
        {
            DatabaseContext.SaveChanges();
        }
    }
}
