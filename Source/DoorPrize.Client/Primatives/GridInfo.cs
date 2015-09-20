using DoorPrize.GUI;

namespace DoorPrize.Client.Primatives
{
    public class GridInfo : ModelBase<GridInfo>
    {
        private bool received;

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
    }
}
