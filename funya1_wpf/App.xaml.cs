using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace funya1_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // UI スレッドでの未処理例外をハンドル
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 非 UI スレッドでの未処理例外をハンドル
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Task 内での未処理例外をハンドル
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogException(e.Exception, "DispatcherUnhandledException");
                ShowErrorMessage(e.Exception);

                // 例外が処理されたとマークする（アプリケーションがクラッシュしないようにする）
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // エラーハンドリング中にエラーが発生した場合
                MessageBox.Show($"エラー処理中に問題が発生しました: {ex.Message}",
                    "致命的なエラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                LogException(exception, "UnhandledException");
                ShowErrorMessage(exception);

                // このイベントでは e.Handled のようなプロパティはないため、
                // アプリケーションは終了する可能性がある
            }
            catch (Exception ex)
            {
                // エラーハンドリング中にエラーが発生した場合
                MessageBox.Show($"エラー処理中に問題が発生しました: {ex.Message}",
                    "致命的なエラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                LogException(e.Exception, "UnobservedTaskException");
                ShowErrorMessage(e.Exception);

                // 例外が処理されたとマークする
                e.SetObserved();
            }
            catch (Exception ex)
            {
                // エラーハンドリング中にエラーが発生した場合
                MessageBox.Show($"エラー処理中に問題が発生しました: {ex.Message}",
                    "致命的なエラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowErrorMessage(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("アプリケーションでエラーが発生しました。");
            errorMessage.AppendLine();
            errorMessage.AppendLine($"エラーの種類: {exception.GetType().Name}");
            errorMessage.AppendLine($"メッセージ: {exception.Message}");

            if (exception.InnerException != null)
            {
                errorMessage.AppendLine();
                errorMessage.AppendLine($"内部エラー: {exception.InnerException.Message}");
            }

            errorMessage.AppendLine();
            errorMessage.AppendLine("スタックトレース:");
            errorMessage.AppendLine(exception.StackTrace);

            errorMessage.AppendLine();
            errorMessage.AppendLine("エラーログは");
            errorMessage.AppendLine(GetLogFolder());
            errorMessage.AppendLine("に保存されています。");
            errorMessage.AppendLine("アプリケーションを再起動してください。");

            MessageBox.Show(errorMessage.ToString(), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LogException(Exception exception, string source)
        {
            if (exception == null)
            {
                return;
            }

            try
            {
                string logFolder = GetLogFolder();

                // ログフォルダが存在しない場合は作成
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                string logFile = Path.Combine(logFolder, $"error_{DateTime.Now:yyyyMMdd}.log");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var logMessage = new StringBuilder();
                logMessage.AppendLine($"[{timestamp}] [{source}]");
                logMessage.AppendLine($"Exception Type: {exception.GetType().FullName}");
                logMessage.AppendLine($"Message: {exception.Message}");

                if (exception.InnerException != null)
                {
                    logMessage.AppendLine($"Inner Exception: {exception.InnerException.Message}");
                }

                logMessage.AppendLine("Stack Trace:");
                logMessage.AppendLine(exception.StackTrace);
                logMessage.AppendLine(new string('-', 80));

                // ログファイルに追記
                File.AppendAllText(logFile, logMessage.ToString());
            }
            catch
            {
                // ログ記録中のエラーは無視（ユーザーエクスペリエンスを優先）
            }
        }

        private static string GetLogFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MifuminSoft", "funya", "Logs");
        }
    }
}
