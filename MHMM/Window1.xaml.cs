using MHMM.ConfigFiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MHMM
{
    
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<ModInfoClass> Modlist = new List<ModInfoClass>();
        string[] ids = new setvalues().ids;
        DataTransfer transferDel;
        int selindex = 0;
        public Window1()
        {
            InitializeComponent();
        }
        public Window1(List<ModInfoClass> modlist, DataTransfer transferDel)
        {
            InitializeComponent();
            Modlist = modlist;
            this.transferDel = transferDel;

            for (int i = 0; i < Modlist.Count; i++)
            {
                win2_listBox.Items.Add(Modlist[i].FolderName);
            }
            for (int i = 0; i < ids.Length; i++)
            {
                win2_idcb.Items.Add(ids[i]);
            }

        }

        private void win2_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedIndex >= 0)
            {
                int selectedModIndex = listBox.SelectedIndex;

                win2_modInfo.Content = "Name: "+Modlist[selectedModIndex].ModName+"\n Author: "+ Modlist[selectedModIndex].ModAuthor + "\n Description: "+ Modlist[selectedModIndex].ModName;

                string modId = Modlist[selectedModIndex].ModPaths[0].Split('\\').Last();
                
                for (int j = 0; j < ids.Length; j++)
                {
                    if (new setvalues().idnumber[j] == modId)
                    {
                        Debug.Print("modId " + modId + ":" + new setvalues().idnumber[j]);
                        win2_idcb.SelectedIndex = j;
                        selindex = selectedModIndex;
                        break;
                    }

                }


            }
            
        }
        private void win2_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void win2_add_Click(object sender, RoutedEventArgs e)
        {
            transferDel.Invoke(selindex, win2_idcb.SelectedIndex);
            this.Close();
        }
    }
}
