﻿using Foundation;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using UIKit;

// ReSharper disable UnusedVariable
#pragma warning disable S1481 // Unused local variable
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
namespace MoneyFox.iOS
{
    // This class is never actually executed, but when Xamarin linking is enabled it does ensure types and properties
    // are preserved in the deployed app
    public class LinkerPleaseInclude
    {
        public void Include(UIButton uiButton) => uiButton.TouchUpInside += (s, e) => uiButton.SetTitle(uiButton.Title(UIControlState.Normal), UIControlState.Normal);

        public void Include(UIBarButtonItem barButton) => barButton.Clicked += (s, e) => barButton.Title = $"{barButton.Title}";

        public void Include(UITextField textField)
        {
            textField.Text = $"{textField.Text}";
            textField.EditingChanged += (sender, args) =>
                                        {
                                            textField.Text = string.Empty;
                                        };
        }

        public void Include(UITextView textView)
        {
            textView.Text = $"{textView.Text}";
            textView.Changed += (sender, args) =>
                                {
                                    textView.Text = string.Empty;
                                };
        }

        public void Include(UILabel label)
        {
            label.Text = $"{label.Text}";
            label.AttributedText = new NSAttributedString($"{label.AttributedText}");
        }

        public void Include(UIImageView imageView) => imageView.Image = new UIImage(imageView.Image?.CGImage);

        public void Include(UIDatePicker date)
        {
            date.Date = date.Date.AddSeconds(1);
            date.ValueChanged += (sender, args) =>
                                 {
                                     date.Date = NSDate.DistantFuture;
                                 };
        }

        public void Include(UISlider slider)
        {
            slider.Value += 1;
            slider.ValueChanged += (sender, args) =>
                                   {
                                       slider.Value = 1;
                                   };
        }

        public void Include(UIProgressView progress) => progress.Progress += 1;

        public void Include(UISwitch sw)
        {
            sw.On = !sw.On;
            sw.ValueChanged += (sender, args) =>
                               {
                                   sw.On = false;
                               };
        }

        public void Include(UIStepper s)
        {
            s.Value += 1;
            s.ValueChanged += (sender, args) =>
                              {
                                  s.Value = 0;
                              };
        }

        public void Include(UIPageControl s)
        {
            s.Pages += 1;
            s.ValueChanged += (sender, args) =>
                              {
                                  s.Pages = 0;
                              };
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) =>
                                         {
                                             string test = $"{e.Action}{e.NewItems}{e.NewStartingIndex}{e.OldItems}{e.OldStartingIndex}";
                                         };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) =>
                                         {
                                             if(command.CanExecute(null))
                                             {
                                                 command.Execute(null);
                                             }
                                         };
        }

        public void Include(INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) =>
                                       {
                                           string test = e.PropertyName;
                                       };
        }
    }
}
