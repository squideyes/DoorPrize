using System;
using System.Windows.Threading;
using PR = DoorPrize.GUI.Properties.Resources;

namespace DoorPrize.GUI
{
    public static class DispatcherExtenders
    {
        public static void Fail(this Dispatcher dispatcher, 
            FailReason reason, Action shutdown)
        {
            const string CAPTION = "Failure";

            string message = null;

            switch (reason)
            {
                case FailReason.EarlyStageInitFailure:
                    message = PR.EarlyStageInitFailure;
                    break;
                case FailReason.FailureDialogInternalError:
                    message = PR.FailureDialogInternalError;
                    break;
            }

            dispatcher.BeginInvoke((Action)(() =>
            {
                Modal.FailureDialog(null, CAPTION, message);

                shutdown();
            }));
        }
    }
}
