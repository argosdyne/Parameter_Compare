using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace ParamComp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Text { get; private set; }

        private string DroneName;
        private string FCVersion;
        public MainWindow()
        {
            InitializeComponent();
        }
        static Dictionary<string, string> refParamList;
        static Dictionary<string, string> tgtParamList;
        static List<string> ignoreParamList;
        static List<string> checkParamList;
        static List<string> difParamList;

        string resultString = string.Empty;

        #region ref파라미터 파일 넣기
        private void RefParambtn_Click(object sender, RoutedEventArgs e)
        {
            // ref 파라미터 파일을 받는다.
            OpenFileDialog refopenFile = new OpenFileDialog();
            refopenFile.DefaultExt = "param";
            refopenFile.Filter = "Param (*.param)|*.param";
            refopenFile.Filter += "|메모장 (*.txt) | *.txt";
            refopenFile.ShowDialog();

            Dictionary<string, string> ReadParametersFromFile(string fileName)
            {

                string line;
                char[] delimiterChars = { ',', ' ' };
                Dictionary<string, string> paramsList;

                if (File.Exists(refopenFile.FileName) == true)
                {
                    paramsList = new Dictionary<string, string>();
                    StreamReader sr = new StreamReader(refopenFile.FileName);

                    while ((line = sr.ReadLine()) != null)
                    {
                        var words = line.Split(delimiterChars);
                        paramsList.Add(words[0], words[1]);
                    }
                }
                else
                {
                    paramsList = null;
                }
                reflabel.Content = refopenFile.FileName;

                return paramsList;
            }
            refParamList = ReadParametersFromFile(refopenFile.FileName);
        }
        #endregion

        #region target 파라미터에 파일 넣기
        private void TargetParambtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog targetopenFile = new OpenFileDialog();
            targetopenFile.DefaultExt = "param";
            targetopenFile.Filter = "Param (*.param)|*.param";
            targetopenFile.Filter += "|메모장 (*.txt) | *.txt";
            targetopenFile.ShowDialog();

            Dictionary<string, string> ReadParametersFromFile(string fileName)
            {
                string line;
                char[] delimiterChars = { ',', ' ' };
                Dictionary<string, string> paramsList;

                if (File.Exists(targetopenFile.FileName) == true)
                {
                    paramsList = new Dictionary<string, string>();
                    StreamReader sr = new StreamReader(targetopenFile.FileName);

                    while ((line = sr.ReadLine()) != null)
                    {
                        var words = line.Split(delimiterChars);
                        paramsList.Add(words[0], words[1]);
                    }
                }
                else
                {
                    paramsList = null;
                }
                targetlabel.Content = targetopenFile.FileName;

                return paramsList;
            }
            tgtParamList = ReadParametersFromFile(targetopenFile.FileName);
            targetlabel.Content = targetopenFile.FileName;
        }
        #endregion

        #region 체크할 파라미터 파일 넣기
        private void ChecksParambtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog checksopenFile = new OpenFileDialog();
            checksopenFile.DefaultExt = "param";
            checksopenFile.Filter = "Param (*.param)|*.param";
            checksopenFile.Filter += "|메모장 (*.txt) | *.txt";
            checksopenFile.ShowDialog();

            List<string> ReadParameterListFromFile(string filename)
            {
                string line;
                List<string> paramList = null;
                char[] delimiterChars = { ',', ' ' };

                if (File.Exists(filename) == true)
                {
                    paramList = new List<string>();
                    StreamReader fileReader = new StreamReader(checksopenFile.FileName);

                    while ((line = fileReader.ReadLine()) != null)
                    {
                        var words = line.Split(delimiterChars);
                        if (string.IsNullOrWhiteSpace(words[0]) == false)
                            paramList.Add(words[0]);
                    }
                }

                return paramList;
            }
            if (checksopenFile.FileName != string.Empty)
            {
                checkParamList = ReadParameterListFromFile(checksopenFile.FileName);
            }
            checkslabel.Content = checksopenFile.FileName;
        }
        #endregion

        #region 무시할 파라미터 파일 넣기
        private void IgnoresParambtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ignoreopenFile = new OpenFileDialog();
            ignoreopenFile.DefaultExt = "param";
            ignoreopenFile.Filter = "Param (*.param)|*.param";
            ignoreopenFile.Filter += "|메모장 (*.txt) | *.txt";
            ignoreopenFile.ShowDialog();

            List<string> ReadParameterListFromFile(string filename)
            {
                string line;
                List<string> paramList = null;
                char[] delimiterChars = { ',', ' ' };

                if (File.Exists(filename) == true)
                {
                    paramList = new List<string>();
                    StreamReader fileReader = new StreamReader(ignoreopenFile.FileName);

                    while ((line = fileReader.ReadLine()) != null)
                    {
                        var words = line.Split(delimiterChars);
                        if (string.IsNullOrWhiteSpace(words[0]) == false)
                            paramList.Add(words[0]);
                    }
                }

                return paramList;
            }

            if (ignoreopenFile.FileName != string.Empty)
            {
                ignoreParamList = ReadParameterListFromFile(ignoreopenFile.FileName);
            }
            ignorelabel.Content = ignoreopenFile.FileName;
        }
        #endregion

        #region 비교하기 버튼6
        private void Comparebtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> CompareParameters(Dictionary<string, string> refParams, Dictionary<string, string> tgtParams)
            {
                List<string> difParams = new List<string>();

                if (refParams == null | tgtParams == null)
                {
                    System.Windows.MessageBox.Show("비교할 값을 입력");
                }
                else
                {
                    datagridview.Columns.Add("refName", "파라미터 이름");
                    datagridview.Columns.Add("refValue", "Reference  param");
                    datagridview.Columns.Add("tgtValue", "Target  param");


                    foreach (var item in refParams.Keys)
                    {
                        string tgtValue;

                        if (tgtParams.TryGetValue(item, out tgtValue) == true)
                        {
                            string refValue = refParams[item];

                            if (string.Compare(refValue, tgtValue) != 0)
                            {
                                difParams.Add(item);
                            }
                        }
                        else
                        {
                            difParams.Add(item);
                        }
                    }
                }
                return difParams;

            }
            difParamList = CompareParameters(refParamList, tgtParamList);

            foreach (var item in difParamList)
            {
                string val;
                if (ignoreParamList != null)
                {
                    var findString = ignoreParamList.Find(x => x == item);
                    if (findString == null)
                    {
                        // ref 파일과 tgt 파일 ignore파일을 넣었을 때 출력
                        // resultString += string.Format("{0},{1},{2},{3}\r\n", item, refParamList[item], item, tgtParamList.TryGetValue(item, out val) == true ? val : "NA");

                        datagridview.Rows.Add(item, refParamList[item], tgtParamList.TryGetValue(item, out val) == true ? val : "NA");
                    }
                }
                // refparam 넣고 tgtparam 넣고 checkparam 없고 ignoreparam 없을때 출력
                if (refParamList != null && tgtParamList != null && checkParamList == null && ignoreParamList == null)
                {
                    //ref 파일과 tgt 파일만 넣었을때 실행
                    // resultString += string.Format("{0},{1},{2},{3}\r\n", item, refParamList[item], item, tgtParamList.TryGetValue(item, out val) == true ? val : "NA");

                    datagridview.Rows.Add(item, refParamList[item], tgtParamList.TryGetValue(item, out val) == true ? val : "NA");
                }
            }
            if (checkParamList != null | ignoreParamList != null && refParamList != null && tgtParamList == null
                )
            {
                System.Windows.MessageBox.Show("비교할 파라미터 입력");
            }
            else
            {

                if (checkParamList != null)
                {
                    string val;
                    foreach (var item in checkParamList)
                    {   // 기존에 값들을 한번씩 다 비교한 뒤에 ref, tgt,check 만 넣으면 ref, tgt를 비교하고 그 밑에 check된 값이 나온다.
                        // resultString += string.Format("{0},{1},{2}\r\n", item, item, tgtParamList.TryGetValue(item, out val) == true ? val : "NA");

                        datagridview.Rows.Add(item, refParamList.TryGetValue(item, out val) == true ? val : "NA", tgtParamList.TryGetValue(item, out val) == true ? val : "NA");

                    }
                }
            }
           
            //전부 다 넣으면 cmd 화면처럼 나온다.            
        }
        #endregion

        #region 텍스트 초기화
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            resultString = "";
            reflabel.Content = "";
            targetlabel.Content = "";
            checkslabel.Content = "";
            ignorelabel.Content = "";
            datagridview.Rows.Clear();
            datagridview.Columns.Clear();
            input_name.Text = "";
            input_FC.Text = "";

            if (difParamList != null)
            {
                foreach (var key in difParamList.ToList())
                {
                    difParamList.Remove(key);
                }
            }
            if (refParamList != null)
            {
                foreach (var key in refParamList.Keys.ToList())
                {
                    refParamList.Remove(key);
                }
            }
            if (tgtParamList != null)
            {
                foreach (var key in tgtParamList.Keys.ToList())
                {
                    tgtParamList.Remove(key);
                }
            }
            if (checkParamList != null)
            {
                foreach (var key in checkParamList.ToList())
                {
                    checkParamList.Remove(key);
                }
            }
            if (ignoreParamList != null)
            {
                foreach (var key in ignoreParamList.ToList())
                {
                    ignoreParamList.Remove(key);
                }
            }
        }
        #endregion

        private void Btn_nameClick(object sender, RoutedEventArgs e)
        {
            DroneName = input_name.Text;
        }
        private void Btn_FCClick(object sender, RoutedEventArgs e)
        {
            FCVersion = input_FC.Text;
        }

        #region CSV 파일로 변환
        private void Change_CSV_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv) | *.csv";
            sfd.Filter += "|TXT (*.txt) | *.txt";

            if (datagridview.RowCount <= 0)
            {
                System.Windows.MessageBox.Show("비교한 결과가 없습니다");
            }
            else
            {

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);

                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    string strHeader = "";

                    for (int i = 0; i < datagridview.Columns.Count; i++)
                    {
                        strHeader += datagridview.Columns[i].HeaderText + ",";
                    }
                    sw.WriteLine("{0},{1},{2}", "", "기체모델명", DroneName);
                    sw.WriteLine("{0},{1},{2}", "","FC Version", FCVersion);
                    sw.WriteLine("{0},{1},{2}", "","","");
                    sw.WriteLine(strHeader);
                           
                    for (int m = 0; m < datagridview.Rows.Count - 1; m++)
                    {
                        string strRowValue = "";
                        
                        for (int n = 0; n < datagridview.Columns.Count; n++)
                        {
                            strRowValue += datagridview.Rows[m].Cells[n].Value + ",";
                        }
                        sw.WriteLine(strRowValue);
                    }
                    sw.Close();

                }
            }

        }
        private void Btn_AdminClick(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
        }
        
        #endregion
        private void datagridview_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {

        }
    }

}
