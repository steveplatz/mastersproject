﻿using BrewersBuddy.Controllers;
using BrewersBuddy.Models;
using BrewersBuddy.Services;
using NSubstitute;
using NUnit.Framework;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System;

namespace BrewersBuddy.Tests.Controllers
{
    [TestFixture]
    public class BatchRatingControllerTest
    {
        [Test]
        public void TestGetCreateWithAnonymousUserWillThrowUnauthorized()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(0);

            var ratingService = Substitute.For<IBatchRatingService>();
            var batchService = Substitute.For<IBatchService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(1);

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        public void TestPostCreateWithAnonymousUserWillThrowUnauthorized()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(0);

            var ratingService = Substitute.For<IBatchRatingService>();
            var batchService = Substitute.For<IBatchService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(new BatchRating());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        public void TestInvalidModelStateWillReturn500Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            controller.ModelState.AddModelError("key", "not valid");

            ActionResult result = controller.Create(new BatchRating()
            {
                Comment = "Test comment"
            });

            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual(500, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestPostPreviousRatingWillReturn403Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch());

            var ratingService = Substitute.For<IBatchRatingService>();
            ratingService.GetUserRatingForBatch(1, 1).Returns(new BatchRating());

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(new BatchRating()
            {
                BatchId = 1
            });

            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual(403, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestGetPreviousRatingWillReturn403Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch());

            var ratingService = Substitute.For<IBatchRatingService>();
            ratingService.GetUserRatingForBatch(1, 1).Returns(new BatchRating());

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(1);

            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual(403, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestPostNonExistingBatchRetuns404Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(null, null);

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(new BatchRating()
            {
                BatchId = 1
            });

            Assert.IsInstanceOf<HttpNotFoundResult>(result);
            Assert.AreEqual(404, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestGetNonExistingBatchRetuns404Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(null, null);

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(1);

            Assert.IsInstanceOf<HttpNotFoundResult>(result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void ValidInputReturnsJson()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch()
            {
                OwnerId = 1
            });

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(new BatchRating()
            {
                BatchId = 1,
                Rating = 95,
                Comment = "My comment"
            });

            Assert.IsInstanceOf<JsonResult>(result);
        }

        [Test]
        public void TestViewBagPopulated()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch()
            {
                OwnerId = 1,
                Name = "Batch name"
            });

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Create(1);

            ViewResult view = result as ViewResult;

            Assert.NotNull(view.ViewBag.Ratings);
            Assert.AreEqual(1, view.ViewBag.BatchId);
            Assert.NotNull("Batch name", view.ViewBag.BatchName);
        }

        [Test]
        public void TestAverageReturnsAverage()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch()
            {
                OwnerId = 1,
                Ratings = new Collection<BatchRating>(new BatchRating[]{
                    new BatchRating() { Rating = 2 },
                    new BatchRating() { Rating = 5 },
                    new BatchRating() { Rating = 100 },
                    new BatchRating() { Rating = 30 },
                    new BatchRating() { Rating = 70 },
                })
            });

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Average(1);

            Assert.IsInstanceOf<JsonResult>(result);

            Assert.AreEqual(41.4, ((JsonResult)result).Data);
        }

        [Test]
        public void TestAverageWithNoRatingsReturnsZero()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(new Batch()
            {
                OwnerId = 1
            });

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Average(1);

            Assert.IsInstanceOf<JsonResult>(result);

            Assert.AreEqual(0, ((JsonResult)result).Data);
        }

        [Test]
        public void TestAverageWithAnonymousUserWillThrowUnauthorized()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(0);

            var ratingService = Substitute.For<IBatchRatingService>();
            var batchService = Substitute.For<IBatchService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Average(1);

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        public void TestAverageWithNonExistingBatchReturns404Error()
        {
            // Set up the controller
            var userService = Substitute.For<IUserService>();
            userService.GetCurrentUserId().Returns(1);

            var batchService = Substitute.For<IBatchService>();
            batchService.Get(1).Returns(null, null);

            var ratingService = Substitute.For<IBatchRatingService>();

            BatchRatingController controller = new BatchRatingController(ratingService, batchService, userService);

            ActionResult result = controller.Average(1);

            Assert.IsInstanceOf<HttpNotFoundResult>(result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void TestNullRatingServiceThrowsArgumentNullException()
        {
            var userService = Substitute.For<IUserService>();
            var batchService = Substitute.For<IBatchService>();

            Assert.Throws<ArgumentNullException>(() =>
                new BatchRatingController(null, batchService, userService)
                );
        }

        [Test]
        public void TestNullBatchServiceThrowsArgumentNullException()
        {
            var userService = Substitute.For<IUserService>();
            var ratingService = Substitute.For<IBatchRatingService>();

            Assert.Throws<ArgumentNullException>(() =>
                new BatchRatingController(ratingService, null, userService)
                );
        }

        [Test]
        public void TestNullUserServiceThrowsArgumentNullException()
        {
            var batchService = Substitute.For<IBatchService>();
            var ratingService = Substitute.For<IBatchRatingService>();

            Assert.Throws<ArgumentNullException>(() =>
                new BatchRatingController(ratingService, batchService, null)
                );
        }
    }
}
