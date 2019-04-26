﻿using System;
using System.Collections.Generic;

namespace BudgetApp
{

    public enum Category { FOOD, HOUSE, LIESURE, NEEDED };
    public enum TimeSpan { ALL, MONTHLY, YEARLY };
    public enum Month { January = 1, February, March, April, May, June, July, August, September, November, December };


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
    class ExpenseDataPointComparer : IComparer<ExpenseDataPoint>
    {
        public int Compare(ExpenseDataPoint x, ExpenseDataPoint y)
        {
            if (x.date.CompareTo(y.date) == 0)
            {
                return 1;
            }
            return x.date.CompareTo(y.date);
        }
    }
    struct ExpenseDataPoint
    {
        public string name;
        public double amount;
        public DateTime date;
        public ExpenseDataPoint(string n, double a, DateTime d)
        {
            name = n;
            amount = a;
            date = d;
        }
        public override string ToString()
        {
            return name + " " + amount + " " + date;
        }
    }


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
        

        private String expenseListFood;
        private String monthlyExpenseListFood;
        private String expenseListHouse;
        private String monthlyExpenseListHouse;
        private String expenseListLiesure;
        private String monthlyExpenseListLiesure;
        private String expenseListNeeded;
        private String monthlyExpenseListNeeded;


        /* Contructor gives default values upon instantiation */
        public Expenses()
        {
            foodTotal = 0;
            monthlyFoodTotal = 0;
            foodDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            houseTotal = 0;
            monthlyHouseTotal = 0;
            houseDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            liesureTotal = 0;
            monthlyLiesureTotal = 0;
            liesureDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            neededTotal = 0;
            monthlyNeededTotal = 0;
            neededDataSet = new SortedSet<ExpenseDataPoint>(new ExpenseDataPointComparer());
            totalSpent = 0;
            monthlyTotalSpent = 0;
            grossIncome = 0;
        }

       
        /* Gets details on an expense made and updates the corresponding set and totals */
        public int AddExpense(string name, double amount, DateTime date, Category category)
        {
            ExpenseDataPoint dataPoint = new ExpenseDataPoint(name, amount, date);
            
            switch (category)
            {
                case Category.FOOD:
                    foodTotal += amount;
                    foodDataSet.Add(dataPoint);
                    break;
                case Category.HOUSE:
                    houseTotal += amount;
                    houseDataSet.Add(dataPoint);
                    break;
                case Category.LIESURE:
                    liesureTotal += amount;
                    liesureDataSet.Add(dataPoint);
                    break;
                case Category.NEEDED:
                    neededTotal += amount;
                    neededDataSet.Add(dataPoint);
                    break;
            }

            totalSpent += amount;
            return 0;
        }

        public string UpdateGrossIncome(double grossIncome)
        {
            this.grossIncome = grossIncome;
            string output = "Gross Income: " + grossIncome.ToString("C") + "\n";
            return output;
        }

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


        /// <summary>
        /// Throws as exception if income is a negative number
        /// Calls UpdateMonthlyTotals if TimeSpan = MONTHLY
        /// </summary>
        /// <param name="timeSpanParam"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns>overview of all money spent in a string</returns>
        public string ShowOverview(TimeSpan timeSpanParam = TimeSpan.ALL, int month = 1, double year = 2018)
        {
            string output = "";
            

            if (timeSpanParam == TimeSpan.ALL)
            {
                output += "Gross Income: " + grossIncome.ToString("C") + "\n";
                output += "\n";
                output += "Food: " + foodTotal.ToString("C") + "\n";
                output += "House: " + houseTotal.ToString("C") + "\n";
                output += "Liesure: " + liesureTotal.ToString("C") + "\n";
                output += "Needed: " + neededTotal.ToString("C") + "\n";
                output += "Net Income: " + (this.grossIncome - this.totalSpent).ToString("C");

            }
            else if(timeSpanParam == TimeSpan.MONTHLY)
            {
                double monthlyGrossIncome = grossIncome / 12;
                UpdateMonthlyTotals(month);

                output += "Monthly Gross Income: " + (grossIncome/12).ToString("C") + "\n";
                output += "Expenses during the month of " + (Month)month + "\n";
                output += "Food: " + monthlyFoodTotal.ToString("C") + "\n";
                output += "House: " + monthlyHouseTotal.ToString("C") + "\n";
                output += "Liesure: " + monthlyLiesureTotal.ToString("C") + "\n";
                output += "Needed: " + monthlyNeededTotal.ToString("C") + "\n";
                output += "Net Income: " + (monthlyGrossIncome- this.monthlyTotalSpent).ToString("C");
            }
            else if(timeSpanParam == TimeSpan.YEARLY)
            {

            }
            return output;
        }

        /* Return: prints out the where the money was spent for one category specified in parameter */
        public string ShowCategoryDetails(Category cat)
        {
            //RepeatedCodeFoundIn GetDetailedList
            //us by all categories
            string output = "Gross Income: " + this.grossIncome.ToString("C") + "\n";
            output = cat.ToString().ToUpper()+"\n";
            output += "Name Date Amount\n";
            SortedSet<ExpenseDataPoint> catDataSet = new SortedSet<ExpenseDataPoint>();
            switch (cat)
            {
                case Category.FOOD:
                    catDataSet = foodDataSet;
                    break;
                case Category.HOUSE:
                    catDataSet = houseDataSet;
                    break;
                case Category.LIESURE:
                    catDataSet = liesureDataSet;
                    break;
                case Category.NEEDED:
                    catDataSet = neededDataSet;
                    break;
            }
            foreach (ExpenseDataPoint point in catDataSet)
            {
                output += point.name + " " + point.date + " "+ point.amount.ToString("C") + "\n";
            }
            return output;
        }


        
        
    }
}