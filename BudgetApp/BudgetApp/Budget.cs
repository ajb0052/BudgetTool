using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetApp
{

    public enum Category { OVERALL, CAT1, CAT2, CAT3, CAT4, CAT5, CAT6, CAT7 };
    public enum TimeSpan { ALL, MONTHLY, YEARLY };
    public enum Month { January = 1, February, March, April, May, June, July, August, September, October, November, December };

    public enum Chart { Pie, Area };


    class InvalidNetIncomeException : Exception
    {
        public InvalidNetIncomeException()
        {
        }

        public InvalidNetIncomeException(string message)
            : base(message)
        {
        }

        public InvalidNetIncomeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    [Serializable]
    public struct ExpenseDataPoint : IComparable<ExpenseDataPoint>
    {
        public string name { get; set; }
        public double amount { get; set; }
        public DateTime date { get; set; }
        public ExpenseDataPoint(string n, double a, DateTime d)
        {
            name = n;
            amount = a;
            date = d;
        }
        public int CompareTo(ExpenseDataPoint y)
        {
            if (this.date.CompareTo(y.date) == 0)
            {
                return this.name.ToUpper().CompareTo(y.name.ToUpper());
            }
            return this.date.CompareTo(y.date);
        }
    }
  
    [Serializable]
    class ExpenseDataPointComparer : IComparer<ExpenseDataPoint>
    {
        public int Compare(ExpenseDataPoint x, ExpenseDataPoint y)
        {
            if (x.date.CompareTo(y.date) == 0)
            {
                return x.name.ToUpper().CompareTo(y.name.ToUpper());
            }
            return x.date.CompareTo(y.date);
        }
    }

    [Serializable]
    public class CategoryData
    {
        public string name;
        public SortedSet<ExpenseDataPoint> setOfExpenses;
        public double total;
        public double monthlyTotal;
        public CategoryData(string Name)
        {
            name = Name;
            setOfExpenses = new SortedSet<ExpenseDataPoint>();
            total = 0;
            monthlyTotal = 0;
        }
        
    }
    [Serializable]
    public class Expenses
    {

        private CategoryData cat1;
        private CategoryData cat2;
        private CategoryData cat3;
        private CategoryData cat4;



        private double totalSpent;
        private double monthlyTotalSpent;
        private double grossIncome;// yearly

        private Chart chart;
         
         
        private String expenseListFood;
        private String monthlyExpenseListFood;
        private String expenseListHouse;
        private String monthlyExpenseListHouse;
        private String expenseListLiesure;
        private String monthlyExpenseListLiesure;
        private String expenseListNeeded;
        private String monthlyExpenseListNeeded;


        public String filename;
        public string[] categories;

        /* Contructor gives default values upon instantiation */
        public Expenses()
        {
            //
            
            this.totalSpent = 0;
            this.monthlyTotalSpent = 0;
            //
            this.cat1 = new CategoryData("food");
            this.cat2 = new CategoryData("house");
            this.cat3 = new CategoryData("liesure");
            this.cat4 = new CategoryData("needed");
            //

            this.grossIncome = 0;
            
            this.monthlyExpenseListFood = "";
            this.expenseListHouse = "";
            this.monthlyExpenseListHouse = "";
            this.expenseListLiesure = "";
            this.monthlyExpenseListLiesure = "";
            this.expenseListNeeded = "";
            this.monthlyExpenseListNeeded = "";
            this.expenseListFood = "";

            this.chart = Chart.Pie;

            //temp 
            categories = new String[8];
            this.categories[0] = "overall";
            this.categories[1] = "food";
            this.categories[2] = "house";
            this.categories[3] = "liesure";
            this.categories[4] = "needed";
        }
        
        /* Gets details on an expense made and updates the corresponding set and totals */
        public void AddExpense(string name, double amount, DateTime date, Category category)
        {
            ExpenseDataPoint dataPoint = new ExpenseDataPoint(name, amount, date);

            switch (category)
            {
                case Category.CAT1:
                    this.cat1.total += amount;
                    this.cat1.setOfExpenses.Add(dataPoint);
                    break;
                case Category.CAT2:
                    this.cat2.total += amount;
                    this.cat2.setOfExpenses.Add(dataPoint);
                    break;
                case Category.CAT3:
                    this.cat3.total += amount;
                    this.cat3.setOfExpenses.Add(dataPoint);
                    break;
                case Category.CAT4:
                    this.cat4.total += amount;
                    this.cat4.setOfExpenses.Add(dataPoint);
                    break;
            }

            totalSpent += amount;
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------
        public string GetTotalSpentAsString()
        {
            return "Total spent: " + this.totalSpent.ToString("C");
        }
        public string GetNetTotalAsString()
        {
            return "Net total: " + (this.grossIncome - this.totalSpent).ToString("C");
        }
        public double GetGrossIncome()
        {
            return this.grossIncome;
        }
        public Chart GetChartType()
        {
            return chart;
        }
        public void SetChartType(Chart chart)
        {
            this.chart = chart;
        }
        public string UpdateGrossIncome(double grossIncome)
        {
            this.grossIncome = grossIncome;
            string output = "Gross Income: " + grossIncome.ToString("C") + "\n";
            return output;
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------
        /* Changes the expense list variables. dependent on the totals and monthly totals having been set */
        public double GetTotalPerCat(Category cat, TimeSpan timeSpan = TimeSpan.ALL)
        {
            double total = 0;
            double monthlyTotal = 0;
            
            switch (cat)
            {
                case Category.CAT1:
                    total = this.cat1.total;
                    monthlyTotal = this.cat1.monthlyTotal;
                    break;
                case Category.CAT2:
                    total = this.cat2.total;
                    monthlyTotal = this.cat2.monthlyTotal;
                    break;
                case Category.CAT3:
                    total = this.cat3.total;
                    monthlyTotal = this.cat3.monthlyTotal;
                    break;
                case Category.CAT4:
                    total = this.cat4.total;
                    monthlyTotal = this.cat4.monthlyTotal;
                    break;
                default:
                    break;
            }
            if(timeSpan == TimeSpan.MONTHLY)
                return monthlyTotal;
            return total;
        }

        ///-----------------------------------------------------------------------------------------------------------------------------------------
        //Should be called before every occurence of GetTotal where the month is important it also updates category lists
        public void UpdateMonthlyTotals(int month)
        {
            string[] expenseDateParsed;

            cat1.monthlyTotal = 0;
            cat2.monthlyTotal = 0;
            cat3.monthlyTotal = 0;
            cat4.monthlyTotal = 0;
            monthlyTotalSpent = 0;

            expenseListFood = "";
            //cycles through expense set in order to update expense lists and monthly totals
            foreach (ExpenseDataPoint expense in cat1.setOfExpenses)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == month)
                {
                    cat1.monthlyTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListFood += expense.ToString() + "\n";
                }
                expenseListFood += expense.ToString() + "\n";
            }

            expenseListHouse = "";
            foreach (ExpenseDataPoint expense in cat2.setOfExpenses)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == month)
                {
                    cat2.monthlyTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListHouse += expense.ToString() + "\n";
                }
                expenseListHouse += expense.ToString() + "\n";
            }

            expenseListLiesure = "";
            foreach (ExpenseDataPoint expense in cat3.setOfExpenses)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == (int)month)
                {
                    cat3.monthlyTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListLiesure += expense.ToString() + "\n";
                }
                expenseListLiesure += expense.ToString() + "\n";
            }

            expenseListNeeded = "";
            foreach (ExpenseDataPoint expense in cat4.setOfExpenses)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == (int)month)
                {
                    cat4.monthlyTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListNeeded += expense.ToString() + "\n";
                }
                expenseListNeeded += expense.ToString() + "\n";
            }
        }

        public void UpdateMonthlyDataSets(int month)
        {
            foreach (ExpenseDataPoint fData in cat1.setOfExpenses)
            {
                if (fData.date.Month == (int)month)
                {

                }
            }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------

        public string GetCategoryTotals(TimeSpan timeSpanParam = TimeSpan.ALL, int month = 1, double year = 2019)
        {
            string output = "";
            if (timeSpanParam == TimeSpan.ALL)
            {
                output += "Food: " + cat1.total.ToString("C") + "\n";
                output += "House: " + cat2.total.ToString("C") + "\n";
                output += "Liesure: " + cat3.total.ToString("C") + "\n";
                output += "Needed: " + cat4.total.ToString("C") + "\n";
            }
            else if(timeSpanParam == TimeSpan.MONTHLY)
            {
                double monthlyGrossIncome = grossIncome / 12;
                UpdateMonthlyTotals(month);
                
                output += "Food: " + cat1.monthlyTotal.ToString("C") + "\n";
                output += "House: " + cat2.monthlyTotal.ToString("C") + "\n";
                output += "Liesure: " + cat3.monthlyTotal.ToString("C") + "\n";
                output += "Needed: " + cat4.monthlyTotal.ToString("C") + "\n";
            }
            else if(timeSpanParam == TimeSpan.YEARLY)
            {

            }
            return output;
        }

        /* Return: prints out the where the money was spent for one category specified in parameter */
        public string ShowCategoryDetails(Category cat, TimeSpan tSpan = TimeSpan.ALL, int month = 1)
        {
            string catString = this.categories[(int)cat];
            string output = "Gross Income: " + this.grossIncome.ToString("C") + "\n";
            output = catString+"\n";
            output += "Name \t Date \t Amount";
            SortedSet<ExpenseDataPoint> catDataSet = new SortedSet<ExpenseDataPoint>();
            CategoryData[] categoryDataArray = new CategoryData[7] {
                this.cat1,
                this.cat2,
                this.cat3,
                this.cat4,
                null,
                null,
                null
            };
            if(cat == Category.OVERALL)
            {
                if(tSpan == TimeSpan.ALL)
                {
                    catDataSet.UnionWith(cat1.setOfExpenses);
                    catDataSet.UnionWith(cat2.setOfExpenses);
                    catDataSet.UnionWith(cat3.setOfExpenses);
                    catDataSet.UnionWith(cat4.setOfExpenses);
                }
                else if(tSpan == TimeSpan.MONTHLY)
                {
                    foreach(CategoryData catData in categoryDataArray)
                    {
                        foreach(ExpenseDataPoint dataPoint in catData.setOfExpenses) 
                        {
                            if (month == dataPoint.date.Month)
                                catDataSet.Add((ExpenseDataPoint)dataPoint);
                        }
                    }
                }
            }
            else{
                if (tSpan == TimeSpan.ALL)
                {
                    catDataSet = categoryDataArray[(int)cat-1].setOfExpenses;
                }
                else if (tSpan == TimeSpan.MONTHLY)
                {
                    foreach (ExpenseDataPoint dataPoint in categoryDataArray[(int)cat-1].setOfExpenses)
                    {
                        if (month == dataPoint.date.Month)
                            catDataSet.Add((ExpenseDataPoint)dataPoint);
                    }
                }
            }
            output += "\n";
            foreach (ExpenseDataPoint point in catDataSet)
            {
                output += point.name + " " + point.date.ToShortDateString() + " "+ point.amount.ToString("C") + "\n";
            }
            return output;
        }
    }
}
