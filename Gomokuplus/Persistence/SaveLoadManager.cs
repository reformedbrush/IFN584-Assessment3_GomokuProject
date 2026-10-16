using System;
using System.IO;
using System.Text.Json;

namespace GomokuPlus.Persistence
{
    // Handles the actual file I/O using JsonSerializer with File.WriteAllText
    // and File.ReadAllText, exactly the pattern shown for JSON serialisation
    // in the file-operations lecture.
    public class SaveLoadManager
    {
        public static void Save(GameStateData data, string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static GameStateData Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Save file '" + filePath + "' was not found.");

            string jsonStringRead = File.ReadAllText(filePath);
            GameStateData loadedData = JsonSerializer.Deserialize<GameStateData>(jsonStringRead);

            if (loadedData == null)
                throw new IOException("The save file is empty or corrupted.");

            return loadedData;
        }
    }
}
