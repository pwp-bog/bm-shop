using System;
using System.Threading.Tasks;
using bm_shop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;

namespace UWPTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Connect_DataBase_ReturnNone()
        {
            try
            {
                DB dB = new DB();
                dB.openConnection();

                var connection = dB.getConnection();

                Assert.IsNotNull(connection, "Passed: TestConnect_DataBase");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_GetCorrectWordFormForProduct_ReturnString()
        {
            try
            {
                int TestNum = 5;
                string res;


                res = Basket.GetCorrectWordFormForProduct(TestNum);


                Assert.IsNotNull(res, "Passed: Test_GetCorrectWordFormForProduct");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_GetCorrectAnswerCount_ReturnString()
        {
            try
            {
                int TestNum = 4;
                string res;


                res = CommentsPage.GetCorrectAnswerCount(TestNum);


                Assert.IsNotNull(res, "Passed: Test_GetCorrectAnswerCount");
            }
            catch (Exception ex)
            {

                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_GetMaterialMark_ReturnInt()
        {
            try
            {
                string one = "★";
                string two = "★";
                string three = "★";
                string four = "☆";
                string five = "☆";
                int res;


                res = CommentsPage.GetMaterialMark(one, two, three, four, five);


                Assert.IsNotNull(res, "Passed: Test_GetMaterialMark");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_GetQuantityStar_ReturnString()
        {
            try
            {
                string TestValue = "5";
                string res;


                res = CommentsPage.GetQuantityStar(TestValue);


                Assert.IsNotNull(res, "Passed: Test_GetQuantityStar");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_ConvertToDate_ReturnString()
        {
            try
            {
                string TestValue = "2024-06-04";
                string res;


                res = CommentsPage.ConvertToDate(TestValue);


                Assert.IsNotNull(res, "Passed: Test_ConvertToDate");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }
    }
}
