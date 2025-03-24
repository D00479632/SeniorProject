using System;
using System.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace InGameAssistant
{
    public partial class TextInputViewModel : INotifyPropertyChanged
    {
        private readonly Random random = new();
        [Notify] private string text = "";
        [Notify] private string randomNumberText = "Click the button to generate a random number";

        public void GenerateRandom()
        {
            int randomNumber = random.Next(1, 101);
            RandomNumberText = $"Your random number is: {randomNumber}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}