using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;
using System.Xml;


namespace SipAAInstaller
{
    public partial class FmSipAAInstaller : Form
    {
        public FmSipAAInstaller()
        {
            InitializeComponent();
        }

        /**
         * 判断服务是否已安装
         * */
        public static bool ISWindowsServiceInstalled(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                    return true;
                
            }
            return false;
        }

        /**
         * 检查服务状态
         * ZQL
         * */
        public static bool IsServiceRunning(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            if (sc.Status.Equals(ServiceControllerStatus.Running))
                return true;
                return false;
            
        }


        /**
         * 关闭一个服务
         * ZQL  
         * */
        public static void StopService(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);//建立服务对象
                 sc.Stop();
                 sc.Refresh();     
        }

        /**
         * 设置一个服务启动项为自动（延迟启动）
         * ZQL  
         * */
        public static void ChangeServiceStartType(string serviceName)
        {

            int StartType = 2;//2为自动启动，3为手动启动
            int Delay = 1;//1为延迟启动，0为不延迟
            RegistryKey regist = Registry.LocalMachine;
            RegistryKey sysReg = regist.OpenSubKey("SYSTEM");
            RegistryKey currentControlSet = sysReg.OpenSubKey("CurrentControlSet");
            RegistryKey services = currentControlSet.OpenSubKey("Services");
            RegistryKey servicesName = services.OpenSubKey(serviceName, true);
            //获取Start的值
            //Convert.ToInt16(servicesName.GetValue("Start"));

            //获取DelayedAutostart的值
            //Convert.ToInt16(servicesName.GetValue("DelayedAutostart"));
            //if()
            servicesName.SetValue("Start",StartType);
            servicesName.SetValue("DelayedAutostart", Delay);
        }

        /**
         * 判断webconfig文件
         * ZQL  
         * */
        public bool CheckWebConfig(string path)
        {
            if (!File.Exists(path))
            {
                ContinueStopService.Visible = false;
                ContinueCopy.Visible = false;
                FirstStart.Visible = false;
                buttonInstall.Visible = false;


                UserName.Visible = false;
                PassWord.Visible = false;
                Unamebox.Visible = false;
                PWDBox.Visible = false;
                UserConfirm.Visible = false;
                ConfirmButton.Visible = false;
                Cancel1.Visible = false;
                CPassWord.Visible = false;
                CPWD.Visible = false;
                Cancel.Visible = true;
                Cancel.Text = "退出";
                labelInfo.Visible = true;
                labelInfo.Text = "文件" + path + "不存在，请安装完SIPAA管理网站后再执行本安装程序，点击“退出”取消安装。";
                return false;

            }
            else
            {
               
                return true;
            }

        }
        /**
         * 修改webconfig文件
         * ZQL  
         * */
        //public void EditWebConfig(string path, string username, string password)
        //{
        //    File.WriteAllText(path, File.ReadAllText(path).Replace("doug",username));
        //    File.WriteAllText(path, File.ReadAllText(path).Replace("771707@mozatt", password));
        
        //}
        public static void EditWebConfig(string path, string userName, string password)
        {
            string fileName = path;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);
            XmlNode xnuser=xmlDoc.SelectSingleNode("configuration/system.web/identity");
            xnuser.Attributes["userName"].InnerText = userName;
            xnuser.Attributes["password"].InnerText = password;
            xmlDoc.Save(fileName);
            
            
        }



        /**
         * 修改文件内容的方法（多少行，修改为的内容，文件路径）
         * ZQL
         * */
        public void EditFile(int curLine, string newLineValue, string patch)
        {
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            string line = sr.ReadLine();
            StringBuilder sb = new StringBuilder();
            for (int i = 1; line != null; i++)
            {
                sb.Append(line + "\r\n");
                if (i != curLine - 1)
                    line = sr.ReadLine();
                else
                {
                    sr.ReadLine();
                    line = newLineValue;
                }
            }
            sr.Close();
            fs.Close();
            FileStream fs1 = new FileStream(patch, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1, Encoding.GetEncoding("utf-8"));
            sw.Write(sb.ToString());
            sw.Close();
            fs1.Close();
        }

        /**
        * 验证输入的windows用户名密码是否正确
        * ZQL  
        * */
        public  bool CheckCurrentNamepwd(string username,string password)
        {
            //string WinUser = "a";
            //string WinPwd = "a";
            //if (WinUser==username&&WinPwd==password)
            //{
                ContinueStopService.Visible = false;
                Cancel.Visible = false;
                ContinueCopy.Visible = false;
                FirstStart.Visible = false;
                labelInfo.Visible = true;
                buttonInstall.Visible = true;

                UserName.Visible = false;
                PassWord.Visible = false;
                Unamebox.Visible = false;
                PWDBox.Visible = false;
                UserConfirm.Visible = false;
                ConfirmButton.Visible = false;
                Cancel1.Visible = false;
                CPassWord.Visible = false;
                CPWD.Visible = false;
                return true;
            //}
            //else
            //{
               
            //    return false;
            
            //}
        }

        

        /**
         * 执行首次启动一个服务，running后停止服务
         * ZQL  
         * */
        public static void FirstStartService(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);//建立服务对象
            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running);
            sc.Refresh();
          
            if (sc.Status.Equals(ServiceControllerStatus.Running))
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped);
            sc.Refresh();
        }


        /**
         * 判断FreeSwitch安装目录下是否存在文件夹db、Scripts、conf、conf\dialplan文件夹
         * ZQL  
         * */

        public static bool CheckDocumentsExists(string path)
        {
            string pathDb = path + "db";
            string pathScripts = path + "Scripts";
            string pathConf = path + "conf\\autoload_configs";
            string pathConfdialplan = path + "conf\\dialplan";

            
            bool ExistsDb = Directory.Exists(pathDb);
            bool ExistsScripts = Directory.Exists(pathScripts);
            bool ExistsConf = Directory.Exists(pathConf);
            bool ExistsConfdialplan = Directory.Exists(pathConfdialplan);

            return ExistsDb && ExistsScripts && ExistsConf & ExistsConfdialplan;
        }

        /**
         * 备份文件
         * ZQL  
         * */
        public static void BackupCopy(string OrignFile,string NewFile)
        {
            string file1 = OrignFile + "db\\sipaacfg.db";
            string file2 = OrignFile + "db\\sipaarecs.db";
            string file3 = OrignFile + "Scripts\\sipaa.lua";
            string file4 = OrignFile + "Scripts\\tsimplify.lua";
            string file5 = OrignFile + "conf\\autoload_configs\\logfile.conf.xml";
            string file6 = OrignFile + "conf\\dialplan\\public.xml";

            //获取当前时间
            string NewDocumentName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            NewFile = NewFile + "\\" + NewDocumentName;

            System.IO.Directory.CreateDirectory(NewFile);

            string file11 = NewFile + "\\sipaacfg.db";
            string file22 = NewFile + "\\sipaarecs.db";
            string file33 = NewFile + "\\sipaa.lua";
            string file44 = NewFile + "\\tsimplify.lua";
            string file55 = NewFile + "\\logfile.conf.xml";
            string file66 = NewFile + "\\public.xml";

            if(File.Exists(file1))
            File.Copy(file1, file11, true);

            if (File.Exists(file2))
            File.Copy(file2, file22, true);

            if (File.Exists(file3))
            File.Copy(file3, file33, true);

            if (File.Exists(file4))
            File.Copy(file4, file44, true);

            if (File.Exists(file5))
            File.Copy(file5, file55, true);

            if (File.Exists(file6))
            File.Copy(file6, file66, true);
   
        }
        /**
         * 还原文件
         * ZQL  
         * */
        public static void RollBack(string OrignFile, string NewFile)
        {
            string file1 = OrignFile + "db\\sipaacfg.db";
            string file2 = OrignFile + "db\\sipaarecs.db";
            string file3 = OrignFile + "Scripts\\sipaa.lua";
            string file4 = OrignFile + "Scripts\\tsimplify.lua";
            string file5 = OrignFile + "conf\\autoload_configs\\logfile.conf.xml";
            string file6 = OrignFile + "conf\\dialplan\\public.xml";

            string file11 = NewFile + "\\sipaacfg.db";
            string file22 = NewFile + "\\sipaarecs.db";
            string file33 = NewFile + "\\sipaa.lua";
            string file44 = NewFile + "\\tsimplify.lua";
            string file55 = NewFile + "\\logfile.conf.xml";
            string file66 = NewFile + "\\public.xml";

            if (File.Exists(file1))
                File.Delete(file1);
            File.Copy(file11, file1, true);

            if (File.Exists(file2))
                File.Delete(file2);
            File.Copy(file22, file2, true);

            if (File.Exists(file3))
                File.Delete(file3);
            File.Copy(file33, file3, true);

            if (File.Exists(file4))
                File.Delete(file4);
            File.Copy(file44, file4, true);

            if (File.Exists(file5))
                File.Delete(file5);
            File.Copy(file55, file5, true);

            if (File.Exists(file6))
                File.Delete(file6);
            File.Copy(file66, file6, true); 

        }
        /**
         * 替换文件
         * ZQL 
         * */
        public static void NewFileCopy(string OrignFile, string NewFile)
        {
            string file1 = OrignFile + "db\\sipaacfg.db";
            //MessageBox.Show("文件路径" + file1);
            string file2 = OrignFile + "db\\sipaarecs.db";
            string file3 = OrignFile + "Scripts\\sipaa.lua";
            string file4 = OrignFile + "Scripts\\tsimplify.lua";
            string file5 = OrignFile + "conf\\autoload_configs\\logfile.conf.xml";
            string file6 = OrignFile + "conf\\dialplan\\public.xml";

            string file11 = NewFile + "\\sipaacfg.db";
            string file22 = NewFile + "\\sipaarecs.db";
            string file33 = NewFile + "\\sipaa.lua";
            string file44 = NewFile + "\\tsimplify.lua";
            string file55 = NewFile + "\\logfile.conf.xml";
            string file66 = NewFile + "\\public.xml";

            if (File.Exists(file1))
                File.Delete(file1);
            File.Copy(file11, file1, true);

            if (File.Exists(file2))
                File.Delete(file2);
            File.Copy(file22, file2, true);

            if (File.Exists(file3))
                File.Delete(file3);
            File.Copy(file33, file3, true);

            if (File.Exists(file4))
                File.Delete(file4);
            File.Copy(file44, file4, true);

            if (File.Exists(file5))
                File.Delete(file5);
            File.Copy(file55, file5, true);

            if (File.Exists(file6))
                File.Delete(file6);
            File.Copy(file66, file6, true);

        }



        /**
        * 判断对Path 是否有 读写改删的权限
        *  
        * */
        private bool checkPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            System.Security.AccessControl.AuthorizationRuleCollection rules = null;
            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
            }
            catch
            {
                return true;
            }
            try
            {
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!current.User.Equals(rule.IdentityReference))
                    {
                        continue;
                    }
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                        {
                            flag8 = true;
                        }
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                        {
                            flag7 = true;
                        }
                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                        {
                            flag5 = true;
                        }
                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                        {
                            flag6 = true;
                        }
                    }
                }
                foreach (IdentityReference reference in current.Groups)
                {
                    foreach (FileSystemAccessRule rule2 in rules)
                    {
                        if (!reference.Equals(rule2.IdentityReference))
                        {
                            continue;
                        }
                        if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag4 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag3 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag2 = true;
                            continue;
                        }
                        if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag8 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag7 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag5 = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag6 = true;
                        }
                    }
                }
                bool flag9 = !flag4 && flag8;
                bool flag10 = !flag3 && flag7;
                bool flag11 = !flag && flag5;
                bool flag12 = !flag2 && flag6;
                bool flag13 = true;
                if (checkRead)
                {
                    flag13 = flag13 && flag11;
                }
                if (checkWrite)
                {
                    flag13 = flag13 && flag12;
                }
                if (checkModify)
                {
                    flag13 = flag13 && flag10;
                }
                if (checkDelete)
                {
                    flag13 = flag13 && flag9;
                }
                return flag13;
            }
            catch (IOException)
            {
            }
            return false;
        }

        /**
        * 获取FREESWITCH的安装路径
        *  
        * */
        private string GetRegistData(string name) 
        { 
            string registData="";
            RegistryKey hkml = Registry.LocalMachine; 
            RegistryKey software = hkml.OpenSubKey("SYSTEM\\CurrentControlSet\\Services", true); 
            RegistryKey aimdir = software.OpenSubKey("FreeSWITCH",true);
            //if (aimdir != null)
            //{
                //labelService.Text = "OpenKey suc.    ";
                registData = aimdir.GetValue(name).ToString();
                //labelService.Text += registData;
            //}
            //else
            //    labelService.Text = "OpenKey fail.";
            return registData; 
        } 
        /**
         * 
         * 程序开始
         * */
        private void FmSipAAInstaller_Load(object sender, EventArgs e)
        {
            string WebConfigFile = "c:\\inetpub\\wwwroot\\sipaa\\web.config";
            
            
            if(CheckWebConfig(WebConfigFile))
            {
                ContinueStopService.Visible = false;
                Cancel.Visible = false;
                ContinueCopy.Visible = false;
                FirstStart.Visible = false;
                labelInfo.Visible = false;
                buttonInstall.Visible = false;

                UserName.Visible = true;
                PassWord.Visible = true;
                CPassWord.Visible = true;
                Unamebox.Visible = true;
                PWDBox.Visible = true;
                CPWD.Visible = true;
                UserConfirm.Visible = true;
                ConfirmButton.Visible = true;
                Cancel1.Visible = true;
                
                
                UserConfirm.Text = "请输入本机的用户名、密码，输入的用户名密码将写入到web.config配置项中，输入完成后点击“确认”继续安装，点击“退出”将退出安装。";
            }


        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            buttonInstall.Visible = false;
            if (IsServiceRunning("FreeSWITCH"))
            {
                ContinueStopService.Visible = true;
                Cancel.Visible = true;

                labelInfo.Text = "FreeSWITCH服务正在运行，点击“下一步”将停止FreeSWITCH服务后继续安装，点击“取消”将退出安装。";
            }
            else 
            {
                labelInfo.Text = "FreeSWITCH服务处于停止状态，点击“下一步”将拷贝文件，点击“取消”将退出安装。";
                //将服务修改为自动（延迟启动）
                ChangeServiceStartType("FreeSWITCH");
                ContinueCopy.Visible = true;
                Cancel.Visible = true;
            
            }


        }

        private void ContinueStopService_Click(object sender, EventArgs e)
        {
            ContinueStopService.Enabled = false;
            StopService("FreeSWITCH");
            ContinueStopService.Visible = false;
            
            ContinueCopy.Visible = true;
            //将服务修改为自动（延迟启动）
            ChangeServiceStartType("FreeSWITCH");
            labelInfo.Text = "FreeSWITCH服务已停止，点击“下一步”将拷贝文件，点击“取消”将退出安装。";

        }

        private void CancelWithSvcRun_Click(object sender, EventArgs e)
        {
            //退出程序安装
            Application.Exit();

        }

        private void ContinueCopy_Click(object sender, EventArgs e)
        {
            //拷贝文件 ZQL
            //FSpath -- FreeSwith安装的路径
            string FSpath = GetRegistData("ImagePath");
            

            FSpath = FSpath.Substring(1, FSpath.LastIndexOf("\\") );
            labelInfo.Text = FSpath;

            //判断当前用户对FreeSwith安装的路径是否有 读写改删的权限

            bool AllPermissions = checkPermissions(FSpath, true, true, true, true);
            if (!AllPermissions)
            {

                labelInfo.Text ="当前用户权限不够，请退出后切换至管理员权限的用户安装。";
                Cancel.Visible = true;
                ContinueCopy.Enabled = false;

            }
            //检查FreeSwitch安装目录是否完整
            else if (!CheckDocumentsExists(FSpath))
            {
                labelInfo.Text = "文件夹db、Scripts、conf、dialplan不存在，FreeSwitch可能还未启动过，点击“下一步”执行首次启动，启动过程中界面可能无法响应，请耐心等待，点击“取消”退出安装。";
                ContinueCopy.Visible = false;
                FirstStart.Visible = true;
            }
            else
            {
                //当前安装程序所在的目录
                string CurrentPath = System.Environment.CurrentDirectory;
                //备份文件
                BackupCopy(FSpath, CurrentPath);
                //替换文件
                NewFileCopy(FSpath, CurrentPath);

                labelInfo.Text = "文件部署成功。";
                ContinueCopy.Visible = false;

                Cancel.Text = "退出程序";

            }


        }

        private void FirstStart_Click(object sender, EventArgs e)
        {
            FirstStart.Visible = false;
            FirstStartService("FreeSWITCH");
            labelInfo.Text = "服务FreeSwitch首次启动已完成，点击“下一步”将拷贝文件，点击“取消”将退出安装。";
            
            ContinueCopy.Visible = true;
        }
        public void BeginInstall()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            bool installed = ISWindowsServiceInstalled("FreeSWITCH");

            if (installed == false)
            {

                buttonInstall.Visible = false;
                Cancel.Visible = true;
                Cancel.Text = "退出";
                labelInfo.Text = "未检测到FreeSWITCH服务，请先安装FreeSWITCH服务，再执行此安装程序。当前用户：" + current.Name+"。";
            }
            else
            {

                labelInfo.Text = "检测到FreeSWITCH服务已安装。当前用户：" + current.Name + "，点击“下一步”继续安装，点击“取消”退出安装。";
                buttonInstall.Enabled = true;
                Cancel.Visible = true;

            }
        }

        private void Cancel1_Click(object sender, EventArgs e)
        {
            //退出程序安装
            Application.Exit();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            string username = Unamebox.Text;
            string password = PWDBox.Text;
            string Cpassword = CPWD.Text;
            string WebConfigFile = "c:\\inetpub\\wwwroot\\sipaa\\web.config";

            if (!(password == Cpassword))
            {
                UserConfirm.Text = "您所输入的密码前后不匹配，请重新输入。";
                
            }
            else
            {
                //验证用户名密码
                if (!CheckCurrentNamepwd(username, password))
                {
                    UserConfirm.Text = "您所输入的用户名或密码不正确，请重新输入。";
                    //UserConfirm.ForeColor = "red";
                }
                else
                {
                    EditWebConfig(WebConfigFile, username, password);
                    BeginInstall();
                }
            }

        }

  
    }
}
