using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Liddup.Controls
{
    public class SegmentedTabControl : StackLayout
    {
        public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create(nameof(SelectedSegment), typeof(int), typeof(SegmentedTabControl), 0, BindingMode.TwoWay, propertyChanged: SelectedSegmentChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SegmentedTabControl), null, propertyChanged: (bo, o, n) => ((SegmentedTabControl)bo).OnCommandChanged());
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SegmentedTabControl), null, propertyChanged: (bindable, oldvalue, newvalue) => ((SegmentedTabControl)bindable).CommandCanExecuteChanged(bindable, EventArgs.Empty));

        private static void TintColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SegmentedTabControl)bindable;
            control.BackgroundColor = (Color)newValue;
        }


        public int SelectedSegment
        {
            get { return (int)GetValue(SelectedSegmentProperty); }
            set { SetValue(SelectedSegmentProperty, value); }
        }

        private static void SelectedSegmentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SegmentedTabControl)bindable;
            var selection = (int)newValue;

            control.UpdateSelectedItem(selection);
        }
        
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (propertyName == CommandProperty.PropertyName)
            {
                ICommand cmd = Command;
                if (cmd != null)
                    cmd.CanExecuteChanged -= CommandCanExecuteChanged;
            }
            base.OnPropertyChanging(propertyName);
        }

        void CommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ICommand cmd = Command;
            if (cmd != null)
                IsVisible = cmd.CanExecute(CommandParameter);
        }

        void OnCommandChanged()
        {
            if (Command != null)
            {
                Command.CanExecuteChanged += CommandCanExecuteChanged;
                CommandCanExecuteChanged(this, EventArgs.Empty);
            }
            else
                IsVisible = true;
        }

        public event EventHandler<int> ItemTapped = (e, a) => { };


        private void UpdateSelectedItem(int index)
        {
            for (var i = 0; i < Children.Count(); i++)
                if (Children[i] is MusicProviderTab tab)
                    tab.IsActive = false;

            if (index < Children.Count() && index >= 0)
            {
                if (Children[index] is MusicProviderTab selectedTab)
                    selectedTab.IsActive = true;

                ItemTapped(this, index);
                CommandParameter = index;
                Command?.Execute(CommandParameter);
            }
            else
                SelectedSegment = -1;
        }

        private void ItemTappedCommand(int index)
        {
            SelectedSegment = index;
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            var currentCount = Children.Count() - 1;

            if (child is MusicProviderTab tab && currentCount >= 0)
            {
                tab.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command<int>(ItemTappedCommand),
                    CommandParameter = currentCount
                });

                if (currentCount == SelectedSegment)
                    tab.IsActive = true;
            }
        }

        public SegmentedTabControl()
        {
            
        }
    }
}

