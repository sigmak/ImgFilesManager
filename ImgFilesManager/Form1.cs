using System;
using System.Reflection.Metadata.Ecma335;

namespace ImgFilesManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            String tmp1 = PNoGenerator("");
            //Console.WriteLine(tmp1);

            String tmp2 = PNoGenerator("2022-11-22_000001");

            //Console.WriteLine(tmp2);

            //TODO : 생성된 순번으로 파일이름 변경해서 filedb에 저장

            //TODO : xml 태그 규칙 pNo, Dsec, fID, fName

        }


        //Done : 순번생성 규칙 : YYYY-MM-DD_###### 년월일 은 매일 받아오고 뒤의 순번은 있으면 +1씩 증가시키기
        // 사용법
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
    }
}