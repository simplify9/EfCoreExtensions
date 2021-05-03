using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using SW.PrimitiveTypes;
using Microsoft.EntityFrameworkCore;

namespace SW.EfCoreExtensions.UnitTests
{
    [TestClass]
    public class SearchyTests
    {

        //[ClassInitialize]
        //public static void ClassInitialize(TestContext tcontext)
        //{
        //}

        [TestMethod]
        public void SearchyFieldSegments()
        {
            //var values = FakeEmployees.Data.Take(3).Select(p => p.FirstName).ToArray();
            var sc = new SearchyCondition(new SearchyFilter
            {
                Value = "120",
                Rule = SearchyRule.GreaterThan,
                Field = $"{nameof(Employee.Salary)}.{nameof(Money.Amount)}"
            });

            var data = FakeEmployees.Data.Search(sc);

            //Assert.AreEqual(FakeEmployees.Data.Where(p => values.Contains(p.FirstName)).Count(), data.Count());
        }

        [TestMethod]
        public void SearchyEqualsToList()
        {
            var values = FakeEmployees.Data.Take(3).Select(p => p.FirstName).ToArray();
            var sc = new SearchyCondition(new SearchyFilter
            {
                Value = values,
                Rule = SearchyRule.EqualsToList,
                Field = nameof(Employee.FirstName)
            });

            var data = FakeEmployees.Data.Search(sc);

            Assert.AreEqual(FakeEmployees.Data.Where(p => values.Contains(p.FirstName)).Count(), data.Count());
        }

        [TestMethod]
        public void SearchyRange()
        {
            var values = new int[] { 30, 40 };
            var sc = new SearchyCondition(new SearchyFilter
            {
                Value = values,
                Rule = SearchyRule.Range,
                Field = nameof(Employee.Age)
            });

            var data = FakeEmployees.Data.Search(sc);

            Assert.AreEqual(FakeEmployees.Data.Where(p => p.Age >= values[0] && p.Age < values[1]).Count(), data.Count());
        }

        [TestMethod]
        public void SearchyOrderBy()
        {
            SearchyCondition _sc = new SearchyCondition(new SearchyFilter { Value = FakeEmployees.Data.First().FirstName, Rule = SearchyRule.Contains, Field = nameof(Employee.FirstName) });
            List<SearchySort> _ob = new List<SearchySort> { new SearchySort(nameof(Employee.LastName), SearchySortOrder.ASC) };
            var _data = FakeEmployees.Data.Search(new SearchyCondition[] { _sc }, _ob, 0, 0).ToList();
        }

        [TestMethod]
        public void SearchyPaging()
        {
            var _data0 = FakeEmployees.Data.Search(new SearchyCondition[] { }, null, 10, 3).ToList();
            Assert.AreEqual(10, _data0.Count);
        }

        [TestMethod]
        public void SearchyMultipleFilters()
        {
            List<SearchyFilter> _fol = new List<SearchyFilter>
            {
                new SearchyFilter { Value = FakeEmployees.Data.First().FirstName, Rule = SearchyRule.Contains, Field = nameof(Employee.FirstName)  },
                new SearchyFilter { Value = FakeEmployees.Data.First().LastName, Rule = SearchyRule.EqualsTo, Field = nameof(Employee.LastName) }
            };

            SearchyCondition _sc = new SearchyCondition(_fol);
            var _data = FakeEmployees.Data.Search(_sc).ToList();

        }
        [TestMethod]
        public void SearchyMultipleConditions()
        {
            SearchyCondition _sc1 = new SearchyCondition(nameof(Employee.FirstName), SearchyRule.EqualsTo, FakeEmployees.Data.First().FirstName);
            SearchyCondition _sc2 = new SearchyCondition(nameof(Employee.LastName), SearchyRule.EqualsTo, FakeEmployees.Data.First().LastName);

            var _data = FakeEmployees.Data.Search(new SearchyCondition[] { _sc1, _sc2 }).ToList();
        }


        [TestMethod]
        public void SearchyMany()
        {
            SearchyCondition _sc1 = new SearchyCondition($"{nameof(Leave.Days)}", SearchyRule.GreaterThan,7);

            var data = FakeEmployees.Data.Search<Employee,Leave>("Leaves",new SearchyCondition[] { _sc1}).ToList();

            Assert.AreEqual(FakeEmployees.Data.Where(s => s.Leaves.Any(l => l.Days > 7)).Count(), data.Count());
        }


        [TestMethod]
        public void SearchyManyWithPaging()
        {
            SearchyCondition _sc1 = new SearchyCondition($"{nameof(Leave.Days)}", SearchyRule.GreaterThan, 7);

            var data = FakeEmployees.Data.Search<Employee, Leave>("Leaves", new SearchyCondition[] { _sc1 }, new SearchySort[] { new SearchySort { Field = "Id", Sort = SearchySortOrder.DEC } },5).ToList();

            Assert.AreEqual(5, data.Count());
        }

        [TestMethod]
        public void IQueryableIf()
        {
         
            var emps = FakeEmployees.Data.ToList();
            var emp = emps.First();
            var  EmpsAppliedCond = FakeEmployees.Data.If(emp != default, q => q.Where(s => s.Id == emp.Id)).ToList();
            Assert.AreEqual(EmpsAppliedCond.Count, 1);
            Assert.AreEqual(EmpsAppliedCond.First().Id, emp.Id);

            emp = default;
            var EmpsDoesntApplyCond = FakeEmployees.Data.If(emp != default, q => q.Where(s => s.Id == 0)).ToList();
            Assert.AreEqual(EmpsDoesntApplyCond.Count, emps.Count);
        }

        [TestMethod]
        public void  IQueryableIncludableQueryableIf()
        {

            var emps = FakeEmployees.Data.ToList();
            var emp = emps.First();
            var empsApplyCondt = FakeEmployees.Data.If(emp != default, q => q.Include(s => s.Leaves)).ToList();
            Assert.AreNotEqual(empsApplyCondt.First().Leaves, null);

            emp = default;
            var empsDoesntApplyCond = FakeEmployees.Data.If(emp != default, q => q.Include(s => s.Leaves)).ToList();
            Assert.AreNotEqual(empsDoesntApplyCond.First().Leaves, null);

        }

    }
}
