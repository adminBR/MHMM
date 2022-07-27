using MHMM.ConfigFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MHMM
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    /// 

    public class RootItem
    {
        public RootItem()
        {
            Categories = new ObservableCollection<CatNode>();
            //Categories.Add(new CatNode { Header = "Cat1" });
            //Categories.Add(new CatNode { Header = "Cat2" });
        }
        public string Header { get; set; }
        public int Id { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public ObservableCollection<CatNode> Categories { get; set; }
    }
    public class CatNode
    {
        public CatNode()
        { }
        //public Button btnhead { get; set; }
        public string Header { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        //public RootItem parent  { get; set; }
        public int ParentID { get; set; }
        public int Id { get; set; }
        //public bool IsSelected { get; set; }
    }

    public delegate void DataTransfer(int data,int idsindex);
    public partial class MainWindow : Window
    {
        string MODSSOURCEPATH = "C:\\mhrtest";
        LoadedModsList LML = new LoadedModsList();
        public List<ModInfoClass> AddedMods = new List<ModInfoClass>();
        public List<ModsClass> activedMods = new List<ModsClass>();

        public int lastSelectedRootNodeIndex = -1;
        public int lastSelectedCatNodeIndex = -1;

        public TreeView mtv;
        public ObservableCollection<RootItem> MyRoot { get; set; }

        private void Window_SourceInitialized(object sender, EventArgs ea)
        {
            WindowAspectRatio.Register((Window)sender);
        }

        public DataTransfer transferDelegate;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            transferDelegate += new DataTransfer(DataMethod);
            //ft();
            LoadJson();
            refreshList();
            //MyRoot.Add(new RootItem());



            string idd = "{\"";
            string idn = "{\"";
            setvalues tempsv = new setvalues();
            for (int i = 0; i < tempsv.ids.Length; i++)
            {
                idd += tempsv.ids[i].Split(' ').First() + "\",\"";
                idn += tempsv.ids[i].Remove(0, 8) + "\",\"";
            }
            File.WriteAllText(MODSSOURCEPATH + "\\idd.json", idd);
            File.WriteAllText(MODSSOURCEPATH + "\\idn.json", idn);

        }
        public void refreshList()
        {
            //if (MyRoot != null)
                //MyRoot.Clear();

            MyRoot = new ObservableCollection<RootItem>();
            for (int i = 0; i < LML.armors.Count; i++)
            {
                RootItem temp = new RootItem();
                temp.Header = "" + new setvalues().ids[i];
                temp.Id = i;
                temp.IsSelected = false;
                temp.Categories = new ObservableCollection<CatNode>();
                for (int j = 0; j < LML.armors[i].Mods.Count; j++)
                {
                    //temp.Categories.Add(new CatNode { Header = "" + LML.armors[i].Mods[j].Name, Id = j, ParentID = i, parent = temp });
                    temp.Categories.Add(new CatNode
                    {
                        Header = "" + LML.armors[i].Mods[j].Name,
                        Id = j,
                        ParentID = i
                    });
                }
                MyRoot.Add(temp);
                Debug.Print("teste001");
            }
            
            //mainTreeView.Items.Refresh();
            //mainTreeView.UpdateLayout();

            Debug.Print("teste002");
        }

        public void ft()
        {
            

            LML.armors = new List<LoadedModsClass>();
            for (int i = 0; i < new setvalues().ids.Length; i++)
            {
                LoadedModsClass lmctemp = new LoadedModsClass();
                lmctemp.Name = new setvalues().ids[i].Split('=').Last();
                lmctemp.Id = new setvalues().ids[i].Split('=').First();

                lmctemp.Mods = new List<ModsClass>();
                ModsClass tempmc = new ModsClass();
                tempmc.Name = "nametest";
                tempmc.Author = "authortest";
                tempmc.ID = "000ok";
                tempmc.HeadCheck = 0;
                tempmc.TorsoCheck = 0;
                tempmc.ArmsCheck = 0;
                tempmc.WaistCheck = 0;
                tempmc.LegsCheck = 0;
                lmctemp.Mods.Add(tempmc);
                LML.armors.Add(lmctemp);
            }
            string newjson = JsonConvert.SerializeObject(LML);
            File.WriteAllText(MODSSOURCEPATH + "\\armorList.json",newjson);
        }

        public void LoadJson()
        {
            string fileName = MODSSOURCEPATH+ "\\armorList.json";
            string jsonString = File.ReadAllText(fileName);
            LoadedModsList templist = JsonConvert.DeserializeObject<LoadedModsList>(jsonString);

            testLbl.Content = "teste:" + templist.armors[0].Name;
            LML = templist;
            //testLbl.Content = "teste:" + LML.armors[0].Name;

            // read JSON directly from a file
            /*using (StreamReader file = File.OpenText(@"c:\videogames.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
            }*/
            modInfoReader();
        }

        public void modInfoReader()
        {
            var directories = Directory.GetDirectories(MODSSOURCEPATH+"\\modsfolder");
            for(int i = 0; i< directories.Length; i++)
            {
                string tempdir = directories[i].Split('\\').Last();
                ModInfoClass temp = new ModInfoClass();

                temp.FolderName = tempdir;
                if (File.Exists(directories[i] + "\\modinfo.ini"))
                {
                    try {
                        var dict = File.ReadAllLines(directories[i] + "\\modinfo.ini");
                        // Or specify a specific name in a specific dir
                        for(var j = 0; j < dict.Length; j++)
                        {
                            var splitline = dict[j].Split('=');
                            switch (splitline[0])
                            {
                                case "name":
                                    temp.ModName = splitline[1];
                                    break;
                                case "author":
                                    temp.ModAuthor = splitline[1];
                                    break;
                                case "description":
                                    temp.ModDescription = splitline[1];
                                    break;
                                case "screenshot":
                                    temp.ImagePath = directories[i] +"\\"+ splitline[1];
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("exc" + e);
                    }
                }
                else
                {
                    temp.ModName = "nofile";
                    temp.ModAuthor = "nofile";
                    temp.ModDescription = "nofile";
                }
                temp.IsActive = false;


                temp.ModPaths = new List<string>();
                temp.ExtraModPaths = new List<string>();
                var modsDir = Directory.GetDirectories(directories[i] + "\\natives\\STM\\player\\mod\\f");
                for (int j = 0; j < modsDir.Length; j++)
                {
                    temp.ModPaths.Add(modsDir[j]);
                }
                var extraModsDir = Directory.GetDirectories(directories[i] + "\\natives\\STM\\player\\mod");
                for (int j = 0; j < modsDir.Length; j++)
                {
                    if (extraModsDir[j].Split('\\').Last() != "f")
                        temp.ExtraModPaths.Add(extraModsDir[j]);
                }
                AddedMods.Add(temp);
            }
            /*var dict2 = File.ReadAllLines(MODSSOURCEPATH + "\\test.txt");
            string temp222 = "[";
            for(int i  = 0; i< dict2.Length; i++)
            {
                if (dict2[i].Length>4)
                    temp222 += dict2[i]+"\",\"";
            }

            File.WriteAllText(MODSSOURCEPATH + "\\test2.txt", temp222);*/
            string modslistjson = JsonConvert.SerializeObject(AddedMods);
            File.WriteAllText(MODSSOURCEPATH + "\\modsfolder\\list.json", modslistjson);
        }

        public void updateDesc(string mdName, string mdDesc, string mdImage)
        {
            MDname.Content = mdName;
            MDdesc.Content = mdDesc;
            MDimg.Source = new BitmapImage(new Uri(@""+mdImage, UriKind.RelativeOrAbsolute));
        }


        //makes the tree view node recognize the click to expand anywhere
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            /*tvi.IsExpanded = !tvi.IsExpanded;
            if (tvi == null || e.Handled) return;
            if (!tvi.IsExpanded)
            {
                tvi.IsExpanded = true;
            }
            tvi.IsSelected = false;
            e.Handled = true;*/
            var rootnode = (sender as FrameworkElement).DataContext;
            TreeView tv = (TreeView)sender;

            if (tv.SelectedItem is RootItem)// the function recognizes child nodes selection, like the catnode
            {
                RootItem rit = (RootItem)tv.SelectedItem;
                testLbl2.Content = "" + rit.Header;
                rit.IsSelected = true;
                rit.IsExpanded = tvi.IsExpanded;

               // rti.rootBackImg = "Images/smallbtn.png";
               // TreeViewItem tvi = (TreeViewItem)rootnode;
                /*TreeViewItem tvi = e.OriginalSource as TreeViewItem;
                tvi.ApplyTemplate();
                var imgtemp2 = tvi.FindName("rootBackImg");
                Image imgtemp = imgtemp2 as Image;
                imgtemp.Source = new BitmapImage(new Uri("Images/selsmallbtn.png", UriKind.Relative));*/

            } else if(tv.SelectedItem is CatNode)
            {
                ItemsControl parent = GetSelectedTreeViewItemParent(tvi);
                TreeViewItem treeitem = parent as TreeViewItem;


                RootItem rit = (RootItem)treeitem.DataContext;

                CatNode cnd = (CatNode)tv.SelectedItem;

                testLbl2.Content = "" + rit.Header;
                int armorindex = MyRoot.IndexOf(rit);
                int modindex = MyRoot[MyRoot.IndexOf(rit)].Categories.IndexOf(cnd);
                ModsClass temp = LML.armors[armorindex].Mods[modindex];

                lastSelectedRootNodeIndex = armorindex;
                lastSelectedCatNodeIndex = modindex;

                updateDesc(temp.Name, temp.Author + "\n" + temp.ID,temp.ImagePath);
                Debug.Print("" + temp.ImagePath);
            }
            
            
        }
        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private void Head_Click(object sender, RoutedEventArgs e)
        {

            //get node and parent index
            var btncatnode = (sender as FrameworkElement).DataContext;
            CatNode btnCN = (CatNode)btncatnode;

            int armorindex = btnCN.ParentID;
            int modindex = MyRoot[armorindex].Categories.IndexOf(btnCN);
            //testLbl.Content = "" + btnCN.ParentID + "/" + btnCN.Id + "/" + index;
            //armorindex = btnCN.ParentID;
            //modindex = btnCN.Id;

            //change image
            Button selbtn = (Button)sender;
            Image btnimage = (Image)selbtn.FindName("image"+selbtn.Name);

            Debug.WriteLine("Images/" + selbtn.Name + ".png" + armorindex + " / " + modindex);
            var image = new BitmapImage(new Uri("Images/armoricons/green/"+selbtn.Name+".png", UriKind.Relative));//usa o nome do botao para saber qual imagem usar
            btnimage.Source = image;



            //falta agora interagir com o json e fazer a mudança a partir do valor no json para ter certeza que a mudança na tela realmente aconteceu nos arquivos



            // TreeView temp = obj.Paren
        }
        public void DataMethod(int data, int idsindex)
        {
            Debug.WriteLine("backmessage!!"+data+" "+idsindex);

            ModsClass templmc = new ModsClass();
            templmc.ImagePath = AddedMods[data].ImagePath;
            templmc.Name = AddedMods[data].ModName;
            templmc.Author = AddedMods[data].ModAuthor;
            templmc.ID = AddedMods[data].ModPaths[0].Split('\\').Last() + " ";
            templmc.HeadCheck = 0;
            templmc.TorsoCheck = 0;
            templmc.ArmsCheck = 0;
            templmc.WaistCheck = 0;
            templmc.LegsCheck = 0;
            LML.armors[idsindex].Mods.Add(templmc);

            MyRoot[idsindex].Categories.Add(new CatNode
            {
                Header = "" + LML.armors[idsindex].Mods.Last().Name,
                ParentID = idsindex,
                Id = MyRoot[idsindex].Categories.Count
            });
            Debug.Print(""+MyRoot.Count+" "+idsindex+" "+templmc);
            // refreshList();
            // Do what you want with your data.
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Window1 win1 = new Window1(AddedMods,transferDelegate);
            win1.Show();

            
        }

        private void btnRm_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print(lastSelectedRootNodeIndex + " " + lastSelectedCatNodeIndex + "   " + MyRoot.Count + " " + MyRoot[lastSelectedRootNodeIndex].Categories.Count);
            MyRoot[lastSelectedRootNodeIndex].Categories.RemoveAt(lastSelectedCatNodeIndex);
            //LML.armors[armorindex].Mods[modindex];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            refreshList();
        }
    }
}
