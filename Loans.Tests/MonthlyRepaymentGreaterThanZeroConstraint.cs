using System;
using System.Collections.Generic;
using System.Text;
using Loans.Domain.Applications;
using NUnit.Framework.Constraints;

namespace Loans.Tests
{
    class MonthlyRepaymentGreaterThanZeroConstraint : Constraint
    {
        public string ExpectedProductName { get; }
        public decimal ExpectedInterestRate { get; }

        public MonthlyRepaymentGreaterThanZeroConstraint(string expectedProductName, decimal expectedInterestRate)
        {
            ExpectedProductName = expectedProductName;
            ExpectedInterestRate = expectedInterestRate;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            MonthlyRepaymentComparison comparison = actual as MonthlyRepaymentComparison;
            
            //if "actual" parameter is not of the correct type, in this case a MonthlyRepaymentComparison,
            //then the cast above will fail and comparison will be null below, working as a type check.
            if (comparison is null)
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Error);
            }

            //Since now we know we are dealing with a MonthlyRepaymentComparison parameter, we can start comparing
            if (comparison.ProductName == ExpectedProductName &&
                comparison.InterestRate == ExpectedInterestRate &&
                comparison.MonthlyRepayment > 0)
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Success);
            }

            return  new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }
    }
}
