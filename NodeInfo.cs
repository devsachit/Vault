using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
  //  [Serializable]
  //public  class NodeInfo
  //  {
  //      public string ParentNode;
  //      public List<Node> Nodes;
  //      public List<Entity> Entities;

  //      [NonSerialized]
  //      private string ActivefolderPath;
  //      [NonSerialized]
  //      private string Name;
  //      [NonSerialized]
  //      public bool IsLoaded = false;

  //      public NodeInfo(string activeFolderPath)
  //      {
  //          this.ActivefolderPath = activeFolderPath;
  //          Nodes = new List<Node>();
  //          Entities = new List<Entity>();
  //          IsLoaded = false;
  //      }
  //      public void SetName(string name)

  //      {
  //          this.Name = name;
  //      }

  //      internal bool Read(bool loadthumbs=false)
  //      {
  //          try
  //          {
  //              if (File.Exists(Path.Combine(ActivefolderPath, Name)))
  //              {
  //                  NodeInfo info = SD.Serialization.LoadObject<NodeInfo>(Path.Combine(ActivefolderPath, Name));
  //                  this.Entities = info.Entities;
  //                  this.Nodes = info.Nodes;
  //                 // this.ParentNode = info.ParentNode;
  //                  LoadInitialThumbnails();
  //              }
  //              else
  //              {
  //                  this.Entities = new List<Entity>();
  //                  this.Nodes = new List<Node>();
  //                  ParentNode = null;
  //              }
  //              IsLoaded = true;

  //              if (loadthumbs)
  //                  ReadAndLoadThumbs();

  //              return true;
  //          }
  //          catch (Exception ee)
  //          {
  //              MessageBox.Show(ee.Message, "Stil working....");
  //              return false;
  //          }
  //      }

      

  //      internal void Add(Node n)
  //      {
  //          Nodes.Add(n);
  //      }
  //      internal void Add(Entity entity)
  //      {
  //          Entities.Add(entity);
  //      }

      

  //      private void LoadInitialThumbnails()
  //      {
  //          for (int i = 0; i < Entities.Count; i++)
  //          {
  //              Entities[i].LoadInitialPicture(ActivefolderPath);
  //          }
  //      }

  //      void ReadAndLoadThumbs()
  //      {
  //          if (IsLoaded == false)
  //              Read();

  //          if (this.Entities.Count > 0)
  //          {
  //              BackgroundWorker bg_loadthumb = new BackgroundWorker();
  //              bg_loadthumb.DoWork += Bg_loadthumb_DoWork;
  //              bg_loadthumb.RunWorkerCompleted += Bg_loadthumb_RunWorkerCompleted;
  //              bg_loadthumb.RunWorkerAsync();
  //          }
  //      }


  //      private void Bg_loadthumb_DoWork(object sender, DoWorkEventArgs e)
  //      {
  //          LoadEntityThumbnails();

  //      }
  //      private void LoadEntityThumbnails()
  //      {
  //          for (int i = 0; i < Entities.Count; i++)
  //          {
  //              Entities[i].LoadPicture(ActivefolderPath);
  //          }
  //      }
  //      private void Bg_loadthumb_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  //      {
  //          GlobalClass.Invalidate();
  //      }

  //      public void Save()
  //      {
  //          if (!Directory.Exists(ActivefolderPath))
  //              Directory.CreateDirectory(ActivefolderPath);

  //          SD.Serialization.SaveObject<NodeInfo>(Path.Combine(ActivefolderPath, Name), this);
  //      }
  //  }
}
