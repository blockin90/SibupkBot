namespace UpkServices
{
    public interface IConfigStore
    {
        string GetDataAsString(string name);
        void SetDataAsString(string name, string value);
        int GetDataAsInt32(string name);
        void SetDataAsInt32(string name, int value);
        double GetDataAsDouble(string name);
        void SetDataAsDouble(string name, double value);
        string this[string paramName] { get; set; }
        void SaveChanges();
    }
}
