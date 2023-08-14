using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Material.Styles.Assists;

namespace Common.Resources.m3
{
    public class Card : ContentControl, ICommandSource
    {
        private bool _commandCanExecute = false;


        static Card()
        {
            CommandProperty.Changed.Subscribe(CommandChanged);
            CommandParameterProperty.Changed.Subscribe(CommandParameterChanged);
            ClickModeProperty.OverrideDefaultValue<Card>(ClickMode.Release);
        }

        static void CommandChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is Card c)
            {
                if (((ILogical) c).IsAttachedToLogicalTree)
                {
                    if (e.OldValue is ICommand oldCommand)
                    {
                        oldCommand.CanExecuteChanged -= c.CanExecuteChanged;
                    }

                    if (e.NewValue is ICommand newCommand)
                    {
                        newCommand.CanExecuteChanged += c.CanExecuteChanged;
                    }
                }

                c.CanExecuteChanged(c, EventArgs.Empty);
            }
        }

        static void CommandParameterChanged(AvaloniaPropertyChangedEventArgs<object> e)
        {
            if (e.Sender is Card card)
            {
                card.CanExecuteChanged(card, EventArgs.Empty);
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
                ClickMode == ClickMode.Press)
            {
                OnClick();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.InitialPressMouseButton == MouseButton.Left &&
                ClickMode == ClickMode.Release &&
                this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
            {
                OnClick();
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            bool canExecute = Command == null || Command.CanExecute(CommandParameter);

            if (canExecute != _commandCanExecute)
            {
                _commandCanExecute = canExecute;
            }
        }

        protected virtual void OnClick()
        {
            if (Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }

        protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
        {
            base.UpdateDataValidation(property, value);
            if (property != CommandProperty)
                return;

            if (value.Type != BindingValueType.BindingError)
                return;

            if (_commandCanExecute)
            {
                _commandCanExecute = false;
            }
        }

        void ICommandSource.CanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged(sender, e);

        public static readonly StyledProperty<object> ImageProperty =
            AvaloniaProperty.Register<Card, object>(nameof(Image));

        /// <summary>
        /// Image
        /// </summary>
        public object Image
        {
            get => GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly StyledProperty<object> HeadlineProperty =
            AvaloniaProperty.Register<Card, object>(nameof(Headline));

        /// <summary>
        /// Headline
        /// </summary>
        public object Headline
        {
            get => GetValue(HeadlineProperty);
            set => SetValue(HeadlineProperty, value);
        }

        public static readonly StyledProperty<object> SubheadProperty =
            AvaloniaProperty.Register<Card, object>(nameof(Subhead));

        /// <summary>
        /// Subhead
        /// </summary>
        public object Subhead
        {
            get => GetValue(SubheadProperty);
            set => SetValue(SubheadProperty, value);
        }

        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<Card, CornerRadius>(nameof(CornerRadius), new CornerRadius(12));

        /// <summary>
        /// Gets or sets the radius of the border rounded corners.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly StyledProperty<ShadowDepth> ShadowDepthProperty =
            AvaloniaProperty.Register<Card, ShadowDepth>(nameof(CornerRadius), ShadowDepth.Depth1);

        /// <summary>
        /// Gets or sets the radius of the border rounded corners.
        /// </summary>
        public ShadowDepth ShadowDepth
        {
            get => GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }

        public static readonly StyledProperty<bool> ScaleOnPointerOverProperty =
            AvaloniaProperty.Register<Card, bool>(nameof(ScaleOnPointerOver));

        public bool ScaleOnPointerOver
        {
            get => GetValue(ScaleOnPointerOverProperty);
            set => SetValue(ScaleOnPointerOverProperty, value);
        }

        public static readonly StyledProperty<bool> InsideClippingProperty =
            AvaloniaProperty.Register<Card, bool>(nameof(InsideClipping), true);

        /// <summary>
        /// Get or set the inside border clipping.
        /// </summary>
        public bool InsideClipping
        {
            get => GetValue(InsideClippingProperty);
            set => SetValue(InsideClippingProperty, value);
        }

        public static readonly StyledProperty<bool> OutlinedImageProperty =
            AvaloniaProperty.Register<Card, bool>(nameof(InsideClipping), false);

        /// <summary>
        /// Get or set the outlined image.
        /// </summary>
        public bool OutlinedImage
        {
            get => GetValue(OutlinedImageProperty);
            set => SetValue(OutlinedImageProperty, value);
        }

        private ICommand _command;

        public ICommand Command
        {
            get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);
        }

        public static readonly DirectProperty<Card, ICommand> CommandProperty =
            Button.CommandProperty.AddOwner<Card>(
                o => o.Command,
                (o, v) => o.Command = v,
                enableDataValidation: true);

        public static readonly StyledProperty<object> CommandParameterProperty =
            Button.CommandParameterProperty.AddOwner<Card>();

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly StyledProperty<ClickMode> ClickModeProperty =
            Button.ClickModeProperty.AddOwner<Card>();

        public ClickMode ClickMode
        {
            get => GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }
    }
}