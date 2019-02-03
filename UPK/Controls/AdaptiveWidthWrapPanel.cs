using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UPK.Controls
{
    class AdaptiveWidthWrapPanel : WrapPanel
    {
        public static DependencyProperty MinElementWidthProperty = DependencyProperty.Register("MinElementWidth", typeof(double), typeof(AdaptiveWidthWrapPanel), new PropertyMetadata(double.NaN));

        public double MinElementWidth
        {
            get => (double)GetValue(MinElementWidthProperty);
            set => SetValue(MinElementWidthProperty, value);
        }
        public AdaptiveWidthWrapPanel() : base()
        {
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count > 0 && Orientation == Orientation.Horizontal) {
                double elementWidth = MinElementWidth;
                //определяем макс. количество элементов, которые могут быть отображены
                int maxCount = (int)(availableSize.Width / MinElementWidth);
                if (maxCount >= 1) {
                    //определяем новую ширину элемента для размещения без пустот
                    elementWidth = availableSize.Width / maxCount;
                }
                foreach (UIElement el in Children) {
                    (el as FrameworkElement).Width = elementWidth;
                    el.Measure(availableSize);
                }
            }
            return base.MeasureOverride(availableSize);
        }
    }
}
