namespace DoorPrize.Client.MVVM.Main
{
    public static class MainRunner
    {
        public static void Run()
        {
            var model = new MainModel();

            var viewModel = new MainViewModel(model);

            var view = new MainWindow();

            view.DataContext = viewModel;

            view.Show();
        }
    }
}
