﻿using MetricsAgent.Controllers;
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
    public class DotNetMetricsControllerUnitTests
    {
        private DotNetMetricsController controller;
        private Mock<IDotNetMetricsRepository> mockRepository;
        private Mock<ILogger<DotNetMetricsController>> mockLogger;

        public DotNetMetricsControllerUnitTests()
        {
            mockRepository = new();
            mockLogger = new();
            controller = new DotNetMetricsController(mockRepository.Object, mockLogger.Object);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            mockRepository.Setup(repository => repository.GetByTimePeriod(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(new List<DotNetMetric>()).Verifiable();
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
            // В заглушке прописываем, что в репозиторий прилетит DotNetMetric - объект
            mockRepository.Setup(repository => repository.Create(It.IsAny<DotNetMetric>())).Verifiable();
            // Выполняем действие на контроллере
            IActionResult result = controller.Create(new MetricsAgent.Requests.DotNetMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mockRepository.Verify(repository => repository.Create(It.IsAny<DotNetMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetAll()).Returns(new List<DotNetMetric>()).Verifiable();
            IActionResult result = controller.GetAll();
            mockRepository.Verify(repository => repository.GetAll(), Times.AtMostOnce());
        }

        [Fact]
        public void GetById_ShouldCall_GetById_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetById(It.IsAny<int>())).Returns(new DotNetMetric()).Verifiable();
            IActionResult result = controller.GetById(1);
            mockRepository.Verify(repository => repository.GetById(It.IsAny<int>()), Times.AtMostOnce());
        }
    }
}
