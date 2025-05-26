using System.Windows.Forms;

namespace Files
{
    public class FileDialog
    {
        public static string OpenJsonFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Выберите JSON файл"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string SaveJsonFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Сохранить файл JSON",
                DefaultExt = "json"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }
    }
}