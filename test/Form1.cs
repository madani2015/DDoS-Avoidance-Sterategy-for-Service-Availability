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
namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream("d:\\ddos_result.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("NORMAL");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    chart1.Series["Series1"].Points.AddXY(sheet.GetRow(row).GetCell(0).NumericCellValue, sheet.GetRow(row).GetCell(1).NumericCellValue);
                }
            }

            sheet = hssfwb.GetSheet("DDOS");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    chart1.Series["Series2"].Points.AddXY(sheet.GetRow(row).GetCell(0).NumericCellValue, sheet.GetRow(row).GetCell(1).NumericCellValue);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
