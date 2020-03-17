using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TelegramEventBot_AdminPanel
{
    /// <summary>
    /// Interaction logic for ChatEntryDialog.xaml
    /// </summary>
    public partial class ChatEntryDialog : Window
    {
        public ChatEntryDialog()
        {
            InitializeComponent();
        }

        public string ResponseName
        {
            get { return ChatGroupName.Text; }
            set { ChatGroupName.Text = value; }
        }

        public long ResponseId
        {
            get {
                    try
                    {
                        return long.Parse(ChatGroupId.Text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return 0;
                    }
                }

            set { ChatGroupId.Text = value.ToString(); }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChatGroupName.Text == "" || ChatGroupId.Text == "")
            {
                DialogResult = false;
            }
            else
            { 
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
