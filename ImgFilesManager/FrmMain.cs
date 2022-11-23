using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LiteDB;
using Numeria.IO;

namespace ImgFilesManager
{
    public partial class FrmMain : Form
    {
        BindingSource bsOrg = new BindingSource();
        DataTable dtOrg = new DataTable();
        DataTable dtFilter = new DataTable();

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            String tmp1 = PNoGenerator("");
            //Console.WriteLine(tmp1);

            String tmp2 = PNoGenerator("2022-11-22_000001");

            //Console.WriteLine(tmp2);

            //TODO : ������ �������� �����̸� �����ؼ� filedb�� ����

            //TODO : xml �±� ��Ģ pNo, Dsec, fID, fName

        }


        //Done : �������� ��Ģ : YYYY-MM-DD_###### ����� �� ���� �޾ƿ��� ���� ������ ������ +1�� ������Ű��
        // ����
        // String tmp1 = PNoGenerator("");
        // //Console.WriteLine(tmp1);
        //
        // String tmp2 = PNoGenerator("2022-11-22_000001");
        // //Console.WriteLine(tmp2);
        public String PNoGenerator(String str)
        {
            String result = "";
            try
            {
                if (str.Length <= 0)
                {
                    //���� ������ ������ �����Ƿ� ���λ�����.
                    //DONE : ����� �޾ƿ���
                    String ymd = DateTime.Now.ToString("yyyy-MM-dd"); // ��ó : https://developer-talk.tistory.com/147
                    int i = 1;
                    String numStr = String.Format("{0:D6}", i); //DONE : ���ڸ� ###### �� ��ĭ��0���� ä���� ǥ���ϱ�

                    result = ymd + @"_" + numStr;
                }
                else
                {
                    //���� ����Ͽ��� ���� ���ڸ� ������Ŵ
                    string[] words = str.Split('_');  // ��ó : https://gent.tistory.com/502
                    String ymd = words[0];
                    int i = Convert.ToInt32(words[1]); // ���ڿ� ���ڸ� ���ڷ�  // ��ó : https://mainia.tistory.com/304
                    i++;
                    String numStr = String.Format("{0:D6}", i); //DONE : ���ڸ� ###### �� ��ĭ��0���� ä���� ǥ���ϱ�
                    result = ymd + @"_" + numStr;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }

            return result;
        }

        private void BtnCreateDB_Click(object sender, EventArgs e)
        {
            string fpath = Application.StartupPath;
            if (radioButton1.Checked == true) //XML
            {
                fpath += @"\myImgXMLdb_v1.xml";
                //------------------------
                // xml ���� 
                XElement doc = new XElement("IMGLIST", new XElement("IMG_INFO",
               new XElement("pNo", PNoGenerator(""))
               , new XElement("Desc", "Test")
               , new XElement("img", "")
               , new XElement("img_id", "")
               ));
                doc.Save(fpath);

                TxtBoxFilePath0.Text = fpath;

            }
            if (radioButton2.Checked == true) //SQLite
            {
                fpath += @"\myImgSQLitedb.db";


            }
            if (radioButton3.Checked == true) //LiteDB
            {
                fpath += @"\myImgLitedb.db";

                // Open database (or create if doesn't exist)
                using (var db = new LiteDatabase(fpath))//@"C:\Temp\MyData.db"
                {
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<pNoItem>("pNoItems");

                    // Create your new customer instance
                    // Creates an OrderedItem.
                    pNoItem i1 = new pNoItem();
                    i1.pNo = PNoGenerator("");
                    i1.Desc = "Test";
                    i1.img = "";//"ADN-433.jpg";
                    i1.img_id = "";

                    // Insert new customer document (Id will be auto-incremented)
                    col.Insert(i1);

                    TxtBoxFilePath0.Text = fpath;

                }

            }

        }

        private void BtnFilePath0_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true) //XML
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    Title = "XML ����"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    TxtBoxFilePath0.Text = ofd.FileName;

            }
            if (radioButton2.Checked == true) //SQLite
            {

            }
            if (radioButton3.Checked == true) //LiteDB
            {
                //myLitedb.db
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "db files (*.db)|*.db|All files (*.*)|*.*",
                    Title = "db ����"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    TxtBoxFilePath0.Text = ofd.FileName;


            }


        }

        private void BtnDBLoad_Click(object sender, EventArgs e)
        {
            if (TxtBoxFilePath0.Text.Trim() == "")
            {
                MessageBox.Show("DB���� ��ΰ� �Էµ��� �ʾҽ��ϴ�. ", "���");

                return;
            }

            //Application.StartupPath + @"\MyFilesDB.fdb";
            TxtDBpath.Text = TxtBoxFilePath0.Text;
            TxtFileDBPath.Text = Path.GetDirectoryName(TxtDBpath.Text) + @"\MyFilesDB.fdb";


            Stopwatch watch = new Stopwatch(); //
            watch.Start();

            if (radioButton1.Checked == true) //XML
            {
                if (File.Exists(TxtBoxFilePath0.Text))
                {
                    try
                    {

                        //��ó : https://hengs.tistory.com/72
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(TxtBoxFilePath0.Text);

                        XmlNodeList nodes = xdoc.SelectNodes("/IMGLIST/IMG_INFO");

                        DataRow dtRow = null;
                        // Define the columns and their names
                        dtOrg = new DataTable();
                        dtOrg.Columns.Add("pNo", typeof(string));
                        dtOrg.Columns.Add("Desc", typeof(string));
                        dtOrg.Columns.Add("img", typeof(string));
                        dtOrg.Columns.Add("img_id", typeof(string));
                        foreach (XmlNode pnode in nodes)
                        {
                            dtRow = dtOrg.NewRow();
                            dtRow["pNo"] = pnode.SelectSingleNode("pNo").InnerText;
                            dtRow["Desc"] = pnode.SelectSingleNode("Desc").InnerText;
                            dtRow["img"] = pnode.SelectSingleNode("img").InnerText;
                            dtRow["img_id"] = pnode.SelectSingleNode("img_id").InnerText;
                            dtOrg.Rows.Add(dtRow);
                        }

                        mainDGV.Columns.Clear(); //������ �÷������� ��ó : https://jw0652.tistory.com/9
                        mainDGV.DataSource = dtOrg;
                        mainDGV.Update();


                        // �������� ����ȭ ��Ű��
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("XML DB ���� �ε� ���� : " + TxtBoxFilePath0.Text);
                    }

                }
            }
            if (radioButton2.Checked == true) //SQLite
            {

            }
            if (radioButton3.Checked == true) //LigtDB
            {
                if (File.Exists(TxtBoxFilePath0.Text))
                {
                    try
                    {
                        //nodes.Count 
                        //mainDGV.Rows.Clear();
                        mainDGV.Columns.Clear(); //������ �÷������� ��ó : https://jw0652.tistory.com/9
                        //int iRow = 0;

                        // Open database (or create if doesn't exist)
                        using (var db = new LiteDatabase(TxtBoxFilePath0.Text))
                        {
                            // Get a collection (or create, if doesn't exist)
                            var collection = db.GetCollection<pNoItem>("pNoItems");

                            BindingList<pNoItem> doclist = new BindingList<pNoItem>();

                            foreach (var deger in collection.FindAll())
                            {
                                doclist.Add(deger);
                                //string[] row1 = new string[] { deger.deger1.ToString() };
                                //dataGridView1.Rows.Add(row1);
                                Application.DoEvents();
                            }
                            mainDGV.DataSource = doclist;
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("XML DB ���� �ε� ���� : " + TxtBoxFilePath0.Text);
                    }

                }


            }

            watch.Stop();

            //�� Rows �� ����
            toolStripStatusLabel1.Text = "Total Rows :";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel2.Text = $"{mainDGV.RowCount}";//$"{num}/{selectedAlignments.Count}";

            toolStripStatusLabel3.Text = "�ҿ�ð�:";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel4.Text = $"{watch.ElapsedMilliseconds}ms";//$"{num}/{selectedAlignments.Count}";

            Application.DoEvents();

        }

        private void mainDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ControlDisp(e.RowIndex);
        }

        private void mainDGV_KeyUp(object sender, KeyEventArgs e)
        {
            int iColumn = mainDGV.CurrentCell.ColumnIndex;
            int iRow = mainDGV.CurrentCell.RowIndex;

            ControlDisp(iRow);

        }

        /// <summary>
        /// DataGridView �� ��Ʈ�� �����ؼ� ǥ��
        /// </summary>
        /// <param name="iRow"></param>
        private void ControlDisp(int iRow)
        {
            if (iRow < 0 || iRow > mainDGV.RowCount - 1) return;
            // �������� ��
            Txt1.Enabled = false; //��� ���ϰ�
            Btn1.Visible = false; //�Ⱥ��̰�

            if (mainDGV[0, iRow].Value == null)
                Txt1.Text = "";
            else
                Txt1.Text = mainDGV.Rows[iRow].Cells[0].Value.ToString();

            if (mainDGV[1, iRow].Value == null)
                Txt2.Text = "";
            else
                Txt2.Text = mainDGV.Rows[iRow].Cells[1].Value.ToString();

            if (mainDGV[2, iRow].Value == null)
                Txt3.Text = "";
            else
                Txt3.Text = mainDGV.Rows[iRow].Cells[2].Value.ToString();

            if (mainDGV[3, iRow].Value == null)
                Txt3.Tag = "";
            else
                Txt3.Tag = mainDGV.Rows[iRow].Cells[3].Value.ToString();

            PicBoxS3.Image = null;

            if (Txt3.Tag != "")
            {
                var pathDB = TxtFileDBPath.Text;

                //ListFiles
                using (var db = new FileDB(pathDB, FileAccess.Read))
                {
                    String id = (String)Txt3.Tag;//"1cb73f89-2ab4-401b-a925-a87421c1b233";//"ADN-433.jpg";
                    var info = db.Search(Guid.Parse(id));
                    //var info = db.Search();

                    //MessageBox.Show(info.FileName);
                    //MessageBox.Show(db.Debug.DisplayPages());

                    using (MemoryStream output = new MemoryStream())
                    {
                        db.Read(info.ID, output);
                        Image image = Image.FromStream(output);

                        PicBoxS3.Image = image;
                        PicBoxS3.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }


        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            //�ߺ�üũ
            if (radioButton1.Checked == true) //XML
            {
                //DONE : XML DB ���� �ڵ��������� �˰��� ����
                if (TxtDBpath.Text.Length <= 0)
                {
                    MessageBox.Show("Loading �� DB �� �����ϴ�!!!");
                    return; //Ż��
                } 
                XDocument doc = XDocument.Load(TxtDBpath.Text);
                var nodes = doc.Root.XPathSelectElements("//IMGLIST//IMG_INFO").ToList(); // Ver 2 ���� xml
                int iMax = 0;
                String ymd = DateTime.Now.ToString("yyyy-MM-dd"); // ��ó : https://developer-talk.tistory.com/147
                for (int i = 0; i < nodes.Count; i++)
                {
                    String str = nodes[i].Element("pNo").Value.ToString();
                    String[] words = str.Split('_');

                    if (words[0] == ymd)
                    {
                        int k = Convert.ToInt32(words[1]);
                        if (iMax < k)
                        {
                            iMax = k;
                        }
                    }
                }
                if (iMax == 0)
                {
                    Txt1.Text = PNoGenerator("");
                } else
                {
                    String numStr = String.Format("{0:D6}", iMax);
                    Txt1.Text = PNoGenerator(ymd + @"_" + numStr);
                }

                nodes = null;
                doc = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return;
            }
            if (radioButton1.Checked == true) //SQLite
            {

            }
            if (radioButton1.Checked == true) //LiteDB
            {

            }

        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            //DONE: �űԹ�ư �������� ��Ʈ�� �ʱ�ȭ
            Txt1.Enabled = true; //��밡���ϰ�
            Btn1.Visible = true;  //�ڵ��������� ��ư ���̰�

            Txt1.Text = ""; //�ʱ�ȭ
            Txt2.Text = ""; //�ʱ�ȭ
            Txt3.Text = ""; //�ʱ�ȭ
            Txt3.Tag = ""; //id �ʱ�ȭ

            PicBoxS3.Image = null; //��ó�ڽ� �ʱ�ȭ
            PicBoxS3.BackColor = Color.White;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

            if ((Txt1.Text).Trim().Length == 0)
                return;
            if (Txt1.Enabled == false)
            {
                // �������

                XDocument doc = XDocument.Load(TxtDBpath.Text);

                var nodes = doc.Root.XPathSelectElements("//IMGLIST//IMG_INFO").ToList(); // Ver 2 ���� xml

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Element("pNo").Value.ToString() == Txt1.Text)
                    {
                        //nodes[i].Element("content").Value = "new value";
                        nodes[i].Element("Desc").Value = Txt2.Text;
                        nodes[i].Element("img").Value = Txt3.Text;
                        nodes[i].Element("img_id").Value = (string)Txt3.Tag;
                        break;
                    }
                }
                doc.Save(TxtDBpath.Text);

            }
            else
            {
                //�űԵ�� ���
                if (File.Exists(TxtDBpath.Text) == false)
                {
                    // ������ ������ ���� ����

                }
                else
                {
                    //
                    //�̹� ������ �����ϸ� ������Ʈ �߰��ϱ�
                    XDocument xDoc = XDocument.Load(TxtDBpath.Text);
                    var nodes = xDoc.Root.XPathSelectElements("//IMGLIST").ToList(); // Ver 1 ���� xml
                    xDoc.Root.Add(
                            new XElement("IMG_INFO",
                            new XElement("pNo", Txt1.Text),
                            new XElement("Desc", Txt2.Text),
                            new XElement("img", Txt3.Text),
                            new XElement("img_id", (string)Txt3.Tag))
                            );

                    xDoc.Save(TxtDBpath.Text);
                }
            }


            //���� �׼��� ���������� �����ϱ�
            BtnNew_Click(sender, e);    // �ű� ��ư Ŭ��
            BtnDBLoad_Click(sender, e); // DB  ���� �ε�

        }

        private void BtnU3_Click(object sender, EventArgs e)
        {
            string fileDirPath = "";  //���丮 ��θ�
            string tmpTag = ""; //�±� 
            string tmpText = ""; //

            string tmpFilter = "";
            string tmpTitle = "";
            switch (((Button)sender).Name)
            {
                case "BtnU3":
                    tmpFilter = "JPG files (*.jpg)|*.jpg|All files (*.*)|*.*";
                    tmpTitle = "JPG ����";
                    break;
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = tmpFilter,
                Title = tmpTitle
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileDirPath = Path.GetDirectoryName(ofd.FileName);

                tmpText = Path.GetFileName(ofd.FileName);

            }
            else
                return;


            //https://github.com/mbdavid/FileDB
            //�̹��� ���Ͼ��ε�
            //var pathImgDB = "";//= @"C:\Temp\MyDB.fdb";
            // ������ ������ ��θ� �����ö� 
            //string filepath = TxtBoxFilePath0.Text ;// @"D:\�ٿ�ε�\POP\Survive You.mp3";
            //Console.WriteLine(Path.GetDirectoryName(filepath));
            var pathFilesDB = TxtFileDBPath.Text;//Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";

            if (File.Exists(pathFilesDB) == false)
            {
                //�̹�������db ���� ����
                // Creating an empty FileDB archive
                FileDB.CreateEmptyFile(pathFilesDB); //??

                var info = FileDB.Store(pathFilesDB, fileDirPath + @"\" + tmpText);
                tmpTag = info.ID.ToString();
                //���� ����Ϸ�
            }
            else
            {
                //���� �ִ� db����;

                var files = FileDB.ListFiles(pathFilesDB);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].FileName == tmpText)
                    {
                        tmpTag = files[i].ID.ToString();
                        break;
                    }
                }
                if (tmpTag == "")
                {
                    var info = FileDB.Store(pathFilesDB, fileDirPath + @"\" + tmpText);
                    tmpTag = info.ID.ToString();
                    //���� ����Ϸ�
                }
            }
            switch (((Button)sender).Name)
            {
                case "BtnU3":
                    Txt3.Text = tmpText;
                    Txt3.Tag = tmpTag;
                    break;
            }

        }
    }
}