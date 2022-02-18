﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

// From: https://stackoverflow.com/questions/33394108/textblock-inline-binding-using-dependencyproperty
namespace CSFileCabinetRenamer
{
    public class BindableTextBlock : TextBlock
    {
        public ICollection<Inline> InlineList
        {
            get { return (ICollection<Inline>)GetValue(InlineListProperty); }
            set { SetValue(InlineListProperty, value); }
        }

        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.Register("InlineList", typeof(List<Inline>), typeof(BindableTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var textBlock = (BindableTextBlock)sender;

            textBlock.Inlines.AddRange((ICollection<Inline>)e.NewValue);
        }
    }
}