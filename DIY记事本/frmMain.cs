using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using ExtendNotePad;

namespace DIY记事本
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //在窗体加载的时候搜索addins目录下的所有的dll文件
            //Assembly.GetExecutingAssembly() 获取当前正在运行的exe文件
            //Assembly.GetExecutingAssembly().Location 获得该exe文件的完整路径
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //将两个路径连接起来
            string addinsPath = Path.Combine(dir, "addins");

          //搜索指定目录下所有的dll文件
            string[] dlls = Directory.GetFiles(addinsPath, "*.dll");

            //循环加载所有的dll文件
            for (int i = 0; i < dlls.Length; i++)
            {
                //将每一个dll文件都加载进来
                Assembly ass = Assembly.LoadFile(dlls[i]);

                //获取当前dll（插件）中定义的所有public类型
                Type[] typs = ass.GetExportedTypes();

                //获得IEditor的Type
                Type typIEditor = typeof(IEditor);

                //循环判断每一个类型（Type）是否实现了IEditor接口
                for (int j = 0; j < typs.Length; j++)
                {
                    //typIEditor.IsAssignableFrom(typs[j])
                    //判断程序集中的类型是否能赋值给IEditor的Type
                    //如果能，则表示该类型实现了IEditor接口

                    //这里其实需要做两个判断
                    //1.该类型实现了IEditor接口
                    //2.该类型可以实例化
                    if (typIEditor.IsAssignableFrom(typs[j]) && !typs[j].IsAbstract)
                    {
                        //根据类型的Type，创建类型的对象
                        //由于已经明确了typs[j]肯定是实现了IEditor接口的类型
                        //所以可以将该类型的对象 显示转换为IEditor类型，然后通过接口变量操作类的成员
                        IEditor editor = (IEditor)Activator.CreateInstance(typs[j]);
                        editor.Excute();
                    }
                }

            }
            
            //获取dll文件中的具有插件功能的类
            //通过调用插件中的类的方法实现对记事本的功能的扩展
            //开发主程序的人必须做一个约定：所有为该程序开发插件的人必须将插件的执行方法命名为Excute()
            //主程序的开发人员不管插件开发者定义了多个类、方法，在主程序中只去找Excute（）方法来调用。这就是主程序的开发者与插件开发者的一个约定
            //接口就是一个约定，这个接口中有一个名字叫Excute()方法

        }
    }
}
