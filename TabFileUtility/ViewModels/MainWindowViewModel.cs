using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabFileUtility.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindowViewModel()
        {
        }

        TaskFactory f = new TaskFactory();

        public string FilenameIn { get; set; } = "";
        public string FilenameOut { get; set; } = "";

        public bool ProcessReady { get; set; } = false;

        private DateTime BaseTime = new DateTime(DateTime.UtcNow.Year, 1, 1);
        public DateTime StartTime { get; set; } = new DateTime(DateTime.UtcNow.Year, 1, 1);
        public bool StartTimeSet = false;
        public DateTime EndTime { get; set; } = new DateTime(DateTime.UtcNow.Year, 1, 1);

        public ObservableCollection<Models.HeaderItem> Headers { get; set; } = new ObservableCollection<Models.HeaderItem>();

        public string Status { get; set; } = "Choose Input File";

        public void Browse()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            var result = dlg.ShowDialog();
            if ((result == DialogResult.OK) && (!string.IsNullOrEmpty(dlg.FileName)) && (System.IO.File.Exists(dlg.FileName)))
            {
                FilenameIn = dlg.FileName;

                var directory = System.IO.Path.GetDirectoryName(dlg.FileName);
                var filename = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                var ext = System.IO.Path.GetExtension(dlg.FileName);

                FilenameOut = directory + System.IO.Path.DirectorySeparatorChar + filename + "_New" + ext;

                StreamReader reader = new StreamReader(FilenameIn);
                var headerTemp = reader.ReadLine();
                var headerItems = headerTemp.Split(new char[] { ';' });
                for (int i = 0; i < headerItems.Length; i++)
                {
                    var Enable = true;
                    if (headerItems[i].ToLower().Contains("latitude") || headerItems[i].ToLower().Contains("longitude"))
                    {
                        Enable = false;
                    }

                    Headers.Add(new Models.HeaderItem()
                    {
                        Name = headerItems[i],
                        Output = Enable,
                    });
                }
                ProcessReady = true;
                Status = "Ready to Process";
            }
        }

        public void Process()
        {
            f.StartNew(() =>
            {
                Status = "Processing";
                StreamReader reader = new StreamReader(FilenameIn);
                StreamWriter writer = new StreamWriter(FilenameOut);

                reader.ReadLine(); //Header

                for (int i = 0; i < Headers.Count; i++) //Write Header
                {
                    if (Headers[i].Output)
                    {
                        writer.Write(Headers[i].Name);
                        writer.Write(";");
                    }
                }
                writer.WriteLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var items = line.Split(new char[] { ';' });

                    if (items.Length >= 2)
                    {
                        if (Double.TryParse(items[1], out double timeTemp))
                        {
                            var ticksTemp = (long)(timeTemp * 10000000);

                            if (!StartTimeSet)
                            {
                                StartTime = BaseTime.AddTicks(ticksTemp);
                                StartTimeSet = true;
                            }
                            else
                            {
                                EndTime = BaseTime.AddTicks(ticksTemp);
                            }
                        }
                    }

                    for (int i = 0; i < Headers.Count; i++) //Write new line
                    {
                        if ((i < line.Length) && (Headers[i].Output))
                        {
                            writer.Write(items[i]);
                            writer.Write(";");
                        }
                    }
                    writer.WriteLine();
                    writer.Flush();
                }
                writer.Close();
                Status = "Processing Complete";
            });
        }
    }
}
