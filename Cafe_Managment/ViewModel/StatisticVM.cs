﻿using Cafe_Managment.Utilities;
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


namespace Cafe_Managment.ViewModel
{
    internal class StatisticVM : ViewModelBase
    {

        public ICommand UpdateIncomeChartx { get; set; }
        public ICommand FillIncomeChartx { get; set; }

        

        StatisticsRepository statisticsRepository;
        public new event PropertyChangedEventHandler PropertyChanged;

        private SeriesCollection _incomeSeriesCollection;
        public SeriesCollection IncomeSeriesCollection
        {
            get { return _incomeSeriesCollection; }
            set
            {
                _incomeSeriesCollection = value;
                OnPropertyChanged(nameof(IncomeSeriesCollection));
            }
        }

        private ObservableCollection<string> _labels;
        public ObservableCollection<string> Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
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

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                //LoadData(); // Перезагружаем данные при изменении начальной даты
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

            IncomeSeriesCollection = new SeriesCollection();

            statisticsRepository = new StatisticsRepository();
            LoadData();
            UpdateIncomeChartx = new RelayCommand(ExecuteUpdateIncomeChart);
            FillIncomeChartx = new RelayCommand(ExecuteFillIncomeChart);

        }

        private void FillIncomeChart(DateTime startDate, DateTime endDate)
        {
            double income = statisticsRepository.GetProfitForTimePeriod(startDate, endDate);
            ColumnSeries incomeSeries = new ColumnSeries
            {
                Title = "Доход",
                Values = new ChartValues<double> { income }
            };
            IncomeSeriesCollection.Clear();
            IncomeSeriesCollection.Add(incomeSeries);

            // Создаем новый экземпляр коллекции Labels, если он еще не был создан
            if (Labels == null)
            {
                Labels = new ObservableCollection<string>();
            }

            // Формируем строку с обеими датами, разделёнными символом "-"
            string selectedPeriod = $"C {startDate.ToString("dd/MM/yyyy")} По {endDate.ToString("dd/MM/yyyy")}";

            // Очищаем коллекцию Labels и добавляем в неё сформированную строку
            Labels.Clear();
            Labels.Add(selectedPeriod);
        }

        private void ExecuteFillIncomeChart(object parameter)
        {
            FillIncomeChart(StartDate, EndDate);
        }

        private void UpdateIncomeChart(DateTime startDate, DateTime endDate)
        {
            double income = statisticsRepository.GetProfitForTimePeriod(startDate, endDate);
            ColumnSeries incomeSeries = new ColumnSeries
            {
                Title = "Доход",
                Values = new ChartValues<double> { income }
            };
            IncomeSeriesCollection.Clear();
            IncomeSeriesCollection.Add(incomeSeries);

            if (Labels == null)
            {
                Labels = new ObservableCollection<string>();
            }

            // Формируем строку с обеими датами, разделёнными символом "-"
            string selectedPeriod = $"C {startDate.ToString("dd/MM/yyyy")} По {endDate.ToString("dd/MM/yyyy")}";

            // Очищаем коллекцию Labels и добавляем в неё сформированную строку
            Labels.Clear();
            Labels.Add(selectedPeriod);

        }

        private void ExecuteUpdateIncomeChart(object parameter)
        {
            UpdateIncomeChart(StartDate, EndDate);
        }



        private void LoadData()
        {
            // Подготовка данных для диаграммы дохода


            FillIncomeChart(StartDate, EndDate);
            // Подготовка данных для диаграммы популярных блюд
            PopularDishesSeriesCollection = new SeriesCollection();
            PopularDishesSeriesCollection.Add(new PieSeries
            {
                Title = "Популярные блюда",
                Values = new ChartValues<double> { 10, 20, 30 }, // Пример данных
                DataLabels = true, // Отображать метки данных
                LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Формат меток данных
            });

            // Подготовка данных для диаграммы непопулярных блюд
            UnpopularDishesSeriesCollection = new SeriesCollection();
            UnpopularDishesSeriesCollection.Add(new PieSeries
            {
                Title = "Непопулярные блюда",
                Values = new ChartValues<double> { 5, 8, 12 }, // Пример данных
                DataLabels = true, // Отображать метки данных
                LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Формат меток данных
            });
        }

        protected new void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}