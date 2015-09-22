using DoorPrize.GUI;
using System.Windows.Media;

namespace DoorPrize.Client.Primatives
{
    public class GridInfo : ModelBase<GridInfo>
    {
        private bool received;
        private Color background;

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Prize { get; set; }

        public bool Received
        {
            get
            {
                return received;
            }
            set
            {
                received = value;

                NotifyPropertyChanged(m => m.Received);
            }
        }

        public Color Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;

                NotifyPropertyChanged(m => m.Background);
            }
        }
    }
}
