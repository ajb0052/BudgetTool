/* Author: Amanda Stewart
 * Purpose: To gain an understanding of C# and Windows Forms
 * Environment: Microsoft Visual Studio 2017 on Windows 10
 * Intent of Use: input your income and all expenses made with associated data
 *      to view what category you spend most of your money on and to
 *      visually see how much you have left at the end of each much
 *      and to set a budget and if you have gone over budget.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BudgetApp
{
    public partial class Form1 : Form
    {
        int month = 1;
        Expenses expense;
        public Form1()
        {
            InitializeComponent();
            expense = new Expenses();
        }
        
        private void IncomeUpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                double input = Double.Parse(incomeTextBox.Text);
                String incomeOutput = expense.UpdateGrossIncome(input);
                IncomeDisplay.Text = incomeOutput;
            }
            catch (Exception)
            {
                System.Media.SystemSounds.Exclamation.Play();
                MessageBox.Show("Error occured!!!" + 
                    " Please ensure the field IS NOT empty and IS a number.");
            }
        }
        private void AddExpense_Click(object sender, EventArgs e)
        {
            try
            {
                double amount = Double.Parse(amountInput.Text);
                Category catInputEnum = Category.OVERALL;
                switch (categoryInput.SelectedItem.ToString())
                {
                    case "Food":
                        catInputEnum = Category.FOOD;
                        break;
                    case "House":
                        catInputEnum = Category.HOUSE;
                        break;
                    case "Liesure":
                        catInputEnum = Category.LIESURE;
                        break;
                    case "Needed":
                        catInputEnum = Category.NEEDED;
                        break;
                    default:
                        break;
                }
                expense.AddExpense(nameInput.Text, amount, dateInput.Value, 
                    catInputEnum);
                this.TotalSpent.Text = expense.GetTotalSpentAsString();
                this.NetIncome.Text = expense.GetNetTotalAsString();
            }
            catch (Exception)
            {
                System.Media.SystemSounds.Exclamation.Play();
                MessageBox.Show("Error occured!!!" + 
                    " Please ensure all fields are filled in and the amount" +
                    " field is a number.");
            }
        }

        #region All
        private void UpdateChartButton_Click(object sender, EventArgs e)
        {
            allChart.Series.Clear();
            DisplayPieChart();
        }
        private void CategoryAllDetailsDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CategoryAllDetailDropDown.SelectedIndex)
            {
               /* case 0:
                    allDetails.Text = expense.ShowCategoryDetails(Category.OVERALL);
                    break;*/
                case 1:
                    allDetails.Text = expense.ShowCategoryDetails(Category.FOOD);
                    break;
                case 2:
                    allDetails.Text = expense.ShowCategoryDetails(Category.HOUSE);
                    break;
                case 3:
                    allDetails.Text = expense.ShowCategoryDetails(Category.LIESURE);
                    break;
                case 4:
                    allDetails.Text = expense.ShowCategoryDetails(Category.NEEDED);
                    break;

            }
        }
        #endregion //All
        #region Monthly
        private void CategoryMonthlyDetailDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CategoryMonthlyDetailDropDown.SelectedIndex)
            {
                case 0:
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.OVERALL, TimeSpan.MONTHLY, this.month);
                    break;
                case 1:
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.FOOD, TimeSpan.MONTHLY, this.month);
                    break;
                case 2:
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.HOUSE, TimeSpan.MONTHLY, this.month);
                    break;
                case 3:
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.LIESURE, TimeSpan.MONTHLY, this.month);
                    break;
                case 4:
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.NEEDED, TimeSpan.MONTHLY, this.month);
                    break;

            }
        }

        private void updateMonthlyChartButton_Click(object sender, EventArgs e)
        {
            DisplayMonthlyPieChart();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            string[] parsedDate = monthCalendar1.SelectionStart.ToString().Split('/');
            this.month = Int32.Parse(parsedDate[0]);
            double year = Double.Parse(parsedDate[2].Substring(0, 4));
            expense.UpdateMonthlyTotals(this.month);
            expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.HOUSE, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.LIESURE, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.NEEDED, TimeSpan.MONTHLY);
        }

        private void timespanControl_Click(object sender, EventArgs e)
        {
            string[] parsedDate = monthCalendar1.SelectionStart.ToString().Split('/');
            this.month = Int32.Parse(parsedDate[0]);
            double year = Double.Parse(parsedDate[2].Substring(0, 4));

            expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.HOUSE, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.LIESURE, TimeSpan.MONTHLY);
            expense.GetTotalPerCat(Category.NEEDED, TimeSpan.MONTHLY);
        }
        #endregion //Monthly
        #region File_Management
        private void saveToolMenuItem_Click(object sender, EventArgs e)
        {
            //delete old file
            foreach (System.Collections.DictionaryEntry file in Environment.GetEnvironmentVariables())
                if (file.ToString() == expense.filename)
                    Environment.SetEnvironmentVariable(file.ToString(), null);
            //create new file
            MenuFile.Save(expense);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "budget|*.budg|Binary|*.bin";
            saveFileDialog1.Title = "Save File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  

                //SERIALIZE
                FileStream fileStream =
                   (FileStream)saveFileDialog1.OpenFile();
                expense.filename = saveFileDialog1.FileName;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fileStream, expense);

                }
                catch (SerializationException ee)
                {
                    Console.WriteLine("Failed to serialize: " + ee.Message);
                    throw;
                }
                finally
                {
                    fileStream.Close();
                }

            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Budget|*.budg|Binary|*.bin";
            openFileDialog1.Title = "Open File";
            openFileDialog1.ShowDialog();

            try
            {

                var filePath = openFileDialog1.FileName;

                using (FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    expense = (Expenses)formatter.Deserialize(fs);
                    //Update Display
                    this.Text = "Budget Tool - " + openFileDialog1.FileName;
                    this.IncomeDisplay.Text = expense.GetGrossIncomeAsString();
                    this.overview.Text = expense.ShowOverview();
                    this.TotalSpent.Text = expense.GetTotalSpentAsString();
                    this.NetIncome.Text = expense.GetNetTotalAsString();
                    allChart.Series.Clear();
                    DisplayPieChart();
                    allDetails.Text = expense.ShowCategoryDetails(Category.OVERALL);
                    
                    DisplayMonthlyPieChart();
                    monthlyDetails.Text = expense.ShowCategoryDetails(Category.OVERALL); //when added in clicking update chart button clears the chart and does not add it back until populating the details list
                }
            }
            catch (SerializationException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                $"Details:\n\n{ex.StackTrace}");
            }
            catch (System.ArgumentException ex)
            {

            }
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion //File_Management
        #region Pie_Chart_Palette
        private void BrightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
        }

        private void PastelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
        }

        private void BrightPastelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.BrightPastel;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.BrightPastel;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.BrightPastel;
        }

        private void ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
        }

        private void LightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Light;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Light;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Light;
        }

        private void EarthTonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
        }

        private void SemiTransparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
        }

        private void BerryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
        }

        private void ChocolateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
        }

        private void FireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
        }

        private void SeaGreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            monthlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            yearlyChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
        }
        #endregion
        #region Helper_Classes
        private void DisplayPieChart()
        {
            String seriesname = "allSeries";
            allChart.Series.Add(seriesname);
            allChart.Series[seriesname].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            allChart.Series[seriesname].Label = "#PERCENT";

            allChart.Series[seriesname].Points.AddXY("Food", expense.GetTotalPerCat(Category.FOOD));
            allChart.Series[seriesname].Points.AddXY("House", expense.GetTotalPerCat(Category.HOUSE));
            allChart.Series[seriesname].Points.AddXY("Liesure", expense.GetTotalPerCat(Category.LIESURE));
            allChart.Series[seriesname].Points.AddXY("Needed", expense.GetTotalPerCat(Category.NEEDED));

            allChart.Series[seriesname].Points[0].LegendText = "Food";
            allChart.Series[seriesname].Points[1].LegendText = "House";
            allChart.Series[seriesname].Points[2].LegendText = "Liesure";
            allChart.Series[seriesname].Points[3].LegendText = "Needed";

            //allChart.Series[seriesname]["PieLabelStyle"] = "Outside";
        }
        private void DisplayMonthlyPieChart()
        {
            monthlyChart.Series.Clear();

            //Draw the pie chart
            String seriesname = "monthlySeries";
            monthlyChart.Series.Add(seriesname);
            monthlyChart.Series[seriesname].ChartType =
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            monthlyChart.Series[seriesname].Label = "#PERCENT";

            monthlyChart.Series[seriesname].Points.AddXY("Food",
                expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("House",
                expense.GetTotalPerCat(Category.HOUSE, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("Liesure",
                expense.GetTotalPerCat(Category.LIESURE, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("Needed",
                expense.GetTotalPerCat(Category.NEEDED, TimeSpan.MONTHLY));


            monthlyChart.Series[seriesname].Points[0].LegendText = "Food";
            monthlyChart.Series[seriesname].Points[1].LegendText = "House";
            monthlyChart.Series[seriesname].Points[2].LegendText = "Liesure";
            monthlyChart.Series[seriesname].Points[3].LegendText = "Needed";
        }




        #endregion



    }
}
