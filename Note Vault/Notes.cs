using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Note_Vault
{
    [Serializable]
    public class Notes
    {
        public List<WriteEntity> Entities { get; set; }
        
        public Notes(string activepath)
        {
            LoadWriteEntities(activepath);
        }

        private void LoadWriteEntities(string activepath)
        {
          string  ActiveWriterPath = Path.Combine(activepath, "Writer");
            string path = Path.Combine(ActiveWriterPath, "Notes");
            if (File.Exists(path))
                this.Entities = SD.Serialization.LoadObject<Notes>(path).Entities;
            else
                this.Entities = new List<WriteEntity>();
        }



        public void Save(string activepath)
        {
            string ActiveWriterPath = Path.Combine(activepath, "Writer");
            if (!Directory.Exists(ActiveWriterPath))
                Directory.CreateDirectory(ActiveWriterPath);
            string path = Path.Combine(ActiveWriterPath, "Notes");
            SD.Serialization.SaveObject<Notes>(path, this);
        }
        public Note AddNewNote(string activepath)
        {
            WriteEntity entity = new WriteEntity();
            this.Entities.Add(entity);
            return entity.CreateNewNote(activepath);
        }
        public Note GetNoteofIndex(int index,string activepath)
        {
            if (Entities == null || index > Entities.Count)
                return null;
            return Entities[index].GetNote(activepath);
        }
        internal Note GetLastNote(string activepath)
        {
            if (Entities.Count == 0)
                return null;
            else
                return Entities[Entities.Count - 1].GetNote(activepath);
        }

        internal void Delete(int selectedIndex,string activepath)
        {
            if (selectedIndex != -1)
            {
                Entities[selectedIndex].DeleteNote();
                Entities.RemoveAt(selectedIndex);

                Save(activepath);
            }


        }
    }

    [Serializable]
    public class WriteEntity
    {
        public string EntityName { get; set; }
        public String DisplayName { get; set; }
        [NonSerialized]
        public Note note;
        public WriteEntity()
        {
            EntityName = "WriteEntity" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Hour + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year;
            DisplayName = "New Note";
        }
        public void LoadNote(string activepath)
        {

            if (note == null)
            {
                note = new Note(EntityName,activepath);
                note.Load();
            }
        }
        public void SaveNote(string activepath)
        {
            if (note != null)
                note.Save(activepath);
        }

        internal Note CreateNewNote(string activepath)
        {
            note = new Note(EntityName,activepath);
            return note;

        }

        internal Note GetNote(string activepath)
        {
            LoadNote(activepath);
            return note;
        }

        internal void DeleteNote()
        {
            if (note != null)
            {
                note.Delete();
            }
        }
    }
    [Serializable]
    public class Note
    {
        string Title { get; set; }
        string Content { get; set; }
        public DateTime dateTime { get; set; }

        [NonSerialized]
        string path;
        public Note(string EntityName,string activepath)
        {
            string ActiveWriterPath = Path.Combine(activepath, "Writer");
            dateTime = DateTime.Now;
            Title = "New Title";
            Content = "";

            path = Path.Combine(ActiveWriterPath, EntityName);
        }
        public void UpdateNote(string title, String content)
        {
            this.Title = SD.Encryption.Crypto.EncryptStringAES(title, "loca#.-+@SD321");
            this.Content = SD.Encryption.Crypto.EncryptStringAES(content, "loca#.-+@SD321");
        }
        public string GetPlainTitle()
        {

            if (this.Title == "")
                return "Create New Title";
            return SD.Encryption.Crypto.DecryptStringAES(this.Title, "loca#.-+@SD321");

        }
        public string GetPlainContent()
        {
            if (this.Content == "")
                return "Create New Content";
            return SD.Encryption.Crypto.DecryptStringAES(this.Content, "loca#.-+@SD321");
        }

        internal void Load()
        {

            if (File.Exists(path))
            {
                Note n = SD.Serialization.LoadObject<Note>(path);
                this.Title = n.Title;
                this.Content = n.Content;
                this.dateTime = n.dateTime;
            }
            else
            {
                Title = "No Note Found";
                Content = "No content Found";
                dateTime = DateTime.Now;
            }
        }

        internal void Save(string activepath)
        {
            string ActiveWriterPath = Path.Combine(activepath, "Writer");
            if (!Directory.Exists(ActiveWriterPath))
                Directory.CreateDirectory(ActiveWriterPath);

            if (path != null && path != "")
                SD.Serialization.SaveObject<Note>(path, this);
        }

        internal void Delete()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
