using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.UnitTest.Mock;
using LearnerApp.ViewModels;
using Moq;
using Xunit;

namespace LearnerApp.UnitTest.ViewModel
{
    public class CoursesViewModelTest : IClassFixture<XamarinFixture>
    {
        public CoursesViewModelTest()
        {
            // Will run on each test
            MockServiceManager.Current
                .MockIdentityService()
                .MockDialogService()
                .MockNavigationService()
                .MockExceptionHandler();
        }

        [Fact]
        public async Task WhenNavigatedToNewlyAddedCoursePage_ExpectCallTheRightApiToLoadData()
        {
            // Arrange
            var commonServiceMock = new Moq.Mock<ICommonServices>();
            commonServiceMock.Setup(x => x.GetNewlyAddedCollection(
                1,
                It.IsAny<Action<int>>()))
                .ReturnsAsync(new List<ItemCard>()
                {
                    new ItemCard()
                    {
                        Id = "Something"
                    }
                });

            MockServiceManager.Current.MockService(commonServiceMock.Object);
            MockServiceManager.Current.Build();

            var coursesViewModel = new CoursesViewModel();
            bool isCourseCollectionTriggered = false;

            coursesViewModel.PropertyChanged += (sender, args) =>
            {
                isCourseCollectionTriggered = args.PropertyName == nameof(coursesViewModel.CourseCollection);
            };
            var navigationParameters = new NavigationParameters();
            navigationParameters.SetParameter("SourceScreen", LearningScreenType.NewlyAdded.ToString());

            // Action
            await coursesViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            commonServiceMock.Verify(
                x => x.GetNewlyAddedCollection(
                    It.IsAny<int>(),
                    It.IsAny<Action<int>>()),
                Times.Once);

            Assert.Single(coursesViewModel.CourseCollection);
            Assert.True(isCourseCollectionTriggered);
        }

        [Fact]
        public async Task WhenNavigatedToRecommendationsForYouPage_ExpectCallTheRightApiToLoadData()
        {
            // Arrange
            var commonServiceMock = new Moq.Mock<ICommonServices>();
            commonServiceMock.Setup(x => x.GetRecommendationsCollection(
                    It.IsAny<string>(),
                    1,
                    It.IsAny<Action<int>>()))
                .ReturnsAsync(new List<ItemCard>()
                {
                    new ItemCard()
                    {
                        Id = "Something"
                    }
                });

            MockServiceManager.Current.MockService(commonServiceMock.Object);
            MockServiceManager.Current.Build();

            var coursesViewModel = new CoursesViewModel();

            var navigationParameters = new NavigationParameters();
            navigationParameters.SetParameter("SourceScreen", LearningScreenType.RecommendationsForYou.ToString());

            bool isCourseCollectionTriggered = false;
            coursesViewModel.PropertyChanged += (sender, args) =>
            {
                isCourseCollectionTriggered = args.PropertyName == nameof(coursesViewModel.CourseCollection);
            };

            // Action
            await coursesViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            commonServiceMock.Verify(
                x => x.GetRecommendationsCollection(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<Action<int>>()),
                Times.Once);

            Assert.Single(coursesViewModel.CourseCollection);
            Assert.True(isCourseCollectionTriggered);
        }
    }
}
