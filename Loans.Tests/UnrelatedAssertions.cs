using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Loans.Tests
{
    [TestFixture]
    class UnrelatedAssertions
    {

        //for and exhaustive list, head here:
        //https://github.com/nunit/docs/wiki/Constraints
        [Test]
        public void StringTestsExamples()
        {
            //Commenting out failing Assertions but leaving for examples
            string name = "Sarah";

            //Assert.That(name, Is.Empty);//fail
            
            Assert.That(name, Is.Not.Empty);
            
            Assert.That(name, Is.EqualTo("Sarah"));
            
            //Assert.That(name, Is.EqualTo("SARAH"));//fail
            
            Assert.That(name, Is.EqualTo("SARAH").IgnoreCase);
            
            Assert.That(name, Does.StartWith("Sa"));
            
            Assert.That(name, Does.EndWith("ah"));
            
            Assert.That(name, Does.Contain("ara"));
            
            Assert.That(name, Does.Not.Contain("Brian"));
            
            Assert.That(name, Does.StartWith("Sa")
                .And.EndWith("rah"));

            Assert.That(name, Does.StartWith("xyz")
                .Or.EndWith("rah"));
        }

        [Test]
        public void BooleanTestsExamples()
        {
            bool isNew = true;

            Assert.That(isNew);
            Assert.That(isNew, Is.True);

            bool areMarried = false;
            
            Assert.That(areMarried == false);
            Assert.That(areMarried, Is.False);
            Assert.That(areMarried, Is.Not.True);
        }

        [Test]
        public void RangeTestsExamples()
        {
            int i = 42;

            //Assert.That(i, Is.GreaterThan(42));
            Assert.That(i, Is.GreaterThanOrEqualTo(42));
            //Assert.That(i, Is.LessThan(42));
            Assert.That(i, Is.LessThanOrEqualTo(42));
            Assert.That(i, Is.InRange(40, 50));
        }

        [Test]
        public void DateRangeTestsExamples()
        {
            DateTime d1 = new DateTime(2000, 2, 20);
            DateTime d2 = new DateTime(2000, 2, 25);

            //Assert.That(d1, Is.EqualTo(d2));
            //Assert.That(d1, Is.EqualTo(d2).Within(2).Days);
            Assert.That(d1, Is.EqualTo(d2).Within(5).Days);
        }

    }
}
