using System;

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
            string ymd = "";
            //TODO : 순번생성 규칙 : YYYY-MM-DD_###### 년월일 은 매일 받아오고 뒤의 순번은 있으면 +1씩 증가시키기

            //DONE : 문자열 구분자로 나누기
            string str = "C#_Java";
            string[] words = str.Split('_');  // 출처 : https://gent.tistory.com/502
            Console.WriteLine(words[0]);
            Console.WriteLine(words[1]);


            //DONE : 년월일 받아오기
            ymd = DateTime.Now.ToString("yyyy-MM-dd"); // 출처 : https://developer-talk.tistory.com/147
            //Console.WriteLine(ymd);

            //TODO : 숫자를 ###### 로 빈칸은0으로 채워서 표시하기
            int i = 20;
            string numStr = "";
            numStr = String.Format("{0:D6}", i);
            //Console.WriteLine(numStr);

            //string -> int : Convert.ToInt32(문자열);  // 출처 : https://mainia.tistory.com/304

            //TODO : 생성된 순번으로 파일이름 변경해서 filedb에 저장

            //TODO : xml 태그 규칙 pNo, Dsec, fID, fName

        }
    }
}