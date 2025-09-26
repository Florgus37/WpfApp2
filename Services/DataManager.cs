using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp2.Services
{
    public static class DataManager
    {
        private const string FileName = "data.json";

        public static void SaveData(List<Models.TaskItem> tasks, List<Models.NoteItem> notes)
        {
            var data = new { Tasks = tasks, Notes = notes };
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        public static (List<Models.TaskItem>, List<Models.NoteItem>) LoadData()
        {
            if (!File.Exists(FileName))
            {
                return (new List<Models.TaskItem>(), new List<Models.NoteItem>());
            }

            string json = File.ReadAllText(FileName);
            var data = JsonConvert.DeserializeObject<DynamicData>(json);

            return (data.Tasks ?? new List<Models.TaskItem>(), data.Notes ?? new List<Models.NoteItem>());
        }

        private class DynamicData
        {
            public List<Models.TaskItem> Tasks { get; set; }
            public List<Models.NoteItem> Notes { get; set; }
        }
    }
}