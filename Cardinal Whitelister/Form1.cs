using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Data.SQLite;

namespace Cardinal_Whitelister
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Cleanup and FTP Download of ur0:/shell/db/app.db
            if (File.Exists("app.db"))
            {
                File.Delete("app.db");
            }
            if (File.Exists("app.db.bak"))
            {
                File.Delete("app.db.bak");
            }
            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential("anonymous", "");
            byte[] fileData = request.DownloadData("ftp://" + IPTextBox.Text + ":" + PortTextBox.Text + "/ur0/shell/db/app.db");

            FileStream file = File.Create("app.db");
            file.Write(fileData, 0, fileData.Length);
            file.Close();

            //Backup app.db
            File.Copy("app.db", "app.db.bak");

            //Modify app.db with sqlite
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=app.db;Version=3;");
            m_dbConnection.Open();
            string sql = "CREATE TRIGGER CHANGE_CATEGORY_GPC AFTER INSERT ON tbl_appinfo WHEN new.val LIKE 'gpc' BEGIN UPDATE tbl_appinfo SET val='gdb' WHERE key='566916785' and titleid=new.titleid; END; CREATE TRIGGER CHANGE_CATEGORY_GP AFTER INSERT ON tbl_appinfo WHEN new.val LIKE 'gp' BEGIN UPDATE tbl_appinfo SET val='gdb' WHERE key='566916785' and titleid=new.titleid; END; CREATE TRIGGER CHANGE_CATEGORY_GDC AFTER INSERT ON tbl_appinfo WHEN new.val LIKE 'gdc' BEGIN UPDATE tbl_appinfo SET val='gdb' WHERE key='566916785' and titleid=new.titleid; END; CREATE TRIGGER CHANGE_CATEGORY_GD AFTER INSERT ON tbl_appinfo WHEN new.val LIKE 'gd' BEGIN UPDATE tbl_appinfo SET val='gdb' WHERE key='566916785' and titleid=new.titleid; END;";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            m_dbConnection.Close();

            //Delete remote app.db via FTP
            DeleteAppDB();

            //Upload app.db via FTP
            WebClient Client = new WebClient();
            Client.Credentials = new System.Net.NetworkCredential("anonymous", "");
            Client.BaseAddress = "ftp://" + IPTextBox.Text + ":" + PortTextBox.Text;
            Client.UploadFile(Client.BaseAddress + "/ur0/shell/db/app.db", "app.db");



        }
        private string DeleteAppDB()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + IPTextBox.Text + ":" + PortTextBox.Text + "/ur0/shell/db/app.db");
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential("anonymous", "");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response.StatusDescription;
            }
        }
    }
}
