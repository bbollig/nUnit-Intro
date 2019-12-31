using System;
using Loans.Domain.Applications;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Loans.Tests
{
    [TestFixture]
    //[Ignore("Need to complete update work.")]
    public class LoanTermShould
    {
        [Test]
        //[Ignore("Need to complete update work.")]
        public void ReturnTermInMonths()
        {
            //Constraint method of assertion is the new feature
            //Assert.That(*test result, *constraint instance);
            //vs. Classic method of assertion which would look like this:
            //Assert.AreEqual(1, sut.Years);
            var sut = new LoanTerm(1);
            Assert.That(sut.ToMonths(), Is.EqualTo(12), "Months should be 12* number of years.");
        }

        [Test]
        public void StoreYears()
        {
            var sut = new LoanTerm(1);
            Assert.That(sut.Years, Is.EqualTo(1));
        }

        [Test]
        public void RespectValueEquality()
        {
            var a = new LoanTerm(1);
            var b = new LoanTerm(1);

            Assert.That(a, Is.EqualTo(b));


        }

        [Test]
        public void RespectValueInEquality()
        {
            var a = new LoanTerm(1);
            var b = new LoanTerm(2);

            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void ReferenceEqualityExample()
        {
            var a = new LoanTerm(1);
            var b = a;
            var c =  new LoanTerm(1);

            Assert.That(a, Is.SameAs(b));
            Assert.That(a, Is.Not.SameAs(c));

            var x = new List<string> {"a", "b"};
            var y = x;
            var z = new List<string> { "a", "b" };

            Assert.That(y, Is.SameAs(x));
            Assert.That(z, Is.Not.SameAs(x));
        }

        [Test]
        public void Double()
        {
            double a = 1.0 / 3.0;
            //can add extension method to EqualTo to specify a tolerance
            Assert.That(a, Is.EqualTo(0.33).Within(0.004));
            //as a percent
            Assert.That(a, Is.EqualTo(0.33).Within(10).Percent);
        }

        [Test]
        public void NotAllowZeroYears()
        {
            //check if particular error type is thrown
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>());

            //check if particular error type is thrown with particular property having specific value
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property("Message")
                .EqualTo("Please specify a value greater than 0. (Parameter 'years')"));

            //checking the Error message of an error of type x
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Message
                .EqualTo("Please specify a value greater than 0. (Parameter 'years')"));

            //checking the value of a particular Property
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property("ParamName")
                .EqualTo("years"));

            //same as above but using Matches extension with lambda
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                                                   .With
                                                   .Matches<ArgumentOutOfRangeException>(
                                                   e => e.ParamName == "years"));

        }
    }
}
