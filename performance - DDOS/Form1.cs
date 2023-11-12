using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace performance___DDOS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
            var sys_sum = system_summary();
            label1.Text = string.Format(label1.Text, new object[] { sys_sum[0], sys_sum[1], sys_sum[2], sys_sum[3], sys_sum[4], sys_sum[5], sys_sum[6], sys_sum[7] });
            label2.Text = string.Format(label1.Text, new object[] { sys_sum[0], sys_sum[1], sys_sum[2], sys_sum[3], sys_sum[4], sys_sum[5], sys_sum[6], sys_sum[7] });
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(Application.StartupPath + "\\ddos_result.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("NORMAL");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    chart1.Series["Normal"].Points.AddXY(sheet.GetRow(row).GetCell(0).NumericCellValue, sheet.GetRow(row).GetCell(1).NumericCellValue);
                }
            }

            sheet = hssfwb.GetSheet("DDOS");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    chart1.Series["DDOS"].Points.AddXY(sheet.GetRow(row).GetCell(0).NumericCellValue, sheet.GetRow(row).GetCell(1).NumericCellValue);
                }
            }

            sheet = hssfwb.GetSheet("avoidance");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    chart2.Series["DDOS"].Points.AddXY(sheet.GetRow(row).GetCell(0).NumericCellValue, sheet.GetRow(row).GetCell(1).NumericCellValue);
                }
            }

        }


        double[] system_summary()
        {
            double[] sum = new double[8];
            //overal_normal
            sum[0]=0;
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(Application.StartupPath + "\\ddos_result.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("NORMAL");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    sum[0] += sheet.GetRow(row).GetCell(1).NumericCellValue;
                }
            }

            //overal_ddos
            sum[1] = 0;
            using (FileStream file = new FileStream(Application.StartupPath + "\\ddos_result.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            sheet = hssfwb.GetSheet("DDOS");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    sum[1] += sheet.GetRow(row).GetCell(1).NumericCellValue;
                }
            }

            // ddos start
            sum[2] = sim.ddos_time;
             
            //avoidance
            int duration = 1;
            sum[3] = 0;
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row).GetCell(1).NumericCellValue >= 3000)
                {
                    sum[3] = sheet.GetRow(row).GetCell(0).NumericCellValue;
                    break;
                }
                else duration++;
            }

             //ddos dura
            sum[4] = duration - sim.ddos_time;
             //maxload
            sum[5]=0;
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row).GetCell(1).NumericCellValue > sum[5])
                {
                    sum[5] = sheet.GetRow(row).GetCell(1).NumericCellValue;
                }
            }

            //T1
            sum[6] = sim.T1;
            //T2
            sum[7] = sim.T2;
            return sum;
        }
    }
}
