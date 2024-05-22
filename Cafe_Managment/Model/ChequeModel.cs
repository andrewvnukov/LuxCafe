using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Model
{
    public class ChequeModel : ViewModelBase
    {

        public int Id { get; set; }
        public List<DishData> dishes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public float TotalPrice { get; set; }
        public int SpotNumber { get; set; }
        public int GuestNumber { get; set; }

        private TimeSpan _waitingTime;
        public TimeSpan WaitingTime
        {
            get => _waitingTime;
            set
            {
                if (_waitingTime != value)
                {
                    _waitingTime = value;
                    OnPropertyChanged(nameof(WaitingTime));
                    OnPropertyChanged(nameof(FormattedWaitingTime));
                    OnPropertyChanged(nameof(BorderBrushColor));
                    OnPropertyChanged(nameof(TimerForegroundColor));
                }
            }
        }

        public string FormattedWaitingTime => $"{WaitingTime.Minutes:D2}:{WaitingTime.Seconds:D2}";

        public string BorderBrushColor => WaitingTime.TotalMinutes < 10 ? "Red" : "Transparent";

        public string TimerForegroundColor => WaitingTime.TotalMinutes < 10 ? "Red" : "White";

    }
}
