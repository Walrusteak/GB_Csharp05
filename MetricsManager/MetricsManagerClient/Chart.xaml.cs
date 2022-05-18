using LiveCharts;
using LiveCharts.Wpf;
using MetricsManagerClient.Models;
using MetricsManagerClient.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetricsManagerClient
{
    /// <summary>
    /// Interaction logic for CpuChart.xaml
    /// </summary>
    public partial class Chart : UserControl, INotifyPropertyChanged
    {
        private SeriesCollection _columnSeriesValues;
        private int _minAxisValue;
        private int _maxAxisValue;

        internal delegate Task<List<IMetric>> GetMetricsFunc(GetMetricsApiRequest request);
        public event PropertyChangedEventHandler PropertyChanged;

        internal GetMetricsFunc GetMetrics { get; set; }

        public SeriesCollection ColumnSeriesValues
        {
            get => _columnSeriesValues;
            set
            {
                _columnSeriesValues = value;
                if (_columnSeriesValues == null)
                    BorderChart.Background = new SolidColorBrush(Colors.Gray);
                OnPropertyChanged(nameof(ColumnSeriesValues));
            }
        }

        public int MinAxisValue
        {
            get => _minAxisValue;
            set
            {
                _minAxisValue = value;
                OnPropertyChanged(nameof(MinAxisValue));
            }
        }

        public int MaxAxisValue
        {
            get => _maxAxisValue;
            set
            {
                _maxAxisValue = value;
                OnPropertyChanged(nameof(MaxAxisValue));
            }
        }

        public int AgentId { get; set; }

        public string Caption
        {
            get => caption.Text;
            set => caption.Text = value;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public Chart()
        {
            InitializeComponent();
            DataContext = this;
            _minAxisValue = 0;
            _maxAxisValue = 100;
        }

        private async void UpdateOnСlick(object sender, RoutedEventArgs e)
        {
            List<IMetric> metrics = await GetMetrics(new()
            {
                AgentId = AgentId,
                FromTime = TimeSpan.FromSeconds(0),
                ToTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks)
            });

            if (metrics == null || !metrics.Any())
                return;

            List<int> values = metrics.Where(x => x != null).TakeLast(100).Select(x => x.Value).ToList();
            int minAxisValue = (int)(values.Min() * 0.9);
            if (Math.Abs(minAxisValue) < 100)
                minAxisValue = 0;
            MinAxisValue = minAxisValue;
            int maxAxisValue = (int)(values.Max() * 1.1);
            if (Math.Abs(maxAxisValue - 100) < 100)
                maxAxisValue = 100;
            MaxAxisValue = maxAxisValue;

            double avg = values.Average();
            AverageTextBlock.Text = $"{avg:F2}";

            BorderChart.Background = new SolidColorBrush((values.Last() / avg) switch
            {
                < 0.5 => Colors.Green,
                < 1 => Colors.GreenYellow,
                < 1.5 => Colors.OrangeRed,
                _ => Colors.IndianRed
            });                

            ColumnSeriesValues = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<int>(values)
                }
            };
        }
    }
}
