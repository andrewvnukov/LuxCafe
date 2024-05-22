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
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;
using System.Windows.Media;

namespace Cafe_Managment.ViewModel
{
    internal class StatisticVM : ViewModelBase
    {
        double _graphWidth;
        private Notifier _notifier;


        public ICommand ConfirmPeriodUpdatedCommand { get; set; }
        public ICommand FillIncomeChartx { get; set; }
        public ICommand LoadPopularDishes { get; set; }
        public ICommand LoadUnpopularDishes { get; set; }
        public ICommand LoadTrendCommand { get; set; }


        StatisticsRepository statisticsRepository;

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

        private ObservableCollection<string> _trendlabels;
        public ObservableCollection<string> TrendLabels
        {
            get { return _trendlabels; }
            set
            {
                _trendlabels = value;
                OnPropertyChanged(nameof(TrendLabels));
            }
        }

        private DishData selectedDish;

        public DishData SelectedDish
        {
            get { return selectedDish; }
            set
            {
                selectedDish = value;
                OnPropertyChanged(nameof(SelectedDish));
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

        private ObservableCollection<DishData> allDishes;

        public ObservableCollection<DishData> AllDishes
        {
            get { return allDishes; }
            set
            {
                allDishes = value;
                OnPropertyChanged(nameof(AllDishes));
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

        int _selectedIndexInTrend;
        public int SelectedIndexInTrend
        {
            get { return _selectedIndexInTrend; }
            set { _selectedIndexInTrend = value;
            OnPropertyChanged(nameof(SelectedIndexInTrend));}
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
            _notifier = CreateNotifier();

            // Инициализация репозитория
            statisticsRepository = new StatisticsRepository();  // Убедитесь, что аргументы передаются корректно, например, строка подключения

            // Устанавливаем конечную дату в текущую дату
            EndDate = DateTime.Today;

            // Устанавливаем начальную дату как конечную дату минус 30 дней
            StartDate = EndDate.AddDays(-30);

            // Инициализация коллекций
            PopularDishesSeriesCollection = new SeriesCollection();
            UnpopularDishesSeriesCollection = new SeriesCollection();

            // Инициализация ObservableCollection для AllDishes
            var dishes = statisticsRepository.GetAllDishes();  // Здесь statisticsRepository уже инициализирован
            IEnumerable<DishData> dish = dishes as IEnumerable<DishData>;
            dish = dish.Append(new DishData { Title = "Выберите блюдо" });
            AllDishes = new ObservableCollection<DishData>(dish);
            SelectedIndexInTrend = AllDishes.Count-1;


            // Инициализация команд
            LoadTrendCommand = new RelayCommand(ExecuteLoadTrendCommand, CanExecuteLoadTrendCommand);
            LoadPopularDishes = new RelayCommand(ExecuteLoadPopularDishes);
            LoadUnpopularDishes = new RelayCommand(ExecuteLoadUnpopularDishes);
            FillIncomeChartx = new RelayCommand(ExecuteFillIncomeChart);
            ConfirmPeriodUpdatedCommand = new RelayCommand(ExecuteConfirmPeriodUpdatedCommand);

            // Инициализация дополнительных данных
            IncomeSeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            TrendLabels = new ObservableCollection<string>();

            LoadData();
        }
        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        private void ExecuteConfirmPeriodUpdatedCommand(object obj)
        {
            _notifier.ShowSuccess($"Период успешно обновлен!");
        }

        private bool CanExecuteLoadTrendCommand(object arg)
        {
            return !(SelectedDish == null || StartDate == null || EndDate == null|| SelectedIndexInTrend == AllDishes.Count-1);
        }

        public List<DishData> GetAllDishes()
        {
            List<DishData> dishes = new List<DishData>();
            // Заполнение данными
            return dishes;
        }

        public void ExecuteLoadTrendCommand(object parameter)
        {
            DateTime today = DateTime.Today;

            if (StartDate > today)
            {
                _notifier.ShowWarning("Начальная дата не может быть позже сегодняшнего дня. Выберите корректный период.");
                return;
            }
            if (EndDate < StartDate)
            {
                _notifier.ShowWarning("Конечная дата не может быть раньше начальной даты. Выберите корректный период.");
                return;
            }

            // Получение данных о популярности из репозитория
            var trends = statisticsRepository.GetDishPopularityTrend(SelectedDish.Id, StartDate, EndDate);

            // Создание новой серии для графика
            var lineSeries = new LineSeries
            {
                Title = SelectedDish.Title,
                Values = new ChartValues<int>(trends.Select(t => t.Count)),
                
            };

            // Обновление данных в графике
            TrendSeriesCollection = new SeriesCollection { lineSeries };

            // Обновление меток по осям
            TrendLabels = new ObservableCollection<string>(trends.Select(t => t.CreatedAt.ToString("dd/MM/yyyy")));
        }

        private void FillIncomeChart(DateTime startDate, DateTime endDate)
        {          
            Dictionary<DateTime, double> profitData = statisticsRepository.GetProfitForTimePeriod(startDate, endDate);

            // Очистить существующие данные
            IncomeSeriesCollection = new SeriesCollection();

            // Создать новый набор данных
            ColumnSeries incomeSeries = new ColumnSeries
            {
                Title = "Доход",
                Values = new ChartValues<double>(profitData.Values) // Используем значения из словаря
            };

            // Добавляем новый набор данных
            IncomeSeriesCollection.Add(incomeSeries);

            // Используем даты как метки
            Labels = new ObservableCollection<string>(profitData.Keys.Select(date => date.ToString("dd/MM/yyyy")));
        }



        private void ExecuteFillIncomeChart(object parameter)
        {
            DateTime today = DateTime.Today; 

            if (StartDate > today)
            {
                _notifier.ShowWarning("Начальная дата не может быть позже сегодняшнего дня. Выберите корректный период.");
                return; 
            }
            if (EndDate < StartDate)
            {
                _notifier.ShowWarning("Конечная дата не может быть раньше начальной даты. Выберите корректный период.");
                return; 
            }

            FillIncomeChart(StartDate, EndDate);
            ExecuteConfirmPeriodUpdatedCommand(null);
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


        

        private void LoadData()
        {
            FillIncomeChart(StartDate, EndDate);

            ExecuteLoadPopularDishes(null);
            // Вызов метода для загрузки данных о непопулярных блюдах
            ExecuteLoadUnpopularDishes(null);

        }


    }
}
