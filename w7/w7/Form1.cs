// updated jan032017 correction regarding 000name file created unnecessarly
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml.Schema;
using System.IO;

namespace w7
{
    public partial class Form1 : Form
    {
        public OracleConnection conn;
        public OracleCommand cmd,cmd1,cmd2,cmd3,cmd5,cmd6;
        public string lstday, lstmonth, lstyear;
        public int zippedalready = 0;
        public int dateselected = 0;
        public int dateselectedn = 0;
        public int maxappno;
        public int minappno;
        public int updatedatabase = 0;
       
        public int fullordiff = 0;
        string ftpServerIP;
        string ftpRemotePath;
        string ftpUserID;
        string ftpPassword;
        string ftpURI;
        public static string validfile;
        public static string filenameonly;
       
        public string filetosend1, filetosend2;
        public string foldertosend1, foldertosend2;
        public string s1, files, filesi;
        public string grtl, grtli;
        public int folnomax = 0;
        public DateTime stdate;
        public string datestr, rootf, datef;
        //public DateTime stdate = DateTime.Now;
        //string datestr = stdate.Year.ToString() + "-" + stdate.Month.ToString() + "-" + stdate.Day.ToString();
        //string rootf = "D:/";
        //string datef = datestr + "/";
        public int cnt = 0; // file name counter 
        public string[] filenames = new string[100000];  // string array : that use to store names of files 
        public string[] foldernamesxml = new string[1000];  // string array : that use to store names of files 
        public string[] foldernamesimg = new string[1000];  // string array : that use to store names of files 
        public string[] localfoldersequencelist = new string[1000];
        public string[] localfoldersequencelistimg = new string[1000];

        
        public void connectora()
        {
            //conn = new OracleConnection("DATA SOURCE=tmrdb;PASSWORD=registry;USER ID=C##TMRINDIA");
            conn = new OracleConnection("DATA SOURCE=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 10.199.2.69)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = tmrdb)));PASSWORD=registry;USER ID=C##TMRINDIA");
            conn.Open();
        }

        public void deletewrongxml()
        {

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader("D:/errors.txt");
            
            while ((line = file.ReadLine()) != null)
            {
                //System.IO.File.Delete(line);
                           
                counter++;
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();

        }

        private static void Validate(String filename, XmlSchemaSet schemaSet)
        {
        
            XmlSchema compiledSchema = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            foreach (XmlSchema schema in schemaSet.Schemas())
            {
                compiledSchema = schema;
                settings.Schemas.Add(compiledSchema);
            }
            validfile = filename;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.ValidationType = ValidationType.Schema;
            //Create the schema validating reader.
            XmlReader vreader = XmlReader.Create(filename, settings);
            while (vreader.Read()) { }
            //Close the reader.
            vreader.Close();
            
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                MessageBox.Show("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
            }
            else
            {
                //MessageBox.Show("\tValidation error: " + args.Message); // 03NOV2016
                //File.Move("source", "destination");
              //System.IO.File.Move(validfile,"D:/temp/"+ filenameonly); 
  
                //System.IO.File.AppendAllText("D:/errors.txt", validfile+Environment.NewLine);
                System.IO.File.AppendAllText("D:/errorlog.txt", validfile + args.Message + Environment.NewLine);
                System.IO.File.AppendAllText("D:/errors.txt", validfile +Environment.NewLine); // this file is used to delete wrong XML 
                                
                //MessageBox.Show(validfile);
            }

        }
        
// module to convert month name e.g "01" converted to "JAN"
        public string monthnames(int m)
        {
            string month_name = "";
            switch (m)
            {
                case 1:
                    month_name = "JAN";
                    break;
                case 2:
                    month_name = "FEB";
                    break;
                case 3:
                    month_name = "MAR";
                    break;
                case 4:
                    month_name = "APR";
                    break;
                case 5:
                    month_name = "MAY";
                    break;
                case 6:
                    month_name = "JUN";
                    break;
                case 7:
                    month_name = "JUL";
                    break;
                case 8:
                    month_name = "AUG";
                    break;
                case 9:
                    month_name = "SEP";
                    break;
                case 10:
                    month_name = "OCT";
                    break;
                case 11:
                    month_name = "NOV";
                    break;
                case 12:
                    month_name = "DEC";
                    break;
                default:
                    break;

            }
            return month_name;
        }

        public string padzero(int cntr) // not in use 
        {
            string lp = ""; // string for left padding zero 
            int lnth = 0;  // according to length of string the the remaining lenth will be padded by zero 

            lnth = cntr.ToString().Length;
            // ----loop for padding zero 
            for (int k = 1; k <= 4 - lnth; k++)
            {
                lp = lp + "0";
            }
            return lp;
        }

        void deletedir(string pth)
        { // this modile is to make directory empty 
            System.IO.DirectoryInfo di = new DirectoryInfo(pth);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }


        public string[] filesindir(string pth)
        { // this modile is to make directory empty 
            string[] s = new string[10];
            int lvar = 0;
            System.IO.DirectoryInfo di = new DirectoryInfo(pth);
            foreach (FileInfo file in di.GetFiles())
            {
                s[lvar] = file.Name;
                lvar = lvar + 1;
             }
            return s;
        }
       

 public void copydir(string source,string destination) 
 {
            string destDirectory = Path.GetDirectoryName(destination);
          
      if (File.Exists(source) && Directory.Exists(destDirectory)) {
    
       File.Move(source, destination);
}
else {
    MessageBox.Show("error in copy");
    // Throw error or alert
}
        
        }


 public void Upload(string filename, string ftpRemotePath,string xmlorimage)
        {
            string ftpServerIP;
            //string ftpRemotePath;
            string ftpUserID;
            string ftpPassword;
            string ftpURI;
            //--FTP upload section start
            //ftpServerIP = "109.232.208.203";
            ftpServerIP = "ftp.tmdn.org";

     
           //ftpServerIP = "10.199.2.41";
            //ftpRemotePath = "/tmview/";
            //ftpUserID = "ftpusers";
            //ftpPassword = "FTP-user";
            //ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
            
            //ftpServerIP = "109.232.208.203";
            
            ftpRemotePath = "/utest/";
            
            ftpUserID = "IPO_INftpusr";
            ftpPassword = "+F6aL_<d";
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
     
            rootf = "D:/";
            //datestr = "2016-00-00";

            xmlorimage = "xml";
    
     String[] filePaths = Directory.GetFiles("D:/2016-12-7/zipped/images/");
     //String[] filePaths = Directory.GetFiles("D:/2016-11-2-x/2016-11-2-IN-FULL-INDX(IMGA)-0001/");
     int counfolder = 0;
     int loadfolder=0;

     
     foreach (string filepath in filePaths)
     {
     counfolder++;
     }
     textBox4.Text = counfolder.ToString();
     textBox4.Refresh();


            foreach (string filepath in filePaths)
            {
            
                FileInfo fileInf = new FileInfo(filepath);
                string uri = ftpURI + fileInf.Name;
                FtpWebRequest reqFTP;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                //reqFTP.UsePassive = false;
                reqFTP.ContentLength = fileInf.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                FileStream fs = fileInf.OpenRead();
                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
            //        MessageBox.Show("file writing over");
                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    //Insert_Standard_ErrorLog.Insert("FtpWeb", "Upload Error --> " + ex.Message);
                    MessageBox.Show(ex.Message);
                   // Console.Write(ex.Message);
                }
               // Console.Write(filepath.ToString());
                loadfolder++;
                textBox1.Text = loadfolder.ToString();
                textBox1.Refresh();
                //-- to check if file exist on server or not 

               


                //-- END - to check if file exist on server or not 
            }// end of for loop foreach filepath
            
           // MessageBox.Show("done");
        }

        
        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.CustomFormat = "dd-mmm-yyyy";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          /*
            ftpServerIP = "10.199.2.41";
            ftpRemotePath = "/tmview/";
            ftpUserID = "ftpusers";
            ftpPassword = "FTP-user";
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
            */

            //textBox3.Text = "15/11/2016 00:00:00";
            //textBox2.Text = "05/11/2016 23:59:59";

            //ftpServerIP = "109.232.208.203";
            ftpServerIP = "ftp.tmdn.org";

            ftpRemotePath = "/utest/"; // overwritten later 
                        
            ftpUserID = "IPO_INftpusr";
            ftpPassword = "+F6aL_<d";
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
            

            connectora();
            OracleCommand cmd4 = new OracleCommand();
            cmd4 = new OracleCommand();
            cmd4.Connection = conn;
            cmd4.CommandType = CommandType.Text;
            //cmd4.CommandText = "select min(application_number) from tmr_device_image_details";
            cmd4.CommandText = "select min(application_number) from tmr_application_details where tmr_application_status in ('REG','REC')";
            // tmr_application_details
            OracleDataReader odr4 = cmd4.ExecuteReader();
            odr4.Read();
            textBox3.Text = odr4[0].ToString();
            minappno = Convert.ToInt32(textBox3.Text.ToString());

            cmd4.CommandText = "select max(application_number) from tmr_application_details where tmr_application_status in ('REG','REC')";
            odr4 = cmd4.ExecuteReader();
            odr4.Read();
            textBox2.Text = odr4[0].ToString();
            maxappno = Convert.ToInt32(textBox2.Text.ToString());
            //radioButton1.Select();
            radioButton2.Select();
            dateTimePicker1.Visible = true;
            dateTimePicker2.Visible = true;
            checkBox1.Checked = true;

            textBox5.Text = "1";
       //    allcodes(); // psuse these two to make program attended mode , if these two not paused it means unattended mode 
       //    Application.Exit();
         

        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            //FormBorderStyle = FormBorderStyle.None;  // for FULL SCREEN 
            //WindowState = FormWindowState.Maximized;
            label1.Visible=false;
            label2.Visible=false;
            button1.Text = "Wait for task to be completed....";

            //if (Convert.ToInt32(textBox3.Text.ToString()) > Convert.ToInt32(textBox2.Text.ToString()))
            //{
            //    MessageBox.Show("Application Number range is wrong");
            //    textBox3.Focus();
            //}

            //connectora();
            cmd = new OracleCommand();
            cmd1 = new OracleCommand();
            cmd2 = new OracleCommand();
            cmd3 = new OracleCommand();
            cmd5 = new OracleCommand();
            cmd6 = new OracleCommand();

            cmd.Connection = conn;
            cmd1.Connection = conn;
            cmd2.Connection = conn;
            cmd3.Connection = conn;
            cmd5.Connection = conn;
            cmd6.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd1.CommandType = CommandType.Text;
            cmd2.CommandType = CommandType.Text;
            cmd3.CommandType = CommandType.Text;
            cmd5.CommandType = CommandType.Text;
            cmd6.CommandType = CommandType.Text;
            int recordfound = 0;

            // - for generated XML file verification against XSD
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            /*
            schemaSet.Add("http://www.oami.europa.eu/TM-Search", "C:/Users/uma/Desktop/IN-TMview-CDM-Schema-V1-0.xsd");
            schemaSet.Add("http://www.tm-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/ISOCountryCodeType-V2011.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/ISOLanguageCodeType-V2002.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/WIPOST3CodeType-V2011.xsd");
            */
            schemaSet.Add("http://www.oami.europa.eu/TM-Search", "myschemas/IN-TMview-CDM-Schema-V1-0.xsd");
            schemaSet.Add("http://www.tm-xml.org/XMLSchema/common", "myschemas/external/ISOCountryCodeType-V2011.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "myschemas/external/ISOLanguageCodeType-V2002.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "myschemas/external/WIPOST3CodeType-V2011.xsd");

            schemaSet.Compile();

            //System.IO.File.Delete("D:/errors.txt"); // this is to delete file have list of errorsome xml files

              // END OF generated XML file verification against XSD

            // query to read unique application numbers
            //fullordiff = 1; // tmp HC

            if (radioButton2.Checked == true)
            {
                fullordiff = 1;
              }
            else if (radioButton1.Checked==true)
            {
                fullordiff = 0;
            }


            if (fullordiff == 0) 
            {   // application number based
                
                cmd2.CommandText = "select app.application_number as col1 from TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC') and pub.publish_date is NULL";
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC') and pub.publish_date is NULL and app.application_number in (select APPLICATION_NUMBER from TMR_APPLICATION_DETAILS where WM_SEARCH_STRING is NULL and TRANSLATION is NULL)";// agrwall 
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC') and pub.publish_date is NULL and app.application_number in (select APPLICATION_NUMBER from oNLINE_FORMS_DETAILS)";// agrwall1 
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC')";
                
            
            }
            else // date based 
            {
                //cmd2.CommandText = "select unique(application_number) as col1 from  TMR_DEVICE_IMAGE_DETAILS where LAST_UPDATED_DATE>='" + textBox3.Text.ToString() + "' and LAST_UPDATED_DATE<='" + textBox2.Text.ToString() + "'";
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('" + textBox3.Text.ToString() + " 00:00:00','dd/MM/yyyy hh24:mi:ss')  and pub.LAST_UPDATED_DATE<=to_date('" + textBox2.Text.ToString() + " 23:59:59' ,'dd/MM/yyyy hh24:mi:ss')  and (publish_date < pub.last_updated_date or publish_date is null)  and app.tmr_application_status in ('REG','REC')";
                //cmd2.CommandText = "select app.application_number as col1 from  tmr_tmview_publish_details pub left outer join tmr_application_details app on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('06/12/2016 00:00:00 ' , 'dd/MM/yyyy hh24:mi:ss'  ) and pub.LAST_UPDATED_DATE<=to_date('07/12/2016 23:59:59 ' , 'dd/MM/yyyy hh24:mi:ss'  ) and (pub.publish_date <pub.last_updated_date or pub.publish_date is null)  and app.tmr_application_status in ('REG','REC')";
                cmd2.CommandText = "select app.application_number as col1 from  tmr_tmview_publish_details pub left outer join tmr_application_details app on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('" + textBox3.Text + " ' , 'dd/MM/yyyy hh24:mi:ss'  ) and pub.LAST_UPDATED_DATE<=to_date('" + textBox2.Text + "' , 'dd/MM/yyyy hh24:mi:ss'  ) and (pub.publish_date <pub.last_updated_date or pub.publish_date is null)  and app.tmr_application_status in ('REG','REC')";
            }
            string[] applicaionnumbers=new string[500000];


            OracleDataReader dr2 = cmd2.ExecuteReader();
            //dr2.Read();
            int j = 0;
            int max = 0;

           
            while (dr2.Read())
            {
                applicaionnumbers[j] = dr2[0].ToString();
                j++;
                max = j;
             }

            if (max == 0)  // if blank application number found
            {
               MessageBox.Show("No application number for this range"); // paused for unattended mode 

               Application.Exit();
            }
            else
            {

                j = 0;
                textBox4.Text = max.ToString();
                textBox4.Refresh();

                stdate = DateTime.Now;
                int appno = 0;
                
                string monthnumberstr = "";
                string daynumberstr = "";

                if (stdate.Month <= 9)
                {
                    monthnumberstr = "0" + stdate.Month.ToString();
                
                }
                else
                {
                    monthnumberstr = stdate.Month.ToString();
                
                }


                if (stdate.Day <= 9)
                {
                
                    daynumberstr = "0" + stdate.Day.ToString();
                }
                else
                {
                
                    daynumberstr = stdate.Day.ToString();
                }
                
                
                datestr = stdate.Year.ToString() + "-" + monthnumberstr + "-" + daynumberstr.ToString();  
                //datestr = "2017-02-19";

                rootf = "D:/";
                datef = datestr + "/";

                string localfixfoldernameimg1;
                string localfixfoldernamexml1;
                if (fullordiff == 0)
                {

                    localfixfoldernameimg1 = "-IN-FULL-INDX(IMGA)-";
                    localfixfoldernamexml1 = "-IN-FULL-INDX-";
                }
                else
                {
                                        localfixfoldernameimg1 = "-IN-DIFF-INDX(IMGA)-";
                                        localfixfoldernamexml1 = "-IN-DIFF-INDX-";
                }
                
                               

                string localfoldersequencestr = "0000";

                int folseqinitial = 0;
                cmd6.CommandText = "select nvl(max(ZIPFILE_SEQ),0)  from tmr_tmview_pub_seq  where trunc(transaction_date)='16-NOV-2016'";
                folseqinitial = Convert.ToInt32(cmd6.ExecuteScalar());


                int folderseqno = folseqinitial + 1;// initial folder seq for that  date/day   //select nvl(max(ZIPFILE_SEQ),0)  from tmr_tmview_pub_seq  where trunc(transaction_date)='16-NOV-2016'  
                string filename = "";
                string filenamepart1 = "-IN50";
                string filenamepart2 = "";
                string xmlcontent = "";
                int folno = 0;
                // pick from db 


            
                   for (j = 0; j <= max-1; j++) 
                {

                    appno = Convert.ToInt32(applicaionnumbers[j]);

                    if (j % 1000 == 0)
                    {
                        folno = folno + 1;
                        folnomax = folno;
                        string lpf = ""; // string for left padding zero 
                        int lnthf = 0;  // according to length of string the the remaining lenth will be padded by zero 

                        lnthf = folno.ToString().Length;
                        // ----loop for padding zero for folder name ...
                        for (int kf = 1; kf <= 4 - lnthf; kf++)
                        {
                            lpf = lpf + "0";
                        }

                        localfoldersequencestr = lpf + folno.ToString();
                        zippedalready = 1;
                        localfoldersequencelist[folno] = localfoldersequencestr;
                        localfoldersequencelistimg[folno] = localfoldersequencestr;
                    }


                    cmd1.CommandText = "SELECT trade_mark_image from TMR_DEVICE_IMAGE_DETAILS where image_number=1 and TRADE_MARK_IMAGE IS NOT NULL and application_number=" + appno;
                    //cmd.CommandText = "select publish_for_tmview(" + appno.ToString() + ") from dual "; // changes done 23june
                    cmd.CommandText = "select publish_for_tmview(" + appno.ToString() + "," + "'Insert'" + ") from dual ";

                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    xmlcontent = dr.GetString(0).ToString();
                    string lp = ""; // string for left padding zero 
                    int lnth = 0;  // according to length of string the the remaining lenth will be padded by zero 

                    lnth = appno.ToString().Length;
                    // ----loop for padding zero for file name ...
                    for (int k = 1; k <= 13 - lnth; k++)
                    {
                        lp = lp + "0";
                    }

                    filenamepart2 = lp + appno.ToString();


                    if (System.IO.Directory.Exists(rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr);
                    }
                    filename = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr + "/" + datestr + filenamepart1 + filenamepart2 + ".xml"; // modified  june23..
                    foldertosend1 = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    System.IO.File.WriteAllText(filename, xmlcontent);
                    filenameonly = filenamepart1 + filenamepart2 + ".xml";
                    Validate(filename, schemaSet);
                    foldertosend1 = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    //localfoldersequencelist[folno] = rootf + datef + "/zipped/" + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    localfoldersequencelist[folno] = rootf + datef + "/zipped/xmls/" + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    foldernamesxml[folno] = foldertosend1;

                    OracleDataReader dr1 = cmd1.ExecuteReader();
                    if (dr1.Read())
                    {
                        OracleLob blob = dr1.GetOracleLob(0); // this will convert recieved data into BLOB data type variale
                        byte[] buffer = new byte[blob.Length];   // this will transfer blob data in byte stream according to length
                        int x = blob.Read(buffer, 0, System.Convert.ToInt32(blob.Length)); // this will measure the length of string 


                        filename = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr + "/" + datestr + filenamepart1 + filenamepart2 + ".jpg"; // changes done
                        //filename = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr + "/" + filenamepart1 + filenamepart2 + ".jpg";

                        foldertosend2 = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        //localfoldersequencelistimg[folno] = rootf + datef + "/zipped/" + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        localfoldersequencelistimg[folno] = rootf + datef + "/zipped/images/" + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        foldernamesimg[folno] = foldertosend2;

                        if (System.IO.Directory.Exists(rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr);
                        }

                        FileStream fs2 = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write); // complementry code for writing IMAGE data 
                        fs2.Write(buffer, 0, (int)blob.Length);  //================================ write IMAGE data into files 
                        filenamepart2 = "";
                        fs2.Close();
                    }
                       // - section for database update -- START 

                    if (checkBox1.Checked==true)
                    {
                        folderseqno = Convert.ToInt32(localfoldersequencestr);
                        cmd3.CommandText = "insert into  tmr_tmview_pub_seq(application_number,transaction_date,zipfile_seq) values(" + appno + "," + "sysdate" + "," + folderseqno + ")";

                        cmd3.ExecuteNonQuery();

                        cmd5.CommandText = "Select count(application_number) from tmr_tmview_publish_details where application_number=" + appno;
                        recordfound = Convert.ToInt32(cmd5.ExecuteScalar());

                        if (recordfound == 0)
                        {
                            cmd5.CommandText = "insert into tmr_tmview_publish_details(application_number,publish_date) values(" + appno + ",sysdate)";
                        }
                        else
                        {
                            cmd5.CommandText = "update tmr_tmview_publish_details set publish_date=sysdate where application_number=" + appno;
                        }

                        cmd5.ExecuteNonQuery();
                    }
                    else
                    {
                        
                    }

                        // - section for database update -- END 
                        
                    textBox1.Text = j.ToString();
                    textBox1.Refresh();

                }// end of loop j for application number

                //-- program to delete wrong XML files 
              //  deletewrongxml();

                // -- pe

                //if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/"))
                   if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/xmls/"))
                {
                }
                else
                {
//                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/");
                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/xmls");
                }

                //if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/"))
                   if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/images/"))
                {
                }
                else
                {
                    //System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/");
                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/images/");
                }



                for (folno = 1; folno <= folnomax; folno++)
                {
                    if (String.IsNullOrEmpty(foldernamesxml[folno]))
                    {
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(foldernamesxml[folno]))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(foldernamesxml[folno]);
                        }

                        if (System.IO.Directory.Exists(foldernamesimg[folno]))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(foldernamesimg[folno]);
                        }

                        
                        ZipFile.CreateFromDirectory(foldernamesxml[folno], localfoldersequencelist[folno] + ".zip");
                        ZipFile.CreateFromDirectory(foldernamesimg[folno], localfoldersequencelistimg[folno] + ".zip");
                        
//                        Upload(localfoldersequencelist[folno] + ".zip","/Images/2016-11-17/","xmls");
//                        Upload(localfoldersequencelistimg[folno] + ".zip","/TM-XML/2016-11-17/","images");
                       
                    }

                } // end of for loop 

                MessageBox.Show("Work done");
                MessageBox.Show("at location D:/Errors.txt file generated for log of wrong xml files");
                button1.Text = "Generate Files";
                label1.Visible = true;
                label2.Visible = true;
                textBox2.Visible = true;
                textBox3.Visible = true;
            }
                       
//            WindowState = FormWindowState.Normal;
  //          FormBorderStyle = FormBorderStyle.Fixed3D;
            
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            Upload("a", "b", "c");
            // dummy argument passed 
         

      }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
            lstday = dateTimePicker1.Value.Day.ToString();
            lstmonth = dateTimePicker1.Value.Month.ToString();
            lstyear = dateTimePicker1.Value.Year.ToString();
            
            //textBox3.Text = lstday+"-"+lstmonth+"-"+lstyear;
            //textBox3.Text = lstday + "-" + monthnames(Convert.ToInt32(lstmonth)) + "-" + lstyear;
            textBox3.Text = lstday + "/" + lstmonth+ "/" + lstyear +" "+"00:00:00";
            dateselected = 1;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = true;
            dateTimePicker2.Visible = true;
            dateTimePicker1.Focus();
            dateselectedn = 1;
            if (radioButton2.Checked)
                fullordiff=1;
           
         }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            lstday = dateTimePicker2.Value.Day.ToString();
            lstmonth = dateTimePicker2.Value.Month.ToString();
            lstyear = dateTimePicker2.Value.Year.ToString();

            //textBox2.Text = lstday + "-" + monthnames(Convert.ToInt32(lstmonth)) + "-" + lstyear;
            textBox2.Text = lstday + "/" + lstmonth + "/" + lstyear + " " + "23:59:59";

            //textBox4.Text = monthnames(Convert.ToInt32(lstmonth));           
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                fullordiff = 0;
            dateTimePicker1.Visible = false;
            dateTimePicker2.Visible = false;
            textBox5.Visible = false;
            label5.Visible = false;
            textBox3.Text = minappno.ToString();
            textBox2.Text = maxappno.ToString();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //int daybefore = Convert.ToInt32(textBox5.Text);
            //lstday = (dateTimePicker1.Value.Day-daybefore).ToString();
            //lstmonth = dateTimePicker1.Value.Month.ToString();
            //lstyear = dateTimePicker1.Value.Year.ToString();

            ////textBox3.Text = lstday+"-"+lstmonth+"-"+lstyear;
            ////textBox3.Text = lstday + "-" + monthnames(Convert.ToInt32(lstmonth)) + "-" + lstyear;
            //textBox3.Text = lstday + "/" + lstmonth + "/" + lstyear + " " + "00:00:00";
            //dateselected = 1;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            

          if (textBox5.Text == "1" || textBox5.Text == "2" || textBox5.Text == "3" || textBox5.Text == "4" || textBox5.Text == "5" || textBox5.Text == "6" || textBox5.Text == "7" || textBox5.Text == "8" || textBox5.Text == "9")
          //  if (!Regex.IsMatch(textBox5.Text, @"[a-zA-Z]"))
            {
                int daybefore = Convert.ToInt32(textBox5.Text);
                lstday = (dateTimePicker1.Value.Day - daybefore).ToString();
                lstmonth = dateTimePicker1.Value.Month.ToString();
                lstyear = dateTimePicker1.Value.Year.ToString();
              // -- section for crossing month 

                if (Convert.ToInt32(lstday) <= 0)
                {
                    int dayofthatmonth = 30;
                    lstmonth = (Convert.ToInt32(lstmonth) - 1).ToString();
                    switch (lstmonth) {
                        case "1":
                            dayofthatmonth = 31;
                            break;
                        case "2":
                            dayofthatmonth = 28;
                            break;
                        case "3":
                            dayofthatmonth = 31;
                            break;
                        case "5":
                            dayofthatmonth = 31;
                            break;
                        case "7":
                            dayofthatmonth = 31;
                            break;
                        case "8":
                            dayofthatmonth = 31;
                            break;
                        case "10":
                            dayofthatmonth=31;
                                      break;
                        case "12":
                                      dayofthatmonth = 31;
                                      break;
                                                
                        default:
                            dayofthatmonth=30;
                            break;
                    }

                    lstday = (dayofthatmonth + Convert.ToInt32(lstday)).ToString();
                }
                else
                {
                }


                // -- section for crossing month END


                textBox3.Text = lstday + "/" + lstmonth + "/" + lstyear + " " + "00:00:00";
                dateselected = 1;
                lstday = (dateTimePicker2.Value.Day - 1).ToString();
                lstmonth = dateTimePicker2.Value.Month.ToString();
                lstyear = dateTimePicker2.Value.Year.ToString();

                //textBox2.Text = lstday + "-" + monthnames(Convert.ToInt32(lstmonth)) + "-" + lstyear;
                textBox2.Text = lstday + "/" + lstmonth + "/" + lstyear + " " + "23:59:59";

                //textBox4.Text = monthnames(Convert.ToInt32(lstmonth));           

            }
          else { }
        }

        // same code as button1 click
    public void allcodes()    
    {
            //FormBorderStyle = FormBorderStyle.None;  // for FULL SCREEN 
            //WindowState = FormWindowState.Maximized;
            label1.Visible=false;
            label2.Visible=false;
            button1.Text = "Wait for task to be completed....";

            //if (Convert.ToInt32(textBox3.Text.ToString()) > Convert.ToInt32(textBox2.Text.ToString()))
            //{
            //    MessageBox.Show("Application Number range is wrong");
            //    textBox3.Focus();
            //}

            
            //connectora();
            cmd = new OracleCommand();
            cmd1 = new OracleCommand();
            cmd2 = new OracleCommand();
            cmd3 = new OracleCommand();
            cmd5 = new OracleCommand();
            cmd6 = new OracleCommand();

            cmd.Connection = conn;
            cmd1.Connection = conn;
            cmd2.Connection = conn;
            cmd3.Connection = conn;
            cmd5.Connection = conn;
            cmd6.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd1.CommandType = CommandType.Text;
            cmd2.CommandType = CommandType.Text;
            cmd3.CommandType = CommandType.Text;
            cmd5.CommandType = CommandType.Text;
            cmd6.CommandType = CommandType.Text;
            int recordfound = 0;

            // - for generated XML file verification against XSD
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            /*
            schemaSet.Add("http://www.oami.europa.eu/TM-Search", "C:/Users/uma/Desktop/IN-TMview-CDM-Schema-V1-0.xsd");
            schemaSet.Add("http://www.tm-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/ISOCountryCodeType-V2011.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/ISOLanguageCodeType-V2002.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "C:/Users/uma/Desktop/WIPOST3CodeType-V2011.xsd");
            */
            schemaSet.Add("http://www.oami.europa.eu/TM-Search", "D:/w7/w7/bin/Debug/myschemas/IN-TMview-CDM-Schema-V1-0.xsd");
            schemaSet.Add("http://www.tm-xml.org/XMLSchema/common", "D:/w7/w7/bin/Debug/myschemas/external/ISOCountryCodeType-V2011.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "D:/w7/w7/bin/Debug/myschemas/external/ISOLanguageCodeType-V2002.xsd");
            schemaSet.Add("http://www.ds-xml.org/XMLSchema/common", "D:/w7/w7/bin/Debug/myschemas/external/WIPOST3CodeType-V2011.xsd");

            schemaSet.Compile();

            //System.IO.File.Delete("D:/errors.txt"); // this is to delete file have list of errorsome xml files

              // END OF generated XML file verification against XSD

            // query to read unique application numbers
            //fullordiff = 1; // tmp HC

            if (radioButton2.Checked == true)
            {
                fullordiff = 1;
              }
            else if (radioButton1.Checked==true)
            {
                fullordiff = 0;
            }


            if (fullordiff == 0) 
            {
                //cmd2.CommandText = "select unique(application_number) as col1 from  TMR_DEVICE_IMAGE_DETAILS where application_number>='" + textBox3.Text.ToString() + "' and application_number<='" + textBox2.Text.ToString() + "'";
                //cmd2.CommandText = "select unique(application_number) as col1 from  TMR_application_DETAILS where application_number>='" + textBox3.Text.ToString() + "' and application_number<='" + textBox2.Text.ToString() + "'";
                cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC') and pub.publish_date is NULL";
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where app.application_number>='" + textBox3.Text.ToString() + "' and app.application_number<='" + textBox2.Text.ToString() + "' and app.tmr_application_status in ('REG','REC')";
                
            
            }
            else
            {
                //cmd2.CommandText = "select unique(application_number) as col1 from  TMR_DEVICE_IMAGE_DETAILS where LAST_UPDATED_DATE>='" + textBox3.Text.ToString() + "' and LAST_UPDATED_DATE<='" + textBox2.Text.ToString() + "'";
                //cmd2.CommandText = "select app.application_number as col1 from  TMR_application_DETAILS app left outer join tmr_tmview_publish_details pub on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('" + textBox3.Text.ToString() + " 00:00:00','dd/MM/yyyy hh24:mi:ss')  and pub.LAST_UPDATED_DATE<=to_date('" + textBox2.Text.ToString() + " 23:59:59' ,'dd/MM/yyyy hh24:mi:ss')  and (publish_date < pub.last_updated_date or publish_date is null)  and app.tmr_application_status in ('REG','REC')";
                //cmd2.CommandText = "select app.application_number as col1 from  tmr_tmview_publish_details pub left outer join tmr_application_details app on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('06/12/2016 00:00:00 ' , 'dd/MM/yyyy hh24:mi:ss'  ) and pub.LAST_UPDATED_DATE<=to_date('07/12/2016 23:59:59 ' , 'dd/MM/yyyy hh24:mi:ss'  ) and (pub.publish_date <pub.last_updated_date or pub.publish_date is null)  and app.tmr_application_status in ('REG','REC')";
                cmd2.CommandText = "select app.application_number as col1 from  tmr_tmview_publish_details pub left outer join tmr_application_details app on pub.application_number=app.application_number where pub.LAST_UPDATED_DATE>=to_date('" + textBox3.Text + " ' , 'dd/MM/yyyy hh24:mi:ss'  ) and pub.LAST_UPDATED_DATE<=to_date('" + textBox2.Text + "' , 'dd/MM/yyyy hh24:mi:ss'  ) and (pub.publish_date <pub.last_updated_date or pub.publish_date is null)  and app.tmr_application_status in ('REG','REC')";
            }
            string[] applicaionnumbers=new string[500000];


            OracleDataReader dr2 = cmd2.ExecuteReader();
            //dr2.Read();
            int j = 0;
            int max = 0;

           
            while (dr2.Read())
            {
                applicaionnumbers[j] = dr2[0].ToString();
                j++;
                max = j;
             }

            if (max == 0)  // if blank application number found
            {
              // MessageBox.Show("No application number for this range");
               // Application.Exit();
            }
            else
            {

                j = 0;
                textBox4.Text = max.ToString();
                textBox4.Refresh();

                stdate = DateTime.Now;
                int appno = 0;
                
                string monthnumberstr = "";
                string daynumberstr = "";

                if (stdate.Month <= 9)
                {
                    monthnumberstr = "0" + stdate.Month.ToString();
                
                }
                else
                {
                    monthnumberstr = stdate.Month.ToString();
                
                }


                if (stdate.Day <= 9)
                {
                
                    daynumberstr = "0" + stdate.Day.ToString();
                }
                else
                {
                
                    daynumberstr = stdate.Day.ToString();
                }
                
                
                datestr = stdate.Year.ToString() + "-" + monthnumberstr + "-" + daynumberstr.ToString(); 
                

                rootf = "D:/";
                datef = datestr + "/";

                string localfixfoldernameimg1;
                string localfixfoldernamexml1;
                if (fullordiff == 0)
                {

                    localfixfoldernameimg1 = "-IN-FULL-INDX(IMGA)-";
                    localfixfoldernamexml1 = "-IN-FULL-INDX-";
                }
                else
                {
                                        localfixfoldernameimg1 = "-IN-DIFF-INDX(IMGA)-";
                                        localfixfoldernamexml1 = "-IN-DIFF-INDX-";
                }
                
                               

                string localfoldersequencestr = "0000";

                int folseqinitial = 0;
                cmd6.CommandText = "select nvl(max(ZIPFILE_SEQ),0)  from tmr_tmview_pub_seq  where trunc(transaction_date)='16-NOV-2016'";
                folseqinitial = Convert.ToInt32(cmd6.ExecuteScalar());


                int folderseqno = folseqinitial + 1;// initial folder seq for that  date/day   //select nvl(max(ZIPFILE_SEQ),0)  from tmr_tmview_pub_seq  where trunc(transaction_date)='16-NOV-2016'  
                string filename = "";
                string filenamepart1 = "-IN50";
                string filenamepart2 = "";
                string xmlcontent = "";
                int folno = 0;
                // pick from db 


            
                   for (j = 0; j <= max-1; j++) 
                {

                    appno = Convert.ToInt32(applicaionnumbers[j]);

                    if (j % 1000 == 0)
                    {
                        folno = folno + 1;
                        folnomax = folno;
                        string lpf = ""; // string for left padding zero 
                        int lnthf = 0;  // according to length of string the the remaining lenth will be padded by zero 

                        lnthf = folno.ToString().Length;
                        // ----loop for padding zero for folder name ...
                        for (int kf = 1; kf <= 4 - lnthf; kf++)
                        {
                            lpf = lpf + "0";
                        }

                        localfoldersequencestr = lpf + folno.ToString();
                        zippedalready = 1;
                        localfoldersequencelist[folno] = localfoldersequencestr;
                        localfoldersequencelistimg[folno] = localfoldersequencestr;
                    }


                    cmd1.CommandText = "SELECT trade_mark_image from TMR_DEVICE_IMAGE_DETAILS where image_number=1 and TRADE_MARK_IMAGE IS NOT NULL and application_number=" + appno;
                    //cmd.CommandText = "select publish_for_tmview(" + appno.ToString() + ") from dual "; // changes done 23june
                    cmd.CommandText = "select publish_for_tmview(" + appno.ToString() + "," + "'Insert'" + ") from dual ";

                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    xmlcontent = dr.GetString(0).ToString();
                    string lp = ""; // string for left padding zero 
                    int lnth = 0;  // according to length of string the the remaining lenth will be padded by zero 

                    lnth = appno.ToString().Length;
                    // ----loop for padding zero for file name ...
                    for (int k = 1; k <= 13 - lnth; k++)
                    {
                        lp = lp + "0";
                    }

                    filenamepart2 = lp + appno.ToString();


                    if (System.IO.Directory.Exists(rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr);
                    }
                    filename = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr + "/" + datestr + filenamepart1 + filenamepart2 + ".xml"; // modified  june23..
                    foldertosend1 = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    System.IO.File.WriteAllText(filename, xmlcontent);
                    foldertosend1 = rootf + datef + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    localfoldersequencelist[folno] = rootf + datef + "/zipped/xmls/" + datestr + localfixfoldernamexml1 + localfoldersequencestr;
                    foldernamesxml[folno] = foldertosend1;

                    OracleDataReader dr1 = cmd1.ExecuteReader();
                    if (dr1.Read())
                    {
                        OracleLob blob = dr1.GetOracleLob(0); // this will convert recieved data into BLOB data type variale
                        byte[] buffer = new byte[blob.Length];   // this will transfer blob data in byte stream according to length
                        int x = blob.Read(buffer, 0, System.Convert.ToInt32(blob.Length)); // this will measure the length of string 


                        filename = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr + "/" + datestr + filenamepart1 + filenamepart2 + ".jpg"; // changes done
                        //filename = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr + "/" + filenamepart1 + filenamepart2 + ".jpg";

                        foldertosend2 = rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        //localfoldersequencelistimg[folno] = rootf + datef + "/zipped/" + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        localfoldersequencelistimg[folno] = rootf + datef + "/zipped/images/" + datestr + localfixfoldernameimg1 + localfoldersequencestr;
                        foldernamesimg[folno] = foldertosend2;

                        if (System.IO.Directory.Exists(rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(rootf + datef + datestr + localfixfoldernameimg1 + localfoldersequencestr);
                        }

                        FileStream fs2 = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write); // complementry code for writing IMAGE data 
                        fs2.Write(buffer, 0, (int)blob.Length);  //================================ write IMAGE data into files 
                        filenamepart2 = "";
                        fs2.Close();
                    }
                       // - section for database update -- START 

                    if (checkBox1.Checked==true)
                    {
                        folderseqno = Convert.ToInt32(localfoldersequencestr);
                        cmd3.CommandText = "insert into  tmr_tmview_pub_seq(application_number,transaction_date,zipfile_seq) values(" + appno + "," + "sysdate" + "," + folderseqno + ")";

                        cmd3.ExecuteNonQuery();

                        cmd5.CommandText = "Select count(application_number) from tmr_tmview_publish_details where application_number=" + appno;
                        recordfound = Convert.ToInt32(cmd5.ExecuteScalar());

                        if (recordfound == 0)
                        {
                            cmd5.CommandText = "insert into tmr_tmview_publish_details(application_number,publish_date) values(" + appno + ",sysdate)";
                        }
                        else
                        {
                            cmd5.CommandText = "update tmr_tmview_publish_details set publish_date=sysdate where application_number=" + appno;
                        }

                        cmd5.ExecuteNonQuery();
                    }
                    else
                    {
                        
                    }

                        // - section for database update -- END 
                        
                    textBox1.Text = j.ToString();
                    textBox1.Refresh();

                }// end of loop j for application number

                //-- program to delete wrong XML files 
              //  deletewrongxml();

                // -- pe

                //if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/"))
                   if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/xmls/"))
                {
                }
                else
                {
//                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/");
                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/xmls");
                }

                //if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/"))
                   if (System.IO.Directory.Exists("D:/" + datestr + "/zipped/images/"))
                {
                }
                else
                {
                    //System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/");
                    System.IO.Directory.CreateDirectory("D:/" + datestr + "/zipped/images/");
                }



                for (folno = 1; folno <= folnomax; folno++)
                {
                    if (String.IsNullOrEmpty(foldernamesxml[folno]))
                    {
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(foldernamesxml[folno]))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(foldernamesxml[folno]);
                        }

                        if (System.IO.Directory.Exists(foldernamesimg[folno]))
                        {
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(foldernamesimg[folno]);
                        }

                        
                        ZipFile.CreateFromDirectory(foldernamesxml[folno], localfoldersequencelist[folno] + ".zip");
                        ZipFile.CreateFromDirectory(foldernamesimg[folno], localfoldersequencelistimg[folno] + ".zip");
                        
//                        Upload(localfoldersequencelist[folno] + ".zip","/Images/2016-11-17/","xmls");
//                        Upload(localfoldersequencelistimg[folno] + ".zip","/TM-XML/2016-11-17/","images");
                       
                    }

                } // end of for loop 

               // MessageBox.Show("Work done");
               // MessageBox.Show("at location D:/Errors.txt file generated for log of wrong xml files");
                button1.Text = "Generate Files";
                label1.Visible = true;
                label2.Visible = true;
                textBox2.Visible = true;
                textBox3.Visible = true;
            }
                       
//            WindowState = FormWindowState.Normal;
  //          FormBorderStyle = FormBorderStyle.Fixed3D;
            
        }

        
             
    }
}
