using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LiteDB;
using Numeria.IO;

namespace ImgFilesManager
{
    public partial class FrmMain : Form
    {
        //readonly BindingSource bsOrg = new();
        DataTable dtOrg = new();
        //readonly DataTable dtFilter = new();

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DONE : 폼 캡션에 버전 표시하기 출처 : https://lioncho.tistory.com/60
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            this.Text = "ImgFilesManager Ver " + version;

            mainDGV.ReadOnly = true; //읽기 전용
            mainDGV.AllowUserToAddRows = false; //마지막 빈행 생성 안되게
            mainDGV.Dock = DockStyle.Fill;


            Data_View_Design1();

            toolStripStatusLabel1.Text = "Total Rows :";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel2.Text = $"{mainDGV.RowCount}";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel3.Text = "";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel4.Text = "";// $"{mainDGV.RowCount}";//$"{num}/{selectedAlignments.Count}";

            IsChecked(ChkBoxDetailView.Checked);


            PicZoom.Visible = false;
            PicZoom.Top = mainDGV.Top;
            PicZoom.Left = mainDGV.Left;
            PicZoom.Width = mainDGV.Width;
            PicZoom.Height = mainDGV.Height;

            dtOrg.Clear();
            dtOrg.Columns.Add("pNo", typeof(string));
            dtOrg.Columns.Add("Desc", typeof(string));
            dtOrg.Columns.Add("img", typeof(string));
            dtOrg.Columns.Add("img_id", typeof(string));

            Application.DoEvents();

            //String tmp1 = PNoGenerator("");
            //Console.WriteLine(tmp1);

            //String tmp2 = PNoGenerator("2022-11-22_000001");
            //Console.WriteLine(tmp2);

            //DONE : 생성된 순번으로 파일이름 변경해서 filedb에 저장

            //DONE : xml 태그 규칙 pNo, Dsec, fName, fID 

        }

        /// <summary>
        /// DataGridView 사이즈 조절
        /// </summary>
        private void Data_View_Design1()
        {
            try
            {
                //mainDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //행높이 자동...고정???
                mainDGV.RowTemplate.Height = 90;//출처: https://docko.tistory.com/632 

                mainDGV.AutoGenerateColumns = true;
                // DataGridView 사이즈에 맞게 자동 조정
                mainDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                this.mainDGV.EditMode = DataGridViewEditMode.EditOnEnter;

                //컬럼별 넓이 지정
                mainDGV.Columns[0].Width = 70;
                mainDGV.Columns[1].Width = 100;
                mainDGV.Columns[2].Width = 80;
                mainDGV.Columns[3].Width = 60;


                //DataGridView 일반 Row 열 디자인
                mainDGV.BorderStyle = BorderStyle.None;
                mainDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
                mainDGV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                mainDGV.DefaultCellStyle.Font = new Font("굴림", 11, FontStyle.Bold);


                //Row 왼쪽 정렬
                mainDGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                mainDGV.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
                mainDGV.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

                //DataGridView ColumnHeader 디자인
                mainDGV.EnableHeadersVisualStyles = false;
                mainDGV.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                mainDGV.ColumnHeadersDefaultCellStyle.Font = new Font("굴림", 10, FontStyle.Bold);

                //dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 50, 72);
                mainDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(91, 155, 213);

                //Header Column 가운데 정렬
                mainDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                mainDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; //컬럼명 폰트 컬러

                //DataGridView RowHeader 디자인
                //dataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 50, 72);
                mainDGV.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(91, 155, 213);
                mainDGV.RowHeadersDefaultCellStyle.Font = new Font("굴림", 10, FontStyle.Bold);
                mainDGV.RowHeadersDefaultCellStyle.ForeColor = Color.White; //로우명 폰트 칼러
                mainDGV.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //Done : 순번생성 규칙 : YYYY-MM-DD_###### 년월일 은 매일 받아오고 뒤의 순번은 있으면 +1씩 증가시키기
        // 사용법
        // String tmp1 = PNoGenerator("");
        // //Console.WriteLine(tmp1);
        //
        // String tmp2 = PNoGenerator("2022-11-22_000001");
        // //Console.WriteLine(tmp2);
        public static string PNoGenerator(string str)
        {
            string result = "";
            try
            {
                if (str.Length <= 0)
                {
                    //현재 일자의 순번이 없으므로 새로생성함.
                    //DONE : 년월일 받아오기
                    String ymd = DateTime.Now.ToString("yyyy-MM-dd"); // 출처 : https://developer-talk.tistory.com/147
                    int i = 1;
                    String numStr = String.Format("{0:D6}", i); //DONE : 숫자를 ###### 로 빈칸은0으로 채워서 표시하기

                    result = ymd + @"_" + numStr;
                }
                else
                {
                    //같은 년월일에서 뒷의 숫자만 증가시킴
                    string[] words = str.Split('_');  // 출처 : https://gent.tistory.com/502
                    String ymd = words[0];
                    int i = Convert.ToInt32(words[1]); // 문자열 숫자를 숫자로  // 출처 : https://mainia.tistory.com/304
                    i++;
                    String numStr = String.Format("{0:D6}", i); //DONE : 숫자를 ###### 로 빈칸은0으로 채워서 표시하기
                    result = ymd + @"_" + numStr;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }

            return result;
        }




        public void IsChecked(bool Checked)
        {
            //출처 : https://manniz.tistory.com/entry/C-%EC%B2%B4%ED%81%AC%EB%B0%95%EC%8A%A4-%EC%B2%B4%ED%81%AC%EC%97%AC%EB%B6%80-%ED%99%95%EC%9D%B8-%ED%95%98%EA%B8%B0%EC%86%8C%EC%8A%A4-%EC%BD%94%EB%93%9C-%ED%94%84%EB%A1%9C%EC%A0%9D%ED%8A%B8-%EC%B2%A8%EB%B6%80
            if (Checked)
            {
                panel2.Visible = true;
            }
            else
            {
                panel2.Visible = false;
            }
        }

        private void BtnCreateDB_Click(object sender, EventArgs e)
        {
            string fpath = Application.StartupPath;
            if (radioButton1.Checked == true) //XML
            {
                fpath += @"\myImgXMLdb_v1.xml";
                //------------------------
                // xml 생성 
                XElement doc = new("IMGLIST", new XElement("IMG_INFO",
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
                using var db = new LiteDatabase(fpath);//@"C:\Temp\MyData.db"
                                                       // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<pNoItem>("pNoItems");

                // Create your new customer instance
                // Creates an OrderedItem.
                pNoItem i1 = new()
                {
                    pNo = PNoGenerator(""),
                    Desc = "Test",
                    img = "",//"ADN-433.jpg";
                    img_id = ""
                };

                // Insert new customer document (Id will be auto-incremented)
                col.Insert(i1);

                TxtBoxFilePath0.Text = fpath;

            }

        }

        private void BtnFilePath0_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true) //XML
            {
                OpenFileDialog ofd = new()
                {
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    Title = "XML 파일"
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
                OpenFileDialog ofd = new()
                {
                    Filter = "db files (*.db)|*.db|All files (*.*)|*.*",
                    Title = "db 파일"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    TxtBoxFilePath0.Text = ofd.FileName;


            }


        }

        private void BtnDBLoad_Click(object sender, EventArgs e)
        {
            if (TxtBoxFilePath0.Text.Trim() == "")
            {
                MessageBox.Show("DB파일 경로가 입력되지 않았습니다. ", "경고");

                return;
            }

            //Application.StartupPath + @"\MyFilesDB.fdb";
            TxtDBpath.Text = TxtBoxFilePath0.Text;
            TxtFileDBPath.Text = Path.GetDirectoryName(TxtDBpath.Text) + @"\MyFilesDB.fdb";


            Stopwatch watch = new(); //
            watch.Start();

            if (radioButton1.Checked == true) //XML
            {
                if (File.Exists(TxtBoxFilePath0.Text))
                {
                    try
                    {

                        //출처 : https://hengs.tistory.com/72
                        XmlDocument xdoc = new();
                        xdoc.Load(TxtBoxFilePath0.Text);

                        using (XmlNodeList nodes = xdoc.SelectNodes("/IMGLIST/IMG_INFO"))
                        {
                            DataRow? dtRow = null;
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
                        }

                        mainDGV.Columns.Clear(); //기존의 컬럼날리기 출처 : https://jw0652.tistory.com/9
                        mainDGV.DataSource = dtOrg;
                        mainDGV.Update();


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("XML DB 파일 로딩 실패 : " + TxtBoxFilePath0.Text);
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
                        mainDGV.Columns.Clear(); //기존의 컬럼날리기 출처 : https://jw0652.tistory.com/9

                        // Open database (or create if doesn't exist)
                        using LiteDatabase db = new(TxtBoxFilePath0.Text);
                        // Get a collection (or create, if doesn't exist)
                        var collection = db.GetCollection<pNoItem>("pNoItems");

                        BindingList<pNoItem> doclist = new();

                        foreach (var deger in collection.FindAll())
                        {
                            doclist.Add(deger);
                            //string[] row1 = new string[] { deger.deger1.ToString() };
                            //dataGridView1.Rows.Add(row1);
                            Application.DoEvents();
                        }
                        mainDGV.DataSource = doclist;


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("XML DB 파일 로딩 실패 : " + TxtBoxFilePath0.Text);
                    }

                }


            }

            watch.Stop();

            //총 Rows 수 갱신
            toolStripStatusLabel1.Text = "Total Rows :";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel2.Text = $"{mainDGV.RowCount}";//$"{num}/{selectedAlignments.Count}";

            toolStripStatusLabel3.Text = "소요시간:";//$"{num}/{selectedAlignments.Count}";
            toolStripStatusLabel4.Text = $"{watch.ElapsedMilliseconds}ms";//$"{num}/{selectedAlignments.Count}";

            Application.DoEvents();

        }

        private void MainDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ControlDisp(e.RowIndex);
        }

        private void MainDGV_KeyUp(object sender, KeyEventArgs e)
        {
            _ = mainDGV.CurrentCell.ColumnIndex; //무시항목 사용시??
            int iRow = mainDGV.CurrentCell.RowIndex;

            ControlDisp(iRow);

        }

        /// <summary>
        /// DataGridView 에 컨트롤 연동해서 표시
        /// </summary>
        /// <param name="iRow"></param>
        private void ControlDisp(int iRow)
        {
            if (iRow < 0 || iRow > mainDGV.RowCount - 1) return;
            // 수정모드로 들어감
            Txt1.Enabled = false; //사용 못하게
            Btn1.Visible = false; //안보이게

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

            if ((string)Txt3.Tag != "")
            {
                var pathDB = TxtFileDBPath.Text;

                //ListFiles
                using FileDB db = new(pathDB, FileAccess.Read);
                String id = (string)Txt3.Tag;//"1cb73f89-2ab4-401b-a925-a87421c1b233";//"ADN-433.jpg";
                EntryInfo info = db.Search(Guid.Parse(input: id));
                //var info = db.Search();

                //MessageBox.Show(info.FileName);
                //MessageBox.Show(db.Debug.DisplayPages());

                using MemoryStream output = new();

                if (System.IO.Path.GetExtension(info.FileName).ToUpper() == ".GIF") //대문자로 변환 .ToUpper()
                {

                    PicBoxS3.Image = null; //픽처박스 초기화
                    PicBoxS3.BackColor = Color.White;
                }
                else
                {
                    db.Read(info.ID, output);
                    Image image = Image.FromStream(output);

                    PicBoxS3.Image = image;
                    PicBoxS3.SizeMode = PictureBoxSizeMode.StretchImage;

                }

            }

        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            //중복체크
            if (radioButton1.Checked == true) //XML
            {
                //DONE : XML DB 사용시 자동순번생성 알고리즘 적용
                if (TxtDBpath.Text.Length <= 0)
                {
                    MessageBox.Show("Loading 된 DB 가 없습니다!!!");
                    return; //탈출
                } 
                XDocument doc = XDocument.Load(TxtDBpath.Text);
                List<XElement> nodes = doc.Root.XPathSelectElements("//IMGLIST//IMG_INFO").ToList(); // Ver 2 버전 xml
                int iMax = 0;
                String ymd = DateTime.Now.ToString("yyyy-MM-dd"); // 출처 : https://developer-talk.tistory.com/147
                for (int i = 0; i < nodes.Count; i++)
                {
                    string str = nodes[i].Element("pNo").Value.ToString();
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
            //DONE: 신규버튼 눌렀을때 컨트롤 초기화
            Txt1.Enabled = true; //사용가능하게
            Btn1.Visible = true;  //자동순번생성 버튼 보이게

            Txt1.Text = ""; //초기화
            Txt2.Text = ""; //초기화
            Txt3.Text = ""; //초기화
            Txt3.Tag = ""; //id 초기화

            PicBoxS3.Image = null; //픽처박스 초기화
            PicBoxS3.BackColor = Color.White;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

            if ((Txt1.Text).Trim().Length == 0)
                return;
            if (Txt1.Enabled == false)
            {
                // 수정모드

                XDocument doc = XDocument.Load(TxtDBpath.Text);

                var nodes = doc.Root.XPathSelectElements("//IMGLIST//IMG_INFO").ToList(); // Ver 2 버전 xml

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Element("pNo").Value.ToString() == (string)Txt1.Text)
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
                //신규등록 모드
                if (File.Exists(TxtDBpath.Text) == false)
                {
                    // 파일이 없으면 파일 생성

                }
                else
                {
                    //
                    //이미 파일이 존재하면 엘리먼트 추가하기
                    XDocument xDoc = XDocument.Load(TxtDBpath.Text);
                    _ = xDoc.Root.XPathSelectElements("//IMGLIST").ToList(); // Ver 1 버전 xml
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


            //뭔가 액션이 취해졌으면 갱신하기
            BtnNew_Click(sender, e);    // 신규 버튼 클릭
            BtnDBLoad_Click(sender, e); // DB  파일 로드

        }

        private void BtnU3_Click(object sender, EventArgs e)
        {
            string tmpTag = ""; //태그 

            string tmpFilter = "";
            string tmpTitle = "";
            switch (((Button)sender).Name)
            {
                case "BtnU3":
                    tmpFilter = "JPG files (*.jpg)|*.jpg|All files (*.*)|*.*";
                    tmpTitle = "JPG 파일";
                    break;
            }

            OpenFileDialog ofd = new()
            {
                Filter = tmpFilter,
                Title = tmpTitle
            };
            string tmpText;
            string fileDirPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileDirPath = Path.GetDirectoryName(ofd.FileName);

                tmpText = Path.GetFileName(ofd.FileName);

            }
            else
                return;


            //https://github.com/mbdavid/FileDB
            //이미지 파일업로드
            //var pathImgDB = "";//= @"C:\Temp\MyDB.fdb";
            // 파일을 제외한 경로명만 가져올때 
            //string filepath = TxtBoxFilePath0.Text ;// @"D:\다운로드\POP\Survive You.mp3";
            //Console.WriteLine(Path.GetDirectoryName(filepath));
            var pathFilesDB = TxtFileDBPath.Text;//Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";

            if (File.Exists(pathFilesDB) == false)
            {
                //이미지파일db 새로 생성
                // Creating an empty FileDB archive
                FileDB.CreateEmptyFile(pathFilesDB); //??

                var info = FileDB.Store(pathFilesDB, fileDirPath + @"\" + tmpText);
                tmpTag = info.ID.ToString();
                //파일 저장완료
            }
            else
            {
                //기존 있는 db파일;

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
                    //파일 저장완료
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

        private void BtnDel_Click(object sender, EventArgs e)
        {
            //아직 모르겠다...필요한 기능인지도...???
            //이미지 잘못 올라갔을때 삭제가 필요하다능...ㅜ.ㅠ;

            // Deleting a file
            //var ok = FileDB.Delete(pathDB, fileGuid);

            if ((Txt1.Text).Trim().Length == 0)
                return;
            if (Txt1.Enabled == false)
            {
                // 수정모드


                var confirmResult = MessageBox.Show("완전히 삭제를 하시겠습니까?",
                            "Confirm Delete!!", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    // If 'Yes', do something here. 

                    XDocument doc = XDocument.Load(TxtDBpath.Text);

                    List<XElement> nodes = doc.Root.XPathSelectElements("//IMGLIST//IMG_INFO").ToList(); // 

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (nodes[i].Element("pNo").Value.ToString() == Txt1.Text)
                        {
                            //nodes[i].Element("Desc").Value = Txt2.Text;
                            //nodes[i].Element("img").Value = Txt3.Text;
                            //nodes[i].Element("img_id").Value = (string)Txt3.Tag;

                            // Deleting a file
                            //var ok = FileDB.Delete(pathDB, fileGuid);

                            var pathFilesDB = TxtFileDBPath.Text;//Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";

                            if (File.Exists(pathFilesDB) == false)
                            {

                            }
                            else
                            {
                                //기존 있는 db파일;

                                // var files = FileDB.ListFiles(pathFilesDB);

                                //var pathFilesDB = TxtFileDBPath.Text;//Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";

                                using FileDB db = new(pathFilesDB, FileAccess.ReadWrite);
                                if (nodes[i].Element("img_id").Value.Length > 0)
                                {
                                    var info = db.Search(Guid.Parse(nodes[i].Element("img_id").Value));
                                    db.Delete(info.ID);
                                }
                            }

                            nodes[i].Remove(); //이게 맞나???

                            break;
                        }
                    }
                    doc.Save(TxtDBpath.Text);

                }

                //뭔가 액션이 취해졌으면 갱신하기
                BtnNew_Click(sender, e);    // 신규 버튼 클릭
                BtnDBLoad_Click(sender, e); // DB  파일 로드
            }
            else
            {
                // If 'No', do something here. 
                return; //탈출
            }

        }

        private void ChkBoxDetailView_CheckedChanged(object sender, EventArgs e)
        {
            IsChecked(ChkBoxDetailView.Checked);
        }

        private void BtnFinder_Click(object sender, EventArgs e)
        {
            try
            {
                string tmpSql = " 1=1 ";
                if (RbQ1.Checked == true)
                {
                    //
                }
                if (RbQ2.Checked == true)
                {
                    tmpSql += " AND (img <>'') ";
                }
                if (RbQ3.Checked == true)
                {
                    tmpSql += " AND (img ='') ";
                }


                if (TxtQ1.Text.Length > 0)
                {
                    tmpSql += " AND pNo like '%" + TxtQ1.Text + "%' ";
                }
                if (TxtQ2.Text.Length > 0)
                {
                    tmpSql += " AND Desc like '%" + TxtQ2.Text + "%' ";
                }


                dtOrg.DefaultView.RowFilter
                    = tmpSql;//("pNo like '%" + TxtQ1.Text + "%' AND Actress like '%" + TxtQ2.Text + "%' AND Desc like '%" + TxtQ3.Text + "%'  ");//string.Format

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnQOrg_Click(object sender, EventArgs e)
        {
            TxtQ1.Text = "";
            TxtQ2.Text = "";
            RbQ1.Checked = true;
            RbQ2.Checked = false;
            RbQ3.Checked = false;
            dtOrg.DefaultView.RowFilter = "";
        }

        private void PicBoxS3_MouseHover(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Image != null)
            {
                PicZoom.Width = mainDGV.Width;
                PicZoom.Height = mainDGV.Height;

                
                switch (((PictureBox)sender).Name)
                {
                    case "PicBoxS3":
                        //PicZoom.Image = ((PictureBox)sender).Image;
                        //PicZoom.SizeMode = PictureBoxSizeMode.StretchImage;
                        PicZoom.Image = ResizeImage(((PictureBox)sender).Image, new Size(PicZoom.Width, PicZoom.Height));
                        break;
                }
                PicZoom.Visible = true;
            }

        }

        //출처 : https://www.c-sharpcorner.com/UploadFile/ishbandhu2009/resize-an-image-in-C-Sharp/
        //사용법 :     System.Drawing.Image i = resizeImage(b, new Size(100, 100));
        private static Image ResizeImage(Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            //Calulate  width with new desired size  
            float nPercentW = size.Width / (float)sourceWidth;
            //Calculate height with new desired size  
            float nPercentH = size.Height / (float)sourceHeight;
            float nPercent;
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            PicZoom.Top = mainDGV.Top;
            PicZoom.Left = mainDGV.Left;
            PicZoom.Width = mainDGV.Width;
            PicZoom.Height = mainDGV.Height;
        }

        private void PicBoxS3_MouseLeave(object sender, EventArgs e)
        {
            PicZoom.Visible = false;
            PicZoom.Image = null;
        }

        private void BtnDGVtoXML_Click(object sender, EventArgs e)
        {
            if (mainDGV.RowCount <= 0) return;

            DataTable dt = new()
            {
                TableName = "IMG_INFO"
            };

            for (int i = 0; i < mainDGV.Columns.Count; i++)
            {
                //if (dataGridView1.Columns[i].Visible) // Add's only Visible columns (if you need it)
                //{
                string headerText = mainDGV.Columns[i].HeaderText;
                //headerText = Regex.Replace(headerText, "[-/, ]", "_");

                DataColumn column = new(headerText);
                dt.Columns.Add(column);
                //}
            }

            foreach (DataGridViewRow DataGVRow in mainDGV.Rows)
            {
                DataRow dataRow = dt.NewRow();
                for (int i = 0; i < mainDGV.ColumnCount; i++)
                {
                    // Add's only the columns that you want
                    string headerText = mainDGV.Columns[i].HeaderText;
                    dataRow[headerText] = DataGVRow.Cells[headerText].Value;
                }
                dt.Rows.Add(dataRow); //dt.Columns.Add();
            }
            DataSet ds = new();
            ds.Tables.Add(dt);
            //Finally the save part:
            string XML_Save_Path_Filename = Path.GetDirectoryName(TxtDBpath.Text) + @"\convertImgDB_v1.xml";
            XmlTextWriter xmlSave = new(XML_Save_Path_Filename, Encoding.UTF8);
            xmlSave.WriteStartDocument(); // 첫라인에 xml 태그 추가
            xmlSave.Formatting = Formatting.Indented;
            ds.DataSetName = "IMGLIST";
            ds.WriteXml(xmlSave);
            xmlSave.Close();
            MessageBox.Show(XML_Save_Path_Filename + " 파일저장 성공!!!");

        }

        public Image IdToPic(string id)
        {
            Image tmpImage = null;
            if (id == "")
            {
                return tmpImage;
            }

            var pathDB = TxtFileDBPath.Text;

            //ListFiles
            using (var db = new FileDB(pathDB, FileAccess.Read))
            {
                //string id = (string)Txt4.Tag;//"1cb73f89-2ab4-401b-a925-a87421c1b233";//"ADN-433.jpg";
                var info = db.Search(Guid.Parse(id));
                //var info = db.Search();

                //MessageBox.Show(info.FileName);
                //MessageBox.Show(db.Debug.DisplayPages());

                using MemoryStream output = new();
                db.Read(info.ID, output);
                //Image image = Image.FromStream(output);
                tmpImage = Image.FromStream(output);

                //PicBoxS4.Image = image;
                //PicBoxS4.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            return tmpImage;
        }

        public static int GetLineNumber(Exception ex)
        {
            ////사용 예시
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "row = " + GetLineNumber(ex));
            //}

            //출처 : http://daplus.net/c-%EC%98%88%EC%99%B8%EA%B0%80-%EB%B0%9C%EC%83%9D%ED%95%9C-%EC%A4%84-%EB%B2%88%ED%98%B8%EB%8A%94-%EC%96%B4%EB%96%BB%EA%B2%8C-%EC%95%8C-%EC%88%98-%EC%9E%88%EC%8A%B5%EB%8B%88%EA%B9%8C/
            var lineNumber = 0;
            const string lineSearch = ":line ";
            int index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                string lineNumberText = ex.StackTrace[(index + lineSearch.Length)..];
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }

        private void BtnD3_Click(object sender, EventArgs e)
        {
            string tmpTag = "";
            switch (((Button)sender).Name)
            {
                case "BtnD3":
                    tmpTag = (string)Txt3.Tag;
                    break;
            }
            if (tmpTag == "") return;

            var pathFilesDB = TxtFileDBPath.Text;//Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";

            using (var db = new FileDB(pathFilesDB, FileAccess.Read))
            {
                var info = db.Search(Guid.Parse(tmpTag));
                //Path.GetDirectoryName(filepath) + @"\MyFilesDB.fdb";
                //return db.Read(info.ID , info.FileName);
                db.Read(info.ID, Path.GetDirectoryName(pathFilesDB) + @"\" + info.FileName);

                MessageBox.Show("이미지파일 다운로드 성공 : " + Path.GetDirectoryName(pathFilesDB) + @"\" + info.FileName); 
            }

        }

        private void mainDGV_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //행번호 표시되게
            //출처 : https://www.wake-up-neo.com/ko/c%23/datagridview%EC%9D%98-%ED%96%89-%ED%97%A4%EB%8D%94%EC%97%90-%ED%96%89-%EB%B2%88%ED%98%B8-%ED%91%9C%EC%8B%9C/942184626/amp/
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,

                LineAlignment = StringAlignment.Center
            };
            //get the size of the string
            Size textSize = TextRenderer.MeasureText(rowIdx, this.Font);
            //if header width lower then string width then resize
            if (grid.RowHeadersWidth < textSize.Width + 40)
            {
                grid.RowHeadersWidth = textSize.Width + 40;
            }
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);

        }

        #region 유사 이미지 찾기 버튼 클릭시 처리하기 - findButton_Click(sender, e)

        /// <summary>
        /// 유사 이미지 찾기 버튼 클릭시 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>

        private void findButton_Click(object sender, EventArgs e)
        {
            //filedb 선택부터


            //----------------------------------------------------------------

            //유사 이미지 찾기 알고리즘 출처 : https://icodebroker.tistory.com/5432
            //
        }
        #endregion

    }
}