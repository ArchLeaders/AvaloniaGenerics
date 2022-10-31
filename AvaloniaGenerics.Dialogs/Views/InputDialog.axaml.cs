using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AvaloniaGenerics.Dialogs
{
    public partial class InputDialog : Window
    {
        private Dictionary<string, string> DataRoot { get; set; } = new();

        public InputDialog() => AvaloniaXamlLoader.Load(this);
        public InputDialog(Dictionary<string, string> root, string title)
        {
            AvaloniaXamlLoader.Load(this);
            DataRoot = root;

            StackPanel uiRoot = this.FindControl<StackPanel>("Root")!;
            this.FindControl<TextBlock>("TitleBox")!.Text = title;

            foreach ((var key, var value) in DataRoot) {

                var tb = new TextBox() {
                    Watermark = key,
                    Text = value,
                    Margin = new Thickness(0, 0, 0, 15),
                    UseFloatingWatermark = true
                };

                tb.GetObservable(TextBlock.TextProperty).Subscribe(x => DataRoot[key] = x!);
                uiRoot.Children.Add(tb);
            }
        }

        public static void ShowSync(Dictionary<string, string> root, string title = "Input Dialog")
        {
            using var source = new CancellationTokenSource();
            Show(root, title).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        }

        public static async Task<Dictionary<string, string>?> Show(Dictionary<string, string> root, string title = "Input Dialog")
        {
            InputDialog dialog = new(root, title);
            var res = MessageBoxResult.Ok;

            var buttonPanel = dialog.FindControl<StackPanel>("Buttons")!;
            var close = dialog.FindControl<Button>("Close")!;

            close.Click += (_, __) => {
                res = MessageBoxResult.Cancel;
                dialog.Close();
            };

            void AddBtn(string caption, MessageBoxResult r, bool def = false, int mode = 0)
            {
                var btn = new Button {
                    Content = caption,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0),
                    MinWidth = 67,
                    IsDefault = mode == 1,
                    IsCancel = mode == 2
                };

                btn.Click += (_, __) => {
                    res = r;
                    dialog.Close();
                };

                buttonPanel.Children.Add(btn);
                if (def) res = r;
            }

            AddBtn("Ok", MessageBoxResult.Ok, true, 1);
            AddBtn("Cancel", MessageBoxResult.Cancel, true, 2);

            var tcs = new TaskCompletionSource<MessageBoxResult>();
            dialog.Closed += delegate { tcs.TrySetResult(res); };

            if (View != null) {
                await dialog.ShowDialog(View);
            }
            else {
                dialog.Show();
            }

            if ((await tcs.Task) == MessageBoxResult.Ok) {
                return dialog.DataRoot;
            }

            return null;
        }
    }
}