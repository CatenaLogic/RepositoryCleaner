// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasViewbox.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Markup
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class CanvasViewbox : Catel.Windows.Markup.UpdatableMarkupExtension
    {
        #region Constructors
        public CanvasViewbox()
        {
            Foreground = Brushes.Transparent;
        }

        public CanvasViewbox(string pathName)
            : this()
        {
            PathName = pathName;
        }
        #endregion

        #region Properties
        public SolidColorBrush Foreground { get; set; }

        [ConstructorArgument("pathName")]
        public string PathName { get; set; }
        #endregion

        #region Methods
        protected override object ProvideDynamicValue(IServiceProvider serviceProvider)
        {
            return GetImageSource();
        }

        private Viewbox GetImageSource()
        {
            var viewbox = new Viewbox();

            viewbox.Stretch = Stretch.Uniform;

            var canvas = System.Windows.Application.Current.FindResource(PathName) as Canvas;
            if (canvas is not null && Foreground != Brushes.Transparent)
            {
                foreach (var child in canvas.Children)
                {
                    var path = child as Path;
                    if (path is not null)
                    {
                        path.SetCurrentValue(Shape.FillProperty, Foreground);
                    }
                }
            }
            viewbox.Child = canvas;

            return viewbox;
        }
        #endregion
    }
}
