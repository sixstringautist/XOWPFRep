using XOWPF.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace XOWPF.VM
{
    class MainVM : INotifyPropertyChanged
    {
        readonly XOModel model;
        public ObservableCollection<string> Diff { get; private set; }
        private string selectetdiff;
        public string SelectedDiff { get => selectetdiff; set { selectetdiff = value; OnPropertyChanged(); } }
        public bool OnAI { get; set; }
        public int Points1
        {
            get => model.XPlayerPoints;
           
        }
        public int Points2
        {
            get => model.OPlayerPoints;
            
        }


        private Command startGame;
        public Command StartGame
        {
            get
            {
                return startGame ??
                    (
                    new Command(obj => { model.OnAI = (bool)OnAI;
                        model.SetAiDiff = selectetdiff;
                        model.ResetGame();
                        if (model.CurrentPlayer.IsAI)
                            model.CurrentMove();
                        OnPropertyChanged("Field");
                        OnPropertyChanged("State");})
                    );
            }
        }
        private Command currentMove;
        public Command CurrentMove
        {
            get
            {
                return currentMove ??
                    (
                    new Command(obj =>
                    {
                        string C = (string)obj;
                        Point p = new Point(int.Parse(C[0].ToString()), int.Parse(C[2].ToString()));
                        model.CurrentMove(p);
                        OnPropertyChanged("Points1");
                        OnPropertyChanged("Points2"); 
                        OnPropertyChanged("Field");
                        OnPropertyChanged("State");
                        if (model.CurrentPlayer.IsAI)
                        {
                            model.CurrentMove();
                            OnPropertyChanged("Points1");
                            OnPropertyChanged("Points2");
                            OnPropertyChanged("Field");
                            OnPropertyChanged("State");
                        }
                        
                    })
                    );

            }
        }
        private Command chooseType;
        public Command ChooseType
        {
            get
            {
                return chooseType ??
                  (
                    new Command
                    (obj =>
                    {
                        string Type = (string)obj;
                        if (Type is "X")
                        {
                            model.ChangePlayerTypes(PlayerType.x);
                        }
                        else
                        {
                            model.ChangePlayerTypes(PlayerType.o);
                        }
                        model.ResetGame();
                        if (model.CurrentPlayer.IsAI)
                            model.CurrentMove();
                        OnPropertyChanged("Field");
                        OnPropertyChanged("State");
                    })
                  );
            }
        }
        public string State
        {
            get
            {
                if (model.GameState)
                    return "Ход игрока " + model.CurrentPlayer.Type.ToString().ToUpper();
                else return "Игра завершена";
            }
        }
        public MainVM()
        {
            model = new XOModel();
            Diff = new ObservableCollection<string>() {"Easy","Normal","Hard"};
            OnAI = false;
        }
    
        public XOField Field
        {
            get
            {
                return model.Field;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
