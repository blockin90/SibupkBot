using System;
using System.Collections.Generic;
using System.Text;
using UpkModel.Database;

namespace UpkServices
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    public class Configs
    {
        protected Configs()
        {
        }

        public int MaxAttempts { get; } = 5;
        public int SleepInterval { get; } = 100;

        private static Configs _configs;

        public static Configs Instance
        {
            get { return _configs ?? (_configs = new Configs()); }
        }
        public string GetData(string name)
        {
            return UpkDatabaseContext.Instance.Configs.Find(name)?.Value;
        }
        public void SetData(string name, string value)
        {
            var config = UpkDatabaseContext.Instance.Configs.Find(name);
            if (config == null) {
                config = new Config() { Key = name, Value = value };
                UpkDatabaseContext.Instance.Configs.Add(config);
            } else {
                config.Value = value;
                UpkDatabaseContext.Instance.Update(config);
            }
        }
        public string this[string paramName]
        {
            get => GetData(paramName);
            set => SetData(paramName, value);
        }
        public void SaveChanges()
        {
            UpkDatabaseContext.Instance.SaveChanges();
        }
    }
}
