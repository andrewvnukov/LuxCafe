using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LiveCharts.Wpf;
using Cafe_Managment.Repositories;
using System.Windows.Input;
using System.Windows;
using Cafe_Managment.Model;
using Cafe_Managment.Controls;

namespace Cafe_Managment.ViewModel
{
    internal class StatisticVM : ViewModelBase
    {
        double _graphWidth;
        public ObservableCollection<DishData> AllDishes { get; set; }
        public DishData SelectedDish { get; set; }
    


        public ICommand UpdateIncomeChartx { get; set; }
        public ICommand FillIncomeChartx { get; set; }
        public ICommand LoadPopularDishes { get; set; }
        public ICommand LoadUnpopularDishes { get; set; }
        public ICommand LoadTrendCommand { get; set; }



        public ObservableCollection<string> TrendLabels { get; set; }


        StatisticsRepository statisticsRepository;


        public new event PropertyChangedEventHandler PropertyChanged;

        private SeriesCollection _incomeSeriesCollection;
        public SeriesCollection IncomeSeriesCollection
        {
            get { return _incomeSeriesCollection; }
            set { _incomeSeriesCollection = value;
                OnPropertyChanged(nameof(IncomeSeriesCollection));
            }
        }

        public double GraphWidth
        {
            get { return _graphWidth; }
            set { _graphWidth = value;
            OnPropertyChanged(nameof(GraphWidth));}
        } 

        private ObservableCollection<string> _labels;
        public ObservableCollection<string> Labels
        {
            get { return _labels; }
            set { _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }


        private SeriesCollection _popularDishesSeriesCollection;
        public SeriesCollection PopularDishesSeriesCollection
        {
            get { return _popularDishesSeriesCollection; }
            set
            {
                _popularDishesSeriesCollection = value;
                OnPropertyChanged(nameof(PopularDishesSeriesCollection));
            }
        }

        private SeriesCollection _unpopularDishesSeriesCollection;
        public SeriesCollection UnpopularDishesSeriesCollection
        {
            get { return _unpopularDishesSeriesCollection; }
            set
            {
                _unpopularDishesSeriesCollection = value;
                OnPropertyChanged(nameof(UnpopularDishesSeriesCollection));
            }
        }


        private SeriesCollection _trendSeriesCollection;
        public SeriesCollection TrendSeriesCollection
        {
            get { return _trendSeriesCollection; }
            set
            {
                _trendSeriesCollection = value;
                OnPropertyChanged(nameof(TrendSeriesCollection));
            }
        }


        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

       

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                //LoadData(); // Перезагружаем данные при изменении конечной даты
            }
        }

        public StatisticVM()
        {
            //// Устанавливаем конечную дату в текущую дату
            EndDate = DateTime.Today;

            //// Устанавливаем начальную дату как конечную дату минус 30 дней
            StartDate = EndDate.AddDays(-30);

            PopularDishesSeriesCollection = new SeriesCollection();
            UnpopularDishesSeriesCollection = new SeriesCollection();
            LoadTrendCommand = new RelayCommand(ExecuteLoadTrendCommand);

            // Предварительная загрузка всех блюд в комбобокс
            // Инициализация IncomeSeriesCollection
            IncomeSeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>(); 

            statisticsRepository = new StatisticsRepository();
            // Вызываем LoadData после инициализации IncomeSeriesCollection
            UpdateIncomeChartx = new RelayCommand(ExecuteUpdateIncomeChart);
            LoadPopularDishes = new RelayCommand(ExecuteLoadPopularDishes);
            LoadUnpopularDishes = new RelayCommand(ExecuteLoadUnpopularDishes);

            LoadData();
            FillIncomeChartx = new RelayCommand(ExecuteFillIncomeChart);

        }


        public void ExecuteLoadTrendCommand(object parameter)
        {
            if (SelectedDish == null) return; // Убедиться, что выбрано блюдо

            // Получение данных о популярности из репозитория
            var trends = statisticsRepository.GetDishPopularityTrend(SelectedDish.Id, StartDate, EndDate);

            // Создание новой серии для графика
            var lineSeries = new LineSeries
            {
                Title = SelectedDish.Title,
                Values = new ChartValues<int>(trends.Select(t => t.Count))
            };

            // Обновление данных в графике
            TrendSeriesCollection = new SeriesCollection { lineSeries };

            // Обновление меток по осям
            Labels = new ObservableCollection<string>(trends.Select(t => t.CreatedAt.ToString("yyyy-MM-dd")));
        }

        private void FillIncomeChart(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, double> profitData = statisticsRepository.GetProfitForTimePeriod(startDate, endDate);

            // Clear existing data
            IncomeSeriesCollection= new SeriesCollection();

            // Create new series
            ColumnSeries incomeSeries = new ColumnSeries
            {
                Title = "Доход",
                Values = new ChartValues<double>(profitData.Values) // Use the values from the dictionary
            };

            // Add the series to the collection
            IncomeSeriesCollection.Add(incomeSeries);

            // Use the dates as labels
            Labels = new ObservableCollection<string>(profitData.Keys.Select(date => date.ToString("dd/MM/yyyy")));
        }


        private void ExecuteFillIncomeChart(object parameter)
        {
            FillIncomeChart(StartDate, EndDate);
        }

        private void UpdateIncomeChart(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, double> profitData = statisticsRepository.GetProfitForTimePeriod(startDate, endDate);

            // Проверяем, инициализированы ли свойства
            if (IncomeSeriesCollection == null)
            {
                IncomeSeriesCollection = new SeriesCollection();
            }
            if (Labels == null)
            {
                Labels = new ObservableCollection<string>();
            }

            // Clear existing data

            if (Labels != null)
            {
                Labels.Clear();
            }

            // Create new series
            ColumnSeries incomeSeries = new ColumnSeries
            {
                Title = "Доход",
                Values = new ChartValues<double>(profitData.Values) // Use the values from the dictionary
            };

            // Add the series to the collection
            if (IncomeSeriesCollection != null)
            {
                IncomeSeriesCollection.Add(incomeSeries);
            }

            // Use the dates as labels
            if (Labels != null)
            {
                foreach (var date in profitData.Keys)
                {
                    Labels.Add(date.ToString("dd/MM/yyyy"));
                }
            }
        }

        public void ExecuteLoadPopularDishes(object parameter)
        {
            var popularDishes = statisticsRepository.GetPopularDishes();
            var series = new SeriesCollection();

            foreach (var dish in popularDishes)
            {
                series.Add(new PieSeries
                {
                    Title = dish.Key,
                    Values = new ChartValues<double> { dish.Value }
                });
            }

            PopularDishesSeriesCollection = series;
        }

        public void ExecuteLoadUnpopularDishes(object parameter)
        {
            var unpopularDishes = statisticsRepository.GetUnpopularDishes();
            var series = new SeriesCollection();

            foreach (var dish in unpopularDishes)
            {
                series.Add(new PieSeries
                {
                    Title = dish.Key,
                    Values = new ChartValues<double> { dish.Value }
                });
            }

            UnpopularDishesSeriesCollection = series;
        }


        private void ExecuteUpdateIncomeChart(object parameter)
        {
            UpdateIncomeChart(StartDate, EndDate);
        }



        private void LoadData()
        {
            FillIncomeChart(StartDate, EndDate);

            ExecuteLoadPopularDishes(null);

            // Вызов метода для загрузки данных о непопулярных блюдах
            ExecuteLoadUnpopularDishes(null);

        }


    }
}
