using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetApp
{

    public enum Category { OVERALL, FOOD, HOUSE, LIESURE, NEEDED }
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
    public class Expenses
    {

        private SortedSet<ExpenseDataPoint> foodDataSet;
        private double foodTotal;
        private double monthlyFoodTotal;
         
        private SortedSet<ExpenseDataPoint> houseDataSet;
        private double houseTotal;
        private double monthlyHouseTotal;
         
        private SortedSet<ExpenseDataPoint> liesureDataSet;
        private double liesureTotal;
        private double monthlyLiesureTotal;
         
        private SortedSet<ExpenseDataPoint> neededDataSet;
        private double neededTotal;
        private double monthlyNeededTotal;
         
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


        /* Contructor gives default values upon instantiation */
        public Expenses()
        {
            this.foodTotal = 0;
            this.monthlyFoodTotal = 0;
            this.foodDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            this.houseTotal = 0;
            this.monthlyHouseTotal = 0;
            this.houseDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            this.liesureTotal = 0;
            this.monthlyLiesureTotal = 0;
            this.liesureDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            this.neededTotal = 0;
            this.monthlyNeededTotal = 0;
            this.neededDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            this.totalSpent = 0;
            this.monthlyTotalSpent = 0;
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

            //this.fileStream = null;
        }
        
        /* Gets details on an expense made and updates the corresponding set and totals */
        public void AddExpense(string name, double amount, DateTime date, Category category)
        {
            ExpenseDataPoint dataPoint = new ExpenseDataPoint(name, amount, date);
            
            switch (category)
            {
                case Category.FOOD:
                    this.foodTotal += amount;
                    this.foodDataSet.Add(dataPoint);
                    break;
                case Category.HOUSE:
                    this.houseTotal += amount;
                    this.houseDataSet.Add(dataPoint);
                    break;
                case Category.LIESURE:
                    this.liesureTotal += amount;
                    this.liesureDataSet.Add(dataPoint);
                    break;
                case Category.NEEDED:
                    this.neededTotal += amount;
                    this.neededDataSet.Add(dataPoint);
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
                case Category.FOOD:
                    total = this.foodTotal;
                    monthlyTotal = this.monthlyFoodTotal;
                    break;
                case Category.HOUSE:
                    total = this.houseTotal;
                    monthlyTotal = this.monthlyHouseTotal;
                    break;
                case Category.LIESURE:
                    total = this.liesureTotal;
                    monthlyTotal = this.monthlyLiesureTotal;
                    break;
                case Category.NEEDED:
                    total = this.neededTotal;
                    monthlyTotal = this.monthlyNeededTotal;
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

            monthlyFoodTotal = 0;
            monthlyHouseTotal = 0;
            monthlyLiesureTotal = 0;
            monthlyNeededTotal = 0;
            monthlyTotalSpent = 0;

            expenseListFood = "";
            //cycles through expense set in order to update expense lists and monthly totals
            foreach (ExpenseDataPoint expense in foodDataSet)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == month)
                {
                    monthlyFoodTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListFood += expense.ToString() + "\n";
                }
                expenseListFood += expense.ToString() + "\n";
            }

            expenseListHouse = "";
            foreach (ExpenseDataPoint expense in houseDataSet)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == month)
                {
                    monthlyHouseTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListHouse += expense.ToString() + "\n";
                }
                expenseListHouse += expense.ToString() + "\n";
            }

            expenseListLiesure = "";
            foreach (ExpenseDataPoint expense in liesureDataSet)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == (int)month)
                {
                    monthlyLiesureTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListLiesure += expense.ToString() + "\n";
                }
                expenseListLiesure += expense.ToString() + "\n";
            }

            expenseListNeeded = "";
            foreach (ExpenseDataPoint expense in neededDataSet)
            {
                expenseDateParsed = expense.date.ToString().Split('/');
                int expenseMonth = Int32.Parse(expenseDateParsed[0]);
                if (expenseMonth == (int)month)
                {
                    monthlyNeededTotal += expense.amount;
                    monthlyTotalSpent += expense.amount;
                    monthlyExpenseListNeeded += expense.ToString() + "\n";
                }
                expenseListNeeded += expense.ToString() + "\n";
            }
        }

        public void UpdateMonthlyDataSets(int month)
        {
            foreach (ExpenseDataPoint fData in foodDataSet)
            {
                if (fData.date.Month == (int)month)
                {

                }
            }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------

        public string ShowOverview(TimeSpan timeSpanParam = TimeSpan.ALL, int month = 1, double year = 2018)
        {
            string output = "";
            

            if (timeSpanParam == TimeSpan.ALL)
            {
                //output += "Gross Income: " + grossIncome.ToString("C") + "\n";
                //output += "\n";
                output += "Food: " + foodTotal.ToString("C") + "\n";
                output += "House: " + houseTotal.ToString("C") + "\n";
                output += "Liesure: " + liesureTotal.ToString("C") + "\n";
                output += "Needed: " + neededTotal.ToString("C") + "\n";
                //output += "Net Income: " + (this.grossIncome - this.totalSpent).ToString("C");

            }
            else if(timeSpanParam == TimeSpan.MONTHLY)
            {
                double monthlyGrossIncome = grossIncome / 12;
                UpdateMonthlyTotals(month);

               // output += "Monthly Gross Income: " + (grossIncome/12).ToString("C") + "\n";
                //output += "Expenses during the month of " + (Month)month + "\n";
                output += "Food: " + monthlyFoodTotal.ToString("C") + "\n";
                output += "House: " + monthlyHouseTotal.ToString("C") + "\n";
                output += "Liesure: " + monthlyLiesureTotal.ToString("C") + "\n";
                output += "Needed: " + monthlyNeededTotal.ToString("C") + "\n";
                //output += "Net Income: " + (monthlyGrossIncome- this.monthlyTotalSpent).ToString("C");
            }
            else if(timeSpanParam == TimeSpan.YEARLY)
            {

            }
            return output;
        }

        /* Return: prints out the where the money was spent for one category specified in parameter */
        public string ShowCategoryDetails(Category cat, TimeSpan tSpan = TimeSpan.ALL, int month = 1)
        {
            //RepeatedCodeFoundIn GetDetailedList
            //us by all categories
            string output = "Gross Income: " + this.grossIncome.ToString("C") + "\n";
            output = cat.ToString().ToUpper()+"\n";
            output += "Name \t Date \t Amount";
            SortedSet<ExpenseDataPoint> catDataSet = new SortedSet<ExpenseDataPoint>();
            switch (cat)
            {
                case Category.OVERALL:
                    //output += "\t Category";
                    if (tSpan == TimeSpan.MONTHLY)
                    {
                        foreach (ExpenseDataPoint fData in foodDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                        foreach (ExpenseDataPoint fData in houseDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                        foreach (ExpenseDataPoint fData in liesureDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                        foreach (ExpenseDataPoint fData in neededDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                    }
                    else
                    {
                        catDataSet.UnionWith(foodDataSet);
                        catDataSet.UnionWith(houseDataSet);
                        catDataSet.UnionWith(liesureDataSet);
                        catDataSet.UnionWith(neededDataSet);
                    }
                    break;
                case Category.FOOD:
                    if (tSpan == TimeSpan.MONTHLY)
                    {
                        foreach (ExpenseDataPoint fData in foodDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                    }
                    else
                    {
                        catDataSet = foodDataSet;
                    }
                    break;
                case Category.HOUSE:
                    if (tSpan == TimeSpan.MONTHLY)
                    {
                        foreach (ExpenseDataPoint fData in houseDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                    }
                    else
                    {
                        catDataSet = houseDataSet;
                    }
                    break;
                case Category.LIESURE:
                    if (tSpan == TimeSpan.MONTHLY)
                    {
                        foreach (ExpenseDataPoint fData in liesureDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                    }
                    else
                    {
                        catDataSet = liesureDataSet;
                    }
                    break;
                case Category.NEEDED:
                    if (tSpan == TimeSpan.MONTHLY)
                    {
                        foreach (ExpenseDataPoint fData in neededDataSet)
                        {
                            if (fData.date.Month == month)
                            {
                                catDataSet.Add((ExpenseDataPoint)fData);
                            }
                        }
                    }
                    else
                    {
                        catDataSet = neededDataSet;
                    }
                    break;
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
