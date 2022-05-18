using MetricsManagerClient.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MetricsManagerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Client.MetricsManagerClient _metricsAgentClient;
        private List<Agent> _agents;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Agent> Agents
        {
            get => _agents;
            set
            {
                _agents = value;
                OnPropertyChanged(nameof(Agents));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _metricsAgentClient = new(new());
            DataContext = this;

            ChartCpu.GetMetrics = _metricsAgentClient.GetCpuMetrics;
            ChartRam.GetMetrics = _metricsAgentClient.GetRamMetrics;
            ChartHdd.GetMetrics = _metricsAgentClient.GetHddMetrics;
            ChartNetwork.GetMetrics = _metricsAgentClient.GetNetworkMetrics;
            ChartDotNet.GetMetrics = _metricsAgentClient.GetDotNetMetrics;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdateAgents(object sender, RoutedEventArgs e)
        {
            Agents = await _metricsAgentClient.GetAgents();
            if (Agents == null)
                MessageBox.Show("Не удалось получить список агентов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SelectedAgentChanged(object sender, SelectionChangedEventArgs e)
        {
            int agentId = (int)cbAgents.SelectedValue;
            ChartCpu.AgentId = agentId;
            ChartRam.AgentId = agentId;
            ChartHdd.AgentId = agentId;
            ChartNetwork.AgentId = agentId;
            ChartDotNet.AgentId = agentId;

            ChartCpu.ColumnSeriesValues = null;
            ChartRam.ColumnSeriesValues = null;
            ChartHdd.ColumnSeriesValues = null;
            ChartNetwork.ColumnSeriesValues = null;
            ChartDotNet.ColumnSeriesValues = null;
        }
    }
}
