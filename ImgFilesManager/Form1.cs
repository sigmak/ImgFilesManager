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
    }
}