namespace AutoNetLoad
{
    using Microsoft.VisualBasic;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class FormAutoLoad : Form
    {
        public const bool Alert = false;
        public const string RootKeyName = "HKEY_LOCAL_MACHINE";
        public static bool Is64Bit = (IntPtr.Size == 8);
        public Regex FYDLLNameRegex = new Regex(@"^飞鹰\d{4}[(]For\w+[)]\.dll$");
        private bool checkedListInitialized = false;
        private bool zwCheckedListInitialized = false;
        private readonly List<RegistryKey> autoCADVersions = new List<RegistryKey>();
        private readonly string dllName2007 = "飞鹰2020(ForCAD2007)";
        private readonly string productName2007CAD = "AutoCAD 2007";
        private readonly string dllName2020 = "飞鹰2020(ForCAD2020)";
        private readonly string productName2020CAD = "AutoCAD 2020 - 简体中文 (Simplified Chinese)";
        private readonly IContainer components = null;
        private Label FilePath_Text;
        private TextBox FeiYinGFilePath;
        private Label APPName_Text;
        private Label APPDes_text;
        private TextBox textBoxApp;
        private TextBox textBoxAppDesc;
        private Button buttonBrowse;
        private OpenFileDialog openFileDialog1;
        private Label LoadNET_Text;
        private Button AddStart;
        private Button DelStart;
        private Button buttonExit;
        private ListView listViewAssembly;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label ACADSeletct_Text;
        private ComboBox IntACADList;
        private Button Unt2007;
        private Button Int2007;
        private Button Unt2020;
        private Button Int2020;
        private CheckedListBox ACADListBox;
        private Panel panel1;
        private Label ACAD_Text;
        private Label ZWCAD_Text;
        private Button buttonOpen;
        private CheckedListBox ZwCADListBox;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FormAutoLoad()
        {
            this.InitializeComponent();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AllCADVersionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listViewAssembly.Items.Clear();
            RegistryKey key = Registry.CurrentUser.CreateSubKey(this.GetAutoCADKeyName(this.IntACADList.Text) + @"\Applications");
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(subKeyName);
                if (subKey.GetValue("MANAGED") != null)
                {
                    ListViewItem item = new ListViewItem(subKeyName)
                    {
                        SubItems = { subKey.GetValue("LOADER").ToString() }
                    };
                    this.listViewAssembly.Items.Add(item);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Unt2007_But(object sender, EventArgs e)
        {
            _ = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + this.dllName2007 + ".dll";
            if (MessageBox.Show("你确实想删除" + this.dllName2007 + "注册表项吗？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this.RemoveDemandLoadingEntries(this.productName2007CAD, this.dllName2007, true, CADType.AutoCAD))
                {
                    MessageBox.Show("删除成功");
                }
                else
                {
                    MessageBox.Show("删除失败，没有找到该项信息");
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Int2007_But(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + this.dllName2007 + ".dll";
            if (!File.Exists(path))
            {
                MessageBox.Show("文件不存在，添加失败");
            }
            else if (this.CreateDemandLoadingEntries(this.productName2007CAD, this.dllName2007, this.dllName2007, path, true, false, 2, CADType.AutoCAD))
            {
                MessageBox.Show("添加成功！");
            }
            else
            {
                MessageBox.Show("已添加，不要重复添加！");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Unt2020_But(object sender, EventArgs e)
        {
            _ = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + this.dllName2020 + ".dll";
            if (MessageBox.Show("你确实想删除" + this.dllName2020 + "注册表项吗？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this.RemoveDemandLoadingEntries(this.productName2020CAD, this.dllName2020, true, CADType.AutoCAD))
                {
                    MessageBox.Show("删除成功");
                }
                else
                {
                    MessageBox.Show("删除失败，没有找到该项信息");
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Int2020_But(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + this.dllName2020 + ".dll";
            if (!File.Exists(path))
            {
                MessageBox.Show("文件不存在，添加失败");
            }
            else if (this.CreateDemandLoadingEntries(this.productName2020CAD, this.dllName2020, this.dllName2020, path, true, false, 2, CADType.AutoCAD))
            {
                MessageBox.Show("添加成功！");
            }
            else
            {
                MessageBox.Show("已添加，不要重复添加！");
            }
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (this.textBoxApp.Text == "")
            {
                MessageBox.Show("请浏览到要加载的应该程序路径下！");
            }
            else
            {
                string appName = this.textBoxApp.Text;
                string appPath = this.FeiYinGFilePath.Text;
                this.CreateDemandLoadingEntries(this.IntACADList.Text, appName, this.textBoxAppDesc.Text, appPath, true, false, 2, CADType.AutoCAD);
                ListViewItem item = new ListViewItem(appName)
                {
                    SubItems = { appPath }
                };
                bool isDuplicate = false;
                foreach (ListViewItem listViewItem in this.listViewAssembly.Items)
                {
                    if (listViewItem.SubItems[1].Text.ToString().Equals(appPath))
                    {
                        isDuplicate = true;
                        MessageBox.Show("该程序已经添加到启动组,请勿重复添加！");
                        break;
                    }
                }
                if (!isDuplicate)
                {
                    this.listViewAssembly.Items.Add(item);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.FeiYinGFilePath.Text = this.openFileDialog1.FileName;
                this.textBoxApp.Text = Path.GetFileNameWithoutExtension(this.FeiYinGFilePath.Text);
                this.textBoxAppDesc.Text = this.textBoxApp.Text;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.listViewAssembly.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择要删除的应用程序！");
            }
            else
            {
                string appName = this.listViewAssembly.SelectedItems[0].Text;
                if ((appName != null) && ((MessageBox.Show("你确实想删除" + appName + "注册表项吗？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes) && this.RemoveDemandLoadingEntries(this.IntACADList.Text, appName, true, CADType.AutoCAD)))
                {
                    this.listViewAssembly.Items.Remove(this.listViewAssembly.SelectedItems[0]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ButtonExit_Click(object sender, EventArgs e)
        {
            base.Dispose();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.checkedListInitialized)
            {
                int startNumber = 0;
                string sourceText = this.ACADListBox.Items[e.Index].ToString();
                string versionNumber = GetLastNumberFromText(sourceText, ref startNumber);
                string appName = "飞鹰2020(ForCAD" + versionNumber + ")";
                string appPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + appName + ".dll";
                if (this.ACADListBox.GetItemChecked(e.Index))
                {
                    this.RemoveDemandLoadingEntries(sourceText, appName, true, CADType.AutoCAD);
                }
                else
                {
                    this.CreateDemandLoadingEntries(sourceText, appName, appName, appPath, true, false, 2, CADType.AutoCAD);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CheckedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.zwCheckedListInitialized)
            {
                string productName = this.ZwCADListBox.Items[e.Index].ToString();
                string versionNumber = this.ZwCADListBox.Items[e.Index].ToString();
                string appName = "飞鹰2020(ForCADZW" + versionNumber + ")";
                string appPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\程序文件\" + appName + ".dll";
                if (this.ZwCADListBox.GetItemChecked(e.Index))
                {
                    this.RemoveDemandLoadingEntries(productName, appName, true, CADType.ZWCAD);
                }
                else
                {
                    this.CreateDemandLoadingEntries(productName, appName, appName, appPath, false, false, 2, CADType.ZWCAD);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CheckLoaded(string cadVersionName)
        {
            bool isLoaded = false;
            RegistryKey key = Registry.CurrentUser.CreateSubKey(this.GetAutoCADKeyName(cadVersionName) + @"\Applications");
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(subKeyName);
                if (subKey.GetValue("MANAGED") != null)
                {
                    string path = subKey.GetValue("LOADER").ToString();
                    if (this.FYDLLNameRegex.IsMatch(Path.GetFileName(path)))
                    {
                        isLoaded = true;
                        break;
                    }
                }
            }
            return isLoaded;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CheckZWLoaded(string zwProductName)
        {
            bool isLoaded = false;
            this.GetZWKey(zwProductName);
            if (!Environment.Is64BitOperatingSystem)
            {
            }
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)0x100).OpenSubKey(this.GetZWKey(zwProductName) + @"\Applications");
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(subKeyName);
                if (subKey.GetValue("MANAGED") != null)
                {
                    string path = subKey.GetValue("LOADER").ToString();
                    if (this.FYDLLNameRegex.IsMatch(Path.GetFileName(path)))
                    {
                        isLoaded = true;
                        break;
                    }
                }
            }
            return isLoaded;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool CreateDemandLoadingEntries(string productName, string appName, string appDesc, string appPath, bool currentUser, bool overwrite, int loadCtrlsFlag, CADType cadType = 0)
        {
            bool isSuccess;
            string autoCADKeyName = "";
            if (cadType == CADType.AutoCAD)
            {
                autoCADKeyName = this.GetAutoCADKeyName(productName);
            }
            if (cadType == CADType.ZWCAD)
            {
                autoCADKeyName = this.GetZWKey(productName);
            }
            RegistryKey baseKey = currentUser ? Registry.CurrentUser : Registry.LocalMachine;
            if (ReferenceEquals(baseKey, Registry.LocalMachine))
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)0x100);
            }
            RegistryKey appKey = baseKey.OpenSubKey(autoCADKeyName + @"\Applications", true);
            if (!overwrite && appKey.GetSubKeyNames().Contains<string>(appName))
            {
                isSuccess = false;
            }
            else
            {
                RegistryKey newAppKey = appKey.CreateSubKey(appName);
                newAppKey.SetValue("DESCRIPTION", appDesc, RegistryValueKind.String);
                newAppKey.SetValue("LOADCTRLS", loadCtrlsFlag, RegistryValueKind.DWord);
                newAppKey.SetValue("LOADER", appPath, RegistryValueKind.String);
                newAppKey.SetValue("MANAGED", 1, RegistryValueKind.DWord);
                isSuccess = true;
            }
            return isSuccess;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void FormAutoLoad_Load(object sender, EventArgs e)
        {
            // 定义注册表的父路径 "HKEY_LOCAL_MACHINE"，用于后续的注册表读取
            string parentKeyName = "HKEY_LOCAL_MACHINE";
            // 注册表键名 "ProductName"，用于获取产品名称
            string keyName = "ProductName";

            // 获取与 AutoCAD 相关的注册表键名称列表
            List<string> autoCADKeyNames = this.GetAutoCADKeyNames(CADType.AutoCAD);

            // 遍历所有 AutoCAD 注册表键名
            for (int i = 0; i < autoCADKeyNames.Count; i++)
            {
                // 获取当前 AutoCAD 注册表键名称
                string cadName;
                string keyPath = autoCADKeyNames[i];

                // 获取 HKEY_LOCAL_MACHINE 注册表项
                RegistryKey localMachine = Registry.LocalMachine;

                // 如果是 32 位系统，执行此处的代码（Is64Bit 变量表示是否为 64 位系统）
                if (!Is64Bit)
                {
                    // 可能是为 32 位系统处理的一部分，但此部分暂时没有代码
                }

                // 打开当前 AutoCAD 注册表键
                RegistryKey subKey = localMachine.OpenSubKey(keyPath);

                // 如果 subKey 为 null，表示该注册表路径不存在，则调用 `Utility.Get64BitRegistryKey()` 方法获取 64 位注册表路径
                if (subKey != null && subKey.GetValue(keyName) != null)
                {
                    cadName = subKey.GetValue(keyName).ToString();
                }
                else
                {
                    cadName = Utility.Get64BitRegistryKey(parentKeyName, keyPath, keyName);
                }

                // 如果 cadName 非空，将其添加到 ComboBox 和 CheckedListBox 中
                if (cadName != "")
                {
                    // 将 AutoCAD 版本信息添加到下拉框（ComboBox）
                    this.IntACADList.Items.Add(cadName);

                    // 将 AutoCAD 版本信息添加到 CheckedListBox
                    int index = this.ACADListBox.Items.Add(cadName);

                    // 如果该 AutoCAD 版本已加载，默认勾选该项
                    if (this.CheckLoaded(cadName))
                    {
                        this.ACADListBox.SetItemChecked(index, true);
                    }
                }
            }

            // 设置 ComboBox 完成初始化
            this.checkedListInitialized = true;

            // 如果 ComboBox 中有项目，默认选择第一个项目
            if (this.IntACADList.Items.Count != 0)
            {
                this.IntACADList.SelectedIndex = 0;
            }

            // 获取与 ZWCAD 相关的所有版本信息，并添加到第二个 CheckedListBox（zwCheckedListBox）
            foreach (string zwVersion in this.GetCADVersions(CADType.ZWCAD))
            {
                // 将 ZWCAD 版本信息添加到第二个 CheckedListBox 中，并根据 `CheckZWLoaded()` 方法的返回值决定是否选中
                this.ZwCADListBox.Items.Add(zwVersion, this.CheckZWLoaded(zwVersion));
            }

            // 设置 ZWCAD 的 CheckedListBox 完成初始化
            this.zwCheckedListInitialized = true;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetAutoCADKeyName(string productName)
        {
            // 获取与 AutoCAD 相关的注册表键名称列表
            List<string> autoCADKeyNames = this.GetAutoCADKeyNames(CADType.AutoCAD);

            // 获取 HKEY_LOCAL_MACHINE 注册表项
            RegistryKey localMachine = Registry.LocalMachine;

            // 如果是 64 位系统，可以在这里处理 64 位注册表的特殊逻辑
            if (Is64Bit)
            {
                // 64 位系统的处理逻辑，当前没有实现的部分
            }

            // 使用 Enumerator 遍历 autoCADKeyNames 列表
            using (List<string>.Enumerator enumerator = autoCADKeyNames.GetEnumerator())
            {
                // 使用 while 循环遍历列表中的每个 AutoCAD 注册表键
                while (true)
                {
                    // 获取当前注册表键名
                    string productNameValue;

                    // 判断是否还有下一个元素，如果没有则退出循环
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    // 获取当前的 AutoCAD 注册表键名称
                    string currentKey = enumerator.Current;

                    // 打开当前注册表键
                    RegistryKey subKey = localMachine.OpenSubKey(currentKey);

                    // 如果当前键为 null，表示该路径无效，尝试从 64 位注册表中获取键值
                    if (subKey != null)
                    {
                        object productNameObj = subKey.GetValue("ProductName");
                        if (productNameObj != null)
                        {
                            productNameValue = productNameObj.ToString();
                        }
                        else
                        {
                            productNameValue = Utility.Get64BitRegistryKey("HKEY_LOCAL_MACHINE", currentKey, "ProductName");
                        }
                    }
                    else
                    {
                        productNameValue = Utility.Get64BitRegistryKey("HKEY_LOCAL_MACHINE", currentKey, "ProductName");
                    }

                    // 如果找到的 "ProductName" 与传入的参数匹配，则返回当前的注册表键名
                    if (productName == productNameValue)
                    {
                        return currentKey;
                    }
                }
            }

            // 如果没有找到匹配的注册表键，返回空字符串
            return "";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public List<string> GetAutoCADKeyNames(CADType cadType = 0)
        {
            // 创建一个空的 List 用于存储最后的结果
            List<string> resultList;
            List<string> list = new List<string>();

            // 获取当前用户的注册表路径
            RegistryKey currentUserReg = Registry.CurrentUser;

            // 获取与指定 CAD 类型相关的注册表路径
            RegistryKey cadReg = currentUserReg.OpenSubKey(this.GetCADPath(cadType));

            // 如果指定的注册表路径不存在
            if (cadReg is null)
            {
                // 如果是 ZWCAD 类型，则没有额外的处理逻辑
                if ((cadType != CADType.AutoCAD) && (cadType == CADType.ZWCAD))
                {
                    // 没有进一步处理
                }

                // 将返回结果设置为一个空列表
                resultList = list;
            }
            else
            {
                // 创建正则表达式对象，用于匹配以 "R" 开头的子键
                Regex regex = new Regex("R*.*");

                // 获取该路径下所有子键的名称
                string[] subKeyNames = cadReg.GetSubKeyNames();

                int index = 0;

                // 遍历所有子键
                while (true)
                {
                    // 如果遍历完所有子键，退出循环
                    if (index >= subKeyNames.Length)
                    {
                        // 设置返回结果为列表，结束方法
                        resultList = list;
                        break;
                    }

                    // 获取当前子键名称
                    string input = subKeyNames[index];

                    // 如果子键名称符合正则表达式规则（以 "R" 开头）
                    if (regex.IsMatch(input))
                    {
                        // 打开该子键
                        RegistryKey nowKey = cadReg.OpenSubKey(input);

                        // 获取子键中的 "CurVer" 键值（表示当前版本）
                        object verCAD = nowKey.GetValue("CurVer");

                        // 如果 "CurVer" 键值存在
                        if (verCAD != null)
                        {
                            RegistryKey verKey = nowKey.OpenSubKey(verCAD.ToString());
                            if (verKey != null)
                            {
                                list.Add(verKey.Name.Substring(currentUserReg.Name.Length + 1));
                            }
                        }
                    }

                    // 增加索引，继续遍历下一个子键
                    index++;
                }
            }

            // 返回结果列表
            return resultList;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        // 获取与指定 CAD 类型相关的注册表路径
        private string GetCADPath(CADType cadType = 0)
        {
            string path; // 用于存储返回的注册表路径字符串

            // 根据传入的 CAD 类型选择不同的注册表路径
            switch (cadType)
            {
                // 如果 CAD 类型是 AutoCAD，返回 AutoCAD 的注册表路径
                case CADType.AutoCAD:
                    path = @"Software\Autodesk\AutoCAD";
                    break;

                // 如果 CAD 类型是 ZWCAD，返回 ZWCAD 的注册表路径
                case CADType.ZWCAD:
                    path = @"Software\ZWSOFT\ZWCAD";
                    break;

                // 如果 CAD 类型是 GRCAD，返回 GstarCAD 的注册表路径
                case CADType.GRCAD:
                    path = @"Software\Gstarsoft\GstarCAD";
                    break;

                // 如果 CAD 类型是 OtherCAD，返回空字符串
                case CADType.OtherCAD:
                    path = "";
                    break;

                // 默认情况下，返回空字符串
                default:
                    path = "";
                    break;
            }

            // 返回最终的注册表路径
            return path;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        // 获取与指定 CAD 类型相关的所有版本信息
        public List<string> GetCADVersions(CADType cadType = CADType.ZWCAD)
        {
            List<string> resultList; // 用于存储最终返回的版本列表
            List<string> list = new List<string>(); // 临时列表，用于存储遍历过程中的版本信息

            // 打开当前用户注册表下与指定 CAD 类型相关的路径
            RegistryKey cadReg = Registry.CurrentUser.OpenSubKey(this.GetCADPath(cadType));

            // 如果找不到对应的注册表键，则直接返回空列表
            if (cadReg is null)
            {
                // 如果找不到 ZWCAD 的注册表路径，可以在此进行额外处理（这里的逻辑为空）
                if (cadType != CADType.ZWCAD)
                {
                    // 可以在这里添加其他处理逻辑（比如日志记录或错误处理）
                }
                // 直接返回空的列表
                resultList = list;
            }
            else
            {
                // 获取该注册表键下所有子键的名称（即所有安装的版本名称）
                string[] subKeyNames = cadReg.GetSubKeyNames();

                // 遍历每个子键名称（即每个版本），并将它们添加到版本列表中
                int index = 0;
                while (true)
                {
                    // 如果遍历到子键名称数组的结尾，退出循环
                    if (index >= subKeyNames.Length)
                    {
                        resultList = list; // 将临时列表赋值给最终的返回列表
                        break;
                    }

                    // 获取当前子键名称（即版本号或版本名称）
                    string item = subKeyNames[index];

                    // 将当前版本名称添加到列表中
                    list.Add(item);

                    // 移动到下一个子键名称
                    index++;
                }
            }

            // 返回包含所有版本名称的列表
            return resultList;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        // 从给定的文本 sourceText 中提取最后一个数字，并返回该数字的字符串表示。
        // 参数 startNumber 用于返回找到的数字的起始位置。
        public static string GetLastNumberFromText(string sourceText, ref int startNumber)
        {
            string lastNumber = null; // 用于存储最后提取的数字字符串
            string expression = ""; // 用于构建数字字符串的临时变量
            int textLength = Microsoft.VisualBasic.Strings.Len(sourceText); // 获取 sourceText 的长度

            // 遍历源文本中的每一个字符
            for (int i = 1; i <= textLength; i++)
            {
                // 获取源文本中当前位置的字符
                string currentChar = Microsoft.VisualBasic.Strings.Mid(sourceText, i, 1);

                // 如果字符不是数字且不是小数点，则重置临时变量 expression
                if (!(Information.IsNumeric(currentChar) | (currentChar == ".")))
                {
                    expression = ""; // 清空临时字符串
                }
                else
                {
                    // 如果是第一个数字字符，记录数字的起始位置
                    if (Microsoft.VisualBasic.Strings.Len(expression) == 0)
                    {
                        startNumber = i; // 记录起始位置
                    }

                    // 将数字字符追加到临时字符串 expression 中
                    lastNumber = expression + currentChar;
                    expression = lastNumber; // 更新临时字符串
                }
            }

            // 返回提取到的数字字符串
            return lastNumber;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetZWKey(string zwProductName)
        {
            return (this.GetCADPath(CADType.ZWCAD) + @"\" + zwProductName + @"\zh-CN");
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoLoad));
            this.FilePath_Text = new System.Windows.Forms.Label();
            this.FeiYinGFilePath = new System.Windows.Forms.TextBox();
            this.APPName_Text = new System.Windows.Forms.Label();
            this.APPDes_text = new System.Windows.Forms.Label();
            this.textBoxApp = new System.Windows.Forms.TextBox();
            this.textBoxAppDesc = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.LoadNET_Text = new System.Windows.Forms.Label();
            this.AddStart = new System.Windows.Forms.Button();
            this.DelStart = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.listViewAssembly = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ACADSeletct_Text = new System.Windows.Forms.Label();
            this.IntACADList = new System.Windows.Forms.ComboBox();
            this.Unt2007 = new System.Windows.Forms.Button();
            this.Int2007 = new System.Windows.Forms.Button();
            this.Unt2020 = new System.Windows.Forms.Button();
            this.Int2020 = new System.Windows.Forms.Button();
            this.ACADListBox = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ACAD_Text = new System.Windows.Forms.Label();
            this.ZWCAD_Text = new System.Windows.Forms.Label();
            this.ZwCADListBox = new System.Windows.Forms.CheckedListBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilePath_Text
            // 
            this.FilePath_Text.AutoSize = true;
            this.FilePath_Text.Location = new System.Drawing.Point(20, 78);
            this.FilePath_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FilePath_Text.Name = "FilePath_Text";
            this.FilePath_Text.Size = new System.Drawing.Size(157, 15);
            this.FilePath_Text.TabIndex = 2;
            this.FilePath_Text.Text = "飞鹰程序文件路径选择";
            // 
            // FeiYinGFilePath
            // 
            this.FeiYinGFilePath.Location = new System.Drawing.Point(0, 98);
            this.FeiYinGFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.FeiYinGFilePath.Name = "FeiYinGFilePath";
            this.FeiYinGFilePath.Size = new System.Drawing.Size(305, 25);
            this.FeiYinGFilePath.TabIndex = 3;
            // 
            // APPName_Text
            // 
            this.APPName_Text.AutoSize = true;
            this.APPName_Text.Location = new System.Drawing.Point(13, 143);
            this.APPName_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.APPName_Text.Name = "APPName_Text";
            this.APPName_Text.Size = new System.Drawing.Size(82, 15);
            this.APPName_Text.TabIndex = 4;
            this.APPName_Text.Text = "应用名称：";
            // 
            // APPDes_text
            // 
            this.APPDes_text.AutoSize = true;
            this.APPDes_text.Location = new System.Drawing.Point(13, 177);
            this.APPDes_text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.APPDes_text.Name = "APPDes_text";
            this.APPDes_text.Size = new System.Drawing.Size(82, 15);
            this.APPDes_text.TabIndex = 4;
            this.APPDes_text.Text = "应用描述：";
            // 
            // textBoxApp
            // 
            this.textBoxApp.Location = new System.Drawing.Point(100, 135);
            this.textBoxApp.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxApp.Name = "textBoxApp";
            this.textBoxApp.ReadOnly = true;
            this.textBoxApp.Size = new System.Drawing.Size(287, 25);
            this.textBoxApp.TabIndex = 5;
            // 
            // textBoxAppDesc
            // 
            this.textBoxAppDesc.Location = new System.Drawing.Point(100, 169);
            this.textBoxAppDesc.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxAppDesc.Name = "textBoxAppDesc";
            this.textBoxAppDesc.ReadOnly = true;
            this.textBoxAppDesc.Size = new System.Drawing.Size(287, 25);
            this.textBoxAppDesc.TabIndex = 5;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonBrowse.ForeColor = System.Drawing.Color.White;
            this.buttonBrowse.Location = new System.Drawing.Point(315, 78);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(71, 42);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Text = "浏览";
            this.buttonBrowse.UseVisualStyleBackColor = false;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = ".NET程序集(*.dll)|*.dll";
            // 
            // LoadNET_Text
            // 
            this.LoadNET_Text.AutoSize = true;
            this.LoadNET_Text.Location = new System.Drawing.Point(17, 358);
            this.LoadNET_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LoadNET_Text.Name = "LoadNET_Text";
            this.LoadNET_Text.Size = new System.Drawing.Size(711, 45);
            this.LoadNET_Text.TabIndex = 7;
            this.LoadNET_Text.Text = "已自动加载的.NET程序集： （点击右侧-<路径>按钮即可打开框内飞鹰路径） 点击<关闭/X>即可退出程序\r\n\r\n\r\n";
            // 
            // AddStart
            // 
            this.AddStart.ForeColor = System.Drawing.Color.Red;
            this.AddStart.Location = new System.Drawing.Point(0, 211);
            this.AddStart.Margin = new System.Windows.Forms.Padding(4);
            this.AddStart.Name = "AddStart";
            this.AddStart.Size = new System.Drawing.Size(179, 32);
            this.AddStart.TabIndex = 9;
            this.AddStart.Text = "加入自启";
            this.AddStart.UseVisualStyleBackColor = true;
            this.AddStart.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // DelStart
            // 
            this.DelStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.DelStart.Location = new System.Drawing.Point(215, 211);
            this.DelStart.Margin = new System.Windows.Forms.Padding(4);
            this.DelStart.Name = "DelStart";
            this.DelStart.Size = new System.Drawing.Size(173, 32);
            this.DelStart.TabIndex = 9;
            this.DelStart.Text = "删除自启";
            this.DelStart.UseVisualStyleBackColor = true;
            this.DelStart.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.buttonExit.Location = new System.Drawing.Point(634, 427);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(173, 39);
            this.buttonExit.TabIndex = 9;
            this.buttonExit.Text = "关闭";
            this.buttonExit.UseVisualStyleBackColor = false;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // listViewAssembly
            // 
            this.listViewAssembly.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewAssembly.HideSelection = false;
            this.listViewAssembly.Location = new System.Drawing.Point(20, 385);
            this.listViewAssembly.Margin = new System.Windows.Forms.Padding(4);
            this.listViewAssembly.Name = "listViewAssembly";
            this.listViewAssembly.Size = new System.Drawing.Size(610, 80);
            this.listViewAssembly.TabIndex = 11;
            this.listViewAssembly.UseCompatibleStateImageBehavior = false;
            this.listViewAssembly.View = System.Windows.Forms.View.Details;
            this.listViewAssembly.SelectedIndexChanged += new System.EventHandler(this.ListViewAssembly_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "程序名";
            this.columnHeader1.Width = 121;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "文件路径";
            this.columnHeader2.Width = 662;
            // 
            // ACADSeletct_Text
            // 
            this.ACADSeletct_Text.AutoSize = true;
            this.ACADSeletct_Text.Location = new System.Drawing.Point(20, 7);
            this.ACADSeletct_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACADSeletct_Text.Name = "ACADSeletct_Text";
            this.ACADSeletct_Text.Size = new System.Drawing.Size(318, 15);
            this.ACADSeletct_Text.TabIndex = 0;
            this.ACADSeletct_Text.Text = "已安装的AutoCAD版本，下拉选择要设置的版本";
            // 
            // IntACADList
            // 
            this.IntACADList.FormattingEnabled = true;
            this.IntACADList.Location = new System.Drawing.Point(0, 34);
            this.IntACADList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.IntACADList.Name = "IntACADList";
            this.IntACADList.Size = new System.Drawing.Size(383, 23);
            this.IntACADList.TabIndex = 12;
            this.IntACADList.SelectedIndexChanged += new System.EventHandler(this.AllCADVersionCombo_SelectedIndexChanged);
            // 
            // Unt2007
            // 
            this.Unt2007.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Unt2007.ForeColor = System.Drawing.Color.Blue;
            this.Unt2007.Location = new System.Drawing.Point(20, 311);
            this.Unt2007.Margin = new System.Windows.Forms.Padding(4);
            this.Unt2007.Name = "Unt2007";
            this.Unt2007.Size = new System.Drawing.Size(168, 32);
            this.Unt2007.TabIndex = 13;
            this.Unt2007.Text = "2007卸载";
            this.Unt2007.UseVisualStyleBackColor = false;
            this.Unt2007.Click += new System.EventHandler(this.Unt2007_But);
            // 
            // Int2007
            // 
            this.Int2007.BackColor = System.Drawing.Color.Blue;
            this.Int2007.ForeColor = System.Drawing.Color.White;
            this.Int2007.Location = new System.Drawing.Point(20, 256);
            this.Int2007.Margin = new System.Windows.Forms.Padding(4);
            this.Int2007.Name = "Int2007";
            this.Int2007.Size = new System.Drawing.Size(172, 42);
            this.Int2007.TabIndex = 14;
            this.Int2007.Text = "2007加载";
            this.Int2007.UseVisualStyleBackColor = false;
            this.Int2007.Click += new System.EventHandler(this.Int2007_But);
            // 
            // Unt2020
            // 
            this.Unt2020.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Unt2020.ForeColor = System.Drawing.Color.Blue;
            this.Unt2020.Location = new System.Drawing.Point(232, 311);
            this.Unt2020.Margin = new System.Windows.Forms.Padding(4);
            this.Unt2020.Name = "Unt2020";
            this.Unt2020.Size = new System.Drawing.Size(172, 32);
            this.Unt2020.TabIndex = 15;
            this.Unt2020.Text = "2020卸载";
            this.Unt2020.UseVisualStyleBackColor = false;
            this.Unt2020.Click += new System.EventHandler(this.Unt2020_But);
            // 
            // Int2020
            // 
            this.Int2020.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Int2020.ForeColor = System.Drawing.Color.White;
            this.Int2020.Location = new System.Drawing.Point(232, 256);
            this.Int2020.Margin = new System.Windows.Forms.Padding(4);
            this.Int2020.Name = "Int2020";
            this.Int2020.Size = new System.Drawing.Size(172, 42);
            this.Int2020.TabIndex = 16;
            this.Int2020.Text = "2020加载";
            this.Int2020.UseVisualStyleBackColor = false;
            this.Int2020.Click += new System.EventHandler(this.Int2020_But);
            // 
            // ACADListBox
            // 
            this.ACADListBox.Font = new System.Drawing.Font("宋体", 9F);
            this.ACADListBox.FormattingEnabled = true;
            this.ACADListBox.HorizontalScrollbar = true;
            this.ACADListBox.Location = new System.Drawing.Point(17, 29);
            this.ACADListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ACADListBox.Name = "ACADListBox";
            this.ACADListBox.ScrollAlwaysVisible = true;
            this.ACADListBox.Size = new System.Drawing.Size(385, 224);
            this.ACADListBox.TabIndex = 17;
            this.ACADListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox1_ItemCheck);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ACADSeletct_Text);
            this.panel1.Controls.Add(this.FilePath_Text);
            this.panel1.Controls.Add(this.FeiYinGFilePath);
            this.panel1.Controls.Add(this.APPName_Text);
            this.panel1.Controls.Add(this.APPDes_text);
            this.panel1.Controls.Add(this.textBoxApp);
            this.panel1.Controls.Add(this.IntACADList);
            this.panel1.Controls.Add(this.textBoxAppDesc);
            this.panel1.Controls.Add(this.buttonBrowse);
            this.panel1.Controls.Add(this.AddStart);
            this.panel1.Controls.Add(this.DelStart);
            this.panel1.Location = new System.Drawing.Point(419, 100);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 248);
            this.panel1.TabIndex = 18;
            // 
            // ACAD_Text
            // 
            this.ACAD_Text.AutoSize = true;
            this.ACAD_Text.Font = new System.Drawing.Font("宋体", 12F);
            this.ACAD_Text.ForeColor = System.Drawing.Color.Red;
            this.ACAD_Text.Location = new System.Drawing.Point(84, 6);
            this.ACAD_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ACAD_Text.Name = "ACAD_Text";
            this.ACAD_Text.Size = new System.Drawing.Size(239, 20);
            this.ACAD_Text.TabIndex = 7;
            this.ACAD_Text.Text = "钩选要自动加载的CAD版本";
            // 
            // ZWCAD_Text
            // 
            this.ZWCAD_Text.AutoSize = true;
            this.ZWCAD_Text.Font = new System.Drawing.Font("宋体", 12F);
            this.ZWCAD_Text.ForeColor = System.Drawing.Color.Blue;
            this.ZWCAD_Text.Location = new System.Drawing.Point(531, 6);
            this.ZWCAD_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ZWCAD_Text.Name = "ZWCAD_Text";
            this.ZWCAD_Text.Size = new System.Drawing.Size(159, 20);
            this.ZWCAD_Text.TabIndex = 7;
            this.ZWCAD_Text.Text = "中望CAD自动加载";
            // 
            // ZwCADListBox
            // 
            this.ZwCADListBox.Font = new System.Drawing.Font("宋体", 9F);
            this.ZwCADListBox.FormattingEnabled = true;
            this.ZwCADListBox.HorizontalScrollbar = true;
            this.ZwCADListBox.Location = new System.Drawing.Point(420, 29);
            this.ZwCADListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ZwCADListBox.Name = "ZwCADListBox";
            this.ZwCADListBox.ScrollAlwaysVisible = true;
            this.ZwCADListBox.Size = new System.Drawing.Size(385, 64);
            this.ZwCADListBox.TabIndex = 21;
            this.ZwCADListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox2_ItemCheck);
            // 
            // buttonOpen
            // 
            this.buttonOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.buttonOpen.Location = new System.Drawing.Point(634, 379);
            this.buttonOpen.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(173, 39);
            this.buttonOpen.TabIndex = 22;
            this.buttonOpen.Text = "路径";
            this.buttonOpen.UseVisualStyleBackColor = false;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // FormAutoLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 469);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.Unt2007);
            this.Controls.Add(this.Int2007);
            this.Controls.Add(this.Unt2020);
            this.Controls.Add(this.ZwCADListBox);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.Int2020);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ACADListBox);
            this.Controls.Add(this.listViewAssembly);
            this.Controls.Add(this.ZWCAD_Text);
            this.Controls.Add(this.ACAD_Text);
            this.Controls.Add(this.LoadNET_Text);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAutoLoad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "飞鹰自动加载管理设置";
            this.Load += new System.EventHandler(this.FormAutoLoad_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ListViewAssembly_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool RemoveDemandLoadingEntries(string ProductName, string appName, bool currentUser, CADType cADType = 0)
        {
            try
            {
                string autoCADKeyName = "";
                if (cADType == CADType.AutoCAD)
                {
                    autoCADKeyName = this.GetAutoCADKeyName(ProductName);
                }
                if (cADType == CADType.ZWCAD)
                {
                    autoCADKeyName = this.GetZWKey(ProductName);
                }
                RegistryKey key = currentUser ? Registry.CurrentUser : Registry.LocalMachine;
                if (cADType == CADType.ZWCAD)
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (RegistryView)0x100);
                }
                key.OpenSubKey(autoCADKeyName + @"\Applications", true).DeleteSubKeyTree(appName);
            }
            catch (Exception exception1)
            {
                MessageBox.Show(exception1.Message);
                return false;
            }
            return true;
        }

        public enum CADType
        {
            AutoCAD,
            ZWCAD,
            GRCAD,
            OtherCAD
        }

        private async void buttonOpen_Click(object sender, EventArgs e)
        {
            bool found = false;
            foreach (ListViewItem item in listViewAssembly.Items)
            {
                string programName = item.Text;
                string programPath = item.SubItems[1].Text;

                if (programName.Contains("飞鹰"))
                {
                    found = true;
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{programPath}\"");
                        buttonOpen.BackColor = Color.Green;
                        await Task.Run(() => System.Threading.Thread.Sleep(2000));
                        buttonOpen.BackColor = Color.FromArgb(255, 192, 128); // 恢复原来的颜色
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"无法打开目录: {ex.Message}");
                    }
                }
            }

            if (!found)
            {
                buttonOpen.BackColor = Color.Red;
                await Task.Run(() => System.Threading.Thread.Sleep(2000));
                buttonOpen.BackColor = Color.FromArgb(255, 192, 128); // 恢复原来的颜色
            }
        }
    }
}

