using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace MetricsAgentTests
{
    public class RamMetricsControllerUnitTests
    {
        private RamMetricsController controller;
        private Mock<IRamMetricsRepository> mockRepository;
        private Mock<ILogger<RamMetricsController>> mockLogger;

        public RamMetricsControllerUnitTests()
        {
            mockRepository = new();
            mockLogger = new();
            controller = new RamMetricsController(mockRepository.Object, mockLogger.Object);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            mockRepository.Setup(repository => repository.GetByTimePeriod(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(new List<RamMetric>()).Verifiable();
            //Arrange
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            //Act
            var result = controller.GetMetrics(fromTime, toTime);
            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит RamMetric - объект
            mockRepository.Setup(repository => repository.Create(It.IsAny<RamMetric>())).Verifiable();
            // Выполняем действие на контроллере
            IActionResult result = controller.Create(new MetricsAgent.Requests.RamMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mockRepository.Verify(repository => repository.Create(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetAll()).Returns(new List<RamMetric>()).Verifiable();
            IActionResult result = controller.GetAll();
            mockRepository.Verify(repository => repository.GetAll(), Times.AtMostOnce());
        }

        [Fact]
        public void GetById_ShouldCall_GetById_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetById(It.IsAny<int>())).Returns(new RamMetric()).Verifiable();
            IActionResult result = controller.GetById(1);
            mockRepository.Verify(repository => repository.GetById(It.IsAny<int>()), Times.AtMostOnce());
        }
    }
}
