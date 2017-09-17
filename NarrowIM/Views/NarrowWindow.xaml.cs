using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EnvDTE80;
using NarrowIM.Common;

namespace NarrowIM.Views
{
    /// <summary>
    /// NarrowWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class NarrowWindow : System.Windows.Window
    {
        /// <summary></summary>
        private DTE2 _dte;
        /// <summary></summary>
        private CollectorBase   _collector;
        /// <summary></summary>
        private IEnumerable<Candidate> _initialCandidates;
        /// <summary>
        /// </summary>
        public NarrowWindow(DTE2 dte, CollectorBase collector)
        {
            InitializeComponent();
            _dte       = dte;
            _collector = collector;
            Initialize(collector); 
        }
        /// <summary>
        /// </summary>
        private void Initialize(CollectorBase collector)
        {
            SearchText.Text = string.Empty;
            // for performance ?
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchText.Focus();
                _initialCandidates = collector.Collect();
                Candidates.ItemsSource   = _initialCandidates;
                Candidates.SelectedIndex = 0;

            }), DispatcherPriority.Normal, null);
        }
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control))
            {
                if (Candidates.Items.Count == 0)
                {
                    return;
                }
                // selected item
                Candidate candidate = Candidates.SelectedItem as Candidate;
                // call back
                if (candidate.Invoke())
                {
                    Close();
                }

                return;
            }
            if (e.Key == Key.Escape || (e.Key == Key.J && Keyboard.Modifiers == ModifierKeys.Control))
            {
                Close();
                return;
            }
            if (IsKeyDown(e))
            {
                Candidates.SelectedIndex += 1 ;
                return;
            }
            if (IsKeyUp(e))
            {
                if (Candidates.SelectedIndex != 0)
                {
                    Candidates.SelectedIndex -= 1;
                }
                return;
            }
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SearchText.Text = string.Empty;
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool IsKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control) || e.Key == Key.Down)
            {
                return true;
            }
            if ((e.Key == Key.OemPeriod && Keyboard.Modifiers == ModifierKeys.Control) || e.Key == Key.Down)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool IsKeyUp(KeyEventArgs e)
        {
            if ((e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control) || e.Key == Key.Up)
            {
                return true;
            }
            if ((e.Key == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Control) || e.Key == Key.Up)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChange change = e.Changes.FirstOrDefault();
            // reset candidates
            if (change != null && change.RemovedLength > 0)
            {
                Candidates.ItemsSource = _initialCandidates;
            }
            // narrow candidates process
            string text = SearchText.Text.ToLower();
            // for performance
            //if (text.Length < 2)
            //{
            //    return;
            //}
            // narrow
            List<Candidate> candidates = new List<Candidate>();
            // check source one by one
            foreach(Candidate candidate in Candidates.ItemsSource)
            {
                if (candidate.Word.ToLower().Contains(text))
                {
                    candidates.Add(candidate);
                }
            }
            // reset candidates
            Candidates.ItemsSource   = candidates;
            Candidates.SelectedIndex = 0;
        }
    }
}
