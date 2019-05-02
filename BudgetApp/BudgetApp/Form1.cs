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
                Category catInputEnum = Category.FOOD;
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
            }
            catch (Exception)
            {
                System.Media.SystemSounds.Exclamation.Play();
                MessageBox.Show("Error occured!!!" + 
                    " Please ensure all fields are filled in and the amount" +
                    " field is a number.");
            }
        }

        private void allOverviewButton_Click(object sender, EventArgs e)
        {

            allDetails.Text = expense.ShowOverview();
            allChart.Series.Clear();

            //Draw the pie Chart
            String seriesname = "allSeries";
            allChart.Series.Add(seriesname);
            allChart.Series[seriesname].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;

            allChart.Series[seriesname].Points.AddXY("Food", expense.GetTotalPerCat(Category.FOOD));
            allChart.Series[seriesname].Points.AddXY("House", expense.GetTotalPerCat(Category.HOUSE));
            allChart.Series[seriesname].Points.AddXY("Liesure", expense.GetTotalPerCat(Category.LIESURE));
            allChart.Series[seriesname].Points.AddXY("Needed", expense.GetTotalPerCat(Category.NEEDED));
        }

        private void allFoodButton_Click(object sender, EventArgs e)
        {
            allDetails.Text = expense.ShowCategoryDetails(Category.FOOD);
        }

        private void allHouseButton_Click(object sender, EventArgs e)
        {
            allDetails.Text = expense.ShowCategoryDetails(Category.HOUSE);
        }

        private void allLiesureButton_Click(object sender, EventArgs e)
        {
            allDetails.Text = expense.ShowCategoryDetails(Category.LIESURE);
        }

        private void allNeeedButton_Click(object sender, EventArgs e)
        {
            allDetails.Text = expense.ShowCategoryDetails(Category.NEEDED);
        }

        private void monthlyOverviewButton_Click(object sender, EventArgs e)
        {

            monthlyDetails.Text = expense.ShowOverview(TimeSpan.MONTHLY, 
                this.month);
            monthlyChart.Series.Clear();

            //Draw the pie chart
            String seriesname = "monthlySeries";
            monthlyChart.Series.Add(seriesname);
            monthlyChart.Series[seriesname].ChartType = 
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            
            monthlyChart.Series[seriesname].Points.AddXY("Food", 
                expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("House", 
                expense.GetTotalPerCat(Category.HOUSE, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("Liesure", 
                expense.GetTotalPerCat(Category.LIESURE, TimeSpan.MONTHLY));
            monthlyChart.Series[seriesname].Points.AddXY("Needed", 
                expense.GetTotalPerCat(Category.NEEDED, TimeSpan.MONTHLY));
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            string[] parsedDate = monthCalendar1.SelectionStart.ToString().Split('/');
            this.month = Int32.Parse(parsedDate[0]);
            double year = Double.Parse(parsedDate[2].Substring(0, 4));
            expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY);
        }

        private void monthlyFoodButton_Click(object sender, EventArgs e)
        {
            monthlyDetails.Text = expense.ShowCategoryDetails(Category.FOOD);
        }

        private void monthlyHouseButton_Click(object sender, EventArgs e)
        {
            monthlyDetails.Text = expense.ShowCategoryDetails(Category.HOUSE);
        }

        private void monthlyLiesureButton_Click(object sender, EventArgs e)
        {
            monthlyDetails.Text = expense.ShowCategoryDetails(Category.LIESURE);
        }

        private void monthlyNeededButton_Click(object sender, EventArgs e)
        {
            monthlyDetails.Text = expense.ShowCategoryDetails(Category.NEEDED);
        }

        private void timespanControl_Click(object sender, EventArgs e)
        {
            string[] parsedDate = monthCalendar1.SelectionStart.ToString().Split('/');
            this.month = Int32.Parse(parsedDate[0]);
            double year = Double.Parse(parsedDate[2].Substring(0, 4));

            expense.UpdateMonthlyTotals(this.month);
            expense.GetTotalPerCat(Category.FOOD, TimeSpan.MONTHLY);
        }

        private void saveToolMenuItem_Click(object sender, EventArgs e)
        {
            MenuFile.Save(expense);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Binary|*.bin";
            saveFileDialog1.Title = "Save a Binary File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  

                //SERIALIZE
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                try
                {
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            BinaryFormatter f = new BinaryFormatter();
                            f.Serialize(fs, expense);
                            break;
                    }
                }
                catch (SerializationException ee)
                {
                    Console.WriteLine("Failed to serialize: " + ee.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }

            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Binary|*.bin";
            openFileDialog1.Title = "Open a Binary File";
            openFileDialog1.ShowDialog();
            
            // Saves the Image via a FileStream created by the OpenFile method.  

            //DESERIALIZE
            try
            {
                var filePath = openFileDialog1.FileName;
                using(FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    //Process.Start("notepad.exe, filePath");//"the system cannot find the file specified"
                    BinaryFormatter formatter = new BinaryFormatter();
                    expense = (Expenses)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                $"Details:\n\n{ex.StackTrace}");
            }
                

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
