using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    [Serializable]
    public class Tasks
    {
        List<TaskInfo> remainTasks;
        [NonSerialized]
        string Activefolderpath;
        [NonSerialized]
        BackgroundWorker worker;
        [NonSerialized]
        bool CanExit = false;
        [NonSerialized]
        bool HaltAction = false;
        public Tasks(string ActiveFolderPath)
        {

            this.Activefolderpath = ActiveFolderPath;
            remainTasks = new List<TaskInfo>();
            GlobalClass.ProgramExiting += GlobalClass_ProgramExiting;
            Load();

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            Execute();


        }

        private void GlobalClass_ProgramExiting(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                HaltAction = true;
                while (CanExit == false) ;
            }
        }

        internal void Execute()
        {
            if (worker.IsBusy == false)
                worker.RunWorkerAsync();
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GlobalClass.Invalidate();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartTask();
        }
       
        private void StartTask()
        {
            CanExit = false;
            for (int i = 0; i < remainTasks.Count; i++)
            {
               
                if (remainTasks[i].type == TaskInfo.TaskType.AddNode)
                {
                    if (Controller.AddNode(remainTasks[i].ActiveNode, remainTasks[i].NodeToAdd.DisplayName, remainTasks[i].NodeToAdd.Name, remainTasks[i].ActiveFolder))
                    {
                        remainTasks.RemoveAt(i);
                        //  worker.ReportProgress(i);
                        i--;
                    }
                }
                else if (remainTasks[i].type == TaskInfo.TaskType.AddFile)
                {
                    if (Controller.AddEntity(remainTasks[i].ActiveNode, remainTasks[i].FileToAdd, remainTasks[i].ActiveFolder))
                        {
                        remainTasks.RemoveAt(i);
                        //     worker.ReportProgress(i);
                        i--;
                    }
                }

                if (HaltAction)
                {
                    break;
                }
            }
            Save();
            Controller.SaveLastParentNodeUpdated();
            CanExit = true;
        }

       
        public void AddTask(TaskInfo info)
        {
            remainTasks.Add(info);
        }
        public void Load()
        {
            if (File.Exists(Path.Combine(Activefolderpath, "RemainingTasks")))
            {
                Tasks tasks = SD.Serialization.LoadObject<Tasks>(Path.Combine(Activefolderpath, "RemainingTasks"));
                this.remainTasks = tasks.remainTasks;
            }
            else
            {
                remainTasks = new List<TaskInfo>();
            }
        }
        public void Save()
        {
            SD.Serialization.SaveObject<Tasks>(Path.Combine(Activefolderpath, "RemainingTasks"), this);
        }

    }

    [Serializable]
    public class TaskInfo
    {
        public string ActiveNode { get; set; }
        public string FileToAdd { get; set; }
        public Node NodeToAdd { get; set; }
        public String ActiveFolder { get; set; }
        public enum TaskType
        {
            AddFile,
            AddNode,
        }
        public TaskType type = TaskType.AddFile;

        public TaskInfo(Node activenode, string filetoAdd,string Activefolder)
        {
            this.ActiveNode = activenode.Name;
            this.FileToAdd = filetoAdd;
            type = TaskType.AddFile;
            this.ActiveFolder = Activefolder;
        }
        public TaskInfo(Node activenode, Node nodetoadd, string Activefolder)
        {
            this.ActiveNode = activenode.Name;
            this.NodeToAdd = nodetoadd;
            type = TaskType.AddNode;
            this.ActiveFolder = Activefolder;
        }

    }
}
