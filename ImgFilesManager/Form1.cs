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
            //TODO : �������� ��Ģ : YYYY-MM-DD_###### ����� �� ���� �޾ƿ��� ���� ������ ������ +1�� ������Ű��

            //DONE : ���ڿ� �����ڷ� ������
            string str = "C#_Java";
            string[] words = str.Split('_');  // ��ó : https://gent.tistory.com/502
            Console.WriteLine(words[0]);
            Console.WriteLine(words[1]);


            //DONE : ����� �޾ƿ���
            ymd = DateTime.Now.ToString("yyyy-MM-dd"); // ��ó : https://developer-talk.tistory.com/147
            //Console.WriteLine(ymd);

            //TODO : ���ڸ� ###### �� ��ĭ��0���� ä���� ǥ���ϱ�
            int i = 20;
            string numStr = "";
            numStr = String.Format("{0:D6}", i);
            //Console.WriteLine(numStr);

            //string -> int : Convert.ToInt32(���ڿ�);  // ��ó : https://mainia.tistory.com/304

            //TODO : ������ �������� �����̸� �����ؼ� filedb�� ����

            //TODO : xml �±� ��Ģ pNo, Dsec, fID, fName

        }
    }
}