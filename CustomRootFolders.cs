using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    [Serializable]
   public class CustomRootFolders
    {
        public List<string> FolderNames { get; set; }
        [NonSerialized]
        public string ActiveFolderPath;

        public CustomRootFolders(string activefolderpath)
        {
            this.ActiveFolderPath = activefolderpath;
            Load();
        }

        private void Load()
        {
            if (File.Exists(Path.Combine(ActiveFolderPath, "CustomRootFolders")))
            {
                CustomRootFolders fs = SD.Serialization.LoadObject<CustomRootFolders>(Path.Combine(ActiveFolderPath, "CustomRootFolders"));
                this.FolderNames = fs.FolderNames;
            }
            else
            {
                this.FolderNames = new List<string>();
            }
        }

        public bool AddFolderName(string name)
        {
            if (name.ToLower() == "rootnode" || name.ToLower() == "downloads" || name.ToLower() == "favorite" || name.ToLower() == "recyclebin")
                return false;
            foreach (string s in FolderNames)
            {
                if (s.ToLower() == name.ToLower())
                    return false;
            }
            FolderNames.Add(name);
            save();
            return true;
        }
        public bool RemoveFolder(string name,string Activefolder)
        {
            this.ActiveFolderPath = Activefolder;
            if(FolderNames.Contains(name))
            {
                Node n = new Node(ActiveFolderPath, name, "", name);
                n.Read();
                if (n.Nodes.Count > 0 || n.Entities.Count > 0)
                {
                    MessageBox.Show("Folder is not empty!");
                    return false;
                }
                else
                {
                    if (n.DeleteNode(ActiveFolderPath))
                    {
                        FolderNames.Remove(name);
                        save();
                        return true;
                    }
                    return false;
                }
            }
            return false;
                
        }

        private void save()
        {
            SD.Serialization.SaveObject<CustomRootFolders>(Path.Combine(ActiveFolderPath, "CustomRootFolders"), this);
        }
    }
}
