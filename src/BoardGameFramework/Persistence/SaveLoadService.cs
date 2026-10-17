using System.IO;
using System.Text.Json;

namespace BoardGameFramework.Persistence
{
    public static class SaveLoadService
    {
        public static void Save(GameSnapshot snapshot, string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(snapshot, options);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(json);
            }
        }

        public static GameSnapshot Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Save file '" + filePath + "' was not found.");

            string json;
            using (StreamReader reader = new StreamReader(filePath))
            {
                json = reader.ReadToEnd();
            }

            GameSnapshot snapshot = JsonSerializer.Deserialize<GameSnapshot>(json);
            if (snapshot == null)
                throw new IOException("The save file is empty or corrupted.");
            return snapshot;
        }
    }
}
