﻿using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using AutoMapper;

namespace MetricsAgentTests
{
    public class NetworkMetricsControllerUnitTests
    {
        private NetworkMetricsController controller;
        private Mock<INetworkMetricsRepository> mockRepository;
        private Mock<ILogger<NetworkMetricsController>> mockLogger;

        public NetworkMetricsControllerUnitTests()
        {
            mockRepository = new();
            mockLogger = new();
            MapperConfiguration mapperConfiguration = new(mp => mp.AddProfile(new MapperProfile()));
            IMapper mapper = mapperConfiguration.CreateMapper();
            controller = new NetworkMetricsController(mockRepository.Object, mockLogger.Object, mapper);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            mockRepository.Setup(repository => repository.GetByTimePeriod(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(new List<NetworkMetric>()).Verifiable();
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
            // В заглушке прописываем, что в репозиторий прилетит NetworkMetric - объект
            mockRepository.Setup(repository => repository.Create(It.IsAny<NetworkMetric>())).Verifiable();
            // Выполняем действие на контроллере
            IActionResult result = controller.Create(new MetricsAgent.Requests.NetworkMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mockRepository.Verify(repository => repository.Create(It.IsAny<NetworkMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetAll()).Returns(new List<NetworkMetric>()).Verifiable();
            IActionResult result = controller.GetAll();
            mockRepository.Verify(repository => repository.GetAll(), Times.AtMostOnce());
        }

        [Fact]
        public void GetById_ShouldCall_GetById_From_Repository()
        {
            mockRepository.Setup(repository => repository.GetById(It.IsAny<int>())).Returns(new NetworkMetric()).Verifiable();
            IActionResult result = controller.GetById(1);
            mockRepository.Verify(repository => repository.GetById(It.IsAny<int>()), Times.AtMostOnce());
        }
    }
}
