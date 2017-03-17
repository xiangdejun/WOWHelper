using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WOWHelperWPF.Models;
using MahApps.Metro.Controls;


namespace WOWHelperWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Hero> heros;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            heros = new List<Hero>();
        }

        string GetHtml(string url)
        {
            var request = HttpWebRequest.CreateHttp(url); // WebRequest.Create(url);
            request.Proxy = HttpWebRequest.GetSystemWebProxy();
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            var strHTML = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            response.Close();
            return strHTML;
        }

        List<Reputaion> GetHeroReputaion(string html)
        {
            var repGroupList = new List<Reputaion>();

            var strReputation = String.Empty;
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//ul");
            foreach (HtmlNode link in collection)
            {
                if (link.Attributes["class"].Value == "reputation-list")
                {
                    strReputation = link.OuterHtml;

                    break;
                }
            }

            document.LoadHtml(strReputation);
            collection = document.DocumentNode.SelectNodes("//li");
            foreach (HtmlNode node in collection)
            {
                if (node.Attributes["class"].Value == "reputation-category")
                {
                    var h3 = node.ChildNodes["h3"];
                    var repGroupName = h3.InnerText.Trim();
                    var repGroup = new Reputaion { Name = repGroupName, ShowProgress = false };

                    var liCollections = node.ChildNodes["ul"].SelectNodes("li");
                    foreach (HtmlNode li in liCollections)
                    {
                        if (li.Attributes["class"].Value == "faction-details")
                        {
                            var rep = GetSingleReputaion(li.OuterHtml);
                            repGroup.RepList.Add(rep);
                        }
                        else if (li.Attributes["class"].Value == "reputation-subcategory")
                        {
                            var subGroup = GetSubCategoryReputation(li.OuterHtml);
                            repGroup.RepList.Add(subGroup);
                        }
                    }
                    repGroupList.Add(repGroup);
                }

            }

            return repGroupList;
        }

        Reputaion GetSubCategoryReputation(string html)
        {

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var h4 = document.DocumentNode.SelectSingleNode("//h4");
            var repGroupName = h4.InnerText.Trim();
            var repGroup = new Reputaion { Name = repGroupName };
            var divScore = document.DocumentNode.SelectSingleNode("//div[@class='faction-score']");
            var repScore = divScore.InnerText.Trim();
            repGroup.Score = repScore;
            var divLevel = document.DocumentNode.SelectSingleNode("//div[@class='faction-level']");
            var repLevel = divLevel.InnerText.Trim();
            repGroup.Level = repLevel;
            var liCollections = document.DocumentNode.ChildNodes[0].ChildNodes["ul"].SelectNodes("li");
            foreach (HtmlNode li in liCollections)
            {
                if (li.Attributes["class"].Value == "faction-details")
                {
                    var rep = GetSingleReputaion(li.OuterHtml);
                    repGroup.RepList.Add(rep);
                }
            }

            return repGroup;

        }

        Reputaion GetSingleReputaion(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);


            var span = document.DocumentNode.SelectSingleNode("//span[@class='faction-name']"); //node.SelectNodes("//*[@class='faction-name']");
            var repName = span.InnerText.Trim();

            var divScore = document.DocumentNode.SelectSingleNode("//div[@class='faction-score']");
            var repScore = divScore.InnerText.Trim();

            var divLevel = document.DocumentNode.SelectSingleNode("//div[@class='faction-level']");
            var repLevel = divLevel.InnerText.Trim();

            return new Reputaion
            {
                Name = repName,
                Score = repScore,
                Level = repLevel
            };
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            AddHero(txtHeroName.Text.Trim(), txtServerName.Text.Trim());

            //AddHero("别怕有我", "阿卡玛");


            //PrepareData("http://www.battlenet.com.cn/wow/zh/character/阿卡玛/别怕有我/reputation/", TvRep, TvHeader, "别怕有我");
            //PrepareData("http://www.battlenet.com.cn/wow/zh/character/阿卡玛/伴你左右/reputation/", TvRep1, TvHeader1, "伴你左右");
            //PrepareData("http://www.battlenet.com.cn/wow/zh/character/阿卡玛/像个男人/reputation/", TvRep2, TvHeader2, "像个男人");
            //PrepareData("http://www.battlenet.com.cn/wow/zh/character/阿卡玛/伊人归来/reputation/", TvRep3, TvHeader3, "伊人归来");
        }

        void PrepareData(string url, TreeView tv, TextBlock tb, string header)
        {
            var html = GetHtml(url);

            var repList = GetHeroReputaion(html);

            tv.ItemsSource = repList;

            tb.Text = header;
        }

        void AddHero(string strHeroName, string strServerName)
        {
            var hero = new Hero { Name = strHeroName, ServerName = strServerName };

            var url = GetUrl(strHeroName, strServerName);

            var html = GetHtml(url);

            File.WriteAllText("HtmlSource", html);

            var repList = GetHeroReputaion(html);

            TreeView tr = new TreeView();
            tr.ItemTemplate = (HierarchicalDataTemplate)FindResource("TreeTemplate");
            tr.ItemsSource = repList;

            this.RepuPanel.Children.Add(tr);

            hero.Reputations = repList;

            DataPersistence.ModelToJson.SaveToLocal(hero);

            heros.Add(hero);
        }

        string GetUrl(string strHeroName, string strServerName)
        {
            return string.Format("http://www.battlenet.com.cn/wow/zh/character/{0}/{1}/reputation/", strServerName, strHeroName);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (var hero in heros)
            {
                DataPersistence.ModelToJson.SaveToLocal(hero);
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var hero = DataPersistence.ModelToJson.LoadJson("别怕有我");
            LoadHero(hero);
            this.heros.Add(hero);
        }

        void LoadHero(Hero hero)
        {
            TreeView tr = new TreeView();
            tr.ItemTemplate = (HierarchicalDataTemplate)FindResource("TreeTemplate");
            tr.ItemsSource = hero.Reputations;

            this.RepuPanel.Children.Add(tr);
        }

        private void BtnLoadHtml_Click(object sender, RoutedEventArgs e)
        {

            var hero = new Hero { Name = txtHeroName.Text.Trim() };

            var html = File.ReadAllText("HtmlSource");

            var repList = GetHeroReputaion(html);

            TreeView tr = new TreeView();
            tr.ItemTemplate = (HierarchicalDataTemplate)FindResource("TreeTemplate");
            tr.ItemsSource = repList;

            this.RepuPanel.Children.Add(tr);

            hero.Reputations = repList;

            DataPersistence.ModelToJson.SaveToLocal(hero);

            heros.Add(hero);
        }


    }
}
